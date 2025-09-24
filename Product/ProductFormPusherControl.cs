using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class ProductFormPusherControl : Form
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 
        private ProductProcessEvent m_ProductProcessEvent;
        bool IsSidewallPusherInUpPosition = false;
        bool IsSidewallPusherInDownPosition = false;
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

        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductFormPusherControl()
        {
            InitializeComponent();
            FormEvent();
        }
        void FormEvent()
        {
            buttonAdditionalSWPusherDown.Click += ButtonAdditionalSWPusherDown_Click;
            buttonAdditonalSWPusherUp.Click += ButtonAdditonalSWPusherUp_Click;
            buttonClose.Click += ButtonClose_Click;
        }
        virtual public void Initialize()
        {
            m_ProductRTSSProcess.SetProductionBool("PusherControlSWPusher", true);
            IsSidewallPusherInDownPosition = m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown");
            IsSidewallPusherInUpPosition = m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherUp");
            if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown")==false && m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherUp")==true)
            {
                buttonAdditionalSWPusherDown.Enabled = true;
                buttonAdditonalSWPusherUp.Enabled = false;
            }
            else if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown") == true && m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherUp") == false)
            {
                buttonAdditionalSWPusherDown.Enabled = false;
                buttonAdditonalSWPusherUp.Enabled = true;
            }
        }
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown") == IsSidewallPusherInDownPosition
                && m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherUp") == IsSidewallPusherInUpPosition)
            {
                m_ProductRTSSProcess.SetProductionBool("PusherControlSWPusher", false);
                this.Dispose();
            }
            else if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown") == true)
            {
                MessageBox.Show("Side Wall Pusher is not in up position. Please move pusher up before you close this form.", "Pusher Control Warning");
            }
            else if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown") == false)
            {
                MessageBox.Show("Side Wall Pusher is not in up position. Please move pusher up before you close this form.", "Pusher Control Warning");
            }
        }

        private void ButtonAdditonalSWPusherUp_Click(object sender, EventArgs e)
        {
            if (m_ProductRTSSProcess.GetEvent("StartPause") == true || m_ProductRTSSProcess.GetEvent("JobPause") == true)
            {
                buttonAdditonalSWPusherUp.Enabled = false;
                buttonAdditionalSWPusherDown.Enabled = true;
                m_ProductRTSSProcess.SetEvent("StartSidewallPusherUp", true);
                m_ProductRTSSProcess.SetEvent("SidewallPusherUpDone", false);
                m_ProductRTSSProcess.SetEvent("SidewallPusherDownDone", false);
            }
            else
            {
                MessageBox.Show("Machine Not in Pause Mode.");
            }
        }

        private void ButtonAdditionalSWPusherDown_Click(object sender, EventArgs e)
        {
            if (m_ProductRTSSProcess.GetEvent("StartPause") == true || m_ProductRTSSProcess.GetEvent("JobPause") == true)
            {
                buttonAdditionalSWPusherDown.Enabled = false;
                buttonAdditonalSWPusherUp.Enabled = true;
                m_ProductRTSSProcess.SetEvent("StartSidewallPusherDown", true);
                m_ProductRTSSProcess.SetEvent("SidewallPusherUpDone", false);
                m_ProductRTSSProcess.SetEvent("SidewallPusherDownDone", false);
            }
            else
            {
                MessageBox.Show("Machine Not in Pause Mode.");
            }
        }
    }
}
