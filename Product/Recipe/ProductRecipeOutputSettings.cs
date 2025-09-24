using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeOutputSettings : Machine.RecipeOutputSettings
    {
        public uint NoOfDeviceInColOutput;
        public uint NoOfDeviceInRowOutput;
        public uint DeviceXPitchOutput;
        public uint DeviceYPitchOutput;
        public uint OutputPocketDepth_um;
        public uint OutputTrayThickness;
        public uint LowYieldAlarm;
        public double YieldPercentage;

        public int TableXOffsetOutput;
        public int TableYOffsetOutput;
        public int UnitPlacementRotationOffsetOutput;

        public uint InputOutputDefectRejectTray = 0;
        public uint SetupDefectRejectTray = 0;
        public uint S2DefectRejectTray = 0;
        public uint S1DefectRejectTray = 0;
        public uint SidewallLeftDefectRejectTray = 0;
        public uint SidewallRightDefectRejectTray = 0;
        public uint S3DefectRejectTray = 0;
        public uint SidewallFrontDefectRejectTray = 0;
        public uint SidewallRearDefectRejectTray = 0;

        public uint BarcodeLabelSize = 0;

        //public bool EnableGoodSamplingSequence = false;
        //public bool EnableScanBarcodeOnOutputTray = false;

        public bool EnableCheckContinuousDefectCode = false;
        public uint CheckContinuousDefectCode = 0;
        public bool EnableOutputTableVacuum;

        public List<SortTrayInfo> listSortTrayInfo = new List<SortTrayInfo>();
    }
}
