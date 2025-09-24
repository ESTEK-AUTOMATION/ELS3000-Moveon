using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    class CustomerComunicationThread : Product.ProductCommunicationSequenceThread
    {
        private CustomerShareVariables m_CustomerShareVariables;
        private CustomerProcessEvent m_CustomerProcessEvent;

        private CustomerRTSSProcess m_CustomerRTSSProcess;

        public CustomerShareVariables customerShareVariables
        {
            set
            {
                m_CustomerShareVariables = value;
                productShareVariables = m_CustomerShareVariables;
            }
        }

        public CustomerProcessEvent customerProcessEvent
        {
            set
            {
                m_CustomerProcessEvent = value;
                productProcessEvent = m_CustomerProcessEvent;
            }
        }

        public CustomerRTSSProcess customerRTSSProcess
        {
            set
            {
                m_CustomerRTSSProcess = value;
                base.productRTSSProcess = m_CustomerRTSSProcess;
            }
        }
    }
}
