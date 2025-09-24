using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using Common;

namespace Product
{
    public class ProductRTSSProcess : Machine.RTSSProcess
    {
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemorySettingDouble([MarshalAs(UnmanagedType.LPStr)]string settingName, double value);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern int GetShareMemoryProductionPatternResultInt(int patternNo, int resultNo, [MarshalAs(UnmanagedType.LPStr)]string parameterName, [MarshalAs(UnmanagedType.LPStr)]string resultName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionPatternResultInt(int patternNo, int resultNo, [MarshalAs(UnmanagedType.LPStr)]string parameterName, [MarshalAs(UnmanagedType.LPStr)]string resultName, int resultValue);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void GetShareMemoryProductionPatternResultString(int patternNo, int resultNo, [MarshalAs(UnmanagedType.LPStr)]string parameterName, StringBuilder resultValue, int resultLength);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionPatternResultString(int patternNo, int resultNo, [MarshalAs(UnmanagedType.LPStr)]string parameterName, [MarshalAs(UnmanagedType.LPStr)]string resultName, [MarshalAs(UnmanagedType.LPStr)]string resultValue);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryProductionBool([MarshalAs(UnmanagedType.LPStr)]string paramterName);
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool SetShareMemoryProductionBool([MarshalAs(UnmanagedType.LPStr)]string parameterName, bool enable);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionArrayBool([MarshalAs(UnmanagedType.LPStr)]string parameterName, int arrayNo, bool enable);
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryProductionArrayBool([MarshalAs(UnmanagedType.LPStr)]string parameterName, int arrayNo);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryModuleStatusBool([MarshalAs(UnmanagedType.LPStr)]string paramterName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryModuleStatusBool([MarshalAs(UnmanagedType.LPStr)]string moduleStatusName, bool enable);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern double GetShareMemoryProductionDouble([MarshalAs(UnmanagedType.LPStr)]string paramterName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionDouble([MarshalAs(UnmanagedType.LPStr)]string moduleStatusName, double Value);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemorySettingArrayBool([MarshalAs(UnmanagedType.LPStr)]string parameterName, int arrayNo);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemorySettingArray([MarshalAs(UnmanagedType.LPStr)]string parameterName, int arrayNo, [MarshalAs(UnmanagedType.LPStr)]string resultName, int parameterValue);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern int GetShareMemorySettingArray([MarshalAs(UnmanagedType.LPStr)]string parameterName, int arrayNo, [MarshalAs(UnmanagedType.LPStr)]string resultName);
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void GetShareMemoryProductionString([MarshalAs(UnmanagedType.LPStr)]string parameterName, StringBuilder stringWord);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionString([MarshalAs(UnmanagedType.LPStr)]string parameterName, [MarshalAs(UnmanagedType.LPStr)]string message);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionDoubleArray([MarshalAs(UnmanagedType.LPStr)]string variableName, int arrayNo, [MarshalAs(UnmanagedType.LPStr)]string resultName, double value);
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern double GetShareMemoryProductionDoubleArray([MarshalAs(UnmanagedType.LPStr)]string variableName, int arrayNo, [MarshalAs(UnmanagedType.LPStr)]string resultName);

        public ProductShareVariables m_ProductShareVariables;
        public ProductGUIEvent m_ProductGUIEvent;
        public ProductProcessEvent m_ProductProcessEvent;

        ProductConfigurationSettings m_ProductConfigurationSettings;
        ProductOptionSettings m_ProductOptionSettings;
        ProductRecipeInputSettings m_ProductRecipeInputSettings;
        ProductRecipeOutputSettings m_ProductRecipeOutputSettings;
        ProductRecipeDelaySettings m_ProductRecipeDelaySettings;
        ProductRecipeMotorPositionSettings m_ProductRecipeMotorPositionSettings;
        ProductTeachPointSettings m_ProductTeachPointSettings;

        ProductRecipeCassetteSettings m_ProductRecipeInputCassetteSettings;
        ProductRecipeCassetteSettings m_ProductRecipeOutputCassetteSettings;
        ProductRecipeVisionSettings m_ProductRecipeVisionSetting;
        ProductRecipeSortingSetting m_ProductRecipeSortingSetting;
        ProductRecipePickUpHeadSeting m_ProductRecipePickUpHeadSetting;
        
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

        public ProductProcessEvent productProcessEvent
        {
            set
            {
                m_ProductProcessEvent = value;
                processEvent = m_ProductProcessEvent;
            }
        }

        public ProductConfigurationSettings productConfigurationSettings
        {
            set
            {
                m_ProductConfigurationSettings = value;
                configurationSettings = m_ProductConfigurationSettings;
            }
        }

        public ProductOptionSettings productOptionSettings
        {
            set
            {
                m_ProductOptionSettings = value;
                optionSettings = m_ProductOptionSettings;
            }
        }

        public ProductRecipeInputSettings productRecipeInputSettings
        {
            set
            {
                m_ProductRecipeInputSettings = value;
                recipeInputSettings = m_ProductRecipeInputSettings;
            }
        }

        public ProductRecipeOutputSettings productRecipeOutputSettings
        {
            set
            {
                m_ProductRecipeOutputSettings = value;
                recipeOutputSettings = m_ProductRecipeOutputSettings;
            }
        }

        public ProductRecipeDelaySettings productRecipeDelaySettings
        {
            set
            {
                m_ProductRecipeDelaySettings = value;
                recipeDelaySettings = m_ProductRecipeDelaySettings;
            }
        }

        public ProductRecipeMotorPositionSettings productRecipeMotorPositionSettings
        {
            set
            {
                m_ProductRecipeMotorPositionSettings = value;
                recipeMotorPositionSettings = m_ProductRecipeMotorPositionSettings;
            }
        }

        public ProductTeachPointSettings productTeachPointSettings
        {
            set
            {
                m_ProductTeachPointSettings = value;
                teachPointSettings = m_ProductTeachPointSettings;
            }
        }

        public ProductRecipeCassetteSettings productRecipeInputCassetteSettings
        {
            set { m_ProductRecipeInputCassetteSettings = value; }
        }

        public ProductRecipeCassetteSettings productRecipeOutputCassetteSettings
        {
            set { m_ProductRecipeOutputCassetteSettings = value; }
        }

        public ProductRecipeVisionSettings productRecipeVisionSettings
        {
            set { m_ProductRecipeVisionSetting = value; }
        }

        public ProductRecipeSortingSetting productRecipeSortingSettings
        {
            set { m_ProductRecipeSortingSetting = value; }
        }

        public ProductRecipePickUpHeadSeting productRecipePickUpHeadSettings
        {
            set { m_ProductRecipePickUpHeadSetting = value; }
        }

        public bool ProductRunRtssSequence
        {
            set
            {
                RunRtssSequence = value;
            }
        }

        public bool ProductRunExeSequence
        {
            set
            {
                RunExeSequence = value;
            }
        }
        
        virtual public void SetProductionPatternResultInt(int patternNo, int resultNo, string parameterName, string resultName, int resultValue)
        {
            try
            {
                SetShareMemoryProductionPatternResultInt(patternNo, resultNo, parameterName, resultName, resultValue);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void GetProductionPatternResultInt(int patternNo, int resultNo, string parameterName, string resultName)
        {
            try
            {
                GetShareMemoryProductionPatternResultInt(patternNo, resultNo, parameterName, resultName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void SetProductionPatternResultString(int patternNo, int resultNo, string parameterName, string resultName, string resultValue)
        {
            try
            {
                SetShareMemoryProductionPatternResultString(patternNo, resultNo, parameterName, resultName, resultValue);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void GetProductionPatternResultString(int patternNo, int resultNo, string parameterName, StringBuilder resultValue, int resultLength)
        {
            try
            {
                GetShareMemoryProductionPatternResultString(patternNo, resultNo, parameterName, resultValue, resultLength);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void SetProductionBool(string parameterName, bool enable)
        {
            try
            {
                SetShareMemoryProductionBool(parameterName, enable);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public bool GetProductionBool(string parameterName)
        {
            try
            {
                return GetShareMemoryProductionBool(parameterName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return false;
        }
        virtual public void SetProductionArrayBool (string parameterName, int ArrayNo, bool enable)
        {
            try
            {
                SetShareMemoryProductionArrayBool(parameterName, ArrayNo, enable);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public bool GetProductionArrayBool (string parameterName, int ArrayNo)
        {
            try
            {
                return GetShareMemoryProductionArrayBool(parameterName, ArrayNo);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return false;
        }
        virtual public bool GetModuleStatusBool(string moduleStatusName)
        {
            try
            {
                return GetShareMemoryModuleStatusBool(moduleStatusName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return false;
            
        }
        virtual public void SetModuleStatusBool(string moduleStatusName, bool enable)
        {
            try
            {
                SetShareMemoryModuleStatusBool(moduleStatusName, enable);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }


        virtual public double GetProductionDouble(string moduleStatusName)
        {
            try
            {
                return GetShareMemoryProductionDouble(moduleStatusName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return 0.0;
        }
        virtual public void SetProductionDouble(string moduleStatusName, double Value)
        {
            try
            {
                SetShareMemoryProductionDouble(moduleStatusName, Value);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public bool GetSettingArrayBool(string parameterName, int ArrayNo)
        {
            try
            {
                return GetShareMemorySettingArrayBool(parameterName, ArrayNo);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return false;
        }

        virtual public void SetSettingArray(string parameterName, int arrayNo, string resultName, int resultValue)
        {
            try
            {
                SetShareMemorySettingArray(parameterName, arrayNo, resultName, resultValue);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public int GetSettingArray(string parameterName, int arrayNo, string resultName)
        {
            try
            {
                return GetShareMemorySettingArray(parameterName, arrayNo, resultName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return 0;
            }
        }
        virtual public void SetSettingDoubleArray(string parameterName, int arrayNo, string resultName, double resultValue)
        {
            try
            {
                SetShareMemoryProductionDoubleArray(parameterName, arrayNo, resultName, resultValue);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void GetSettingDoubleArray(string parameterName, int arrayNo, string resultName)
        {
            try
            {
                GetShareMemoryProductionDoubleArray(parameterName, arrayNo, resultName);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void SetProductionString(string parameterName, string message)
        {
            try
            {
                SetShareMemoryProductionString(parameterName, message);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        virtual public void GetProductionString(string parameterName, StringBuilder stringWord)
        {
            try
            {
                GetShareMemoryProductionString(parameterName, stringWord);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        override public int LoadApplicationRecipeToSettingShareMemory()
        {
            int nError = 0;
            //nError = LoadInputCassetteRecipeToSettingShareMemory();
            //if(nError != 0)
            //{
            //    return nError;
            //}
            //nError = LoadOutputCassetteRecipeToSettingShareMemory();
            //if (nError != 0)
            //{
            //    return nError;
            //}
            nError = LoadVisionRecipeToSettingShareMemory();
            if (nError != 0)
            {
                return nError;
            }
            //nError = LoadSortingRecipeToSettingShareMemory();
            //if (nError != 0)
            //{
            //    return nError;
            //}
            nError = LoadPickUpHeadRecipeToSettingShareMemory();
            if (nError != 0)
            {
                return nError;
            }
            return nError;
        }
        virtual public int LoadSortingRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                if (File.Exists(m_ProductShareVariables.strRecipeSortingPath + m_ProductShareVariables.productRecipeMainSettings.SortingRecipeName + m_ProductShareVariables.strXmlExtension))
                {
                    if (LoadRecipeSortingSettings())
                    {
                       if (SetRecipeSortingSettingsToShareMemory())
                            m_ProductShareVariables.strStatusLabelText = "Update recipe Sorting done.";
                        else
                        {
                            nError = 3;
                            return nError;
                        }
                    }
                    else
                    {
                        m_ProductShareVariables.strStatusLabelText = "Update recipe Sorting fail.";
                        nError = 2;
                        return nError;
                    }
                }
                else
                {
                    m_ProductShareVariables.strStatusLabelText = "Sorting recipe file not exist.";
                    m_ProductGUIEvent.PCS_GUI_UpdateStatusLabel.Set();
                     nError = 1;
                    return nError;
                }
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return nError;
        }

        virtual public int LoadPickUpHeadRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                if (File.Exists(m_ProductShareVariables.strRecipePickUpHeadPath + m_ProductShareVariables.productRecipeMainSettings.PickUpHeadRecipeName + m_ProductShareVariables.strXmlExtension))
                {
                    if (LoadRecipePickUpHeadSettings())
                    {
                        if (SetRecipePickUpHeadSettingsToShareMemory())
                            m_ProductShareVariables.strStatusLabelText = "Update recipe Pick Up Head done.";
                        else
                        {
                            nError = 3;
                            return nError;
                        }
                    }
                    else
                    {
                        m_ProductShareVariables.strStatusLabelText = "Update recipe Pick Up Head fail.";

                        nError = 2;
                        return nError;
                    }
                }
                else
                {
                    m_ProductShareVariables.strStatusLabelText = "Pick Up Head recipe file not exist.";
                    m_ProductGUIEvent.PCS_GUI_UpdateStatusLabel.Set();
                    nError = 1;
                    return nError;
                }
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return nError;
        }

        
        virtual public int LoadVisionRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {                
                if (File.Exists(m_ProductShareVariables.strRecipeVisionPath + m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName + m_ProductShareVariables.strXmlExtension))
                {
                    if (LoadRecipeVisionSettings())
                    {
                        if (SetRecipeVisionSettingsToShareMemory())
                            m_ProductShareVariables.strStatusLabelText = "Update recipe vision done.";
                        else
                        {
                            nError = 3;
                            return nError;
                        }
                    }
                    else
                    {
                        m_ProductShareVariables.strStatusLabelText = "Update recipe vision fail.";
                        nError = 2;
                        return nError;
                    }
                }
                else
                {
                    m_ProductShareVariables.strStatusLabelText = "Vision recipe file not exist.";
                    m_ProductGUIEvent.PCS_GUI_UpdateStatusLabel.Set();
                    nError = 1;
                    return nError;
                }
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return nError;
        }
        
        override public void SetupTeachPointShareMemoryDuringEnterTeachPoint()
        {
            try
            {
                #region Turret Application
//#if TurretApplication
//                SetShareMemoryTeachPointLong("MarkingVisionMotorHomeOffset", 0);
//                SetShareMemoryTeachPointLong("RotaryAndAlignerAMotorHomeOffset", 0);
//                SetShareMemoryTeachPointLong("RotaryAndAlignerBMotorHomeOffset", 0);
//                SetShareMemoryTeachPointLong("SortingMotorHomeOffset", 0);
//                SetShareMemoryTeachPointLong("TestSite1HomeOffset", 0);
//                SetShareMemoryTeachPointLong("TestSite2HomeOffset", 0);
//                SetShareMemoryTeachPointLong("TestSite3HomeOffset", 0);
//                SetShareMemoryTeachPointLong("TestSite4HomeOffset", 0);
//#endif
                #endregion

            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }

        override public bool LoadConfigurationSettings()
        {
            try
            {
                m_ProductConfigurationSettings = new ProductConfigurationSettings();
                m_ProductConfigurationSettings = Tools.Deserialize<ProductConfigurationSettings>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strConfigurationFile);
                configurationSettings = m_ProductConfigurationSettings;
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

                #region Motion Controller
                SetShareMemoryCustomizeBool("EnableMotionController1", m_ProductConfigurationSettings.EnableMotionController1);
                SetShareMemoryCustomizeBool("EnableMotionController2", m_ProductConfigurationSettings.EnableMotionController2);
                SetShareMemoryCustomizeBool("EnableMotionController3", m_ProductConfigurationSettings.EnableMotionController3);
                SetShareMemoryCustomizeBool("EnableMotionController4", m_ProductConfigurationSettings.EnableMotionController4);
                SetShareMemoryCustomizeBool("EnableMotionController5", m_ProductConfigurationSettings.EnableMotionController5);
                SetShareMemoryCustomizeBool("EnableMotionController6", m_ProductConfigurationSettings.EnableMotionController6);
                SetShareMemoryCustomizeBool("EnableMotionController7", m_ProductConfigurationSettings.EnableMotionController7);
                #endregion Motion Controller

                #region Driver

                SetShareMemoryCustomizeBool("EnablePickAndPlace1XAxisMotor", m_ProductConfigurationSettings.EnablePickAndPlace1XAxisMotor);
                SetShareMemoryCustomizeBool("EnablePickAndPlace2XAxisMotor", m_ProductConfigurationSettings.EnablePickAndPlace2XAxisMotor);
                SetShareMemoryCustomizeBool("EnablePickAndPlace1YAxisMotor", m_ProductConfigurationSettings.EnablePickAndPlace1YAxisMotor);
                SetShareMemoryCustomizeBool("EnableInputTrayTableXAxisMotor", m_ProductConfigurationSettings.EnableInputTrayTableXAxisMotor);
                SetShareMemoryCustomizeBool("EnableInputTrayTableYAxisMotor", m_ProductConfigurationSettings.EnableInputTrayTableYAxisMotor);
                SetShareMemoryCustomizeBool("EnableInputTrayTableZAxisMotor", m_ProductConfigurationSettings.EnableInputTrayTableZAxisMotor);

                SetShareMemoryCustomizeBool("EnablePickAndPlace2YAxisMotor", m_ProductConfigurationSettings.EnablePickAndPlace2YAxisMotor);

                SetShareMemoryCustomizeBool("EnableOutputTrayTableXAxisMotor", m_ProductConfigurationSettings.EnableOutputTrayTableXAxisMotor);
                SetShareMemoryCustomizeBool("EnableOutputTrayTableYAxisMotor", m_ProductConfigurationSettings.EnableOutputTrayTableYAxisMotor);
                SetShareMemoryCustomizeBool("EnableOutputTrayTableZAxisMotor", m_ProductConfigurationSettings.EnableOutputTrayTableZAxisMotor);

                SetShareMemoryCustomizeBool("EnableInputVisionMotor", m_ProductConfigurationSettings.EnableInputVisionMotor);
                SetShareMemoryCustomizeBool("EnableS2VisionMotor", m_ProductConfigurationSettings.EnableS2VisionMotor);
                SetShareMemoryCustomizeBool("EnableS1VisionMotor", m_ProductConfigurationSettings.EnableS1VisionMotor);
                SetShareMemoryCustomizeBool("EnableS3VisionMotor", m_ProductConfigurationSettings.EnableS3VisionMotor);

                SetShareMemoryCustomizeBool("EnablePickAndPlace1Module", m_ProductConfigurationSettings.EnablePickAndPlace1Module);
                SetShareMemoryCustomizeBool("EnablePickAndPlace2Module", m_ProductConfigurationSettings.EnablePickAndPlace2Module);
                #endregion Driver

                #region Vision
                SetShareMemoryCustomizeBool("EnableInputVisionModule", m_ProductConfigurationSettings.EnableInputVisionModule);
                SetShareMemoryCustomizeBool("EnableS2VisionModule", m_ProductConfigurationSettings.EnableS2VisionModule);
                SetShareMemoryCustomizeBool("EnableBottomVisionModule", m_ProductConfigurationSettings.EnableBottomVisionModule);
                SetShareMemoryCustomizeBool("EnableS1VisionModule", m_ProductConfigurationSettings.EnableS1VisionModule);
                SetShareMemoryCustomizeBool("EnableS3VisionModule", m_ProductConfigurationSettings.EnableS3VisionModule);
                SetShareMemoryCustomizeBool("EnableOutputVisionModule", m_ProductConfigurationSettings.EnableOutputVisionModule);
                #endregion Vision

                #region Misc
                //SetShareMemoryCustomizeBool("EnableBarcodeReader", m_ProductConfigurationSettings.EnableBarcodeReader);

                //SetShareMemoryCustomizeBool("EnableAutoInputLoading", m_ProductConfigurationSettings.EnableAutoInputLoading);
                //SetShareMemoryCustomizeBool("EnableAutoOutputLoading", m_ProductConfigurationSettings.EnableAutoOutputLoading);

                #endregion Misc

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
                m_ProductOptionSettings = new ProductOptionSettings();
                m_ProductOptionSettings = Tools.Deserialize<ProductOptionSettings>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strOptionFile);
                optionSettings = m_ProductOptionSettings;
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

                SetShareMemorySettingArrayBool("EnablePH", 0, m_ProductOptionSettings.EnablePH1);
                SetShareMemorySettingArrayBool("EnablePH", 1, m_ProductOptionSettings.EnablePH2);

                SetShareMemorySettingBool("EnableVision", m_ProductOptionSettings.EnableVision);
                //SetShareMemorySettingBool("EnableInputVision", m_ProductOptionSettings.EnableInputVision);
                //SetShareMemorySettingBool("EnableS2Vision", m_ProductOptionSettings.EnableS2Vision);
                //SetShareMemorySettingBool("EnableSetupVision", m_ProductOptionSettings.EnableSetupVision);
                //SetShareMemorySettingBool("EnableBottomVision", m_ProductOptionSettings.EnableBottomVision);
                //SetShareMemorySettingBool("EnableSWLRVision", m_ProductOptionSettings.EnableSWLRVision);
                //SetShareMemorySettingBool("EnableSWFRVision", m_ProductOptionSettings.EnableSWFRVision);
                //SetShareMemorySettingBool("EnableS3Vision", m_ProductOptionSettings.EnableS3Vision);
                //SetShareMemorySettingBool("EnableOutputVision", m_ProductOptionSettings.EnableOutputVision);
                SetShareMemorySettingBool("EnableVisionWaitResult", m_ProductOptionSettings.EnableVisionWaitResult);
                //SetShareMemorySettingBool("EnableGoodSamplingSequence", m_ProductOptionSettings.EnableGoodSamplingSequence);

                SetShareMemorySettingBool("EnablePickupHeadRetryPickingNo", m_ProductOptionSettings.EnablePickupHeadRetryPickingNo);
                SetShareMemorySettingBool("EnablePickupHeadRetryPlacingNo", m_ProductOptionSettings.EnablePickupHeadRetryPlacingNo);
                SetShareMemorySettingBool("EnableBarcodePrinter", m_ProductOptionSettings.bEnableBarcodePrinter);
                SetShareMemorySettingUInt("PickupHeadRetryPickingNo", m_ProductOptionSettings.PickupHeadRetryPickingNo);
                SetShareMemorySettingUInt("PickupHeadRetryPlacingNo", m_ProductOptionSettings.PickupHeadRetryPlacingNo);
                //SetShareMemorySettingUInt("PulseWidth", m_ProductOptionSettings.PulseWidth);
                for (int i = 0; i < 2; i++)
                {
                    SetShareMemorySettingArrayLong("PickUpHeadCompensationXOffset", i, m_ProductOptionSettings.PickUpHeadCompensationXOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadCompensationYOffset", i, m_ProductOptionSettings.PickUpHeadCompensationYOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadOutputCompensationXOffset", i, m_ProductOptionSettings.PickUpHeadOutputCompensationXOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadOutputCompensationYOffset", i, m_ProductOptionSettings.PickUpHeadOutputCompensationYOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadOutputCompensationThetaOffset", i, m_ProductOptionSettings.PickUpHeadOutputCompensationThetaOffset[i]);

                    SetShareMemorySettingArrayLong("PickUpHeadHeadCompensationXOffset", i, m_ProductOptionSettings.PickUpHeadHeadCompensationXOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadHeadCompensationYOffset", i, m_ProductOptionSettings.PickUpHeadHeadCompensationYOffset[i]);

                    SetShareMemorySettingArrayLong("PickUpHeadRotationXOffset", i, m_ProductOptionSettings.PickUpHeadRotationXOffset[i]);
                    SetShareMemorySettingArrayLong("PickUpHeadRotationYOffset", i, m_ProductOptionSettings.PickUpHeadRotationYOffset[i]);
                }
                
                //SetShareMemoryCustomizeBool("EnableCheckingBarcodeID", m_ProductOptionSettings.bEnableCheckingBarcodeID);

                SetShareMemoryProductionInt("TrayPresentSensorOffTimeBeforeAlarm_ms", (m_ProductOptionSettings.TrayPresentSensorOffTimeBeforeAlarm_ms));

                //SetShareMemorySettingBool("EnableInputTableVacuum", m_ProductOptionSettings.EnableInputTableVacuum);
                SetShareMemorySettingBool("EnableCountDownByInputQuantity", m_ProductOptionSettings.EnableCountDownByInputQuantity);
                SetShareMemorySettingBool("EnableCountDownByInputTrayNo", m_ProductOptionSettings.EnableCountDownByInputTrayNo);
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
                m_ProductShareVariables.recipeMainSettings = Tools.Deserialize<ProductRecipeMainSettings>(m_ProductShareVariables.strRecipeMainPath + recipeMainName + m_ProductShareVariables.strXmlExtension);
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
                m_ProductRecipeInputSettings = new ProductRecipeInputSettings();
                m_ProductRecipeInputSettings = Tools.Deserialize<ProductRecipeInputSettings>(m_ProductShareVariables.strRecipeInputPath + m_ProductShareVariables.recipeMainSettings.InputRecipeName + m_ProductShareVariables.strXmlExtension);
                recipeInputSettings = m_ProductRecipeInputSettings;
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


                //SetShareMemorySettingUInt("DeviceSizeX_um", m_ProductRecipeInputSettings.DeviceSizeX_um);
                //SetShareMemorySettingUInt("DeviceSizeY_um", m_ProductRecipeInputSettings.DeviceSizeY_um);
                SetShareMemorySettingUInt("NoOfDeviceInColInput", m_ProductRecipeInputSettings.NoOfDeviceInColInput);
                SetShareMemorySettingUInt("NoOfDeviceInRowInput", m_ProductRecipeInputSettings.NoOfDeviceInRowInput);
                SetShareMemorySettingUInt("DeviceXPitchInput", m_ProductRecipeInputSettings.DeviceXPitchInput);
                SetShareMemorySettingUInt("DeviceYPitchInput", m_ProductRecipeInputSettings.DeviceYPitchInput);
                SetShareMemorySettingUInt("UnitThickness_um", m_ProductRecipeInputSettings.UnitThickness_um);
                SetShareMemorySettingUInt("InputPocketDepth_um", m_ProductRecipeInputSettings.InputPocketDepth_um);
                SetShareMemorySettingUInt("InputTrayThickness", m_ProductRecipeInputSettings.InputTrayThickness);


                SetShareMemorySettingLong("PickingCenterXOffsetInput", m_ProductRecipeInputSettings.PickingCenterXOffsetInput);
                SetShareMemorySettingLong("PickingCenterYOffsetInput", m_ProductRecipeInputSettings.PickingCenterYOffsetInput);
                SetShareMemorySettingLong("UnitPlacementRotationOffsetInput", m_ProductRecipeInputSettings.UnitPlacementRotationOffsetInput);

                SetShareMemorySettingLong("BottomVisionXOffset", m_ProductRecipeInputSettings.BottomVisionXOffset);
                SetShareMemorySettingLong("BottomVisionYOffset", m_ProductRecipeInputSettings.BottomVisionYOffset);
                SetShareMemorySettingLong("ContinuouslyEmptyPocket", m_ProductRecipeInputSettings.ContinuouslyEmptyPocket);

                SetShareMemorySettingUInt("EmptyUnit", (uint)m_ProductRecipeInputSettings.EmptyUnit);

                SetShareMemorySettingBool("EnableCheckEmptyUnit", m_ProductRecipeInputSettings.EnableCheckEmptyUnit);
                SetShareMemorySettingBool("EnableInputTableVacuum", m_ProductRecipeInputSettings.EnableInputTableVacuum);
                SetShareMemorySettingBool("EnablePurging", m_ProductRecipeInputSettings.EnablePurging);

                
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
                m_ProductRecipeOutputSettings = new ProductRecipeOutputSettings();
                m_ProductRecipeOutputSettings = Tools.Deserialize<ProductRecipeOutputSettings>(m_ProductShareVariables.strRecipeOutputPath + m_ProductShareVariables.recipeMainSettings.OutputRecipeName + m_ProductShareVariables.strXmlExtension);
                recipeOutputSettings = m_ProductRecipeOutputSettings;
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
                
                SetShareMemorySettingUInt("NoOfDeviceInColOutput", m_ProductRecipeOutputSettings.NoOfDeviceInColOutput);
                SetShareMemorySettingUInt("NoOfDeviceInRowOutput", m_ProductRecipeOutputSettings.NoOfDeviceInRowOutput);
                SetShareMemorySettingUInt("DeviceXPitchOutput", m_ProductRecipeOutputSettings.DeviceXPitchOutput);
                SetShareMemorySettingUInt("DeviceYPitchOutput", m_ProductRecipeOutputSettings.DeviceYPitchOutput);
                SetShareMemorySettingUInt("OutputPocketDepth_um", m_ProductRecipeOutputSettings.OutputPocketDepth_um);
                SetShareMemorySettingUInt("OutputTrayThickness", m_ProductRecipeOutputSettings.OutputTrayThickness);
                SetShareMemorySettingUInt("LowYieldAlarm", m_ProductRecipeOutputSettings.LowYieldAlarm);

                SetShareMemorySettingLong("TableXOffsetOutput", m_ProductRecipeOutputSettings.TableXOffsetOutput);
                SetShareMemorySettingLong("TableYOffsetOutput", m_ProductRecipeOutputSettings.TableYOffsetOutput);
                SetShareMemorySettingLong("UnitPlacementRotationOffsetOutput", m_ProductRecipeOutputSettings.UnitPlacementRotationOffsetOutput);
                SetShareMemorySettingBool("EnableOutputTableVacuum", m_ProductRecipeOutputSettings.EnableOutputTableVacuum);
            
                //SetShareMemorySettingBool("EnableGoodSamplingSequence", m_ProductRecipeOutputSettings.EnableGoodSamplingSequence);
                //SetShareMemorySettingBool("EnableScanBarcodeOnOutputTray", m_ProductRecipeOutputSettings.EnableScanBarcodeOnOutputTray);

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
                m_ProductRecipeDelaySettings = new ProductRecipeDelaySettings();
                m_ProductRecipeDelaySettings = Tools.Deserialize<ProductRecipeDelaySettings>(m_ProductShareVariables.strRecipeDelayPath + m_ProductShareVariables.recipeMainSettings.DelayRecipeName + m_ProductShareVariables.strXmlExtension);
                recipeDelaySettings = m_ProductRecipeDelaySettings;
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
                FieldInfo[] fields = typeof(ProductRecipeDelaySettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    SetShareMemorySettingUInt(_fields.Name, (uint)_fields.GetValue(m_ProductRecipeDelaySettings));
                }
                //SetShareMemorySettingUInt("DelayBeforeInputPusherMoveDown", m_ProductRecipeDelaySettings.DelayBeforeInputPusherMoveDown);

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
                m_ProductRecipeMotorPositionSettings = new ProductRecipeMotorPositionSettings();
                m_ProductRecipeMotorPositionSettings = Tools.Deserialize<ProductRecipeMotorPositionSettings>(m_ProductShareVariables.strRecipeMotorPositionPath + m_ProductShareVariables.recipeMainSettings.MotorPositionRecipeName + m_ProductShareVariables.strXmlExtension);
                recipeMotorPositionSettings = m_ProductRecipeMotorPositionSettings;
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
                //SetShareMemorySettingLong("InputPusherOffsetFromTouchingPosition", m_ProductRecipeMotorPositionSettings.InputPusherDownDistance);
                FieldInfo[] fields = typeof(ProductRecipeMotorPositionSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    SetShareMemorySettingLong(_fields.Name, (int)_fields.GetValue(m_ProductRecipeMotorPositionSettings));
                }
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
                m_ProductTeachPointSettings = new ProductTeachPointSettings();
                m_ProductTeachPointSettings = Tools.Deserialize<ProductTeachPointSettings>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strTeachPointFile);
                teachPointSettings = m_ProductTeachPointSettings;
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
                FieldInfo[] fields = typeof(ProductTeachPointSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                MotionLibrary.Motion.MotorProfile motionProfile = new MotionLibrary.Motor.MotorProfile();
                foreach (FieldInfo _fields in fields)
                {
                    if (_fields.FieldType.Name == "MotorProfile")
                    {
                        try
                        {
                            motionProfile = (MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_ProductTeachPointSettings);
                            SetShareMemoryTeachPointLong(_fields.Name, motionProfile.TeachPoint);
                        }
                        catch(Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(_fields.Name);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        //virtual public bool LoadRecipeInputCassetteSettings()
        //{
        //    try
        //    {
        //        m_ProductRecipeInputCassetteSettings = new ProductRecipeCassetteSettings();
        //        m_ProductRecipeInputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_ProductShareVariables.strRecipeInputCassettePath + m_ProductShareVariables.productRecipeMainSettings.InputCassetteRecipeName + m_ProductShareVariables.strXmlExtension);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        //virtual public bool SetRecipeInputCassetteSettingsToShareMemory()
        //{
        //    try
        //    {
        //        SetShareMemorySettingUInt("InputCassetteTotalSlot", m_ProductRecipeInputCassetteSettings.CassetteTotalSlot);
        //        SetShareMemorySettingUInt("InputCassetteSlotPitch_um", m_ProductRecipeInputCassetteSettings.CassetteSlotPitch_um);

        //        SetShareMemorySettingLong("InputCassetteFirstSlotOffset_um", m_ProductRecipeInputCassetteSettings.CassetteFirstSlotOffset_um);
        //        SetShareMemorySettingLong("InputCassetteUnloadOffset_um", m_ProductRecipeInputCassetteSettings.CassetteUnloadOffset_um);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        //virtual public bool LoadRecipeOutputCassetteSettings()
        //{
        //    try
        //    {
        //        m_ProductRecipeOutputCassetteSettings = new ProductRecipeCassetteSettings();
        //        m_ProductRecipeOutputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_ProductShareVariables.strRecipeOutputCassettePath + m_ProductShareVariables.productRecipeMainSettings.OutputCassetteRecipeName + m_ProductShareVariables.strXmlExtension);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}

        virtual public bool LoadRecipeSortingSettings()
        {
            try
            {
                m_ProductRecipeSortingSetting = new ProductRecipeSortingSetting();
                m_ProductRecipeSortingSetting = Tools.Deserialize<ProductRecipeSortingSetting>(m_ProductShareVariables.strRecipeSortingPath + m_ProductShareVariables.productRecipeMainSettings.SortingRecipeName + m_ProductShareVariables.strXmlExtension);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool LoadRecipePickUpHeadSettings()
        {
            try
            {
                m_ProductRecipePickUpHeadSetting = new ProductRecipePickUpHeadSeting();
                m_ProductRecipePickUpHeadSetting = Tools.Deserialize<ProductRecipePickUpHeadSeting>(m_ProductShareVariables.strRecipePickUpHeadPath + m_ProductShareVariables.productRecipeMainSettings.PickUpHeadRecipeName + m_ProductShareVariables.strXmlExtension);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool SetRecipeOutputCassetteSettingsToShareMemory()
        {
            try
            {
                SetShareMemorySettingUInt("OutputCassetteTotalSlot", m_ProductRecipeOutputCassetteSettings.CassetteTotalSlot);
                SetShareMemorySettingUInt("OutputCassetteSlotPitch_um", m_ProductRecipeOutputCassetteSettings.CassetteSlotPitch_um);

                SetShareMemorySettingLong("OutputCassetteFirstSlotOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteFirstSlotOffset_um);
                SetShareMemorySettingLong("OutputCassetteUnloadOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteUnloadOffset_um);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool SetRecipeSortingSettingsToShareMemory()
        {
            try
            {
                SetShareMemoryProductionBool("EnableSortingMode", m_ProductRecipeSortingSetting.EnableSortingMode);
                //SetShareMemorySettingUInt("OutputCassetteSlotPitch_um", m_ProductRecipeOutputCassetteSettings.CassetteSlotPitch_um);

                //SetShareMemorySettingLong("OutputCassetteFirstSlotOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteFirstSlotOffset_um);
                //SetShareMemorySettingLong("OutputCassetteUnloadOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteUnloadOffset_um);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool SetRecipePickUpHeadSettingsToShareMemory()
        {
            try
            {
                SetShareMemorySettingBool("EnablePickSoftlanding", m_ProductRecipePickUpHeadSetting.EnablePickSoftlanding);
                SetShareMemorySettingBool("EnablePlaceSoftlanding", m_ProductRecipePickUpHeadSetting.EnablePlaceSoftlanding);
                
                SetShareMemorySettingDouble("PickUpHead1FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead1FlowRate);
                SetShareMemorySettingDouble("PickUpHead1Force", m_ProductRecipePickUpHeadSetting.PickUpHead1Force);
                SetShareMemorySettingDouble("PickUpHead1Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead1Pressure);

                SetShareMemorySettingDouble("PickUpHead2FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead2FlowRate);
                SetShareMemorySettingDouble("PickUpHead2Force", m_ProductRecipePickUpHeadSetting.PickUpHead2Force);
                SetShareMemorySettingDouble("PickUpHead2Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead2Pressure);

                //SetShareMemorySettingDouble("PickUpHead3FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead3FlowRate);
                //SetShareMemorySettingDouble("PickUpHead3Force", m_ProductRecipePickUpHeadSetting.PickUpHead3Force);
                //SetShareMemorySettingDouble("PickUpHead3Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead3Pressure);

                //SetShareMemorySettingDouble("PickUpHead4FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead4FlowRate);
                //SetShareMemorySettingDouble("PickUpHead4Force", m_ProductRecipePickUpHeadSetting.PickUpHead4Force);
                //SetShareMemorySettingDouble("PickUpHead4Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead4Pressure);

                //SetShareMemorySettingDouble("PickUpHead5FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead5FlowRate);
                //SetShareMemorySettingDouble("PickUpHead5Force", m_ProductRecipePickUpHeadSetting.PickUpHead5Force);
                //SetShareMemorySettingDouble("PickUpHead5Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead5Pressure);

                //SetShareMemorySettingDouble("PickUpHead6FlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead6FlowRate);
                //SetShareMemorySettingDouble("PickUpHead6Force", m_ProductRecipePickUpHeadSetting.PickUpHead6Force);
                //SetShareMemorySettingDouble("PickUpHead6Pressure", m_ProductRecipePickUpHeadSetting.PickUpHead6Pressure);

                SetShareMemorySettingDouble("PickUpHead1PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceFlowRate);
                SetShareMemorySettingDouble("PickUpHead1PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceForce);
                SetShareMemorySettingDouble("PickUpHead1PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead1PlacePressure);

                SetShareMemorySettingDouble("PickUpHead2PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceFlowRate);
                SetShareMemorySettingDouble("PickUpHead2PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceForce);
                SetShareMemorySettingDouble("PickUpHead2PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead2PlacePressure);

                //SetShareMemorySettingDouble("PickUpHead3PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead3PlaceFlowRate);
                //SetShareMemorySettingDouble("PickUpHead3PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead3PlaceForce);
                //SetShareMemorySettingDouble("PickUpHead3PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead3PlacePressure);

                //SetShareMemorySettingDouble("PickUpHead4PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead4PlaceFlowRate);
                //SetShareMemorySettingDouble("PickUpHead4PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead4PlaceForce);
                //SetShareMemorySettingDouble("PickUpHead4PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead4PlacePressure);

                //SetShareMemorySettingDouble("PickUpHead5PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead5PlaceFlowRate);
                //SetShareMemorySettingDouble("PickUpHead5PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead5PlaceForce);
                //SetShareMemorySettingDouble("PickUpHead5PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead5PlacePressure);

                //SetShareMemorySettingDouble("PickUpHead6PlaceFlowRate", m_ProductRecipePickUpHeadSetting.PickUpHead6PlaceFlowRate);
                //SetShareMemorySettingDouble("PickUpHead6PlaceForce", m_ProductRecipePickUpHeadSetting.PickUpHead6PlaceForce);
                //SetShareMemorySettingDouble("PickUpHead6PlacePressure", m_ProductRecipePickUpHeadSetting.PickUpHead6PlacePressure);


                //SetShareMemorySettingUInt("OutputCassetteSlotPitch_um", m_ProductRecipeOutputCassetteSettings.CassetteSlotPitch_um);

                //SetShareMemorySettingLong("OutputCassetteFirstSlotOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteFirstSlotOffset_um);
                //SetShareMemorySettingLong("OutputCassetteUnloadOffset_um", m_ProductRecipeOutputCassetteSettings.CassetteUnloadOffset_um);

                SetShareMemorySettingBool("EnableSafetyPnPMovePickStation", m_ProductRecipePickUpHeadSetting.EnableSafetyPnPMovePickStation);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool LoadRecipeVisionSettings()
        {
            try
            {
                m_ProductRecipeVisionSetting = new ProductRecipeVisionSettings();
                m_ProductRecipeVisionSetting = Tools.Deserialize<ProductRecipeVisionSettings>(m_ProductShareVariables.strRecipeVisionPath + m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName + m_ProductShareVariables.strXmlExtension);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        //public bool SetPnPDataOffsetToShareMemory(List<LookUpTableOffsetData> lookUpTables1, List<LookUpTableOffsetData> lookUpTables2)
        //{
        //    try
        //    {
        //        int NoPnP1 = 0;
        //        foreach (LookUpTableOffsetData PnP1LookUpTable in lookUpTables1)
        //        {
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "Angle", PnP1LookUpTable.Angle);
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "XOffset", PnP1LookUpTable.XOffset);
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "YOffset", PnP1LookUpTable.YOffset);
        //            NoPnP1++;
        //        }
        //        int NoPnP2 = 0;
        //        foreach (LookUpTableOffsetData PnP2LookUpTable in lookUpTables2)
        //        {
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "Angle", PnP2LookUpTable.Angle);
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "XOffset", PnP2LookUpTable.XOffset);
        //            SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "YOffset", PnP2LookUpTable.YOffset);
        //            NoPnP2++;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
        //        return false;
        //    }
        //}
        public bool SetPnPDataOffsetToShareMemory(List<LookUpTableOffsetData> lookUpTables1, List<LookUpTableOffsetData> lookUpTables2)
        {
            try
            {
                int NoPnP1 = 0;
                foreach (LookUpTableOffsetData PnP1LookUpTable in lookUpTables1)
                {
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "Angle", PnP1LookUpTable.Angle);
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "XOffset", PnP1LookUpTable.XOffset);
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData1", NoPnP1, "YOffset", PnP1LookUpTable.YOffset);
                    NoPnP1++;
                }
                int NoPnP2 = 0;
                foreach (LookUpTableOffsetData PnP2LookUpTable in lookUpTables2)
                {
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "Angle", PnP2LookUpTable.Angle);
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "XOffset", PnP2LookUpTable.XOffset);
                    SetSettingDoubleArray("PickAndPlaceLookUpTableData2", NoPnP2, "YOffset", PnP2LookUpTable.YOffset);
                    NoPnP2++;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }

        virtual public bool SetRecipeVisionSettingsToShareMemory()
        {
            try
            {
                #region Input Vision
                SetShareMemorySettingBool("EnableInputVision", m_ProductRecipeVisionSetting.EnableInputVision);
                //SetShareMemorySettingUInt("InputVisionInspectionCountInCol", m_ProductRecipeVisionSetting.InputVisionInspectionCountInCol);
                //SetShareMemorySettingUInt("InputVisionInspectionCountInRow", m_ProductRecipeVisionSetting.InputVisionInspectionCountInRow);
                SetShareMemorySettingUInt("InputVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.InputVisionRetryCountAfterFail);
				SetShareMemorySettingUInt("InputVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.InputVisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("InputVisionUnitThetaOffset", m_ProductRecipeVisionSetting.InputVisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("InputVision", i, "No", 0);
                    SetSettingArray("InputVision", i, "Enable", 0);
                    SetSettingArray("InputVision", i, "FocusOffset_um", 0);
                    SetSettingArray("InputVision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("InputVision", i, "XOffset_um", 0);
                    SetSettingArray("InputVision", i, "YOffset_um", 0);
                }
                int NoInput = 0;
                foreach (VisionSnapInfo _InfoInp in m_ProductRecipeVisionSetting.listInputVisionSnap)
                {
                    SetSettingArray("InputVision", NoInput, "No", _InfoInp.SnapNo);
                    SetSettingArray("InputVision", NoInput, "Enable", _InfoInp.EnableSnap ? 1 : 0);
                    SetSettingArray("InputVision", NoInput, "FocusOffset_um", _InfoInp.VisionZAxisOffset);
                    SetSettingArray("InputVision", NoInput, "ThetaOffset_mDegree", _InfoInp.VisionThetaAxisOffset);
                    SetSettingArray("InputVision", NoInput, "XOffset_um", _InfoInp.VisionXAxisOffset);
                    SetSettingArray("InputVision", NoInput, "YOffset_um", _InfoInp.VisionYAxisOffset);
                    if(_InfoInp.EnableSnap == true && m_ProductShareVariables.LastInputSnapNo<_InfoInp.SnapNo)
                    {
                        m_ProductShareVariables.LastInputSnapNo = _InfoInp.SnapNo;
                    }
                    NoInput++;
                }
                //SetShareMemorySettingBool("TriggerTeachAlignment", m_ProductRecipeVisionSetting.TriggerTeachAlignment);
                #endregion Input Vision

                #region Bottom Vision
                SetShareMemorySettingBool("EnableBottomVision", m_ProductRecipeVisionSetting.EnableBottomVision);
                //SetShareMemorySettingUInt("BTMVisionInspectionCountInCol", m_ProductRecipeVisionSetting.BTMVisionInspectionCountInCol);
                //SetShareMemorySettingUInt("BTMVisionInspectionCountInRow", m_ProductRecipeVisionSetting.BTMVisionInspectionCountInRow);
                SetShareMemorySettingUInt("BottomVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.BottomVisionRetryCountAfterFail);
                SetShareMemorySettingUInt("BottomVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.BottomVisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("BottomVisionUnitThetaOffset", m_ProductRecipeVisionSetting.BottomVisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("BottomVision", i, "No", 0);
                    SetSettingArray("BottomVision", i, "Enable", 0);
                    SetSettingArray("BottomVision", i, "FocusOffset_um", 0);
                    SetSettingArray("BottomVision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("BottomVision", i, "XOffset_um", 0);
                    SetSettingArray("BottomVision", i, "YOffset_um", 0);
                }
                int NoBtm = 0;
                foreach (VisionSnapInfo _InfoBtm in m_ProductRecipeVisionSetting.listBottomVisionSnap)
                {
                    SetSettingArray("BottomVision", NoBtm, "No", _InfoBtm.SnapNo);
                    SetSettingArray("BottomVision", NoBtm, "Enable", _InfoBtm.EnableSnap ? 1 : 0);
                    SetSettingArray("BottomVision", NoBtm, "FocusOffset_um", _InfoBtm.VisionZAxisOffset);
                    SetSettingArray("BottomVision", NoBtm, "ThetaOffset_mDegree", _InfoBtm.VisionThetaAxisOffset);
                    SetSettingArray("BottomVision", NoBtm, "XOffset_um", _InfoBtm.VisionXAxisOffset);
                    SetSettingArray("BottomVision", NoBtm, "YOffset_um", _InfoBtm.VisionYAxisOffset);
                    SetSettingArray("BottomVision", NoBtm, "EnableDiffuser", _InfoBtm.EnableDiffuser ? 1 : 0);
                    if (_InfoBtm.EnableSnap == true && m_ProductShareVariables.LastS1SnapNo < _InfoBtm.SnapNo)
                    {
                        m_ProductShareVariables.LastS1SnapNo = _InfoBtm.SnapNo;
                    }
                    NoBtm++;
                }

                #endregion Bottom Vision

                #region Setup Vision
                SetShareMemorySettingBool("EnableSetupVision", m_ProductRecipeVisionSetting.EnableSetupVision);
                //SetShareMemorySettingUInt("SetupVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.SetupVisionRetryCountAfterFail);
                SetShareMemorySettingUInt("SetupVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.SetupVisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("SetupVisionUnitThetaOffset", m_ProductRecipeVisionSetting.SetupVisionUnitThetaOffset);

                //SetShareMemorySettingBool("TriggerTeachAlignment", m_ProductRecipeVisionSetting.TriggerTeachAlignment);
                #endregion Setup Vision

                #region S1 Vision
                SetShareMemorySettingBool("EnableS1Vision", m_ProductRecipeVisionSetting.EnableS1Vision);
                //SetShareMemorySettingUInt("S1VisionInspectionCountInCol", m_ProductRecipeVisionSetting.S1VisionInspectionCountInCol);
                //SetShareMemorySettingUInt("S1VisionInspectionCountInRow", m_ProductRecipeVisionSetting.S1VisionInspectionCountInRow);
                //SetShareMemorySettingUInt("S1VisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.S1VisionRetryCountAfterFail);
                //SetShareMemorySettingUInt("S1VisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.S1VisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("S1VisionUnitThetaOffset", m_ProductRecipeVisionSetting.S1VisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("S1Vision", i, "No", 0);
                    SetSettingArray("S1Vision", i, "Enable", 0);
                    SetSettingArray("S1Vision", i, "FocusOffset_um", 0);
                    SetSettingArray("S1Vision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("S1Vision", i, "XOffset_um", 0);
                    SetSettingArray("S1Vision", i, "YOffset_um", 0);
                }
                int NoS1 = 0;
                foreach (VisionSnapInfo _InfoS1 in m_ProductRecipeVisionSetting.listS1VisionSnap)
                {
                    SetSettingArray("S1Vision", NoS1, "No", _InfoS1.SnapNo);
                    SetSettingArray("S1Vision", NoS1, "Enable", _InfoS1.EnableSnap ? 1 : 0);
                    SetSettingArray("S1Vision", NoS1, "FocusOffset_um", _InfoS1.VisionZAxisOffset);
                    SetSettingArray("S1Vision", NoS1, "ThetaOffset_mDegree", _InfoS1.VisionThetaAxisOffset);
                    SetSettingArray("S1Vision", NoS1, "XOffset_um", _InfoS1.VisionXAxisOffset);
                    SetSettingArray("S1Vision", NoS1, "YOffset_um", _InfoS1.VisionYAxisOffset);
                    if (_InfoS1.EnableSnap == true && m_ProductShareVariables.LastS1SnapNo < _InfoS1.SnapNo)
                    {
                        m_ProductShareVariables.LastS1SnapNo = _InfoS1.SnapNo;
                    }
                    NoS1++;
                }

                #endregion S1 Vision

                #region S2 Parting
                SetShareMemorySettingBool("EnableS2Vision", m_ProductRecipeVisionSetting.EnableS2Vision);
                //SetShareMemorySettingUInt("S2VisionInspectionCountInCol", m_ProductRecipeVisionSetting.S2VisionInspectionCountInCol);
                //SetShareMemorySettingUInt("S2VisionInspectionCountInRow", m_ProductRecipeVisionSetting.S2VisionInspectionCountInRow);
               // SetShareMemorySettingUInt("S2VisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.S2VisionRetryCountAfterFail);
                //SetShareMemorySettingUInt("S2VisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.S2VisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("S2VisionUnitThetaOffset", m_ProductRecipeVisionSetting.S2VisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("S2Vision", i, "No", 0);
                    SetSettingArray("S2Vision", i, "Enable", 0);
                    SetSettingArray("S2Vision", i, "FocusOffset_um", 0);
                    SetSettingArray("S2Vision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("S2Vision", i, "XOffset_um", 0);
                    SetSettingArray("S2Vision", i, "YOffset_um", 0);
                }
                int NoS2 = 0;
                foreach (VisionSnapInfo _InfoS2 in m_ProductRecipeVisionSetting.listS2VisionSnap)
                {
                    SetSettingArray("S2Vision", NoS2, "No", _InfoS2.SnapNo);
                    SetSettingArray("S2Vision", NoS2, "Enable", _InfoS2.EnableSnap ? 1 : 0);
                    SetSettingArray("S2Vision", NoS2, "FocusOffset_um", _InfoS2.VisionZAxisOffset);
                    SetSettingArray("S2Vision", NoS2, "ThetaOffset_mDegree", _InfoS2.VisionThetaAxisOffset);
                    SetSettingArray("S2Vision", NoS2, "XOffset_um", _InfoS2.VisionXAxisOffset);
                    SetSettingArray("S2Vision", NoS2, "YOffset_um", _InfoS2.VisionYAxisOffset);
                    if (_InfoS2.EnableSnap == true && m_ProductShareVariables.LastS2SnapNo < _InfoS2.SnapNo)
                    {
                        m_ProductShareVariables.LastS2SnapNo = _InfoS2.SnapNo;
                    }
                    NoS2++;
                }

                #endregion S2 Parting

                #region S2 Facet
                SetShareMemorySettingUInt("S2FacetVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.S2FacetVisionRetryCountAfterFail);
                //SetShareMemorySettingUInt("S2FacetVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.S2FacetVisionContinuousFailCountToTriggerAlarm);
                //SetShareMemorySettingLong("S2FacetVisionUnitThetaOffset", m_ProductRecipeVisionSetting.S2FacetVisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("S2FacetVision", i, "No", 0);
                    SetSettingArray("S2FacetVision", i, "Enable", 0);
                    SetSettingArray("S2FacetVision", i, "FocusOffset_um", 0);
                    SetSettingArray("S2FacetVision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("S2FacetVision", i, "XOffset_um", 0);
                    SetSettingArray("S2FacetVision", i, "YOffset_um", 0);
                }
                int NoS2Facet = 0;
                foreach (VisionSnapInfo _InfoS2 in m_ProductRecipeVisionSetting.listS2FacetVisionSnap)
                {
                    SetSettingArray("S2FacetVision", NoS2Facet, "No", _InfoS2.SnapNo);
                    SetSettingArray("S2FacetVision", NoS2Facet, "Enable", _InfoS2.EnableSnap ? 1 : 0);
                    SetSettingArray("S2FacetVision", NoS2Facet, "FocusOffset_um", _InfoS2.VisionZAxisOffset);
                    SetSettingArray("S2FacetVision", NoS2Facet, "ThetaOffset_mDegree", _InfoS2.VisionThetaAxisOffset);
                    SetSettingArray("S2FacetVision", NoS2Facet, "XOffset_um", _InfoS2.VisionXAxisOffset);
                    SetSettingArray("S2FacetVision", NoS2Facet, "YOffset_um", _InfoS2.VisionYAxisOffset);
                    if (_InfoS2.EnableSnap == true && m_ProductShareVariables.LastS2FacetSnapNo < _InfoS2.SnapNo)
                    {
                        m_ProductShareVariables.LastS2FacetSnapNo = _InfoS2.SnapNo;
                    }
                    NoS2Facet++;
                }

                #endregion S2 Facet

                #region S3 Parting
                SetShareMemorySettingBool("EnableS3Vision", m_ProductRecipeVisionSetting.EnableS3Vision);
                //SetShareMemorySettingUInt("S3VisionInspectionCountInCol", m_ProductRecipeVisionSetting.S3VisionInspectionCountInCol);
                //SetShareMemorySettingUInt("S3VisionInspectionCountInRow", m_ProductRecipeVisionSetting.S3VisionInspectionCountInRow);
               // SetShareMemorySettingUInt("S3VisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.S3VisionRetryCountAfterFail);
                //SetShareMemorySettingUInt("S3VisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.S3VisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("S3VisionUnitThetaOffset", m_ProductRecipeVisionSetting.S3VisionUnitThetaOffset);
                for(int i =0; i<10;i++)
                {
                    SetSettingArray("S3Vision", i, "No", 0);
                    SetSettingArray("S3Vision", i, "Enable", 0);
                    SetSettingArray("S3Vision", i, "FocusOffset_um", 0);
                    SetSettingArray("S3Vision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("S3Vision", i, "XOffset_um", 0);
                    SetSettingArray("S3Vision", i, "YOffset_um", 0);
                }
                int NoS3 = 0;
                foreach (VisionSnapInfo _InfoS3 in m_ProductRecipeVisionSetting.listS3VisionSnap)
                {
                    SetSettingArray("S3Vision", NoS3, "No", _InfoS3.SnapNo);
                    SetSettingArray("S3Vision", NoS3, "Enable", _InfoS3.EnableSnap ? 1 : 0);
                    SetSettingArray("S3Vision", NoS3, "FocusOffset_um", _InfoS3.VisionZAxisOffset);
                    SetSettingArray("S3Vision", NoS3, "ThetaOffset_mDegree", _InfoS3.VisionThetaAxisOffset);
                    SetSettingArray("S3Vision", NoS3, "XOffset_um", _InfoS3.VisionXAxisOffset);
                    SetSettingArray("S3Vision", NoS3, "YOffset_um", _InfoS3.VisionYAxisOffset);
                    if (_InfoS3.EnableSnap == true && m_ProductShareVariables.LastS3SnapNo < _InfoS3.SnapNo)
                    {
                        m_ProductShareVariables.LastS3SnapNo = _InfoS3.SnapNo;
                    }
                    NoS3++;
                }

                #endregion S3 Parting

                #region S3 Facet
                SetShareMemorySettingUInt("S3FacetVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.S3FacetVisionRetryCountAfterFail);
                //SetShareMemorySettingUInt("S3FacetVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.S3FacetVisionContinuousFailCountToTriggerAlarm);
                //SetShareMemorySettingLong("S3FacetVisionUnitThetaOffset", m_ProductRecipeVisionSetting.S3FacetVisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("S3FacetVision", i, "No", 0);
                    SetSettingArray("S3FacetVision", i, "Enable", 0);
                    SetSettingArray("S3FacetVision", i, "FocusOffset_um", 0);
                    SetSettingArray("S3FacetVision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("S3FacetVision", i, "XOffset_um", 0);
                    SetSettingArray("S3FacetVision", i, "YOffset_um", 0);
                }
                int NoS3Facet = 0;
                foreach (VisionSnapInfo _InfoS3 in m_ProductRecipeVisionSetting.listS3FacetVisionSnap)
                {
                    SetSettingArray("S3FacetVision", NoS3Facet, "No", _InfoS3.SnapNo);
                    SetSettingArray("S3FacetVision", NoS3Facet, "Enable", _InfoS3.EnableSnap ? 1 : 0);
                    SetSettingArray("S3FacetVision", NoS3Facet, "FocusOffset_um", _InfoS3.VisionZAxisOffset);
                    SetSettingArray("S3FacetVision", NoS3Facet, "ThetaOffset_mDegree", _InfoS3.VisionThetaAxisOffset);
                    SetSettingArray("S3FacetVision", NoS3Facet, "XOffset_um", _InfoS3.VisionXAxisOffset);
                    SetSettingArray("S3FacetVision", NoS3Facet, "YOffset_um", _InfoS3.VisionYAxisOffset);
                    if (_InfoS3.EnableSnap == true && m_ProductShareVariables.LastS3FacetSnapNo < _InfoS3.SnapNo)
                    {
                        m_ProductShareVariables.LastS3FacetSnapNo = _InfoS3.SnapNo;
                    }
                    NoS3Facet++;
                }

                #endregion S3 Facet

                SetShareMemorySettingBool("EnableS2S3BothSnapping", m_ProductRecipeVisionSetting.EnableS2S3BothSnapping);

                #region Output Vision
                SetShareMemorySettingBool("EnableOutputVision", m_ProductRecipeVisionSetting.EnableOutputVision);
                SetShareMemorySettingBool("EnableOutputVision2ndPostAlign", m_ProductRecipeVisionSetting.EnableOutputVision2ndPostAlign);
               // SetShareMemorySettingUInt("OutputVisionRetryCountAfterFail", (uint)m_ProductRecipeVisionSetting.OutputVisionRetryCountAfterFail);

                //SetShareMemorySettingUInt("OutputVisionContinuousFailCountToTriggerAlarm", (uint)m_ProductRecipeVisionSetting.OutputVisionContinuousFailCountToTriggerAlarm);
                SetShareMemorySettingLong("OutputVisionUnitThetaOffset", m_ProductRecipeVisionSetting.OutputVisionUnitThetaOffset);
                for (int i = 0; i < 10; i++)
                {
                    SetSettingArray("OutputVision", i, "No", 0);
                    SetSettingArray("OutputVision", i, "Enable", 0);
                    SetSettingArray("OutputVision", i, "FocusOffset_um", 0);
                    SetSettingArray("OutputVision", i, "ThetaOffset_mDegree", 0);
                    SetSettingArray("OutputVision", i, "XOffset_um", 0);
                    SetSettingArray("OutputVision", i, "YOffset_um", 0);
                }
                int NoOut = 0;
                foreach (VisionSnapInfo _InfoOut in m_ProductRecipeVisionSetting.listOutputVisionSnap)
                {
                    SetSettingArray("OutputVision", NoOut, "No", _InfoOut.SnapNo);
                    SetSettingArray("OutputVision", NoOut, "Enable", _InfoOut.EnableSnap ? 1 : 0);
                    SetSettingArray("OutputVision", NoOut, "FocusOffset_um", _InfoOut.VisionZAxisOffset);
                    SetSettingArray("OutputVision", NoOut, "ThetaOffset_mDegree", _InfoOut.VisionThetaAxisOffset);
                    SetSettingArray("OutputVision", NoOut, "XOffset_um", _InfoOut.VisionXAxisOffset);
                    SetSettingArray("OutputVision", NoOut, "YOffset_um", _InfoOut.VisionYAxisOffset);
                    NoOut++;
                }
                #endregion Output Vision

                SetShareMemorySettingBool("EnableTeachUnitAtVision", m_ProductRecipeVisionSetting.EnableTeachUnitAtVision);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return false;
            }
        }
        override public int LoadInputRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                nError = base.LoadInputRecipeToSettingShareMemory();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return nError;
            }
        }
        override public int LoadOutputRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                nError = base.LoadOutputRecipeToSettingShareMemory();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return nError;
            }
        }
        override public int LoadDelayRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                nError = base.LoadDelayRecipeToSettingShareMemory();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return nError;
            }
        }
        override public int LoadMotorPositionRecipeToSettingShareMemory()
        {
            int nError = 0;
            try
            {
                nError = base.LoadMotorPositionRecipeToSettingShareMemory();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return nError;
            }
        }
    }
}
