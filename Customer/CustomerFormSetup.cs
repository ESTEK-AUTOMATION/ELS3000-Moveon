using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace Customer
{
    public class CustomerFormSetup : Product.ProductFormSetup
    {
        private CustomerShareVariables m_CustomerShareVariables;
        private CustomerProcessEvent m_CustomerProcessEvent;
        private CustomerRTSSProcess m_CustomerRTSSProcess;

        CustomerSetupSettings m_CustomerSetupSettings = new CustomerSetupSettings();

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
                productRTSSProcess = m_CustomerRTSSProcess;
            }
        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(m_strSetupPath + m_strFile))
                {
                    m_CustomerSetupSettings = Tools.Deserialize<CustomerSetupSettings>(m_strSetupPath + m_strFile);
                    productSetupSettings = m_CustomerSetupSettings;
                    return true;
                }
                else
                {
                    updateRichTextBoxMessage("Setup file not exist.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public bool SaveSetupSettings()
        {
            try
            {
                Tools.Serialize(m_strSetupPath + m_strFile, m_CustomerSetupSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }
    }
}
