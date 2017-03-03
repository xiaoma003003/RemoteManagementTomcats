using deno.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deno
{
    public partial class TomcatList : Form
    {
        DBHelper db = null;
        String conf = null;
        SCHelper sc = null;
        FileHelper fh = null;
        string port = null;
        string host = null;
        LogHelper log = new LogHelper();
        SynchronizationContext syn = null;
        public TomcatList()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //打开数据库
            db = new DBHelper();
            this.ControlBox = false;
        }
        //获取配置文件信息
        private void getConf()
        {
            //获取服务端配置文件信息
            conf = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("bin\\", "").Replace("Debug\\", "") + "conf\\deno.conf";
            fh = new FileHelper();
            List<string> strList = fh.getListFromFile(conf);
            foreach (string str2 in strList)
            {
                if (str2.IndexOf("port") >= 0)
                {
                    this.port = str2.Replace("port:", "");
                }
                if (str2.IndexOf("host") >= 0)
                {
                    this.host = str2.Replace("host:", "");
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            syn = SynchronizationContext.Current;
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.FullRowSelect = true;
            //添加列  
            listView1.Columns.Add("编号", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("名称", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("路径", 230, HorizontalAlignment.Left);
            listView1.Columns.Add("状态", 100, HorizontalAlignment.Left);  
            //获取tomcat列表线程    
            Thread myLoadThread = new Thread(this.loadTomcatThread);
            myLoadThread.Start();
            //更新tomcat状态线程
            Thread myUpdataTomcatStatesThread = new Thread(this.updataTomcatStatesThread);
            myUpdataTomcatStatesThread.Start();
        }
        //加载tomcat线程
        private void loadTomcatThread() {
            syn.Post(new SendOrPostCallback((o) => {
                DataTable dt;
                dt = db.ExecuteQuery("SELECT id,lujing,lujingstop,tomcatname,serverid FROM tomcatlist", CommandType.Text);
                int num = 1;
                foreach (DataRow dr2 in dt.Rows)
                {
                    //添加行  
                    var item = new ListViewItem();
                    item.ImageIndex = num;
                    item.Text = dr2["id"].ToString();
                    item.SubItems.Add(dr2["tomcatname"].ToString());
                    item.SubItems.Add(dr2["lujing"].ToString());
                    item.SubItems.Add("更新中");
                    listView1.BeginUpdate();
                    listView1.Items.Add(item);
                    listView1.Items[listView1.Items.Count - 1].EnsureVisible();
                    listView1.EndUpdate();
                    num += 1;
                }
            }), null);
        }
        //更新tomcat状态线程
        private void updataTomcatStatesThread()
        {
            syn.Post(new SendOrPostCallback((o) => { updateAllStates(); }), null);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.ContextMenuStrip = contextMenuStrip;
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                log.writeLog(System.DateTime.Now + "  开始关闭tomcat...");
                int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
                //根据Id获取tomcat信息
                DataTable dt;
                string str;
                dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id AND  tl.id=" + id, CommandType.Text);
                foreach (DataRow dr2 in dt.Rows)
                {
                    log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                    sc = new SCHelper(int.Parse(dr2["port"].ToString()), dr2["ipaddress"].ToString());
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  hi");
                    str = sc.sendCommond("hi");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    if (str != null && str.Equals("hi"))
                    {
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  executeCommand");
                        str = sc.sendCommond("executeCommand");
                        log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        if (str != null && str.StartsWith("ok"))
                        {
                            //tomcat路径
                            str = dr2["lujingstop"].ToString();
                            //获取关闭tomcat命令
                            string sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getStopTomcatCommond(str);
                            log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                            str = sc.sendCommond(sendStr);
                            log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        }
                    }
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                    str = sc.sendCommond("exit");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    sc.socketClose();
                }
            }
        }
        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
                log.writeLog(System.DateTime.Now + "  开始启动tomcat...");
                //根据Id获取tomcat信息
                DataTable dt;
                string str;
                dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id AND  tl.id=" + id, CommandType.Text);
                foreach (DataRow dr2 in dt.Rows)
                {
                    log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                    sc = new SCHelper(int.Parse(dr2["port"].ToString()), dr2["ipaddress"].ToString());
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  hi");
                    str = sc.sendCommond("hi");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    if (str != null && str.Equals("hi"))
                    {
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  executeCommand");
                        str = sc.sendCommond("executeCommand");
                        log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        if (str != null && str.StartsWith("ok"))
                        {
                            //tomcat路径
                            str = dr2["lujing"].ToString();
                            //获取tomcat启动命令
                            string sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getStartTomcatCommond(str);
                            log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                            str = sc.sendCommond(sendStr);
                            log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        }
                    }
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                    str = sc.sendCommond("exit");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    sc.socketClose();
                }
            }
        }
        private void 重启ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
                log.writeLog(System.DateTime.Now + "  开始重启tomcat...");
                //根据Id获取tomcat信息
                DataTable dt;
                string str;
                dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id AND  tl.id=" + id, CommandType.Text);
                foreach (DataRow dr2 in dt.Rows)
                {
                    //发送关闭命令
                    string sendStr = null;
                    log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                    sc = new SCHelper(int.Parse(dr2["port"].ToString()), dr2["ipaddress"].ToString());
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  hi");
                    str = sc.sendCommond("hi");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    if (str != null && str.Equals("hi"))
                    {
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  executeCommand");
                        str = sc.sendCommond("executeCommand");
                        log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        if (str != null && str.StartsWith("ok"))
                        {
                            //tomcat路径
                            str = dr2["lujingstop"].ToString();
                            //获取关闭tomcat命令
                            sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getStopTomcatCommond(str);
                            log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                            str = sc.sendCommond(sendStr);
                            log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                    //发送启动命令
                    str = dr2["lujing"].ToString();
                    //获取tomcat启动命令
                    sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getStartTomcatCommond(str);
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                    str = sc.sendCommond(sendStr);
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                    str = sc.sendCommond("exit");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    sc.socketClose();
                }
            }
        }
        private void 刷新状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
                string status = checkTomcatStatus(id);
                this.listView1.SelectedItems[0].SubItems[3].Text = status;
            }
        }
        private string checkTomcatStatus(int id)
        {
            //根据Id获取tomcat信息
            log.writeLog(System.DateTime.Now + "  开始刷新tomcat状态...");
            DataTable dt;
            string str;
            dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id AND  tl.id=" + id, CommandType.Text);
            foreach (DataRow dr2 in dt.Rows)
            {
                //发送关闭命令
                log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                sc = new SCHelper(int.Parse(dr2["port"].ToString()), dr2["ipaddress"].ToString());
                log.writeLog(System.DateTime.Now + "  向服务端发送消息：  hi");
                str = sc.sendCommond("hi");
                log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                if (str != null && str.Equals("hi"))
                {
                    log.writeLog(System.DateTime.Now + "  向服务端发送消息：  getresponse");
                    str = sc.sendCommond("getresponse");
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    if (str != null && str.StartsWith("ok"))
                    {
                        //tomcat路径
                        str = dr2["lujingstop"].ToString();
                        //str = "D:\\apache-tomcat-7.0.62\\conf\\";
                        //获取tocat配置文件
                        string sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getTomcatServerXml(str);
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                        str = sc.sendCommond(sendStr);
                        log.writeLog(System.DateTime.Now + "  服务端返回消息：  " + str);
                        //提取端口信息
                        string pattern = @"<Connector port=""(\d+)""";
                        Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                        Match m = r.Match(str.ToString());
                        MatchCollection matchs = r.Matches(str);
                        //如果匹配的tomcat为空，则返回未知
                        if (matchs.Count <= 0)
                        {
                            return "未知";
                        }
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            Match match = matchs[i];
                            //match.Value是匹配的内容
                            // Console.WriteLine(match.Value);
                            string name = match.Groups[1].Value;
                            log.writeLog(System.DateTime.Now + "  匹配到端口：  " + name);
                            log.writeLog(System.DateTime.Now + "  开始验证端口是否开放：  " + name);
                            sendStr = "command" + "netstat -ano | findstr " + name;
                            log.writeLog(System.DateTime.Now + "  向服务端发送消息：  " + sendStr);
                            str = sc.sendCommond(sendStr);
                            log.writeLog(System.DateTime.Now + "  服务端返回消息：  " + str);
                            //判断端口是否被监听
                            bool l = Regex.IsMatch(str, @"([\s\S]*?):" + name + "(.*?)LISTENING");
                            if (!l)
                            {
                                return "关闭";
                            }
                        }
                    }
                }
                log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                str = sc.sendCommond("exit");
                log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                sc.socketClose();
            }
            return "开启";
        }
    
        //点击更新按钮更新状态
        private void button2_Click(object sender, EventArgs e)
        {
            updateAllStates();
        }
        private void updateAllStates()
        {
            int count = listView1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                int id = int.Parse(listView1.Items[i].SubItems[0].Text.ToString());
                string status = checkTomcatStatus(id);
                listView1.Items[i].SubItems[3].Text = status;
                System.Threading.Thread.Sleep(1000);
            }
        }
       
    }
}
