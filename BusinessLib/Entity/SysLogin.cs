using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysLogin")]
    public class SysLogin : EntityBase
    {
        [Column(Name = "session"), NotNull]
        public string session { get; set; }

        [Column(Name = "account"), NotNull]
        public string account { get; set; }

        [Column(Name = "category")]
        public int category { get; set; }

        [Column(Name = "ip")]
        public string ip { get; set; }

        [Column(Name = "data")]
        public System.String data { get; set; }

        [Column(Name = "area")]
        public string area { get; set; }

        [Column(Name = "result")]
        public int result { get; set; }

        [Column(Name = "describe")]
        public string describe { get; set; }
    }
}
