using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class VerifyVacationRequest
    {
        public int VacationId { get; set; }
        public bool IsPass { get; set; }
    }
}