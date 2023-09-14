using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class DistanceRequest
    {
        public int CompanyId { get; set; }

        public int StaffId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}