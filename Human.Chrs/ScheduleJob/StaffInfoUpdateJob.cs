﻿using Human.Chrs.Domain;
using Human.Chrs.Admin.ApiControllers;
using Quartz;

namespace Human.Chrs.ScheduleJob
{
    public class StaffInfoUpdateJob : IJob
    {
        private readonly ILogger<StaffInfoUpdateJob> _logger;
        private readonly ScheduleDomain _scheduleDomain;

        public StaffInfoUpdateJob(ScheduleDomain scheduleDomain, ILogger<StaffInfoUpdateJob> logger)
        {
            _logger = logger;
            _scheduleDomain = scheduleDomain;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _scheduleDomain.DeleteNotificationAsync();
            await _scheduleDomain.UpdateStaffInfoAsync();
        }

    }
}