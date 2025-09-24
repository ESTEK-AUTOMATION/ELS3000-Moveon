using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeDelaySettings : Machine.RecipeDelaySettings
    {


        public uint DelayBeforePickupHeadGoingDownForPicking_ms = 0;
        public uint DelayForPickupHeadAtDownPositionBeforeVacuumOn_ms = 0;
        public uint DelayForPickupHeadAtDownPositionWithVacuumOn_ms = 0;
        public uint DelayForPickupHeadAtSoftlandingPositionForPicking_ms = 0;

        public uint DelayBeforePickupHeadGoingDownForPickingOutput_ms = 0;
        public uint DelayForPickupHeadAtDownPositionBeforeVacuumOnOutput_ms = 0;
        public uint DelayForPickupHeadAtDownPositionWithVacuumOnOutput_ms = 0;
        public uint DelayForPickupHeadAtSoftlandingPositionForPickingOutput_ms = 0;

        public uint DelayBeforePickupHeadGoingDownForPlacement_ms = 0;
        public uint DelayForPickupHeadAtDownPositionBeforePurging_ms = 0;
        public uint DelayForPickupHeadAtDownPositionWithPurging_ms = 0;
        public uint DelayForPickupHeadAtDownPositionAfterPurging_ms = 0;
        public uint DelayForPickupHeadAtSoftlandingPositionForPlacement_ms = 0;
        
        public uint DelayForPickupHeadMoveUpAfterPicking_ms = 0;

	    public uint DelayBeforeInputVisionSnap_ms = 0;
        public uint DelayBeforeBottomVisionSnap_ms = 0;
        public uint DelayBeforeSetupVisionSnap_ms = 0;
        public uint DelayBeforeS1VisionSnap_ms = 0;
        public uint DelayAfterS1VisionGrabDone_ms = 0;
        public uint DelayBeforeS2VisionSnap_ms = 0;        
        public uint DelayBeforeS3VisionSnap_ms = 0;
        public uint DelayAfterS2S3VisionGrabDone_ms = 0;
        public uint DelayBeforeOutputVisionSnap_ms = 0;
        public uint DelayAfterOutputVisionSnap_ms = 0;

        //public uint DelayForCheckingDiffuserActuatorOnOffCompletelyBeforeNextStep_ms = 0;
        public uint DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms = 0;
        public uint DelayForCheckingOutputTableVacuumOnOffCompletelyBeforeNextStep_ms = 0;
        public uint DelayForPickupHeadAtDownPositionWithPurgingAtInput_ms = 0;

        public uint DelayForPickupHeadPurgeAtDownPositionWhenPickFail = 0;

        public uint DelayForCheckingInputOutputTableZAtSecondSigulationTrayAvalable_ms = 0;
    }
}
