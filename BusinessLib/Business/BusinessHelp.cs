using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLib.Business
{
    using BusinessLib.Entity;
    using BusinessLib.Extensions;
    using BusinessLib.Result;
    using Newtonsoft.Json;
    using BusinessLib.Data;
    using BusinessLib.Mark;

    public static class BusinessHelp
    {
        public delegate TResult LoginFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(out T1 error, T2 session, ref T3 account, T4 password, T5 ip, T6 time, T7 data);
        /// <summary>
        /// 示例登陆方法。内置算法，请勿更改。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string Login(BusinessLib.Data.IData db, BusinessLib.Log.ILog log, BusinessLib.Cache.ICache cache, string value, out string error, string data, LoginFunc<System.String, System.String, System.String, System.String, System.String, System.DateTime, System.String, System.Boolean> member)
        {
            error = System.String.Empty;

            var time = System.DateTime.Now;//set server current time

            try
            {
                //========================== session begin //==========================//
                if (System.String.IsNullOrEmpty(value)) { return System.String.Empty; }
                var session = JsonConvert.DeserializeObject<BusinessLib.BasicAuthentication.Session>(value);

                //check time
                /*
                var seconds = System.DateTime.Now.Subtract(time).TotalSeconds;
                if (-10 > seconds || seconds >= 0) { error = ErrorLogin2; return System.String.Empty; }
                */
                //check site

                //if (!cache.Contains(session.Site)) { error = MarkBase.GetObject<string>(MarkEnum.Exp_SiteIllegal); return System.String.Empty; }
                //session.Site = cache.Get<string>(session.Site);
                //========================== session end //==========================//

                //send value-key
                var key = System.Guid.NewGuid().ToString("N");
                session.Key = string.Format("Session_{0}", key);

                switch (session.Site)
                {
                    //system account
                    case "0":
                        #region ^^^^^^^^^^^^^^^

                        var errorCount = MarkBase.GetObject<int>(MarkEnum.Login_ErrorCount);// 3 次数
                        var errorRange = MarkBase.GetObject<int>(MarkEnum.Login_ErrorRange);// 2 分钟
                        var errorFreeze = MarkBase.GetObject<int>(MarkEnum.Login_ErrorFreeze);// 3 分钟
                        var strFreeze = string.Format("{0}{1} Minutes", MarkBase.GetObject<string>(MarkEnum.Exp_UserFrozen), errorFreeze);
                        var result = -99;

                        using (var con = db.GetConnection())
                        {
                            var sysAccount = con.Entity.Get<SysAccount>().Where(c => c.account == session.Account).ToList();
                            switch (sysAccount.Count)
                            {
                                case 0: error = MarkBase.GetObject<string>(MarkEnum.Exp_UserError); break;
                                case 1:
                                    var sysLogin = con.Entity.Get<SysLogin>().Where(c => c.account == session.Account).OrderByDescending(c => c.dtt).Take(errorCount).ToList();

                                    if (0 < sysLogin.Count && -1 == sysLogin[0].result && 0 > time.CompareTo(sysLogin[0].dtt.AddMinutes(errorFreeze)))
                                    {
                                        error = strFreeze;
                                        result = -2;//冻结中
                                        break;
                                    }

                                    if (!System.Object.Equals(sysAccount[0].password, session.Password) || !System.Object.Equals(sysAccount[0].securityCode, session.SecurityCode))
                                    {
                                        error = MarkBase.GetObject<string>(MarkEnum.Exp_PasswordError);
                                        result = 0;//密码错误

                                        var c = sysLogin.FindIndex(l => 0 != l.result);
                                        var range = sysLogin.Take(-1 == c ? sysLogin.Count : c);
                                        c = range.Count(l => 0 < l.dtt.CompareTo(time.Subtract(TimeSpan.FromMinutes(errorRange))));
                                        sysAccount[0].errorCount = 0 == c ? 1 : sysAccount[0].errorCount + 1;
                                        if ((errorCount - 1) == c)
                                        {
                                            error = strFreeze;
                                            result = -1;//准备冻结
                                        }
                                    }
                                    else
                                    {
                                        result = 1;//成功

                                        session.RoleCompetence = GetRoleCompetence(cache, sysAccount[0].parent, sysAccount[0].gid, sysAccount[0].account);
                                    }
                                    break;
                            }

                            //write data
                            if (1 == sysAccount.Count)
                            {
                                //数据操作示例
                                con.BeginTransaction();

                                sysAccount[0].loginIp = session.IP;
                                sysAccount[0].loginDtt = time;
                                if (1 == result) { sysAccount[0].errorCount = 0; }
                                if (0 == con.Update(sysAccount[0])) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysAccount).Name); }
                                if (-2 != result)//冻结中
                                {
                                    if (0 == con.Save(new SysLogin { category = 0, session = session.Key, account = session.Account, ip = session.IP, data = data, result = result, describe = error, dtt = time })) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysLogin).Name); }
                                }
                                con.Commit();
                            }
                        }

                        if (1 != result) { return System.String.Empty; }
                        #endregion
                        break;
                    //member account
                    //case "1":
                    //    var account = session.Account;
                    //    if (!member.Invoke(out error, ref account, session.Password, session.IP, time)) { return System.String.Empty; }
                    //    session.Account = account;
                    //    break;
                    default:
                        var account = session.Account;
                        if (!member.Invoke(out error, session.Key, ref account, session.Password, session.IP, time, data)) { return System.String.Empty; }
                        session.Account = account;
                        break;
                }

                /*
                session.Key = string.Format("Session_{0}", session.Account);

                var _session = cache.Get<BusinessLib.BasicAuthentication.Session>(session.Key);
                if (null != _session)
                {
                    var _time = System.Convert.ToInt64(_session.Time.Split('_')[0]).ConvertTime();
                    if (0 > time.Subtract(_time).CompareTo(System.TimeSpan.FromSeconds(10)))
                    {
                        error = MarkBase.GetObject<string>(MarkEnum.Exp_LoginTimeSpace); 
                        return System.String.Empty;
                    }
                }
                */
                //=============================== end ================================//
                //session.Time
                session.Time = time;// string.Format("{0}_{1}", time.ConvertTime().ToString(), System.Guid.NewGuid().ToString("N"));
                cache.SetSession(session);
                //=============================//

                //cache.FirstOrDefault(c => c.Key.StartsWith(string.Format("Session_{0}", key)));
                //var session1 = new BasicAuthentication.Session1 { Account = string.Format("Session_{0}", session.Account), SessionKey = session.Key };
                //cache.SetSession1(session1);
                //cache.RemovedHandler += new RemovedCallback<string, object>();
                //=============================//
                //return string.Format("{0}|{1}", string.Format("{0}{1}", session.Key, session.Time).MD5Encoding(), session.Key);
                return key;
            }
            catch (System.Exception ex)
            {
                log.WriteAsync(BusinessLib.Log.LogType.Exception, "Sys", value, ex, System.DateTime.Now.Subtract(time).TotalSeconds, description: "Login");

                error = JsonConvert.SerializeObject(ex);//allow hide or show!

                return System.String.Empty;//hide to log!
            }
        }

        public static string Logout(string token, BusinessLib.Cache.ICache cache)
        {
            var _token = token.GetToken();
            if (null == _token) { return ResultExtensions.Result(MarkEnum.Exp_SessionIllegal).ToString(); }

            cache.Remove(_token.Key);
            return ResultExtensions.Result("OK").ToString();
        }

        public static BusinessLib.BasicAuthentication.ITokenNot GetTokenNot(this string token)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return null;
            }

            var _token = Help.JsonDeserialize<BusinessLib.BasicAuthentication.TokenNot>(token);
            if (default(BusinessLib.BasicAuthentication.TokenNot).Equals(_token))
            {
                return null;
            }

            return _token;
        }

        public static BusinessLib.BasicAuthentication.IToken GetToken(this string token)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return null;
            }

            var _token = Help.JsonDeserialize<BusinessLib.BasicAuthentication.Token>(token);
            if (default(BusinessLib.BasicAuthentication.Token).Equals(_token))
            {
                return null;
            }

            return _token;
        }

        public static BusinessLib.BasicAuthentication.ISession GetSession(this BusinessLib.BasicAuthentication.IToken token, BusinessLib.Cache.ICache cache)
        {//<BusinessLib.BasicAuthentication.Session>
            return null == token ? null : cache.Get(token.Key) as BusinessLib.BasicAuthentication.ISession;
        }
        //-----------------------------?????????????????????
        //public static BusinessLib.BasicAuthentication.ISession GetSession(this string token, BusinessLib.Cache.ICache cache)
        //{
        //    var _token = token.GetToken();
        //    return null == _token ? null : _token.GetSession(cache);
        //}
        public static void SetSession(this BusinessLib.Cache.ICache cache, BusinessLib.BasicAuthentication.ISession session)
        {
            cache.Set(session.Key, session, TimeSpan.FromMinutes(MarkBase.GetObject<int>(MarkEnum.OutTime_Session_Time)));
        }

        public static void UpdateRoleCompetence(this BusinessLib.BasicAuthentication.ISession session, BusinessLib.Cache.ICache cache)
        {
            session.RoleCompetence = GetRoleCompetence(cache, session.RoleCompetence.Account.Parent, session.RoleCompetence.Account.Key, session.RoleCompetence.Account.Account);
            cache.SetSession(session);

            if (null == session.RoleCompetence.AccountAll) { return; }

            foreach (var item in cache.Where(c => c.Key.StartsWith("Session_") && session.RoleCompetence.Account.Key == ((BusinessLib.BasicAuthentication.ISession)c.Value).RoleCompetence.Account.Parent))
            {
                if (!default(System.Collections.Generic.KeyValuePair<string, object>).Equals(item))
                {
                    ((BusinessLib.BasicAuthentication.ISession)item.Value).UpdateRoleCompetence(cache);
                }
            }
        }

        public static BusinessLib.BasicAuthentication.IToken GetToken(string value, string ip)
        {
            return value.JsonDeserialize<BusinessLib.BasicAuthentication.IToken>() ?? new BusinessLib.BasicAuthentication.Token { Key = value, IP = ip };
        }

        public static string SysInfoDecode(string data)
        {
            try
            {
                var decode = System.Text.Encoding.UTF8.GetString(BusinessLib.Extensions.Help.RSADecrypt(System.Convert.FromBase64String(data)));
                //sysinfo
                var sysInfo = BusinessLib.Extensions.Help.JsonDeserialize<BusinessLib.BasicAuthentication.SysInfo>(decode);
                return sysInfo.ToString();
            }
            catch (System.Exception ex)
            {
                return string.Format("{1}{0}======================================================={0}{2}", System.Environment.NewLine, System.Convert.ToString(ex), data);
            }
        }

        /*
        public static void MailSendBLNew(this string subject, string content, string from = "noreply@mail.shuaxinyong.net", string displayName = "百利网", string host = "smtpcloud.sohu.com", int port = 25, bool enableSsl = false, string credentialsUserName = "postmaster@shuaxinyong.sendcloud.org", string credentialsPassword = "U59BAQJXbeniArm1", params string[] to)
        {
            subject.MailSend(content, from, displayName, host, credentialsUserName, credentialsPassword, port, enableSsl, to: to);
        }
        */
        #region RoleCompetence

        #region base

        /*
        public static void GetRoleCompetence(BusinessLib.Data.IData db, string accountKey, ref BusinessLib.BasicAuthentication.RoleCompetence roleCompetence, out List<BusinessLib.BasicAuthentication.Accounts> accountAll, out List<BusinessLib.BasicAuthentication.RoleCompetence> roleCompetencAll)
        {
            #region RoleCompetence
//            
//SELECT SysAccount_Role.gid AS account_Role_gid, Role.gid AS role_gid, Role.account AS role_account, Role.parent AS role_parent, Role.role, Role.remark AS role_remark, Role_Competence.gid AS role_Competence_gid, Competence.gid AS competence_gid, Competence.parent AS competence_parent, Competence.competence, Competence.remark AS competence_remark
//FROM SysAccount_Role
//LEFT JOIN SysAccount ON SysAccount.gid = SysAccount_Role.sysAccount
//LEFT JOIN Role ON Role.gid = SysAccount_Role.role
//LEFT JOIN Role_Competence ON Role.gid = Role_Competence.role
//LEFT JOIN Competence ON Competence.gid = Role_Competence.competence
//WHERE SysAccount.gid = '1111'
//GROUP BY Competence.competence
//            
            using (var con = data.GetConnection())
            {
                var roleCompetenceView = con.GetData<RoleCompetenceView>("SysAccount_Role LEFT JOIN SysAccount ON SysAccount.gid = SysAccount_Role.account LEFT JOIN Role ON Role.gid = SysAccount_Role.role LEFT JOIN Role_Competence ON Role.gid = Role_Competence.role LEFT JOIN Competence ON Competence.gid = Role_Competence.competence", "SysAccount_Role.gid AS account_Role_gid, Role.gid AS role_gid, Role.account AS role_account, Role.parent AS role_parent, Role.role, Role.remark AS role_remark, Role_Competence.gid AS role_Competence_gid, Competence.gid AS competence_gid, Competence.parent AS competence_parent, Competence.competence, Competence.remark AS competence_remark", "SysAccount.gid = @account", parameter: con.GetParameter("@account", accountKey));
                //competences
                if (roleCompetenceView.Exists(c => !System.String.IsNullOrEmpty(c.competence_gid))) { roleCompetence.CompetenceAll = new List<BasicAuthentication.Competences>(); }
                //role
                var _roles = roleCompetenceView.GroupBy(g => g.role_gid);
                if (0 < _roles.Count()) { roleCompetence.Roles = new List<BasicAuthentication.Roles>(); roleCompetence.RoleAll = new List<BasicAuthentication.Roles>(); }
                foreach (var item in _roles)
                {
                    var _role = roleCompetenceView.Find(r => item.Key == r.role_gid);
                    var role = new BasicAuthentication.Roles { SysAccount_Role_gid = _role.account_Role_gid, CreateKey = _role.role_account, AccountKey = accountKey, Key = item.Key, Parent = _role.role_parent, Role = _role.role, Remark = _role.role_remark };//IsChild
                    //competence
                    var competences = item.Where(c => !System.String.IsNullOrEmpty(c.competence_gid)).GroupBy(g => g.role_Competence_gid);
                    if (0 < competences.Count()) { role.Competences = new List<BasicAuthentication.Competences>(); }
                    foreach (var item1 in competences)
                    {
                        var _competence = roleCompetenceView.Find(r => item1.Key == r.role_Competence_gid);
                        var competence = new BasicAuthentication.Competences { Role_Competence_gid = _competence.role_Competence_gid, Role = _competence.role_gid, Key = _competence.competence_gid, Parent = _competence.competence_parent, Competence = _competence.competence, Remark = _competence.competence_remark };
                        role.Competences.Add(competence);
                        roleCompetence.CompetenceAll.Add(competence);
                    }
                    //add
                    roleCompetence.Roles.Add(role);
                    roleCompetence.RoleAll.Add(role);
                }
                accountAll = new List<BasicAuthentication.Accounts>();
                roleCompetencAll = new List<BasicAuthentication.RoleCompetence>();
                //account
                var _accounts = con.GetData<SysAccount>("SysAccount", "gid, parent, account", "parent = @account", parameter: con.GetParameter("@account", accountKey));
                if (0 < _accounts.Count)
                {
                    roleCompetence.RoleCompetences = new List<BasicAuthentication.RoleCompetence>();
                    roleCompetence.Account = new BasicAuthentication.Accounts { Parent = roleCompetence.Account.Parent, Key = roleCompetence.Account.Key, Account = roleCompetence.Account.Account, IsChild = true };//IsChild
                    //roleCompetence.AccountAll = new List<BasicAuthentication.Accounts> { roleCompetence.Account };
                    //roleCompetence.RoleCompetenceAll = new List<BasicAuthentication.RoleCompetence> { roleCompetence };
                    roleCompetence.AccountAll = new List<BasicAuthentication.Accounts>();
                    roleCompetence.RoleCompetenceAll = new List<BasicAuthentication.RoleCompetence>();
                }

                foreach (var item in _accounts)
                {
                    var account = new BasicAuthentication.Accounts { Parent = item.parent, Key = item.gid, Account = item.account };
                    var _roleCompetence = new BusinessLib.BasicAuthentication.RoleCompetence { Account = account };
                    GetRoleCompetence(data, _roleCompetence.Account.Key, ref _roleCompetence, out accountAll, out roleCompetencAll);
                    roleCompetence.RoleCompetences.Add(_roleCompetence);

                    if (null != _roleCompetence.RoleAll)
                    {
                        if (null == roleCompetence.RoleAll) { roleCompetence.RoleAll = new List<BasicAuthentication.Roles>(); }
                        roleCompetence.RoleAll.AddRange(_roleCompetence.RoleAll);
                    }
                    //====================Account====================//
                    if (_roleCompetence.Account.IsChild) { account.IsChild = true; }
                    accountAll.Add(account);
                    roleCompetence.AccountAll.AddRange(accountAll);
                    //====================RoleCompetencAll====================//
                    roleCompetencAll.Add(_roleCompetence);
                    roleCompetence.RoleCompetenceAll.AddRange(roleCompetencAll);
                }
                //if (0 < accountAll.Count)
                //{
                //    roleCompetence.AccountAll.AddRange(accountAll);
                //}
                //if (0 < accountAll.Count)
                //{
                //    roleCompetence.AccountAll = new List<BasicAuthentication.Accounts>();
                //    roleCompetence.AccountAll.AddRange(accountAll);
                //}
                //if (0 < roleChild.Count)
                //{
                //    if (null == roleCompetence.RoleAll) { roleCompetence.RoleAll = new List<BasicAuthentication.Roles>(); }
                //    roleCompetence.RoleAll.AddRange(roleChild);
                //}
            }

            #endregion
        }

        public static BusinessLib.BasicAuthentication.RoleCompetence GetRoleCompetence(BusinessLib.Data.IData db, string accountParent, string accountKey, string account)
        {
            var roleCompetence = new BasicAuthentication.RoleCompetence { Account = new BasicAuthentication.Accounts { Parent = accountParent, Key = accountKey, Account = account } };
            List<BusinessLib.BasicAuthentication.RoleCompetence> roleCompetencAll;
            List<BasicAuthentication.Accounts> accountAll;
            GetRoleCompetence(data, roleCompetence.Account.Key, ref roleCompetence, out accountAll, out roleCompetencAll);
            return roleCompetence;
        }
        */

        public static void GetRoleCompetence(System.Collections.Specialized.ListDictionary entitys, string accountKey, ref BusinessLib.BasicAuthentication.RoleCompetence roleCompetence, out List<BusinessLib.BasicAuthentication.Accounts> accountAll, out List<BusinessLib.BasicAuthentication.RoleCompetence> roleCompetencAll)
        {
            #region RoleCompetence
            /*
SELECT SysAccount_Role.gid AS account_Role_gid, Role.gid AS role_gid, Role.account AS role_account, Role.parent AS role_parent, Role.role, Role.remark AS role_remark, Role_Competence.gid AS role_Competence_gid, Competence.gid AS competence_gid, Competence.parent AS competence_parent, Competence.competence, Competence.remark AS competence_remark
FROM SysAccount_Role
LEFT JOIN SysAccount ON SysAccount.gid = SysAccount_Role.sysAccount
LEFT JOIN Role ON Role.gid = SysAccount_Role.role
LEFT JOIN Role_Competence ON Role.gid = Role_Competence.role
LEFT JOIN Competence ON Competence.gid = Role_Competence.competence
WHERE SysAccount.gid = '1111'
GROUP BY Competence.competence
            */

            var sysAccount = entitys[MarkBase.GetObject<string>(MarkEnum.R_SysAccount)] as List<Entity.SysAccount>;

            var roleCompetenceView = GetRoleCompetenceView(accountKey, sysAccount,
                entitys[MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role)] as List<Entity.SysAccount_Role>,
                entitys[MarkBase.GetObject<string>(MarkEnum.R_SysRole)] as List<Entity.SysRole>,
                entitys[MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence)] as List<Entity.SysRole_Competence>,
                entitys[MarkBase.GetObject<string>(MarkEnum.R_SysCompetence)] as List<Entity.SysCompetence>);

            //competences
            if (roleCompetenceView.Exists(c => !System.String.IsNullOrEmpty(c.competence_gid))) { roleCompetence.CompetenceAll = new List<BasicAuthentication.Competences>(); }
            //role
            var _roles = roleCompetenceView.Where(r => !System.String.IsNullOrEmpty(r.role_gid)).GroupBy(g => g.role_gid);
            if (0 < _roles.Count()) { roleCompetence.Roles = new List<BasicAuthentication.Roles>(); roleCompetence.RoleAll = new List<BasicAuthentication.Roles>(); }
            foreach (var item in _roles)
            {
                var _role = roleCompetenceView.Find(r => item.Key == r.role_gid);
                var role = new BasicAuthentication.Roles { SysAccount_Role_gid = _role.account_Role_gid, CreateKey = _role.role_account, AccountKey = accountKey, Key = item.Key, Parent = _role.role_parent, Role = _role.role, Descrip = _role.role_describe };//IsChild
                //competence
                var competences = item.Where(c => !System.String.IsNullOrEmpty(c.competence_gid)).GroupBy(g => g.role_Competence_gid);
                if (0 < competences.Count()) { role.Competences = new List<BasicAuthentication.Competences>(); }
                foreach (var item1 in competences)
                {
                    var _competence = roleCompetenceView.Find(r => item1.Key == r.role_Competence_gid);
                    var competence = new BasicAuthentication.Competences { Role_Competence_gid = _competence.role_Competence_gid, Role = _competence.role_gid, Key = _competence.competence_gid, Parent = _competence.competence_parent, Competence = _competence.competence, Descrip = _competence.competence_describe };
                    role.Competences.Add(competence);
                    roleCompetence.CompetenceAll.Add(competence);
                }
                //add
                roleCompetence.Roles.Add(role);
                roleCompetence.RoleAll.Add(role);
            }
            accountAll = new List<BasicAuthentication.Accounts>();
            roleCompetencAll = new List<BasicAuthentication.RoleCompetence>();
            //account
            var _accounts = sysAccount.Where(a => accountKey == a.parent);
            if (0 < _accounts.Count())
            {
                roleCompetence.RoleCompetences = new List<BasicAuthentication.RoleCompetence>();
                roleCompetence.Account = new BasicAuthentication.Accounts { Parent = roleCompetence.Account.Parent, Key = roleCompetence.Account.Key, Account = roleCompetence.Account.Account, IsChild = true };//IsChild
                //roleCompetence.AccountAll = new List<BasicAuthentication.Accounts> { roleCompetence.Account };
                //roleCompetence.RoleCompetenceAll = new List<BasicAuthentication.RoleCompetence> { roleCompetence };
                roleCompetence.AccountAll = new List<BasicAuthentication.Accounts>();
                roleCompetence.RoleCompetenceAll = new List<BasicAuthentication.RoleCompetence>();
            }

            foreach (var item in _accounts)
            {
                var account = new BasicAuthentication.Accounts { Parent = item.parent, Key = item.gid, Account = item.account };
                var _roleCompetence = new BusinessLib.BasicAuthentication.RoleCompetence { Account = account };
                GetRoleCompetence(entitys, _roleCompetence.Account.Key, ref _roleCompetence, out accountAll, out roleCompetencAll);
                roleCompetence.RoleCompetences.Add(_roleCompetence);

                if (null != _roleCompetence.RoleAll)
                {
                    if (null == roleCompetence.RoleAll) { roleCompetence.RoleAll = new List<BasicAuthentication.Roles>(); }
                    roleCompetence.RoleAll.AddRange(_roleCompetence.RoleAll);
                }
                //====================Account====================//
                if (_roleCompetence.Account.IsChild) { account.IsChild = true; }
                accountAll.Add(account);
                roleCompetence.AccountAll.AddRange(accountAll);
                //====================RoleCompetencAll====================//
                roleCompetencAll.Add(_roleCompetence);
                roleCompetence.RoleCompetenceAll.AddRange(roleCompetencAll);
            }

            #endregion
        }

        public static BusinessLib.BasicAuthentication.RoleCompetence GetRoleCompetence(BusinessLib.Cache.ICache cache, string accountParent, string accountKey, string account)
        {
            var roleCompetence = new BasicAuthentication.RoleCompetence { Account = new BasicAuthentication.Accounts { Parent = accountParent, Key = accountKey, Account = account } };
            List<BusinessLib.BasicAuthentication.RoleCompetence> roleCompetencAll;
            List<BasicAuthentication.Accounts> accountAll;
            //get cache

            var list = new System.Collections.Specialized.ListDictionary();
            list.Add(MarkBase.GetObject<string>(MarkEnum.R_SysAccount), cache.Get<List<Entity.SysAccount>>(MarkBase.GetObject<string>(MarkEnum.R_SysAccount)));
            list.Add(MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role), cache.Get<List<Entity.SysAccount_Role>>(MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role)));
            list.Add(MarkBase.GetObject<string>(MarkEnum.R_SysRole), cache.Get<List<Entity.SysRole>>(MarkBase.GetObject<string>(MarkEnum.R_SysRole)));
            list.Add(MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence), cache.Get<List<Entity.SysRole_Competence>>(MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence)));
            list.Add(MarkBase.GetObject<string>(MarkEnum.R_SysCompetence), cache.Get<List<Entity.SysCompetence>>(MarkBase.GetObject<string>(MarkEnum.R_SysCompetence)));

            GetRoleCompetence(list, roleCompetence.Account.Key, ref roleCompetence, out accountAll, out roleCompetencAll);
            list.Clear();

            return roleCompetence;
        }

        public static void GetRoles(this BusinessLib.BasicAuthentication.RoleCompetence roleCompetence, string roleKey, ref List<BusinessLib.BasicAuthentication.Roles> roles)
        {
            if (System.String.IsNullOrEmpty(roleKey)) { return; }
            foreach (var role in roleCompetence.RoleAll.Where(r => roleKey == r.Parent)) { roleCompetence.GetRoles(role.Key, ref roles); roles.Add(role); }
        }
        public static void GetAccounts(this BusinessLib.BasicAuthentication.RoleCompetence roleCompetence, string accountKey, ref List<BusinessLib.BasicAuthentication.Accounts> accounts)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { return; }
            foreach (var account in roleCompetence.AccountAll.Where(a => accountKey == a.Parent)) { roleCompetence.GetAccounts(account.Key, ref accounts); accounts.Add(account); }
        }

        public static List<RoleCompetenceView> GetRoleCompetenceView(string accountKey, List<SysAccount> sysAccount, List<SysAccount_Role> sysAccount_Role, List<SysRole> role, List<SysRole_Competence> role_Competence, List<SysCompetence> competence)
        {
            /*
             SysAccount_Role 
             LEFT JOIN SysAccount ON SysAccount.gid = SysAccount_Role.account 
             LEFT JOIN Role ON Role.gid = SysAccount_Role.role 
             LEFT JOIN Role_Competence ON Role.gid = Role_Competence.role 
             LEFT JOIN Competence ON Competence.gid = Role_Competence.competence"

             SysAccount_Role.gid AS account_Role_gid, Role.gid AS role_gid, Role.account AS role_account, Role.parent AS role_parent, Role.role, Role.remark AS role_remark, Role_Competence.gid AS role_Competence_gid, Competence.gid AS competence_gid, Competence.parent AS competence_parent, Competence.competence, Competence.remark AS competence_remark
             
             var roleCompetenceView = con.GetData<RoleCompetenceView>("SysAccount_Role LEFT JOIN SysAccount ON SysAccount.gid = SysAccount_Role.account LEFT JOIN Role ON Role.gid = SysAccount_Role.role LEFT JOIN Role_Competence ON Role.gid = Role_Competence.role LEFT JOIN Competence ON Competence.gid = Role_Competence.competence", "SysAccount_Role.gid AS account_Role_gid, Role.gid AS role_gid, Role.account AS role_account, Role.parent AS role_parent, Role.role, Role.remark AS role_remark, Role_Competence.gid AS role_Competence_gid, Competence.gid AS competence_gid, Competence.parent AS competence_parent, Competence.competence, Competence.remark AS competence_remark", "SysAccount.gid = @account", parameter: con.GetParameter("@account", accountKey));
            */

            var query = from _sysAccount in sysAccount

                        join _sysAccount_Role in sysAccount_Role on _sysAccount.gid equals _sysAccount_Role.account into sysAccount_Role_Result
                        from _sysAccount_Role in sysAccount_Role_Result.DefaultIfEmpty(new SysAccount_Role { gid = null })

                        join _role in role on _sysAccount_Role.role equals _role.gid into role_Result
                        from _role in role_Result.DefaultIfEmpty(new SysRole { gid = null })

                        join _role_Competence in role_Competence on _role.gid equals _role_Competence.role into role_Competence_Result
                        from _role_Competence in role_Competence_Result.DefaultIfEmpty(new SysRole_Competence { gid = null })

                        join _competence in competence on _role_Competence.competence equals _competence.gid into competence_Result
                        from _competence in competence_Result.DefaultIfEmpty(new SysCompetence { gid = null })

                        where _sysAccount.gid == accountKey

                        select new RoleCompetenceView
                        {
                            account_Role_gid = _sysAccount_Role.gid,
                            role_gid = _role.gid,
                            role_account = _role.account,
                            role_parent = _role.parent,
                            role = _role.role,
                            role_describe = _role.describe,
                            role_Competence_gid = _role_Competence.gid,
                            competence_gid = _competence.gid,
                            competence_parent = _competence.parent,
                            competence = _competence.competence,
                            competence_describe = _competence.describe,
                        };
            return query.ToList();
        }

        #endregion

        #region //==========================================================================//

        //平级关系展示用 GetAccountAll
        public static string GetAccountAll(BusinessLib.BasicAuthentication.ISession session)
        {
            return ResultExtensions.Result(session.RoleCompetence.AccountAll).ToString();
        }
        //平级关系展示用 GetRoleCompetenceAll
        public static string GetRoleCompetenceAll(BusinessLib.BasicAuthentication.ISession session)
        {
            return ResultExtensions.Result(session.RoleCompetence.RoleCompetenceAll).ToString();
        }

        public static string GetAccountRoot(BusinessLib.BasicAuthentication.ISession session)
        {
            return ResultExtensions.Result(session.RoleCompetence.Account).ToString();
        }

        public static string AddAccount(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string parentAccountKey, string account, string password, string securityCode)
        {
            if (System.String.IsNullOrEmpty(parentAccountKey)) { throw new ArgumentNullException("parentAccountKey"); }
            if (System.String.IsNullOrEmpty(account)) { throw new ArgumentNullException("account"); }
            if (System.String.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }

            var sysAccount = new SysAccount { parent = parentAccountKey, account = account, password = password, securityCode = securityCode };
            //update cache
            var sysAccount_Name = MarkBase.GetObject<string>(MarkEnum.R_SysAccount);
            var cache_SysAccount = cache.Get<List<Entity.SysAccount>>(sysAccount_Name);
            cache_SysAccount.Add(sysAccount);
            cache.Set(sysAccount_Name, cache_SysAccount);
            //update database
            if (0 == db.SaveOrUpdate<Entity.SysAccount>(sysAccount)) { throw new System.Data.DBConcurrencyException(typeof(SysAccount).Name); }

            session.UpdateRoleCompetence(cache);

            return ResultExtensions.Result("OK").ToString();
        }

        public static string DelAccount(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string accountKey)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }

            if (null == session.RoleCompetence.RoleCompetenceAll) { return ResultExtensions.Result(MarkEnum.Exp_RoleCompetenceNotExist).ToString(); }

            var roleCompetence = session.RoleCompetence.RoleCompetenceAll.FirstOrDefault(a => accountKey == a.Account.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.RoleCompetence), roleCompetence)) { return ResultExtensions.Result(MarkEnum.Exp_RoleCompetenceNotExist).ToString(); }

            using (var con = db.GetConnection())
            {
                con.BeginTransaction();
                if (null != roleCompetence.RoleAll)
                {
                    foreach (var item in roleCompetence.RoleAll)
                    {
                        if (!DelRole(con, session, cache, item.SysAccount_Role_gid)) { ResultExtensions.Result("NO").ToString(); }
                    }
                }
                //update cache
                var sysAccount_Name = MarkBase.GetObject<string>(MarkEnum.R_SysAccount);
                var cache_SysAccount = cache.Get<List<Entity.SysAccount>>(sysAccount_Name);
                var _sysAccount = cache_SysAccount.FirstOrDefault(a => a.gid == roleCompetence.Account.Key);
                if (null != _sysAccount) { cache_SysAccount.Remove(_sysAccount); cache.Set(sysAccount_Name, cache_SysAccount); }
                //update database
                if (0 == con.Delete(new SysAccount { gid = roleCompetence.Account.Key, account = roleCompetence.Account.Account, password = System.String.Empty, parent = roleCompetence.Account.Parent })) { con.Rollback(); throw new System.Data.DBConcurrencyException(); }
                con.Commit();
                session.UpdateRoleCompetence(cache);
            }

            return ResultExtensions.Result("OK").ToString();
        }

        public static string GetRoles(BusinessLib.BasicAuthentication.ISession session, string accountKey)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }

            return ResultExtensions.Result(GetRoleCompetences(session).FirstOrDefault(a => accountKey == a.Account.Key).Roles).ToString();
        }

        public static string AddRole(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string accountKey, string parentRole, string role, string descrip)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }
            if (System.String.IsNullOrEmpty(parentRole)) { throw new ArgumentNullException("parentRole"); }
            if (System.String.IsNullOrEmpty(role)) { throw new ArgumentNullException("role"); }

            var roleCompetence = GetRoleCompetences(session).FirstOrDefault(a => accountKey == a.Account.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.RoleCompetence), roleCompetence)) { return ResultExtensions.Result(MarkEnum.Exp_RoleCompetenceNotExist).ToString(); }

            if (!roleCompetence.Roles.Exists(r => parentRole == r.Key)) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }

            using (var con = db.GetConnection())
            {
                con.BeginTransaction();
                var _role = new SysRole { account = accountKey, parent = parentRole, role = role, describe = descrip };
                //update cache
                var role_Name = MarkBase.GetObject<string>(MarkEnum.R_SysRole);
                var cache_Role = cache.Get<List<Entity.SysRole>>(role_Name);
                cache_Role.Add(_role);
                cache.Set(role_Name, cache_Role);
                //update database
                if (0 == con.Save(_role)) { con.Rollback(); throw new System.Data.DBConcurrencyException(); }
                //==================================//
                var sysAccount_Role = new SysAccount_Role { account = accountKey, role = _role.gid };
                //update cache
                var sysAccount_Role_Name = MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role);
                var cache_SysAccount_Role = cache.Get<List<Entity.SysAccount_Role>>(sysAccount_Role_Name);
                cache_SysAccount_Role.Add(sysAccount_Role);
                cache.Set(sysAccount_Role_Name, cache_SysAccount_Role);
                //update database
                if (0 == con.Save(sysAccount_Role)) { con.Rollback(); throw new System.Data.DBConcurrencyException(); }
                con.Commit();
            }

            session.UpdateRoleCompetence(cache);

            return ResultExtensions.Result("OK").ToString();
        }

        public static string GetAccountChild(BusinessLib.BasicAuthentication.ISession session, string accountKey)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }

            var accounts = GetRoleCompetences(session).FirstOrDefault(a => accountKey == a.Account.Key).AccountAll;

            return ResultExtensions.Result(null == accounts ? accounts : accounts.Where(a => accountKey == a.Parent)).ToString();
        }

        public static string SetRole(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string parentAccountKey, string[] roleKeys, string setAccountKey)
        {
            if (System.String.IsNullOrEmpty(parentAccountKey)) { throw new ArgumentNullException("parentAccountKey"); }
            if (null == roleKeys) { throw new ArgumentNullException("roleKeys"); }
            if (System.String.IsNullOrEmpty(setAccountKey)) { throw new ArgumentNullException("setAccountKey"); }

            var roleCompetence = GetRoleCompetences(session).FirstOrDefault(a => parentAccountKey == a.Account.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.RoleCompetence), roleCompetence)) { return ResultExtensions.Result(MarkEnum.Exp_RoleCompetenceNotExist).ToString(); }

            if (!roleCompetence.AccountAll.Exists(a => setAccountKey == a.Key)) { return ResultExtensions.Result(MarkEnum.Exp_AccountNotExist).ToString(); }

            if (!roleCompetence.Roles.Exists(r => roleKeys.Contains(r.Key))) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }

            var sysAccount_Role_Name = MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role);
            using (var con = db.GetConnection())
            {
                con.BeginTransaction();
                var sysAccount_Role = cache.Get<List<Entity.SysAccount_Role>>(sysAccount_Role_Name);
                foreach (var item in roleKeys)
                {
                    //update cache
                    var gid = System.Guid.NewGuid().ToString("N");
                    if (!sysAccount_Role.Exists(c => c.account == setAccountKey && c.role == item)) { sysAccount_Role.Add(new SysAccount_Role { gid = gid, account = setAccountKey, role = item }); }
                    //=====================================//
                    //update database
                    //if (0 == con.Entity.SysAccount_Role.Count(c => c.account == setAccountKey && c.role == item))
                    //{
                    //    con.Save(new SysAccount_Role { gid = gid, account = setAccountKey, role = item });
                    //}
                    con.ExecuteNonQuery("INSERT INTO SysAccount_Role (gid, account, role) SELECT @gid, @account, @role WHERE NOT EXISTS (SELECT * FROM SysAccount_Role WHERE account = @account AND role = @role)", parameter: new DataParameter[] { new DataParameter { Name = "gid", Value = gid }, new DataParameter { Name = "account", Value = setAccountKey }, new DataParameter { Name = "role", Value = item } });
                    //data.ExecuteNonQuery(con, "INSERT INTO SysAccount_Role (gid, account, role) SELECT @gid, @account, @role WHERE NOT EXISTS (SELECT * FROM SysAccount_Role WHERE account = @account AND role = @role)", t, parameter: new System.Data.Common.DbParameter[] { con.GetParameter("@gid", gid), con.GetParameter("@account", setAccountKey), con.GetParameter("@role", item) });
                }
                cache.Set(sysAccount_Role_Name, sysAccount_Role);
                con.Commit();
                session.UpdateRoleCompetence(cache);
            }

            return ResultExtensions.Result("OK").ToString();
        }

        public static string DelRole(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string sysAccount_Role_Key)
        {
            if (System.String.IsNullOrEmpty(sysAccount_Role_Key)) { throw new ArgumentNullException("sysAccount_Role_Key"); }

            using (var con = db.GetConnection())
            {
                con.BeginTransaction();
                if (DelRole(con, session, cache, sysAccount_Role_Key))
                {
                    con.Commit();
                    session.UpdateRoleCompetence(cache);
                }
            }
            return ResultExtensions.Result("OK").ToString();
        }

        static bool DelRole(BusinessLib.Data.IConnection con, BusinessLib.BasicAuthentication.ISession session, BusinessLib.Cache.ICache cache, string sysAccount_Role_Key)
        {
            var role = session.RoleCompetence.RoleAll.FirstOrDefault(r => sysAccount_Role_Key == r.SysAccount_Role_gid);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.Roles), role)) { return false; }
            //CreateKey
            //var isCreateKey = session.RoleCompetence.Account.Key.Equals(role.CreateKey);
            //if (!isCreateKey) { return new BusinessLib.Business.Result(-39).ToString(); }
            //AccountKey
            //var isAccountKey = session.RoleCompetence.Account.Key.Equals(role.AccountKey);

            var roleCompetence = GetRoleCompetences(session).FirstOrDefault(a => role.AccountKey == a.Account.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.RoleCompetence), roleCompetence)) { return false; }

            //roles
            var roles = roleCompetence.RoleAll.Where(r => role.Key == r.Key).ToList();
            roleCompetence.GetRoles(role.Key, ref roles);

            var role_Competence = new List<Entity.SysRole_Competence>();
            var sysAccount_Role = new List<Entity.SysAccount_Role>();
            var _role = new List<Entity.SysRole>();
            foreach (var item in roles)
            {
                var isCreate = (null != roleCompetence.AccountAll && roleCompetence.AccountAll.Exists(a => item.CreateKey == a.Key)) || item.CreateKey == roleCompetence.Account.Key;

                if (isCreate)
                {
                    if (null != item.Competences)
                    {
                        foreach (var competence in item.Competences)
                        {
                            if (!role_Competence.Exists(c => competence.Role_Competence_gid == c.gid))
                            {
                                role_Competence.Add(new SysRole_Competence { gid = competence.Role_Competence_gid, role = item.Key, competence = competence.Key });
                            }
                        }
                    }
                }

                if (!sysAccount_Role.Exists(r => item.SysAccount_Role_gid == r.gid))
                {
                    sysAccount_Role.Add(new Entity.SysAccount_Role { gid = item.SysAccount_Role_gid, account = item.AccountKey, role = System.String.IsNullOrEmpty(item.Key) ? System.String.Empty : item.Key });
                }

                if (isCreate)
                {
                    if (!_role.Exists(r => item.Key == r.gid))
                    {
                        _role.Add(new Entity.SysRole { gid = item.Key, parent = item.Parent, role = item.Role, describe = item.Descrip });
                    }
                }
            }
            if (0 < roles.Count)
            {
                //update cache
                if (0 < role_Competence.Count)
                {
                    var role_Competence_Name = MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence);
                    var cache_Role_Competence = cache.Get<List<Entity.SysRole_Competence>>(role_Competence_Name);
                    foreach (var item in role_Competence)
                    {
                        var _item = cache_Role_Competence.FirstOrDefault(a => a.gid == item.gid);
                        if (null != _item)
                        {
                            cache_Role_Competence.Remove(_item);
                            cache.Set(role_Competence_Name, cache_Role_Competence);
                        }
                    }
                }
                //update database
                foreach (var item in role_Competence) { con.Delete(item); }
                //if (-1 == data.SaveOrUpdate<Role_Competence>(con, role_Competence, t, true)) { return false; }
                //=======================================//
                //update cache
                if (0 < sysAccount_Role.Count)
                {
                    var sysAccount_Role_Name = MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role);
                    var cache_SysAccount_Role = cache.Get<List<Entity.SysAccount_Role>>(sysAccount_Role_Name);
                    foreach (var item in sysAccount_Role)
                    {
                        var _item = cache_SysAccount_Role.FirstOrDefault(a => a.gid == item.gid);
                        if (null != _item) { cache_SysAccount_Role.Remove(_item); cache.Set(sysAccount_Role_Name, cache_SysAccount_Role); }
                    }
                }
                //update database
                foreach (var item in sysAccount_Role) { con.Delete(item); }
                //if (-1 == data.SaveOrUpdate<SysAccount_Role>(con, sysAccount_Role, t, true)) { return false; }
                //=======================================//
                //update cache
                if (0 < _role.Count)
                {
                    var role_Name = MarkBase.GetObject<string>(MarkEnum.R_SysRole);
                    var cache_Role = cache.Get<List<Entity.SysRole>>(role_Name);
                    foreach (var item in _role)
                    {
                        var _item = cache_Role.FirstOrDefault(a => a.gid == item.gid);
                        if (null != _item) { cache_Role.Remove(_item); cache.Set(role_Name, cache_Role); }
                    }
                }
                //update database
                foreach (var item in _role) { con.Delete(item); }
                //if (-1 == data.SaveOrUpdate<Role>(con, _role, t, true)) { return false; }
                return true;
            }
            return false;
        }

        public static string GetRoleAll(BusinessLib.BasicAuthentication.ISession session, string accountKey)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }

            var roleCompetence = GetRoleCompetences(session).FirstOrDefault(a => accountKey == a.Account.Key);
            return ResultExtensions.Result(roleCompetence.RoleAll).ToString();
        }

        static System.Collections.Generic.IEnumerable<BusinessLib.BasicAuthentication.RoleCompetence> GetRoleCompetences(BusinessLib.BasicAuthentication.ISession session)
        {
            return null == session.RoleCompetence.RoleCompetenceAll ? new List<BusinessLib.BasicAuthentication.RoleCompetence> { session.RoleCompetence } : session.RoleCompetence.RoleCompetenceAll.Concat(new List<BusinessLib.BasicAuthentication.RoleCompetence> { session.RoleCompetence });
        }

        public static string SetCompetence(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string roleKey, string[] competenceKeys)
        {
            if (System.String.IsNullOrEmpty(roleKey)) { throw new ArgumentNullException("roleKey"); }
            if (null == competenceKeys) { throw new ArgumentNullException("competenceKeys"); }

            var role = session.RoleCompetence.RoleAll.FirstOrDefault(r => roleKey == r.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.Roles), role)) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }
            var parentRole = session.RoleCompetence.RoleAll.FirstOrDefault(r => role.Parent == r.Key);
            var isParentRole = !System.Object.Equals(default(BusinessLib.BasicAuthentication.Roles), parentRole);

            if (!System.String.IsNullOrEmpty(role.Parent) && !isParentRole) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }

            if (isParentRole && (null == parentRole.Competences || !parentRole.Competences.Exists(c => competenceKeys.Contains(c.Key)))) { return ResultExtensions.Result(MarkEnum.Exp_CompetenceNotExist).ToString(); }

            using (var con = db.GetConnection())
            {
                con.BeginTransaction();
                var role_Competence_Name = MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence);
                var cache_Role_Competence = cache.Get<List<Entity.SysRole_Competence>>(role_Competence_Name);
                foreach (var item in competenceKeys)
                {
                    //update cache
                    var gid = System.Guid.NewGuid().ToString("N");
                    if (!cache_Role_Competence.Exists(c => c.role == roleKey && c.competence == item)) { cache_Role_Competence.Add(new SysRole_Competence { gid = gid, role = roleKey, competence = item }); }
                    //=====================================//
                    //update database
                    if (0 == con.Entity.Get<SysRole_Competence>().Count(c => c.role == roleKey && c.competence == item))
                    {
                        con.Save(new SysRole_Competence { gid = gid, role = roleKey, competence = item });
                    }
                    //data.ExecuteNonQuery(con, "INSERT INTO Role_Competence (gid, role, competence) SELECT @gid, @role, @competence WHERE NOT EXISTS (SELECT * FROM Role_Competence WHERE role = @role AND competence = @competence)", t, parameter: new System.Data.Common.DbParameter[] { con.GetParameter("@gid", gid), con.GetParameter("@role", roleKey), con.GetParameter("@competence", item) });
                }
                cache.Set(role_Competence_Name, cache_Role_Competence);
                con.Commit();
                session.UpdateRoleCompetence(cache);
            }

            return ResultExtensions.Result("OK").ToString();
        }

        public static string DelCompetence(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache, string roleKey, string[] role_Competence_Key)
        {
            if (System.String.IsNullOrEmpty(roleKey)) { throw new ArgumentNullException("roleKey"); }
            if (null == role_Competence_Key) { throw new ArgumentNullException("role_Competence_Key"); }

            var role = session.RoleCompetence.RoleAll.FirstOrDefault(r => roleKey == r.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.Roles), role)) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }

            var roles = new List<BusinessLib.BasicAuthentication.Roles> { role };
            session.RoleCompetence.GetRoles(role.Key, ref roles);

            var list = new List<Entity.SysRole_Competence>();
            foreach (var item in role_Competence_Key)
            {
                var competence = session.RoleCompetence.CompetenceAll.FirstOrDefault(c => item == c.Role_Competence_gid);
                if (System.Object.Equals(default(BusinessLib.BasicAuthentication.Competences), competence)) { return ResultExtensions.Result(MarkEnum.Exp_CompetenceNotExist).ToString(); }

                var _roles = roles.Where(r => null != r.Competences && r.Competences.Exists(c => competence.Key == c.Key)).GroupBy(g => g.Key).Select(s => s.First());

                var competences = _roles.Select(r => new Entity.SysRole_Competence { gid = r.Competences.Find(c => competence.Key == c.Key).Role_Competence_gid, role = r.Key, competence = competence.Key });
                list.AddRange(competences);
            }

            var role_Competence_Name = MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence);
            //update cache
            var cache_Role_Competence = cache.Get<List<Entity.SysRole_Competence>>(role_Competence_Name);
            foreach (var item in list)
            {
                var _item = cache_Role_Competence.FirstOrDefault(a => a.gid == item.gid);
                if (null != _item) { cache_Role_Competence.Remove(_item); cache.Set(role_Competence_Name, cache_Role_Competence); }
            }
            //update database
            foreach (var item in list) { db.Delete(item); }
            //data.SaveOrUpdate<Entity.Role_Competence>(list, isDelete: true);

            session.UpdateRoleCompetence(cache);

            return ResultExtensions.Result("OK").ToString();
        }

        public static string GetCompetence(BusinessLib.BasicAuthentication.ISession session, string roleKey)
        {
            if (System.String.IsNullOrEmpty(roleKey)) { throw new ArgumentNullException("roleKey"); }

            var role = session.RoleCompetence.RoleAll.FirstOrDefault(r => roleKey == r.Key);
            if (System.Object.Equals(default(BusinessLib.BasicAuthentication.Roles), role)) { return ResultExtensions.Result(MarkEnum.Exp_RoleNotExist).ToString(); }

            return ResultExtensions.Result(role.Competences).ToString();
        }

        //========================================================================//

        /*

        public static string SetRoleCompetence(string token, string roleKey, string[] competenceKeys, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache)
        {
            if (System.String.IsNullOrEmpty(roleKey)) { throw new ArgumentNullException("roleKey"); }
            if (null == competenceKeys || 0 == competenceKeys.Length) { throw new ArgumentNullException("competenceKeys"); }

            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            var list = new List<Entity.Role_Competence>();
            foreach (var competenceKey in competenceKeys)
            {
                if (!session.RoleCompetence.CompetenceAll.Exists(c => competenceKey == c.Key)) { return new BusinessLib.Business.Result(-39).ToString(); }
                list.Add(new Role_Competence { role = roleKey, competence = competenceKey });
            }

            if (0 == data.SaveOrUpdate<Entity.Role_Competence>(list)) { throw new System.Data.DBConcurrencyException(); }

            token.UpdateSession(data, cache);

            return new BusinessLib.Business.Result("OK").ToString();
        }

        public static string SetAccount(string token, string parent, string account, string password, string securityCode, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache)
        {
            if (System.String.IsNullOrEmpty(parent)) { throw new ArgumentNullException("parent"); }
            if (System.String.IsNullOrEmpty(account)) { throw new ArgumentNullException("account"); }
            if (System.String.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }
            if (System.String.IsNullOrEmpty(securityCode)) { throw new ArgumentNullException("securityCode"); }

            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            if (!session.RoleCompetence.AccountAll.Exists(r => parent == r.Key)) { return new BusinessLib.Business.Result(-17).ToString(); }

            using (var con = data.GetConnection())
            {
                if (0 < System.Convert.ToInt32(data.ExecuteScalar(con, "SELECT COUNT(*) FROM SysAccount WHERE account = @account", parameter: con.GetParameter("@account", account)))) { return new BusinessLib.Business.Result(-16).ToString(); }
            }

            if (0 == data.SaveOrUpdate<Entity.SysAccount>(new SysAccount { parent = parent, account = account, password = password, securityCode = securityCode })) { throw new System.Data.DBConcurrencyException(); }

            token.UpdateSession(data, cache);

            return new BusinessLib.Business.Result("OK").ToString();
        }

        public static string SetAccountRole(string token, string accountKey, string[] roleKeys, BusinessLib.Data.IData db, BusinessLib.Cache.ICache cache)
        {
            if (System.String.IsNullOrEmpty(accountKey)) { throw new ArgumentNullException("accountKey"); }
            if (null == roleKeys || 0 == roleKeys.Length) { throw new ArgumentNullException("roleKeys"); }

            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            if (!session.RoleCompetence.AccountAll.Exists(r => accountKey == r.Key))
            {
                return new
                    BusinessLib.Business.Result(-17).ToString();
            }

            var list = new List<SysAccount_Role>();

            foreach (var roleKey in roleKeys)
            {
                if (!session.RoleCompetence.RoleAll.Exists(r => roleKey == r.Key)) { return new BusinessLib.Business.Result(-39).ToString(); }
                list.Add(new SysAccount_Role { account = accountKey, role = roleKey });
            }

            if (0 == data.SaveOrUpdate<Entity.SysAccount_Role>(list)) { throw new System.Data.DBConcurrencyException(); }

            token.UpdateSession(data, cache);

            return new BusinessLib.Business.Result("OK").ToString();
        }

        
        

        public static string GetRoleRoot(string token, string accountKey, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }
            return new BusinessLib.Business.Result(session.RoleCompetence.RoleAll.Where(r => accountKey == r.AccountKey)).ToString();
        }
        public static string GetRoleChild(string token, string accountKey, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            var roles = session.RoleCompetence.RoleCompetenceAll.Find(a => accountKey == a.Account.Key).Roles;
            if (null == roles) { return new BusinessLib.Business.Result(roles).ToString(); }
            return new BusinessLib.Business.Result(roles.Where(r => !roles.Exists(r1 => r1.Parent == r.Key))).ToString();
        }
        public static string GetRoleChildAll(string token, string accountKey, string roleKey, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            var roles = new List<BusinessLib.BasicAuthentication.Roles>();
            session.RoleCompetence.RoleCompetenceAll.Find(a => accountKey == a.Account.Key).GetRoles(roleKey, ref roles);
            return new BusinessLib.Business.Result(roles).ToString();
        }

        public static string GetAccountRoot(string token, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            return new BusinessLib.Business.Result(session.RoleCompetence.Account).ToString();
        }
        
        public static string GetAccountChildAll(string token, string accountKey, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            var accounts = new List<BusinessLib.BasicAuthentication.Accounts>();
            session.RoleCompetence.GetAccounts(accountKey, ref accounts);
            return new BusinessLib.Business.Result(accounts).ToString();
        }

        public static string GetCompetence(string token, string roleKey, BusinessLib.Cache.ICache cache)
        {
            var session = token.GetSession(cache);
            if (null == session) { return new BusinessLib.Business.Result(-2).ToString(); }

            var role = session.RoleCompetence.RoleAll.FirstOrDefault(r => roleKey == r.Key);
            if (System.String.IsNullOrEmpty(role.Key)) { return new BusinessLib.Business.Result(-39).ToString(); }
            return new BusinessLib.Business.Result(role.Competences).ToString();
        }

        */

        #endregion

        #endregion
    }
}