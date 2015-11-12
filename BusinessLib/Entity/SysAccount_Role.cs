using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysAccount_Role")]
    public class SysAccount_Role : EntityBase
    {
        [Column(Name = "account"), NotNull]
        public string account { get; set; }

        [Column(Name = "role"), NotNull]
        public string role { get; set; }
    }
}
