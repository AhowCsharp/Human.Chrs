using AutoMapper;
using EFCore.BulkExtensions;
using Human.Chrs.Domain.IRepository;
using Newtonsoft.Json;
using Human.Repository.Repository.Base;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Human.Repository.EF;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.Helper;

namespace LineTag.Infrastructure.Repositories
{
    public class StaffRepository : BaseRepository<Staff, StaffDTO, int>, IStaffRepository
    {
        public StaffRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<StaffDTO> VerifyLoginStaffAsync(string account, string password)
        {
            var data = await _context.Staff.SingleOrDefaultAsync(x => x.StaffAccount == account && x.StaffPassWord == password && x.Status == 1);

            return _mapper.Map<StaffDTO>(data);
        }

        public async Task<bool> VerifyAccountAsync(string account)
        {
            var data = await _context.Staff.AnyAsync(x => x.StaffAccount == account);

            return data;
        }

        public async Task<bool> VerifyExistStaffAsync(int staffId, int companyId)
        {
            var data = await _context.Staff.AnyAsync(x => x.Id == staffId && x.CompanyId == companyId && x.Status == 1);

            return data;
        }

        public async Task<StaffDTO> GetUsingStaffAsync(int staffId, int companyId)
        {
            var data = await _context.Staff.SingleOrDefaultAsync(x => x.Id == staffId && x.CompanyId == companyId && x.Status == 1);

            return _mapper.Map<StaffDTO>(data);
        }

        public async Task<IEnumerable<StaffDTO>> GetAllStaffAsync(int companyId)
        {
            var data = await _context.Staff.Where(x => x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<StaffDTO>);
        }

        public async Task<IEnumerable<StaffDTO>> GetDepartmentStaffAsync(int companyId,int departrmentId)
        {
            var data = await _context.Staff.Where(x => x.CompanyId == companyId && x.DepartmentId == departrmentId).ToListAsync();

            return data.Select(_mapper.Map<StaffDTO>);
        }

        public async Task<IEnumerable<StaffDTO>> GetAllParttimeStaffAsync(int companyId)
        {
            var data = await _context.Staff.Where(x => x.CompanyId == companyId && x.EmploymentTypeId == 2).ToListAsync();

            return data.Select(_mapper.Map<StaffDTO>);
        }

        public async Task<bool> UpdateWorkDaysAndFindStaffAsync()
        {
            var log = new UpdateStaffInfoLogs();
            try
            {
                bool isStartDateOfMonth = DateTimeHelper.TaipeiNow.Day == 1;
                var currentYear = DateTimeHelper.TaipeiNow.Year;

                var staffList = await _context.Staff
                                              .Where(x => x.Status == 1)
                                              .ToListAsync();

                foreach (var staff in staffList)
                {
                    staff.StayInCompanyDays += 1;

                    if (isStartDateOfMonth)
                    {
                        var detailInfo = await _context.PersonalDetail.FirstOrDefaultAsync(x => x.StaffId == staff.Id && x.CompanyId == staff.CompanyId);
                        if (detailInfo != null && detailInfo.Gender == "女性")
                        {
                            var count = await _context.VacationLog
                                .Where(x => x.StaffId == staff.Id && x.CompanyId == staff.CompanyId
                                        && x.ActualStartDate.Year == currentYear && x.VacationType == 8)
                                .CountAsync();

                            staff.MenstruationDays = count > 3 ? 0 : 1;
                        }
                    }

                    if (staff.StayInCompanyDays <= 4015)
                    {
                        staff.SpecialRestDays += staff.StayInCompanyDays switch
                        {
                            180 => 3,
                            365 => 7,
                            730 => 10,
                            1095 => 14,
                            1460 => 14,
                            1825 => 15,
                            2190 => 15,
                            2555 => 15,
                            2920 => 15,
                            3285 => 15,
                            3650 => 16,
                            4015 => 17,
                            _ => 0
                        };
                    }
                    else
                    {
                        int yearsOver10 = (staff.StayInCompanyDays - 4015) / 365;
                        int additionalDays = 17 + yearsOver10;

                        if (additionalDays > 30)
                        {
                            additionalDays = 30;
                        }

                        staff.SpecialRestDays += additionalDays;
                    }
                }

                log.Finished = 1;
                log.UpdateTime = DateTimeHelper.TaipeiNow;
                log.ErrorMessage = string.Empty;
                await _context.UpdateStaffInfoLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Finished = 0;
                log.UpdateTime = DateTimeHelper.TaipeiNow;
                log.ErrorMessage = ex.ToString();
                await _context.UpdateStaffInfoLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                return false;
            }
        }
    }
}