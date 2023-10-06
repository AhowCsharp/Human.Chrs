using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class AmendCheckCheckRequest
    {
        public DateTime CheckDate { get; set; }

        public DateTime CheckTime { get; set; }

        public int CheckType { get; set; }  //0 上班  1 下班

        public string Reason { get; set; }
    }
}