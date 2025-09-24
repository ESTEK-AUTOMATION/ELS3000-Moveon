using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public class CustomerShareVariables : Product.ProductShareVariables
    {
        public CustomerReportEvent customerReportEvent = new CustomerReportEvent();

        public CustomerOptionSettings customerOptionSettings = new CustomerOptionSettings();
        public CustomerConfigurationSettings customerConfigurationSettings = new CustomerConfigurationSettings();

        public CustomerRecipeMainSettings customerRecipeMainSettings = new CustomerRecipeMainSettings();
        public CustomerRecipeInputSettings customerRecipeInputSettings = new CustomerRecipeInputSettings();
        public CustomerRecipeOutputSettings customerRecipeOutputSettings = new CustomerRecipeOutputSettings();
        public CustomerRecipeDelaySettings customerRecipeDelaySettings = new CustomerRecipeDelaySettings();
        public CustomerRecipeMotorPositionSettings customerRecipeMotorPositionSettings = new CustomerRecipeMotorPositionSettings();
        public CustomerRecipeOutputFileSettings customerRecipeOutputFileSettings = new CustomerRecipeOutputFileSettings();

        public CustomerRecipeCassetteSettings customerRecipeInputCassetteSettings = new CustomerRecipeCassetteSettings();
        public CustomerRecipeCassetteSettings customerRecipeOutputCassetteSettings = new CustomerRecipeCassetteSettings();
        public CustomerRecipeVisionSettings customerRecipeVisionSettings = new CustomerRecipeVisionSettings();
        public CustomerRecipeSortingSettings customerRecipeSortingSettings = new CustomerRecipeSortingSettings();
        public CustomerRecipePickUpHeadSetting customerRecipePickUpHeadSettings = new CustomerRecipePickUpHeadSetting();
    }
}
