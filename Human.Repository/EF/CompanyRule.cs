﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Human.Repository.EF;

public partial class CompanyRule
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int DepartmentId { get; set; }

    public TimeSpan CheckInStartTime { get; set; }

    public TimeSpan CheckInEndTime { get; set; }

    public TimeSpan CheckOutStartTime { get; set; }

    public TimeSpan CheckOutEndTime { get; set; }

    public string DepartmentName { get; set; }

    public string AfternoonTime { get; set; }
}