﻿using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.CommonModels;

namespace Human.Chrs.Domain.IRepository
{
    public interface IAdminRepository : IRepository<AdminDTO, int>
    {
        Task<AdminDTO> GetAvailableAdminAsync(int id, int companyId);

        Task<AdminDTO> VerifyLoginAdminAsync(string account);

        Task<bool> VerifyAdminTokenAsync(CurrentUser admin);

        Task<IEnumerable<AdminDTO>> GetAllAdminsAsync(int companyId);

        Task<int> GetAllAdminsCountAsync(int companyId);

        Task<bool> VerifyWebSocketAdminTokenAsync(string token, int adminId);

        Task<bool> VerifyAdminAccountAsync(string account);

        Task<bool> VerifyAdminAccountAsync(string account, int adminId);
    }
}