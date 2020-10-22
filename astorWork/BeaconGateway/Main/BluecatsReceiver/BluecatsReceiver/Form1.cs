using System;
using System.Windows.Forms;
using BluecatsLibrary.Helper;

namespace BluecatsReceiver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            
            InitializeComponent();
            
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (IPEndPoint.Text == string.Empty)
            {
                MessageBox.Show("Please Enter EndPoint");
                return;
            }
            else
            {
                UDPClient uDPClient = new UDPClient(IPEndPoint.Text, 1883);
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (IPEndPoint.Text == string.Empty)
                {
                    MessageBox.Show("Please Enter EndPoint");
                    return;
                }
                else
                {
                    MQTTClient mQTTClient = new BluecatsLibrary.Helper.MQTTClient(IPEndPoint.Text);
                    MessageBox.Show("Connection Established");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        

        private void button2_Click(object sender, EventArgs e)
        {

            BluecatsLibrary.Helper.MQTTClient mQTTClient = new BluecatsLibrary.Helper.MQTTClient();
            mQTTClient.Send(textBox1.Text).Wait();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (IPEndPoint.Text == string.Empty)
            {
                MessageBox.Show("Please Enter EndPoint");
                return;
            }
            else
            {
                //BluecatsLibrary.Helper.MQTTClient mQTTClient = new BluecatsLibrary.Helper.MQTTClient();
                //Task.Factory.StartNew(() => { mQTTClient.Subcribe(txtBeaconData); });
            }
         
        }

        public void LoadData (string strTopic,string strMessage)
        {
            string message= strTopic +"_"+ strMessage;
            txtBeaconData.Text += message;
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}
