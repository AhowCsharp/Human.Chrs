using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class LoginDTO
    {
        public string UserId { get; set; }

        public string StaffName { get; set; }

        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public int? Auth { get; set; }

        public string? AdminToken { get; set; }

        public string AvatarUrl { get; set; }

        public string SuperToken { get; set; }

        public bool IsSuper { get; set; } = false;

        public string DeviceId { get; set; }
    }
}