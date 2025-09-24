using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public struct StationResult
    {
        //public int UnitPresent;
        //public int InputResult;
        //public int MarkingVisionResult;
        //public int RotaryAndAlignerAResult;
        //public int TestSite1Result;
        //public int TestSite2Result;
        //public int TestSite3Result;
        //public int TestSite4Result;
        //public int RotaryAndAlignerBResult;
        //public int FiveSResult;
        //public int SortingResult;
        //public int TapeAndReelResult;
        //public int InPocketResult;
        //public int PostSealResult;

        public int UnitNo;
        public int UnitPresent;
        public int TopVisionResult;
        public int InputResult;
        public int AlignerResult;
        public int AlignerXOffset_um;
        public int AlignerYOffset_um;
        public int AlignerThetaOffset_mDegree;
        public int BottomResult;
        public int BottomXOffset_um;
        public int BottomYOffset_um;
        public int BottomThetaOffset_mDegree;
        public int SidewallLeftRightResult;
        public int SidewallFrontRearResult;
        public int InputColumn;
        public int InputRow;
        public int InputTrayNo;

    }
}
