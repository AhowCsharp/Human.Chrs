﻿using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class OverTimeRequest
    {
        public DateTime ChooseDate { get; set; }
        public int Hours { get; set; }
        public string Reason { get; set; }
    }
}