using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.ViewModel.Request
{
    public partial class NewStaffSaveRequest
    {
        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public string StaffAccount { get; set; }

        public string StaffPassWord { get; set; }

        public string Department { get; set; }

        public DateTime EntryDate { get; set; }

        public string LevelPosition { get; set; }

        public string WorkPosition { get; set; }

        public string Email { get; set; }

        public string StaffPhoneNumber { get; set; }

        public string StaffName { get; set; }

        public int? Auth { get; set; }

        public int DepartmentId { get; set; }

        public int EmploymentTypeId { get; set; }
    }
}