using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace 校园网客户端
{
    public partial class client : Form
    {
        private Boolean autoFlag;
        private String dir;
        private Boolean isLogin;
        private String code;
        private String loginTime;
        private String serveTime;
        private String useFlow;
        private String useTime;
        private String ip;
        private int thisTimeUse;
        private List<user> userList;

        public client()
        {
            if (!Directory.Exists("D:\\Program Files\\校园网客户端"))
            {
                Directory.CreateDirectory("D:\\Program Files\\校园网客户端");
            }
            
                dir = "D:\\Program Files\\校园网客户端\\hautSrunUserMessage.bin";
            InitializeComponent();
            userList = new List<user>();
            this.StartPosition = FormStartPosition.CenterScreen;
            ReserializeMethod();
            ClientInit();
        }

        private void welcome()
        {
            if (!File.Exists("D:\\Program Files\\校园网客户端\\clientMessage.bin"))
            {
                System.Diagnostics.Process.Start("http://srun.zhihao.online");
                FileStream fs = new FileStream("D:\\Program Files\\校园网客户端\\clientMessage.bin", FileMode.Create);
            }
        }

        private void ReserializeMethod()
        {
            if (File.Exists(dir))
            {
                using (FileStream fs = new FileStream(dir, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    userList = bf.Deserialize(fs) as List<user>;
                }
                comboBox1.Text=userList[0].getUserCode();
                if (userList != null)
                {
                    for(int i = 0; i < userList.Count; i++)
                    {
                        comboBox1.Items.Add(userList[i].getUserCode());
                    }
                    textBox2.Text = userList[0].getUserPasswd();
                    checkBox1.CheckState = CheckState.Checked;
                }
            }
            else
            {
                clearPasswd.Visible = false;
            }
        }

        private void SerializeMethod()
        {
            using (FileStream fs = new FileStream(dir, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, userList);
                clearPasswd.Visible = true;
            }  
        }

        private void init()
        {
            String data = "";
            String url = "http://172.16.154.130/cgi-bin/rad_user_info";
            String back = HttpServer.RequestWebAPI(url, data, "GET");
            if (!back.Equals("not_online"))
            {
                isLogin = true;
                button2.Text = "注销";
                button1.Text = "刷新";
                string[] str = back.Split(',');
                {
                    code = str[0];
                    loginTime = str[1];
                    serveTime = str[2];
                    useFlow = str[6];
                    useTime = str[7];
                    ip = str[8];
                }
                messageInit();
                messagePanel.Visible = true;
            }
            else
            {
                isLogin = false;
                messagePanel.Visible = false;
            }
        }

        private void ClientInit()
        {
            String data = "";
            String url = "http://172.16.154.130/cgi-bin/rad_user_info";
            String back = HttpServer.RequestWebAPI(url, data, "GET");
            if (!back.Equals("not_online"))
            {
                isLogin = true;
                button2.Text = "注销";
                button1.Text = "刷新";
                string[] str = back.Split(',');
                {
                    code = str[0];
                    loginTime = str[1];
                    serveTime = str[2];
                    useFlow = str[6];
                    useTime = str[7];
                    ip = str[8];
                }
                messageInit();
                messagePanel.Visible = true;
            }
            else
            {
                
                isLogin = false;
                messagePanel.Visible = false;
                if (File.Exists("D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin"))
                {
                    
                    autoFlag = true;
                    checkBox2.CheckState = CheckState.Checked;
                    button2.PerformClick();
                    //登陆
                    data = encrypt.getUrlData(comboBox1.Text, textBox2.Text);
                    url = "http://172.16.154.130:69/cgi-bin/srun_portal";
                    back = HttpServer.RequestWebAPI(url, data, "POST");
                    if (back.Contains("login_ok"))
                    {
                        MessageBox.Show("登陆成功");
                        welcome();
                        init();
                        button1.Text = "刷新";
                    }
                 }
            }
            
            
        }

        private void messageInit()
        {
            ip_label.Text = ip;
            thisTimeUse = (int)(double.Parse(serveTime)-double.Parse(loginTime));
            thisTime_label.Text = getTime(thisTimeUse);
            totalTime_label.Text = getTime(int.Parse(useTime));
            double flow = double.Parse(useFlow);
            flow /= (1024*1024*1024);
            useFlow_label.Text = flow.ToString("f3")+"GB";
        }

        Boolean hasItem(String code)
        {
            for(int i = 0; i < userList.Count; i++)
            {
                if (userList[i].getUserCode().Equals(code))
                {
                    if (!userList[i].getUserPasswd().Equals(textBox2.Text))
                    {
                        userList[i].setUserPasswd(textBox2.Text);
                        SerializeMethod();

                        comboBox1.Items.Clear();
                        comboBox1.Text = userList[0].getUserCode();
                        if (userList != null)
                        {
                            for (int x = 0; x < userList.Count; x++)
                            {
                                comboBox1.Items.Add(userList[x].getUserCode());
                            }
                            textBox2.Text = userList[0].getUserPasswd();
                            checkBox1.CheckState = CheckState.Checked;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isLogin)
            {
                init();
                MessageBox.Show("刷新成功");
            }
            else
            {
                String data = encrypt.getUrlData(comboBox1.Text, textBox2.Text);
                String url = "http://172.16.154.130:69/cgi-bin/srun_portal";
                String back = HttpServer.RequestWebAPI(url, data, "POST");
                if (back.Contains("login_ok"))
                {
                    if (checkBox1.CheckState == CheckState.Checked && !hasItem(comboBox1.Text))
                    {
                        user u = new user(comboBox1.Text, textBox2.Text);
                        userList.Add(u);
                        user x = userList[0];
                        userList[0] = u;
                        userList[userList.Count - 1] = x;
                        SerializeMethod();


                        comboBox1.Items.Clear();
                        comboBox1.Text = userList[0].getUserCode();
                        if (userList != null)
                        {
                            for (int i = 0; i < userList.Count; i++)
                            {
                                comboBox1.Items.Add(userList[i].getUserCode());
                            }
                            textBox2.Text = userList[0].getUserPasswd();
                            checkBox1.CheckState = CheckState.Checked;
                        }
                        
                    }

                    if (checkBox2.CheckState == CheckState.Checked)
                    {
                        FileStream fs = new FileStream("D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin", FileMode.Create);

                    }
                    else
                    {
                        if (File.Exists(@"D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin"))
                        {
                            //如果存在则删除
                            File.Delete(@"D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin");
                        }
                    }
                    

                    MessageBox.Show("登陆成功");
                    welcome();
                    init();
                    button1.Text = "刷新";
                }
                else
                {
                    MessageBox.Show("登录失败，错误原因:"+back);
                }
            }
        }

        private string getTime(int t)
        {
            //计算小时,用毫秒总数除以(1000*60*24),后去掉小数点
            int hour = t / (60 * 60);
            //计算分钟,用毫秒总数减去小时乘以(1000*60*24)后搜索,除以(1000*60),再去掉小数点
            int min = (t - hour * (60 * 60)) / (60);
            //同上
            int sec = (t - hour * (60 * 60) - min * (60));
            //拼接字符串
            String timeString = hour.ToString() + "小时:" + min.ToString() + "分钟:" + sec.ToString() + "秒";
            return timeString;
        }

        private void client_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isLogin)
            {
                button2.Text = "注销中";
                String data = "action=logout&ac_id=1&username=" + code + "&mac=&type=2";
                String url = "http://172.16.154.130:69/cgi-bin/srun_portal";
                String back = HttpServer.RequestWebAPI(url, data, "POST");
                if (back.Contains("logout_ok"))
                {
                    
                    System.Threading.Thread.Sleep(1000);
                    init();
                    button1.Text = "登陆";
                    button2.Text = "服务";
                    MessageBox.Show("注销成功");
                }
            }
            else
            {
                System.Diagnostics.Process.Start("http://172.16.154.130:8800/");
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            thisTimeUse++;
            thisTime_label.Text = getTime(thisTimeUse);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = userList[comboBox1.SelectedIndex].getUserPasswd();
            
            checkBox1.CheckState = CheckState.Checked;

        }

        private void clearPasswd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            userList = new List<user>();
            comboBox1.Items.Clear();
            textBox2.Text = "";
            if (File.Exists(@dir))
            {
                //如果存在则删除
                File.Delete(@dir);
            }
            if (File.Exists(@"D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin"))
            {
                //如果存在则删除
                File.Delete(@"D:\\Program Files\\校园网客户端\\clientAutoLoginMessage.bin");
            }
            clearPasswd.Visible = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://srun.zhihao.online");
        }
    }
}
