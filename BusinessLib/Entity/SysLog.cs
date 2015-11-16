using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysLog")]
    public class SysLog : LinqToDBEntity
    {
        [Column(Name = "type"), NotNull]
        public System.Int32 type { get; set; }
        [Column(Name = "session")]
        public string session { get; set; }
        [Column(Name = "account")]
        public System.String account { get; set; }
        [Column(Name = "member"), NotNull]
        public System.String member { get; set; }
        [Column(Name = "value"), NotNull]
        public System.String value { get; set; }
        [Column(Name = "result")]
        public System.String result { get; set; }
        [Column(Name = "time")]
        public System.Double time { get; set; }
        [Column(Name = "ip")]
        public System.String ip { get; set; }
        [Column(Name = "describe")]
        public System.String describe { get; set; }
    }
}
