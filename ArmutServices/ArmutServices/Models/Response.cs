using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Models
{
    public class Response
    {
        //i made a custom response class to send related process information to user/consumer
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
