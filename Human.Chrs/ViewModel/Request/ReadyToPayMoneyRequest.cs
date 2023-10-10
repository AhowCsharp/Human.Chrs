using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public int? TocolysisHours { get; set; }

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

        public int? ParttimeSalary { get; set; }

        public int FoodSuportMoney { get; set; }
    }



    //setSalaryRequest({
    //StaffId: selectedRow.id,
    //      BasicSalary: 0,
    //      FullCheckInMoney: 0,
    //      OverTimeHours: selectedRow.OverTimeHours,
    //      Bonus: 0,
    //      SickHours: 0,
    //      ThingHours: 0,
    //      MenstruationHours: 0,
    //      ChildbirthHours: 0,
    //      TakeCareBabyHours: 0,
    //      IncomeTax: 0,
    //      HealthInsurance: 0,
    //      WorkerInsurance: 0,
    //      EmployeeRetirement: 0,
    //      SupplementaryPremium: 0, //
    //      HealthInsuranceFromCompany: 0,
    //      WorkerInsuranceFromCompany: 0,
    //      EmployeeRetirementFromCompany: 0,
    //      AdvanceFundFromCompany: 0,
    //      EarlyOrLateAmount: 0, 
    //      OutLocationAmount: 0,
    //      OverTimeMoney: 0,
    //      SalaryOfMonth: month,
    //      StaffIncomeAmount: calculateWage(selectedRow.ParttimeMoney, selectedRow.TotalPartimeHours, selectedRow.TotalPartimeMinutes) ,
    //      StaffActualIncomeAmount: 0,
    //      StaffDeductionAmount: 0,
    //      CompanyCostAmount: 0,
    //      ChangeOverTimeToMoney: true,
    //    })











}