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
    public partial class ProductionOutputMotionMove : Form
    {
        public ProductShareVariables m_ProductShareVariables;
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;
        bool FirstTime = false;
        public ProductRTSSProcess productRTSSProcess
        {
            set { m_ProductRTSSProcess = value; }
        }
        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }
        public ProductionOutputMotionMove()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Dispose();
            Close();
        }

        private void btnMoveX_Click(object sender, EventArgs e)
        {
            int xPosition = 0;
            int XCurrentPosition = m_ProductRTSSProcess.GetProductionLong("OutputTrayTableXAxisMovePosition");
            xPosition = XCurrentPosition + (int)nudXToMove.Value;
            m_ProductRTSSProcess.SetProductionLong("OutputTrayTableXAxisMovePosition", xPosition);
            m_ProductRTSSProcess.SetEvent("StartOutputTrayTableXAxisMotorMove", true);
        }

        private void btnMoveY_Click(object sender, EventArgs e)
        {
            int yPosition = 0;
            int YCurrentPosition = m_ProductRTSSProcess.GetProductionLong("OutputTrayTableYAxisMovePosition");
            yPosition = YCurrentPosition + (int)nudYToMove.Value;
            m_ProductRTSSProcess.SetProductionLong("OutputTrayTableYAxisMovePosition", yPosition);
            m_ProductRTSSProcess.SetEvent("StartOutputTrayTableYAxisMotorMove", true);
        }
    }
}
