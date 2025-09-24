using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public class CustomerFormIO : Product.ProductFormIO
    {
        private CustomerShareVariables m_CustomerShareVariables;
        private CustomerGUIEvent m_CustomerGUIEvent;
        private CustomerRTSSProcess m_CustomerRTSSProcess;

        public CustomerShareVariables customerShareVariables
        {
            set
            {
                m_CustomerShareVariables = value;
                productShareVariables = m_CustomerShareVariables;
            }
        }

        public CustomerGUIEvent customerGUIEvent
        {
            set
            {
                m_CustomerGUIEvent = value;
                productGUIEvent = m_CustomerGUIEvent;
            }
        }

        public CustomerRTSSProcess customerRTSSProcess
        {
            set
            {
                m_CustomerRTSSProcess = value;
                productRTSSProcess = value;
            }
        }
    }
}
