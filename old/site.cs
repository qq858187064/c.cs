using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Data;

using System.Collections;



namespace C
{
    #region 处理页面相关
    public class p
    {
        public p()
        {
        }
        //protected override void OnPreInit(EventArgs e)
        //{
        //    base.OnPreInit(e);
        //}
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
                        if ((pi.DeclaringType == ct || isBs)  && pi.Name != "w" && (t.Columns.Contains(s) || s == "did"))//&& pi.Name != "sp"
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
            if(!Db.isNull(p[5]))
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
           if(!Db.isNull(p[6]))
           {
               string s = p[6].ToString();
               foreach(string j in s.Split(','))
               {
                   uc.Iptjs(pg, j);
               }
           }
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
        public string tg { get; set; }//样式名
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

        public uc()
        {
            this.Load += baseControl_Load;//避免在load事件中绑定数据
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
}
