using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class EventDTO
    {
        public int id { get; set; }
        public string Title { get; set; }

        public string Detail { get; set; }
        
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        public bool AllDay { get; set; }

        public int LevelStatus { get; set; }

        public int StaffId { get; set; }

        public string StaffName { get; set; }
    }
}