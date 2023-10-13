using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class NewCompanyRequest
    {
        public int id { get; set; }

        public string CompanyName { get; set; }

        public double? CapitalAmount { get; set; }

        public string Address { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public int? ContractType { get; set; }
    }
}