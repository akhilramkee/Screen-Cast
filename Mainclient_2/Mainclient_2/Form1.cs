using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace Mainclient_2
{
    public partial class Form1 : Form
    {
        NetworkStream ns;
        IPAddress ip;
        int port;
        Image im;
        TcpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        public void start_client()
        {
            if (textBox1.Text != "")
            {
                string ipAddr = textBox1.Text;
                IPAddress ip = IPAddress.Parse(ipAddr);
                port = 12348;
                IPEndPoint localEndPoint = new IPEndPoint(ip, port);
                client = new TcpClient();
                client.Connect(ipAddr, port);
                MessageBox.Show("Connected to server");
                pictureBox1.Top = this.Top;
                pictureBox1.Size = this.Size;
                button1.Visible = false;
                textBox1.Visible = false;
                ns=client.GetStream();
                Thread th = new Thread(new ThreadStart(trreadimage));
                th.Start();
                th.IsBackground=true;
            }
        }

        public void trreadimage()
        {
            try
            {
                    BinaryReader reader = new BinaryReader(ns);
                    int ctBytes = reader.ReadInt32();
                    using (MemoryStream ms = new MemoryStream(reader.ReadBytes(ctBytes)))
                    {
                        im = Image.FromStream(ms);
                        pictureBox1.Image = im;
                        ms.Close();
                        ms.Dispose();
                    }
            }
            catch(Exception ee)
            {
                GC.Collect(0);
            }
            trreadimage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start_client();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
