using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysCompetence")]
    public class SysCompetence : EntityBase
    {
        [Column(Name = "parent")]
        public string parent { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        [Column(Name = "competence"), NotNull]
        public string competence { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Column(Name = "describe")]
        public string describe { get; set; }
    }
}
