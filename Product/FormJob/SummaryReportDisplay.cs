using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class SummaryReportDisplay : UserControl
    {
        public SummaryReportDisplay()
        {
            InitializeComponent();
        }

        public void LoadDataToDisplay(string FolderName, string FileName)
        {
            try
            {
                using (StreamReader srSummary = new StreamReader(FolderName + "\\" + FileName))
                {
                    richTextBoxSummaryReport.Clear();
                    richTextBoxSummaryReport.Text = srSummary.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));              
            }
        }
    }
}
