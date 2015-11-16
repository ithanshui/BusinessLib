using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysRole_Competence")]
    public class SysRole_Competence : LinqToDBEntity
    {
        [Column(Name = "role"), NotNull]
        public string role { get; set; }

        [Column(Name = "competence"), NotNull]
        public string competence { get; set; }
    }
}
