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

        public decimal TotalCheckInLateDays { get; set; }

        public decimal TotalCheckInLateMinutes { get; set; }
        public decimal TotalCheckOutEarlyDays { get; set; }
        public decimal TotalCheckOutEarlyMinutes { get; set; }
        public decimal OutLocationCheckInDays { get; set; }
        public decimal OutLocationCheckOutDays { get; set; }

        public decimal SpecialRestDays { get; set; }

        public decimal SpecialRestHours { get; set; }

        public decimal TotalSpecialRestHours { get; set; }

        public decimal SickDays { get; set; }

        public decimal SickHours { get; set; }

        public decimal TotalSickHours { get; set; }

        public decimal ThingDays { get; set; }

        public decimal ThingHours { get; set; }

        public decimal TotalThingHours { get; set; }

        public decimal ChildbirthDays { get; set; }

        public decimal ChildbirthHours { get; set; }

        public decimal TotalChildbirthHours { get; set; }

        public decimal DeathDays { get; set; }

        public decimal DeathHours { get; set; }

        public decimal TotalDeathHours { get; set; }

        public decimal WorkthingDays { get; set; }

        public decimal WorkthingHours { get; set; }

        public decimal TotalWorkthingHours { get; set; }
        public decimal WorkhurtDays { get; set; }

        public decimal WorkhurtHours { get; set; }

        public decimal TotalWorkhurtHours { get; set; }

        public decimal MarryDays { get; set; }

        public decimal MarryHours { get; set; }

        public decimal TotalMarryHours { get; set; }

        public decimal MenstruationDays { get; set; }

        public decimal MenstruationHours { get; set; }

        public decimal TotalMenstruationHours { get; set; }

        public decimal TocolysisDays { get; set; }

        public decimal TocolysisHours { get; set; }

        public decimal TotalTocolysisHours { get; set; }

        public decimal TackeCareBabyDays { get; set; }

        public decimal TackeCareBabyHours { get; set; }

        public decimal TotalTackeCareBabyHours { get; set; }

        public decimal PrenatalCheckUpDays { get; set; }

        public decimal PrenatalCheckUpHours { get; set; }

        public decimal TotalPrenatalCheckUpHours { get; set; }

        public decimal OverTimeHours { get; set; }

        public decimal TotalSalaryNoOvertime { get; set; }
        public decimal PerHourSalary { get; set; }

        public decimal OverTimeMoney { get; set; }
         
        public decimal FoodSuportMoney { get; set; }
    }
}