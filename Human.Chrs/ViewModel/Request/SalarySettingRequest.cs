﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.ViewModel.Request
{
    public partial class SalarySettingRequest
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public int BasicSalary { get; set; }

        public int FullCheckInMoney { get; set; }

        public int? OtherPercent { get; set; }

        public int? FoodSuportMoney { get; set; }
    }
}