using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using System.Drawing.Imaging;

namespace Multiple_1
{
    public partial class Form1 : Form
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients= new Dictionary<int, TcpClient>();
        int count_of_client;
        static TcpListener ServerSocket;
        string localIP = string.Empty;

        private Bitmap FullScreenShot()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            return bitmap;
        }

        public Form1()
        { 
            InitializeComponent();
        }

        private void create_server()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localIP), 12348);
            ServerSocket = new TcpListener(localEndPoint);
            ServerSocket.Start();
            for (int i = 0; i < count_of_client; i++)
            {
                Thread newThread = new Thread(new ThreadStart(Listeners));
                newThread.Start();
                newThread.IsBackground = true;
            }
        }

        static void Listeners()
        {
            Socket socketForClient = ServerSocket.AcceptSocket();
            MessageBox.Show("Connected Service");
            if (socketForClient.Connected)
            {
                try
                {
                    NetworkStream ns = new NetworkStream(socketForClient);
                    System.IO.BinaryWriter Writer = new System.IO.BinaryWriter(ns);
                    while (true)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Rectangle bounds = Screen.GetBounds(Point.Empty);
                            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
                            Graphics g = Graphics.FromImage(bitmap);
                            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                            bitmap.Save(ms, ImageFormat.Jpeg);
                            byte[] buffer = new byte[ms.Length];
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.Read(buffer, 0, buffer.Length);
                            Writer.Write(buffer.Length);
                            Writer.Write(buffer);
                            ms.Close();
                        }
                    }
                }catch(Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            count_of_client = Convert.ToInt32(textBox1.Text);
            create_server();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry hostname = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in hostname.AddressList)
            {
                localIP = ip.ToString();
            }
            this.Text = this.Text + "|" + localIP;
        }
    }
}
