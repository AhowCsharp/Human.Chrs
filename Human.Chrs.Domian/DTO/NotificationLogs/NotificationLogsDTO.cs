using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class NotificationLogsDTO : IDTO
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Avatar { get; set; }

        public string Type { get; set; }

        public bool IsUnRead { get; set; }

        public DateTime CreateDate { get; set; }

        public string Creator { get; set; }

        public int DepartmentId { get; set; }

        public string ReadStaffIds { get; set; }

    }
}