using ArmutServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Data
{
    public interface IMessageRepo
    {
        bool CreateMessage(Message msg);
        IEnumerable<Message> PastMessages(string userA, string userB);
    }
}
