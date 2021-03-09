using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Models
{
    public class RegisterModel
    {
        //Simple model for easy postman testing
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
