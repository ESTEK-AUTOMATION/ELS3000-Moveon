using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product.FormConsumablePart
{
    public partial class MaintenanceCountItem : UserControl
    {
        public string ItemName = "";
        public int CurrentCountValue = 0;
        public int CurrentCountValue_Backup = 0;
        public int CleanCountValue = 0;
        public int WarningCountValue = 0;
        public int DueCountValue = 0;
        public int MinimumCleanCount = 0;
        public int nSettingCleanCountValue = 0;
        bool isValueChange = false;

        public MaintenanceCountItem()
        {
            InitializeComponent();
        }
        public void updateInterface()
        {
            label1.ForeColor = Color.Black;
            label1.Text = ItemName;

            numericUpDownCleanCount.Value = nSettingCleanCountValue;

            numericUpDownCurrentCount.Value = CurrentCountValue;
            numericUpDownWarningCount.Value = WarningCountValue;
            numericUpDownDueCount.Value = DueCountValue;

            progressBarClean.Minimum = MinimumCleanCount;
            progressBarClean.Maximum = CleanCountValue;
            progressBarWarningDue.Minimum = 0;
            progressBarWarningDue.Maximum = DueCountValue;

            labelCleanCount.Text = (CurrentCountValue - MinimumCleanCount).ToString();
            //progressBarWarning.Minimum = CleanCountValue;
            //progressBarWarning.Maximum = WarningCountValue;

            //progressBarDueCount.Minimum = WarningCountValue;
            //progressBarDueCount.Maximum = DueCountValue;
            if (ItemName == "Pick Up Head")
            {
                panelCleanCount.Visible = false;
            }
            if (CurrentCountValue < CleanCountValue && CurrentCountValue < WarningCountValue)
            {
                progressBarClean.Value = CurrentCountValue;
                progressBarWarningDue.Value = CurrentCountValue;
                panel1.BackColor = Color.PaleTurquoise;
            }
            else if (CurrentCountValue >= CleanCountValue && CurrentCountValue < WarningCountValue)
            {
                progressBarClean.Value = CleanCountValue;
                progressBarWarningDue.Value = CurrentCountValue;
                //panel1.BackColor = Color.Yellow;
                if (ItemName.Contains("Pick Up Head"))
                {
                    panel1.BackColor = Color.Yellow;
                }
                else
                {
                    panel1.BackColor = Color.PaleTurquoise;
                }
            }
            else if (CurrentCountValue >= WarningCountValue && CurrentCountValue < DueCountValue && CurrentCountValue < CleanCountValue)
            {
                progressBarClean.Value = CurrentCountValue;
                //progressBarWarningDue.Value = WarningCountValue;
                progressBarWarningDue.Value = CurrentCountValue;
                panel1.BackColor = Color.Orange;
            }
            else if (CurrentCountValue >= WarningCountValue && CurrentCountValue < DueCountValue && CurrentCountValue >= CleanCountValue)
            {
                progressBarClean.Value = CleanCountValue;
                //progressBarWarningDue.Value = WarningCountValue;
                progressBarWarningDue.Value = CurrentCountValue;
                //panel1.BackColor = Color.Orange;
                //panel1.BackColor = Color.Yellow;
                if (!ItemName.Contains("Pick Up Head"))
                {
                    panel1.BackColor = Color.Yellow;
                }
                else
                {
                    panel1.BackColor = Color.Orange;
                }
            }
            else if (CurrentCountValue >= DueCountValue && CurrentCountValue < CleanCountValue)
            {
                progressBarClean.Value = CurrentCountValue;
                progressBarWarningDue.Value = DueCountValue;
                panel1.BackColor = Color.Red;
            }
            else if (CurrentCountValue >= DueCountValue && CurrentCountValue >= CleanCountValue)
            {
                progressBarClean.Value = CleanCountValue;
                progressBarWarningDue.Value = DueCountValue;
                panel1.BackColor = Color.Red;
            }
        }
    }
}
