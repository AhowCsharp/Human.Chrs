﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Human.Repository.EF;

public partial class VacationLog
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public int CompanyId { get; set; }

    public string VacationType { get; set; }

    public DateTime ApplyDate { get; set; }

    public DateTime ActualStartDate { get; set; }

    public DateTime ActualEndDate { get; set; }

    public int Hours { get; set; }

    public int IsPass { get; set; }

    public string ApproverName { get; set; }

    public int? ApproverId { get; set; }

    public DateTime? AuditDate { get; set; }
}