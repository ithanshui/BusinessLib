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
        /// <summary>
        /// 角色名称
        /// </summary>
        [Column(Name = "role")]
        public string role { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Column(Name = "describe")]
        public string describe { get; set; }
    }
}
