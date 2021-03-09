using ArmutServices.Data;
using ArmutServices.DTOs;
using ArmutServices.Models;
using ArmutServices.Supporters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArmutServices.Controllers
{
    
    [Route("api/armutmessage")]
    [ApiController]
    public class ArmutMessageServiceController : ControllerBase, IMessageService
    {
        private readonly IMessageRepo _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly BlockingLogic _blockingLogic;
        private readonly IMapper _mapper;

        public ArmutMessageServiceController(IMessageRepo repository,UserManager<ApplicationUser> userManager, IConfiguration configuration, BlockingLogic blockingLogic, IMapper mapper)
        {
            _repository = repository;
            _userManager = userManager;
            _configuration = configuration;
            _blockingLogic = blockingLogic;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            //i am looking to username in the model to check if there is any user with that name
            //and i am checking incoming password is matching or not
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user!=null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(15),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                    );
                Log.Information($"Successfull login action for username: {user.UserName}");
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            Log.Information($"Invalid login action for username: {model.UserName}");
            return Unauthorized();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if(userExist != null)
            {
                Log.Information($"Failed register action for user: {userExist.UserName} (user already exist)");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "User already exist!" });
            }
            ApplicationUser user = new ApplicationUser()
            {
                
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                Log.Information($"Failed register action for user: {userExist} (user couldn't created on database)");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Couldn't create a member!" });
            }
            Log.Information($"Successfull register action {userExist} user created on database");
            return Ok(new Response { Status = "Success", Message = "User Created Successfully" });
        }

        [Authorize]
        [HttpPost]
        [Route("blockuser/{usernameForBlock}")] // api/armutmessage/blockuser/{username}
        public async Task<IActionResult> BlockUser(string usernameForBlock)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var userExist = await _userManager.FindByNameAsync(usernameForBlock);
            if (userExist == null)
            {
                Log.Information($"Failed blockuser action for user {userId} because user: {usernameForBlock} doesn't exist");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "The user which you want to block doesn't exist!" });
            }
            if (_blockingLogic.BlockUser(userId, usernameForBlock) && userId != usernameForBlock)
            {
                Log.Information($"Successfull blockuser action, user: {userId} blocked user: {usernameForBlock}");
                return Ok( new Response { Status="Success",Message="User blocked successfully!"});
            }
            Log.Information($"Failed blockuser action, for user: {userId}");
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "User is already blocked or there is an issue!" });
            //possible 3 result are: success, fail (user already blocked), fail (saving to database problems)
        }

        [Authorize]
        [HttpPost]
        [Route("unblockuser/{usernameForUnblock}")] // api/armutmessage/unblockuser/{username}
        public async Task<IActionResult> UnblockUser(string usernameForUnblock)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var userExist = await _userManager.FindByNameAsync(usernameForUnblock);
            if (userExist == null)
            {
                Log.Information($"Failed unblockuser action for user {userId} because user: {usernameForUnblock} doesn't exist");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "The user which you want to unblock doesn't exist!" });
            }
            if (_blockingLogic.UnblockUser(userId, usernameForUnblock))
            {
                Log.Information($"Successfull unblockuser action, user: {userId} unblocked user: {usernameForUnblock}");
                return Ok(new Response { Status = "Success", Message = "User unblocked successfully!" });
            }
            Log.Information($"Failed unblockuser action, user: {userId} couldn't block user: {usernameForUnblock}");
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "User is not on the blockeduser list or there is a problem!" });
        }

        [Authorize]
        [HttpPost]
        [Route("sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageSendingModel model)
        {
            //This is a simple action for sending message.
            //It first checks blocking state between sender and receiver.
            //If there is a block between them, i don't send the message and just return status code 500.
            
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var userExist = await _userManager.FindByNameAsync(model.receivedBy);
            if(userExist == null)
            {
                Log.Information($"Failed sendmessage action for user {userId} because user: {model.receivedBy} doesn't exist");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Message couldn't send!" });
            }
            if (_blockingLogic.IsUserBlocked(userId, model.receivedBy) || userId.ToLower() == model.receivedBy.ToLower())
            {
                Log.Information("Failed sendmessage action {@userId} blocking detected",userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Message couldn't send!" });
            }

            var msg = new Message()
            {
                //I'm doing this because some fields shouldn't be defined by the consumer.
                //for example "sendBy" property. If i let it defined by sender, it will be a huge security problem
                sentBy = userId,
                receivedBy = model.receivedBy,
                text = model.text,
                CreatedDate = DateTime.Now
                //I'm calculating datetime on here because i don't want to add more process load to database server.
                //It's already enough busy with saving messages, users and logs and my pc is so slow :)
            };

            var result = _repository.CreateMessage(msg);
            if(result == true)
            {
                Log.Information($"Successfull sendmessage action from user: {userId} to user: {model.receivedBy}");
                return Ok(new Response { Status = "Success", Message = "Message sent successfully" });
            }
            else
            {
                Log.Information($"Failed sendmessage action for user: {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Message couldn't send" });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("pastmessages/{username}")]
        public async Task<IActionResult> PastMessages(string username)
        {
            //When we think of messaging applications even if we blocked someone, we can access past messages.
            //That's why i didn't use block control for this action.
            var userB = await _userManager.FindByNameAsync(username);
            string userAName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userB != null)
            {
                IEnumerable<Message> pastMessages = _repository.PastMessages(userAName, username);
                Log.Information($"Successfull pastmessages action between user: {userAName} and user: {userB} ");
                return Ok( _mapper.Map<IEnumerable<MessageReadDTO>>(pastMessages));
            }
            Log.Information($"Failed pastmessages action between user: {userAName} and user: {username}");
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Couldn't sent the message" });
        }
    }
}
