using LinqToDB.Mapping;
namespace BusinessLib.Entity
{
    public class EntityBase
    {
        private System.String _gid = System.Guid.NewGuid().ToString("N");
        //[Newtonsoft.Json.JsonIgnore]
        [PrimaryKey, Column(Name = "gid"), NotNull]
        public System.String gid { get { return this._gid; } set { this._gid = value; } }
        private System.DateTime _dtt = System.DateTime.Now;
        [Column(Name = "dtt")]
        public System.DateTime dtt { get { return this._dtt; } set { this._dtt = value; } }
        [Column(Name = "hide")]
        public System.Boolean hide { get; set; }
    }
}