using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Common;

namespace Customer
{
    public class CustomerFormTeachPoint : Product.ProductFormTeachPoint
    {
        private CustomerShareVariables m_CustomerShareVariables;
        private CustomerProcessEvent m_CustomerProcessEvent;
        private CustomerRTSSProcess m_CustomerRTSSProcess;

        public CustomerTeachPointSettings m_CustomerTeachPointSettings = new CustomerTeachPointSettings();

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
                if (File.Exists(m_strTeachPointPath + m_strFile))
                {
                    m_CustomerTeachPointSettings = Tools.Deserialize<CustomerTeachPointSettings>(m_strTeachPointPath + m_strFile);
                    productTeachPointSettings = m_CustomerTeachPointSettings;
                    m_listMotorProfile.Clear();

                    if (AddAllTeachpoint() == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Teach Point file not exist." + Environment.NewLine);
                    return false;
                }
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
                Tools.Serialize(m_strTeachPointPath + m_strFile, m_CustomerTeachPointSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public int UpdateAllTeachpoint()
        {
            int nError = 0;
            base.UpdateAllTeachpoint();
            return nError;
        }

        override public int AddAllTeachpoint()
        {
            int nError = 0;
            base.AddAllTeachpoint();
            return nError;
        }

        override public int GetPreviousTeachPointSettings(out List<MotionLibrary.Motion.MotorProfile> listMotionProfile)
        {
            int nError = 0;
            listMotionProfile = new List<MotionLibrary.Motor.MotorProfile>();
            CustomerTeachPointSettings m_PreviousTeachPointSettings = new CustomerTeachPointSettings();
            m_PreviousTeachPointSettings = Tools.Deserialize<CustomerTeachPointSettings>(m_strTeachPointPath + m_strFile);
            FieldInfo[] fields = typeof(CustomerTeachPointSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                //if (_fields.Name != "ListTeachPoint")
                if (_fields.FieldType.Name == "MotorProfile")
                {
                    MotionLibrary.Motion.MotorProfile a = (MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_PreviousTeachPointSettings);
                    listMotionProfile.Add((MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_PreviousTeachPointSettings));
                }
            }
            return nError;
        }
    }
}

