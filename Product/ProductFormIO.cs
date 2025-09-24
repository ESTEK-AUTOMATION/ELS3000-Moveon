using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductFormIO : Machine.FormIO
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductGUIEvent m_ProductGUIEvent;// = new ProductProcessEvent();

        //private ProductStateControl m_ProductStateControl;// = new ProductStateControl();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
                shareVariables = m_ProductShareVariables;
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

        override public int OnInitializeOutputIO()
        {
            int nError = 0;
			m_NoOfIOCard = 21;
            base.OnInitializeOutputIO();
            return nError;
        }

        override public int OnClickOutputIO()
        {
            int nError = 0;
            //if (m_ProductShareVariables.nLoginAuthority <= 4)
            //{
            //    m_ProductRTSSProcess.SetEvent("GPCS_RSEQ_ABORT", true);
            //}
            return nError;
        }
    }
}
