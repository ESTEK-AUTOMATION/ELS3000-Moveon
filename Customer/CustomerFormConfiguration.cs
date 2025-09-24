using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace Customer
{
    public class CustomerFormConfiguration : Product.ProductFormConfiguration
    {
        private CustomerConfigurationSettings m_CustomerConfigurationSettings = new CustomerConfigurationSettings();

        private CustomerShareVariables m_CustomerShareVariables;// = new ProductShareVariables();
        private CustomerProcessEvent m_CustomerProcessEvent;// = new ProductProcessEvent();

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

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(base.SettingFileName))
                {
                    m_CustomerConfigurationSettings = Tools.Deserialize<CustomerConfigurationSettings>(base.SettingFileName);
                    base.productConfigurationSettings = m_CustomerConfigurationSettings;
                    return true;
                }
                else
                {
                    updateRichTextBoxMessage("Configuration file not exist.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public void SaveSetting()
        {
            Tools.Serialize(base.SettingFileName, m_CustomerConfigurationSettings);
        }
    }
}
