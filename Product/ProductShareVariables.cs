using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MoveonMESAPI;

namespace Product
{
    public class ProductShareVariables: Machine.ShareVariables
    {
        //public Machine.ShareVariables shareVariables = new Machine.ShareVariables();
        public ProductOptionSettings productOptionSettings = new ProductOptionSettings();
        public ProductConfigurationSettings productConfigurationSettings = new ProductConfigurationSettings();
        public ProductReportEvent productReportEvent = new ProductReportEvent();

        public ProductRecipeMainSettings productRecipeMainSettings = new ProductRecipeMainSettings();
        public ProductRecipeInputSettings productRecipeInputSettings = new ProductRecipeInputSettings();
        public ProductRecipeOutputSettings productRecipeOutputSettings = new ProductRecipeOutputSettings();
        public ProductRecipeDelaySettings productRecipeDelaySettings = new ProductRecipeDelaySettings();
        public ProductRecipeMotorPositionSettings productRecipeMotorPositionSettings = new ProductRecipeMotorPositionSettings();
        public ProductRecipeOutputFileSettings productRecipeOutputFileSettings = new ProductRecipeOutputFileSettings();

        public ProductPreviousLotInfo productPreviousLotInfoSettings = new ProductPreviousLotInfo();

        public ProductContinueLotInfo productContinueLotInfo = new ProductContinueLotInfo();

        public int nUnitNo;

        public int nNoOfBarcode;
        public int nCurrentUnitNo;
        public int nCurrentPHUnitNo;
        public int nCurrentOutputUnitNo;
        public int nNewValueForXML;
        public int nNoOfXMLForOutput;
        public int nNoOfXMLForSorting;
        public int nNoOfXMLForReject;
        public int nNewValueForXML2;
        public int nCurrentPickUpHeadNo;
        public Input_Product_Info strucInputProductInfo;
        public int nBarcodeIDNo;
        public int InputTrayNo;
        public string strCurrentBarcodeID;
        public string[] ArrayCurrentBarcodeID;
        public DateTime dtProductionEndTime;
        public List<TempBarcodeAdd> lstBarcode = new List<TempBarcodeAdd>();
        public List<TempBarcodeAdd> lstBarcodeForSorting = new List<TempBarcodeAdd>();
        public List<TempBarcodeAdd> lstBarcodeForReject = new List<TempBarcodeAdd>();
        public string strRawBarcodeID;
        public Image imgLastReadBarcode;
        public string strCurrentBarcodeID2;
        public string strRawBarcodeID2;
        public Image imgLastReadBarcode2;
        public int nTotalImages = 0;
        public int nUnitHasBeenPlaceOnOutput = 0;

        public bool bFormProductionProductButtonLotEnable = false;
        public bool bFormProductionProductRecipeEnable = false;
        public bool bFormProductionProductEndLotEnable = false;

        public bool bFormProductionCycle = false;
        public bool bFormProductionCycleEnable = false;
        public bool bFormPusherControlEnable = false;
        public bool bFormLightingCalibrationEnable = false;
        public bool bFormCalibrationEnable = false;
        public bool bFormOutputMotionMoveEnable = false;

        public string strRecipeVisionPath = "..\\Recipe\\Vision\\";
        public string strRecipeFiducialPath = "..\\Recipe\\Fiducial\\";
        public string strRecipeSortingPath = "..\\Recipe\\Sorting\\";
        public string strRecipePickUpHeadPath = "..\\Recipe\\PickUpHead\\";

        public ProductRecipeCassetteSettings productRecipeInputCassetteSettings = new ProductRecipeCassetteSettings();
        public ProductRecipeCassetteSettings productRecipeOutputCassetteSettings = new ProductRecipeCassetteSettings();
        public ProductRecipeVisionSettings productRecipeVisionSettings = new ProductRecipeVisionSettings();
        public ProductRecipeSortingSetting productRecipeSortingSettings = new ProductRecipeSortingSetting();
        public ProductRecipePickUpHeadSeting productRecipePickUpHeadSettings = new ProductRecipePickUpHeadSeting();

        public Equipment.BarcodeReader barcodeReader;
        public Equipment.BarcodeDevice barcodeDevice = new Equipment.BarcodeDevice();
        public Equipment.BarcodeReader barcodeReader2;
        public Equipment.BarcodeDevice barcodeDevice2 = new Equipment.BarcodeDevice();
        public int Barcode2DMode = 1;
        
        public BinInfo mappingInfo = new BinInfo();
        public BinInfo mappingInfo2 = new BinInfo();
        public BinInfo[] MultipleMappingInfo = new BinInfo[150];
        public UnitInfo[] TempPUHResortUnitInfo = new UnitInfo[6];
        public Dictionary<string, UnitInfo> dicMappingInfo = new Dictionary<string, UnitInfo>();
        public Dictionary<string, UnitInfo> dicMappingInfoForSorting = new Dictionary<string, UnitInfo>();
        public Dictionary<string, UnitInfo> dicMappingInfoForReject = new Dictionary<string, UnitInfo>();
        public List<ResortInfo> ResortList = new List<ResortInfo>();

        public BinInfo TempInfo = new BinInfo();
        public TileIdInfo tileIDInfo = new TileIdInfo();
        public MapData mappingData = new MapData();
        public MapData mappingDataOut = new MapData();
        public List<string> mappingUnitIdList = new List<string>();
        public List<LookUpTableOffsetData> PnPCalibrationInfo;
        public List<int> ListTrayToWriteReport = new List<int>();

        public string[] S2FacetVisionResult = new string[10];
        public string[] S3FacetVisionResult = new string[10];


        #region Lumiled AVI Machine
        public struct PPInfo
        {
            public string ServerPath;
            public string FileNameInfo;
            public string SourceFilePath;
            public string SourceFile;
            public string DestinationFilePath;
            public string DestinationFile;
        }
        public OutputFileOption outputFileOption = new OutputFileOption();
        public PPInfo PPInfoToGenerate = new PPInfo();
        #endregion Lumiled AVI Machine
        

        #region VisionVariable
        public int TeachItem = 0;
        public int FiducialNo = 0;
        public string RemoveRC;
        public string lastRecipeSendToVision;
        public bool bolFirstTimeCreateOutputFile;
        public bool bolFirstTimeCreateOutputFileAfterCombineLot;
        #endregion

        public bool IsStateDisplayChanged = false;

        public bool IsSendVisionInfo = false;

        //public bool IsNewLotDone = false;

       	public int nLotIDNumber = 0;
        public List<string> listLotID = new List<string>();
        public int[] InputLotQuantity = new int[3] {0,0,0};
        public int[] InputLotTrayQuantity = new int[3] {0,0,0};
        public string OutputLotID;

        public int nInputTrayNo = 0;

        public bool bFormProductionReview = false;
        public bool bFormProductionReviewEnable = false;
		public struct DetailInfo
        {
            public int TotalQuantity;
            public int TotalDefect;
            public int TotalOutput;
            public DateTime TimeTaken;
        }

        public List<DetailInfo> LotDetail = new List<DetailInfo>();

        public int ShiftATotalQuantity = 0;
        public int ShiftATotalDefect = 0;
        public int ShiftATotalOutput = 0;
        
        public int ShiftBTotalQuantity = 0;
        public int ShiftBTotalDefect = 0;
        public int ShiftBTotalOutput = 0;

        public int ShiftCTotalQuantity = 0;
        public int ShiftCTotalDefect = 0;
        public int ShiftCTotalOutput = 0;

        public int OverallTotalQuantity = 0;
        public int OverallTotalDefect = 0;
        public int OverallTotalOutput = 0;

        public int TotalInputQuantityByTray = 0;
        public int TotalDefectQuantityByTray = 0;
        public int TotalOutputQuantityByTray = 0;

        public string PartName;
        public string PartNumber;
        public string BuildName;

        public string PreviousReportLotID;
        public int PreviousReportTrayNo;

        public bool bEnableContinueLot = false;

        public string UPH;
        public string Throughput;

        public double nCurrentLotMTBF;
        public double nCurrentLotMTBA;
        public int nNoOfAssistSinceCurrentLotStart;
        public int nNoOfFailureSinceCurrentLotStart;

        public string TemporaryNumber;
        public bool PrintTemporary = false;
        //public DateTime dtAlarmStartTime;
        //public TimeSpan dtPreviousAlarmDownTime;
        //public bool bAlarmStart = false;
        //public bool bAlarmRepeat = false;
        public List<string> RecipeListForCalibration = new List<string>();

        public class DefectCounter
        {
            public string DefectCode;
            public int Counter;
        }
        public List<DefectCounter> DefectCounterInput = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterSetup = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterS2 = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterS1 = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterSWL = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterSWR = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterSWF = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterSWREAR = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterS3 = new List<DefectCounter>();
        public List<DefectCounter> DefectCounterOutput = new List<DefectCounter>();

        //public List<string> PreviousInputDefectCode = new List<string>();
        //public List<string> PreviousS2DefectCode = new List<string>();
        //public List<string> PreviousS1DefectCode = new List<string>();
        //public List<string> PreviousSWLDefectCode = new List<string>();
        //public List<string> PreviousSWRDefectCode = new List<string>();
        //public List<string> PreviousSWFDefectCode = new List<string>();
        //public List<string> PreviousSWREARDefectCode = new List<string>();
        //public List<string> PreviousS3DefectCode = new List<string>();
        //public List<string> PreviousOutputDefectCode = new List<string>();

        public List<string> CurrentInputDefectCode = new List<string>();
        public List<string> CurrentSetupDefectCode = new List<string>();
        public List<string> CurrentS2DefectCode = new List<string>();
        public List<string> CurrentS1DefectCode = new List<string>();
        public List<string> CurrentSWLDefectCode = new List<string>();
        public List<string> CurrentSWRDefectCode = new List<string>();
        public List<string> CurrentSWFDefectCode = new List<string>();
        public List<string> CurrentSWREARDefectCode = new List<string>();
        public List<string> CurrentS3DefectCode = new List<string>();
        public List<string> CurrentOutputDefectCode = new List<string>();

        public int LastInputSnapNo = 0;
        public int LastSetupSnapNo = 0;
        public int LastS2SnapNo = 0;
        public int LastS2FacetSnapNo = 0;
        public int LastS1SnapNo = 0;
        public int LastS3SnapNo = 0;
        public int LastS3FacetSnapNo = 0;
        public int LastOutputSnapNo = 0;

        public bool UserChooseContinueLot = false;

        public string strSaveLotInfoPath = "D:\\Estek\\Database\\";

        public List<Lot> TotalLotMES = new List<Lot>();

        public Lot CurrentLotMES = new Lot();
        public List<Defective> CurrentDefectCounterMES = new List<Defective>();

        public List<string> RecoverlistLotID = new List<string>();
        public int[] RecoverInputLotQuantity = new int[3] { 0, 0, 0 };
        public int[] RecoverInputLotTrayQuantity = new int[3] { 0, 0, 0 };
        public string RecoverLotIDOutput;
        public string LastLotIDOutput = "";
        public double LotYield = 0;

        public List<DownTime> CurrentDownTimeCounterMES = new List<DownTime>();
        public int CurrentDownTimeNo;
        public int AlarmStart = 0;

        public string LotIDForTrayRemainedOnInputTrayTable;
		public string OutputTrayIDFromBarcode;

        public string MESInputURLUsed;
        public string MESOutputURLUsed;
        public string MESEndJobURLUsed;

        public bool IsPassYieldMES = false;
        public bool IsPassYieldMESDone = false;

        public List<string> PrintOutputTrayID = new List<string>();
        public string[] Barcode2DID = new string[2];
        public DateTime dtLotStartTime;

        public string MachineVersion;
    }
}
