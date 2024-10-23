using Android.Gms.Common.Api.Internal;
using Java.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Model;
using TripPlanner.Services;

namespace TripPlanner.Test
{
    public class ReportTesting
    {
       
        public ReportTesting() {

           
        }
       
        public static async Task loadTestingData1() {
            await DatabaseService.ClearDatabase();
            await DatabaseService.AddTrip("Iceland Trip", new DateTime(2024, 05, 01), new DateTime(2024, 05, 28));
            await DatabaseService.AddTrip("Mexico Trip", new DateTime(2024, 10, 10), new DateTime(2024, 10, 26));
            await DatabaseService.AddTrip("Cali Road Trip", new DateTime(2024, 11, 22), new DateTime(2024, 11, 30));
            Debug.WriteLine(DateTime.Now + "----------Testing------------Test data 1 loaded");
            
        }

        public static async Task loadTestingData2()
        {
            
            await DatabaseService.ClearDatabase();
            await DatabaseService.AddTrip("Iceland Trip", new DateTime(2024, 05, 01), new DateTime(2024, 05, 28));
            await DatabaseService.AddTrip("Mexico Trip", new DateTime(2025, 10, 10), new DateTime(2025, 10, 26));
            await DatabaseService.AddTrip("Cali Road Trip", new DateTime(2024, 11, 22), new DateTime(2024, 11, 30));
            Debug.WriteLine(DateTime.Now + "----------Testing------------Test data 2 loaded");
        }

        public static async Task TestResult1(string year,List<Trip> trips)
        {
            bool result = true;
            List<Trip> expectedTrips1 = [ new Trip("Iceland Trip", new DateTime(2024, 05, 01), new DateTime(2024, 05, 28)),
                new Trip("Mexico Trip", new DateTime(2024, 10, 10), new DateTime(2024, 10, 26)),
                new Trip("Cali Road Trip", new DateTime(2024, 11, 22), new DateTime(2024, 11, 30))];
                

            if (year.Equals("2024"))                        //First User Input Test
            {
                if (trips.Count == expectedTrips1.Count)
                {
                    foreach (var trip in trips)
                    {
                        Trip expectedTrip = expectedTrips1.ElementAt(trips.IndexOf(trip));
                        if (!expectedTrip.Title.Equals(trip.Title) | !expectedTrip.StartDate.Equals(trip.StartDate) | !expectedTrip.EndDate.Equals(trip.EndDate))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                else
                    result = false;
                string message = result ? "Test 1 Passed" : "Test 1 Failed";

                Debug.WriteLine(DateTime.Now + "----------Testing------------Test Data 1: Test Case 1---------------- \n" + message);
            }
            else if (year.Equals("2025"))
            {
                if (trips.Count == 0)
                    result = true;
                else
                    result = false;
                string message = result ? "Test 2 Passed" : "Test 2 Failed";

                Debug.WriteLine(DateTime.Now + "----------Testing------------Test Data 1: Test Case 2---------------- \n" + message);
            }

           
        }
        public static async Task TestResult2(string year, List<Trip> trips)
        {
            
            bool result = true;
            List<Trip> expectedTrips1 = [ new Trip("Iceland Trip", new DateTime(2024, 05, 01), new DateTime(2024, 05, 28)),
                new Trip("Cali Road Trip", new DateTime(2024, 11, 22), new DateTime(2024, 11, 30))];
            List<Trip> expectedTrips2 = [ new Trip("Mexico Trip", new DateTime(2025, 10, 10), new DateTime(2025, 10, 26)) ];

            if (year.Equals("2024"))                        //First User Input Test
            {
                if (trips.Count == expectedTrips1.Count)
                {
                    foreach (var trip in trips)
                    {
                        Trip expectedTrip = expectedTrips1.ElementAt(trips.IndexOf(trip));
                        if (!expectedTrip.Title.Equals(trip.Title) | !expectedTrip.StartDate.Equals(trip.StartDate) | !expectedTrip.EndDate.Equals(trip.EndDate))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                else
                    result = false;
                string message = result ? "Test 1 Passed" : "Test 1 Failed";

                Debug.WriteLine(DateTime.Now + "----------Testing------------Test Data 2: Test Case 1---------------- \n" + message);
            }
            else if (year.Equals("2025"))
            {
                if (trips.Count == expectedTrips2.Count)
                {
                    foreach (var trip in trips)
                    {
                        Trip expectedTrip = expectedTrips2.ElementAt(trips.IndexOf(trip));
                        if (!expectedTrip.Title.Equals(trip.Title) | !expectedTrip.StartDate.Equals(trip.StartDate) | !expectedTrip.EndDate.Equals(trip.EndDate))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                else
                    result = false;
                string message = result ? "Test 2 Passed" : "Test 2 Failed";

                Debug.WriteLine(DateTime.Now + "----------Testing------------Test Data 2: Test Case 2---------------- \n" + message);
            }


        
    }
    }
}
