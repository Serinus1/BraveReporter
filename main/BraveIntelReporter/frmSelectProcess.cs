using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BraveIntelReporter
{
    public partial class frmSelectProcess : Form
    {
        private string ProcessName = string.Empty;
        public Process SelectedProcess = null;

        public frmSelectProcess(string processname)
        {
            InitializeComponent();
            ProcessName = processname;
        }

        private void frmSelectProcess_Load(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses().Where(p => p.ProcessName.ToLower() == ProcessName.ToLower()).ToList();
            lstProcesses.DisplayMember = "MainWindowTitle";
            lstProcesses.ValueMember = "Id";
            foreach (Process p in processes)
            {
                lstProcesses.Items.Add(p);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select one and only one window.");
                return;
            }
            SelectedProcess = (Process)lstProcesses.SelectedItem;
            this.Close();
        }
    }
}
