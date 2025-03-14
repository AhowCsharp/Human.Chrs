﻿using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface ICheckRecordsRepository : IRepository<CheckRecordsDTO, int>
    {
        Task<CheckRecordsDTO> GetCheckRecordAsync(int companyId, int staffId);

        Task<IEnumerable<CheckRecordsDTO>> GetCheckRecordListAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<CheckRecordsDTO> GetCheckRecordPeriodAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<bool> GetStaffCheckInStatus(int staffId);

        Task<bool> GetStaffCheckOutStatus(int staffId);
    }
}