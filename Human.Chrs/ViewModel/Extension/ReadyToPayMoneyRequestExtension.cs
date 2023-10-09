using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.SeedWork;
using Human.Chrs.ViewModel.Request;
using Human.Repository.EF;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Extension
{
    public static class ReadyToPayMoneyRequestExtension
    {
        public static IncomeLogsDTO ToDTO(this ReadyToPayMoneyRequest request)
        {
            var dto = new IncomeLogsDTO
            {
                id = request.id,
                StaffId = request.StaffId,
                IssueDate = request.IssueDate,
                BasicSalary = request.BasicSalary,
                FullCheckInMoney = request.FullCheckInMoney,
                Bonus = request.Bonus,
                SickHours = request.SickHours,
                ThingHours = request.ThingHours,
                MenstruationHours = request.MenstruationHours,
                TocolysisHours = request.TocolysisHours,
                ChildbirthHours = request.ChildbirthHours,
                TakeCareBabyHours = request.TakeCareBabyHours,
                SalaryOfMonth = request.SalaryOfMonth,
                IncomeTax = request.IncomeTax,
                HealthInsurance = request.HealthInsurance,
                WorkerInsurance = request.WorkerInsurance,
                EmployeeRetirement = request.EmployeeRetirement,
                SupplementaryPremium = request.SupplementaryPremium,
                HealthInsuranceFromCompany = request.HealthInsuranceFromCompany,
                WorkerInsuranceFromCompany = request.WorkerInsuranceFromCompany,
                EmployeeRetirementFromCompany = request.EmployeeRetirementFromCompany,
                AdvanceFundFromCompany = request.AdvanceFundFromCompany,
                StaffIncomeAmount = request.StaffIncomeAmount,
                StaffActualIncomeAmount = request.StaffActualIncomeAmount,
                StaffDeductionAmount = request.StaffDeductionAmount,
                CompanyCostAmount = request.CompanyCostAmount,
                OverTimeHours = request.OverTimeHours,
                EarlyOrLateAmount = request.EarlyOrLateAmount,
                OutLocationAmount = request.OutLocationAmount,
                OverTimeAmount = request.OverTimeMoney,
            };

            if(request.ParttimeSalary.HasValue)
            {
                dto.ParttimeSalary = request.ParttimeSalary.Value;
            }

            return dto;
        }

        //public static NewFriendViewModel ToViewModel(this NewFriendInfoDTO dto)
        //{
        //    var viewModel = new NewFriendViewModel
        //    {
        //        Id = dto.Id,
        //        UserId = dto.UserId,
        //        PictureUrl = dto.PictureUrl,
        //        Name = dto.Name,
        //        StatusMessage = dto.StatusMessage,
        //        Status = dto.Status,
        //        JoinDate = dto.JoinDate,
        //        LastInteractionDate = dto.LastInteractionDate,
        //        PushCount = dto.PushCount,
        //        TalkCount = dto.TalkCount,
        //        NickName = dto.NickName,
        //        Phone = dto.Phone,
        //        Birthday = dto.Birthday,
        //        Email = dto.Email,
        //        Gender = dto.Gender,
        //        Language = dto.Language,
        //        Memo = dto.Memo,
        //        TagList = dto.TagList.ToListViewModel()
        //    };

        //    return viewModel;
        //}

        //public static IEnumerable<NewFriendItemViewModel> ToListViewModel(this IEnumerable<NewFriendItemDTO> dtos)
        //{
        //    foreach (var dto in dtos)
        //    {
        //        yield return ToViewModel(dto);
        //    }
        //}
    }
}