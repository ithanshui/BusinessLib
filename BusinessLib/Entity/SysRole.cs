using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysRole")]
    public class SysRole : EntityBase
    {
        [Column(Name = "account")]
        public string account { get; set; }

        [Column(Name = "parent")]
        public string parent { get; set; }

        [Column(Name = "role")]
        public string role { get; set; }

        [Column(Name = "describe")]
        public string describe { get; set; }
    }
}
