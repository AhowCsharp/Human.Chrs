﻿using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IStaffRepository : IRepository<StaffDTO, int>
    {
        Task<StaffDTO> VerifyLoginStaffAsync(string account, string password);

        Task<StaffDTO> GetUsingStaffAsync(int staffId, int companyId);

        Task<bool> VerifyExistStaffAsync(int staffId, int companyId);

        Task<IEnumerable<StaffDTO>> GetAllStaffAsync(int companyId);

        Task UpdateWorkDaysAndFindStaffAsync();

        Task<bool> VerifyAccountAsync(string account);

        Task<IEnumerable<StaffDTO>> GetAllParttimeStaffAsync(int companyId);

        Task<IEnumerable<StaffDTO>> GetDepartmentStaffAsync(int companyId, int departrmentId);

        Task<bool> VerifyEmailAsync(string email);

        Task<bool> VerifyEmailAsync(string email, int staffId);

        Task<StaffDTO> GetForgetPasswordStaffAsync(string account, string email);

        Task<int> StaffCountAsync(int companyId);

        Task<IEnumerable<StaffDTO>> GetAllDaySalaryStaffAsync(int companyId);
    }
}