using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysCompetence")]
    public class SysCompetence : LinqToDBEntity
    {
        [Column(Name = "parent")]
        public string parent { get; set; }

        [Column(Name = "competence"), NotNull]
        public string competence { get; set; }

        [Column(Name = "describe")]
        public string describe { get; set; }
    }
}
