using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.SeedWork;
using Human.Chrs.Domain.Services;
using Human.Chrs.ViewModel.Request;
using Human.Repository.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Extension
{
    public static class NewStaffSaveRequestExtension
    {
        public static StaffDTO ToDTO(this StaffSaveRequest request)
        {
            var dto = new StaffDTO
            {
                id = request.id,
                StaffNo = request.StaffNo,
                CompanyId = request.CompanyId,
                StaffAccount = request.StaffAccount,
                StaffPassWord = request.StaffPassWord,
                Department = request.Department,
                EntryDate = request.EntryDate,
                ResignationDate = request.ResignationDate,
                LevelPosition = request.LevelPosition,
                WorkLocation = request.WorkLocation,
                Email = request.Email,
                Status = request.Status,
                SpecialRestDays = request.SpecialRestDays,
                SickDays = request.SickDays,
                ThingDays = request.ThingDays,
                ChildbirthDays = request.ChildbirthDays,
                DeathDays = request.DeathDays,
                MarryDays = request.MarryDays,
                SpecialRestHours = request.SpecialRestHours,
                SickHours = request.SpecialRestHours,
                ThingHours = request.ThingHours,
                ChildbirthHours = request.ChildbirthHours,
                DeathHours = request.DeathHours,
                MarryHours = request.MarryHours,
                EmploymentTypeId = request.EmploymentTypeId,
                StaffPhoneNumber = request.StaffPhoneNumber,
                StaffName = request.StaffName,
                Auth = request.Auth,
                DepartmentId = request.DepartmentId,
                MenstruationDays = request.MenstruationDays,
                MenstruationHours = request.MenstruationHours,
                TocolysisDays = request.TocolysisDays,
                TocolysisHours = request.TocolysisHours,
                TackeCareBabyDays = request.TackeCareBabyDays,
                TackeCareBabyHours = request.TackeCareBabyHours,
                PrenatalCheckUpDays = request.PrenatalCheckUpDays,
                PrenatalCheckUpHours = request.PrenatalCheckUpHours,
                OverTimeHours = request.OverTimeHours,
                StayInCompanyDays = (DateTimeHelper.TaipeiNow - request.EntryDate).Days,
                Gender = request.Gender
            };
            if (request.ParttimeMoney.HasValue)
            {
                dto.ParttimeMoney = request.ParttimeMoney;
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