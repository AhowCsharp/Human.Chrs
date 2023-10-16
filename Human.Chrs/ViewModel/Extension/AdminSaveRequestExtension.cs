using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public static class AdminSaveRequestExtension
    {
        public static AdminDTO ToDTO(this AdminSaveRequest request)
        {
            var dto = new AdminDTO
            {
                id = request.id,
                CompanyId = request.CompanyId,
                UserName = request.UserName,
                Account = request.Account,
                Password = request.Password,
                Auth = request.Auth,
                WorkPosition = request.WorkPosition,
                StaffNo = request.StaffNo,
                DepartmentId = request.DepartmentId,
                AdminToken = request.id == 0 ? Guid.NewGuid().ToString() : string.Empty,
                Status = true
            };
            if (!string.IsNullOrEmpty(request.PrePassword))
            {
                dto.PrePassword = request.PrePassword;
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