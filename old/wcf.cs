using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using System.Runtime.Serialization;


namespace C
{
    public class wcf
    {
            public static void invoke<TChannel>(Action<TChannel> func,string endName)//有返回值时可以用Func
            {
                TChannel channel = new ChannelFactory<TChannel>(endName).CreateChannel();
                func(channel);
                IClientChannel ic = channel as IClientChannel;
                try
                {
                    ic.Close();
                }
                catch
                {
                    ic.Abort();
                }
            }
    }
}