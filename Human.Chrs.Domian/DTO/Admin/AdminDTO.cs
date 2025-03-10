﻿using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.DTO
{
    public class AdminDTO : IDTO
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public string UserName { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public int? Auth { get; set; }

        public string WorkPosition { get; set; }

        public string StaffNo { get; set; }

        public int DepartmentId { get; set; }

        public string AdminToken { get; set; }

        public string CompanyName { get; set; }

        public string AvatarUrl { get; set; }

        public bool Status { get; set; }

        public string SuperToken { get; set; }

        public bool IsSuperAdmin { get; set; } = false;

        public string PrePassword { get; set; }
    }
}