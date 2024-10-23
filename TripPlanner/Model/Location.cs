using SQLite;

namespace TripPlanner.Model
{
    [Table("Location")]
    public class Location
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Place_Id { get; set; }

       

        public Location()
        {
            Name = "";
            Place_Id = "";
           
        }

        public Location( string name, string place)
        {
            
            Name = name;
            Place_Id = place;
           
        }

    }
}
