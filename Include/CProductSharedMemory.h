#pragma once
#ifndef __CPRODUCTSHAREDMEMORY_H_INCLUDED__ 
#define __CPRODUCTSHAREDMEMORY_H_INCLUDED__

#ifndef __CPLATFORMSHAREDMEMORY_H_INCLUDED__ 
#include "CPlatformSharedMemory.h"
#endif

#include "RProduct.h"

#ifndef NUMBER_IO_CARD
#define NUMBER_IO_CARD 21
#endif

typedef struct StationResult
{
	int UnitNo;
	int UnitPresent;
	int PlacementResult;
	int InputResult;
	int InputXOffset_um;
	int InputYOffset_um;
	int InputThetaOffset_mDegree;
	int InputSleeveXOffset_um;
	int InputSleeveYOffset_um;
	int InputSleeveThetaOffset_mDegree;
	int S2Result;
	int S2PartingResult;
	int S1Result;
	int S1XOffset_um;
	int S1YOffset_um;
	int S1ThetaOffset_mDegree; 
	int S1ZOffset_um;
	int SetupZOffset_um;
	int SetupThetaOffset_mDegree;
	int SetupResult;
	int SetupThicknessResult;
	int BottomResult;
	int BottomXOffset_um;
	int BottomYOffset_um;
	int BottomThetaOffset_mDegree;
	int BottomZOffset_um;
	int SelveXOffset_um;
	int SelveYOffset_um;
	int S3Result;
	int S3PartingResult;
	int OutputResult;
	int OutputXOffset_um;
	int OutputYOffset_um;
	int OutputThetaOffset_mDegree;
	int OutputResult_Post;
	int OutputXOffset_um_Post;
	int OutputYOffset_um_Post;
	int OutputThetaOffset_mDegree_Post;

	int RejectResult;
	int RejectXOffset_um;
	int RejectYOffset_um;
	int RejectThetaOffset_mDegree;
	int RejectResult_Post;
	int RejectXOffset_um_Post;
	int RejectYOffset_um_Post;
	int RejectThetaOffset_mDegree_Post;

	int InputColumn;
	int InputRow;
	int OutputColumn;
	int OutputRow;
	int RejectColumn;
	int RejectRow;

	int InputTrayNo;
	int OutputTrayNo;
	int RejectTrayNo;
	int InputLotID;
	int OutputLotID;
	int CurrentOutputTableNo; //0 = reject tray 5, 1 = reject tray 4, 2 = reject tray 3, 3 = reject tray 2, 4 = reject tray 1, 5 = output tray

	int InputUnitPresent;
	int OutputUnitPresent;
	int RejectUnitPresent;

	int IsLastUnit;
	int IsLastUnitForCurrentLot;
	bool bolLastUnitInTray;

}sStationResult, *PStationResult;

struct LookUpTableOffsetData {
	double Angle;
	double XOffset;
	double YOffset;
};

typedef struct VisionInfo
{
	int No;
	int Enable;// 0 ==false 1 == true
	int FocusOffset_um;
	int ThetaOffset_mDegree;
	int XOffset_um;
	int YOffset_um;
	int Result;
	int EnableDiffuser; // 0 ==false 1 == true

}sVisionInfo, *PVisionInfo;

class VisionModule
{
public:
	static const int Unknown = 0;
	static const int InputVision = 1;
	static const int S2Vision = 2;
	static const int S1Vision = 3;
	static const int S3Vision = 4;
	static const int OutputVision = 5;
};
VisionModule PickAndPlaceCurrentVisionModule;

class PickAndPlaceStation
{
public:
	static const int UnknownStation = 0;
	static const int HomeStation = 1;
	static const int MovingFromHomeStationToBottomStation = 3;
	static const int MovingFromHomeStationToS3Station = 4;
	static const int InputStation = 5;
	static const int MovingFromInputStationToBottomStation = 7;
	static const int MovingFromInputStationToOutputStation = 8;
	static const int BottomStation = 11;
	static const int MovingFromBottomStationToInputStation = 12;
	static const int MovingFromBottomStationToS3Station = 13;
	static const int S3Station = 14;
	static const int MovingFromS3StationToOutputStation = 15;
	static const int OutputStation = 16;
	static const int MovingFromOutputStationToInputStation = 17;
	static const int MovingFromOutputStationToDisableStation = 18;
	static const int DisableStation = 19;
	static const int MovingFromDisableStationToInputStation = 20;
	static const int MovingFromDisableStationToBottomStation = 21;
};
PickAndPlaceStation PickAndPlaceCurrentStation;
class RProduct_API ProductSharedMemoryCustomize : public SharedMemoryCustomize
{
public:
#pragma region Motion Controller
	bool EnableMotionController1;
	bool EnableMotionController2;
	bool EnableMotionController3;
	bool EnableMotionController4;
	bool EnableMotionController5;
	bool EnableMotionController6;
	bool EnableMotionController7;
#pragma endregion

#pragma region Driver

	bool EnablePickAndPlace1XAxisMotor;
	bool EnablePickAndPlace2XAxisMotor;

	bool EnablePickAndPlace1YAxisMotor;
	bool EnableInputTrayTableXAxisMotor;
	bool EnableInputTrayTableYAxisMotor;
	bool EnableInputTrayTableZAxisMotor;

	bool EnablePickAndPlace2YAxisMotor;
	bool EnableOutputTrayTableXAxisMotor;
	bool EnableOutputTrayTableYAxisMotor;
	bool EnableOutputTrayTableZAxisMotor;

	bool EnableInputVisionMotor;
	bool EnableS2VisionMotor;
	bool EnableS1VisionMotor;
	bool EnableS3VisionMotor;

	//bool EnablePickAndPlace1ZAxisMotor;
	//bool EnablePickAndPlace1ThetaAxisMotor;
	//bool EnablePickAndPlace2ZAxisMotor;
	//bool EnablePickAndPlace2ThetaAxisMotor;

	bool EnablePickAndPlace1Module;
	bool EnablePickAndPlace2Module;
#pragma endregion

#pragma region Vision
	bool EnableInputVisionModule;
	bool EnableBottomVisionModule;
	bool EnableS2VisionModule;
	bool EnableS1VisionModule;
	bool EnableS3VisionModule;
	bool EnableOutputVisionModule;
#pragma endregion

};
class RProduct_API ProductSharedMemoryModuleStatus : public SharedMemoryModuleStatus
{
public:

	bool IsPickAndPlace1XAxisMotorHome;
	bool IsPickAndPlace2XAxisMotorHome;

	bool IsPickAndPlace1YAxisMotorHome;
	bool IsInputTrayTableXAxisMotorHome;
	bool IsInputTrayTableYAxisMotorHome;
	bool IsInputTrayTableZAxisMotorHome;

	bool IsPickAndPlace2YAxisMotorHome;
	bool IsOutputTrayTableXAxisMotorHome;
	bool IsOutputTrayTableYAxisMotorHome;
	bool IsOutputTrayTableZAxisMotorHome;

	bool IsInputVisionMotorHome;
	bool IsS2VisionMotorHome;
	bool IsS1VisionMotorHome;
	bool IsS3VisionMotorHome;

	bool IsPickAndPlace1ZAxisMotorHome;
	bool IsPickAndPlace1ThetaAxisMotorHome;
	bool IsPickAndPlace2ZAxisMotorHome;
	bool IsPickAndPlace2ThetaAxisMotorHome;

	//bool IsKingStarConnected;

	bool IsPickAndPlace1XAxisMoving;
	bool IsPickAndPlace1YAxisMoving;
	bool IsPickAndPlace2XAxisMoving;
	bool IsPickAndPlace2YAxisMoving;
};
class RProduct_API ProductSharedMemorySetting : public SharedMemorySetting
{
public:

	VisionInfo InputVision[10];
	VisionInfo S2Vision[10];
	VisionInfo S2FacetVision[10];
	VisionInfo BottomVision[10];
	VisionInfo S1Vision[10];
	VisionInfo S3Vision[10];
	VisionInfo S3FacetVision[10]; //S3FacetVision
	VisionInfo OutputVision[10];

	bool EnablePH[2];

#pragma region Input
	unsigned int NoOfDeviceInColInput;
	unsigned int NoOfDeviceInRowInput;
	unsigned int DeviceXPitchInput;
	unsigned int DeviceYPitchInput;
	unsigned int UnitThickness_um;
	unsigned int InputPocketDepth_um;
	unsigned int InputTrayThickness;

	signed long PickingCenterXOffsetInput;
	signed long PickingCenterYOffsetInput;
	signed long UnitPlacementRotationOffsetInput;

	signed long BottomVisionXOffset;
	signed long BottomVisionYOffset;
	signed long ContinuouslyEmptyPocket;

	unsigned int EmptyUnit;
	bool EnableCheckEmptyUnit;
	bool EnableInputTableVacuum;

	bool EnablePurging;


	bool EnableVisionWaitResult;
#pragma endregion

#pragma region Output
	unsigned int NoOfDeviceInColOutput;
	unsigned int NoOfDeviceInRowOutput;
	unsigned int DeviceXPitchOutput;
	unsigned int DeviceYPitchOutput;
	unsigned int OutputPocketDepth_um;
	unsigned int OutputTrayThickness;
	unsigned int LowYieldAlarm;

	signed long TableXOffsetOutput;
	signed long TableYOffsetOutput;
	signed long UnitPlacementRotationOffsetOutput;	

	unsigned int TotalOutputUnitQuantity;

	unsigned int InputOutputDefectRejectTray;
	unsigned int S2DefectRejectTray;
	unsigned int SetupDefectRejectTray;
	unsigned int S1DefectRejectTray;
	unsigned int SidewallLeftDefectRejectTray;
	unsigned int SidewallRightDefectRejectTray;
	unsigned int S3DefectRejectTray;
	unsigned int SidewallFrontDefectRejectTray;
	unsigned int SidewallRearDefectRejectTray;

	bool EnableOutputTableVacuum;

#pragma endregion

#pragma region Position
	signed long PickingGap_um;
	signed long PickingGap_um_Output;
	signed long PlacementGap_um;
	signed long Picking1SoftlandingDistance_um;
	signed long Picking2SoftlandingDistance_um;
	signed long PickingSoftlandingDistance_um_Output;
	signed long PickingSoftlandingSpeed_percent;
	signed long PickingSoftlandingSpeed_percent_Output;
	signed long PlacementSoftlandingDistance_um;
	signed long PlacementSoftlandingSpeed_percent;
	signed long PickAndPlaceXAxisOnTheFlyOffsetForSetupVision_um;
#pragma endregion

#pragma region Delay
	unsigned int DelayBeforePickupHeadGoingDownForPicking_ms;
	unsigned int DelayForPickupHeadAtDownPositionBeforeVacuumOn_ms;
	unsigned int DelayForPickupHeadAtDownPositionWithVacuumOn_ms;

	unsigned int DelayBeforePickupHeadGoingDownForPicking_ms_Output;
	unsigned int DelayForPickupHeadAtDownPositionBeforeVacuumOn_ms_Output;
	unsigned int DelayForPickupHeadAtDownPositionWithVacuumOn_ms_Output;
	unsigned int DelayForPickupHeadAtSoftlandingPositionForPicking_ms_Output;

	unsigned int DelayForPickupHeadAtSoftlandingPositionForPicking_ms;

	unsigned int DelayBeforePickupHeadGoingDownForPlacement_ms;
	unsigned int DelayForPickupHeadAtDownPositionBeforePurging_ms;
	unsigned int DelayForPickupHeadAtDownPositionWithPurging_ms;
	unsigned int DelayForPickupHeadAtDownPositionAfterPurging_ms;
	unsigned int DelayForPickupHeadMoveUpAfterPicking_ms;

	unsigned int DelayForPickupHeadAtSoftlandingPositionForPlacement_ms;

	unsigned int DelayBeforeInputVisionSnap_ms;
	unsigned int DelayBeforeS2VisionSnap_ms;	
	unsigned int DelayBeforeSetupVisionSnap_ms;
	unsigned int DelayBeforeBottomVisionSnap_ms;
	unsigned int DelayBeforeS3VisionSnap_ms;
	unsigned int DelayAfterS2S3VisionGrabDone_ms;
	unsigned int DelayBeforeS1VisionSnap_ms;
	unsigned int DelayAfterS1VisionGrabDone_ms;
	unsigned int DelayBeforeOutputVisionSnap_ms;
	unsigned int DelayAfterOutputVisionSnap_ms;

	unsigned int DelayForCheckingDiffuserActuatorOnOffCompletelyBeforeNextStep_ms;

	unsigned int DelayForPickupHeadAtDownPositionWithPurgingAtInput_ms;

	unsigned int DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms;
	unsigned int DelayForCheckingOutputTableVacuumOnOffCompletelyBeforeNextStep_ms;
	unsigned int DelayForPickupHeadPurgeAtDownPositionWhenPickFail;
	unsigned int DelayForCheckingInputOutputTableZAtSecondSigulationTrayAvalable_ms;
	
#pragma endregion

#pragma region Pick Up Head
	bool EnablePickSoftlanding;
	double PickUpHead1FlowRate;
	double PickUpHead1Force;
	double PickUpHead1Pressure;
	double PickUpHead2FlowRate;
	double PickUpHead2Force;
	double PickUpHead2Pressure;

	bool EnablePlaceSoftlanding;
	double PickUpHead1PlaceFlowRate;
	double PickUpHead1PlaceForce;
	double PickUpHead1PlacePressure;
	double PickUpHead2PlaceFlowRate;
	double PickUpHead2PlaceForce;
	double PickUpHead2PlacePressure;
#pragma endregion

#pragma region Vision
	bool EnableVision;

	bool EnableTeachUnitAtVision;

	bool EnableInputVision;
	bool EnableS2Vision;
	bool EnableS1Vision;
	bool EnableSetupVision;
	bool EnableBottomVision;
	bool EnableS3Vision;
	bool EnableOutputVision;
	bool EnableOutputVision2ndPostAlign;

	unsigned int InputVisionRetryCountAfterFail;
	unsigned int InputVisionContinuousFailCountToTriggerAlarm;

	unsigned int S2VisionRetryCountAfterFail;
	unsigned int S2VisionContinuousFailCountToTriggerAlarm;

	unsigned int S2FacetVisionRetryCountAfterFail;
	unsigned int S2FacetVisionContinuousFailCountToTriggerAlarm;

	unsigned int S1VisionRetryCountAfterFail;
	unsigned int S1VisionContinuousFailCountToTriggerAlarm;

	unsigned int SetupVisionRetryCountAfterFail;
	unsigned int SetupVisionContinuousFailCountToTriggerAlarm;

	unsigned int BottomVisionRetryCountAfterFail;
	unsigned int BottomVisionContinuousFailCountToTriggerAlarm;

	unsigned int S3VisionRetryCountAfterFail;
	unsigned int S3VisionContinuousFailCountToTriggerAlarm;

	unsigned int OutputVisionRetryCountAfterFail;
	unsigned int OutputVisionContinuousFailCountToTriggerAlarm;

	unsigned int S3FacetVisionRetryCountAfterFail;
	unsigned int S3FacetVisionContinuousFailCountToTriggerAlarm;

	unsigned int PickupHeadRetryPickingNo;
	unsigned int PickupHeadRetryPlacingNo;

	signed long InputVisionUnitThetaOffset;
	signed long	S2VisionUnitThetaOffset;
	signed long	S2FacetVisionUnitThetaOffset;
	signed long	S1VisionUnitThetaOffset;
	signed long	BottomVisionUnitThetaOffset;
	signed long	S3VisionUnitThetaOffset;
	signed long	S3FacetVisionUnitThetaOffset;
	signed long	OutputVisionUnitThetaOffset;
	signed long SetupVisionUnitThetaOffset;
	//signed long UnitCenterToInputFocusOffset;
	//signed long UnitCenterToS2FocusOffset;
	//signed long UnitCenterToSetupFocusOffset;
	signed long	UnitCenterToBottomVisionFocusOffset;
	signed long	UnitCenterToSidewallLeftFocusOffset;
	signed long	UnitCenterToSidewallRightFocusOffset;
	signed long	UnitCenterToSidewallFrontFocusOffset;
	signed long	UnitCenterToSidewallRearFocusOffset;
	signed long	UnitCenterToS3FocusOffset;
	//signed long UnitCenterToOutputFocusOffset;

	signed long PickUpHeadCompensationXOffset[2];
	signed long PickUpHeadCompensationYOffset[2];

	signed long PickUpHeadHeadCompensationXOffset[2];
	signed long PickUpHeadHeadCompensationYOffset[2];

	signed long PickUpHeadOutputCompensationXOffset[2];
	signed long PickUpHeadOutputCompensationYOffset[2];
	signed long PickUpHeadOutputCompensationThetaOffset[2];

	signed long PickUpHeadRotationXOffset[2];
	signed long PickUpHeadRotationYOffset[2];

	bool EnablePickupHeadRetryPickingNo;
	bool EnablePickupHeadRetryPlacingNo;
	bool EnableBarcodePrinter;
	//bool EnableGoodSamplingSequence;
	//bool EnableScanBarcodeOnOutputTray;

	//bool EnableInputTableVacuum;
	bool EnableCountDownByInputQuantity;
	bool EnableCountDownByInputTrayNo;
	bool EnableS2S3BothSnapping;
	bool EnableSafetyPnPMovePickStation;
#pragma endregion
};
class RProduct_API ProductSharedMemoryEvent : public SharedMemoryEvent
{
public:
#pragma region Motion Controller
	sEvent StartInitializeMotionController1;
	sEvent InitializeMotionController1Done;
	sEvent StartInitializeMotionController2;
	sEvent InitializeMotionController2Done;
	sEvent StartInitializeMotionController3;
	sEvent InitializeMotionController3Done;
	sEvent StartInitializeMotionController4;
	sEvent InitializeMotionController4Done;
	sEvent StartInitializeMotionController5;
	sEvent InitializeMotionController5Done;
	sEvent StartInitializeMotionController6;
	sEvent InitializeMotionController6Done;
	sEvent StartInitializeMotionController7;
	sEvent InitializeMotionController7Done;

	sEvent StartPickAndPlace1YAxisMotorHome;
	sEvent PickAndPlace1YAxisMotorHomeDone;
	sEvent StartPickAndPlace1YAxisMotorSettingUp;
	sEvent PickAndPlace1YAxisMotorSettingUpDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToInputPosition;
	sEvent PickAndPlace1YAxisMotorMoveToInputPositionDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToS1Position;
	sEvent PickAndPlace1YAxisMotorMoveToS1PositionDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToS3Position;
	sEvent PickAndPlace1YAxisMotorMoveToS3PositionDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToOutputPosition;
	sEvent PickAndPlace1YAxisMotorMoveToOutputPositionDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToStandbyPosition;
	sEvent PickAndPlace1YAxisMotorMoveToStandbyPositionDone;
	sEvent StartPickAndPlace1YAxisMotorMove;
	sEvent PickAndPlace1YAxisMotorMoveDone;
	sEvent StartPickAndPlace1YAxisMotorStop;
	sEvent PickAndPlace1YAxisMotorStopDone;
	sEvent StartPickAndPlace1YAxisMotorOff;
	sEvent PickAndPlace1YAxisMotorOffDone;
	sEvent PickAndPlace1YAxisMotorMoveCurve;

	sEvent StartPickAndPlace1YAxisMotorMoveToInputPositionCurve;
	sEvent PickAndPlace1YAxisMotorMoveToInputPositionCurveDone;
	sEvent StartPickAndPlace1YAxisMotorMoveToOutputPositionCurve;
	sEvent PickAndPlace1YAxisMotorMoveToOutputPositionCurveDone;
	sEvent StartPickAndPlace1YAxisMotorMoveCurve;
	sEvent PickAndPlace1YAxisMotorMoveCurveDone;

	sEvent StartInputTrayTableXAxisMotorHome;
	sEvent InputTrayTableXAxisMotorHomeDone;
	sEvent StartInputTrayTableXAxisMotorSettingUp;
	sEvent InputTrayTableXAxisMotorSettingUpDone;
	sEvent StartInputTrayTableXAxisMotorMoveLoad;
	sEvent InputTrayTableXAxisMotorMoveLoadDone;
	sEvent StartInputTrayTableXAxisMotorMoveUnload;
	sEvent InputTrayTableXAxisMotorMoveUnloadDone;
	sEvent StartInputTrayTableXAxisMotorMoveCenter;
	sEvent InputTrayTableXAxisMotorMoveCenterDone;
	sEvent StartInputTrayTableXAxisMotorMove;
	sEvent InputTrayTableXAxisMotorMoveDone;
	sEvent StartInputTrayTableXAxisMotorStop;
	sEvent InputTrayTableXAxisMotorStopDone;

	sEvent StartInputTrayTableYAxisMotorHome;
	sEvent InputTrayTableYAxisMotorHomeDone;
	sEvent StartInputTrayTableYAxisMotorSettingUp;
	sEvent InputTrayTableYAxisMotorSettingUpDone;
	sEvent StartInputTrayTableYAxisMotorMoveLoad;
	sEvent InputTrayTableYAxisMotorMoveLoadDone;
	sEvent StartInputTrayTableYAxisMotorMoveUnload;
	sEvent InputTrayTableYAxisMotorMoveUnloadDone;
	sEvent StartInputTrayTableYAxisMotorMoveCenter;
	sEvent InputTrayTableYAxisMotorMoveCenterDone;
	sEvent StartInputTrayTableYAxisMotorMove;
	sEvent InputTrayTableYAxisMotorMoveDone;
	sEvent StartInputTrayTableYAxisMotorStop;
	sEvent InputTrayTableYAxisMotorStopDone;

	sEvent StartInputTrayTableZAxisMotorHome;
	sEvent InputTrayTableZAxisMotorHomeDone;
	sEvent StartInputTrayTableZAxisMotorSettingUp;
	sEvent InputTrayTableZAxisMotorSettingUpDone;
	sEvent StartInputTrayTableZAxisMotorMoveDown;
	sEvent InputTrayTableZAxisMotorMoveDownDone;
	sEvent StartInputTrayTableZAxisMotorMoveLoad;
	sEvent InputTrayTableZAxisMotorMoveLoadDone;
	sEvent StartInputTrayTableZAxisMotorMoveSingulation;
	sEvent InputTrayTableZAxisMotorMoveSingulationDone;
	sEvent StartInputTrayTableZAxisMotorMoveUnload;
	sEvent InputTrayTableZAxisMotorMoveUnloadDone;
	sEvent StartInputTrayTableZAxisMotorMove;
	sEvent InputTrayTableZAxisMotorMoveDone;
	sEvent StartInputTrayTableZAxisMotorStop;
	sEvent InputTrayTableZAxisMotorStopDone;
	sEvent StartInputTrayTableZAxisMotorMoveSecondSingulation;
	sEvent InputTrayTableZAxisMotorMoveSecondSingulationDone;
	sEvent StartInputTrayTableZAxisMotorChangeSlowSpeed;
	sEvent InputTrayTableZAxisMotorChangeSlowSpeedDone;
	sEvent StartInputTrayTableZAxisMotorChangeNormalSpeed;
	sEvent InputTrayTableZAxisMotorChangeNormalSpeedDone;

	sEvent StartPickAndPlace2YAxisMotorHome;
	sEvent PickAndPlace2YAxisMotorHomeDone;
	sEvent StartPickAndPlace2YAxisMotorSettingUp;
	sEvent PickAndPlace2YAxisMotorSettingUpDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToInputPosition;
	sEvent PickAndPlace2YAxisMotorMoveToInputPositionDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToS1Position;
	sEvent PickAndPlace2YAxisMotorMoveToS1PositionDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToS3Position;
	sEvent PickAndPlace2YAxisMotorMoveToS3PositionDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToOutputPosition;
	sEvent PickAndPlace2YAxisMotorMoveToOutputPositionDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToStandbyPosition;
	sEvent PickAndPlace2YAxisMotorMoveToStandbyPositionDone;
	sEvent StartPickAndPlace2YAxisMotorMove;
	sEvent PickAndPlace2YAxisMotorMoveDone;
	sEvent StartPickAndPlace2YAxisMotorStop;
	sEvent PickAndPlace2YAxisMotorStopDone;
	sEvent StartPickAndPlace2YAxisMotorOff;
	sEvent PickAndPlace2YAxisMotorOffDone;
	sEvent PickAndPlace2YAxisMotorMoveCurve;

	sEvent StartPickAndPlace2YAxisMotorMoveToInputPositionCurve;
	sEvent PickAndPlace2YAxisMotorMoveToInputPositionCurveDone;
	sEvent StartPickAndPlace2YAxisMotorMoveToOutputPositionCurve;
	sEvent PickAndPlace2YAxisMotorMoveToOutputPositionCurveDone;
	sEvent StartPickAndPlace2YAxisMotorMoveCurve;
	sEvent PickAndPlace2YAxisMotorMoveCurveDone;

	sEvent StartOutputTrayTableXAxisMotorHome;
	sEvent OutputTrayTableXAxisMotorHomeDone;
	sEvent StartOutputTrayTableXAxisMotorSettingUp;
	sEvent OutputTrayTableXAxisMotorSettingUpDone;
	sEvent StartOutputTrayTableXAxisMotorMoveLoad;
	sEvent OutputTrayTableXAxisMotorMoveLoadDone;
	sEvent StartOutputTrayTableXAxisMotorMoveUnload;
	sEvent OutputTrayTableXAxisMotorMoveUnloadDone;
	sEvent StartOutputTrayTableXAxisMotorMoveCenter;
	sEvent OutputTrayTableXAxisMotorMoveCenterDone;
	sEvent StartOutputTrayTableXAxisMotorMoveRejectTrayCenter;
	sEvent OutputTrayTableXAxisMotorMoveRejectTrayCenterDone;
	sEvent StartOutputTrayTableXAxisMotorMoveManualLoadUnload;
	sEvent OutputTrayTableXAxisMotorMoveManualLoadUnloadDone;
	sEvent StartOutputTrayTableXAxisMotorMove;
	sEvent OutputTrayTableXAxisMotorMoveDone;
	sEvent StartOutputTrayTableXAxisMotorStop;
	sEvent OutputTrayTableXAxisMotorStopDone;

	sEvent StartOutputTrayTableYAxisMotorHome;
	sEvent OutputTrayTableYAxisMotorHomeDone;
	sEvent StartOutputTrayTableYAxisMotorSettingUp;
	sEvent OutputTrayTableYAxisMotorSettingUpDone;
	sEvent StartOutputTrayTableYAxisMotorMoveLoad;
	sEvent OutputTrayTableYAxisMotorMoveLoadDone;
	sEvent StartOutputTrayTableYAxisMotorMoveUnload;
	sEvent OutputTrayTableYAxisMotorMoveUnloadDone;
	sEvent StartOutputTrayTableYAxisMotorMoveCenter;
	sEvent OutputTrayTableYAxisMotorMoveCenterDone;
	sEvent StartOutputTrayTableYAxisMotorMoveRejectTrayCenter;
	sEvent OutputTrayTableYAxisMotorMoveRejectTrayCenterDone;
	sEvent StartOutputTrayTableYAxisMotorMoveManualLoadUnload;
	sEvent OutputTrayTableYAxisMotorMoveManualLoadUnloadDone;
	sEvent StartOutputTrayTableYAxisMotorMove;
	sEvent OutputTrayTableYAxisMotorMoveDone;
	sEvent StartOutputTrayTableYAxisMotorStop;
	sEvent OutputTrayTableYAxisMotorStopDone;

	sEvent StartOutputTrayTableZAxisMotorHome;
	sEvent OutputTrayTableZAxisMotorHomeDone;
	sEvent StartOutputTrayTableZAxisMotorSettingUp;
	sEvent OutputTrayTableZAxisMotorSettingUpDone;
	sEvent StartOutputTrayTableZAxisMotorMoveDown;
	sEvent OutputTrayTableZAxisMotorMoveDownDone;
	sEvent StartOutputTrayTableZAxisMotorMoveLoad;
	sEvent OutputTrayTableZAxisMotorMoveLoadDone;
	sEvent StartOutputTrayTableZAxisMotorMoveSingulation;
	sEvent OutputTrayTableZAxisMotorMoveSingulationDone;
	sEvent StartOutputTrayTableZAxisMotorMoveSecondSingulation;
	sEvent OutputTrayTableZAxisMotorMoveSecondSingulationDone;
	sEvent StartOutputTrayTableZAxisMotorMoveUnload;
	sEvent OutputTrayTableZAxisMotorMoveUnloadDone;
	sEvent StartOutputTrayTableZAxisMotorMove;
	sEvent OutputTrayTableZAxisMotorMoveDone;
	sEvent StartOutputTrayTableZAxisMotorStop;
	sEvent OutputTrayTableZAxisMotorStopDone;
	sEvent StartOutputTrayTableZAxisMotorChangeSlowSpeed;
	sEvent OutputTrayTableZAxisMotorChangeSlowSpeedDone;
	sEvent StartOutputTrayTableZAxisMotorChangeNormalSpeed;
	sEvent OutputTrayTableZAxisMotorChangeNormalSpeedDone;

	sEvent StartInputVisionModuleMotorHome;
	sEvent InputVisionModuleMotorHomeDone;
	sEvent StartInputVisionModuleMotorSettingUp;
	sEvent InputVisionModuleMotorSettingUpDone;
	sEvent StartInputVisionModuleMotorMoveFocusPosition;
	sEvent InputVisionModuleMotorMoveFocusPositionDone;
	sEvent StartInputVisionModuleMotorMove;
	sEvent InputVisionModuleMotorMoveDone;
	sEvent StartInputVisionModuleMotorStop;
	sEvent InputVisionModuleMotorStopDone;

	sEvent StartS1VisionModuleMotorHome;
	sEvent S1VisionModuleMotorHomeDone;
	sEvent StartS1VisionModuleMotorSettingUp;
	sEvent S1VisionModuleMotorSettingUpDone;
	sEvent StartS1VisionModuleMotorMoveFocusPosition;
	sEvent S1VisionModuleMotorMoveFocusPositionDone;
	sEvent StartS1VisionModuleMotorMoveRetractPosition;
	sEvent S1VisionModuleMotorMoveRetractPositionDone;
	sEvent StartS1VisionModuleMotorMove;
	sEvent S1VisionModuleMotorMoveDone;
	sEvent StartS1VisionModuleMotorStop;
	sEvent S1VisionModuleMotorStopDone;

	sEvent StartS2VisionModuleMotorHome;
	sEvent S2VisionModuleMotorHomeDone;
	sEvent StartS2VisionModuleMotorSettingUp;
	sEvent S2VisionModuleMotorSettingUpDone;
	sEvent StartS2VisionModuleMotorMoveFocusPosition;
	sEvent S2VisionModuleMotorMoveFocusPositionDone;
	sEvent StartS2VisionModuleMotorMove;
	sEvent S2VisionModuleMotorMoveDone;
	sEvent StartS2VisionModuleMotorStop;
	sEvent S2VisionModuleMotorStopDone;

	sEvent StartS3VisionModuleMotorHome;
	sEvent S3VisionModuleMotorHomeDone;
	sEvent StartS3VisionModuleMotorSettingUp;
	sEvent S3VisionModuleMotorSettingUpDone;
	sEvent StartS3VisionModuleMotorMoveFocusPosition;
	sEvent S3VisionModuleMotorMoveFocusPositionDone;
	sEvent StartS3VisionModuleMotorMove;
	sEvent S3VisionModuleMotorMoveDone;
	sEvent StartS3VisionModuleMotorStop;
	sEvent S3VisionModuleMotorStopDone;

	sEvent StartPickAndPlace1XAxisMotorHome;
	sEvent PickAndPlace1XAxisMotorHomeDone;
	sEvent StartPickAndPlace1XAxisMotorSettingUp;
	sEvent PickAndPlace1XAxisMotorSettingUpDone;
	sEvent StartPickAndPlace1XAxisMotorMoveToInputPosition;
	sEvent PickAndPlace1XAxisMotorMoveToInputPositionDone;
	sEvent StartPickAndPlace1XAxisMotorMoveToS1Position;
	sEvent PickAndPlace1XAxisMotorMoveToS1PositionDone;
	sEvent StartPickAndPlace1XAxisMotorMoveToS3Position;
	sEvent PickAndPlace1XAxisMotorMoveToS3PositionDone;
	sEvent StartPickAndPlace1XAxisMotorMoveToOutputPosition;
	sEvent PickAndPlace1XAxisMotorMoveToOutputPositionDone;
	sEvent StartPickAndPlace1XAxisMotorMoveToParkingPosition;
	sEvent PickAndPlace1XAxisMotorMoveToParkingPositionDone;
	sEvent StartPickAndPlace1XAxisMotorSetTriggerStartEnd;
	sEvent PickAndPlace1XAxisMotorSetTriggerStartEndDone;
	sEvent StartPickAndPlace1XAxisMotorEnableTriggerPoint;
	sEvent PickAndPlace1XAxisMotorEnableTriggerPointDone;
	sEvent StartPickAndPlace1XAxisMotorDisableTriggerPoint;
	sEvent PickAndPlace1XAxisMotorDisableTriggerPointDone;
	sEvent StartPickAndPlace1XAxisMotorMove;
	sEvent PickAndPlace1XAxisMotorMoveDone;
	sEvent StartPickAndPlace1XAxisMotorStop;
	sEvent PickAndPlace1XAxisMotorStopDone;
	sEvent StartPickAndPlace1XAxisMotorOff;
	sEvent PickAndPlace1XAxisMotorOffDone;

	sEvent StartPickAndPlace2XAxisMotorHome;
	sEvent PickAndPlace2XAxisMotorHomeDone;
	sEvent StartPickAndPlace2XAxisMotorSettingUp;
	sEvent PickAndPlace2XAxisMotorSettingUpDone;
	sEvent StartPickAndPlace2XAxisMotorMoveToInputPosition;
	sEvent PickAndPlace2XAxisMotorMoveToInputPositionDone;
	sEvent StartPickAndPlace2XAxisMotorMoveToS1Position;
	sEvent PickAndPlace2XAxisMotorMoveToS1PositionDone;
	sEvent StartPickAndPlace2XAxisMotorMoveToS3Position;
	sEvent PickAndPlace2XAxisMotorMoveToS3PositionDone;
	sEvent StartPickAndPlace2XAxisMotorMoveToOutputPosition;
	sEvent PickAndPlace2XAxisMotorMoveToOutputPositionDone;
	sEvent StartPickAndPlace2XAxisMotorMoveToParkingPosition;
	sEvent PickAndPlace2XAxisMotorMoveToParkingPositionDone;
	sEvent StartPickAndPlace2XAxisMotorSetTriggerStartEnd;
	sEvent PickAndPlace2XAxisMotorSetTriggerStartEndDone;
	sEvent StartPickAndPlace2XAxisMotorEnableTriggerPoint;
	sEvent PickAndPlace2XAxisMotorEnableTriggerPointDone;
	sEvent StartPickAndPlace2XAxisMotorDisableTriggerPoint;
	sEvent PickAndPlace2XAxisMotorDisableTriggerPointDone;
	sEvent StartPickAndPlace2XAxisMotorMove;
	sEvent PickAndPlace2XAxisMotorMoveDone;
	sEvent StartPickAndPlace2XAxisMotorStop;
	sEvent PickAndPlace2XAxisMotorStopDone;
	sEvent StartPickAndPlace2XAxisMotorOff;
	sEvent PickAndPlace2XAxisMotorOffDone;

	sEvent StartPickAndPlace1ZAxisMotorHome;
	sEvent PickAndPlace1ZAxisMotorHomeDone;
	sEvent StartPickAndPlace1ZAxisMotorSettingUp;
	sEvent PickAndPlace1ZAxisMotorSettingUpDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveUpPosition;
	sEvent PickAndPlace1ZAxisMotorMoveUpPositionDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveUpPositionAndRotate;
	sEvent PickAndPlace1ZAxisMotorMoveUpPositionAndRotateDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveToInputTrayDownPosition;
	sEvent PickAndPlace1ZAxisMotorMoveToInputTrayDownPositionDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPosition;
	sEvent PickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPositionDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveToOutputTrayDownPosition;
	sEvent PickAndPlace1ZAxisMotorMoveToOutputTrayDownPositionDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPosition;
	sEvent PickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPositionDone;
	sEvent StartPickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPositionForPicking;
	sEvent PickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone;
	sEvent StartPickAndPlace1ZAxisMotorSoftlandingPickS1AndS3Ceramic;
	sEvent PickAndPlace1ZAxisMotorSoftlandingPickS1AndS3CeramicDone;
	sEvent StartPickAndPlace1ZAxisMotorSoftlandingPickSWCeramic;
	sEvent PickAndPlace1ZAxisMotorSoftlandingPickSWCeramicDone;
	sEvent StartPickAndPlace1ZAxisMotorSoftlandingPlaceS1AndS3Ceramic;
	sEvent PickAndPlace1ZAxisMotorSoftlandingPlaceS1AndS3CeramicDone;
	sEvent StartPickAndPlace1ZAxisMotorSoftlandingPlaceSWCeramic;
	sEvent PickAndPlace1ZAxisMotorSoftlandingPlaceSWCeramicDone;
	sEvent StartPickAndPlace1ZAxisMotorMove;
	sEvent PickAndPlace1ZAxisMotorMoveDone;
	sEvent StartPickAndPlace1ZAxisMotorStop;
	sEvent PickAndPlace1ZAxisMotorStopDone;
	sEvent StartPickAndPlace1ZAxisMotorOff;
	sEvent PickAndPlace1ZAxisMotorOffDone;
	sEvent StartPickAndPlace1THKMotorOff;
	sEvent PickAndPlace1THKMotorOffDone;
	sEvent StartPickAndPlace1ZAxisReleaseAndOffValve;
	sEvent PickAndPlace1ZAxisReleaseAndOffValveDone;
	sEvent StartPickAndPlace1ZAxisOffVacuum;
	sEvent PickAndPlace1ZAxisOffVacuumDone;
	sEvent StartPickAndPlace1ZAxisOnValve;
	sEvent PickAndPlace1ZAxisOnValveDone;
	sEvent StartPickAndPlace1ZAxisOffValve;
	sEvent PickAndPlace1ZAxisOffValveDone;

	sEvent StartPickAndPlace2ZAxisMotorHome;
	sEvent PickAndPlace2ZAxisMotorHomeDone;
	sEvent StartPickAndPlace2ZAxisMotorSettingUp;
	sEvent PickAndPlace2ZAxisMotorSettingUpDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveUpPosition;
	sEvent PickAndPlace2ZAxisMotorMoveUpPositionDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate;
	sEvent PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveToInputTrayDownPosition;
	sEvent PickAndPlace2ZAxisMotorMoveToInputTrayDownPositionDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPosition;
	sEvent PickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPositionDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveToOutputTrayDownPosition;
	sEvent PickAndPlace2ZAxisMotorMoveToOutputTrayDownPositionDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPosition;
	sEvent PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionDone;
	sEvent StartPickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPicking;
	sEvent PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone;
	sEvent StartPickAndPlace2ZAxisMotorSoftlandingPickS1AndS3Ceramic;
	sEvent PickAndPlace2ZAxisMotorSoftlandingPickS1AndS3CeramicDone;
	sEvent StartPickAndPlace2ZAxisMotorSoftlandingPickSWCeramic;
	sEvent PickAndPlace2ZAxisMotorSoftlandingPickSWCeramicDone;
	sEvent StartPickAndPlace2ZAxisMotorSoftlandingPlaceS1AndS3Ceramic;
	sEvent PickAndPlace2ZAxisMotorSoftlandingPlaceS1AndS3CeramicDone;
	sEvent StartPickAndPlace2ZAxisMotorSoftlandingPlaceSWCeramic;
	sEvent PickAndPlace2ZAxisMotorSoftlandingPlaceSWCeramicDone;
	sEvent StartPickAndPlace2ZAxisMotorMove;
	sEvent PickAndPlace2ZAxisMotorMoveDone;
	sEvent StartPickAndPlace2ZAxisMotorStop;
	sEvent PickAndPlace2ZAxisMotorStopDone;
	sEvent StartPickAndPlace2ZAxisMotorOff;
	sEvent PickAndPlace2ZAxisMotorOffDone;
	sEvent StartPickAndPlace2THKMotorOff;
	sEvent PickAndPlace2THKMotorOffDone;
	sEvent StartPickAndPlace2ZAxisReleaseAndOffValve;
	sEvent PickAndPlace2ZAxisReleaseAndOffValveDone;
	sEvent StartPickAndPlace2ZAxisOffVacuum;
	sEvent PickAndPlace2ZAxisOffVacuumDone;
	sEvent StartPickAndPlace2ZAxisOnValve;
	sEvent PickAndPlace2ZAxisOnValveDone;
	sEvent StartPickAndPlace2ZAxisOffValve;
	sEvent PickAndPlace2ZAxisOffValveDone;

	sEvent StartPickAndPlace1ThetaAxisMotorHome;
	sEvent PickAndPlace1ThetaAxisMotorHomeDone;
	sEvent StartPickAndPlace1ThetaAxisMotorSettingUp;
	sEvent PickAndPlace1ThetaAxisMotorSettingUpDone;
	sEvent StartPickAndPlace1ThetaAxisMotorMoveStandbyPosition;
	sEvent PickAndPlace1ThetaAxisMotorMoveStandbyPositionDone;
	sEvent StartPickAndPlace1ThetaAxisMotorMove;
	sEvent PickAndPlace1ThetaAxisMotorMoveDone;
	sEvent StartPickAndPlace1ThetaAxisMotorStop;
	sEvent PickAndPlace1ThetaAxisMotorStopDone;


	sEvent StartPickAndPlace2ThetaAxisMotorHome;
	sEvent PickAndPlace2ThetaAxisMotorHomeDone;
	sEvent StartPickAndPlace2ThetaAxisMotorSettingUp;
	sEvent PickAndPlace2ThetaAxisMotorSettingUpDone;
	sEvent StartPickAndPlace2ThetaAxisMotorMoveStandbyPosition;
	sEvent PickAndPlace2ThetaAxisMotorMoveStandbyPositionDone;
	sEvent StartPickAndPlace2ThetaAxisMotorMove;
	sEvent PickAndPlace2ThetaAxisMotorMoveDone;
	sEvent StartPickAndPlace2ThetaAxisMotorStop;
	sEvent PickAndPlace2ThetaAxisMotorStopDone;

	sEvent RTHD_GMAIN_TRAY_PRESENT_ON_TABLE_START;
	sEvent GMAIN_RTHD_TRAY_PRESENT_ON_TABLE_CLEAR_DONE;

#pragma endregion

#pragma region Vision
	sEvent RTHD_RMAIN_INPUT_VISION_END;
	sEvent RTHD_RMAIN_OUTPUT_VISION_END;
	sEvent RTHD_RMAIN_BOTTOM_VISION_END;
	sEvent RTHD_RMAIN_S2_VISION_END;
	sEvent RTHD_RMAIN_S3_VISION_END;
	sEvent RTHD_RMAIN_S1_VISION_END;

	sEvent RSEQ_RINPV_START_VISION;
	sEvent RINPV_RSEQ_VISION_DONE;
	sEvent RSEQ_RINPV_START_VISION_RETEST;
	sEvent RINPV_RSEQ_VISION_RETEST_DONE;
	sEvent RMAIN_RINPV_GET_VISION_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_INP_VISION_RC_START;
	sEvent GMAIN_RTHD_INP_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_INP_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE;
	sEvent RTHD_GMAIN_GET_INP_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_INP_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_INP_VISION_XYTR_FAIL;
	sEvent GMAIN_RTHD_INP_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_INP_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_INP_VISION_LEARN_UNIT_DONE;
	sEvent RINPV_GMAIN_INP_VISION_RESET_EOV;


	sEvent RSEQ_RS2V_START_VISION;
	sEvent RS2V_RSEQ_VISION_DONE;
	sEvent RSEQ_RS2V_START_VISION_RETEST;
	sEvent RS2V_RSEQ_VISION_RETEST_DONE;
	sEvent RMAIN_RS2V_GET_VISION_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE;
	sEvent RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_S2_VISION_RC_START;
	sEvent GMAIN_RTHD_S2_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S2_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START;
	sEvent GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START;
	sEvent GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S2_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_S2_VISION_GET_ADD_RC_DONE;
	sEvent RTHD_GMAIN_GET_S2_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_S2_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_S2_VISION_XYTR_FAIL;
	sEvent GMAIN_RTHD_S2_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_S2_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_S2_VISION_LEARN_UNIT_DONE;
	sEvent RS2V_GMAIN_S2_VISION_RESET_EOV;

	sEvent RSEQ_ROUTV_START_VISION;
	sEvent ROUTV_RSEQ_VISION_DONE;
	sEvent RSEQ_ROUTV_START_VISION_RETEST;
	sEvent ROUTV_RSEQ_VISION_RETEST_DONE;
	sEvent GMAIN_ROUTV_GET_VISION_RESULT_DONE;
	sEvent GMAIN_ROUTV_POST_GET_VISION_RESULT_DONE;

	sEvent RTHD_GMAIN_SEND_OUT_VISION_RC_START;
	sEvent GMAIN_RTHD_OUT_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_OUT_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_REJECT_VISION_RC_START;
	sEvent GMAIN_RTHD_REJECT_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_REJECT_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START;
	sEvent GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_OUT_POST_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_OUT_POST_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_OUT_POST_VISION_GET_ADD_RC_DONE;
	sEvent RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START;
	sEvent GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_REJECT_POST_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_GET_OUT_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL;
	sEvent RTHD_GMAIN_GET_OUT_POST_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_FAIL;
	sEvent RTHD_GMAIN_GET_REJECT_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL;
	sEvent RTHD_GMAIN_GET_REJECT_POST_VISION_XYTR_START;
	sEvent GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_DONE;
	sEvent GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_FAIL;
	sEvent GMAIN_RTHD_OUT_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_OUT_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_OUT_VISION_LEARN_UNIT_DONE;
	sEvent ROUTV_GMAIN_OUT_VISION_RESET_EOV;

	sEvent RSEQ_RSTPV_START_VISION;
	sEvent RSTPV_RSEQ_VISION_DONE;
	sEvent RSEQ_RSTPV_START_VISION_RETEST;
	sEvent RSTPV_RSEQ_VISION_RETEST_DONE;
	sEvent RMAIN_RSTPV_GET_VISION_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_SETUP_VISION_RC_START;
	sEvent GMAIN_RTHD_SETUP_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_SETUP_VISION_GET_RC_NAK;
	sEvent GMAIN_RTHD_SETUP_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_SETUP_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_SETUP_VISION_LEARN_UNIT_DONE;
	sEvent RSTPV_GMAIN_SETUP_VISION_RESET_EOV;

	sEvent RSEQ_RBTMV_START_VISION;
	sEvent RBTMV_RSEQ_VISION_DONE;
	sEvent RSEQ_RBTMV_START_VISION_RETEST;
	sEvent RBTMV_RSEQ_VISION_RETEST_DONE;
	sEvent GMAIN_RBTMV_GET_VISION_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START;
	sEvent GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_BOTTOM_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_BOTTOM_VISION_GET_ADD_RC_DONE;
	sEvent GMAIN_RTHD_BOTTOM_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_BOTTOM_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_BOTTOM_VISION_LEARN_UNIT_DONE;
	sEvent RBTMV_GMAIN_BOTTOM_VISION_RESET_EOV;

	sEvent RSEQ_RS1V_START_VISION;
	sEvent RS1V_RSEQ_VISION_DONE;
	sEvent RSEQ_RS1V_START_VISION_RETEST;
	sEvent RS1V_RSEQ_VISION_RETEST_DONE;
	sEvent GMAIN_RS1V_GET_VISION_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_S1_VISION_RC_START;
	sEvent GMAIN_RTHD_S1_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S1_VISION_GET_RC_NAK;
	sEvent GMAIN_RSTV_GET_VISION_RESULT_DONE;
	sEvent RTHD_GMAIN_SEND_S1_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_S1_VISION_GET_ADD_RC_DONE;
	sEvent GMAIN_RTHD_S1_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_S1_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_S1_VISION_LEARN_UNIT_DONE;
	sEvent RS1V_GMAIN_S1_VISION_RESET_EOV;

	sEvent RSEQ_RS3V_START_VISION;
	sEvent RS3V_RSEQ_VISION_DONE;
	sEvent RSEQ_RS3V_START_VISION_RETEST;
	sEvent RS3V_RSEQ_VISION_RETEST_DONE;
	sEvent GMAIN_RS3V_GET_VISION_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE;
	sEvent GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE;	
	sEvent RTHD_GMAIN_SEND_S3_VISION_RC_START;
	sEvent GMAIN_RTHD_S3_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S3_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START;
	sEvent GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START;
	sEvent GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE;
	sEvent GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK;
	sEvent RTHD_GMAIN_SEND_S3_VISION_ADD_RC_START;
	sEvent GMAIN_RTHD_S3_VISION_GET_ADD_RC_DONE;
	sEvent GMAIN_RTHD_S3_VISION_NEED_LEARN_UNIT;
	sEvent RTHD_GMAIN_S3_VISION_LEARN_UNIT_START;
	sEvent GMAIN_RTHD_S3_VISION_LEARN_UNIT_DONE;
	sEvent RS3V_GMAIN_S3_VISION_RESET_EOV;
#pragma endregion

#pragma region Application	
	sEvent RTHD_RMAIN_MC1_END;
	sEvent RTHD_RMAIN_MC2_END;
	sEvent RTHD_RMAIN_MC3_END;
	sEvent RTHD_RMAIN_MC4_END;
	sEvent RTHD_RMAIN_MC5_END;
	sEvent RTHD_RMAIN_MC6_END;
	sEvent RTHD_RMAIN_MC7_END;

	sEvent RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END;

	sEvent RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END;

	sEvent RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END;
	sEvent RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END;
	sEvent RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END;
	sEvent RTHD_RMAIN_SIDE_WALL_VISION_RIGHT_MODULE_MOTOR_END;

	sEvent RTHD_RMAIN_SIDE_WALL_VISION_FRONT_MODULE_MOTOR_END;
	sEvent RTHD_RMAIN_SIDE_WALL_VISION_REAR_MODULE_MOTOR_END;
	sEvent RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END;

	sEvent RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END;

	sEvent RMAIN_RTHD_INPUT_TRAY_TABLE_START;
	sEvent RTHD_RMAIN_INPUT_TRAY_TABLE_DONE;

	sEvent RMAIN_RTHD_OUTPUT_TRAY_TABLE_START;
	sEvent RTHD_RMAIN_OUTPUT_TRAY_TABLE_DONE;

	sEvent GMAIN_RTHD_ENDLOT;
	sEvent GMAIN_RTHD_ENDLOT_UPDATE_SUMMARY_DONE;

	sEvent RMAIN_RTHD_SEND_OUTPUT_EVENT_START;
	sEvent RMAIN_RTHD_SEND_OUTPUT_EVENT_DONE;

	sEvent RMAIN_RTHD_INPUT_VISION_REQUIRE_ADDITIONAL_MOVE_AND_SNAP;

	sEvent RMAIN_RTHD_INPUT_UNIT_PRESENT;
	sEvent RMAIN_RTHD_INPUT_TRAY_EMPTY;

	sEvent RMAIN_RTHD_OUTPUT_FULL;
	sEvent RMAIN_RTHD_OUTPUT_OR_REJECT_FULL;
	

	sEvent RMAIN_RTHD_PNP_PICK_UNIT_START;
	sEvent RMAIN_RTHD_PNP_PICK_UNIT_DONE;
	sEvent RMAIN_RTHD_PNP_PLACE_UNIT_DONE;

	sEvent RMAIN_RTHD_IS_PNP_AWAY;
	sEvent RTHD_RMAIN_INPUT_TRAY_TABLE_END;
	sEvent RTHD_RMAIN_OUTPUT_TRAY_TABLE_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END;
	sEvent RTHD_RMAIN_PICK_AND_PLACE2_SEQUENCE_END;

	//new
	sEvent RMAIN_RTHD_INPUT_VISION_REQUIRE_ADDITIONAL_MOVE;

	sEvent RMAIN_RTHD_ADDITIONAL_INPUT_VISION_DONE;

	sEvent RMAIN_RTHD_GET_S2_VISION_DONE;
	sEvent RMAIN_RTHD_GET_OUTPUT_VISION_DONE;

	sEvent RSEQ_RINT_SEQUENCE_START;
	sEvent RSEQ_ROUT_SEQUENCE_START;
	sEvent RSEQ_RPNP1_SEQUENCE_START;
	sEvent RSEQ_RPNP2_SEQUENCE_START;

	sEvent RSEQ_RPNP1_STANDBY_START;
	sEvent RPNP1_RSEQ_STANDBY_DONE;
	sEvent RSEQ_RPNP2_STANDBY_START;
	sEvent RPNP2_RSEQ_STANDBY_DONE;

	sEvent RSEQ_RPNP1_MOVE_TO_INPUT_STATION_START;
	sEvent RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE;
	sEvent RSEQ_RPNP1_PROCESS_AT_INPUT_STATION_START;
	sEvent RPNP1_RSEQ_PROCESS_AT_INPUT_STATION_DONE;
	sEvent RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START;
	sEvent RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE;
	sEvent RSEQ_RPNP2_PROCESS_AT_INPUT_STATION_START;
	sEvent RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE;

	sEvent RSEQ_RPNP1_MOVE_TO_BOTTOM_STATION_START;
	sEvent RPNP1_RSEQ_MOVE_TO_BOTTOM_STATION_DONE;
	sEvent RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE;
	sEvent RSEQ_RPNP2_MOVE_TO_BOTTOM_STATION_START;
	sEvent RPNP2_RSEQ_MOVE_TO_BOTTOM_STATION_DONE;
	sEvent RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE;

	sEvent RSEQ_RPNP1_MOVE_TO_S3_STATION_START;
	sEvent RPNP1_RSEQ_MOVE_TO_S3_STATION_DONE;
	sEvent RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE;
	sEvent RSEQ_RPNP2_MOVE_TO_S3_STATION_START;
	sEvent RPNP2_RSEQ_MOVE_TO_S3_STATION_DONE;
	sEvent RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE;
	sEvent RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE;

	sEvent RSEQ_RPNP1_MOVE_TO_OUTPUT_STATION_START;
	sEvent RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE;
	sEvent RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE;
	sEvent RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START;
	sEvent RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE;
	sEvent RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE;

	sEvent RSEQ_RPNP1_POST_PRODUCTION_START;
	sEvent RPNP1_RSEQ_POST_PRODUCTION_DONE;
	sEvent RSEQ_RPNP2_POST_PRODUCTION_START;
	sEvent RPNP2_RSEQ_POST_PRODUCTION_DONE;

	sEvent RSEQ_RPNP1_REMOVE_UNIT_START;
	sEvent RSEQ_RPNP1_REMOVE_UNIT_SKIP;
	sEvent RPNP1_RSEQ_REMOVE_UNIT_DONE;
	sEvent RSEQ_RPNP2_REMOVE_UNIT_START;
	sEvent RSEQ_RPNP2_REMOVE_UNIT_SKIP;
	sEvent RPNP2_RSEQ_REMOVE_UNIT_DONE;

	sEvent RINT_RSEQ_INPUT_VISION_DONE;
	sEvent ROUT_RSEQ_OUTPUT_VISION_DONE;
	sEvent ROUT_RSEQ_OUTPUT_POST_VISION_DONE;

	sEvent RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED;	
	sEvent ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED;
	sEvent ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PICKED;
	sEvent RPNP_RINT_INPUT_UNIT_PICKED_DONE;
	sEvent RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE;
	sEvent RPNP_ROUT_OUTPUT_UNIT_PICKED_DONE;
	sEvent RPNP_ROUT_MOVE_TRAY_READY;
	sEvent RPNP_ROUT_NO_MOVE_TRAY_READY;

	sEvent RTHD_GMAIN_GET_OUTPUT_STATION_DONE;
	sEvent RTHD_GMAIN_START_GET_OUTPUT_STATION;

	sEvent RTHD_GMAIN_LOT_UNLOAD_COMPLETE;
	sEvent RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY;
	sEvent RTHD_GMAIN_ADD_TRAY_TO_WRITE_REPORT;
	sEvent RTHD_GMAIN_CONNECT_TO_CONTROLLER_DONE;

	sEvent RTHD_GMAIN_INPUT_SEND_NEWLOT;
	sEvent GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_S2_SEND_NEWLOT;
	sEvent GMAIN_RTHD_S2_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_BOTTOM_SEND_NEWLOT;
	sEvent GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_S1_SEND_NEWLOT;
	sEvent GMAIN_RTHD_S1_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_S3_SEND_NEWLOT;
	sEvent GMAIN_RTHD_S3_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_OUTPUT_SEND_NEWLOT;
	sEvent GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE;
	sEvent RTHD_GMAIN_REJECT_SEND_NEWLOT;
	sEvent GMAIN_RTHD_REJECT_SEND_NEWLOT_DONE;

	sEvent RTHD_GMAIN_INPUT_SEND_TRAYNO;
	sEvent GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_BTM_SEND_TRAYNO;
	sEvent GMAIN_RTHD_BTM_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_S2_SEND_TRAYNO;
	sEvent GMAIN_RTHD_S2_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_S1_SEND_TRAYNO;
	sEvent GMAIN_RTHD_S1_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_S3_SEND_TRAYNO;
	sEvent GMAIN_RTHD_S3_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_OUTPUT_SEND_TRAYNO;
	sEvent GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE;
	sEvent RTHD_GMAIN_REJECT_SEND_TRAYNO;
	sEvent GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE;
	
	sEvent RTHD_GMAIN_INPUT_SEND_ENDLOT;
	sEvent GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_BTM_SEND_ENDLOT;
	sEvent GMAIN_RTHD_BTM_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_S2_SEND_ENDLOT;
	sEvent GMAIN_RTHD_S2_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_S1_SEND_ENDLOT;
	sEvent GMAIN_RTHD_S1_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_S3_SEND_ENDLOT;
	sEvent GMAIN_RTHD_S3_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_OUTPUT_SEND_ENDLOT;
	sEvent GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE;
	sEvent RTHD_GMAIN_REJECT_SEND_ENDLOT;
	sEvent GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE;

	sEvent RTHD_GMAIN_INPUT_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_BTM_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_S2_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_S2_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_S1_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_S1_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_S3_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_S3_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_OUTPUT_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE;
	sEvent RTHD_GMAIN_REJECT_SEND_ENDTRAY;
	sEvent GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE;

	sEvent RTHD_GMAIN_SEND_INP_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_S2_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_S1_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_S3_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_OUT_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_BTM_VISION_VERIFY_DOTGRID;
	sEvent RTHD_GMAIN_SEND_VISION_VERIFY_DOTGRID_DONE;
	sEvent RTHD_GMAIN_SEND_VISION_VERIFY_DOTGRID_FAIL;

	sEvent RTHD_GMAIN_SEND_INP_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_S2_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_S1_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_S3_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_OUT_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_BTM_VISION_VERIFY_GRAYSCALE;
	sEvent RTHD_GMAIN_SEND_VISION_VERIFY_GRAYSCALE_DONE;
	sEvent RTHD_GMAIN_SEND_VISION_VERIFY_GRAYSCALE_FAIL;

	// WC
	sEvent ROUT_RINT_OUTPUT_PRE_PRODUCTION_DONE;
	sEvent RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE;
	sEvent RTHD_RMAIN_REACH_TOTAL_UNIT_DONE;

	sEvent ROUT_RPNP_PNP_REDO_POST_OFFSET_START;
	sEvent ROUT_RPNP_PNP_PICK_REJECT_FROM_OUTPUT_START;
	sEvent ROUT_RPNP_PNP_PLACE_REJECT_START;

	sEvent RMAIN_RTHD_ALARM_ASSIST_START;
	sEvent RMAIN_RTHD_ALARM_MESSAGE_START;
	sEvent RMAIN_RTHD_ALARM_FAILURE_START;

	sEvent RTHD_GMAIN_UNIT_PLACED_ON_OUTPUT_FRAME;

	sEvent RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_ON_START;
	sEvent RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_OFF_START;
	sEvent GMAIN_RTHD_SETUP_VISION_GET_LATENCY_ON_DONE;
	sEvent GMAIN_RTHD_SETUP_VISION_GET_LATENCY_OFF_DONE;

	sEvent RTHD_RMAIN_CHECK_REJECT_REPLACE_END;
	sEvent ROUT_RTHD_REPLACE_REJECT_TRAY_START;
	sEvent ROUT_RTHD_REPLACE_REJECT_TRAY_DONE;

	sEvent RINT_RSEQ_INPUT_UNIT_PICK_START;
	sEvent RINT_RSEQ_HEAD_AND_OUTPUT_FULL;
	sEvent RINT_RSEQ_LOADING_STACKER_EMPTY;

	sEvent ROUT_RSEQ_OUTPUT_TABLE_READY;

	sEvent RTHD_GMAIN_TEACH_VISION_START;
	sEvent GMAIN_RTHD_TEACH_VISION_DONE;
	sEvent GMAIN_RTHD_TEACHING_VISION;

	sEvent GMAIN_RTHD_VERIFY_POS_START;
	sEvent RTHD_GMAIN_VERIFY_POS_DONE;

	sEvent RINT_RSEQ_START_POST_PRODUCTION;
	sEvent RINT_RSEQ_POST_PRODUCTION_DONE;
	sEvent ROUT_RSEQ_START_POST_PRODUCTION;
	sEvent ROUT_RSEQ_POST_PRODUCTION_DONE;

	sEvent RSEQ_RPNP1_MOVE_STANDBY_START;
	sEvent RSEQ_RPNP2_MOVE_STANDBY_START;

	sEvent RPNP1_RSEQ_MOVE_STANDBY_DONE;
	sEvent RPNP2_RSEQ_MOVE_STANDBY_DONE;

	sEvent ROUT_RSEQ_REJECT1_RESET;
	sEvent ROUT_RSEQ_REJECT2_RESET;
	sEvent ROUT_RSEQ_REJECT3_RESET;
	sEvent ROUT_RSEQ_REJECT4_RESET;
	sEvent ROUT_RSEQ_REJECT5_RESET;

	sEvent ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE;
	sEvent RMAIN_RTHD_TRIGGER_BARCODE_PRINTER;

	sEvent RSEQ_GMAIN_UNLOAD_MATERIAL_START;
	sEvent RSEQ_GMAIN_REJECT_TRAY_EXCHANGE_START;

	sEvent RPNP1_RSEQ_Y_MOVE_STANDBY;
	sEvent RPNP2_RSEQ_Y_MOVE_STANDBY;

	//sEvent RTHD_RMAIN_CHECK_REJECT_REMOVE_END;
	sEvent RTHD_RMAIN_CHECK_REJECT_REMOVE_END;
	sEvent ROUT_RTHD_REMOVE_REJECT_TRAY_START;
	sEvent ROUT_RTHD_REMOVE_REJECT_TRAY_DONE;

	sEvent ROUT_RTHD_REMOVE_AND_REPLACE_REJECT_TRAY_START;
	sEvent ROUT_RTHD_REMOVE_AND_REPLACE_REJECT_TRAY_DONE;

	sEvent RMAIN_RTHD_READ_PREVIOUS_LOTID;
	sEvent RMAIN_RTHD_WRITE_PREVIOUS_LOTID;

	sEvent RPNP1_RSEQ_BYPASS_PICK_FAIL;
	sEvent RPNP2_RSEQ_BYPASS_PICK_FAIL;

	sEvent ReviewMode;

	sEvent RPNP_RSEQ_BYPASS_PLACE_FAIL;

	sEvent RPNP_RSEQ_BYPASS_PICK_FAIL_REJECT;

	sEvent RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE;

	sEvent RMAIN_RTHD_CONTINUE_LOT;
	sEvent RMAIN_RTHD_CONTINUE_ABORT;

	sEvent RINP_RSEQ_INPUT_TRAY_FULL;

	sEvent RMAIN_RTHD_ENDLOT_CONDITION;

	sEvent RMAIN_RTHD_START_RUNNING;

	sEvent ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE;

	sEvent RINP_RPNP_REQUIRE_PURGING;

	sEvent RMAIN_RTHD_CHANGE_MAPPING;

	sEvent RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL;

	sEvent ROUT_RSEQ_OUTPUT_FIRST_UNIT;
	sEvent ROUT_RSEQ_PNP_PICK_FIRST_UNIT;
	//sEvent RPNP2_RSEQ_BYPASS_PLACE_FAIL;
	sEvent GMAIN_RMAIN_STOP_CALIBRATION;
	sEvent GMAIN_RMAIN_LOADING_CALIBRATION_RECIPE_DONE;
	sEvent RMAIN_GMAIN_LOADING_CALIBRATION_RECIPE;

	sEvent RTHD_GMAIN_UPDATE_IN_PROGRESS_MAPPING;
	sEvent RTHD_GMAIN_UPDATE_INPUT_MAPPING;

	sEvent RINT_RSEQ_INPUT_FIRST_UNIT;
	sEvent RINT_RSEQ_SWITCH_NEXT_INPUT_LOT;

	sEvent RMAIN_GMAIN_SAVE_UNFINISHED_LOT;

	sEvent RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_START;
	sEvent RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_FAIL;
	sEvent RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_DONE;

	sEvent RMAIN_RINT_OUTPUT_FINE_TUNE_OR_SKIP_START;
	sEvent RMAIN_RINT_OUTPUT_FINE_TUNE_OR_SKIP_FAIL;
	sEvent RMAIN_RINT_OUTPUT_FINE_TUNE_OR_SKIP_DONE;

	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE;

	sEvent RMAIN_RTHD_CURRENT_INPUT_LOT_DONE;

	sEvent RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT;

	sEvent RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED;

	sEvent RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT;

	sEvent RMAIN_RTHD_UPDATE_MES_DATA;

	sEvent RMAIN_RTHD_UPDATE_MES_DATA_DONE;

	sEvent RMAIN_RTHD_NEW_OR_END_LOT_CONDITION;

	sEvent RMAIN_RTHD_UPDATE_MES_LOT_DATA;

	sEvent RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH;

	sEvent RMAIN_RTHD_IS_REMAINTRAY;
	sEvent RMAIN_RTHD_IS_UNLOADTRAY;
	sEvent RMAIN_RTHD_IS_REMAIN_OR_UNLOADTRAY;

	sEvent INP_SEQ_IS_UNLOADTRAY;
	sEvent SEQ_OUT_IS_UNLOADTRAY;
	sEvent SEQ_PNP_IS_UNLOADTRAY;

	sEvent RMAIN_RTHD_THR_IS_REMAINTRAY;

	sEvent RMAIN_GTHD_TRIGGER_BARCODE;
	sEvent RTHD_GMAIN_GET_BARCODE_DONE;
	sEvent RTHD_GMAIN_GET_BARCODE_FAIL;

	sEvent RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED;
	sEvent RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE;

	sEvent RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST;
	sEvent RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST_DONE;

	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_S1_MISSING_UNIT;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_S3_MISSING_UNIT;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_MISSING_UNIT;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_POST_MISSING_UNIT;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_REJECT_POST_MISSING_UNIT;

	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START_NOMES;
	sEvent RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE_NOMES;

	sEvent RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_START;
	sEvent RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_FAIL;
	sEvent RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_DONE;

	sEvent RTHD_GMAIN_SW1_SEND_SNAP_POS;
	sEvent GMAIN_RTHD_SW1_SEND_SNAP_POS_DONE;
	sEvent RTHD_GMAIN_SW2_FACET_SEND_SNAP_POS;
	sEvent GMAIN_RTHD_SW2_FACET_SEND_SNAP_POS_DONE;
	sEvent RTHD_GMAIN_SW3_FACET_SEND_SNAP_POS;
	sEvent GMAIN_RTHD_SW3_FACET_SEND_SNAP_POS_DONE;

	sEvent RPNP1_ROUT_THREAD_AWAY_OUTPUT;
	sEvent RPNP2_ROUT_THREAD_AWAY_OUTPUT;

	sEvent RPNP1_ROUT_THREAD_AWAY_INPUT;
	sEvent RPNP2_ROUT_THREAD_AWAY_INPUT;

	sEvent START_PNP_Calibration;
	sEvent PNP_CALIBRATION_WRITE_REPORT;

	sEvent RMAIN_GMAIN_RESET_S2S3_VISION_RESULT;
	sEvent RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE;

	sEvent RTHD_GMAIN_UPDATE_REAL_TIME_YEILD;
	sEvent RTHD_GMAIN_START_REAL_TIME_UPH;

	sEvent RMAIN_GMAIN_LOW_YIELD_ALARM_TRIGGER;

#pragma endregion
};
class RProduct_API ProductSharedMemoryTeachPoint : public SharedMemoryTeachPoint
{
public:
	signed long InputTrayTableXAxisForwardLimitPosition;
	signed long InputTrayTableXAxisReverseLimitPosition;
	signed long InputTrayTableXAxisAtInputTrayStackerLoadPosition;
	signed long InputTrayTableXAxisAtInputTrayStackerUnloadPosition;
	signed long InputTrayTableXAxisAtInputTrayTableCenterPosition;
	signed long InputTrayTableXAxisAtInputSideWallVisionDotGridPosition;
	signed long InputTrayTableXAxisAtInputBottomAndVisionDotGridPosition;

	signed long InputTrayTableYAxisForwardLimitPosition;
	signed long InputTrayTableYAxisReverseLimitPosition;
	signed long InputTrayTableYAxisAtInputTrayStackerLoadPosition;
	signed long InputTrayTableYAxisAtInputTrayStackerUnloadPosition;
	signed long InputTrayTableYAxisAtInputTrayTableCenterPosition;
	signed long InputTrayTableYAxisAtInputBottomDotGridPosition;
	signed long InputTrayTableYAxisAtInputVisionCalibrationPosition;

	signed long InputTrayTableZAxisForwardLimitPosition;
	signed long InputTrayTableZAxisReverseLimitPosition;
	signed long InputTrayTableZAxisDownPosition;
	signed long InputTrayTableZAxisAtInputTrayStackerLoadingPosition;
	signed long InputTrayTableZAxisAtInputTrayStackerSingulationPosition;
	signed long InputTrayTableZAxisAtInputTrayStackerSecondSingulationPosition;
	signed long InputTrayTableZAxisAtInputTrayStackerUnloadingPosition;

	signed long OutputTrayTableXAxisForwardLimitPosition;
	signed long OutputTrayTableXAxisReverseLimitPosition;
	signed long OutputTrayTableXAxisAtOutputTrayStackerLoadPosition;
	signed long OutputTrayTableXAxisAtOutputTrayStackerUnloadPosition;
	signed long OutputTrayTableXAxisAtOutputTrayTableCenterPosition;
	signed long OutputTrayTableXAxisAtRejectTrayCenterPosition;
	signed long OutputTrayTableXAxisAtOutputTrayTableManualLoadUnloadPosition;
	signed long OutputTrayTableXAxisAtOutputVisionCalibrationPosition;

	signed long OutputTrayTableYAxisForwardLimitPosition;
	signed long OutputTrayTableYAxisReverseLimitPosition;
	signed long OutputTrayTableYAxisAtOutputTrayStackerLoadPosition;
	signed long OutputTrayTableYAxisAtOutputTrayStackerUnloadPosition;
	signed long OutputTrayTableYAxisAtOutputTrayTableCenterPosition;
	signed long OutputTrayTableYAxisAtRejectTrayCenterPosition;
	signed long OutputTrayTableYAxisAtOutputTrayTableManualLoadUnloadPosition;
	signed long OutputTrayTableYAxisAtOutputVisionCalibrationPosition;

	signed long OutputTrayTableZAxisForwardLimitPosition;
	signed long OutputTrayTableZAxisReverseLimitPosition;
	signed long OutputTrayTableZAxisDownPosition;
	signed long OutputTrayTableZAxisAtOutputTrayStackerLoadingPosition;
	signed long OutputTrayTableZAxisAtOutputTrayStackerSingulationPosition;
	signed long OutputTrayTableZAxisAtOutputTrayStackerSecondSingulationPosition;
	signed long OutputTrayTableZAxisAtOutputTrayStackerUnloadingPosition;

	signed long PickAndPlace1XAxisForwardLimitPosition;
	signed long PickAndPlace1XAxisReverseLimitPosition;
	signed long PickAndPlace1XAxisInputPosition;
	signed long PickAndPlace1XAxisAtS1AndBottomVisionPosition;
	signed long PickAndPlace1XAxisAtS2AndS3VisionPosition;
	signed long PickAndPlace1XAxisOutputPosition;
	signed long PickAndPlace1XAxisParkingPosition;
	signed long PickAndPlace1XAxisAwayOutputPosition;
	signed long PickAndPlace1XAxisAwayInputPosition;

	signed long PickAndPlace1YAxisForwardLimitPosition;
	signed long PickAndPlace1YAxisReverseLimitPosition;
	signed long PickAndPlace1YAxisInputPosition;
	signed long PickAndPlace1YAxisAtS1AndBottomVisionPosition;
	signed long PickAndPlace1YAxisAtS2AndS3VisionPosition;
	signed long PickAndPlace1YAxisOutputPosition;
	signed long PickAndPlace1YAxisStandbyPosition;

	signed long PickAndPlace1ZAxisForwardLimitPosition;
	signed long PickAndPlace1ZAxisReverseLimitPosition;
	signed long PickAndPlace1ZAxisUpPosition;
	signed long PickAndPlace1ZAxisUpPosition1;
	signed long PickAndPlace1ZAxisUpPosition2;
	signed long PickAndPlace1ZAxisAtInputTrayTouchingPosition;
	signed long PickAndPlace1ZAxisAtInputTrayDownPosition;
	signed long PickAndPlace1ZAxisAtInputTraySoftlandingPosition;
	signed long PickAndPlace1ZAxisAtOutputTrayTouchingPosition;
	signed long PickAndPlace1ZAxisAtOutputTrayDownPosition;
	signed long PickAndPlace1ZAxisAtOutputTraySoftlandingPosition;
	signed long PickAndPlace1ZAxisAtVisionFocusPosition;

	signed long PickAndPlace1ThetaAxisForwardLimitPosition;
	signed long PickAndPlace1ThetaAxisReverseLimitPosition;
	signed long PickAndPlace1ThetaAxisStandbyPosition;

	signed long PickAndPlace2XAxisForwardLimitPosition;
	signed long PickAndPlace2XAxisReverseLimitPosition;
	signed long PickAndPlace2XAxisInputPosition;
	signed long PickAndPlace2XAxisAtS1AndBottomVisionPosition;
	signed long PickAndPlace2XAxisAtS2AndS3VisionPosition;
	signed long PickAndPlace2XAxisOutputPosition;
	signed long PickAndPlace2XAxisParkingPosition;
	signed long PickAndPlace2XAxisAwayOutputPosition;
	signed long PickAndPlace2XAxisAwayInputPosition;


	signed long PickAndPlace2YAxisForwardLimitPosition;
	signed long PickAndPlace2YAxisReverseLimitPosition;
	signed long PickAndPlace2YAxisInputPosition;
	signed long PickAndPlace2YAxisAtS1AndBottomVisionPosition;
	signed long PickAndPlace2YAxisAtS2AndS3VisionPosition;
	signed long PickAndPlace2YAxisOutputPosition;
	signed long PickAndPlace2YAxisStandbyPosition;

	signed long PickAndPlace2ZAxisForwardLimitPosition;
	signed long PickAndPlace2ZAxisReverseLimitPosition;
	signed long PickAndPlace2ZAxisUpPosition;
	signed long PickAndPlace2ZAxisUpPosition1;
	signed long PickAndPlace2ZAxisUpPosition2;
	signed long PickAndPlace2ZAxisAtInputTrayTouchingPosition;
	signed long PickAndPlace2ZAxisAtInputTrayDownPosition;
	signed long PickAndPlace2ZAxisAtInputTraySoftlandingPosition;
	signed long PickAndPlace2ZAxisAtOutputTrayTouchingPosition;
	signed long PickAndPlace2ZAxisAtOutputTrayDownPosition;
	signed long PickAndPlace2ZAxisAtOutputTraySoftlandingPosition;
	signed long PickAndPlace2ZAxisAtVisionFocusPosition;

	signed long PickAndPlace2ThetaAxisForwardLimitPosition;
	signed long PickAndPlace2ThetaAxisReverseLimitPosition;
	signed long PickAndPlace2ThetaAxisStandbyPosition;

	signed long InputVisionZAxisForwardLimitPosition;
	signed long InputVisionZAxisReverseLimitPosition;
	signed long InputVisionZAxisAtInputVisionFocusPosition;

	signed long S2VisionForwardLimitPosition;
	signed long S2VisionReverseLimitPosition;
	signed long S2VisionFocusPosition;

	signed long S1VisionForwardLimitPosition;
	signed long S1VisionReverseLimitPosition;
	signed long S1VisionFocusPosition;
	signed long S1VisionRetractPosition;

	signed long S3VisionForwardLimitPosition;
	signed long S3VisionReverseLimitPosition;
	signed long S3VisionFocusPosition;
};
class RProduct_API ProductSharedMemoryProduction : public SharedMemoryProduction
{
public:
	int PickUpHeadCount[2];

	int PickUpHeadNoForCalibration;

	int PickAndPlace1CurrentStation;
	int PickAndPlace2CurrentStation;
	int PickAndPlace1StationToMove;
	int PickAndPlace2StationToMove;

	int nCurrentPickupHeadAtInput;
	int nCurrentPickupHeadAtS1;
	int nCurrentPickupHeadAtS3;
	int nCurrentPickupHeadAtOutput;

	StationResult PickAndPlacePickUpHeadStationResult[2];

	StationResult InputTableResult[1];
	StationResult OutputTableResult[1];	

	LookUpTableOffsetData PickAndPlaceLookUpTableData1[360];
	LookUpTableOffsetData PickAndPlaceLookUpTableData2[360];
	LookUpTableOffsetData LookUpTableCollectUp1;
	LookUpTableOffsetData LookUpTableCollectDown1;
	LookUpTableOffsetData LookUpTableCollectUp2;
	LookUpTableOffsetData LookUpTableCollectDown2;
	
	int OutputTableCheckSequence;

	int nPreviousInputTrayNo;
	int nCurrentInputTrayNo;
	int nCurrentBottomStationTrayNo;
	int nCurrentS3StationTrayNo;
	int nCurrentOutputTrayNo;
	int nCurrentRejectTrayNo;

	int nCurrentProcessRejectTrayNo; //0 = reject tray 5 , 5 = output tray
	int nCurrentInspectionStationBottom; //1=Bottom 2=Setup 3=SW1
	int nCurrentInspectionStationS2S3; //1=S2S3_Parting 2=S2S3_Facet

	int InputTrayUnloadingNo = 0;
	int OutputTrayUnloadingNo = 0;

	int CurrentInputVisionRetryCount;
	int CurrentInputVisionContinuousFailCount;
	int CurrentInputVisionRCRetryCount;

	int CurrentS2VisionRetryCount;
	int CurrentS2VisionContinuousFailCount;
	int CurrentS2VisionRCRetryCount;

	int CurrentBottomVisionRetryCount;
	int CurrentBottomVisionContinuousFailCount;
	int CurrentBottomVisionRCRetryCount;

	int CurrentS3VisionRetryCount;
	int CurrentS3VisionContinuousFailCount;
	int CurrentS3VisionRCRetryCount;

	int CurrentS1VisionRetryCount;
	int CurrentS1VisionContinuousFailCount;
	int CurrentS1VisionRCRetryCount;

	int CurrentOutputVisionRetryCount;
	int CurrentOutputVisionContinuousFailCount;
	int CurrentOutputVisionRCRetryCount;

	int CurrentOutputVisionNotMatchUnitRetryCount;

	int nEdgeCoordinateX;
	int nEdgeCoordinateY;

	int nContinuouslyEmptyPocket;

	int nOutputEdgeCoordinateX;
	int nOutputEdgeCoordinateY;

	int nRejectEdgeCoordinateX;
	int nRejectEdgeCoordinateY;

	signed long InputTrayTableCurrentXPosition;
	signed long InputTrayTableCurrentYPosition;

	signed long OutputTrayTableCurrentXPosition;
	signed long OutputTrayTableCurrentYPosition;

	signed long RejectTrayTableCurrentXPosition;
	signed long RejectTrayTableCurrentYPosition;

	signed long InputTrayTableXIndexPosition; // w/o offset pos
	signed long InputTrayTableYIndexPosition;
	signed long OutputTrayTableXIndexPosition;
	signed long OutputTrayTableYIndexPosition;
	signed long OutputTrayTableXTempPosition;
	signed long OutputTrayTableYTempPosition;
	signed long RejectTrayTableXIndexPosition;
	signed long RejectTrayTableYIndexPosition;

	signed long PickAndPlace1XAxisMovePosition;
	signed long PickAndPlace2XAxisMovePosition;

	signed long PickAndPlace1XAxisMovePosition_BeforeMove;
	signed long PickAndPlace2XAxisMovePosition_BeforeMove;

	signed long PickAndPlace1YAxisMovePosition;
	signed long PickAndPlace1YAxisMovePosition_BeforeMove;

	signed long InputTrayTableXAxisMovePosition;
	signed long InputTrayTableYAxisMovePosition;
	signed long InputTrayTableZAxisMovePosition;

	signed long PickAndPlace2YAxisMovePosition;
	signed long PickAndPlace2YAxisMovePosition_BeforeMove;

	signed long OutputTrayTableXAxisMovePosition;
	signed long OutputTrayTableYAxisMovePosition;
	signed long OutputTrayTableZAxisMovePosition;

	signed long InputVisionModuleMovePosition;
	signed long InputVisionModuleCurrentPosition;
	signed long S2VisionModuleMovePosition;
	signed long S1VisionModuleMovePosition;
	signed long S1VisionModuleMoveThetaCurrentPosition;
	signed long S3VisionModuleMovePosition;

	signed long PickAndPlace1ZAxisMovePosition;
	signed long PickAndPlace1ThetaAxisMovePosition;
	signed long PickAndPlace1ThetaAxisMovePosition_BeforeMove;
	signed long PnP1AngleMaintainance;
	signed long PickAndPlace2ZAxisMovePosition;
	signed long PickAndPlace2ThetaAxisMovePosition;
	signed long PickAndPlace2ThetaAxisMovePosition_BeforeMove;
	signed long PnP2AngleMaintainance;

	signed long BottomXAxisoffsetMaintainance;
	signed long BottomYAxisoffsetMaintainance;
	signed long PnPCurrentAngle;


	double THK1CurrentPressureValue;
	double THK1CurrentForceValue;
	double THK1CurrentFlowRate;

	double THK2CurrentPressureValue;
	double THK2CurrentForceValue;
	double THK2CurrentFlowRate;

	bool IsInputVisionSendRC, IsOutputVisionSendRC;
	bool IsInputVisionRetryXYT, IsOutputVisionRetryXYT;
	bool bIsOutputFirstUnit, bIsRejectFirstUnit;
	bool IsMissingPostOutput, IsMissingPostReject;

	signed long InputXOffset;
	signed long InputYOffset;
	signed long InputThetaOffset;

	double dbCurrentOffset1X;
	double dbCurrentOffset1Y;
	double dbPreviousOffset1X;
	double dbPreviousOffset1Y;
	double dbCurrentOffset2X;
	double dbCurrentOffset2Y;
	double dbPreviousOffset2X;
	double dbPreviousOffset2Y;

	double dbCurrentOffset1Theta;
	double dbCurrentOffset2Theta;

	signed long slCurrentXPosition;
	signed long slCurrentYPosition;

	int nCurrentTotalUnitDone;
	int nCurrrentTotalUnitDoneByLot;
	int nCurrentOutputUnitOnTray;
	int nCurrentRejectUnitOnTray;

	int nCurrentTotalRejectUnit;

	int nCurrentInputUnitOnTray;

	int nCurrentTotalInputUnitDone;

	int TrayPresentSensorOffTimeBeforeAlarm_ms;

	int BypassCheckPostVisionResultForFirstTime;

	int CurrentBottomVisionLoopNo;
	int CurrentS1VisionLoopNo;
	int CurrentS3VisionLoopNo;
	int CurrentS2VisionLoopNo;
	int CurrentInputVisionLoopNo;
	int CurrentS2S3FacetTotalSnap;
	int CurrentS2FacetSnapTimes;
	int CurrentS3FacetSnapTimes;

	int PickupHeadNoAtInputVision;
	int	PickupHeadNoAtS2Vision;
	int	PickupHeadNoAtSetupVision;
	int	PickupHeadNoAtBottomVision;
	int	PickupHeadNoAtS3Vision;
	int	PickupHeadNoAtSidewallLeftVision;
	int	PickupHeadNoAtSidewallRightVision;
	int	PickupHeadNoAtSidewallFrontVision;
	int	PickupHeadNoAtSidewallRearVision;

	int TeachVisionStation;
	int TeachVisionPickAndPlace;

	signed long PickAndPlace1XAxisEncoderPosition;
	signed long PickAndPlace1YAxisEncoderPosition;
	signed long PickAndPlace1ThetaAxisEncoderPosition;
	signed long PickAndPlace1ZAxisEncoderPosition;

	signed long PickAndPlace2XAxisEncoderPosition;
	signed long PickAndPlace2YAxisEncoderPosition;
	signed long PickAndPlace2ThetaAxisEncoderPosition;
	signed long PickAndPlace2ZAxisEncoderPosition;

	signed long InputVisionZAxisEncoderPosition;
	signed long S2VisionZAxisEncoderPosition;
	signed long S3VisionZAxisEncoderPosition;
	
	signed long S1VisionZAxisEncoderPosition;

	signed long OutputThetaOffset;

	int nInputVisionAdditionalSnapNo;
	int nS2VisionAdditionalSnapNo;
	int nS1VisionAdditionalSnapNo;
	int nS3VisionAdditionalSnapNo;
	int nOutputVisionAdditionalSnapNo;

	char InputLotID[100];
	char OutputLotID[100];

	char CurrentInputLotID[100];
	char CurrentS1LotID[100];
	char CurrentS3LotID[100];
	char CurrentOutputLotID[100];

	int nCurrentPickAndPlace1PickingRetry;
	int nCurrentPickAndPlace2PickingRetry;

	int nCurrentPickAndPlace1PlacingRetry;
	int nCurrentPickAndPlace2PlacingRetry;

	char PreviousInputLotID[100];
	//int PreviousTotalQuantityDone;
	//int PreviousInputTrayNo;

	bool IsS1VisionFistSnap;
	bool IsS2VisionFistSnap;
	bool IsS3VisionFistSnap;
	bool IsBottomVisionFistSnap;

	//int PreviousOutputTrayNo;
	//int PreviousReject1TrayNo;
	//int PreviousReject2TrayNo;
	//int PreviousReject3TrayNo;
	//int PreviousReject4TrayNo;
	//int PreviousReject5TrayNo;

	int WriteReportTrayNoOnOutput;
	int WriteReportTrayNo;
	int PreviousInputEdgeCoordinateX;
	int PreviousInputEdgeCoordinateY;
	//int PreviousOutputEdgeCoordinateX;
	//int PreviousOutputEdgeCoordinateY;
	int PreviousRejectEdgeCoordinateX;
	int PreviousRejectEdgeCoordinateY;

	bool bIsDiffuserOn;

	bool bInputContinue;
	bool bOutputContinue;
	bool LastUnit;
	bool LastUnitOutput;
	bool PendingLastUnitOutput;
	bool IsPostDone;

	int UpdateMappingProgressHead;
	int StationSelectedForCalibration; // 1 = input, 2 = s1, 3 = s2s3, 4 = Output
	int nInputRunningState = 0;
	int nPNPRunningState = 0;
	int nOutputRunningState = 0;

	int nInputLotQuantity = 0;
	int nCurrentInputLotQuantityRun = 0;
	int nTotalInputUnitDone = 0;

	int nLotIDNumber = 0;

	int nCurrentLotGoodQuantity = 0;
	int nCurrentLotNotGoodQuantity = 0;

	bool IsUpdateMESAgain = false;

	int nTotalSamplingQty = 0;
	int nInputLotTrayNo = 0;
	int nCurrentInputLotTrayNoRun = 0;

	bool bPNP1AllowS2S3Snap;
	bool bPNP2AllowS2S3Snap;

	bool bPNP1AllowInputSnap;
	bool bPNP2AllowInputSnap;
	bool bAllowInputSnap;

	bool bolIsLastUnitTo1EndTray;
	bool bolIsLastUnitTo2EndTray;
	bool bolIsLastUnitToEndTray;

	int nCurrentInputTrayNumberAtBottom = 0;
	int nCurrentInputTrayNumberAtS2S3 = 0;
	int nCurrentInputTrayNumberAtOutput = 0;

	int nCurrentInputQuantity = 0;
	int nCurrentOutputQuantity = 0;
	int nCurrentRejectQuantity = 0;
	int nCurrentLowYieldAlarmQuantity = 0;
	int nCurrentRejectQuantityBasedOnInputTray = 0;

	int nCurrentSetupFailValue = 0;

	bool MessageForFirstTime_PnP1 = false;
	bool MessageForFirstTime_PnP2 = false;
	bool MessageForFirstTime_Input = false;
	bool MessageForFirstTime_Output = false;
	bool MessageForFirstTime_Setup = false;
	bool MessageForFirstTime_Job = false;

	int nCurrentS2FacetVisionRetryCount = 0;
};
class RProduct_API ProductSharedMemoryGeneral : public SharedMemoryGeneral
{
public:
	LARGE_INTEGER lnClockGlobalStart;
#pragma region TeachPointProtection
	int nXMotorEncoderPosition;
	int nYMotorEncoderPosition;
#pragma endregion
};

class RProduct_API ProductSharedMemoryIO : public SharedMemoryIO
{
public:
	unsigned long ucInput[NUMBER_IO_CARD];
	unsigned long ucOutput[NUMBER_IO_CARD];
	unsigned long nArrayOutput[NUMBER_IO_CARD];
};

#endif
ProductSharedMemorySetting *smProductSetting = NULL;
ProductSharedMemoryTeachPoint *smProductTeachPoint = NULL;
ProductSharedMemoryProduction *smProductProduction = NULL;
ProductSharedMemoryCustomize *smProductCustomize = NULL;
ProductSharedMemoryIO *smProductIO = NULL;
ProductSharedMemoryModuleStatus *smProductModuleStatus = NULL;
ProductSharedMemoryEvent *smProductEvent = NULL;
ProductSharedMemoryGeneral *smProductGeneral = NULL;

