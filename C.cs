#region Usings
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using System.Xml;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Collections;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endregion


namespace C
{

    #region  多播委托实例
    class MathOperations
    {
        public static void MultiplyByTwo(double value)
        {
            Console.WriteLine("Multiplying by 2: {0} gives {1}", value, value * 2);
        }
        public static void Square(double value)
        {
            Console.WriteLine("Squaring: {0} gives {1}", value, value * value);
        }
    }
    class Program
    {
        static void Main()
        {
            Action<double> operations = MathOperations.MultiplyByTwo;
            operations += MathOperations.Square;
            ProcessAndDisplayNumber(operations, 2.0);
            ProcessAndDisplayNumber(operations, 7.94);
            ProcessAndDisplayNumber(operations, 1.414);
            Console.Read();
        }

        static void ProcessAndDisplayNumber(Action<double> action, double value)
        {
            Console.WriteLine();
            Console.WriteLine("ProcessAndDisplayNumber called with value = {0}", value);
            action(value);

        }
    }
    #endregion
    #region C公用类
    public class C
    {
        #region 获取并返回当前HttpContext对象
        //public static HttpContext hc()
        //{
        //    return HttpContext.Current;
        //}
        /// <summary>
        /// 当前运行时对象
        /// </summary>
        public static HttpContext hc { get { return HttpContext.Current; } }
        /// <summary>
        /// 获取用户实体对象u
        /// </summary>
        public static u u
        {get{
                return session<u>("u");
        }
        }
        /// <summary>
        /// 判断用户实体cu是否存在，存在代表已经登录，否而为匿名用户
        /// </summary>
        public static bool hsu
        {
            get
            {
                return C.u == null?false:true;
            }
        }
        /// <summary>
        /// 使用已登录用户实体
        /// </summary>
        public static u cu { get {
           // u cu = session<u>("u");if(C.G('lgb'))lgb.click()
            if (!C.hsu && C.hc.Request.Path != "/login")
              {
                /*
                  String className = MethodBase.GetCurrentMethod().ReflectedType.Name;
                  System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                MethodBase methodName = trace.GetFrame(1).GetMethod();*/
                  C.hc.Response.Write("<script>setTimeout(function(){pop.pop(lgb)},444)</script>");
                  C.hc.Response.Flush();
                  C.hc.Response.End();
                  //C.hc.Response.Write("<script>setTimeout(function(){pop.pop(lgb)console.log(123);var be=C.Ce('script');be.appendChild(document.createTextNode('pop.pop(lgb)'));C.Bd().appendChild(be)},3333)</script>");
                   //最好触发js弹出登录框
                   //C.hc.Response.StatusCode = 403;
                  /*
                   string p = "~/login?url=" + C.hc.Request.Path;
                   C.hc.Server.TransferRequest(p);
                   ContentResult cr = new ContentResult();
                   cr.Content = "<script>alert(123)</script>";
                   C.hc.Response.Write(cr.Content);
                  */
               }         
               return u;
            //如果session中没有，跳到登录？
            //return session<u>("u");
            } }

        #endregion
        #region 泛型类型转换
        public static object Cvt<T>(T V)
        {
            return Convert.ChangeType(V, typeof(T));
        }
        #endregion
        #region 获取并返回数组中的最后一个元素
        public static object Lst(object[] Arr)
        {
            return Arr[Arr.Length - 1];
        }
        #endregion
        #region 将所有参数集合到一个对象数组中并返回
        public static object[] Arr(params object[] Ps)
        {
            return Ps;
        }
        public static T[] Arr<T>(params T[] Ps)
        {
            return Ps;
        }
        #endregion
        #region 操作Config文件
        public static string Gfig(string Key)
        {
            return ConfigurationManager.AppSettings[Key];
        }
        #endregion
        #region Hash加密
        public static string Hash(string Salt, string Pwd, string Et)
        {
            return Crypto.Hash(Salt + Pwd, Et);
        }
        #endregion

        #region 用SqlDataReader第一行的内容读入Array并返回
        /// <summary>
        /// 未遍历行，只能取第一行
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static object[] ToArray(SqlDataReader Reader)
        {
            int n = Reader.FieldCount;
            object[] Os = new object[n];
            if (Reader.Read())
            {
                Reader.GetValues(Os);
            }
            Reader.Close();
            return Os;
        }
        public static object[] ToArray(DataTable dt)
        {
            int n = dt.Rows.Count;
            object[] Os = new object[n];
            for (int i = 0; i < n; i++)
            {
                Os[i] = dt.Rows[i].ItemArray;
            }
            return Os;
        }

        #endregion

        #region 用SqlDataReader将行的内容读入List并返回  该方法应该有错误
        public static List<object[]> ToList(SqlDataReader Reader)
        {
            List<object[]> Ls = new List<object[]>();
            int n = Reader.FieldCount;
            while (Reader.Read())
            {
                object[] Os = new object[n];
                Reader.GetValues(Os);
                Ls.Add(Os);
            }
            return Ls;
        }
        #endregion
        //public static List<T> listByTable(SqlCommand Cmd)
        //{
        //    using (SqlDataReader r = Cmd.ExecuteReader())
        //    {
        //        return C.ToList(r);
        //    }
        //}
        /*
        //将DataTable类型转化为数组
        public static List<T> toArray<T>(DataTable tb)
        {
            List<T> list = new List<T>();
            if (tb != null && tb.Rows.Count > 0)
            {
                T o = default(T);
                for(int i=0;i<tb.Rows.Count)
                {
                    list.Add(tb.Rows[i]);
                }
                foreach (DataRow r in tb.Rows)
                {
                    o = toModel<T>(r);
                    list.Add(o);
                    r.ItemArray
                }
            }
            return list;
        }
        */
        /// <summary>
        /// 将DataTable类型转化为指定类型的实体集合
        /// </summary>
        /// <typeparam name="T">需要将DataRow转成的类型，该类型须有无参构造函数，（如string等没有无参构造函数的类型则不能传入）</typeparam>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static List<T> toList<T>(DataTable tb)
        {
            List<T> list = new List<T>();
            if (tb != null && tb.Rows.Count > 0)
            {
                T o = default(T);
                foreach (DataRow r in tb.Rows)
                {
                    o = toModel<T>(r);
                    list.Add(o);
                }
            }
            return list;
        }
        /// <summary>
        /// 将传入DataRow转成指定类型的实例并返回
        /// </summary>
        /// <typeparam name="T">有无参构造函数的类型</typeparam>
        /// <param name="r">欲转成Ｔ类型对象的DataRow</param>
        /// <returns></returns>
        public static T toModel<T>(DataRow r)
        {
            T o = Activator.CreateInstance<T>(); //T类型须有无参构造函数，否则不能传入，string就没有
            PropertyInfo[] ps = typeof(T).GetProperties();
            foreach (PropertyInfo p in ps)
            {
                string nm = p.Name;
                if (r.Table.Columns.Contains(nm))
                {
                    // 判断此属性是否有Setter 
                    //if (!p.CanWrite) continue;
                    object v = r[nm];
                    if (v != DBNull.Value)
                        p.SetValue(o, v);
                }

                //T obj = Activator.CreateInstance<T>();  //须有T类型要有无参构造函数
                //String str = "{\"Uid\":1,\"Rid\":2,\"Email\":\"3\",\"phone\":\"4\",\"pwd\":null}";
                //json串转实例
                //using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                //{
                //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(us.GetType());
                //    us = (C.u)serializer.ReadObject(ms);
                //}




            }
            return o;
        }
        /*
                /// <summary>
        /// 通过DataRow 填充实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static  T GetModelByDataRow<T>(System.Data.DataRow dr) where T : new()
        {
            T model = new T();
            foreach (PropertyInfo pInfo in model.GetType().GetProperties())
            {
                object val = getValueByColumnName(dr, pInfo.Name);
                pInfo.SetValue(model, val, null);
            }
            return model;
        }

        //返回DataRow 中对应的列的值。
        public static object getValueByColumnName(System.Data.DataRow dr, string columnName)
        {
            if (dr.Table.Columns.IndexOf(columnName) >= 0)
            {
                if (dr[columnName] == DBNull.Value)
                    return null;
                return dr[columnName];
            }
            return null;
        }
        */
        /*
        #region 用SqlDataReader将行的内容读入DataTable并返回
        public static DataTable ToTable(SqlDataReader Reader)
        {
            DataTable Dt = new DataTable();
            if (Reader.HasRows)
            {
                Dt.Load(Reader);
            }
            return Dt;
        }
        #endregion
        */
        #region 将传入对象数组转化为KeyValuePair数组
        /*
        public static KeyValuePair<string, object>[] Ps<s,i>(params object[] Os)
        {
            KeyValuePair<string, object>[] Ps = new KeyValuePair<string, object>[Os.Length / 2];
            for (int j = 0,k=0; j < Os.Length; j += 2,k++)
            {
                Ps[k] = new KeyValuePair<string, object>(Os[j] as string, (object)Cvt(Os[j + 1]));
            }
            return Ps;
        }
         */
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="func"></param>
        /// <param name="endName"></param>
        public static void invoke<TChannel>(Action<TChannel> func, string endName)//有返回值时可以用Func
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
        /*比较下*/
        public static object ExecuteMethod<T>(string pUrl, string pMethodName, params object[] pParams)
        {
            EndpointAddress address = new EndpointAddress(pUrl);
            System.ServiceModel.Channels.Binding bindinginstance = null;
            NetTcpBinding ws = new NetTcpBinding();
            ws.MaxReceivedMessageSize = 20971520;
            ws.Security.Mode = SecurityMode.None;
            bindinginstance = ws;
            using (ChannelFactory<T> channel = new ChannelFactory<T>(bindinginstance, address))
            {
                T instance = channel.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(pMethodName);
                        return mi.Invoke(instance, pParams);
                    }
                    catch (TimeoutException)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw;
                    }
                    catch (CommunicationException)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw;
                    }
                    catch (Exception vErr)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <returns></returns>
        public static string cookie(string k)
        {
            HttpCookie cookie = C.hc.Request.Cookies[k];
            string v = "";
            if (C.hc != null&&cookie != null)
                v = cookie.Value;
            return v;
        }
        public static void cookie(string k, string v, int minute, string path = "", string domain = "")
        {
            HttpContext context = C.hc;
            if (context == null)
                return;
            HttpCookie cookie = new HttpCookie(k, v);
            cookie.Expires = DateTime.Now.AddMinutes(minute);
            if (!string.IsNullOrEmpty(path))
                cookie.Path = path;
            if (!string.IsNullOrEmpty(domain))
                cookie.Domain = domain;

            context.Response.Cookies.Add(cookie);
        }
        #region 未审核的________Cookie和Session操作

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="key">Session名称</param>
        /// <returns></returns>
        public static T session<T>(string key)
        {
            T o = default(T);
            object s = C.hc.Session[key];
            if (s != null)
                o = (T)s;
            return o;
        }
        public static void session<T>(string k,object v)
        {

            C.hc.Session[k] = v;
        }
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key">session名称</param>
        /// <param name="data">存储数据</param>
        public static void SetSession(string key, object data)
        {
            HttpContext context = C.hc;
            if (context == null || context.Session == null)
                return;

            context.Session[key] = data;
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <returns></returns>
        public static string GetCookie(string key)
        {
            HttpCookie cookie = C.hc.Request.Cookies[key];
            if (C.hc == null || cookie == null)
                return string.Empty;
            return cookie.Value;
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <param name="value">存储数据</param>
        /// <param name="minute">有效分钟数</param>
        public static void SetCookie(string key, string value, int minute, string path = "", string domain = "")
        {
            HttpContext context = C.hc;
            if (context == null)
                return;

            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = DateTime.Now.AddMinutes(minute);
            if (!string.IsNullOrEmpty(path))
                cookie.Path = path;
            if (!string.IsNullOrEmpty(domain))
                cookie.Domain = domain;

            context.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取Url参数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <returns></returns>
        public static string qs(string k)
        {
            return C.hc.Request.QueryString[k];
        }
        #endregion
        #region 发邮件

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接受者，多个时可用","分隔</param>
        /// <param name="body">邮件内容</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="cc">抄送者，多个时可用","分隔</param>
        /// <param name="attach">附件</param>
        /// <param name="isHtml">body是否为html</param>
        public static bool mail(string to, string body, string subject = "", string cc = "", string attach = "", bool isHtml = false)
        {
            //邮箱加验证
            bool result = false;
            string host = C.Gfig("mailHost"),
                        port = C.Gfig("mailPort"),
                        account = C.Gfig("mailAccount"),
                        pwd = C.Gfig("mialPwd");

            MailMessage m = new MailMessage();
            //Message.SubjectEncoding = System.Text.Encoding.UTF8;
            m.From = new MailAddress(account, "万佛林");
            m.Body = body;
            m.IsBodyHtml = isHtml;
            m.To.Add(to);
            if (!string.IsNullOrWhiteSpace(subject))
                m.Subject = subject;
            if (!string.IsNullOrWhiteSpace(attach))
                m.Attachments.Add(new Attachment(attach));

            if (!string.IsNullOrWhiteSpace(cc))
                m.CC.Add(cc);
            try
            {
                //设置邮件服务器
                SmtpClient client = new SmtpClient(host, int.Parse(port));
                //client.UseDefaultCredentials = true;
                //client.EnableSsl = true;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(account, pwd);
                client.Send(m);
                result = true;
            }
            catch (SmtpException e)
            {
                throw e;
            }
            /*
         MailMessage msg = new MailMessage();
        msg.To.Add("858652407@qq.com");//收件人地址 
        //msg.CC.Add("cc@qq.com");//抄送人地址 
        msg.From = new MailAddress("858187064@qq.com", "test");//发件人邮箱，名称 
        msg.Subject = "This is a test email from QQ";//邮件标题 
        msg.SubjectEncoding = Encoding.UTF8;//标题格式为UTF8 
        msg.Body = "this is body";//邮件内容 
        msg.BodyEncoding = Encoding.UTF8;//内容格式为UTF8 
        SmtpClient client = new SmtpClient();
        client.Host = "smtp.qq.com";//SMTP服务器地址 
        client.Port = 587;//SMTP端口，QQ邮箱填写587 
        client.EnableSsl = true;//启用SSL加密 
        //发件人邮箱账号，授权码(注意此处，是授权码你需要到qq邮箱里点设置开启Smtp服务，然后会提示你第三方登录时密码处填写授权码)
        client.Credentials = new System.Net.NetworkCredential("858187064@qq.com", "jwyezkrjqahlbdeg");
            client.Send(msg);//发送邮件
                         /// <summary>
             /// 获取邮箱发送模板
             /// </summary>
             /// <param name="temName"></param>
             /// <returns></returns>
             private string GetTemplate(string temName)
             {
                 string S = string.Format("{0}{1}/{2}.html", C.C.hc().Server.MapPath("/"), "EmailTemplate", temName);
                 bool fx = (File.Exists(S));
                 if (File.Exists(S))
                 {
                     return File.ReadAllText(S);
                 }
                 return string.Empty;
             }
     */
            return result;
        }
        #endregion

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url">请求的路径</param>
        /// <param name="method">请求方法，默认为get</param>
        /// <returns></returns>
        public static string req(string url,string method="GET",string param="")
        {
          
          HttpWebResponse rsp=null;
         Stream strm=null;
         string charset, rs="";
           if(method==null)
           {
               method = "GET";
           }
            HttpWebRequest rqt = WebRequest.Create(url) as HttpWebRequest;
            rqt.Method = method;

            if (method == "POST"&&param!="")
            {
                byte[] data = Encoding.UTF8.GetBytes(param);
                //写入请求流
                using (Stream stream = rqt.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }


            //rqt.KeepAlive = true;
            //rqt.ContentType = "application/json; charset=utf-8";
            //rqt.Timeout = times;
            rqt.UserAgent = "MSIE 7.0; Windows NT 5.1";
            //Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.96 Safari/537.36
            try
            {
                string encoding = "";//可以参数化
                rsp = rqt.GetResponse() as HttpWebResponse;//ISO-8859-1GB2312
                charset = string.IsNullOrWhiteSpace(encoding) ? (rsp.CharacterSet == "ISO-8859-1" ? "GB2312" : string.IsNullOrWhiteSpace(rsp.CharacterSet) ? "UTF-8" : rsp.CharacterSet) : encoding;
               // rsp.ContentType = "application/json; charset=utf-8";
                strm = rsp.GetResponseStream();
                rs = new StreamReader(strm).ReadToEnd();
                // ServicePointManager.DefaultConnectionLimit = 200;
            }
            catch (WebException e)
            {
                //[WebException: 远程服务器返回错误: (404) 未找到。]
                //this.rsp = rqt.GetResponse() as HttpWebResponse;操作超时
                //if (resp.StatusCode != HttpStatusCode.OK) //如果服务器未响应，那么继续等待相应
                //    continue;
                ;
            }
            finally
            {
                rsp.Close();
                strm.Dispose();
                strm.Close();
            }
            return rs;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string timeTostring(DateTime t,string p){
            string r = "";
            if (t != DateTime.MinValue)
                r = t.ToString(p);
            return r;
        }
  /// <summary>
        /// 将对象序列化（serialize）成json字符串 考虑换成Newtonjson
            /// </summary>
            /// 

        /*
         var json = JsonConvert.SerializeObject(p);
Console.WriteLine(json);

var pp = JsonConvert.DeserializeObject<Person>(json);
Console.WriteLine(pp.Name);

var jobj = JsonConvert.DeserializeObject(json) as JObject;
Console.WriteLine(jobj.GetType());
foreach (var item in jobj)
{
    Console.WriteLine($"{item.Key}:{item.Value}");
}
         */
        public static string ser<T>(T o)
            {
                //return new JavaScriptSerializer().Serialize(o);
                return JsonConvert.SerializeObject(o);
            }
            /// <summary>
            /// 将json字符串反序列化成对象
            /// </summary>
        public static JObject deser(string json)
            {
                //return (T)new JavaScriptSerializer().Deserialize(json, typeof(T));
                return JsonConvert.DeserializeObject(json) as JObject;
            }
            /// <summary>
            /// 生成验证码
            /// </summary>
            /// <param name="l">验证码长度</param>
            /// <param name="ct">验证码组成字符的类型0为纯数字0-9,1为config的vczm中指定的纯字母，2为config的vcs中指定字符（多用于pc版）</param>
            /// <returns></returns>
           public static string code(int l, int ct = 0)
            {
                System.Web.SessionState.HttpSessionState ss = C.hc.Session;
                string k = ct == 1 ? "vczm" : "vcs",
                    o = ct == 0 ? "0123456789" : ConfigurationManager.AppSettings[k],
                        c = "";
                Random r = new Random();
                for (int i = 0; i < l; i++)
                {
                    c += o[r.Next(1, o.Length - 1)];//Convert.ToChar(r.[Next](65, 90))//从随机数直接转换成字母
                }
                //存入session，暂时不用token，键名规则？
                // ss.Add(kyzm, c);
                ss.Add(ss.SessionID, c);
                /*
                string token = DateTime.Now.ToString("ddHHmmss") + r.Next(1000, 9999);
                //aes.ecode(userId + time + info + key(密钥)
                //aes.decode(token)
                //token可以使用 md5(username + userid + timestamp)_timestamp, 
                */
                return c;
            }

        /*
        //json 序列化

        public static string ToJsJson(object item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, item);
                StringBuilder sb = new StringBuilder();
                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                return sb.ToString();
            }
        }

        //反序列化

        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }

        public class json
        {
          
        }*/
        
    }


    /*
    /// <summary>
    /// 该类中的ToDynamic方法，用于从任意对象扩展出一个动态类型的对象。这个动态类型会根据输入对象中的属性信息，生成对应的公有字段，然后使用反射进行赋值。
    /// </summary>
    public static class dynamicFactory
    {
        private static ConcurrentDictionary<Type, Type> s_dynamicTypes = new ConcurrentDictionary<Type, Type>();

        private static Func<Type, Type> s_dynamicTypeCreator = new Func<Type, Type>(CreateDynamicType);

        public static object ToDynamic(this object entity)
        {
            var entityType = entity.GetType();
            var dynamicType = s_dynamicTypes.GetOrAdd(entityType, s_dynamicTypeCreator);

            var dynamicObject = Activator.CreateInstance(dynamicType);
            foreach (var entityProperty in entityType.GetProperties())
            {
                var value = entityProperty.GetValue(entity, null);
                dynamicType.GetField(entityProperty.Name).SetValue(dynamicObject, value);
            }

            return dynamicObject;
        }

        private static Type CreateDynamicType(Type entityType)
        {
            var asmName = new AssemblyName("DynamicAssembly_" + Guid.NewGuid());
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule("DynamicModule_" + Guid.NewGuid());

            var typeBuilder = moduleBuilder.DefineType(
                entityType.GetType() + "$DynamicType",
                TypeAttributes.Public);

            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            foreach (var entityProperty in entityType.GetProperties())
            {
                typeBuilder.DefineField(entityProperty.Name, entityProperty.PropertyType, FieldAttributes.Public);
            }

            return typeBuilder.CreateType();
        }
    }

    */
   

    #region 验证类手机号、邮箱
    public class validate
    {
        private static string mb = @"^1[34578]+\d{9}$";
        private static string em = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        private static string uri = @"^(https?|ftp|file|ws)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
        public delegate bool dlg(string v);
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static bool mobile(string mobile)
        {
            return Regex.Match(mobile, mb).Success;
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="mail">邮箱号</param>
        /// <returns></returns>
        public static bool mail(string mail)
        {
            return Regex.Match(mail, em).Success;
        }
        public static bool url(string url)
        {
            return Regex.Match(url, uri).Success;
        }

    }
    #endregion
    #region 操作文件、文件夹、流
    public static class file
    {
        /// <summary>
        /// 上传文件到指定路径
        /// </summary>
        /// <returns></returns>
        public static string up(string p)
        {
            string r = string.Empty;
            HttpFileCollection fs = C.hc.Request.Files;
            HttpPostedFile f = fs[0];
            f.SaveAs("");
            r = f.FileName;
            return r;
        }    
        /// <summary>
        /// 根据传入路径创建文件夹
        /// </summary>
        /// <param name="path">相对目录，如~/path等</param>
        public static void aDir(string path)
        {
            path = C.hc.Server.MapPath(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //Directory.Delete(Server.MapPath("~/upimg/hufu"), true);//删除文件夹以及文件夹中的子目录，文件    
            /*
            图片优化：
            public static Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)   
        {           
            System.Drawing.Image imgSource = b;      
            System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;  
            int sW = 0, sH = 0;          
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth) 
            {               
                if ((sWidth * destHeight) > (sHeight * destWidth))    
                {                 
                    sW = destWidth;    
                    sH = (destWidth * sHeight) / sWidth;  
                }               
                else           
                {           
                    sH = destHeight;    
                    sW = (sWidth * destHeight) / sHeight;    
                }           
            }           
            else        
            {          
                sW = sWidth;  
                sH = sHeight;  
            }    
            Bitmap outBmp = new Bitmap(destWidth, destHeight);  
            Graphics g = Graphics.FromImage(outBmp);      
            g.Clear(Color.Transparent);         
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality; 
            g.SmoothingMode = SmoothingMode.HighQuality;       
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;    
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);     
            g.Dispose();      
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();  
            long[] quality = new long[1];      
            quality[0] = 100;      
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);   
            encoderParams.Param[0] = encoderParam;   
            imgSource.Dispose();         
            return outBmp;      
        }

            //判断文件的存在
            if (File.Exists(C.hc.Server.MapPath("~/upimg/Data.html")))
            {
                C.hc.Response.Write("Yes");

                //存在文件

            }

            else
            {
                C.hc.Response.Write("No");
                //不存在文件
                File.Create(C.hc.Server.MapPath("~/upimg/Data.html"));//创建该文件

            }
            */
        }
        //图像处理相关
        //获取GetCodecInfo
        /// <summary>
        /// 根据传入类型名称返回ImageCodecInfo对象
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static ImageCodecInfo ici(string en)
        {
            ImageCodecInfo[] cs = ImageCodecInfo.GetImageEncoders();
            string mt = "image/" + en;
            foreach (ImageCodecInfo i in cs)
            {
                if (i.MimeType == mt) return i;
            }
            return null;
        }

        /// <summary>
        /// 根据传入
        /// </summary>
        /// <param name="qlt">1~100间的整数，指定图像压缩级别，建议60以上，60以下画质明显降低</param>
        /// <returns></returns>
        public static EncoderParameters ep(int q)
        {

            EncoderParameters p = new EncoderParameters(1);
            p.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, q);
            return p;
        }
        #endregion

        /*
        2. 读
1).  第一种方式:
      int count = (int)response.ContentLength;
                int offset = 0;
                buf = new byte[count];
                while (count > 0)
                {
                    int n = resStream.Read(buf,offset,count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                    Console.WriteLine( "in loop " + getString(buf) ); //测试循环次数
                }
     string content = Encoding.Default.GetString(buf, 0, buf.Length);

     必须循环读流, 不能一次读(resStream.Read(buf,0,count); ), 否则读的流可能不完整

2) 第二种方式://用StreamReader读取流
     string content = "";
     using (StreamReader  sr = new StreamReader(resStream))     
     {
          content = sr.ReadToEnd();
     }
        */
    }
    /*
    public static class enums
    {
        public static string des(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;

                FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                   fi, typeof(DescriptionAttribute));

                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch
            {
            }
            return obj.ToString();
        }
        public static List<EnumObject> des(Type T)
        {
            List<EnumObject> result = new List<EnumObject>();

            foreach (var item in T.GetEnumValues())
            {
                EnumObject obj = new EnumObject();
                obj.id = (int)item;
                obj.name = item.des();

                result.Add(obj);
            }

            return result;
        }
    }

    #endregion


    /// <summary>
    /// 菜单分类
    /// </summary>
    public enum EMenuCate
    {
        /// <summary>
        /// 前台菜单 
        /// </summary>
        [Description("前台菜单")]
        PreMenu = 1,
        /// <summary>
        /// 后台菜单
        /// </summary>
        [Description("后台菜单")]
        BackMenu = 2,
        /// <summary>
        /// API接口地址
        /// </summary>
        [Description("API接口地址")]
        Api = 3,
        /// <summary>
        /// 前台功能页面(非菜单项)
        /// </summary>
        [Description("前台功能页面")]
        PreFunPage = 4,
        /// <summary>
        /// 后台功能页面(非菜单项)
        /// </summary>
        [Description("后台功能页面")]
        BackFunPage = 5
    }

        */

    #region 扩展HtmlExtensions
    public static class HtmlExtensions
    {
        static string p = "jscss";//前缀
        public static MvcHtmlString runJs(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            string k = p + Guid.NewGuid();// new Random(111111).Next();
            htmlHelper.ViewContext.HttpContext.Items[k] = template;
            return MvcHtmlString.Empty;
        }
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith(p))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            /*
            IDictionary its = htmlHelper.ViewContext.HttpContext.Items;
            for (var i=0;i<its.Count;i++)
            {
                string key = "";
                if(its[i]!=null)
                key=((KeyValuePair<object, object>)its[i]).Key.ToString();
                if (key.ToString().StartsWith(p))
                {
                    var template = its[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }*/


            return MvcHtmlString.Empty;
        }
    }
    #endregion


    #region 用于处理string的类

    public class str
    {
        #region 将传入的null，转成空字符串""
        /// <summary>
        ///将传入对象转换为string，null将转成空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string toString(object str)
        {
            if (str == null)
                str = "";
            return str.ToString();
        }
        ///判断传入的值是否为null、empty、“null”
        public static bool IsNullOrEmptyOrStrNull(string str)
        {
            return string.IsNullOrEmpty(str) || str.Trim().ToLower() == "null" ? true : false;
        }


        #endregion
    }
    #endregion
    #region 返回结果类
    #region 返回结果类
    /// <summary>
    /// 通用返回值，无泛型data
    /// </summary
    public class result
    {
        /// <summary>
        /// 通用返回值
        /// </summary
        private int Code;
        public int code
        {
            get { return Code; }
            set
            {
                Code = value;
                if (value < er.Length)
                    this.msg = er[value];
            }
        }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string msg { get; set; }
        String[] er = { "操作失败", "操作成功", "账号不存在", "账号已存在", "账号或密码不正确", "验证码错误", "票据过期，请重新登录", "权限不足" };
        public result(int code)
        {
            this.code = code;
            this.msg = code > -1 ? er[code] : "未标识的异常";
        }
    }
    /// <summary>
    /// 通用返回值，含泛型data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class result<T> : result
    {
        public T data { get; set; }
        public result(int code, T t = default(T)) : base(code)
        {
            this.data = t;
        }
    }
    #endregion
    #endregion
    #region 用户实体类
    public class u
    {

        public int id { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int rid { get; set; }
        public string mail { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mb { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nn { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string head { get; set; }
        public string pwd { get; set; }
        public string ticket { get; set; }
        //public int failCount { get; set; }
        public u() { }
        //public u(String act, String pwd)
        //{
        //    //好像不设置也没问题
        //    if (Regex.IsMatch(act, @"^1[34578]+\d{9}$"))
        //    {
        //        this.phone = act;
        //    }
        //    else
        //    {
        //        this.Email = act;
        //    }
        //    this.pwd = pwd;
        //}
    }

    #endregion
    /*
    ///枚举形式返回码的
    /// <summary>
    /// 接口返回状态编码
    /// </summary>
    [Description("返回码")]
    public enum ers
    {

        //>0  操作成功
        //错误码从-100开始
        //[Description("操作成功！")]
        //success =>0,

        [Description("操作失败！")]
        failed = 0,

        [Description("操作成功！")]
        success = -101,
        //public int code { get; set; }

        [Description("用户名或密码错误！")]
        lFailed = -102,
        [Description("错误密码输入5次，请2小时后再试！")]
        lLocked = -103,
        [Description("用户名或密码不能为空！")]
        lFailValid = -104,

        [Description("此邮箱已经存在！")]
        vNotExistsEmail = -105,
        [Description("此手机号已经存在！")]
        vNotExistsPhone = -106,
        [Description("登录票据无效！")]
        vNotExistsTicket = -107,
        [Description("此用户不存在！")]
        vNotExistsUser = -108,
        [Description("手机验证码错误！")]
        vPhoneNumFailed = -109,
        [Description("验证码错误！")]
        vImgNumFailed = -110,
        [Description("邮箱验证码错误！")]
        vEmailNumFailed = -111,



        [Description("原密码输入错误！")]
        rOldPwdFailed = -112,
        [Description("邀请码生成失败！")]
        Code = -113,
        [Description("修改用户邀请码失败！")]
        ezpbf = -114,
        [Description("添加邀请记录失败！")]
        aInCode = -115,

        [Description("票据过期，请重新登录！")]
        ticketExpired = -116,
        [Description("无权访问此接口！")]
        noPermissions = -117,
        [Description("页面过期！")]
        pageOverdue = -118,
        [Description("用户名不能为空！")]
        rEmOrPhIsNull = -119,

        [Description("该角色名称已经存在！")]
        existRole = -120,
        [Description("该角色下已经存在用户！")]
        existRoleUser = -121,
        [Description("无登录后台权限！")]
        noLoginPermissions = -122,
        [Description("绑定银行卡信息错误！")]
        bank = -123,
        [Description("删除银行卡失败！")]
        dbank = -124,
        [Description("获取用户信息失败！")]
        duserinfo = -125,
        [Description("邀请码已存在！")]
        zcode = -126,
        [Description("邮箱验证码发送失败！")]
        emcode = -127,
        [Description("请输入手机号！")]
        phnull = -129,
        [Description("请输入邮箱！")]
        emnull = -130,
        [Description("参数错误！")]
        errpar = -128,
        [Description("已是当前最新版本！")]
        currentNew = -131,
        [Description("有新版本！")]
        hasNew = -132,
        [Description("登录失败次数过多，需要图片验证码！")]
        failToMany = -133,
        [Description("此企业不存在！")]
        vNotExistFirm = -134,
        [Description("两次密码不一致")]
        pwdNotMatch = -135,
        [Description("程序异常！")]
        Exception = -999
    }
    */
    /*————————————————————————————————————————————————————————————————————————————————

        总的来说，DataContract和Newtonsoft.Json这两种方式效率差别不大，随着数量的增加JavaScriptSerializer的效率相对来说会低些。

（2）对于DataTable的序列化，如果要使用json数据通信，使用Newtonsoft.Json更合适，如果是用xml做持久化，使用DataContract合适。

（3）随着数量的增加JavaScriptSerializer序列化效率越来越低，反序列化和其他两种相差不大。

（4）后来发现当对象的DataTime类型属性不赋值时，DataContract和JavaScriptSerializer这两种方式序列化都会报错，而用Newtonsoft.Json方式可以正常序列化。所以看来在容错方便，还是Newtonsoft.Json比较强。
序列化少5000次以下时用DataContractJsonSerializer,序列化最快，5000次以上时反序列化不如Newtonsoft快
多时用Newtonsoft




     
         ------------------------------_________________________________________________   */


    #region 用于处理http的类
    public class http
    {
        public static bool isAjax
        {
            get
            {
                return C.hc.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ? true : false;
            }
        }
    }
        public class h
    {
        //public System.Drawing.Image i;
        //public h(string url)
        //{
        //    HttpWebRequest q = WebRequest.Create(url) as HttpWebRequest;
        //    HttpWebResponse p = q.GetResponse() as HttpWebResponse;
        //    this.i = System.Drawing.Image.FromStream(p.GetResponseStream());
        //}
    }
    /*
    public class json
    {

        /*
        {
          a:"幸福黄金网：金银分化 振荡蓄势",
          b:""超级周"来临 黄金反弹成败在此一举",
          c:"消息面井喷,白银本周上演大戏"
         }
         */
    /*
    public object json(string js)
    {
        string[] so = js.Split(',');
        for (int i = 0; i < so.Length; i++)
        {
            string[] p=so[i].Split(':');
            string k = p[0],
                v = p[1];
            //string[] a = so[i].Split(',', StringSplitOptions.RemoveEmptyEntries);

        }

        return js;
    }

}*/
    #endregion
    #region Db数据库相关
    /// <summary>
    /// 操作数据库的类
    /// </summary>
    public class Db
    {
        ///默认连接串的key
        private static readonly string dk;
        ///默认连接串
        //private static readonly string ds;// conStr(C.Gfig("DefaultDb"));
        static Db()
        {
            dk = C.Gfig("DefaultDb");
            //ds = conStr(dk);
            //SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
            //SqlCommand Cmd = new SqlCommand(CmdTxt, Con);
            //Cmd.CommandType = CommandType.StoredProcedure;
        }

        /// <summary>
        /// 根据传入名称获取cfg中连接串
        /// </summary>
        /// <param name="nm">config中连接字符串的name</param>
        /// <returns>返回传入名称对应的边接串</returns>
        public static string conStr(string nm)
        {
            return ConfigurationManager.ConnectionStrings[nm].ConnectionString;
        }
        /// <summary>
        /// 临时未关闭的connection集合，key为数据库名称
        /// </summary>
        private static Dictionary<string, SqlConnection> tc = new Dictionary<string, SqlConnection>();
        /// <summary>
        /// 关闭、销毁tc中指定connection，并在tc中将它移除
        /// </summary>
        /// <param name="con"></param>
        public static void ClsTc(string con)
        {
            SqlConnection co = tc[con];
            if (co != null)
            {
                co.Close();
                co.Dispose();
                tc.Remove(con);
            }
            //tc.Remove(con.Database);
            //if (con != null && con.State != ConnectionState.Closed)
            //{
            //    con.Close();
            //    con.Dispose();
            //}
        }
        /// <summary>
        /// 根据传入name创建并返回SqlConnection对象
        /// </summary>
        /// <param name="nm">config中connectionString的name</param>
        /// <returns></returns>
        public static SqlConnection con(string nm = "")
        {
            if (string.IsNullOrWhiteSpace(nm))
                nm = dk;
            SqlConnection con = null;
            bool hc = tc.ContainsKey(nm);
            if (!hc)
                con = new SqlConnection(conStr(nm));
            else if(tc[nm]!=null)//&&tc[nm].State == ConnectionState.Open
                con = tc[nm];
            //SqlCommand Cmd = new SqlCommand(s, con);
            //Cmd.CommandType = CmdType == 0 ? CommandType.StoredProcedure : CommandType.Text;//string.IsNullOrEmpty(CmdType)
            return con;
        }
        //public static T Exec<T>(string CmdStr, Dg<T> Ex, int CmdType = 0,int Cls = 1, string ConNm = null,  params object[] Ps)


        /*
        public static void ss(object[] a)
        {
            string k = "sql";
            if(C.hc.Session[k]==null)
            {//先假定数组长度为5
                C.hc.Session[k] = new object[5][];
            }
            (C.hc.Session[k] as object[5][])

            //   Session["myArray"] = new string[]{"Hello","World"};
      //  string[] myArray = Session["myArray"] as string[];
        }


        /// <summary>
        /// 将传入查询集合，集中查询之后返回dataset
        /// </summary>
        /// <returns></returns>

        public static DataSet cmds(object[] a) {

           

            DataSet ds = new DataSet();
            return ds;
        }*/
        public static T cmd<T>(string cmdStr, Dg<T> ex, int cmdType = 0, int cls = 1, string cn = "", params object[] ps)
        {
            T tmp = default(T);
            // string cmdStr = "";
            SqlConnection con = Db.con(cn);
            // SqlTransaction tr = con.BeginTransaction();
            SqlCommand cmd = con.CreateCommand(); //new SqlCommand(cmdStr, con, tr);
            cmd.CommandType = cmdType == 0 ? CommandType.StoredProcedure : CommandType.Text;//string.IsNullOrEmpty(CmdType)
            string[] cs = { cmdStr };
            bool isProc = cmdType == 0;
            cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;//string.IsNullOrEmpty(CmdType)

            DataSet ds = null;
            bool isDs = false;
            if (isProc && cmdStr.IndexOf(",") > 0)
            {
                cs = cmdStr.Split(',');
                if (typeof(T) == typeof(DataSet))
                {
                    ds = new DataSet();
                    isDs = true;
                }
            }
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();//有时报ConnectionString 属性尚未初始化。
                object[] p = ps;
                for (int i = 0; i < cs.Length; i++)
                {
                    if (isDs && ps != null)
                        p = ps[i] as object[];
                    cmd.CommandText = cs[i];
                    cmd.Parameters.Clear();
                    bool hsp = p != null && p.Length > 0;
                    int pi = -1;//输出参数在存储过程中的位置索引
                    if (hsp&&isProc)
                    {
                        pi = Db.Ps(cmd, p);
                    }
                    tmp = ex(cmd);
                    if (hsp && pi != -1)
                        p.SetValue(cmd.Parameters[pi + 1].Value, pi);
                    if (isDs)
                    {
                        DataSet d = tmp as DataSet;
                        DataTable t = d.Tables[0].Copy();
                        t.TableName = t.TableName + i.ToString();
                        ds.Tables.Add(t);//暂时假定每个存储过程只返回一个表
                    }

                    //if (i == 0 && cs.Length > 1 && tmp is DataSet)
                    //    tmp = new DataSet();
                }
                if (isDs)
                    tmp = (T)(object)ds; //(T)Convert.ChangeType(ds, typeof(T));

                cmd.Dispose();//相册页报错ConnectionString 属性尚未初始化
            }
            catch (SqlException e)
            {
                //待处理
                // if (typeof(T).ToString() != "System.Object[]")
                //if (typeof(T) == typeof(String))
                //    return (T)(object)String.Empty;
                HttpResponse rsp = C.hc.Response;
                rsp.Write("数据库操作异常：" + e.Message);//e.Message，跳404？
                rsp.End();
                tmp = default(T);//(T)C.Cvt<string>(e.Message);
            }
            finally
            {
                if (cls == 1)
                {
                    // Db.close(con);
                    tc.Remove(con.Database);
                    con.Close();
                    con.Dispose();

                }
                else if (!tc.ContainsValue(con)&&con.State==ConnectionState.Open)
                {
                    tc[con.Database] = con;
                }
            }

            return (T)tmp;
        }

        #region Install
        /*
        public static void SaveCon(SqlConnectionStringBuilder sb)
        {
            Configuration Cfg = WebConfigurationManager.OpenWebConfiguration("/");

            ConnectionStringsSection Css = Cfg.ConnectionStrings;

            //foreach(ConnectionStringSettings s in Css.ConnectionStrings)
            //{
            //    if(s.Name==sb.InitialCatalog)
            //    {
            //        return;
            //    }
            //}

            Css.ConnectionStrings.Add(new ConnectionStringSettings(sb.InitialCatalog,sb.ConnectionString));
            Cfg.Save();
        }
        public static void addConn(string ip, string db, string uid, string pwd)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = ip;
            sb.InitialCatalog = db;
            sb.UserID = uid;
            sb.Password = pwd;
            String CmdTxt = "";
            SqlConnection Con = new SqlConnection(sb.ConnectionString);
            SqlCommand Cmd = new SqlCommand(CmdTxt, Con);
            Db.SaveCon(sb);
            CmdTxt = sb.ConnectionString;
        }

        */
        #endregion
        /*分配事务   
            if (transaction != null)   
            {   
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        command.Transaction = transaction;   
            }
            */
        /*_用传入数组填充Cmc的参数集合_*/
        public static int Ps(SqlCommand Cmd, object[] Ps)
        {
            int n = -1;//目前仅一个输出参数，多个输出参数时此处需要改成数组
            //Cmd.Parameters.Clear();
            if (Ps.Length > 0)
            {
                SqlCommandBuilder.DeriveParameters(Cmd);
                for (int j = 0, i = 0; j < Cmd.Parameters.Count && i < Ps.Length; j++)
                {
                    SqlParameter Sp = Cmd.Parameters[j];
                    System.Data.ParameterDirection pd = Sp.Direction;
                    //if (Ps[i].ToString().StartsWith("@"))
                    if (Sp.Direction == ParameterDirection.InputOutput)
                    {
                        Sp.Direction = ParameterDirection.Output;
                        n = i;
                    }
                    if (Sp.Direction == ParameterDirection.Input || Sp.Direction == ParameterDirection.Output)//ParameterDirection.InputOutput
                    {
                        Sp.Value = Ps[i];
                        i++;
                    }
                }
            }
            return n;
        }

        //public static T Test<T>(T Str)
        //{
        //    return (T)Convert.ChangeType(Str, typeof(T));
        //}
        #region 执行Sql命令
        public static int Query(SqlCommand Cmd)
        {
            return Cmd.ExecuteNonQuery();
        }
        /*_获取带有返回值的存储过程的返回值_*/
        public static T Rv<T>(SqlCommand Cmd)
        {
            SqlParameter p = new SqlParameter("@return", SqlDbType.Int);
            p.Direction = ParameterDirection.ReturnValue;
            Cmd.Parameters.Add(p);
            //Cmd.Parameters.AddWithValue("@return",DBNull.Value);
            Cmd.ExecuteNonQuery();
            return (T)Cmd.Parameters["@return_value"].Value;
            //sqlCmd.Parameters.Add(new SqlParameter("@outResult", SqlDbType.VarChar，30));
        }
        public static object Scalar(SqlCommand Cmd)
        {
            return Cmd.ExecuteScalar();
        }
        public static T Scalar<T>(SqlCommand Cmd)
        {
            T tmp = default(T);
            object t = Cmd.ExecuteScalar();
            if(t!=null)
            {
                tmp = (T)t;
            }
            return tmp;
        }

        public static SqlDataReader Reader(SqlCommand Cmd)
        {
            return Cmd.ExecuteReader();
        }
        public static object[] Arr(SqlCommand Cmd)
        {
            using (SqlDataReader r = Cmd.ExecuteReader())
            {
                return C.ToArray(r);
            }

        }
        /*
        //实例化泛型：
        public static T Data<T>(SqlCommand Cmd)// where T : new()
        {
            SqlDataAdapter sda = new SqlDataAdapter(Cmd);
            //T tb = new T();
            T tb = Activator.CreateInstance<T>();
            sda.Fill(tb);
            return tb;
        }
        */
        public static DataTable Tb(SqlCommand Cmd)
        {
            SqlDataAdapter sda = new SqlDataAdapter(Cmd);
            DataTable tb = new DataTable();
            sda.Fill(tb);//ExecuteReader 要求已打开且可用的 Connection。连接的当前状态为打开。
            return tb;
            //using (SqlDataReader r = Cmd.ExecuteReader())
            //{
            //    return C.ToTable(r);
            //}
        }
        #region 用SqlDataReader将行的内容读入DataSet并返回
        public static DataSet DataSet(SqlCommand Cmd)
        {
            SqlDataAdapter sda = new SqlDataAdapter(Cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        #endregion
        /*
        public static List<object[]> List(SqlCommand Cmd)
        {
            using (SqlDataReader r = Cmd.ExecuteReader())
            {
                return C.ToList(r);
            }
        }*/

        public static XmlReader XmlReader(SqlCommand Cmd)
        {
            return Cmd.ExecuteXmlReader();
        }
        public delegate T Dg<T>(SqlCommand Cmd);
        //Func<string,string,string,object[],out int> Fun=Reader;
        /*_执行Sql命令_替换成cmd*/
        /*
        /// <summary>
        /// 执行数据库操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CmdStr"></param>
        /// <param name="Ex"></param>
        /// <param name="CmdType"></param>
        /// <param name="ConStr">config中连接串名称</param>
        /// <param name="Cls"></param>
        /// <param name="Ps"></param>
        /// <returns></returns>
        public static T Exec<T>(string CmdStr, Dg<T> Ex, int CmdType = 0, int Cls = 1, string ConNm = null, params object[] Ps)
        {
            T Tmp = default(T);
            //ConStr = string.IsNullOrEmpty(ConStr) ? ds : ConStr;
            SqlConnection Con = con(ConNm);
            SqlCommand Cmd = new SqlCommand(CmdStr, Con);
            Cmd.CommandType = CmdType == 0 ? CommandType.StoredProcedure : CommandType.Text;//string.IsNullOrEmpty(CmdType)
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                if (Ps.Length > 0)
                {
                    Db.Ps(Cmd, Ps);
                }
                Tmp = Ex(Cmd);
                if (Ps != null && Ps.Length > 1 && C.Lst(Ps).ToString().StartsWith("@"))
                {
                    Ps.SetValue(Cmd.Parameters[C.Lst(Ps).ToString()].Value, Ps.Length - 1);
                }
                Cmd.Dispose();
            }
            catch (SqlException e)
            {
                //待处理
                // if (typeof(T).ToString() != "System.Object[]")
                //if (typeof(T) == typeof(String))
                //    return (T)(object)String.Empty;


                HttpResponse rsp = C.hc.Response;
                rsp.Write("数据库操作异常：" + e.Message);//e.Message，跳404？
                rsp.End();
                Tmp = default(T);//(T)C.Cvt<string>(e.Message);
            }
            finally
            {
                if (Cls == 1)
                {
                    Con.Close();
                    Con.Dispose();
                }
            }
            return Tmp;
        }
        */
        /// <summary>
        /// 判断传入对象是否为DBNull，或转字符串后为null或空
        /// </summary>
        /// <param name="o"></param>
        /// <returns>bool</returns>
        public static bool isNull(object o)
        {
            return (o==null||o is DBNull || string.IsNullOrWhiteSpace(o.ToString()));
        }
    }
    #endregion
    #endregion
    #region 处理页面相关
    public class p
    {
        public string nm { get; set; }
        public string mst { get; set; }
        public string css { get; set; }
        public string js { get; set; }
        public DataTable ctrs { get; set; }

        public p()
        {
        }
        public p(string nm)
        {
            DataSet t = Db.cmd("ctrPg", Db.DataSet, ps: nm);
            if (t.Tables.Count > 1)
            {
                DataTable tp = t.Tables[0],
                               tc = t.Tables[1];
                DataRow pr = tp.Rows[0];
                mst = pr["mast"].ToString();
                css = pr["css"].ToString();
                js = pr["js"].ToString();
                ctrs = tc;
            }
            //if (!Db.isNull(pr["css"].ToString()))
            //{
            //    //StringBuilder s = new StringBuilder();
            //    //string css = tp.Rows[0]["css"].ToString();
            //    //string[] ss = css.Split(',');
            //    //// if (!Db.isNull(css))//好像可有可无
            //    //foreach (string pt in ss)
            //    //{
            //    //    s.Append(pt);
            //    //    //HtmlLink link = new HtmlLink();
            //    //    //link.Href = Convert.ToString(p[5]);
            //    //    //link.Attributes["rel"] = "stylesheet";
            //    //    //link.Attributes["type"] = "text/css";
            //    //    //link.Href = pt;
            //    //    //pg.Header.Controls.Add(link);
            //    //    //pg.Master.Master.FindControl("head").FindControl("head").Controls.Add(link);
            //    //}
            //    this.css = pr["css"].ToString();
            //}



            //for(int i=0;i<t.Tables.Count;i++)
            //{
            //    s += string.Concat(t.Tables[i].Rows[0].ItemArray)+"<br />";
            //}
            //s.Append("<br />C start_____" + t.Tables.Count + "_____<br />" + s + "C end<br />");

            //C.hc().Response.Write(s);
        }
        public static void pg(Page pg)//, string mst
        {
            //delegate int Mydelegate(int x, int y);
            // Mydelegate youdelegate = (x, y) => { return (x - y); };
            //pg.PreInit += delegate(object sender, EventArgs e)
            //{
            //    Page o = sender as Page;
            //    o.MasterPageFile = o.Request.QueryString["mst"];

            //};
            object[] p = Db.cmd<object[]>("pg", Db.Arr, ps: pg.Request.QueryString["nm"]);
            DataTable t = Db.cmd<DataTable>("ctrls", Db.Tb, ps: p[0]);
            Control cuc = null;
            Control pc = pg.Master.Master.FindControl("body").FindControl("m");
            //SortedList
            Dictionary<string, Control> ps = new Dictionary<string, Control>();
            foreach (DataRow r in t.Rows)
            {
                object prt = r["prtId"],
                    cont = r["cont"];
                string id = r["id"].ToString();
                int tid = Convert.ToInt32(r["typeId"]);//1、存储过程；2、文本内容；3、容器控件+存储过程、4、容器控件；
                bool nc = (cont is DBNull) || string.IsNullOrWhiteSpace(cont.ToString()),
                    hsp = !(prt is DBNull) && Convert.ToInt32(prt) > 0;
                if (tid == 4 && nc)// )|| tid == 3
                {
                    HtmlGenericControl g = new HtmlGenericControl(r["tg"].ToString());
                    g.Attributes.Add("class", r["cls"].ToString());
                    g.Attributes.Add("id", id);
                    g.ID = prt.ToString();
                    cuc = g;
                }
                else// if (tid == 1 || tid == 3)
                {
                    if (tid == 2)
                    {

                        uc bc = pg.LoadControl(typeof(uc), null) as uc;
                        //bc.prtid = Convert.ToInt32(prt);
                        //LiteralControl l = new LiteralControl();
                        //l.Text = cont.ToString();
                        //bc.Controls.Add(l);
                        bc.uctxt = cont.ToString();
                        cuc = bc;
                    }
                    else
                    {
                        cuc = (Control)pg.LoadControl(r["pt"].ToString());
                    }

                    Type ct = cuc.GetType();
                    PropertyInfo[] pInfo = ct.GetProperties();
                    foreach (PropertyInfo pi in pInfo)
                    {
                        bool isBs = pi.DeclaringType == typeof(uc);
                        string s = pi.Name;
                        //if ((pi.DeclaringType == ct || isBs) && pi.Name != "cId" && pi.Name != "sp" && pi.Name != "w" && t.Columns.Contains(s))
                        if ((pi.DeclaringType == ct || isBs) && pi.Name != "w" && (t.Columns.Contains(s) || s == "did"))//&& pi.Name != "sp"
                        {
                            if (s == "did")
                                s = "id";
                            object o = r[s];

                            if (o is string || o is int || o is bool)
                            {
                                if (s == "id")
                                    s = "did";
                                ct.GetProperty(s).SetValue(cuc, o, null);
                            }
                        }
                    }
                }
                cuc.ID = id;
                ps.Add(id, cuc);
            }

            //pg.LoadComplete += delegate(object sender, EventArgs e) {
            //    pg.Response.Write("complate"+pg.Title);
            //};

            foreach (KeyValuePair<string, Control> k in ps)
            {

                uc bc = k.Value as uc;
                if (bc != null && bc.prtid > 0)
                {
                    Control prtc = ps[bc.prtid.ToString()];
                    prtc.Controls.Add(bc);
                    //uc pbc = prtc as uc;
                    //if(pbc!=null&&pbc.prtid>0)
                }
                else
                {
                    pc.Controls.Add(k.Value);
                }
                //pg.Response.Write("K:" + k.Value.ID+"<br />");
            }
            if (!Db.isNull(p[5]))
                foreach (string pt in p[5].ToString().Split(','))
                {
                    HtmlLink link = new HtmlLink();
                    link.Href = Convert.ToString(p[5]);
                    link.Attributes["rel"] = "stylesheet";
                    link.Attributes["type"] = "text/css";
                    link.Href = pt;
                    pg.Header.Controls.Add(link);
                    //pg.Master.Master.FindControl("head").FindControl("head").Controls.Add(link);
                }
            if (!Db.isNull(p[6]))
            {
                string s = p[6].ToString();
                foreach (string j in s.Split(','))
                {
                    uc.Iptjs(pg, j);
                }
            }
        }
        public static void view(string nm)//, string mst
        {

        }
    }
    #endregion


    #region 处理控件相关
    /// <summary>
    /// 模块基类
    /// </summary>
    public class mo
    {
        //public string tg { get; set; }//标签名
        //public string cls { get; set; }//样式名
       public int id{ get; set; }//模块实例
        public string tit { get; set; }//模块标题
        public string url { get; set; }//标题url
        public string m { get; set; }//标题上的更多
       // public string m { get; set; }//temp
        //public int did { get; set; }//控件实例记录id
        public mo()
        {
            
         }
    }
    #endregion

    #region 处理控件相关
    /// <summary>
    /// 所有自定义控件的父类
    /// </summary>
    public class uc : UserControl
    {
        //Page p = null;
        public string tg { get; set; }//标签名
        public string cls { get; set; }//样式名
        public string tit { get; set; }//控件标题
        public string m { get; set; }//标题更多
        public string turl { get; set; }//标题url
        public int did { get; set; }//控件实例记录id
                                    //        public int did { get; set; }//控件实例记录id


        public int cid { get; set; }//控件数据源的类别id
        public int prtid { get; set; }//控件实例父控件id
        public int typeId { get; set; }//控件类型id：1、存储过程；2、文本内容；3、容器控件+存储过程、4、容器控件；

        //public int typeId { get; set; }//控件数据源的类别id
        public string uctxt { get; set; }

        public dynamic w { get; set; }//
        public uc(int i) { }
        public uc()
        {
            this.Load += baseControl_Load;//避免在load事件中绑定数据,还有就是MVC中部分视图好像不支持Load事件
                                          // baseControl_Load(this, null);
                                          //
                                          // TODO: 在此处添加构造函数逻辑
                                          //
        }
        //

        void baseControl_Load(object sender, EventArgs e)
        {
            //p = this.Page;
            w = this;//p
            string ap = "oid:" + did.ToString() + ",cid:" + cid.ToString() + ",tid:" + typeId.ToString() + ",tp:'" + this.GetType().ToString().Replace("ASP.", "") + "'";
            if (!string.IsNullOrWhiteSpace(this.tg))
            {
                w = new HtmlGenericControl(tg);
                //w.ID = id.ToString();
                //w.Attributes.Add("runat", "server");
                w.Attributes.Add("class", cls);
                w.Attributes.Add("name", "b");
                w.Attributes.Add("p", ap);
                this.Controls.Add(w);
                //this.prt = w as Control;
                Control ccc = this.FindControl("lst");
                if (ccc != null)
                {
                    this.w.Controls.Add(ccc);
                }
            }
            if (typeId == 2)
            {
                LiteralControl l = new LiteralControl();
                int i = uctxt.IndexOf(" ");
                l.Text = i > 0 ? (uctxt.Insert(i + 1, "p=\"" + ap + "\"")) : uctxt;
                w.Controls.Add(l);
            }
            if (!string.IsNullOrWhiteSpace(tit))
            {
                HtmlAnchor ti = new HtmlAnchor();
                ti.HRef = turl;
                ti.InnerText = tit;
                ti.Attributes.Add("id", did.ToString());//did
                ti.Attributes.Add("class", "ha");
                if (!string.IsNullOrWhiteSpace(m))
                {
                    HtmlGenericControl mi = new HtmlGenericControl("i");
                    mi.InnerText = m;
                    ti.Controls.Add(mi);
                }
                w.Controls.AddAt(0, ti);

            }
        }
        public static void Runjs(Control ctl, string js)
        {
            LiteralControl l = new LiteralControl();
            l.Text = js;
            ctl.Page.Master.Master.FindControl("body").FindControl("RunJs").Controls.Add(l);
        }
        public static void Iptjs(Control ctl, string src)
        {
            LiteralControl l = new LiteralControl();
            l.Text = "<script src='" + src + "'></script>";
            ctl.Page.Master.Master.FindControl("body").FindControl("IptJs").Controls.Add(l);
        }
        /// <summary>
        /// 将js文件的url拼成script标记并返回，多个时以英文逗号分隔
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string Iptjs(string src)
        {
            string t = "";
            if (!string.IsNullOrWhiteSpace(src))
            {
                foreach (string s in src.Split(','))
                {
                    t += string.Format("<script src=\"{0}\"></script>", s);
                }
            }
            return t;
        }
        public static void head(Control ctl, string src)
        {
            //HtmlLink l = new HtmlLink();
            //l.Href = "../App_Themes/edit.css";
            //l.Attributes.Add("rel", "stylesheet");
            LiteralControl l = new LiteralControl();
            l.Text = "<link href='" + src + "' rel=\"stylesheet\">";
            //ctl.Page.Master.Master.FindControl("head").FindControl("head").Controls.Add(l);
            ctl.Page.Master.Master.FindControl("head").FindControl("head").Controls.Add(l);
        }
    }

    #endregion
    #region 处理母版相关
    public class baseMaster : MasterPage
    {
        public baseMaster()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public virtual IList mst
        {
            get { return this.ContentPlaceHolders; }
        }
        //ContentPlaceHolder控件的ID值不区别大小写,而
        //public virtual string cph
        //{
        //    get {

        //        string s = "";
        //        foreach (Control control in this.Master.Controls)
        //        {
        //            if (control is ContentPlaceHolder)
        //            {
        //                // s += control.ID+"<br />";
        //                foreach (Control con in control.Controls)
        //                {
        //                    if (con is ContentPlaceHolder)
        //                    {
        //                        s += con.ID;
        //                    }
        //                }
        //            }
        //        }
        //        return s;
        //    }
        //}
    }
    #endregion
    #region 采集处理html的类
    public class picker
    {
        public HttpWebResponse rsp;
        public Stream strm;
        public string charset, html, title, body, time, author, origin, rs;
        public picker(string url, string encoding = "", int times = 5000)
        {
            //WebClient wc=new WebClient();
            //wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1)");
            //wc.Encoding=UnicodeEncoding.Default;
            //string s=wc.DownloadString(url);

            HttpWebRequest rqt = WebRequest.Create(url) as HttpWebRequest;
            //rqt.KeepAlive = true;
            //rqt.ContentType = "application/json; charset=utf-8";
            rqt.Timeout = times;
            rqt.UserAgent = "MSIE 7.0; Windows NT 5.1";
            //Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.96 Safari/537.36
            try
            {

                this.rsp = rqt.GetResponse() as HttpWebResponse;//ISO-8859-1GB2312

                charset = string.IsNullOrWhiteSpace(encoding) ? (rsp.CharacterSet == "ISO-8859-1" ? "GB2312" : string.IsNullOrWhiteSpace(rsp.CharacterSet) ? "UTF-8" : rsp.CharacterSet) : encoding;
               // rsp.ContentType = "application/json; charset=utf-8";
                this.strm = rsp.GetResponseStream();
                this.rs = new StreamReader(this.strm).ReadToEnd();
                // ServicePointManager.DefaultConnectionLimit = 200;
            }
            catch (WebException e)
            {
                //[WebException: 远程服务器返回错误: (404) 未找到。]
                //this.rsp = rqt.GetResponse() as HttpWebResponse;操作超时
                //if (resp.StatusCode != HttpStatusCode.OK) //如果服务器未响应，那么继续等待相应
                //    continue;
                ;
            }
            finally
            {
                //rsp.Close();
                //strm.Dispose();
                //strm.Close();
            }
        }
        public void Html()
        {
            StreamReader oStreamRd = new StreamReader(strm, Encoding.GetEncoding(charset));
            //this.html = oStreamRd.ReadToEnd();
            this.html = Regex.Replace(tagToLower(oStreamRd.ReadToEnd()), @"\n|\r|\t", "");
            //this.html = Regex.Replace(tagToLower(oStreamRd.ReadToEnd()), @"&(quot|#34);", "\"", RegexOptions.IgnoreCase); ;
            oStreamRd.Close();
            rsp.Close();
            strm.Dispose();
            strm.Close();
        }
        public List<string> gHrefs(string start, string end, string last)
        {
            this.Html();
            int j = this.html.IndexOf(start) + start.Length,
                k = this.html.Substring(j).IndexOf(end);
            string reg = "<a.*?href=\"(.*?)\".*?>.*?</a>",
                pp = this.html.Substring(j, k);
            MatchCollection matches = Regex.Matches(pp, reg, RegexOptions.IgnoreCase);
            List<string> hrefs = new List<string>();
            foreach (Match m in matches)
            {
                string h = m.Groups[1].Value;
                if (last != "" && h == last)
                {
                    break;
                }
                hrefs.Add(h);
            }
            return hrefs;
        }
        //正则的匹配模式：缺省或0为不启用，1为启用排除模式，2为选取模式.//原为0为排除模式，1为选取模式

        public void gHtml(string start, string end, string reg, int pattern = 1, string prefix = "<title>", string suffix = "</title>")//bool inner = true, 
        {
            Html();
            int a = this.html.IndexOf(prefix) + prefix.Length,
        b = this.html.IndexOf(suffix, a),
        c = this.html.IndexOf(start, b) + start.Length,
        d = this.html.IndexOf(end, c);
            //if (inner)
            //{
            //    c = c + start.Length;
            //}
            //int d = this.html.IndexOf(end, c)-end.Length;
            this.title = this.html.Substring(a, b - a);
            this.body = this.html.Substring(c, d - c);
            if (pattern == 1)
            {
                this.body = Regex.Replace(Regex.Replace(this.body, reg, ""), @"<[^/]\w*[^\>]*\>\s*</\w*[^\>]*\>", "");
            }
            else if (pattern == 2)
            {
                string s = "";
                MatchCollection matches = Regex.Matches(this.body, reg, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    s += match.Value;
                }
                this.body = s;
            }
            //this.body = filter(this.html.Substring(c, d - c), reg, pattern);
        }
        //规范化标签书写，并过滤指定标签

        //该方法已废弃
        public string filter(string os, string reg, int pattern)
        {
            //reg = @"<[img][^\>]*\>|\<a[^\>]*\>|\</a\>|\s[^\>]*|\<!--/?[^-->]*-->|<\w*[^\>]\>\s*</\w*[^\>]\>";
            //string o=Regex.Replace(tagToLower(os), "\n|\r|" + reg, "");
            string rst = "";
            if (pattern == 0)
            {
                rst = Regex.Replace(Regex.Replace(os, reg, ""), @"<[^/]\w*[^\>]*\>\s*</\w*[^\>]*\>", "");
                //rst = Regex.Replace(Regex.Replace(tagToLower(os), "\n|\r|" + reg, ""), @"<[^/]\w*[^\>]*\>\s*</\w*[^\>]*\>", "");
            }
            else
            {
                MatchCollection matches = Regex.Matches(os, reg, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    rst += match.Value;
                }
            }
            return rst;
        }
        //将大写标签转为小写
        public string tagToLower(string htmls)
        {
            MatchCollection matchs = Regex.Matches(htmls, @"(?<tag><[^\s>]+\s)|(?<tag><[^\s>]+>)");
            foreach (Match match in matchs)
            {
                string tag = match.Value.ToLower();
                htmls = htmls.Replace(match.Value, tag);
            }
            return htmls;
        }
        /// <summary>
        /// 根据传入的文章对象保存至connectionStrings的名称指定的数据库
        /// </summary>
        /// <param name="Csn">连接字符串的名称</param>
        /// <returns></returns>
        public int Save(string Csn, picker p)
        {
            int i = 0;
            if (this.body != null)
            {
                i = Db.cmd("addArticle", Db.Query, 1, 1, Db.conStr(Csn), C.Arr(p.title, p.body, "", ""));
            }
            return i;
        }
        //public int getMedia(string url,string path)
        //{
        //    Bitmap b = new Bitmap(new picker(url).strm);
        //    b.Save(path);
        //}

    }


    #endregion
    #region 计时器
    public class timer
    {
        private System.Threading.Timer t;
        public System.Threading.Timer T
        {
            get { return t; }
            set { t = value; }
        }
        static Dictionary<string, timer> ls = new Dictionary<string, timer>();
        public static Dictionary<string, timer> Ls
        {
            get { return timer.ls; }
            set { timer.ls = value; }
        }
        //private AutoResetEvent e = new AutoResetEvent(false);解决方法重入问题
        public timer(TimerCallback cb, string[] arg, string id, int due, int period)
        {
            t = new System.Threading.Timer(cb, arg, due, period);
            ls.Add(id, this);
        }
        public static int stop(string id)
        {
            int r = 0;
            if (Ls.ContainsKey(id))
            {
                Ls[id].t.Dispose();
                Ls.Remove(id);
                r = 1;
            }
            return r;
        }
    }
    #endregion
    #region wcf
    //public class wcf
    //{
    //    public wcf() {

    //    }
    #endregion
    #region 任务
    public class task
    {
        public task()
        {

        }
    }
    #endregion
    #region 自定义控件相关
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Ca runat=server></{0}:Ca>")]
    public class Ca : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? "[" + this.ID + "]" : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write("AAAAAAAAAAAA");
        }
    }
    #endregion
    #endregion
}