using System;
using System.Collections.Generic;
using System.Text;

namespace Product
{
    public struct Coordinate
    {
        public int Row;
        public int Column;
    }
    public struct InputAndOutputCoordinate
    {
        public int IRow;
        public int ICol;
        public int ORow;
        public int OCol;
    }
    public struct ResortInfo
    {
        public int InputRow;
        public int InputColumn;
        public int OutputOriRow;
        public int OutputOriColumn;
        public int OutputNewRow;
        public int OutputNewColumn;
        public int InputTrayNo;
        public int OutputTrayNo;
    }
    public struct LookUpTableOffsetData
    {
        public double Angle;
        public double XOffset;
        public double YOffset;
    }
    public struct UnitInfo
    {
        public int InputRow;
        public int InputColumn;
        public string UnitID;
        public string BinCode;
        public int FlipperHeadNo;
        public int BondHeadNo;
        public int CellNumber;
        public int InputSequenceNumber;
        public int OutputSequenceNumber;
        //First Position 0 = Top Left, 1 = Top Right, 2 = Bottom Left, 3 = Bottom Right
        public int First_Pos;
        public string InputTrayType;
        public string OutputTrayType;
        public int InputTrayNo;
        public int OutputTrayNo;
        public string BarcodeID;
        public string BarcodeID2;
        public int VisionResult;
        public string InputResult;
        public string BottomResult;
        public string S2Result;
        public string S2PartingResult;
        public string S1Result;
        public string SetupResult;
        public string SWLeftResult;
        public string SWRightResult;
        public string S3Result;
        public string S3PartingResult;
        public string SWFrontResult;
        public string SWRearResult;
        public string OutputResult;
        public string RejectResult;
        public int OutputRow;
        public int OutputColumn;
        public string Destination;
        public string OutputTrayID;
    }

    public struct DefectProperty
    {
        public int No;
        public string Code;
        public string Description;
        public bool EnableInReport;
        public string ColorInHex;
        public string Destination;
        public string Pick;
        public DefectProperty Clone()
        {
            DefectProperty cloned = new DefectProperty();
            cloned.No = this.No;
            cloned.Code = this.Code;
            cloned.Description = this.Description;
            cloned.EnableInReport = this.EnableInReport;
            cloned.ColorInHex = this.ColorInHex;
            return cloned;
        }
    }
    public struct VisionSnapInfo
    {
        public int SnapNo;
        public string Description;
        public bool EnableSnap;
        public int VisionXAxisOffset;
        public int VisionYAxisOffset;
        public int VisionZAxisOffset;
        public int VisionThetaAxisOffset;
        public bool EnableDiffuser;
    }
    public class DefectQuantity
    {
        public DefectProperty Defect;
        public int DefectQty;

        public DefectQuantity Clone()
        {
            DefectQuantity cloned = new DefectQuantity();
            cloned.Defect = this.Defect.Clone();
            cloned.DefectQty = this.DefectQty;
            return cloned;
        }
    }

    public struct BinInfo
    {
        public int Col_Max;
        public int Row_Max;
        public UnitInfo[] arrayUnitInfo;
        public int InputRejectQty;
        public string OriginalFileData;
        public Dictionary<string, int> dicUnitIndex;
    }
    public struct MultipleBinInfo
    {
        public BinInfo MappingInfo;
        public int NoOfTray;
    }
    public struct Input_Product_Info
    {
        public string LotID;
        public string LotIDOutput;
        public string LotID2;
        public string LotIDOutput2;
        public string WorkOrder;
        public string Recipe;
        public string WaferBin;
        public string PPLot;
        public string OperatorID;
        public string Shift;
        public int Total_Tiles;
        public int Total_Inputs;
        public int Total_Reject;
        public int Total_Output;
        public double Total_PassYield;
        public int Total_Reject_Option;
        public int Total_Output_Option;
        public double Total_PassYield_Option;
        public int Total_MD2_Reject;
        public int Total_LS2_Reject;
        public int Total_LO2_Reject;
        public int Total_CT2_Reject;
        public int InputFileNo;
        public DateTime dtStartDate;
    }
    public struct TileIdInfo
    {
        public string TileId;
        public int InputQty;
        public int MD2;
        public int LS2;
        public int LO2;
        public int CT2;
        public int Underblast;
        public int Overblast;
        public int SawDefect;
        public int DentedMark;
        public int CircleMark;
        public int TotalReject;
        public int OutputQty;
        public int TotalRejectOption;
        public int QutputQtyOption;
        public double TilePassYieldOption;
        public double TilePassYield;
    }

    public struct OutputFileOption
    {
        public int nNumberDefectCodeSelect;
        public bool bAddWaferBinIntoPPLot;
 
        public bool bEnablePnPOutputFile;
        public bool bEnableAdditionalFile;

        public List<DefectProperty> listDefect;
    }

    public struct TempBarcodeAdd
    {
        public int Row;
        public int Col;
        public string UnitID;// { get; set=value; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MapData
    {

        public MapDataLOTINFO LOTINFO;

        public MapDataEQPTINFO EQPTINFO;
        [System.Xml.Serialization.XmlElementAttribute("Overlay")]
        public Overlay[] Overlay;

        /// <remarks/>
        //public MapDataLOTINFO LOTINFODETAIL
        //{
        //    get
        //    {
        //        return this.LOTINFO;
        //    }
        //    set
        //    {
        //        this.LOTINFO = value;
        //    }
        //}

        /// <remarks/>
        //public MapDataEQPTINFO EQPTINFO
        //{
        //    get
        //    {
        //        return this.EQPTINFO;
        //    }
        //    set
        //    {
        //        this.EQPTINFO = value;
        //    }
        //}

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("Overlay")]
        //public MapDataOverlay[] Overlay
        //{
        //    get
        //    {
        //        return this.overlayField;
        //    }
        //    set
        //    {
        //        this.overlayField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MapDataLOTINFO
    {

        //public string deviceField;

        //public string lotIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Device;
        //{
        //    get
        //    {
        //        return this.deviceField;
        //    }
        //    set
        //    {
        //        this.deviceField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LotID;
        //{
        //    get
        //    {
        //        return this.lotIDField;
        //    }
        //    set
        //    {
        //        this.lotIDField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MapDataEQPTINFO
    {

        //public string equipOpnField;

        //public string eqptIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EquipOpn;
        //{
        //    get
        //    {
        //        return this.equipOpnField;
        //    }
        //    set
        //    {
        //        this.equipOpnField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EqptID;
        //{
        //    get
        //    {
        //        return this.eqptIDField;
        //    }
        //    set
        //    {
        //        this.eqptIDField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Overlay
    {

        //public MapDataOverlayBinDef[] BinDef;

        //public MapDataOverlayBindefinition[] Bindefinition;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BinDef")]
        public MapDataOverlayBinDef[] BinDef;
        //{
        //    get
        //    {
        //        return this.BinDef;
        //    }
        //    set
        //    {
        //        this.BinDef = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Bindefinition")]
        public MapDataOverlayBindefinition[] Bindefinition;
        //{
        //    get
        //    {
        //        return this.bindefinitionField;
        //    }
        //    set
        //    {
        //        this.bindefinitionField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MapDataOverlayBinDef
    {

        //public string binDescriptionField;

        //public string binCodeField;

        //public string unitidField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BinDescription;
        //{
        //    get
        //    {
        //        return this.binDescriptionField;
        //    }
        //    set
        //    {
        //        this.binDescriptionField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BinCode;
        //{
        //    get
        //    {
        //        return this.binCodeField;
        //    }
        //    set
        //    {
        //        this.binCodeField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Unitid;
        //{
        //    get
        //    {
        //        return this.unitidField;
        //    }
        //    set
        //    {
        //        this.unitidField = value;
        //    }
        //}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MapDataOverlayBindefinition
    {

        //public bool pickField;

        //public string binDescriptionField;

        //public ushort binCountField;

        //private string binCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Pick;
        //{
        //    get
        //    {
        //        return this.pickField;
        //    }
        //    set
        //    {
        //        this.pickField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BinDescription;
        //{
        //    get
        //    {
        //        return this.binDescriptionField;
        //    }
        //    set
        //    {
        //        this.binDescriptionField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort BinCount;
        //{
        //    get
        //    {
        //        return this.binCountField;
        //    }
        //    set
        //    {
        //        this.binCountField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BinCode;
        //{
        //    get
        //    {
        //        return this.binCodeField;
        //    }
        //    set
        //    {
        //        this.binCodeField = value;
        //    }
        //}
    }

    //public partial class MapDataReinitialize
    //{

    //    public MapDataLOTINFOReInitialize lOTINFOField = new MapDataLOTINFOReInitialize();

    //    public MapDataEQPTINFOReInitialize eQPTINFOField = new MapDataEQPTINFOReInitialize();

    //    public MapDataOverlayReInitialize[] overlayField = new MapDataOverlayReInitialize[2];

        
    //    public MapDataLOTINFOReInitialize LOTINFO = new MapDataLOTINFOReInitialize();


    //    /// <remarks/>
    //    public MapDataEQPTINFOReInitialize EQPTINFO = new MapDataEQPTINFOReInitialize();

    //    public MapDataOverlayReInitialize[] Overlay = new MapDataOverlayReInitialize[2];

    //}
    //public partial class MapDataLOTINFOReInitialize
    //{

    //    public string deviceField = "";

    //    public string lotIDField = "";

    //    public string Device = "";

    //    public string LotID = "";
    //}

   
    //public partial class MapDataEQPTINFOReInitialize
    //{

    //    public string equipOpnField = "";

    //    public string eqptIDField = "";

    //    public string EquipOpn = "";

    //    public string EqptID = "";
       
    //}

    //public class MapDataOverlayReInitialize
    //{

    //    public MapDataOverlayBinDefReInitialize[] binDefField;

    //    public MapDataOverlayBinDefReInitialize[] BinDef;
      
    //}
    
    //public class MapDataOverlayBinDefReInitialize
    //{

    //    public string binDescriptionField = "";

    //    public string binCodeField = "";

    //    public string unitidField = "";

    //    public string BinDescription = "";

    //    public string BinCode = "";

    //    public string Unitid = "";
    //}
    //public partial class MapDataOverlayBindefinitionReInitialize
    //{

    //    public bool pickField = true;

    //    public string binDescriptionField = "";

    //    public ushort binCountField = 1;

    //    private string binCodeField = "";

    //    public bool Pick = false;

    //    public string BinDescription ="";

    //    public ushort BinCount = 0;

    //    public string BinCode = "";
       
    //}
    public struct SortTrayInfo
    {
        public string DefectCode;
        public int RejectTray;
    }
}
