using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    [Table(Name = "SysAccount")]
    public class SysAccount : LinqToDBEntity
    {
        [Column(Name = "parent")]
        public string parent { get; set; }

        [Column(Name = "account"), NotNull]
        public string account { get; set; }

        [Column(Name = "password"), NotNull]
        public string password { get; set; }

        [Column(Name = "loginIp")]
        public string loginIp { get; set; }

        private System.DateTime _loginDtt = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        [Column(Name = "loginDtt")]
        public System.DateTime loginDtt { set { this._loginDtt = value; } get { return this._loginDtt; } }

        [Column(Name = "errorCount")]
        public int errorCount { get; set; }

        [Column(Name = "frozen")]
        public bool frozen { get; set; }

        [Column(Name = "securityCode")]
        public string securityCode { get; set; }
    }
}
