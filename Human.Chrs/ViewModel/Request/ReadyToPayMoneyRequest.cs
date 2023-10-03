using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class ReadyToPayMoneyRequest
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public DateTime IssueDate { get; set; }

        public int BasicSalary { get; set; }

        public int? FullCheckInMoney { get; set; }

        public int? Bonus { get; set; }

        public int? SickHours { get; set; }

        public int? ThingHours { get; set; }

        public int? MenstruationHours { get; set; }

        public int SalaryOfMonth { get; set; }

        public int? ChildbirthHours { get; set; }

        public int? TakeCareBabyHours { get; set; }

        public int? IncomeTax { get; set; }

        public int HealthInsurance { get; set; }

        public int WorkerInsurance { get; set; }

        public int EmployeeRetirement { get; set; }

        public int SupplementaryPremium { get; set; }

        public int HealthInsuranceFromCompany { get; set; }

        public int WorkerInsuranceFromCompany { get; set; }

        public int EmployeeRetirementFromCompany { get; set; }

        public int AdvanceFundFromCompany { get; set; }

        public int StaffIncomeAmount { get; set; }

        public int StaffActualIncomeAmount { get; set; }

        public int StaffDeductionAmount { get; set; }

        public int CompanyCostAmount { get; set; }

        public int OverTimeHours { get; set; }

        public int OverTimeMoney { get; set; }

        public int EarlyOrLateAmount { get; set; }

        public int OutLocationAmount { get; set; }

        public bool ChangeOverTimeToMoney { get; set; }
    }
}