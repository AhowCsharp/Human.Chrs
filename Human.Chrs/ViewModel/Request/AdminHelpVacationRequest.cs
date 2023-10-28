using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class AdminHelpVacationRequest
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public int Hours { get; set; }

        public int Type { get; set; }
        public string Reason { get; set; }

        public int staffId { get; set; }
    }
}