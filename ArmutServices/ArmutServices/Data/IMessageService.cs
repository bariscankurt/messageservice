using ArmutServices.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Data
{
    public interface IMessageService
    {
        Task<IActionResult> Register(RegisterModel model);
        Task<IActionResult> Login(LoginModel model);
        Task<IActionResult> SendMessage(MessageSendingModel model);
        Task<IActionResult> UnblockUser(string usernameForUnblock);
        Task<IActionResult> BlockUser(string usernameForBlock);
        Task<IActionResult> PastMessages(string username);
    }
}
