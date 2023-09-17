using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class EventRequest
    {
        public string Title { get; set; }
        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        public string Detail { get; set; }

        public int LevelStatus { get; set; }
    }
}