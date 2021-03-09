using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Models
{
    public class MessageSendingModel
    {
        //
        [Required]
        public string receivedBy { get; set; }
        [Required]
        [MaxLength(500)]
        public string text { get; set; }
    }
}
