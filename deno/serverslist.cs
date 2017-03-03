using deno.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace deno
{
    public partial class serverslist : Form
    {
        DBHelper db = null;
        LogHelper log = new LogHelper();
        SCHelper sc = null;
        public serverslist()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //打开数据库
            db = new DBHelper();
            this.ControlBox = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.ContextMenuStrip = contextMenuStrip;
        }
        //页面加载完成后更新tomcat状态
        private void serverslist_Shown(object sender, EventArgs e)
        {
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.FullRowSelect = true;
            //添加列  
            listView1.Columns.Add("编号", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("名称", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("系统", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("地址", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("端口", 100, HorizontalAlignment.Left);
            DataTable dt;
            dt = db.ExecuteQuery("SELECT id,ipaddress,port,name,operatingsystem FROM serverslist", CommandType.Text);
            int num = 1;
            foreach (DataRow dr2 in dt.Rows)
            {
                //添加行  
                var item = new ListViewItem();
                item.ImageIndex = num;
                item.Text = dr2["id"].ToString();
                item.SubItems.Add(dr2["name"].ToString());
                item.SubItems.Add(dr2["operatingsystem"].ToString());
                item.SubItems.Add(dr2["ipaddress"].ToString());
                item.SubItems.Add(dr2["port"].ToString());
                listView1.BeginUpdate();
                listView1.Items.Add(item);
                listView1.Items[listView1.Items.Count - 1].EnsureVisible();
                listView1.EndUpdate();
                num += 1;
            }
        }
        //重启
        private void restartServer(object sender, EventArgs e)
        {
            string str = null;
            int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
            //根据Id获取tomcat信息
            DataTable dt;
            dt = db.ExecuteQuery("SELECT id,ipaddress,port,name,operatingsystem FROM serverslist WHERE id=" + id, CommandType.Text);
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
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  shutdown -s -t 1");
                        //获取重启命令
                        string sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getRestartServer();
                        str = sc.sendCommond(sendStr);
                        MessageBox.Show("重启服务器命令已发出", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                    }
                }
                log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
                str = sc.sendCommond("exit");
                log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                sc.socketClose();
            }
        //关机
        private void shudownServer(object sender, EventArgs e)
        {
            string str = null;
            int id = int.Parse(this.listView1.SelectedItems[0].SubItems[0].Text);
            //根据Id获取tomcat信息
            DataTable dt;
            dt = db.ExecuteQuery("SELECT id,ipaddress,port,name,operatingsystem FROM serverslist WHERE id=" + id, CommandType.Text);
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
                        log.writeLog(System.DateTime.Now + "  向服务端发送消息：  shutdown -s -t 1");
                        //获取关机命令
                        string sendStr = new CDHelper(dr2["operatingsystem"].ToString()).getMakeCommond().getStopServer();
                        str = sc.sendCommond(sendStr);
                        MessageBox.Show("关闭服务器命令已发出", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
                }
            }
            log.writeLog(System.DateTime.Now + "  向服务端发送消息：  exit");
            str = sc.sendCommond("exit");
            log.writeLog(System.DateTime.Now + "  服务端返回消息消息：  " + str);
            sc.socketClose();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void serverslist_Load(object sender, EventArgs e)
        {

        }
    }
}
