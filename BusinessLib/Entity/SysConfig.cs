using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysConfig")]
    public class SysConfig : EntityBase
    {
        [Column(Name = "type"), NotNull]
        public System.Int32 type { get; set; }
        [Column(Name = "childType")]
        public System.Int32 childType { get; set; }
        [Column(Name = "name"), NotNull]
        public System.String name { get; set; }
        [Column(Name = "value"), NotNull]
        public System.String value { get; set; }
        [Column(Name = "describe")]
        public System.String describe { get; set; }
    }
}
