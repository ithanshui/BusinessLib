using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using Business.Auth;
using Business.Result;
using Business.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using Business;
using Business.Extensions;
using NLog;
using LinqToDB.Mapping;
using LinqToDB;
using LinqToDB.DataProvider;
using System.Linq;
using Newtonsoft.Json;

namespace UnitTest
{
    [TestClass]
    public class TestCommand
    {
        static byte[] GetToken1()
        {
            var session = new Session { Account = "admin", Password = "test", IP = "192.168.1.111", Site = "Site1" }.ToString();
            var error = System.String.Empty;

            var value = Command.GetCommandData("Login", "LoginToken", false, session);
            return SocketServer(value);
        }

        static Parameters.Register Parameter()
        {
            return new Parameters.Register() { account = "hello    ", password = " 1234 69", d1 = new List<string>() { "aaa", "bbb" }, d2 = new int[] { 1, 2, 3, 4, 5 } };
        }

        static void Asserts1(byte[] bytes)
        {
            var ddd1 = (List<System.Tuple<string, string>>)Help.ProtoBufDeserialize(bytes, typeof(List<System.Tuple<string, string>>));
            Assert.IsNotNull(ddd1);

            var ddd2 = (List<Result>)Help.ProtoBufDeserialize(bytes, typeof(List<Result>));
            Assert.IsNotNull(ddd2);
        }

        static string SessionID = System.Guid.NewGuid().ToString("N");

        static byte[] SocketServer(byte[] value)
        {
            var ip = "192.168.1.111";
            var result = Command.CommandCall(value, Common.Interceptor1, null, ip, SessionID);

            if (null != result)
            {
                return result;
                //session.TrySend(result, 0, result.Length);
            }
            return null;
        }

        static string token;

        [TestMethod]
        public void TestCommand1()
        {
            var vaele = GetToken1();

            var command = Command.CommandCall(vaele);

            Result(command);
            //=================================//
            var ps = Parameter();
            var value = Command.GetCommandData("H2", "H2Token", true, ps.ToBytes());

            command = Command.CommandCall(SocketServer(value));

            Result(command);
        }

        static void Result(Command.CommandResult result)
        {
            switch (result.Token)
            {
                case "LoginToken":
                    if (0 < result.State)
                    {
                        token = System.Text.Encoding.UTF8.GetString(result.Data);
                        Assert.IsNotNull(token);
                    }
                    break;
                case "H2Token":
                    if (0 < result.State)
                    {
                        Asserts1(result.Data);
                    }
                    break;
                default: break;
            }
        }
    }
}
