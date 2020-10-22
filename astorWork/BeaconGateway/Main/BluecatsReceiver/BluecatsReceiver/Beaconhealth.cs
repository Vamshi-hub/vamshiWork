using BluecatsLibrary;
using BluecatsLibrary.Helper;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace BluecatsReceiver
{
    public partial class Beaconhealth : Form
    {
        MQTTClient mQTTClient;
        //UDPClient mQTTClient;

        public Beaconhealth()
        {
            new BluecatsLibrary.Helper.MQTTClient();
            InitializeComponent();
            
            this.WindowState = FormWindowState.Maximized;
        }

        private void Beaconhealth_Load(object sender, EventArgs e)
        {
            btnStop.Visible = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            txtEndPoin.Enabled = false;
            btnStop.Visible = true;
            try
            {
                if (txtEndPoin.Text == string.Empty)
                {
                    MessageBox.Show("Please Enter EndPoint");
                    return;
                }
                else
                {
                    mQTTClient = new BluecatsLibrary.Helper.MQTTClient(txtEndPoin.Text);
                    //mQTTClient = new BluecatsLibrary.Helper.UDPClient(txtEndPoin.Text, 1883);
                    Task.Factory.StartNew(() => { mQTTClient.Subcribe(); });
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            mQTTClient.Disconnect();
            btnConnect.Enabled = true;
            txtEndPoin.Enabled = true;
            btnStop.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                app.Visible = true;
                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;
                worksheet.Name = "Beaconhealth";
                for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                }
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j]?.Value?.ToString();
                    }
                }
                string ExcelSheets = Application.StartupPath + @"\ExcelSheets";// your code goes here

                bool exists = System.IO.Directory.Exists(ExcelSheets);

                if (!exists)
                    System.IO.Directory.CreateDirectory(ExcelSheets);
                workbook.SaveAs(ExcelSheets + @"\" + $"{DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")}.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workbook.Close(null);
                app.Quit();
            }
            catch(Exception ex)
            {

            }
        }
       
    }
}
