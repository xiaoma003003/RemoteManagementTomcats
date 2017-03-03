using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.IO;
using deno.util;

namespace deno
{
    public partial class home : Form
    {
        DBHelper db = null;
        SCHelper sc = null;
        FileHelper fh = null;
        String conf = null;
        LogHelper log = new LogHelper();
        public home()
        {
            InitializeComponent();
            //打开数据库
            db = new DBHelper();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            conf = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("bin\\", "").Replace("Debug\\", "") + "conf\\deno.conf";
            fh = new FileHelper();
            List<string> strList = fh.getListFromFile(conf);
            foreach (string str in strList){
                if (str.IndexOf("port") >=0) {
                    this.txtboxPort.Text = str.Replace("port:","");
                }
                if (str.IndexOf("host") >= 0)
                {
                    this.txtboxIP.Text = str.Replace("host:", "");
                }
            }
        }
        //开启tomcat
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt;
            dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id", CommandType.Text);
            foreach (DataRow dr2 in dt.Rows)
            {
                string str = null;
                int port = int.Parse(dr2["port"].ToString());
                string host = dr2["ipaddress"].ToString();
                log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                sc = new SCHelper(port, host);
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
                        //发送启动命令
                        string reslut = sc.sendCommond(sendStr);
                        log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + reslut);
                        textBox1.AppendText("开启tomcat:[" + dr2["tomcatname"] + "]命令已发出。\r\n");
                        System.Threading.Thread.Sleep(1000);

                    }
                }
                log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                str = sc.sendCommond("exit");
                log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                sc.socketClose();
            }
        }
       
        //关闭全部tomcat
        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt;
            dt = db.ExecuteQuery("SELECT tl.id,tl.lujing,tl.tomcatname,tl.lujingstop,ts.ipaddress,ts.port,ts.name,ts.operatingsystem FROM tomcatlist tl, serverslist ts WHERE tl.serverid=ts.id", CommandType.Text);
            foreach (DataRow dr2 in dt.Rows)
            {
                string str = null;
                int port = int.Parse(dr2["port"].ToString());
                string host = dr2["ipaddress"].ToString();
                log.writeLog(System.DateTime.Now + "  开始连接服务端...");
                sc = new SCHelper(port, host);
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
                        string reslut = sc.sendCommond(sendStr);
                        log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + reslut);
                        textBox1.AppendText("关闭tomcat:[" + dr2["tomcatname"] + "]命令已发出。\r\n");
                        System.Threading.Thread.Sleep(1000);
                  
                    }
                }
                log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                str = sc.sendCommond("exit");
                log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                sc.socketClose();
            }
        }
        //打开tomcat列表
        private void button7_Click(object sender, EventArgs e)
        {
            TomcatList fm = new TomcatList(); //新建一个窗口
            fm.Owner = this;
            this.Hide(); //隐藏现在这个窗口 
            fm.Show();//新窗口显现
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<string> strList = new List<string>();
            string host = txtboxIP.Text;
            String port = txtboxPort.Text;
            if (host != null && !host.Equals("") && UtilsHelper.IsIPSect(host))
            {
                strList.Add("host:" + host);
                strList.Add("port:" + port);
                fh.writeFileFromList(strList,conf);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void txtboxPort_TextChanged(object sender, EventArgs e)
        {
            List<string> strList = new List<string>();
            string host = txtboxIP.Text;
            String port = txtboxPort.Text;
            strList.Add("host:" + host);
            strList.Add("port:" + port);
            fh.writeFileFromList(strList, conf);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
        //服务器列表
        private void button2_Click(object sender, EventArgs e)
        {
            serverslist fm = new serverslist(); //新建一个窗口
            fm.Owner = this;
            this.Hide(); //隐藏现在这个窗口 
            fm.Show();//新窗口显现
        }
    }
}
