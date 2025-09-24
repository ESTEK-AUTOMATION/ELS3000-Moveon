using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeOutputFileSettings : Machine.RecipeOutputFileSettings
    {
        public List<DefectProperty> listDefect = new List<DefectProperty>();
       
        public string strOutputFilePath;
        public string strOutputLocalFilePath;

        public string EmptyUnitColorInHex;
        public string UnitSlantedColorInHex;
    }
}
