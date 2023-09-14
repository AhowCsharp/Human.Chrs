using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.ViewModel.Request
{
    public partial class StaffDetailSaveRequest
    {
        public int Id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public DateTime BirthDay { get; set; }

        public string Gender { get; set; }

        public int IsMarried { get; set; }

        public int HasLicense { get; set; }

        public double Height { get; set; }

        public int Weight { get; set; }

        public string IdentityNo { get; set; }

        public int? HasCrimeRecord { get; set; }

        public string Memo { get; set; }
    }
}