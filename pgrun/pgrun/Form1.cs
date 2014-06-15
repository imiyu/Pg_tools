using System;
using System.Configuration;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
namespace pgrun
{
    public partial class Form1 : Form
    {
        string result="";
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnPgbin_Click(object sender, EventArgs e)
        {
            getpath(txtPgbin);
        }
        public void getpath(TextBox tb)
        {
            folderBrowserDialog1.ShowDialog();
            if (String.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
                return;
            tb.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnPgdata_Click(object sender, EventArgs e)
        {
            getpath(txtPgdata);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPgdata.Text)) return;
            txtCmd.Text = "";
            //pg_ctl start -W -D "%CD%/pgsql/data" -l "%CD%/pgsql/logfile" 
            txtCmd.Text = String.Format("pg_ctl start -D \"{0}\"", txtPgdata.Text);
        }

        private void btnRuncmd_Click(object sender, EventArgs e)
        {
            if (!txtCmd.Text.StartsWith("pg_ctl "))
            {
                MessageBox.Show("命令错误！");
                return;
            }
            Thread t1 = new Thread(new ThreadStart(runcmd));
            t1.Start();
            //runcmd("");
        }
        public void runcmd()
        {
            try
            {
                string exe = txtPgbin.Text + "\\pg_ctl.exe";
                if (!File.Exists(exe))
                {
                    throw new ApplicationException("Can not find: " + exe);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(txtCmd.Text.Substring(6));
                //sb.Append(" yr");
                //textBox2.AppendText(sb.ToString() + "\r\n");
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = exe;
                proc.StartInfo.Arguments = sb.ToString();
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd() + "\r\n" + proc.StandardError.ReadToEnd();
                if (String.IsNullOrEmpty(output) == false)
                    result += output+"\n";
                    //MessageBox.Show(output);
                //    txtResult.AppendText(output + "\r\n");
                proc.WaitForExit();
                //proc.BeginOutputReadLine();
                proc.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtResult.Text = result;
            if (!File.Exists(txtPgbin.Text + "\\pg_ctl.exe")) return;
            if (String.IsNullOrEmpty(txtPgdata.Text)) return;
            toolStripStatusLabel1.Text = "状态：" + runcmd(String.Format("status -D \"{0}\"",txtPgdata.Text)).Split('\r')[0];
        }
        public string runcmd(string arge)
        {
            try
            {
                string result = "";
                string exe = txtPgbin.Text + "\\pg_ctl.exe";
                if (!File.Exists(exe))
                {
                    throw new ApplicationException("Can not find: " + exe);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(arge);
                //sb.Append(" yr");
                //textBox2.AppendText(sb.ToString() + "\r\n");
                //txtResult.Text = "";
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = exe;
                proc.StartInfo.Arguments = sb.ToString();
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd() + "\r\n" + proc.StandardError.ReadToEnd();
                if (String.IsNullOrEmpty(output) == false)
                    result += output + "\r\n";
                    //txtResult.AppendText(output + "\r\n");
                proc.WaitForExit();
                //proc.BeginOutputReadLine();
                proc.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            txtCmd.Text =String.Format( "pg_ctl -D \"{0}\" stop -m immediate",txtPgdata.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            result = "";
            txtResult.Text = "";
        }

        private void cmbCmd_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbCmd.Text)
	        {
                case "help" :
                    txtCmd.Text = "pg_ctl --help";
                    break;
                case "Init":
                    txtCmd.Text = String.Format("pg_ctl init -D \"{0}\" ", txtPgdata.Text);
                    break;
                case "regServer":
                    txtCmd.Text =String.Format("pg_ctl register -N PostgreSQL -D \"{0}\" ", txtPgdata.Text);
                    break;
                case "unregServer":
                    txtCmd.Text = "pg_ctl unregister -N PostgreSQL";
                    break;
		        default:
                    txtCmd.Text = cmbCmd.Text;
                    break;
	        }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ConfigurationSettings.AppSettings[]
            txtPgbin.Text= ConfigurationManager.AppSettings["pgbin"];
            txtPgdata.Text = ConfigurationManager.AppSettings["pgdata"];
        }

        private void txtPgbin_TextChanged(object sender, EventArgs e)
        {
            config.AppSettings.Settings["pgbin"].Value = txtPgbin.Text; 
            config.Save(ConfigurationSaveMode.Modified);  
            //ConfigurationManager.RefreshSection("appSettings");   
        }

        private void txtPgdata_TextChanged(object sender, EventArgs e)
        {
            config.AppSettings.Settings["pgbin"].Value = txtPgbin.Text;
            config.Save(ConfigurationSaveMode.Modified);  
        }

    }
}
