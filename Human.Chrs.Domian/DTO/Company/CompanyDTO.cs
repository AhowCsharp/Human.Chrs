﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public partial class CompanyDTO : IDTO
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public double? CapitalAmount { get; set; }

        public string Address { get; set; }

        public string Coordinate { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public int? ContractType { get; set; }
    }
}