using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.DTOs
{
    public class MessageReadDTO
    {
        //I just wanted to show you i am aware of dtos and logic behind it.
        //In this particular case hiding message id isn't usefull but
        //In real world situations dtos are very important and i know them.
        //I used automapper for this data mapping.
        public string sentBy { get; set; }
        
        public string receivedBy { get; set; }
        
        public string text { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
