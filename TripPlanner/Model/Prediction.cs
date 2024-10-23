using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Model
{

    public class PlacesResponse
    {

        public List<Prediction> Predictions { get; set; }
    }

public class Prediction
    {

        public string description { get;set; }
        public string place_id { get;set; } 
    }

    public class PlaceDetails
    {
        public Result result { get; set; }

    }
    public class Result
    {
        public Geometry geometry { get; set; }
    }
    public class Geometry
    {
        public Locations location { get; set; }

    }
    public class Locations
    {
        public string lat{ get; set; }
        public string lng { get; set; }
    }
}
