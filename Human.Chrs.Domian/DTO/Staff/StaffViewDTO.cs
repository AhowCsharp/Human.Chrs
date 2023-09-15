using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class StaffViewDTO
    {
        public bool IsOverLocation { get; set; }

        public bool IsCheckIn { get; set; }

        public string CheckInRange { get; set; }

        public string CheckOutRange { get; set; }

        public string AfternoonRange { get; set; }

        public IEnumerable<VacationLogDTO> VacationLogDTOs { get; set; }
    }
}