using System;

namespace Human.Chrs.Domain.CommonModels
{
    public class CurrentUser
    {
        public string ApToken { get; set; }

        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string StaffName { get; set; }

        public int DepartmentId { get; set; }

        public string Email { get; set; }

        public int? Auth { get; set; }

        public string? AdminToken { get; set; }
    }
}