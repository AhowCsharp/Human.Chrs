using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class CheckRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Memo { get; set; }
    }
}