using SQLite;

namespace SimpleTodo.Models
{
    [Table("filmek")]
    public class Film
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string cim { get; set; }
        public string mufaj { get; set; }
        public string korhatar { get; set; }
        public string vetites_datum { get; set; }
    }
}
