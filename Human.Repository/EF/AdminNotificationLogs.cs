﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Human.Repository.EF;

public partial class AdminNotificationLogs
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public int CompanyId { get; set; }

    public string EventType { get; set; }

    public string EventDetail { get; set; }

    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public bool IsUnRead { get; set; }

    public string ReadAdminIds { get; set; }
}