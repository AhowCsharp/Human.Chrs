using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class CheckRecordsViewDTO
    {
        public int TotalCheckInLateDays { get; set; }

        public int TotalCheckInLateMinutes { get; set; }
        public int TotalCheckOutEarlyDays { get; set; }
        public int TotalCheckOutEarlyMinutes { get; set; }
        public int OutLocationCheckInDays { get; set; }
        public int OutLocationCheckOutDays { get; set; }

        public IEnumerable<CheckRecordsDTO> CheckRecords { get; set; }

        public IEnumerable<VacationLogDTO> VacationLogs { get; set; }
    }
}