using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    [Serializable]
    public class ProductRecipeVisionSettings
    {
        public uint InputVisionInspectionCountInCol;
        public uint InputVisionInspectionCountInRow;
        
        public uint S2VisionInspectionCountInCol;
        public uint S2VisionInspectionCountInRow;

        public uint SetupVisionInspectionCountInCol;
        public uint SetupVisionInspectionCountInRow;

        public uint BTMVisionInspectionCountInCol;
        public uint BTMVisionInspectionCountInRow;
        
        public uint SWLVisionInspectionCountInCol;
        public uint SWLVisionInspectionCountInRow;

        public uint SWRightVisionInspectionCountInCol;
        public uint SWRightVisionInspectionCountInRow;

        public uint SWFVisionInspectionCountInCol;
        public uint SWFVisionInspectionCountInRow;

        public uint SWRearVisionInspectionCountInCol;
        public uint SWRearVisionInspectionCountInRow;

        public uint S3VisionInspectionCountInCol;
        public uint S3VisionInspectionCountInRow;

        public uint OutputVisionInspectionCountInCol;
        public uint OutputVisionInspectionCountInRow;

        public bool EnableTeachUnitAtVision = false;

        public bool EnableVision = false;
        public bool EnableInputVision = false;
        public bool EnableSetupVision = false;
        public bool EnableS1Vision = false;
        public bool EnableS2Vision = false;
        public bool EnableBottomVision = false;
        public bool EnableS3Vision = false;
        public bool EnableOutputVision = false;
        
        public int InputVisionRetryCountAfterFail = 0;
        public int S2VisionRetryCountAfterFail = 0;
        public int S2FacetVisionRetryCountAfterFail = 0;
        public int S1VisionRetryCountAfterFail = 0;
        public int BottomVisionRetryCountAfterFail = 0;
        public int SetupVisionRetryCountAfterFail = 0;
        public int S3VisionRetryCountAfterFail = 0;
        public int S3FacetVisionRetryCountAfterFail = 0;
        public int OutputVisionRetryCountAfterFail = 0;

        public int InputVisionContinuousFailCountToTriggerAlarm = 0;
        public int S2VisionContinuousFailCountToTriggerAlarm = 0;
        public int S2FacetVisionContinuousFailCountToTriggerAlarm = 0;
        public int S1VisionContinuousFailCountToTriggerAlarm = 0;
        public int BottomVisionContinuousFailCountToTriggerAlarm = 0;
        public int SetupVisionContinuousFailCountToTriggerAlarm = 0;
        public int S3VisionContinuousFailCountToTriggerAlarm = 0;
        public int S3FacetVisionContinuousFailCountToTriggerAlarm = 0;
        public int OutputVisionContinuousFailCountToTriggerAlarm = 0;

        public List<VisionSnapInfo> listInputVisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listS2VisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listS2FacetVisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listS1VisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listBottomVisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listS3VisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listS3FacetVisionSnap = new List<VisionSnapInfo>();
        public List<VisionSnapInfo> listOutputVisionSnap = new List<VisionSnapInfo>();

        public int InputVisionUnitThetaOffset = 0;
        public int S2VisionUnitThetaOffset = 0;
        public int S2FacetVisionUnitThetaOffset = 0;
        public int S1VisionUnitThetaOffset = 0;
        public int SetupVisionUnitThetaOffset = 0;
        public int BottomVisionUnitThetaOffset = 0;
        public int S3VisionUnitThetaOffset = 0;
        public int S3FacetVisionUnitThetaOffset = 0;
        public int OutputVisionUnitThetaOffset = 0;

        public bool EnableOutputVision2ndPostAlign = false;
        public bool EnableS2S3BothSnapping = false;
    }
}
