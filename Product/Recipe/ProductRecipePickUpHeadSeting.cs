using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
   public class ProductRecipePickUpHeadSeting
   {
        public bool EnablePickSoftlanding;
        public bool EnablePlaceSoftlanding;
        public double PickUpHead1Force;
        public double PickUpHead1Pressure;
        public double PickUpHead1FlowRate;
              
        public double PickUpHead2Force;
        public double PickUpHead2Pressure;
        public double PickUpHead2FlowRate;
               
        public double PickUpHead1PlaceForce;
        public double PickUpHead1PlacePressure;
        public double PickUpHead1PlaceFlowRate;

        public double PickUpHead2PlaceForce;
        public double PickUpHead2PlacePressure;
        public double PickUpHead2PlaceFlowRate;

        public bool EnableSafetyPnPMovePickStation;
    }
}
