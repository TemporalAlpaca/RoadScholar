using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace road_scholar
{
    public class Route
    {
        private List<CoordinatePoint> coordinatePoints;
        public double totalDistance { get; set; }
        public string totalTime { get; set; }

        public Route()
        {
            coordinatePoints = new List<CoordinatePoint>();
        }

        public void addCoordinatePoint(CoordinatePoint coordinatePoint)
        {
            coordinatePoints.Add(coordinatePoint);
        }

        public List<CoordinatePoint> getCoordinatePoints()
        {
            return coordinatePoints;
        }
    }
}
