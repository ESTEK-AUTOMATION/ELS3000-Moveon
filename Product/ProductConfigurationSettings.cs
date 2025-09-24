using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductConfigurationSettings : Machine.ConfigurationSettings
    {
        //public Machine.ConfigurationSettings configurationSettings = new Machine.ConfigurationSettings();
        #region Motion Controller
        public bool EnableMotionController1 = false;
        public bool EnableMotionController2 = false;
        public bool EnableMotionController3 = false;
        public bool EnableMotionController4 = false;
        public bool EnableMotionController5 = false;
        public bool EnableMotionController6 = false;
        public bool EnableMotionController7 = false;
        #endregion Motion Controller

        #region Driver
        public bool EnablePickAndPlace1XAxisMotor = false;
        public bool EnablePickAndPlace1YAxisMotor = false;

        public bool EnablePickAndPlace2XAxisMotor = false;
        public bool EnablePickAndPlace2YAxisMotor = false;

        public bool EnableInputTrayTableXAxisMotor = false;
        public bool EnableInputTrayTableYAxisMotor = false;
        public bool EnableInputTrayTableZAxisMotor = false;

        public bool EnableOutputTrayTableXAxisMotor = false;
        public bool EnableOutputTrayTableYAxisMotor = false;
        public bool EnableOutputTrayTableZAxisMotor = false;

        public bool EnableInputVisionMotor = false;
        public bool EnableS2VisionMotor = false;
        public bool EnableS1VisionMotor = false;
        public bool EnableS3VisionMotor = false;

        public bool EnablePickAndPlace1Module = false;
        public bool EnablePickAndPlace2Module = false;
        #endregion Driver

        #region Vision
        public bool EnableInputVisionModule = false;
        public bool EnableS2VisionModule = false;
        public bool EnableBottomVisionModule = false;
        public bool EnableS1VisionModule = false;
        public bool EnableS3VisionModule = false;
        public bool EnableOutputVisionModule = false;

        #endregion Vision

        #region Misc
        public bool EnableSecsgem = false;
        public bool EnableBarcodeReader = false;
        public bool EnableCognexBarcodeReader = false;
        public bool EnableZebexScanner = false;
        public bool EnableVisionCamera = false;
        #region BarcodeReaderKeyence
        public bool EnableKeyenceBarcodeReader = false;
        public int KeyenceBarcodeReaderCommunicationInterface;
        #endregion
        public bool EnableAutoInputLoading = true;
        public bool EnableAutoOutputLoading = true;
        #endregion Misc
    }
}
