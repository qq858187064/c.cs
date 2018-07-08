#region Usings
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Helpers;
using System.Web.Configuration;
using System.Xml;
using System.Net;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.ServiceModel;

#endregion

using System.Reflection;

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
    #region Db数据库相关
    public class Db
    {
        private static string ds;
        public static string Ds
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DefaultDb"]].ConnectionString;
            }
            set
            {
                Db.ds = value;
            }
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

            return C.ToArray(Cmd.ExecuteReader());
        }
        public static DataTable Tb(SqlCommand Cmd)
        {
            return C.ToTable(Cmd.ExecuteReader());
        }
        public static List<object[]> List(SqlCommand Cmd)
        {
            return C.ToList(Cmd.ExecuteReader());
        }
        public static XmlReader XmlReader(SqlCommand Cmd)
        {
            return Cmd.ExecuteXmlReader();
        }
        public delegate T Dg<T>(SqlCommand Cmd);
        //Func<string,string,string,object[],out int> Fun=Reader;

        /*_执行Sql命令_*/
        public static T Exec<T>(string CmdStr, Dg<T> Ex,int Cls = 1, string ConStr = null, string CmdType = null,params object[] Ps) 
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
            }
            catch (SqlException e)
            {
                if(typeof(T).ToString()!="System.Object[]")
                Tmp = (T)C.Cvt<string>(e.Message);
            }
            finally
            {
                if (Cls == 1)
                    Con.Close();
            }
            return Tmp;
        }
    }
        #endregion
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
            rqt.UserAgent="MSIE 7.0; Windows NT 5.1";
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
            if(pattern==1)
            {
                this.body = Regex.Replace(Regex.Replace(this.body, reg, ""), @"<[^/]\w*[^\>]*\>\s*</\w*[^\>]*\>", "");
            }
            else if (pattern == 2)
            {
                string s = "";
                MatchCollection matches = Regex.Matches(s, reg, RegexOptions.IgnoreCase);
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
                i = Db.Exec("addArticle", Db.Query, 1,Db.conStr(Csn), Ps: C.Arr(p.title, p.body, "", ""));
            }
            return i;
        }

    }


    #endregion
    #region C公用类

    public class C
    {
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
         public static object ExecuteMethod<T>(string pUrl,string pMethodName, params object[] pParams)
        {
            EndpointAddress address = new EndpointAddress(pUrl);
            System.ServiceModel.Channels.Binding bindinginstance = null;
            NetTcpBinding ws = new NetTcpBinding();
            ws.MaxReceivedMessageSize = 20971520;
            ws.Security.Mode = SecurityMode.None;
            bindinginstance = ws;
            using (ChannelFactory<T> channel = new ChannelFactory<T>(bindinginstance,address))
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




        #region 获取并返回当前HttpContext对象
        public static HttpContext hc()
        {
            return HttpContext.Current;
        }
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
            return Arr[Arr.Length-1];
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
    }
    #endregion
    #region 计时器
    public class timer
    {
        private Timer t;
        public Timer T
        {
            get{return t;}
            set{t = value;}
        }
        static Dictionary<string, timer> ls = new Dictionary<string, timer>();
        public static Dictionary<string, timer> Ls
        {
            get { return timer.ls; }
            set { timer.ls = value; }
        }
        //private AutoResetEvent e = new AutoResetEvent(false);解决方法重入问题
        public timer(TimerCallback cb, string[] arg,string id,int due,int period)
        {
            t = new Timer(cb, arg, due, period);
            ls.Add(id, this);
        }
        public static int stop(string id)
        {
            int r = 0;
            if(Ls.ContainsKey(id))
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
        public task() {
        
        }
    }
    #endregion

    #region 用于处理http的类
    public class h
    {
        public System.Drawing.Image i;
        public h(string url)
        {
            HttpWebRequest q = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse p = q.GetResponse() as HttpWebResponse;
            this.i = System.Drawing.Image.FromStream(p.GetResponseStream());
        }
    }

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
         */
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
}