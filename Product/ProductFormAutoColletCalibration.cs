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
    public partial class ProductFormAutoColletCalibration : Form
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }

        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }

        public ProductFormAutoColletCalibration()
        {
            InitializeComponent();
            GenerateFormEvents();
            //this.FormBorderStyle = FormBorderStyle.None;
        }

        void GenerateFormEvents()
        {
            comboBoxAutoColletRecipeName.SelectedIndexChanged += ComboBoxAutoColletRecipeName_SelectedIndexChanged;
            buttonStartAutoColletCalibration.Click += ButtonStartAutoColletCalibration_Click;
            buttonCloseAutoColletCalibration.Click += ButtonCloseAutoColletCalibration_Click;
        }

        private void ButtonCloseAutoColletCalibration_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonStartAutoColletCalibration_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ComboBoxAutoColletRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
    }
}
