using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Product
{
    public class ProductProcessEvent : Machine.ProcessEvent
    {
        public AutoResetEvent GUI_PCS_ConnectCommunication = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_DisconnectCommunication = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_CommunicationSend = new AutoResetEvent(false);

        public AutoResetEvent GUI_PCS_NewLotDone = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_NewLotCancel = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_ShowBarcodeRetry = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_StartVerifyBarcode = new AutoResetEvent(false);
        public ManualResetEvent PCS_PCS_VerifyBarcodeDone = new ManualResetEvent(true);
        public AutoResetEvent PCS_GUI_ShowBarcode2Retry = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_StartVerifyBarcode2 = new AutoResetEvent(false);
        public ManualResetEvent PCS_PCS_VerifyBarcode2Done = new ManualResetEvent(true);

        public AutoResetEvent PCS_PCS_StartReadInputFile = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_ReadInputFileDone = new AutoResetEvent(false);

        public ManualResetEvent GUI_PCS_WaitingBarcodeConfirmation = new ManualResetEvent(false);
        public ManualResetEvent GUI_PCS_WaitingBarcodeConfirmation2 = new ManualResetEvent(false);

        public AutoResetEvent GUI_PCS_ConnectBarcodeReader = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_TriggerBarcodeReader = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_ConnectBarcodeReader2 = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_TriggerBarcodeReader2 = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_UpdateRecipe = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_UpdateRecipe2 = new AutoResetEvent(false);

        public AutoResetEvent PCS_PCS_Send_Vision_NewLot = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Resend_Vision_NewLot = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_EndLot = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_Setting = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_Calibration_Setting = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Need_Vision_Setting = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Resend_Vision_Setting = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_TrayNo = new AutoResetEvent(false);

        public AutoResetEvent PCS_PCS_Send_Vision_LotID_BarcodeID = new AutoResetEvent(false);

        public ManualResetEvent PCS_PCS_Send_Write_XML = new ManualResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_EndTile = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_FailUnit = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_OtherSettings = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_InputDefectCode = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_CanisterDefectCode = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_PurgeDefectCode = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Vision_All_Inspection_Complete = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Vision_All_Inspection_Not_Complete = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Live_On = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Live_Off = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Teach = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Launch_Teach_Window = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Close_Teach_Window = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Verify_FD = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Verify_XY = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Set_Vision_Verify_Theta = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_Manual_Mode_On = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_Manual_Mode_Off = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_DryRun_Mode_On = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_DryRun_Mode_Off = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_Send_Vision_Remove_RC = new AutoResetEvent(false);

        public AutoResetEvent PCS_PCS_Send_Vision_Resort_Mode_Off = new AutoResetEvent(false);

        public AutoResetEvent PCS_PCS_Send_Vision_Report = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Initial_Map = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Update_Map = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_EndLot = new AutoResetEvent(false);

        public AutoResetEvent PCS_PCS_Generate_PPLotFile = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_CreateFrameSelection = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_SelectSlotToRun = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_UpdateInputBarcodeID = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_UpdateOutputBarcodeID = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_EnableFrameSelection = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_ClearCheckBoxText = new AutoResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_INPSetting = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_BTMSetting = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_PREOUTSetting = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_POSTOUTSetting = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_PRESORTSetting = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_POSTSORTSetting = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_Setting = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_BTMRCDirection = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_OUTRCDirection = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_FOVUnitInfo = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_InputDefectCode = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_CanisterDefectCode = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_PurgeDefectCode = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_INPTRAY = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_OUTTRAY = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_SORTTRAY = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_REJECTTRAY = new ManualResetEvent(false);
        
        public ManualResetEvent PCS_PCS_Vision_Receive_AllStationDefectCode = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_AllStationDefectCode_Error = new ManualResetEvent(false);
        public AutoResetEvent PCS_PCS_Start_Map_Drive = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Launch_NewLot = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_Need_NewLot = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Close_Remaining_Active_Form = new AutoResetEvent(false);

        public AutoResetEvent GUI_PCS_Force_Generate_Last_Data = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Last_Data_Generated = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Copy_Task_Complete = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Writing_File_Start = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Error_Copy_Task = new AutoResetEvent(false);

        public ManualResetEvent PCS_GUI_Clean_Count_Alarm = new ManualResetEvent(false);
        public ManualResetEvent PCS_GUI_Warning_Count_Alarm = new ManualResetEvent(false);
        public ManualResetEvent PCS_GUI_Due_Count_Alarm = new ManualResetEvent(false);

        public ManualResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_Choose = new ManualResetEvent(false);
        public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_Initialize = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_NAK = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_Snap = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Snap_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Snap_NAK = new AutoResetEvent(false);
        //public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_Rotate_30_Start = new AutoResetEvent(false);
        //public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Rotate_30_Done = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_Snap2 = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Snap2_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_Snap2_NAK = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_GetResult = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_GetResult_Done = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_GetResult_Fail = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_Aligner_Center_Rotation_Teach_End = new AutoResetEvent(false);
        //public AutoResetEvent PCS_GUI_Aligner_Center_Rotation_Teach_End_ACK = new AutoResetEvent(false);

        public ManualResetEvent GUI_PCS_IPT_Center_Rotation_Teach_Choose = new ManualResetEvent(false);
        public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_Initialize = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Initialize_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Initialize_NAK = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_Snap = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Snap_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Snap_NAK = new AutoResetEvent(false);
        //public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_Rotate_30_Start = new AutoResetEvent(false);
        //public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Rotate_30_Done = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_Snap2 = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Snap2_ACK = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_Snap2_NAK = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_GetResult = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_GetResult_Done = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_GetResult_Fail = new AutoResetEvent(false);
        public AutoResetEvent GUI_PCS_IPT_Center_Rotation_Teach_End = new AutoResetEvent(false);
        //public AutoResetEvent PCS_GUI_IPT_Center_Rotation_Teach_End_ACK = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Update_Summary_Report = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Update_Yeild_Pareto = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_Update_Summary_Report_EndLot = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Update_Yeild_Pareto_EndLot = new AutoResetEvent(false);
        public AutoResetEvent PCS_GUI_Update_Alarm_Pareto_EndLot = new AutoResetEvent(false);

        public ManualResetEvent PCS_PCS_Is_Sending_Vision_Setting = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Start_Write_OutputFile_Only = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Start_Write_SortingFile_Only = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Start_Write_RejectFile_Only = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Start_Write_AllFile = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Start_Motion_Controller = new ManualResetEvent(false);
        public AutoResetEvent PCS_PCS_Motion_Controller_Ready = new AutoResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_INP = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_OUT = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_REJECT = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_SW1 = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_SW2 = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_SW3 = new ManualResetEvent(false);
        public ManualResetEvent PCS_PCS_Vision_Receive_NewLot_BTM = new ManualResetEvent(false);
      

        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_INP = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_OUT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_REJECT1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_REJECT2 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_REJECT3 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_REJECT4 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_REJECT5 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_S1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_S2 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_S3 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SWLEFT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SWRIGHT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SWFRONT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SWREAR = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SETUP = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_OUT1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_RCDirection_SAMPLING1 = new ManualResetEvent(false);


        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_INP = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_OUT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_REJECT1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_REJECT2 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_REJECT3 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_REJECT4 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_REJECT5 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_S1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_S2 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_S3 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SWLEFT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SWRIGHT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SWFRONT = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SWREAR = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SETUP = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_OUT1 = new ManualResetEvent(false);
        //public ManualResetEvent PCS_PCS_Vision_Receive_MoveDirection_SAMPLING1 = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_Info_Fail = new ManualResetEvent(false);

        public ManualResetEvent PCS_PCS_Current_Lot_Is_Running = new ManualResetEvent(false);

        public AutoResetEvent PCS_PCS_Set_Mapping_Result = new AutoResetEvent(false);

        public ManualResetEvent GUI_PCS_NewLotDone2 = new ManualResetEvent(false);

        public ManualResetEvent GUI_GUI_GET_DATA = new ManualResetEvent(false);

        public AutoResetEvent PCS_PCS_GET_INPUT_DATA_MES = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_SEND_OUTPUT_DATA_MES = new AutoResetEvent(false);
        public AutoResetEvent PCS_PCS_SEND_ENDJOB_DATA_MES = new AutoResetEvent(false);

        public AutoResetEvent PCS_GUI_GET_MACHINE_VERSION = new AutoResetEvent(false);

        public ManualResetEvent PCS_PCS_Vision_Receive_AvailableDiskSpaceLow = new ManualResetEvent(false);
    }
}
