using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BLLib.View
{
    public class ViewBase : Business.BusinessBase
    {
        internal string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        internal T SerializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
        //internal string SerializeObject(System.Data.DataSet ds)
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(ds, new Newtonsoft.Json.Converters.DataTableConverter());
        //}
        ////Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0].DefaultView.ToTable(), new Newtonsoft.Json.Converters.DataTableConverter());
        /*
        static string GetClientIP()
        {
            //获取消息发送的远程终结点IP和端口
            var endpoint = System.ServiceModel.OperationContext.Current.IncomingMessageProperties[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
            return string.Format("{0}:{1}", endpoint.Address, endpoint.Port);
        }
        */
    }
}
