using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysRole_Competence")]
    public class SysRole_Competence : EntityBase
    {
        /// <summary>
        /// 角色gid
        /// </summary>
        [Column(Name = "role"), NotNull]
        public string role { get; set; }
        /// <summary>
        /// 权限gid
        /// </summary>
        [Column(Name = "competence"), NotNull]
        public string competence { get; set; }
    }
}
