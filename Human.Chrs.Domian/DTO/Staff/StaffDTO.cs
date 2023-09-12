using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public partial class StaffDTO : IDTO
    {
        public int Id { get; set; }

        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public string StaffAccount { get; set; }

        public string StaffPassWord { get; set; }

        public string Department { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime? ResignationDate { get; set; }

        public string LevelPosition { get; set; }

        public string WorkPosition { get; set; }

        public string Email { get; set; }

        public int Status { get; set; }

        public int? SpecialRestDays { get; set; }

        public int? SickDays { get; set; }

        public int? ThingDays { get; set; }

        public int? ChildbirthDays { get; set; }

        public int? DeathDays { get; set; }

        public int? MarryDays { get; set; }

        public int? SpecialRestHours { get; set; }

        public int? SickHours { get; set; }

        public int? ThingHours { get; set; }

        public int? ChildbirthHours { get; set; }

        public int? DeathHours { get; set; }

        public int? MarryHours { get; set; }

        public int? PersonalDetailId { get; set; }

        public int? EmploymentTypeId { get; set; }

        public string WorkDay { get; set; }

        public string RestDay { get; set; }

        public string StaffPhoneNumber { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Creator { get; set; }

        public DateTime? EditDate { get; set; }

        public string Editor { get; set; }

        public string StaffName { get; set; }

        public int? Auth { get; set; }
    }
}