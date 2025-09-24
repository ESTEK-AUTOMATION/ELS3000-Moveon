using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Customer
{
    public class CustomerFormTeachMap : Product.ProductFormTeachMap
    {
        private CustomerRecipeVisionSettings m_CustomerRecipeVisionSetting = new CustomerRecipeVisionSettings();
        private CustomerRecipeInputSettings m_CustomerRecipeInputSetting = new CustomerRecipeInputSettings();
        private CustomerRecipeVisionSettings m_Ori_CustomerRecipeVisionSetting = new CustomerRecipeVisionSettings();
        private CustomerRecipeInputSettings m_Ori_CustomerRecipeInputSetting = new CustomerRecipeInputSettings();

        private CustomerShareVariables m_CustomerShareVariables;// = new ProductShareVariables();
        private CustomerProcessEvent m_CustomerProcessEvent;// = new ProductProcessEvent();

        private CustomerStateControl m_CustomerStateControl;// = new ProductStateControl();
        private CustomerRTSSProcess m_CustomerRTSSProcess;// = new ProductRTSSProcess();

        
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

        public CustomerStateControl customerStateControl
        {
            set
            {
                m_CustomerStateControl = value;
                productStateControl = m_CustomerStateControl;
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
                if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeVisionPath, m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName, m_CustomerShareVariables.strXmlExtension))
                {
                    m_Ori_CustomerRecipeVisionSetting = Tools.Deserialize<CustomerRecipeVisionSettings>(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension);
                    m_CustomerRecipeVisionSetting = Tools.Deserialize<CustomerRecipeVisionSettings>(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension);
                    m_ProductRecipeVisionSetting = m_CustomerRecipeVisionSetting;
                }
                else
                {
                    updateRichTextBoxMessage("Vision recipe not exist");
                }
                if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeInputPath, m_CustomerShareVariables.recipeMainSettings.InputRecipeName, m_CustomerShareVariables.strXmlExtension))
                {
                    m_Ori_CustomerRecipeInputSetting = Tools.Deserialize<CustomerRecipeInputSettings>(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension);
                    m_CustomerRecipeInputSetting = Tools.Deserialize<CustomerRecipeInputSettings>(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension);
                    m_ProductRecipeInputSetting = m_CustomerRecipeInputSetting;
                }
                else
                {
                    updateRichTextBoxMessage("Input recipe not exist");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public bool SaveSettings()
        {
            try
            {
                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_CustomerShareVariables.strLoginUserName}, made changes at {m_strmode}");

                FieldInfo[] fi = m_ProductRecipeVisionSetting.GetType().GetFields();
                foreach (FieldInfo f in fi)
                {
                    if (f.FieldType.IsArray)
                    {
                        Array array = f.GetValue(m_ProductRecipeVisionSetting) as Array;
                        //Array array_Ori = f.GetValue(m_Ori_CustomerOptionSettings) as Array;

                        if (array.GetType().GetElementType().ToString().Contains("FiducialSettings"))
                        {
                            //Product.FiducialSettings[] FiducialData_New = (Product.FiducialSettings[])f.GetValue(m_ProductRecipeVisionSetting);
                            //Product.FiducialSettings[] FiducialData_Ori = (Product.FiducialSettings[])f.GetValue(m_Ori_CustomerRecipeVisionSetting);
                            //FieldInfo[] fiducialField = typeof(Product.FiducialSettings).GetFields();
                            //for (int i = 0; i < FiducialData_New.Length; i++)
                            //{
                            //    foreach (FieldInfo field in fiducialField)
                            //    {
                            //        if (!object.Equals(field.GetValue(FiducialData_New[i]), field.GetValue(FiducialData_Ori[i])))
                            //        {
                            //            Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Recipe Type: Vision Recipe, Recipe Name: {m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName}, Parameter changed: {f.Name}, Fiducial {i + 1}, {field.Name} changed from {field.GetValue(FiducialData_Ori[i])} to {field.GetValue(FiducialData_New[i])} at {m_strmode}");
                            //        }
                            //    }
                            //}
                        }
                    }
                    if (!object.Equals(f.GetValue(m_ProductRecipeVisionSetting), f.GetValue(m_Ori_CustomerRecipeVisionSetting)) && f.FieldType.IsArray == false)
                    {
                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Recipe Type: Vision Recipe, Recipe Name: {m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName}, Parameter changed: {f.Name}, Value changed from {f.GetValue(m_Ori_CustomerRecipeVisionSetting)} to {f.GetValue(m_ProductRecipeVisionSetting)} in {m_strmode}");
                    }
                }

                FieldInfo[] fi2 = m_ProductRecipeInputSetting.GetType().GetFields();
                foreach (FieldInfo f in fi2)
                {
                    if (!object.Equals(f.GetValue(m_ProductRecipeInputSetting), f.GetValue(m_Ori_CustomerRecipeInputSetting)))
                    {
                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Recipe Type: Input Recipe, Recipe Name: {m_CustomerShareVariables.recipeMainSettings.InputRecipeName}, Parameter changed: {f.Name}, Value changed from {f.GetValue(m_Ori_CustomerRecipeInputSetting)} to {f.GetValue(m_ProductRecipeInputSetting)} in {m_strmode}");
                    }
                }

                Tools.Serialize(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension, m_ProductRecipeVisionSetting);
                Tools.Serialize(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension, m_ProductRecipeInputSetting);
                
                m_Ori_CustomerRecipeVisionSetting = Tools.Deserialize<CustomerRecipeVisionSettings>(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_Ori_CustomerRecipeInputSetting = Tools.Deserialize<CustomerRecipeInputSettings>(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension);

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
