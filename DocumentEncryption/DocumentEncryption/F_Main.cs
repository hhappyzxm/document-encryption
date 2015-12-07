using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentEncryption
{
    public partial class F_Main : Form
    {
        public F_Main()
        {
            InitializeComponent();
        }

        private DataTable GenerateDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("AccountName");
            dt.Columns.Add("Website");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Password");
            dt.Columns.Add("TradePassword");
            dt.Columns.Add("Comment");

            return dt;
        }

        public class AccountViewModel
        {
            public string AccountName { get; set; }
            public string WebSite { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string TradePassword { get; set; }
            public string Comment { get; set; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataGridViewRow row in dgvAccount.Rows)
                {
                    if (row.Index == dgvAccount.Rows.Count - 1)
                    {
                        continue;
                    }

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        sb.Append(cell.Value + (cell.ColumnIndex == row.Cells.Count - 1 ? string.Empty : "|"));
                    }

                    if (row.Index < dgvAccount.Rows.Count - 2)
                    {
                        sb.Append("$");
                    }
                }
                

                using (FileStream fs = (FileStream) saveFileDialog1.OpenFile())
                {
                    var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    var estr = Convert.ToBase64String(bytes);
                    var ebytes = Encoding.UTF8.GetBytes(estr);
                    fs.Write(ebytes, 0, ebytes.Length);
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                using (StreamReader fileStream = new StreamReader(openFileDialog1.FileName))
                {
                    var str = fileStream.ReadToEnd();
                    str = Encoding.UTF8.GetString(Convert.FromBase64String(str));

                    var dt = GenerateDataTable();

                    foreach (var strRow in str.Split('$'))
                    {
                        var newRow = dt.NewRow();

                        var i = 0;
                        foreach (var strColumn in strRow.Split('|'))
                        {
                            newRow[i] = strColumn;

                            i++;
                        }

                        dt.Rows.Add(newRow);
                    }

                    dgvAccount.DataSource = dt;
                }
            }
        }
    }
}
