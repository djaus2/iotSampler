using System;
using SQLite;

namespace MySQLiteUWPApp
{
    [Table("Sensors")]
    public class Sensor
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public DateTime dateTime { get; set;}
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
