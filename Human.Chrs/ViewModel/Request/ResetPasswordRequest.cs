using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class ResetPasswordRequest
    {
        public string Account { get; set; }

        public string Email { get; set; }
    }
}