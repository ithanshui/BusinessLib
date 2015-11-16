using LinqToDB.Mapping;

namespace BusinessLib.Entity
{
    public class LinqToDBEntity : EntityBase
    {
        private System.String _gid = System.Guid.NewGuid().ToString("N");
        [PrimaryKey, Column(Name = "gid"), NotNull]
        public override System.String gid { get { return this._gid; } set { this._gid = value; } }

        private System.DateTime _dtt = System.DateTime.Now;
        [Column(Name = "dtt")]
        public override System.DateTime dtt { get { return this._dtt; } set { this._dtt = value; } }

        [Column(Name = "hide")]
        public override System.Boolean hide { get; set; }
    }
}