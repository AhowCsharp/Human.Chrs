using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public static class MeetRequestExtension
    {
        public static EventLogsDTO ToDTO(this MeetRequest request)
        {
            var dto = new EventLogsDTO
            {
                StaffId = request.StaffId.HasValue? request.StaffId.Value:0,
                DepartmentId = request.DepartmentId.HasValue? request.DepartmentId.Value:0,
                Title = request.Title,
                Detail = request.Detail,
                StartDate = request.EventStartDate,
                EndDate = request.EventStartDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                LevelStatus = request.LevelStatus,
                MeetType = request.MeetType,
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