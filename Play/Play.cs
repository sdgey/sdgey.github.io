using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Play
{
    public partial class Play : Form
    {
        public Play()
        {
            InitializeComponent();
        }

        private string Key = "";

        //禁止通过拖动，双击标题栏改变窗体大小。
        public const int WM_NCLBUTTONDBLCLK = 0xA3;
        const int WM_NCLBUTTONDOWN = 0x00A1;
        const int HTCAPTION = 2;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCLBUTTONDOWN && m.WParam.ToInt32() == HTCAPTION)
                return;
            if (m.Msg == WM_NCLBUTTONDBLCLK)
                return;
            base.WndProc(ref m);
        }
        private HookKeyBoard hkb = null;

        //引入API函数
        [DllImport("user32 ")]
        //这个是调用windows的系统锁定
        public static extern bool LockWorkStation();
        [DllImport("user32.dll")]
        static extern void BlockInput(bool Block);




        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue("BaichuiMonitor", Application.ExecutablePath);//"BaichuiMonitor"可以自定义
            //FileStream fs = new FileStream(System.IO.Path.Combine(Environment.SystemDirectory, "taskmgr.exe"), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            ControlBox = false; //不在窗体标题栏中显示控件
            ShowCursor(0);
            DisabledMouseKey();
            Thread.Sleep(20000);
            //while (true)
            //{
            //    Play p = new Play();
            //    p.ShowDialog();
            //}
            //DialogResult t = MessageBox.Show("程序加载失败！", "警告", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
            //if (t == DialogResult.Retry)
            //Thread.Sleep(10000);
            //return;
            //try
            //{
            //    //bat文件路径
            //    string path = "D:\\BC\\Tool\\1.bat";
            //    Process pro = new Process();
            //    FileInfo file = new FileInfo(path);
            //    pro.StartInfo.WorkingDirectory = file.Directory.FullName;
            //    pro.StartInfo.FileName = path;
            //    pro.StartInfo.CreateNoWindow = false;
            //    pro.Start();
            //    pro.WaitForExit();
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show("执行失败! 错误原因:" + ex.Message);
            //}

        }

        private void DisabledMouseKey()
        {
            hkb = new HookKeyBoard();
            hkb.keyeventhandler += new KeyEventHandler(keyhandler);
            hkb.InstallHook(this);
            HookKeyBoard.tagMSG Msgs;
            while (HookKeyBoard.GetMessage(out Msgs, IntPtr.Zero, 0, 0) > 0)
            {
                HookKeyBoard.TranslateMessage(ref Msgs);
                HookKeyBoard.DispatchMessage(ref Msgs);
            }
        }

        private void EnableMouseKey()
        {
            hkb.Hook_Clear();
        }

        /// <summary>
        /// 设立一个口子，以防禁用完还得重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyhandler(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Key) && e.KeyCode == Keys.Enter)
                if (Key.Equals("TANG"))
                    Application.ExitThread();
            Key += e.KeyData.ToString();
            if (e.KeyCode == Keys.Delete)
                Key = "";
            if (e.KeyCode == Keys.Escape)
                Application.ExitThread();
            if (e.KeyData.ToString() == "b" || e.KeyData.ToString() == "B")
            {
                hkb.Hook_Clear();
                ShowCursor(1);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.FromArgb(11, 118, 216), ButtonBorderStyle.Solid);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        /// <summary>
        /// 控制鼠标指针显示和隐藏
        /// </summary>
        /// <summary>
        /// 调用系统API函数操作鼠标指针
        /// </summary>
        /// <param name="status">0表示隐藏鼠标指针，1表示显示鼠标指针</param>
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern void ShowCursor(int status);

    }
}