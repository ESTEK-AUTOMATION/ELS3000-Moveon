using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeMainSettings : Machine.RecipeMainSettings
    {
        //public string InputCassetteRecipeName = "";
        //public string OutputCassetteRecipeName = "";
        public string VisionRecipeName = "";
        public string InspectionRecipeName = "";
        public string SortingRecipeName = "";
        public string PickUpHeadRecipeName = "";
    }
}
