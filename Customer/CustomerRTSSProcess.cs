using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Customer
{
    public class CustomerRTSSProcess : Product.ProductRTSSProcess
    {
        private CustomerShareVariables m_CustomerShareVariables;// = new CustomerShareVariables();
        private CustomerGUIEvent m_CustomerGUIEvent;// = new CustomerGUIEvent();
        private CustomerProcessEvent m_CustomerProcessEvent;// = new CustomerProcessEvent();

        CustomerConfigurationSettings m_CustomerConfigurationSettings;
        CustomerOptionSettings m_CustomerOptionSettings;
        CustomerRecipeInputSettings m_CustomerRecipeInputSettings;
        CustomerRecipeOutputSettings m_CustomerRecipeOutputSettings;
        CustomerRecipeDelaySettings m_CustomerRecipeDelaySettings;
        CustomerRecipeMotorPositionSettings m_CustomerRecipeMotorPositionSettings;
        CustomerTeachPointSettings m_CustomerTeachPointSettings;

        //CustomerRecipeCassetteSettings m_CustomerRecipeInputCassetteSettings;
        //CustomerRecipeCassetteSettings m_CustomerRecipeOutputCassetteSettings;
        CustomerRecipeVisionSettings m_CustomerRecipeVisionSettings;
        CustomerRecipeSortingSettings m_CustomerRecipeSortingSettings;
        CustomerRecipePickUpHeadSetting m_CustomerRecipePickUpHeadSettings;

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

        public CustomerProcessEvent customerProcessEvent
        {
            set
            {
                m_CustomerProcessEvent = value;
                productProcessEvent = m_CustomerProcessEvent;
            }
        }

        public CustomerConfigurationSettings customerConfigurationSettings
        {
            set
            {
                m_CustomerConfigurationSettings = value;
                productConfigurationSettings = m_CustomerConfigurationSettings;
            }
        }

        public CustomerOptionSettings customerOptionSettings
        {
            set
            {
                m_CustomerOptionSettings = value;
                productOptionSettings = m_CustomerOptionSettings;
            }
        }

        public CustomerRecipeInputSettings customerRecipeInputSettings
        {
            set
            {
                m_CustomerRecipeInputSettings = value;
                productRecipeInputSettings = m_CustomerRecipeInputSettings;
            }
        }

        public CustomerRecipeOutputSettings customerRecipeOutputSettings
        {
            set
            {
                m_CustomerRecipeOutputSettings = value;
                productRecipeOutputSettings = m_CustomerRecipeOutputSettings;
            }
        }

        public CustomerRecipeDelaySettings customerRecipeDelaySettings
        {
            set
            {
                m_CustomerRecipeDelaySettings = value;
                productRecipeDelaySettings = m_CustomerRecipeDelaySettings;
            }
        }

        public CustomerRecipeMotorPositionSettings customerRecipeMotorPositionSettings
        {
            set
            {
                m_CustomerRecipeMotorPositionSettings = value;
                productRecipeMotorPositionSettings = m_CustomerRecipeMotorPositionSettings;
            }
        }

        public CustomerTeachPointSettings customerTeachPointSettings
        {
            set
            {
                m_CustomerTeachPointSettings = value;
                productTeachPointSettings = m_CustomerTeachPointSettings;
            }
        }

        //public CustomerRecipeCassetteSettings customerRecipeInputCassetteSettings
        //{
        //    set
        //    {
        //        m_CustomerRecipeInputCassetteSettings = value;
        //        productRecipeInputCassetteSettings = m_CustomerRecipeInputCassetteSettings;
        //    }
        //}

        //public CustomerRecipeCassetteSettings customerRecipeOutputCassetteSettings
        //{
        //    set
        //    {
        //        m_CustomerRecipeOutputCassetteSettings = value;
        //        productRecipeOutputCassetteSettings = m_CustomerRecipeOutputCassetteSettings;
        //    }
        //}

        public CustomerRecipeVisionSettings customerRecipeVisionSettings
        {
            set
            {
                m_CustomerRecipeVisionSettings = value;
                productRecipeVisionSettings = m_CustomerRecipeVisionSettings;
            }
        }

        public CustomerRecipeSortingSettings customerRecipeSortingSettings
        {
            set
            {
                m_CustomerRecipeSortingSettings = value;
                productRecipeSortingSettings = m_CustomerRecipeSortingSettings;
            }
        }

        public CustomerRecipePickUpHeadSetting customerRecipePickUpHeadSettings
        {
            set
            {
                m_CustomerRecipePickUpHeadSettings = value;
                productRecipePickUpHeadSettings = m_CustomerRecipePickUpHeadSettings;
            }
        }
        public bool CustomerRunRtssSequence
        {
            set
            {
                ProductRunRtssSequence = value;
            }
        }

        public bool CustomerRunExeSequence
        {
            set
            {
                ProductRunExeSequence = value;
            }
        }
        override public bool LoadConfigurationSettings()
        {
            try
            {
                m_CustomerConfigurationSettings = new CustomerConfigurationSettings();
                m_CustomerConfigurationSettings = Tools.Deserialize<CustomerConfigurationSettings>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strConfigurationFile);
                productConfigurationSettings = m_CustomerConfigurationSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetConfigurationSettingsToShareMemory()
        {
            try
            {
                base.SetConfigurationSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadOptionSettings()
        {
            try
            {
                m_CustomerOptionSettings = new CustomerOptionSettings();
                m_CustomerOptionSettings = Tools.Deserialize<CustomerOptionSettings>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strOptionFile);
                productOptionSettings = m_CustomerOptionSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetOptionSettingsToShareMemory()
        {
            try
            {
                base.SetOptionSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipeMainSettings(string recipeMainName)
        {
            try
            {
                m_CustomerShareVariables.customerRecipeMainSettings = Tools.Deserialize<CustomerRecipeMainSettings>(m_CustomerShareVariables.strRecipeMainPath + recipeMainName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeMainSettings = m_CustomerShareVariables.customerRecipeMainSettings;
                m_CustomerShareVariables.recipeMainSettings = m_CustomerShareVariables.customerRecipeMainSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeMainSettingsToShareMemory()
        {
            try
            {
                base.SetRecipeMainSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipeInputSettings()
        {
            try
            {
                m_CustomerRecipeInputSettings = new CustomerRecipeInputSettings();
                m_CustomerRecipeInputSettings = Tools.Deserialize<CustomerRecipeInputSettings>(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipeInputSettings = m_CustomerRecipeInputSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeInputSettingsToShareMemory()
        {
            try
            {
                base.SetRecipeInputSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipeOutputSettings()
        {
            try
            {
                m_CustomerRecipeOutputSettings = new CustomerRecipeOutputSettings();
                m_CustomerRecipeOutputSettings = Tools.Deserialize<CustomerRecipeOutputSettings>(m_CustomerShareVariables.strRecipeOutputPath + m_CustomerShareVariables.recipeMainSettings.OutputRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipeOutputSettings = m_CustomerRecipeOutputSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeOutputSettingsToShareMemory()
        {
            try
            {
                base.SetRecipeOutputSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipeDelaySettings()
        {
            try
            {
                m_CustomerRecipeDelaySettings = new CustomerRecipeDelaySettings();
                m_CustomerRecipeDelaySettings = Tools.Deserialize<CustomerRecipeDelaySettings>(m_CustomerShareVariables.strRecipeDelayPath + m_CustomerShareVariables.recipeMainSettings.DelayRecipeName + m_CustomerShareVariables.strXmlExtension);
                customerRecipeDelaySettings = m_CustomerRecipeDelaySettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeDelaySettingsToShareMemory()
        {
            try
            {
                base.SetRecipeDelaySettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipeMotorPositionSettings()
        {
            try
            {
                m_CustomerRecipeMotorPositionSettings = new CustomerRecipeMotorPositionSettings();
                m_CustomerRecipeMotorPositionSettings = Tools.Deserialize<CustomerRecipeMotorPositionSettings>(m_CustomerShareVariables.strRecipeMotorPositionPath + m_CustomerShareVariables.recipeMainSettings.MotorPositionRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipeMotorPositionSettings = m_CustomerRecipeMotorPositionSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeMotorPositionSettingsToShareMemory()
        {
            try
            {
                base.SetRecipeMotorPositionSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadTeachPointSettings()
        {
            try
            {
                m_CustomerTeachPointSettings = new CustomerTeachPointSettings();
                m_CustomerTeachPointSettings = Tools.Deserialize<CustomerTeachPointSettings>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strTeachPointFile);
                productTeachPointSettings = m_CustomerTeachPointSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetTeachPointSettingsToShareMemory()
        {
            try
            {
                base.SetTeachPointSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        //override public bool LoadRecipeInputCassetteSettings()
        //{
        //    try
        //    {
        //        m_CustomerRecipeInputCassetteSettings = new CustomerRecipeCassetteSettings();
        //        m_CustomerRecipeInputCassetteSettings = Tools.Deserialize<CustomerRecipeCassetteSettings>(m_CustomerShareVariables.strRecipeInputCassettePath + m_CustomerShareVariables.productRecipeMainSettings.InputCassetteRecipeName + m_CustomerShareVariables.strXmlExtension);
        //        productRecipeInputCassetteSettings = m_CustomerRecipeInputCassetteSettings;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        //override public bool LoadRecipeOutputCassetteSettings()
        //{
        //    try
        //    {
        //        m_CustomerRecipeOutputCassetteSettings = new CustomerRecipeCassetteSettings();
        //        m_CustomerRecipeOutputCassetteSettings = Tools.Deserialize<CustomerRecipeCassetteSettings>(m_CustomerShareVariables.strRecipeOutputCassettePath + m_CustomerShareVariables.productRecipeMainSettings.OutputCassetteRecipeName + m_CustomerShareVariables.strXmlExtension);
        //        productRecipeOutputCassetteSettings = m_CustomerRecipeOutputCassetteSettings;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        override public bool LoadRecipeSortingSettings()
        {
            try
            {
                m_CustomerRecipeSortingSettings = new CustomerRecipeSortingSettings();
                m_CustomerRecipeSortingSettings = Tools.Deserialize<CustomerRecipeSortingSettings>(m_CustomerShareVariables.strRecipeSortingPath + m_CustomerShareVariables.productRecipeMainSettings.SortingRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipeSortingSettings = m_CustomerRecipeSortingSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool LoadRecipePickUpHeadSettings()
        {
            try
            {
                m_CustomerRecipePickUpHeadSettings = new CustomerRecipePickUpHeadSetting();
                m_CustomerRecipePickUpHeadSettings = Tools.Deserialize<CustomerRecipePickUpHeadSetting>(m_CustomerShareVariables.strRecipePickUpHeadPath + m_CustomerShareVariables.productRecipeMainSettings.PickUpHeadRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipePickUpHeadSettings = m_CustomerRecipePickUpHeadSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        //override public bool SetRecipeInputCassetteSettingsToShareMemory()
        //{
        //    try
        //    {
        //        base.SetRecipeInputCassetteSettingsToShareMemory();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        override public bool LoadRecipeVisionSettings()
        {
            try
            {
                m_CustomerRecipeVisionSettings = new CustomerRecipeVisionSettings();
                m_CustomerRecipeVisionSettings = Tools.Deserialize<CustomerRecipeVisionSettings>(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension);
                productRecipeVisionSettings = m_CustomerRecipeVisionSettings;
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        override public bool SetRecipeVisionSettingsToShareMemory()
        {
            try
            {
                base.SetRecipeVisionSettingsToShareMemory();
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }
    }
}
