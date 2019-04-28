using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace road_scholar
{
    public class CoordinatePoint
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string timeStamp { get; set; }

        //TODO: Update these to the appropriate data types
        public string distance { get; set; }
        public string speed { get; set; }
        public string heartRate { get; set; }

        public bool HasLatLong()
        {
            return latitude != null && latitude != "" 
                && longitude != null && longitude != "";
        }
    }
}
