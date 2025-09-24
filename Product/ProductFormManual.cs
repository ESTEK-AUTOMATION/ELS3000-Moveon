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
    public partial class ProductFormManual : Machine.FormManual
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();
        private ProductGUIEvent m_ProductGUIEvent;// = new ProductProcessEvent();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
                shareVariables = m_ProductShareVariables;
            }
        }

        public ProductProcessEvent productProcessEvent
        {
            set
            {
                m_ProductProcessEvent = value;
                processEvent = m_ProductProcessEvent;
            }
        }

        public ProductGUIEvent productGUIEvent
        {
            set
            {
                m_ProductGUIEvent = value;
                guiEvent = m_ProductGUIEvent;
            }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
                rtssProcess = value;
            }
        }

        override public void Initialize()
        {
            try
            {
                base.Initialize();
                m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_On.Set();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void OnCloseForm()
        {
            try
            {
                base.OnCloseForm();
                m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_Off.Set();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
    }
}
