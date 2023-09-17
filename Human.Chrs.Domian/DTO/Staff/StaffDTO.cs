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
        public int Id { get; set; }

        public string StaffNo { get; set; } //員編

        public int CompanyId { get; set; }

        public string StaffAccount { get; set; }

        public string StaffPassWord { get; set; }

        public string Department { get; set; }//部門名稱

        public DateTime EntryDate { get; set; }//入職日

        public DateTime? ResignationDate { get; set; }//註冊日

        public string LevelPosition { get; set; }//職等

        public string WorkPosition { get; set; }//工作地點

        public string Email { get; set; }

        public int Status { get; set; }

        public int SpecialRestDays { get; set; }//特修

        public int SickDays { get; set; }//病假

        public int ThingDays { get; set; }//市價

        public int ChildbirthDays { get; set; }//育嬰假

        public int DeathDays { get; set; }//喪假

        public int MarryDays { get; set; }//婚嫁

        public int SpecialRestHours { get; set; }

        public int SickHours { get; set; }

        public int ThingHours { get; set; }

        public int ChildbirthHours { get; set; }

        public int DeathHours { get; set; }

        public int MarryHours { get; set; }

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

        public int DepartmentId { get; set; }
    }
}