using System;
using System.Collections.Generic;
using System.Text;
using Machine.Platform;
using Product;

namespace Customer
{
    //AlarmID
    //X0    : Failure
    //00    : Software Sequence Alarm
    //10    : Maintenance/Engineering Alarm
    //20    : Operation Alarm

    //X1    : Assist
    //01    : Software Sequence Alarm
    //11    : Maintenance/Engineering Alarm
    //21    : Operation Assist

    //X3    : Message
    //23    : Operation Message

    [Serializable]
    public class CustomerAlarmList : Product.ProductAlarmList
    {
        #region 010000 Application
        
        #region 070000 Input XY Table
#if InputXYTable
        //public Alarm Alarm070001 = new Alarm
        //{
        //    AlarmID = 70001,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Vision Output Path Not Exist",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};

        //public Alarm Alarm070002 = new Alarm
        //{
        //    AlarmID = 70002,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Vision Output Tray Folder Not Exist",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};

        //public Alarm Alarm070003 = new Alarm
        //{
        //    AlarmID = 70003,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Report Tray Number Out Of 150 Limit",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070004 = new Alarm
        //{
        //    AlarmID = 70004,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Vision Output Image Folder Not Exist",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm010005 = new Alarm
        //{
        //    AlarmID = 10005,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Vision Output Image Count Not Tally",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm010006 = new Alarm
        //{
        //    AlarmID = 10006,
        //    AlarmType = 23,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Handler Copy Previous Lot Image Not Complete",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070007 = new Alarm
        //{
        //    AlarmID = 70007,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Fail To Read Input File",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070008 = new Alarm
        //{
        //    AlarmID = 70008,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Input File Not Exist",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070009 = new Alarm
        //{
        //    AlarmID = 70009,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "No Input File In Input Directory",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070010 = new Alarm
        //{
        //    AlarmID = 70010,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Input Directory Not Exist",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070011 = new Alarm
        //{
        //    AlarmID = 70011,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Lot ID Invalid",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070012 = new Alarm
        //{
        //    AlarmID = 70012,
        //    AlarmType = 23,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Vision Inspection Not Complete",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm010013 = new Alarm
        //{
        //    AlarmID = 10013,
        //    AlarmType = 11,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Error During Read Summary File",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        public Alarm Alarm070014 = new Alarm
        {
            AlarmID = 70014,
            AlarmType = 11,
            AlarmImageFile = @"E:\Estek\Database\Alarm Image",
            AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
            AlarmMessageEnglish = "Error During Write Output File",
            AlarmMessageSimplifiedChinese = "警报",
            AlarmMessageTraditionalChinese = "警報",
            AlarmDetailEnglish = "Alarm Message In Detail",
            AlarmDetailSimplifiedChinese = "警报详情",
            AlarmDetailTraditionalChinese = "警報详情",
            AlarmPossibleReasonEnglish = "Possible reason",
            AlarmPossibleReasonSimplifiedChinese = "警报详情",
            AlarmPossibleReasonTraditionalChinese = "警報详情",
            AlarmCorrectiveActionEnglish = "Corrective action",
            AlarmCorrectiveActionSimplifiedChinese = "警报详情",
            AlarmCorrectiveActionTraditionalChinese = "警報详情",
        };

        //public Alarm Alarm070015 = new Alarm
        //{
        //    AlarmID = 70015,
        //    AlarmType = 21,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Input File Not In Correct Format",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm070016 = new Alarm
        //{
        //    AlarmID = 70016,
        //    AlarmType = 11,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Error During Generate Output XML File",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        public Alarm Alarm070017 = new Alarm
        {
            AlarmID = 70017,
            AlarmType = 11,
            AlarmImageFile = @"E:\Estek\Database\Alarm Image",
            AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
            AlarmMessageEnglish = "Error During Generate Output Map File",
            AlarmMessageSimplifiedChinese = "警报",
            AlarmMessageTraditionalChinese = "警報",
            AlarmDetailEnglish = "Alarm Message In Detail",
            AlarmDetailSimplifiedChinese = "警报详情",
            AlarmDetailTraditionalChinese = "警報详情",
            AlarmPossibleReasonEnglish = "Possible reason",
            AlarmPossibleReasonSimplifiedChinese = "警报详情",
            AlarmPossibleReasonTraditionalChinese = "警報详情",
            AlarmCorrectiveActionEnglish = "Corrective action",
            AlarmCorrectiveActionSimplifiedChinese = "警报详情",
            AlarmCorrectiveActionTraditionalChinese = "警報详情",
        };
        //public Alarm Alarm070018 = new Alarm
        //{
        //    AlarmID = 70018,
        //    AlarmType = 11,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Error During Generate Rejected Map File",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm010021 = new Alarm
        //{
        //    AlarmID = 10021,
        //    AlarmType = 11,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Some Of the Vision Files is missing",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};
        //public Alarm Alarm010029 = new Alarm
        //{
        //    AlarmID = 10029,
        //    AlarmType = 11,
        //    AlarmImageFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmImageDetailFile = @"E:\Estek\Database\Alarm Image",
        //    AlarmMessageEnglish = "Error During Create Shortcut Folder To Vision Image",
        //    AlarmMessageSimplifiedChinese = "警报",
        //    AlarmMessageTraditionalChinese = "警報",
        //    AlarmDetailEnglish = "Alarm Message In Detail",
        //    AlarmDetailSimplifiedChinese = "警报详情",
        //    AlarmDetailTraditionalChinese = "警報详情",
        //    AlarmPossibleReasonEnglish = "Possible reason",
        //    AlarmPossibleReasonSimplifiedChinese = "警报详情",
        //    AlarmPossibleReasonTraditionalChinese = "警報详情",
        //    AlarmCorrectiveActionEnglish = "Corrective action",
        //    AlarmCorrectiveActionSimplifiedChinese = "警报详情",
        //    AlarmCorrectiveActionTraditionalChinese = "警報详情",
        //};


#endif
        #endregion

      
        #endregion 007000 Application

    }
}
