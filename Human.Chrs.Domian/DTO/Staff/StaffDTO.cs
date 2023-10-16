using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class StaffDTO : IDTO
    {
        public int id { get; set; }

        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public string StaffAccount { get; set; }

        public string StaffPassWord { get; set; }

        public string Department { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime? ResignationDate { get; set; }

        public string LevelPosition { get; set; }

        public string WorkLocation { get; set; }

        public string Email { get; set; }

        public int Status { get; set; }

        public int SpecialRestDays { get; set; }

        public int SickDays { get; set; }

        public int ThingDays { get; set; }

        public int ChildbirthDays { get; set; }

        public int DeathDays { get; set; }

        public int MarryDays { get; set; }

        public int SpecialRestHours { get; set; }

        public int SickHours { get; set; }

        public int ThingHours { get; set; }

        public int ChildbirthHours { get; set; }

        public int DeathHours { get; set; }

        public int MarryHours { get; set; }

        public int? PersonalDetailId { get; set; }

        public int? EmploymentTypeId { get; set; }

        public string StaffPhoneNumber { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Creator { get; set; }

        public DateTime? EditDate { get; set; }

        public string Editor { get; set; }

        public string StaffName { get; set; }

        public int? Auth { get; set; }

        public int DepartmentId { get; set; }

        public int MenstruationDays { get; set; }

        public int MenstruationHours { get; set; }

        public int TocolysisDays { get; set; }

        public int TocolysisHours { get; set; }

        public int TackeCareBabyDays { get; set; }

        public int TackeCareBabyHours { get; set; }

        public int PrenatalCheckUpDays { get; set; }

        public int PrenatalCheckUpHours { get; set; }

        public int OverTimeHours { get; set; }

        public int StayInCompanyDays { get; set; }

        public string Gender { get; set; }

        public string AvatarUrl { get; set; }

        public int TotalPartimeHours { get; set; }

        public int TotalPartimeMinutes { get; set; }

        public int? ParttimeMoney { get; set; }

        public int? ParttimeOverTimeHours { get; set; }

        public int? ParttimeOverTimeTotalMony { get; set; }

        public string Language { get; set; }

        public string DeviceId { get; set; }
    }
}