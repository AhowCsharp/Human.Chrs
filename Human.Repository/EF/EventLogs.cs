﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Human.Repository.EF;

public partial class EventLogs
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public int? CompanyId { get; set; }

    public string Title { get; set; }

    public string Detail { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int LevelStatus { get; set; }

    public int? MeetId { get; set; }
}