using ArmutServices.Data;
using ArmutServices.Models;
using ArmutServices.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Supporters
{
    public class BlockingLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly StringListConverter _slconverter;

        public BlockingLogic(ApplicationDbContext context, StringListConverter slconverter)
        {
            //you can ask me that this applicationdbcontext injection is safe or error prone?
            //because you injected it at your controller and know injecting it second time.
            //how do you keep tracking instances?
            //answer is simple AddScoped doing this for me. When i inject applicationdbcontext
            //to the controller, DI is instanciating it for me and keeping the same instance 
            //everywhere in the same request. That means i am not actually instanciating it twice :)
            _context = context;
            _slconverter = slconverter;
        }
        public bool IsUserBlocked(string senderUserName,string receiverUserName)
        {
            //I am checking blocking situation in here
            var senderUser = GetUser(senderUserName);
            var receiverUser = GetUser(receiverUserName);
            List<string> blockedULForSender = _slconverter.StringToList(senderUser.blockedUsers);
            List<string> blockedULForReceiver = _slconverter.StringToList(receiverUser.blockedUsers);
            if (blockedULForReceiver.Contains(senderUserName) || blockedULForSender.Contains(receiverUserName))
            {
                return true;
            }
            return false;
        }
        public bool BlockUser(string senderUserName, string receiverUserName)
        {
            //this method do blocking job. But it's not that simple.
            //first of all i am using relational database (mssql) and i can't save lists.
            //because of that i wrote a converter class with 2 methods in it.
            //and with that 2 method i am able to do blocking actions without any problem.
            var senderUser = GetUser(senderUserName);
            List<string> blockedULForSender = _slconverter.StringToList(senderUser.blockedUsers);
            if (blockedULForSender.Contains(receiverUserName))
            {
                return false;
            }
            blockedULForSender.Add(receiverUserName);
            string blockedUsers = _slconverter.ListToString(blockedULForSender);
            senderUser.blockedUsers = blockedUsers;
            int affects = _context.SaveChanges();
            if (affects > 0)
            {
                return true;
            }
            else
                return false;
        }

        public bool UnblockUser(string senderUserName, string receiverUserName)
        {
            var senderUser = GetUser(senderUserName);
            List<string> blockedULForSender = _slconverter.StringToList(senderUser.blockedUsers);
            if (!blockedULForSender.Contains(receiverUserName))
            {
                //if list doesn't contain blockuser name that means our job is done
                return false;
            }
            blockedULForSender.Remove(receiverUserName);
            blockedULForSender = EmptyListRuler(blockedULForSender); 
            //nullable logic for better (errorless) data transfer
            string blockedUsers = _slconverter.ListToString(blockedULForSender); 
            //converting 
            senderUser.blockedUsers = blockedUsers; 
            //turning our blockeduser list back to user's db
            
            //we are checking savechanges return count to understand did we cange anything on db
            int affects = _context.SaveChanges();  
            if (affects > 0)
            {
                return true;
            }
            else
                return false;
        }

        public ApplicationUser GetUser(string username)
        {
            //simple method for getting user
            var user = _context.Users.Where(a => a.UserName == username).Single();
            return user;
        }
        
        public List<string> EmptyListRuler(List<string> list)
        {
            //to turn empty lists into null objects
            if(list.Count == 0)
                return null;
            return list;
        }
    }
}
