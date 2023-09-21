﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Human.Repository.EF;

public partial class HumanChrsContext : DbContext
{
    public HumanChrsContext(DbContextOptions<HumanChrsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admin { get; set; }

    public virtual DbSet<Application> Application { get; set; }

    public virtual DbSet<CheckRecords> CheckRecords { get; set; }

    public virtual DbSet<Company> Company { get; set; }

    public virtual DbSet<CompanyRule> CompanyRule { get; set; }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<EventLogs> EventLogs { get; set; }

    public virtual DbSet<IncomeLogs> IncomeLogs { get; set; }

    public virtual DbSet<OverTimeLog> OverTimeLog { get; set; }

    public virtual DbSet<PersonalDetail> PersonalDetail { get; set; }

    public virtual DbSet<SalarySetting> SalarySetting { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<UpdateStaffInfoLogs> UpdateStaffInfoLogs { get; set; }

    public virtual DbSet<VacationLog> VacationLog { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.Property(e => e.Account).IsRequired();
            entity.Property(e => e.AdminToken).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.StaffNo)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.WorkPosition)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.Property(e => e.Id).HasComment("編號");
            entity.Property(e => e.ApId)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("帳號");
            entity.Property(e => e.ApName)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("名稱");
            entity.Property(e => e.Expire)
                .HasComment("權杖效期")
                .HasColumnType("datetime");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("密碼");
            entity.Property(e => e.SkipLineUserId).HasComment("不驗證 LINE UserId");
            entity.Property(e => e.Status).HasComment("狀態");
            entity.Property(e => e.Token)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasComment("權杖");
        });

        modelBuilder.Entity<CheckRecords>(entity =>
        {
            entity.Property(e => e.CheckInMemo).HasMaxLength(50);
            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutMemo).HasMaxLength(50);
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.ContractEndDate).HasColumnType("date");
            entity.Property(e => e.ContractStartDate).HasColumnType("date");
        });

        modelBuilder.Entity<CompanyRule>(entity =>
        {
            entity.Property(e => e.AfternoonTime).HasMaxLength(50);
            entity.Property(e => e.DepartmentName).HasMaxLength(50);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValueSql("('???')");
        });

        modelBuilder.Entity<EventLogs>(entity =>
        {
            entity.Property(e => e.Detail).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<IncomeLogs>(entity =>
        {
            entity.Property(e => e.IssueDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OverTimeLog>(entity =>
        {
            entity.Property(e => e.Inspector).HasMaxLength(50);
            entity.Property(e => e.OvertimeDate).HasColumnType("date");
            entity.Property(e => e.ValidateDate).HasColumnType("date");
        });

        modelBuilder.Entity<PersonalDetail>(entity =>
        {
            entity.Property(e => e.BirthDay).HasColumnType("date");
            entity.Property(e => e.EnglishName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.IdentityNo).HasMaxLength(50);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<SalarySetting>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.EditDate).HasColumnType("datetime");
            entity.Property(e => e.Editor)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Creator).HasMaxLength(50);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.EditDate).HasColumnType("datetime");
            entity.Property(e => e.Editor).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("date");
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(8)
                .HasDefaultValueSql("('??')");
            entity.Property(e => e.LevelPosition).HasMaxLength(50);
            entity.Property(e => e.ResignationDate).HasColumnType("date");
            entity.Property(e => e.StaffAccount).IsRequired();
            entity.Property(e => e.StaffName).HasMaxLength(50);
            entity.Property(e => e.StaffNo)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.StaffPassWord).IsRequired();
            entity.Property(e => e.StaffPhoneNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<UpdateStaffInfoLogs>(entity =>
        {
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<VacationLog>(entity =>
        {
            entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.ApplyDate).HasColumnType("datetime");
            entity.Property(e => e.ApproverName).HasMaxLength(10);
            entity.Property(e => e.AuditDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}