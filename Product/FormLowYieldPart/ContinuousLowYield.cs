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
    public partial class ContinuousLowYield : UserControl
    {
        public ProductOptionSettings m_productOptionSettings = new ProductOptionSettings();
        public string ItemName = "";
        public int nContinuousLowYieldTotalCountValue = 0;
        //public int nLowYieldCurrentCountValue_Backup = 0;
        public int nContinuousLowYieldFailCountValue = 0;
        bool isValueChange = false;
        public ContinuousLowYield()
        {
            InitializeComponent();
        }
        public void updateInterface()
        {
            label7.Text = ItemName;
            numericUpDownContinuousLowYieldFailCount.Value = nContinuousLowYieldFailCountValue;
            numericUpDownContinuousLowYieldTotalCount.Value = nContinuousLowYieldTotalCountValue;
            if (ItemName.Contains("Bond"))
            {
                //if (m_productOptionSettings.nBondHeadMaxContinuousFailCount > 0)
                //{
                //    if (nContinuousLowYieldFailCountValue <= m_productOptionSettings.nBondHeadMaxContinuousFailCount)
                //    {
                //        panelContinuousLY.BackColor = Color.LightSkyBlue;
                //    }
                //    else if (nContinuousLowYieldFailCountValue > m_productOptionSettings.nBondHeadMaxContinuousFailCount)
                //    {
                //        panelContinuousLY.BackColor = Color.Red;
                //        //Machine.EventLogger.WriteLog(string.Format("{0} : Continuous Low Yield Alarm Triggered.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                //        //Machine.SequenceControl.SetAlarm(60103);
                //        //Machine.LogDisplay.AddLogDisplay("Caution", "Continuous Low Yield Alarm Triggered");
                //    }
                //}
                //else
                //{
                //    panelContinuousLY.BackColor = Color.LightSkyBlue;
                //}
            }
            else
            {
                //if (m_productOptionSettings.nFlipperHeadMaxContinuousFailCount > 0)
                //{
                //    if (nContinuousLowYieldFailCountValue <= m_productOptionSettings.nFlipperHeadMaxContinuousFailCount)
                //    {
                //        panelContinuousLY.BackColor = Color.LightSkyBlue;
                //    }
                //    else if (nContinuousLowYieldFailCountValue > m_productOptionSettings.nFlipperHeadMaxContinuousFailCount)
                //    {
                //        panelContinuousLY.BackColor = Color.Red;
                //        //Machine.EventLogger.WriteLog(string.Format("{0} : Continuous Low Yield Alarm Triggered.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                //        //Machine.SequenceControl.SetAlarm(60104);
                //        //Machine.LogDisplay.AddLogDisplay("Caution", "Continuous Low Yield Alarm Triggered");
                //    }
                //}
                //else
                //{
                //    panelContinuousLY.BackColor = Color.LightSkyBlue;
                //}
            }
        }
    }
}
