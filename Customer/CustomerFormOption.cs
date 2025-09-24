using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;
using System.Reflection;

namespace Customer
{
    public class CustomerFormOption : Product.ProductFormOption
    {
        private CustomerShareVariables m_CustomerShareVariables = new CustomerShareVariables();
        private CustomerProcessEvent m_CustomerProcessEvent = new CustomerProcessEvent();

        public CustomerOptionSettings m_CustomerOptionSettings = new CustomerOptionSettings();

        public CustomerOptionSettings m_Ori_CustomerOptionSettings = new CustomerOptionSettings();

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

        override public void UpdateGUI()
        {
            base.UpdateGUI();

            m_tabpageFilePath.txtVisFileFolder.Text = m_CustomerOptionSettings.InvisFileFolderName;
            m_tabpageFilePath.txtBluFileFolder.Text = m_CustomerOptionSettings.InbluFileFolderName;
            m_tabpageFilePath.txtOutbluFileFolder.Text = m_CustomerOptionSettings.OutbluFileFolderName;
            m_tabpageFilePath.textBoxServerPPLotPath.Text = m_CustomerOptionSettings.ServerPPLotPath;

            m_tabpageFilePath.textBoxInputMachineProcessCode.Text = m_CustomerOptionSettings.strInputMachineProcessCode;
            m_tabpageFilePath.textBoxCurrentMachineProcessCode.Text = m_CustomerOptionSettings.strCurrentMachineProcessCode;           
        }

        override public bool VerifyOption()
        {
            if (base.VerifyOption() == false)
                return false;

            m_CustomerOptionSettings.InvisFileFolderName = m_tabpageFilePath.txtVisFileFolder.Text;
            m_CustomerOptionSettings.InbluFileFolderName = m_tabpageFilePath.txtBluFileFolder.Text;
            m_CustomerOptionSettings.OutbluFileFolderName = m_tabpageFilePath.txtOutbluFileFolder.Text;
            m_CustomerOptionSettings.ServerPPLotPath = m_tabpageFilePath.textBoxServerPPLotPath.Text;

            m_CustomerOptionSettings.strInputMachineProcessCode = m_tabpageFilePath.textBoxInputMachineProcessCode.Text;
            m_CustomerOptionSettings.strCurrentMachineProcessCode = m_tabpageFilePath.textBoxCurrentMachineProcessCode.Text;

            return true;
        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(m_strOptionPath + m_strFile))
                {
                    m_Ori_CustomerOptionSettings = Tools.Deserialize<CustomerOptionSettings>(m_strOptionPath + m_strFile);
                    m_CustomerOptionSettings = Tools.Deserialize<CustomerOptionSettings>(m_strOptionPath + m_strFile);
                    productOptionSettings = m_CustomerOptionSettings;
                }
                else
                {
                    updateRichTextBoxMessage("Option file not exist.");
                    return false;
                }
                if (File.Exists(m_strOptionPath + m_strReportFile))
                {
                    m_ReportSetting = Tools.Deserialize<Machine.ReportSetting>(m_strOptionPath + m_strReportFile);
                }
                else
                {
                    updateRichTextBoxMessage("Report Setting file not exist.");
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

        override public bool SaveOptionSettings()
        {
            try
            {
                #region WriteLog
                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_CustomerShareVariables.strLoginUserName}, made changes at {m_strmode}");
                FieldInfo[] fi = m_CustomerOptionSettings.GetType().GetFields();
                foreach (FieldInfo f in fi)
                {
                    if (f.FieldType.IsArray)
                    {
                        Array array = f.GetValue(m_CustomerOptionSettings) as Array;
                        //Array array_Ori = f.GetValue(m_Ori_CustomerOptionSettings) as Array;
                        
                        if (array.GetType().GetElementType().ToString().Contains("Int"))
                        {
                            int[] array_New = (int[])f.GetValue(m_CustomerOptionSettings);
                            int[] array_Ori = (int[])f.GetValue(m_Ori_CustomerOptionSettings);

                            for (int i = 0; i < array_New.Length; i++)
                            {
                                if (array_New[i] != array_Ori[i])
                                {
                                    if (f.Name.Contains("InputTable"))
                                    {
                                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {f.Name}, Flipper {i + 1} changed from {array_Ori[i]} to {array_New[i]} at {m_strmode}");
                                    }
                                    else if (f.Name.Contains("OutputTable"))
                                    {
                                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {f.Name}, Turret {i + 1} changed from {array_Ori[i]} to {array_New[i]} at {m_strmode}");
                                    }
                                    else
                                    {
                                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {f.Name}, Array/Selection No {i + 1} changed from {array_Ori[i]} to {array_New[i]} at {m_strmode}");
                                    }
                                }
                            }
                        }
                        else if (array.GetType().GetElementType().ToString().Contains("string"))
                        {
                            string[] array_New = (string[])f.GetValue(m_CustomerOptionSettings);
                            string[] array_Ori = (string[])f.GetValue(m_Ori_CustomerOptionSettings);
                            for (int i = 0; i < array_New.Length; i++)
                            {
                                if (array_New[i] != array_Ori[i])
                                {
                                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {f.Name}, Array/Selection No {i + 1} changed from {array_Ori[i]} to {array_New[i]} at {m_strmode}");
                                }
                            }
                        }
                    }
                    if (!object.Equals(f.GetValue(m_CustomerOptionSettings), f.GetValue(m_Ori_CustomerOptionSettings)) && f.FieldType.IsArray == false)
                    {
                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {f.Name} changed from {f.GetValue(m_Ori_CustomerOptionSettings)} to {f.GetValue(m_CustomerOptionSettings)} at {m_strmode}");
                    }

                }
                #endregion WriteLog
                Tools.Serialize(m_strOptionPath + m_strFile, m_CustomerOptionSettings);

                m_Ori_CustomerOptionSettings = Tools.Deserialize<CustomerOptionSettings>(m_strOptionPath + m_strFile);
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
