﻿using AutoMapper;
using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Dtos
{
    public class RoleDto : Profile
    {
        public RoleName Roles { get; set; }
    }
}
