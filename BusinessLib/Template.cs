using BusinessLib.Attributes;
using BusinessLib.Authentication;
using BusinessLib.Business;
using BusinessLib.Data;
using BusinessLib.Entity;
using BusinessLib.Extensions;
using BusinessLib.Mark;
using BusinessLib.Result;
using LinqToDB;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonData = Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Template
{
    #region Data

    public class Data : BusinessLib.Data.DataBase<DataConnection>
    {
        public override DataConnection GetConnection()
        {
            return new DataConnection();
        }
    }

    public class DataConnection : BusinessLib.Data.LinqToDBConnection<Entitys>
    {
        readonly static IDataProvider provider = new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider(System.String.Empty, LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008);
        const string conString = "Server=localhost;User Id=sa;Password=test;Database=BusinessLib;";

        public DataConnection()
            : base(provider, conString) { entity = new Entitys(this); }

        readonly Entitys entity;

        public override Entitys Entity
        {
            get { return entity; }
        }
    }

    public class Entitys : BusinessLib.Data.Entitys
    {
        readonly IDataContext con;
        public Entitys(IDataContext con)
        { this.con = con; }

        public System.Linq.IQueryable<Member> Member { get { return Get<Member>(); } }

        public override IQueryable<T> Get<T>()
        {
            return this.con.GetTable<T>();
        }
    }

    #region Entitys
    [Table(Name = "Member")]
    public class Member : LinqToDBEntity
    {
        [Column(Name = "account", Length = 16, DataType = DataType.VarChar), NotNull]
        public System.String account { get; set; }

        [Column(Name = "password", Length = 32, DataType = DataType.VarChar), NotNull]
        public System.String password { get; set; }

        [Column(Name = "loginIp", Length = 32, DataType = DataType.VarChar)]
        public System.String loginIp { get; set; }

        private System.DateTime _loginDtt = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        [Column(Name = "loginDtt", Length = 23, DataType = DataType.DateTime)]
        public System.DateTime loginDtt { get { return this._loginDtt; } set { this._loginDtt = value; } }

        [Column(Name = "errorCount", Length = 10, DataType = DataType.Int32)]
        public System.Int32 errorCount { get; set; }

        [Column(Name = "number", Length = 16, DataType = DataType.VarChar)]
        public System.String number { get; set; }

        [Column(Name = "name", Length = 16, DataType = DataType.VarChar)]
        public System.String name { get; set; }

        [Column(Name = "email", Length = 32, DataType = DataType.VarChar)]
        public System.String email { get; set; }

        [Column(Name = "gold", Length = 18, Scale = 2, DataType = DataType.Decimal)]
        public System.Decimal gold { get; set; }

        [Column(Name = "integral", Length = 53, DataType = DataType.Double)]
        public System.Double integral { get; set; }

        [Column(Name = "frozen", Length = 1, DataType = DataType.Boolean)]
        public System.Boolean frozen { get; set; }

        [Column(Name = "sex", Length = 10, DataType = DataType.Int32)]
        public System.Int32 sex { get; set; }

        [Column(Name = "phone", Length = 13, DataType = DataType.VarChar)]
        public System.String phone { get; set; }

        [Column(Name = "qq", Length = 16, DataType = DataType.VarChar)]
        public System.String qq { get; set; }

        [Column(Name = "describe", Length = 1024, DataType = DataType.VarChar)]
        public System.String describe { get; set; }
    }

    #endregion

    #endregion

    #region Common

    public static class Common
    {
        public static Data OnlyDB = new Data();

        public static BusinessLib.Cache.CacheBase OnlyCache = new BusinessLib.Cache.CacheBase();

        public static BusinessLib.Log.LogBase OnlyLog = new BusinessLib.Log.LogBase(OnlyDB, OnlyCache);

        static BusinessLib.Authentication.Interceptor<ResultProtoBuf> Authentication = new BusinessLib.Authentication.Interceptor<ResultProtoBuf>(OnlyLog, OnlyCache, true);
        static BusinessLib.Authentication.InterceptorNot<ResultProtoBuf> AuthenticationNot = new BusinessLib.Authentication.InterceptorNot<ResultProtoBuf>(OnlyLog, OnlyCache, true);

        public static BusinessLib.Extensions.InterceptorBind<BusinessMember> Interceptor = new BusinessLib.Extensions.InterceptorBind<BusinessMember>(Authentication);
        public static BusinessLib.Extensions.InterceptorBind<BusinessMemberNot> InterceptorNot = new BusinessLib.Extensions.InterceptorBind<BusinessMemberNot>(AuthenticationNot);

        public static IResult<DataType> GetResult<DataType>(this DataType data)
        {
            return new ResultProtoBuf<DataType>();
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultProtoBuf : IResult
    {
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public virtual System.Int32 State { get; set; }

        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public virtual System.String Message { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public byte[] ToBytes()
        {
            return BusinessLib.Extensions.Help.ProtoBufSerialize(this);
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultProtoBuf<DataType> : ResultProtoBuf, IResult<DataType>
    {
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public override System.Int32 State { get; set; }

        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public override System.String Message { get; set; }

        [ProtoBuf.ProtoMember(3, Name = "D")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
        public DataType Data { get; set; }
    }

    #endregion

    #region UnitTest

    [TestClass]
    public class UnitTest
    {
        #region debug x86 Common.Data ...Sql To SQLServer
        static string Template(string tbl, List<System.Tuple<string, string, bool, bool, int, int, string>> cols)
        {
            var sb = new System.Text.StringBuilder(null);

            var line = System.Environment.NewLine;

            sb.AppendFormat("    [Table(Name = \"{1}\")]{0}", line, tbl);

            sb.AppendFormat("    public class {1} : LinqToDBEntity{0}", line, tbl);

            sb.AppendFormat("    {{{0}", line);

            for (int i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
                var name = System.Convert.ToString(col.Item1);
                var type = System.Convert.ToString(col.Item2);
                var pk = System.Convert.ToBoolean(col.Item3);
                var isnull = System.Convert.ToBoolean(col.Item4);
                var maxlength = System.Convert.ToInt32(col.Item5);
                var scale = System.Convert.ToInt32(col.Item6);
                var description = System.Convert.ToString(col.Item7);

                var _name = string.Format("_{0}", name);
                var _type = SqlTypeString2SqlType(type);
                var _type1 = SqlTypeString2SqlType1(type);
                if (_type == System.Data.SqlDbType.DateTime)
                {
                    sb.AppendFormat("        private System.DateTime {1} = System.Data.SqlTypes.SqlDateTime.MinValue.Value;{0}", line, _name);
                }

                //=================================================//
                if (!System.String.IsNullOrEmpty(description))
                {
                    sb.AppendFormat("        /// <summary>{0}", line);
                    sb.AppendFormat("        /// {1}{0}", line, description);
                    sb.AppendFormat("        /// </summary>{0}", line);
                }

                //var var = _type == System.Data.SqlDbType.VarChar || _type == System.Data.SqlDbType.NChar || _type == System.Data.SqlDbType.NText || _type == System.Data.SqlDbType.NVarChar || _type == System.Data.SqlDbType.Text || _type == System.Data.SqlDbType.Char;
                //CanBeNull

                sb.AppendFormat("        [{3}Column(Name = \"{1}\"{4}{5}{6}){2}]{0}", line, name, isnull ? System.String.Empty : ", NotNull", pk ? "PrimaryKey" : System.String.Empty, string.Format(", Length = {0}", maxlength), _type == System.Data.SqlDbType.Decimal ? string.Format(", Scale = {0}", scale) : System.String.Empty, string.Format(", DataType = DataType.{0}", System.Enum.GetName(typeof(DataType), _type1)));

                if (_type == System.Data.SqlDbType.DateTime)
                {
                    sb.AppendFormat("        public {2} {1} {{ get {{ return this.{3}; }} set {{ this.{3} = value; }} }}{0}", line, name, SqlType2CsharpType(_type).FullName, _name);
                }
                else
                {
                    sb.AppendFormat("        public {2} {1} {{ get; set; }}{0}", line, name, SqlType2CsharpType(_type).FullName);
                }

                if (i < cols.Count - 1) { sb.Append(line); }
            }
            //=============================================//

            sb.AppendFormat("    }}{0}", line);

            return sb.ToString();
        }
        static Type SqlType2CsharpType(System.Data.SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case System.Data.SqlDbType.BigInt:
                    return typeof(Int64);
                case System.Data.SqlDbType.Binary:
                    return typeof(Object);
                case System.Data.SqlDbType.Bit:
                    return typeof(Boolean);
                case System.Data.SqlDbType.Char:
                    return typeof(String);
                case System.Data.SqlDbType.DateTime:
                    return typeof(DateTime);
                case System.Data.SqlDbType.Decimal:
                    return typeof(Decimal);
                case System.Data.SqlDbType.Float:
                    return typeof(Double);
                case System.Data.SqlDbType.Image:
                    return typeof(Object);
                case System.Data.SqlDbType.Int:
                    return typeof(Int32);
                case System.Data.SqlDbType.Money:
                    return typeof(Decimal);
                case System.Data.SqlDbType.NChar:
                    return typeof(String);
                case System.Data.SqlDbType.NText:
                    return typeof(String);
                case System.Data.SqlDbType.NVarChar:
                    return typeof(String);
                case System.Data.SqlDbType.Real:
                    return typeof(Single);
                case System.Data.SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case System.Data.SqlDbType.SmallInt:
                    return typeof(Int16);
                case System.Data.SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case System.Data.SqlDbType.Text:
                    return typeof(String);
                case System.Data.SqlDbType.Timestamp:
                    return typeof(Object);
                case System.Data.SqlDbType.TinyInt:
                    return typeof(Byte);
                case System.Data.SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case System.Data.SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case System.Data.SqlDbType.VarBinary:
                    return typeof(Object);
                case System.Data.SqlDbType.VarChar:
                    return typeof(String);
                case System.Data.SqlDbType.Variant:
                    return typeof(Object);
                case System.Data.SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }
        static System.Data.SqlDbType SqlTypeString2SqlType(string sqlTypeString)
        {
            System.Data.SqlDbType dbType = System.Data.SqlDbType.Variant;//默认为Object

            switch (sqlTypeString)
            {
                case "int":
                    dbType = System.Data.SqlDbType.Int;
                    break;
                case "varchar":
                    dbType = System.Data.SqlDbType.VarChar;
                    break;
                case "bit":
                    dbType = System.Data.SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = System.Data.SqlDbType.DateTime;
                    break;
                case "decimal":
                    dbType = System.Data.SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = System.Data.SqlDbType.Float;
                    break;
                case "image":
                    dbType = System.Data.SqlDbType.Image;
                    break;
                case "money":
                    dbType = System.Data.SqlDbType.Money;
                    break;
                case "ntext":
                    dbType = System.Data.SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = System.Data.SqlDbType.NVarChar;
                    break;
                case "smalldatetime":
                    dbType = System.Data.SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = System.Data.SqlDbType.SmallInt;
                    break;
                case "text":
                    dbType = System.Data.SqlDbType.Text;
                    break;
                case "bigint":
                    dbType = System.Data.SqlDbType.BigInt;
                    break;
                case "binary":
                    dbType = System.Data.SqlDbType.Binary;
                    break;
                case "char":
                    dbType = System.Data.SqlDbType.Char;
                    break;
                case "nchar":
                    dbType = System.Data.SqlDbType.NChar;
                    break;
                case "numeric":
                    dbType = System.Data.SqlDbType.Decimal;
                    break;
                case "real":
                    dbType = System.Data.SqlDbType.Real;
                    break;
                case "smallmoney":
                    dbType = System.Data.SqlDbType.SmallMoney;
                    break;
                case "sql_variant":
                    dbType = System.Data.SqlDbType.Variant;
                    break;
                case "timestamp":
                    dbType = System.Data.SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = System.Data.SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    dbType = System.Data.SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    dbType = System.Data.SqlDbType.VarBinary;
                    break;
                case "xml":
                    dbType = System.Data.SqlDbType.Xml;
                    break;
            }
            return dbType;
        }
        static LinqToDB.DataType SqlTypeString2SqlType1(string sqlTypeString)
        {
            LinqToDB.DataType dbType = LinqToDB.DataType.Variant;//默认为Object

            switch (sqlTypeString)
            {
                case "int":
                    dbType = LinqToDB.DataType.Int32;
                    break;
                case "varchar":
                    dbType = LinqToDB.DataType.VarChar;
                    break;
                case "bit":
                    dbType = LinqToDB.DataType.Boolean;
                    break;
                case "datetime":
                    dbType = LinqToDB.DataType.DateTime;
                    break;
                case "decimal":
                    dbType = LinqToDB.DataType.Decimal;
                    break;
                case "float":
                    dbType = LinqToDB.DataType.Double;
                    break;
                case "image":
                    dbType = LinqToDB.DataType.Image;
                    break;
                case "money":
                    dbType = LinqToDB.DataType.Money;
                    break;
                case "ntext":
                    dbType = LinqToDB.DataType.NText;
                    break;
                case "nvarchar":
                    dbType = LinqToDB.DataType.NVarChar;
                    break;
                case "smalldatetime":
                    dbType = LinqToDB.DataType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = LinqToDB.DataType.Int16;
                    break;
                case "text":
                    dbType = LinqToDB.DataType.Text;
                    break;
                case "bigint":
                    dbType = LinqToDB.DataType.Int64;
                    break;
                case "binary":
                    dbType = LinqToDB.DataType.Binary;
                    break;
                case "char":
                    dbType = LinqToDB.DataType.Char;
                    break;
                case "nchar":
                    dbType = LinqToDB.DataType.NChar;
                    break;
                case "numeric":
                    dbType = LinqToDB.DataType.Decimal;
                    break;
                case "real":
                    dbType = LinqToDB.DataType.Single;
                    break;
                case "smallmoney":
                    dbType = LinqToDB.DataType.SmallMoney;
                    break;
                case "sql_variant":
                    dbType = LinqToDB.DataType.Variant;
                    break;
                case "timestamp":
                    dbType = LinqToDB.DataType.Timestamp;
                    break;
                case "tinyint":
                    dbType = LinqToDB.DataType.Byte;
                    break;
                //case "uniqueidentifier":
                //    dbType = LinqToDB.DataType.UniqueIdentifier;
                //    break;
                case "varbinary":
                    dbType = LinqToDB.DataType.VarBinary;
                    break;
                case "xml":
                    dbType = LinqToDB.DataType.Xml;
                    break;
            }
            return dbType;
        }
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Entitys()
        {
            var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Data\Entity");

            using (var con = CommonData.ADO.SqlHelp.GetConnection(CommonData.ADO.DataBaseType.SqlServer, "Server=localhost;User Id=sa;Password=test;Database=BusinessLib;"))
            {
                using (var dt = CommonData.ADO.SqlHelp.ExecuteTable(con, "SELECT id,name FROM sysobjects WHERE xtype='U'AND name<>'dtproperties' AND name <> 'SysAccount' AND name <> 'SysAccount_Role' AND name <> 'SysCompetence' AND name <> 'SysConfig' AND name <> 'SysLog' AND name <> 'SysLogin' AND name <> 'SysRole' AND name <> 'SysRole_Competence' order by name"))
                {
                    var count = dt.Rows.Count;
                    var tbls = new List<string>(count);
                    var sql = new System.Text.StringBuilder(null);

                    foreach (var row in dt.Rows)
                    {
                        var tbl = System.Convert.ToString(row["name"]);
                        tbls.Add(tbl);
                        var cols = new List<System.Tuple<string, string, bool, bool, int, int, string>>();

                        using (var ds = CommonData.ADO.SqlHelp.ExecuteReadOnlyDataSet(con, string.Format("select c.name,t.name,convert(bit,c.IsNullable)  as isnullable,convert(bit,case when exists(select 1 from sysobjects where xtype='PK' and parent_obj=c.id and name in (select name from sysindexes where indid in(select indid from sysindexkeys where id = c.id and colid=c.colid))) then 1 else 0 end) as pk,convert(bit,COLUMNPROPERTY(c.id,c.name,'IsIdentity')) as isidentity,c.Length as space ,COLUMNPROPERTY(c.id,c.name,'PRECISION') as maxlength,isnull(COLUMNPROPERTY(c.id,c.name,'Scale'),0) as scale,ISNULL(CM.text,'') as defaultvalue,isnull(ETP.value,'') AS description from syscolumns c inner join systypes t on c.xusertype = t.xusertype left join sys.extended_properties ETP on ETP.major_id = c.id and ETP.minor_id = c.colid and ETP.name ='MS_Description' left join syscomments CM on c.cdefault=CM.id where c.id = object_id('{0}')", tbl), null, null, null))
                        {
                            foreach (System.Data.DataRow row1 in ds.Tables[0].Rows)
                            {
                                var name = System.Convert.ToString(row1["name"]);
                                if (name == "gid" || name == "dtt" || name == "hide") { continue; }
                                var type = System.Convert.ToString(row1["name1"]);
                                var pk = System.Convert.ToBoolean(row1["pk"]);
                                var isnull = System.Convert.ToBoolean(row1["isnullable"]);
                                var maxlength = System.Convert.ToInt32(row1["maxlength"]);
                                var scale = System.Convert.ToInt32(row1["scale"]);
                                var description = System.Convert.ToString(row1["description"]);

                                cols.Add(System.Tuple.Create(name, type, pk, isnull, maxlength, scale, description));
                            }
                        }

                        sql.AppendLine(Template(tbl, cols));
                    }

                    var t = System.String.Join(System.Environment.NewLine, tbls.Select(c => string.Format("public System.Linq.IQueryable<{0}> {0} {{ get {{ return Get<{0}>(); }} }}", c))) + System.Environment.NewLine + System.Environment.NewLine;
                    sql.Insert(0, t);

                    if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }

                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, string.Format("{0}.cs", "Entitys")), sql.ToString(), System.Text.Encoding.UTF8);
                }
            }
        }

        #endregion

        static string GetToken()
        {
            var error = System.String.Empty;
            var session = new BusinessLib.Authentication.Session { Account = "admin", Password = "test", IP = "192.168", Site = "Site" }.ToString();
            var sss = Common.Interceptor.Instance.Login(session, out error);
            if (null != sss) { }
            var token = new BusinessLib.Authentication.Token { Key = sss, IP = "192.168" }.ToString();
            return token;
        }
        static string GetTokenNot()
        {
            return new BusinessLib.Authentication.TokenNot { Site = "TobaccoService", IP = "192.168" }.ToString();
        }

        [TestMethod]
        public void TestRegister()
        {
            var token = GetTokenNot();

            var result = Common.InterceptorNot.Instance.Register(token, new Template.Parameters.Register { account = "user1", password = "123456", email = "123456@sina.com" }.ToString());
        }

        [ProtoBuf.ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
        public class A { public string a { get; set; } public string b { get; set; } }

        [TestMethod]
        public void TestResult()
        {
            var token = GetTokenNot();

            var ps = new Parameters.Register() { account = "hello    ", password = " 1234 69", ddd = new List<string>() { "aaa", "bbb" }, ddd1111 = new int[] { 1, 2, 3, 4, 5 }, fff = 9999999, dttt = System.DateTime.Now };

            var rrr = Common.InterceptorNot.Instance.Test(token, ps.ToString());//database
            var rrr2 = Common.InterceptorNot.MetaData["Test"].Method(token, ps.ToString());

            var json = rrr.ToString();//out

            var bytes = rrr.ToBytes();//out

            var r1 = ResultFactory.DeserializeResultProtoBuf<System.Tuple<string, string>, ResultProtoBuf<System.Tuple<string, string>>>(bytes);
            Assert.AreEqual(r1.Data.Item1, "12121");

            var r2 = ResultFactory.DeserializeResultProtoBuf<A>(bytes);
            Assert.AreEqual(r2.Data.a, "12121");

            var r3 = ResultFactory.DeserializeResultJson<A>(json);
            Assert.AreEqual(r3.Data.a, "12121");
        }

    }

    #endregion

    #region Arguments

    public class Parameters
    {
        [Arguments(TrimAllChar = true)]
        public struct Register
        {
            [CanNotNull(Code = -11)]
            [Size(Min = 4, Max = "8", Code = 12, Message = "\"account\" length verification failed. min 4, max 8")]
            [CheckChar(Mode = Help.CheckCharMode.All, Code = -13)]
            public string account;

            [CanNotNull(Code = -14)]
            [Size(Min = 4, Max = 8, Code = 15)]
            public string password;

            [CheckEmail(Code = 16)]
            [Size(Min = 4, Max = 32, Code = -17)]
            public string email;

            //[Size(Min = 4, Max = 8, Code = -18)]
            public List<string> ddd;

            [Size(Min = 4, Max = 8, Code = -19)]
            public int[] ddd1111;

            public string dda { get; set; }
            public int fff { get; set; }
            public DateTime dttt { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }

    #endregion

    public class BusinessMember : BusinessBase<Data, BusinessLib.Log.LogBase, BusinessLib.Cache.CacheBase>
    {
        public BusinessMember()
            : base(Common.OnlyDB, Common.OnlyLog, Common.OnlyCache, () =>
            { })
        {
            this.IsRoleCompetence = true;
        }

        public string Login(string value, out string error, string data = null)
        {
            return BusinessLib.Business.BusinessHelp.Login(this.DB, this.Log, this.Cache, value, out error, data, (out System.String _error, System.String session, ref System.String _account, System.String _password, System.String _ip, System.DateTime time, System.String _data) =>
            {
                _error = System.String.Empty;
                var _account1 = _account;
                //=============================================//
                var errorCount = MarkBase.GetObject<int>(MarkEnum.Login_ErrorCount);// 3
                var errorRange = MarkBase.GetObject<int>(MarkEnum.Login_ErrorRange);// 2
                var errorFreeze = MarkBase.GetObject<int>(MarkEnum.Login_ErrorFreeze);// 3
                var strFreeze = string.Format("{0}{1} Minutes", MarkBase.GetObject<string>(MarkEnum.Exp_UserFrozen), errorFreeze);
                var result = -99;

                using (var con = this.DB.GetConnection())
                {
                    var member = con.Entity.Member.Where(c => c.account == _account1).ToList();
                    switch (member.Count)
                    {
                        case 0: _error = MarkBase.GetObject<string>(MarkEnum.Exp_UserError); break;
                        case 1:
                            var sysLogin = con.Entity.Get<SysLogin>().Where(c => c.account == _account1).OrderByDescending(c => c.dtt).Take(errorCount).ToList();

                            if (0 < sysLogin.Count && -1 == sysLogin[0].result && 0 > time.CompareTo(sysLogin[0].dtt.AddMinutes(errorFreeze)))
                            {
                                _error = strFreeze;
                                result = -2;//freeze ing...
                                break;
                            }

                            if (!System.Object.Equals(member[0].password, _password))
                            {
                                _error = MarkBase.GetObject<string>(MarkEnum.Exp_PasswordError);
                                result = 0;//password error

                                var c = sysLogin.FindIndex(l => 0 != l.result);
                                var range = sysLogin.Take(-1 == c ? sysLogin.Count : c);
                                c = range.Count(l => 0 < l.dtt.CompareTo(time.Subtract(TimeSpan.FromMinutes(errorRange))));
                                member[0].errorCount = 0 == c ? 1 : member[0].errorCount + 1;
                                if ((errorCount - 1) == c)
                                {
                                    _error = strFreeze;
                                    result = -1;//ready freeze
                                }
                            }
                            else
                            {
                                if (member[0].frozen)
                                {
                                    result = -2;//freeze ing...
                                    break;
                                }
                                result = 1;//OK
                            }
                            break;
                    }

                    //write data
                    if (1 == member.Count)
                    {
                        con.BeginTransaction();

                        member[0].loginIp = _ip;
                        member[0].loginDtt = time;
                        if (1 == result) { member[0].errorCount = 0; }
                        if (0 == con.Update(member[0])) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysAccount).Name); }
                        if (-2 != result)//freeze ing...
                        {
                            if (0 == con.Save(new SysLogin { category = 0, session = session, account = _account1, ip = _ip, data = _data, result = result, describe = _error, dtt = time })) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysLogin).Name); }
                        }
                        con.Commit();
                    }
                }
                return 1 == result;
            });
        }

        public virtual IResult Test(string token, object arguments = null, BusinessLib.Authentication.ISession session = null, Parameters.Register ags = default(Parameters.Register))
        {
            if (ags.account == null) { }

            var data = new { a = "12121", b = "321321" };
            return ResultFactory.Create(data, data.GetResult(), 33);
        }
    }

    public class BusinessMemberNot : BusinessBase<Data, BusinessLib.Log.LogBase, BusinessLib.Cache.CacheBase>
    {
        public BusinessMemberNot()
            : base(Common.OnlyDB, Common.OnlyLog, Common.OnlyCache, () =>
            { })
        {
        }

        /// <summary>
        /// error code -12 to -18
        /// </summary>
        /// <param name="token"></param>
        /// <param name="arguments"></param>
        /// <param name="ags"></param>
        /// <returns></returns>
        public virtual IResult Register(string token, object arguments, Parameters.Register ags = default(Parameters.Register))
        {
            //check account
            using (var con = this.DB.GetConnection())
            {
                var query = from c in con.Entity.Member
                            where c.account == ags.account
                            select c.account;
                if (0 < query.Count()) { return ResultFactory.Create(-17, "User already exists"); }

                var query1 = from c in con.Entity.Member
                             where c.email == ags.email
                             select c.account;
                if (0 < query1.Count()) { return ResultFactory.Create(-18, "Email already exists"); }

                //check account number
                var nuid = System.String.Empty;
                IQueryable<Member> query2;
                do
                {
                    nuid = BusinessLib.Extensions.Help.NewGuidNumber();
                    query2 = from c in con.Entity.Member
                             where c.number == nuid
                             select c;
                } while (0 < query2.Count());

                //wait
                var member = new Member()
                {
                    account = ags.account,
                    password = ags.password,
                    email = ags.email,
                    number = nuid
                };
                if (0 >= this.DB.Save(member)) { throw new System.Data.DBConcurrencyException(); }

                //return
                return ResultFactory.Create();
            }
        }

        public virtual IResult Test(string token, object arguments = null, BusinessLib.Authentication.ISession session = null, Parameters.Register ags = default(Parameters.Register))
        {
            if (ags.account == null) { }

            var data = new { a = "12121", b = "321321" };
            return ResultFactory.Create(data, data.GetResult(), 33);
        }

        public virtual IResult Test(string token)
        {
            return ResultFactory.Create();
        }
        public virtual IResult Test2(string token, object arguments = null)
        {
            return ResultFactory.Create();
        }
        public virtual IResult Test3(string token, object arguments = null, BusinessLib.Authentication.ISession session = null)
        {
            return ResultFactory.Create();
        }
    }
}
