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
    public static class CompanyRuleRequestExtension
    {
        public static CompanyRuleDTO ToDTO(this CompanyRuleRequest request)
        {
            var dto = new CompanyRuleDTO
            {
                id = request.id,
                CompanyId = request.CompanyId,
                DepartmentId = request.DepartmentId,
                CheckInStartTime = request.CheckInStartTime,
                CheckInEndTime = request.CheckInEndTime,
                CheckOutStartTime = request.CheckOutStartTime,
                CheckOutEndTime = request.CheckOutEndTime,
                DepartmentName = request.DepartmentName,
                AfternoonTime = request.AfternoonTime,
                NeedWorkMinute = request.NeedWorkMinute
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