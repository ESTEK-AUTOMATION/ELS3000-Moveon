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
    public partial class ProductFormLightingCalibration : Form
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;

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
        public ProductFormLightingCalibration()
        {
            InitializeComponent();
        }
        virtual public void Initialize()
        {

        }

        private void buttonDotGridINP_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_INP", true);
        }

        private void buttonDotGridTOP_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_TOP", true);
        }

        private void buttonDotGridALN_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_ALN", true);
        }

        private void buttonDotGridBTM_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_BTM", true);
        }

        private void buttonDotGridSWL_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_SWL", true);
        }

        private void buttonDotGridSWR_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_SWR", true);
        }

        private void buttonDotGridSWB_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_SWB", true);
        }

        private void buttonDotGridSWT_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_SWT", true);
        }

        private void buttonDotGridOUT_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_DOT_GRIDS_OUT", true);
        }

        private void buttonGrayScaleINP_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_INP", true);
        }

        private void buttonGrayScaleTOP_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_TOP", true);
        }

        private void buttonGrayScaleALN_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_ALN", true);
        }

        private void buttonGrayScaleBTM_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_BTM", true);
        }

        private void buttonGrayScaleSWL_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_SWL", true);
        }

        private void buttonGrayScaleSWR_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_SWR", true);
        }

        private void buttonGrayScaleSWB_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_SWB", true);
        }

        private void buttonGrayScaleSWT_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_SWT", true);
        }

        private void buttonGrayScaleOut_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_GRAY_SCALE_OUT", true);
        }
    }
}
