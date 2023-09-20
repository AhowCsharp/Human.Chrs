using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public static class StaffDetailSaveRequestExtension
    {
        public static PersonalDetailDTO ToDTO(this StaffDetailSaveRequest request)
        {
            var dto = new PersonalDetailDTO
            {
                id = request.Id,
                Name = request.Name,
                StaffId = request.StaffId,
                CompanyId = request.CompanyId,
                EnglishName = request.EnglishName,
                BirthDay = request.BirthDay,
                Gender = request.Gender,
                IsMarried = request.IsMarried,
                HasLicense = request.HasLicense,
                Height = request.Height,
                Weight = request.Weight,
                IdentityNo = request.IdentityNo,
                HasCrimeRecord = request.HasCrimeRecord,
                Memo = request.Memo,
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