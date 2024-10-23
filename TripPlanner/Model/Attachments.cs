using SQLite;

namespace TripPlanner.Model
{
    [Table("Attachments")]
    public class Attachments
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int EntryId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }


        public Attachments()
        {
            Name = "New Assessment";
            Path = "";
        }

        public Attachments(int entryId, string name, string path)
        {
            EntryId = entryId;
            Name = name;
            Path = path;
        }

    }
}
