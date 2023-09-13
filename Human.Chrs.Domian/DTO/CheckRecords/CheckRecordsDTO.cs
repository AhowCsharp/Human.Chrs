using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public partial class CheckRecordsDTO : IDTO
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int StaffId { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string CheckInMemo { get; set; }

        public string CheckOutMemo { get; set; }

        public int? IsCheckInOutLocation { get; set; }

        public int? IsCheckOutOutLocation { get; set; }

        public int? IsCheckInLate { get; set; }

        public int? CheckInLateTimes { get; set; }

        public int? CheckOutEarlyTimes { get; set; }

        public int? IsCheckOutEarly { get; set; }
    }
}