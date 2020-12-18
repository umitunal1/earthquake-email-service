using System;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace KandilliAlarmer
{
    public partial class Form1 : Form
    {
        string LastDate = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://www.koeri.boun.edu.tr/scripts/lst7.asp");
            GetLastDate();
        }

        private void GetData()
        {
            webBrowser1.Navigate("http://www.koeri.boun.edu.tr/scripts/lst7.asp");
            Thread.Sleep(2000);
            foreach (HtmlElement element in webBrowser1.Document.GetElementsByTagName("pre"))
            {
                using (StreamWriter writer = new StreamWriter("C:\\kand\\kandilli.txt"))
                {
                    writer.Write(element.InnerHtml, false);
                }
            }
            Thread.Sleep(1000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (HtmlElement element in webBrowser1.Document.GetElementsByTagName("pre"))
            {
                using (StreamWriter writer = new StreamWriter("C:\\kand\\kandilli.txt"))
                {
                    writer.Write(element.InnerHtml, false);
                }
            }
            Thread.Sleep(1000);
            timer1.Start();
            this.WindowState = FormWindowState.Minimized;
        }

        private void GetLastDate()
        {
            using (StreamReader reader = new StreamReader("C:\\kand\\kandilli.txt"))
            {
                for (int i = 0; i < 10; i++)
                {
                    var line = reader.ReadLine();
                    if (i == 6)
                    {
                        LastDate = line.Substring(0, 20);
                        break;
                    }
                }
            }
        }

        private void CheckEarthQuake()
        {
            using (StreamReader reader = new StreamReader("C:\\kand\\kandilli.txt"))
            {
                for (int i = 0; i < 10; i++)
                {
                    var line = reader.ReadLine();
                    if (i == 6)
                    {
                        string loc = line.Substring(70, 30);
                        string ml = line.Substring(58, 6);
                        string date = line.Substring(0, 20);
                        double x = Convert.ToDouble(ml.Replace(" ", ""));
                        string message = "";
                        if (LastDate != date)
                        {
                            if (x >= 4.0) // 4'ten büyük depremleri bildir
                            {
                                message = "Meydana Geldigi Tarih : " + date + Environment.NewLine + "Meydana geldiği yer: " + loc + Environment.NewLine + " Şiddet: " + ml;
                                SendMail("umit.unal@zerodensity.tv", message);
                            }
                            LastDate = date;
                        }
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
        }
        private void SendMail(string to, string message)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("senderemail", "senderemailpassowrd");
            MailMessage mm = new MailMessage("donotreply@domain.com", to, "Deprem bildirim sistemi", message);
            //mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Deprem kontrol ediliyor");
            GetData();
            CheckEarthQuake();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
