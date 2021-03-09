using ArmutServices.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Data
{
    public class SqlMessageRepo : IMessageRepo
    {
        private readonly ApplicationDbContext _context;

        public SqlMessageRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateMessage(Message msg)
        {
            //this method helps my sendmessage method in controller.
            //the logic behind this method is whenever i use savechanges method
            //if something changes on database, savechanges method returns an integer that different from 0
            //CreateMessage method returns a boolean variable and with that variable i can check if this method
            //worked correctly or not.
            _context.messages.Add(msg);
            int affects = _context.SaveChanges();
            if (affects > 0)
            {
                return true;
            }
            else
                return false;
        }

        
        public IEnumerable<Message> PastMessages(string userA, string userB)
        {
            //In this method, i am returning all messages between userA and userB
            //I used reverse method to see most recent message on top but it's optional.
            IEnumerable<Message> pastMessages = _context.messages.Where(
                msg => msg.sentBy == userA && msg.receivedBy == userB 
                || msg.sentBy == userB && msg.receivedBy == userA);
            return pastMessages.Reverse();
        }
    }
}
