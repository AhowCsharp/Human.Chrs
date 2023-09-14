using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.CommonModels
{
    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Geometry
    {
        public Location Location { get; set; }
    }

    public class Result
    {
        public Geometry Geometry { get; set; }
    }

    public class RootObject
    {
        public List<Result> Results { get; set; }
    }
}
