using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.ViewModel.Request
{
    public partial class AdminSaveRequest
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public string UserName { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string? PrePassword { get; set; }

        public int? Auth { get; set; }

        public string WorkPosition { get; set; }

        public string StaffNo { get; set; }

        public int DepartmentId { get; set; }
    }
}