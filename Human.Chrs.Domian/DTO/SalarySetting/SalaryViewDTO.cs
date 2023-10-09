using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class SalaryViewDTO
    {
        public SalarySettingDTO SalarySetting { get; set; }

        public int TotalCheckInLateDays { get; set; }

        public int TotalCheckInLateMinutes { get; set; }
        public int TotalCheckOutEarlyDays { get; set; }
        public int TotalCheckOutEarlyMinutes { get; set; }
        public int OutLocationCheckInDays { get; set; }
        public int OutLocationCheckOutDays { get; set; }

        public int SpecialRestDays { get; set; }

        public int SpecialRestHours { get; set; }

        public int TotalSpecialRestHours { get; set; }

        public int SickDays { get; set; }

        public int SickHours { get; set; }

        public int TotalSickHours { get; set; }

        public int ThingDays { get; set; }

        public int ThingHours { get; set; }

        public int TotalThingHours { get; set; }

        public int ChildbirthDays { get; set; }

        public int ChildbirthHours { get; set; }

        public int TotalChildbirthHours { get; set; }

        public int DeathDays { get; set; }

        public int DeathHours { get; set; }

        public int TotalDeathHours { get; set; }

        public int WorkthingDays { get; set; }

        public int WorkthingHours { get; set; }

        public int TotalWorkthingHours { get; set; }
        public int WorkhurtDays { get; set; }

        public int WorkhurtHours { get; set; }

        public int TotalWorkhurtHours { get; set; }

        public int MarryDays { get; set; }

        public int MarryHours { get; set; }

        public int TotalMarryHours { get; set; }

        public int MenstruationDays { get; set; }

        public int MenstruationHours { get; set; }

        public int TotalMenstruationHours { get; set; }

        public int TocolysisDays { get; set; }

        public int TocolysisHours { get; set; }

        public int TotalTocolysisHours { get; set; }

        public int TackeCareBabyDays { get; set; }

        public int TackeCareBabyHours { get; set; }

        public int TotalTackeCareBabyHours { get; set; }

        public int PrenatalCheckUpDays { get; set; }

        public int PrenatalCheckUpHours { get; set; }

        public int TotalPrenatalCheckUpHours { get; set; }

        public int OverTimeHours { get; set; }

        public int TotalSalaryNoOvertime { get; set; }
        public int PerHourSalary { get; set; }

        public int OverTimeMoney { get; set; }
    }
}