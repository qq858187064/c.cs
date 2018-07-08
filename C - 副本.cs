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
        public static HttpContext hc()
        {
            return HttpContext.Current;
        }
        //HttpContext hc { get { return HttpContext.Current; } }

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

        #region 用SqlDataReader将行的内容读入Array并返回
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
        #endregion

        #region 用SqlDataReader将行的内容读入List并返回
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


        //将DataTable类型转化为指定类型的实体集合
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
        public static T toModel<T>(DataRow r)
        {
            T o = Activator.CreateInstance<T>();
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

        #region 未审核的________Cookie和Session操作

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <returns></returns>
        public static object GetSession(string key)
        {
            HttpContext context = C.hc();
            if (context == null || context.Session == null)
                return string.Empty;
            return context.Session[key];
        }
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key">session名称</param>
        /// <param name="data">存储数据</param>
        public static void SetSession(string key, object data)
        {
            HttpContext context = C.hc();
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
            HttpContext context = C.hc();
            HttpCookie cookie = context.Request.Cookies[key];
            if (context == null || cookie == null)
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
            HttpContext context = C.hc();
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
        public static string GetQueryString(string key)
        {
            HttpContext context = C.hc();
            if (context == null)
                return string.Empty;

            string result = string.Empty;
            foreach (string item in context.Request.QueryString.Keys)
            {
                if (item.ToLower() == key.ToLower())
                {
                    result = context.Request.QueryString[key];
                    break;
                }
            }
            return result;
        }
        //HttpContext hc { get { return HttpContext.Current; } }

        #endregion


    }
    #region 验证类手机号、邮箱
    public class validate
    {
        private static string mb = @"^1[34578]+\d{9}$";
        private static string em = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
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
    }
        #endregion
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























        #region 用于处理string的类

        public class str
        {
            #region 将传入的null，转成空字符串""
            /// <summary>
            ///将传入的null转换成空字符串
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static string toEmpty(string str)
            {

                if (str == null)
                    str = "";
                return str;
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
           /// <summary>
        /// 通用返回值
        /// </summary
        public class result<T>
        {
            /// <summary>
            /// 通用返回值
            /// </summary
            //public int code { get; set; }
            private int Code;
            public int code { get { return Code; } set { Code = value; this.msg = er[value]; } }
            /// <summary>
            /// 错误消息
            /// </summary>
            public string msg { get; set; }
            public T data { get; set; }



            String[] er = { "操作失败", "操作成功", "账号不存在", "账号已存在", "账号或密码不正确", "验证码错误", "票据过期，请重新登录", "权限不足" };
            //public result(int code,String msg, T data = default(T))
            //{
            //    this.code = code;
            //    this.msg = msg;
            //    this.data = data;
            //}
            public result(int code, T data = default(T))
            {
                this.code = code;
                this.msg = code > -1 ? er[code] : "未标识的异常";
                this.data = data;
                //if(data!=null)
                //this.data = JavaScriptConvert.SerializeObject(data);








                //    // 从一个对象信息生成Json串
                //public static string ObjectToJson(object obj)
                //{
                //    return JavaScriptConvert.SerializeObject(obj);
                //}
                //// 从一个Json串生成对象信息
                //public static object JsonToObject(string jsonString, object obj)
                //{
                //    return JavaScriptConvert.DeserializeObject(jsonString, obj.GetType());
                //}







            }
            //用户相关code描述，从

            /// <summary>
            /// jsonp返回
            /// </summary>
            public static void Cb<T>(T r)
            {
                T arg = (T)r;
                String s = "Cb(" + arg + ")";
                Rt(s);
            }
            public static void Rt<T>(T r)
            {
                HttpContext hc = C.hc();
                hc.Response.Write(r);
            }
            public static void Rt(int r)
            {
                HttpContext hc = C.hc();
                hc.Response.Write(r);
            }
        }
        #endregion
        #region 用户实体类
        public class u
        {
            //Uid Email   Rid phone
            public int id { get; set; }
            public int rid { get; set; }
            public string mail { get; set; }
            public string phone { get; set; }
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



            public class json
            {
                /// <summary>
                /// 将对象序列化（serialize）成json字符串
                /// </summary>
                public static string ser<T>(T o)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new DataContractJsonSerializer(typeof(T)).WriteObject(ms, o);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                /// <summary>
                /// 将json字符串反序列化成对象
                /// </summary>
                public static T deser<T>(string json)
                {
                    //双引号问题未解决
                    //json = json.Replace('\"', '\"');
                    // json = json.replace(/\"/gi,"""");
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        return (T)(new DataContractJsonSerializer(typeof(T)).ReadObject(ms));
                    }
                }
            }
             ------------------------------_________________________________________________   */























        #region 用于处理http的类
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
            private static string ds;
            /// <summary>
            /// 获取cfg中默认连接串
            /// </summary>
            public static string Ds
            {
                get
                {
                    return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DefaultDb"]].ConnectionString;
                }
                //set
                //{
                //    Db.ds = value;
                //}
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
            /// <summary>
            /// 根据连接串名称初始化SqlCommand
            /// </summary>
            /// <param name="ConnectionName"></param>
            public Db(string ConnectionName)
            {
                String CmdTxt = "";
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
                SqlCommand Cmd = new SqlCommand(CmdTxt, Con);
                Cmd.CommandType = CommandType.StoredProcedure;
            }
            public static string conStr(string csNm)
            {
                return ConfigurationManager.ConnectionStrings[csNm].ConnectionString;
            }
            /*_用传入数组填充Cmc的参数集合_*/
            public static void Ps(SqlCommand Cmd, object[] Ps)
            {
                Cmd.Parameters.Clear();
                if (Ps.Length > 0)
                {
                    SqlCommandBuilder.DeriveParameters(Cmd);
                    for (int j = 0, i = 0; j < Cmd.Parameters.Count && i < Ps.Length; j++)
                    {
                        SqlParameter Sp = Cmd.Parameters[j];
                        if (Sp.Direction == ParameterDirection.Input || Sp.Direction == ParameterDirection.InputOutput)
                        {
                            Sp.Value = Ps[i];
                            i++;
                        }
                    }
                }
            }
            public static T Test<T>(T Str)
            {
                return (T)Convert.ChangeType(Str, typeof(T));
            }
            #region 执行Sql命令
            public static int Query(SqlCommand Cmd)
            {
                return Cmd.ExecuteNonQuery();
            }
            //public static DataSet qr(SqlCommand Cmd)
            //{
            //    return Cmd.ExecuteNonQuery();
            //}
            /*_获取带有返回值的存储过程的返回值_*/
            public static object Rv(SqlCommand Cmd)
            {
                SqlParameter p = new SqlParameter("@return", SqlDbType.Int);
                p.Direction = ParameterDirection.ReturnValue;
                Cmd.Parameters.Add(p);
                Cmd.ExecuteNonQuery();
                return Cmd.Parameters["@return_value"].Value;
                //sqlCmd.Parameters.Add(new SqlParameter("@outResult", SqlDbType.VarChar，30));
            }
            public static object Scalar(SqlCommand Cmd)
            {
                return Cmd.ExecuteScalar();
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
            //实例化泛型的两种方法：
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
                sda.Fill(tb);
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

            /*_执行Sql命令_*/
            public static T Exec<T>(string CmdStr, Dg<T> Ex, int Cls = 1, string ConStr = null, string CmdType = null, params object[] Ps)
            {
                T Tmp = default(T);
                ConStr = string.IsNullOrEmpty(ConStr) ? Db.Ds : ConStr;
                SqlConnection Con = new SqlConnection(ConStr);
                SqlCommand Cmd = new SqlCommand(CmdStr, Con);
                Cmd.CommandType = string.IsNullOrEmpty(CmdType) ? CommandType.StoredProcedure : CommandType.Text;
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
                    //待处理：无法将类型为“System.String”的对象强制转换为类型“System.Data.DataTable”。
                    if (typeof(T).ToString() != "System.Object[]")
                        //    HttpContext.Current.Response.Write(e.Message);
                        //return default(T);
                        Tmp = (T)C.Cvt<string>(e.Message);
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
            /// <summary>
            /// 判断传入对象是否为DBNull，或转字符串后为null或空
            /// </summary>
            /// <param name="o"></param>
            /// <returns>bool</returns>
            public static bool isNull(object o)
            {
                return (o is DBNull || string.IsNullOrWhiteSpace(o.ToString()));
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
                DataSet t = Db.Exec("ctrPg", Db.DataSet, Ps: nm);
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
                object[] p = Db.Exec<object[]>("pg", Db.Arr, Ps: pg.Request.QueryString["nm"]);
                DataTable t = Db.Exec<DataTable>("ctrls", Db.Tb, Ps: p[0]);
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
            public string charset,
                             html, title, body, time, author, origin;
            public picker(string url, string encoding = "", int times = 5000)
            {
                //WebClient wc=new WebClient();
                //wc.Encoding=UnicodeEncoding.Default;
                //string s=wc.DownloadString(url);

                HttpWebRequest rqt = WebRequest.Create(url) as HttpWebRequest;
                rqt.Timeout = times;
                rqt.UserAgent = "MSIE 7.0; Windows NT 5.1";
                this.rsp = rqt.GetResponse() as HttpWebResponse;//ISO-8859-1
                charset = encoding == "" ? (rsp.CharacterSet == "ISO-8859-1" ? "GB2312" : rsp.CharacterSet) : encoding;
                this.strm = rsp.GetResponseStream();
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
                    i = Db.Exec("addArticle", Db.Query, 1, Db.conStr(Csn), Ps: C.Arr(p.title, p.body, "", ""));
                }
                return i;
            }

        }


        #endregion
        #region 计时器
        public class timer
        {
            private Timer t;
            public Timer T
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
                t = new Timer(cb, arg, due, period);
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