using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product.FormLowYieldPart
{
    public partial class LowYieldCountingItem : UserControl
    {
        public ProductOptionSettings m_productOptionSettings = new ProductOptionSettings();
        public string ItemName = "";
        public int nLowYieldTotalCountValue = 0;
        //public int nLowYieldCurrentCountValue_Backup = 0;
        public int nLowYieldPassCountValue = 0;
        public int nLowYieldPercent = 0;
        bool isValueChange = false;
        public double nValueDiffer = 0.00;
        public LowYieldCountingItem()
        {
            InitializeComponent();
        }
        public void updateInterface()
        {
            label1.ForeColor = Color.Black;
            label4.Text = ItemName;
            numericUpDownLowYieldPassCount.Value = nLowYieldPassCountValue;
            numericUpDownLowYieldTotalCount.Value = nLowYieldTotalCountValue;
            
            progressBarLowYieldPercentage.Minimum = 0;
            progressBarLowYieldPercentage.Maximum = 100;

            if (nLowYieldTotalCountValue == 0)
            {
                progressBarLowYieldPercentage.Value = 100;
                panel1.BackColor = Color.LightSkyBlue;
            }
            else
            {
                nValueDiffer = Convert.ToDouble( nLowYieldPassCountValue)/ Convert.ToDouble(nLowYieldTotalCountValue);
                nLowYieldPercent= (int)(nValueDiffer * 100);
                progressBarLowYieldPercentage.Value = nLowYieldPercent;
                if (progressBarLowYieldPercentage.Value > 0)
                {
                    if (ItemName.Contains("Bond"))
                    {
                        //if (nLowYieldPercent >= m_productOptionSettings.nBondHeadLowYieldMaxLowYieldPercent)
                        //{
                        //    panel1.BackColor = Color.LightSkyBlue;
                        //}
                        //else if (nLowYieldPercent < m_productOptionSettings.nBondHeadLowYieldMaxLowYieldPercent)
                        //{
                        //    panel1.BackColor = Color.Red;
                        //    if (nLowYieldTotalCountValue >= m_productOptionSettings.nBondHeadLowYieldMinTotalCount)
                        //    {
                        //        //Machine.EventLogger.WriteLog(string.Format("{0} : Bond Head Low Yield Alarm Triggered.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        //        //Machine.SequenceControl.SetAlarm(61001);
                        //        //Machine.LogDisplay.AddLogDisplay("Caution", "Bond Head Low Yield Alarm Triggered");
                        //    }
                        //}
                    }
                    else
                    {
                        //if (nLowYieldPercent >= m_productOptionSettings.nFlipperHeadLowYieldMaxLowYieldPercent)
                        //{
                        //    panel1.BackColor = Color.LightSkyBlue;
                        //}
                        //else if (nLowYieldPercent < m_productOptionSettings.nFlipperHeadLowYieldMaxLowYieldPercent)
                        //{
                        //    panel1.BackColor = Color.Red;
                        //    if (nLowYieldTotalCountValue >= m_productOptionSettings.nFlipperHeadLowYieldMinTotalCount)
                        //    {
                        //        //Machine.EventLogger.WriteLog(string.Format("{0} : Flipper Head Low Yield Alarm Triggered.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        //        //Machine.SequenceControl.SetAlarm(60102);
                        //        //Machine.LogDisplay.AddLogDisplay("Caution", "Flipper Head Low Yield Alarm Triggered");
                        //    }
                        //}
                    }
                }
                else
                {
                    panel1.BackColor = Color.LightSkyBlue;
                }
            }
        }
    }
}
