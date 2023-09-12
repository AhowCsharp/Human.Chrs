using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public partial class LoginDTO : IDTO
    {
        public int Id { get; set; }

        public string StaffName { get; set; }

        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public string StaffAccount { get; set; }

        public string Department { get; set; }

        public int? Auth { get; set; }
    }
}