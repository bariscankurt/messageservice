using ArmutServices.DTOs;
using ArmutServices.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Profiles
{
    public class MessagesProfile : Profile
    {
        //part of automapper. I am defining map route in here
        public MessagesProfile()
        {
            CreateMap<Message, MessageReadDTO>();
        }
    }
}
