using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeMotorPositionSettings : Machine.RecipeMotorPositionSettings
    {
        public int PickingGap_um = 0;
        public int PickingGapOutput_um = 0;
        public int PlacementGap_um = 0;
        public int Picking1SoftlandingDistance_um = 0;
        public int Picking2SoftlandingDistance_um = 0;
        //public int PickingSoftlandingDistanceOutput_um = 0;
        public int PickingSoftlandingSpeed_percent = 0;
        public int PickingSoftlandingSpeedOutput_percent = 0;
        public int PlacementSoftlandingDistance_um = 0;
        public int PlacementSoftlandingSpeed_percent = 0;
        
        public int PickAndPlaceXAxisOnTheFlyOffsetForSetupVision_um = 0;
    }
}
