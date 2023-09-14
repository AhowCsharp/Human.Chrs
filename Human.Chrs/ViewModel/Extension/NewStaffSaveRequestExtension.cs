using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.SeedWork;
using Human.Chrs.ViewModel.Request;
using Human.Repository.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Extension
{
    public static class NewStaffSaveRequestExtension
    {
        public static StaffDTO ToDTO(this NewStaffSaveRequest request)
        {
            var dto = new StaffDTO
            {
                StaffNo = request.StaffNo,
                CompanyId = request.CompanyId,
                StaffAccount = request.StaffAccount,
                StaffPassWord = request.StaffPassWord,
                Department = request.Department,
                EntryDate = request.EntryDate,
                LevelPosition = request.LevelPosition,
                WorkPosition = request.WorkPosition,
                Email = request.Email,
                StaffPhoneNumber = request.StaffPhoneNumber,
                StaffName = request.StaffName,
                Auth = request.Auth,
                DepartmentId = request.DepartmentId,
                EmploymentTypeId = request.EmploymentTypeId,
            };

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