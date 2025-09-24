using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class ProductFormOutputAlignment : Form
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 
        private ProductProcessEvent m_ProductProcessEvent;
        private ProductStateControl m_ProductStateControl;

        string m_strmode = "Output Alignment";
        long nOutputTableAlignmentXAxis = 0;
        long nOutputTableAlignmentYAxis = 0;
        int nNoOfOutputTableXAlignment = 0;
        int nNoOfOutputTableYAlignment = 0;
        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }
        public ProductProcessEvent productProcessEvent
        {
            set
            {
                m_ProductProcessEvent = value;
            }
        }
        public ProductStateControl productStateControl
        {
            set
            {
                m_ProductStateControl = value;
            }
        }
        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductFormOutputAlignment()
        {
            InitializeComponent();
        }
        virtual public void Initialize()
        {
            try
            {
                nOutputTableAlignmentXAxis = 0;
                nOutputTableAlignmentYAxis = 0;
                nNoOfOutputTableXAlignment = 0;
                nNoOfOutputTableYAlignment = 0;
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_Output_Alignment_Form_Initialize", true);
                labelTopUnit.Text = $"X = {m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput1").ToString()}, Y = {m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput1").ToString()}";
                labelBottomUnit.Text = $"X = {m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput2").ToString()}, Y = {m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput2").ToString()}";
                labelLeftUnit.Text = $"X = {m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput3").ToString()}, Y = {m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput3").ToString()}";
                labelRightUnit.Text = $"X = {m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput4").ToString()}, Y = {m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput4").ToString()}";

                if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput1") != 0)
                {
                    nOutputTableAlignmentXAxis += (-m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput1"));//um
                    nNoOfOutputTableXAlignment++;
                }
                if (m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput1") != 0)
                {
                    nOutputTableAlignmentYAxis += (-m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput1")) + m_ProductRTSSProcess.GetSettingUInt("DeviceYPitchOutput");//um
                    nNoOfOutputTableYAlignment++;
                }

                if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput2") != 0)
                {
                    nOutputTableAlignmentXAxis += -m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput2");//um
                    nNoOfOutputTableXAlignment++;
                }
                if (m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput2") != 0)
                {
                    nOutputTableAlignmentYAxis += (-m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput2")) - m_ProductRTSSProcess.GetSettingUInt("DeviceYPitchOutput");//um
                    nNoOfOutputTableYAlignment++;
                }

                if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput3") != 0)
                {
                    nOutputTableAlignmentXAxis += -m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput3") - m_ProductRTSSProcess.GetSettingUInt("DeviceXPitchOutput");//um
                    nNoOfOutputTableXAlignment++;
                }
                if (m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput3") != 0)
                {
                    nOutputTableAlignmentYAxis += -m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput3");//um
                    nNoOfOutputTableYAlignment++;
                }

                if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput4") != 0)
                {
                    nOutputTableAlignmentXAxis += (-m_ProductRTSSProcess.GetProductionLong("nVisionXOffsetOutput4")) + m_ProductRTSSProcess.GetSettingUInt("DeviceXPitchOutput");//um
                    nNoOfOutputTableXAlignment++;
                }
                if (m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput4") != 0)
                {
                    nOutputTableAlignmentYAxis += -m_ProductRTSSProcess.GetProductionLong("nVisionYOffsetOutput4");//um
                    nNoOfOutputTableYAlignment++;
                }
                if (nNoOfOutputTableXAlignment != 0 && nNoOfOutputTableYAlignment != 0)
                {
                    labelOutputTableXAxisMoveResult.Text = $"{nOutputTableAlignmentXAxis / nNoOfOutputTableXAlignment} um";
                    labelOutputTableYAxisMoveResult.Text = $"{nOutputTableAlignmentYAxis / nNoOfOutputTableYAlignment} um";
                }
                else if (nNoOfOutputTableXAlignment == 0 && nNoOfOutputTableYAlignment != 0)
                {
                    labelOutputTableXAxisMoveResult.Text = $"0 um";
                    labelOutputTableYAxisMoveResult.Text = $"{nOutputTableAlignmentYAxis / nNoOfOutputTableYAlignment} um";
                }
                else if (nNoOfOutputTableXAlignment != 0 && nNoOfOutputTableYAlignment == 0)
                {
                    labelOutputTableXAxisMoveResult.Text = $"{nOutputTableAlignmentXAxis / nNoOfOutputTableXAlignment} um";
                    labelOutputTableYAxisMoveResult.Text = $"0 um";
                }
                else if (nNoOfOutputTableXAlignment == 0 && nNoOfOutputTableYAlignment == 0)
                {
                    labelOutputTableXAxisMoveResult.Text = $"0 um";
                    labelOutputTableYAxisMoveResult.Text = $"0 um";
                }
                radioButtonOutputAlignment3.Click += RadioButtonOutputAlignment3_Click;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void RadioButtonOutputAlignment3_Click(object sender, EventArgs e)
        {
            groupBoxOffset.Visible = true;
        }

        private void buttonOutputAlignUnload_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("JobStop", true);
            if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
            {
                m_ProductRTSSProcess.SetEvent("StartReset", true);
                Thread.Sleep(500);
                m_ProductRTSSProcess.SetEvent("StartJob", true);
            }
            updateRichTextBoxMessage("Unload frame or tile.");
            this.Dispose();
        }

        private void buttonOutputAlignContinue_Click(object sender, EventArgs e)
        {
            if (radioButtonOutputAlignment1.Checked == true)
            {
                if (MessageBox.Show("Are you sure you want to use Vision Offset?", "Confirm To Perform This Option",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_Output_Alignment_Use_Vision_Offset", true);
                    if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                    {
                        m_ProductRTSSProcess.SetEvent("StartReset", true);
                        Thread.Sleep(500);
                        m_ProductRTSSProcess.SetEvent("StartJob", true);
                    }
                    this.Dispose();
                }
            }
            else if (radioButtonOutputAlignment2.Checked == true)
            {
                if (MessageBox.Show("Are you sure you want to continue without doing Output Table Alignment?", "Confirm To Perform This Option",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_Output_Alignment_Continue_Without_Align", true);
                    if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                    {
                        m_ProductRTSSProcess.SetEvent("StartReset", true);
                        Thread.Sleep(500);
                        m_ProductRTSSProcess.SetEvent("StartJob", true);
                    }
                    this.Dispose();
                }
            }
            else if (radioButtonOutputAlignment3.Checked == true)
            {
                if (MessageBox.Show($"Are you sure you want to continue by using Offset X = {(int)numericUpDownOutputAlignXOffset.Value}, Offset Y = {(int)numericUpDownOutputAlignYOffset.Value}?", "Confirm To Perform This Option",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_Output_Alignment_Use_Customize_Offset", true);
                    m_ProductRTSSProcess.SetProductionLong("nVisionOutputXOffsetCustomizeMove", (int)numericUpDownOutputAlignXOffset.Value);
                    m_ProductRTSSProcess.SetProductionLong("nVisionOutputYOffsetCustomizeMove", (int)numericUpDownOutputAlignYOffset.Value);
                    if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                    {
                        m_ProductRTSSProcess.SetEvent("StartReset", true);
                        Thread.Sleep(500);
                        m_ProductRTSSProcess.SetEvent("StartJob", true);
                    }
                    this.Dispose();
                }
            }
            else
            {
                richTextBoxOutputAlignment.Text = ("Please select an option.\n");
            }
        }
        public void updateRichTextBoxMessage(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(richTextBoxOutputAlignment, message);
            Machine.EventLogger.WriteLog(string.Format("{0} " + message + " at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        }
    }
}
