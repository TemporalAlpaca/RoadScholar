using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace road_scholar
{
    public class CoordinatePoint
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double timeStamp { get; set; }

        //TODO: Update these to the appropriate data types
        public double distance { get; set; }
        public double speed { get; set; }
        public double heartRate { get; set; }

        public bool HasLatLong()
        {
            return latitude != 0
                && longitude != 0;
        }
    }
}
