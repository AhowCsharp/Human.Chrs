using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class ResetPasswordLogsDTO : IDTO
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public DateTime CreateDate { get; set; }
    }
}