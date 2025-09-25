#pragma once
#ifndef __CPLATFORMSHAREDMEMORY_H_INCLUDED__ 
#define __CPLATFORMSHAREDMEMORY_H_INCLUDED__

//#ifndef _WINDOWS_
//#include <windows.h>
//#endif

#include "RPlatform.h"

typedef struct sEvent
{
	bool Set;
	bool ManualReset;
	char Name;
}sEvent, *PEvent;

class RPlatform_API SharedMemorySetting
{
public:
	bool EnableOnlineMode;
};

class RPlatform_API SharedMemoryTeachPoint
{
};

class RPlatform_API SharedMemoryProduction
{
public:
	int nTime[100];
	int OutputQuantity;
};

class RPlatform_API SharedMemoryCustomize
{

};

class RPlatform_API SharedMemoryIO
{
public:

};

class RPlatform_API SharedMemoryModuleStatus
{
};

class RPlatform_API SharedMemoryEvent
{
public:
	sEvent ExitRTX;
	sEvent ExitRTXDone;
	sEvent RTXWaitThreadExitTimeout;
	sEvent Alarm;
	sEvent StepMode;
	sEvent IsStepButtonTrigger;
	sEvent GPCS_RSEQ_ABORT;
	sEvent RTXSoftwareInitializeDone;
	sEvent JobMode;
	sEvent PowerLost;
	sEvent CycleMode;

	sEvent Homed;
	sEvent StartPause;
	sEvent StartJob;
	sEvent StartReset;
	sEvent StartEnding;
	sEvent StartSetup;
	sEvent WaitingResponseDone;

	sEvent JobStart;
	sEvent JobPause;
	sEvent JobStop;
	sEvent JobStep;
	sEvent JobSlow;

	sEvent GMNL_RMNL_MANUAL_MODE;
	sEvent StartManualMode;
	sEvent StopManualMode;
	sEvent SeqGUISendMsgDone;
	sEvent SEQ_GUI_SEND_MSG_DONE;
	sEvent GUI_SEQ_RECEIVE_MSG_DONE;

	sEvent GTCH_RTCH_TEACH_MODE;
	sEvent GTCH_RTCH_TEACH_POINT;
	sEvent GTCH_RTCH_MOTOR_ON;
	sEvent GTCH_RTCH_MOTOR_OFF;
	sEvent GTCH_RTCH_MOTOR_HOME;
	sEvent GTCH_RTCH_MOTOR_MV_ABS;
	sEvent GTCH_RTCH_MOTOR_MV_REL;
	sEvent GTCH_RTCH_MOTOR_STOP;
	sEvent GTCH_RTCH_MOTOR_SPEED;
	sEvent GTCH_RTCH_MOTOR_SEMI_TEACH;

	sEvent GIO_RIO_IO_MODE;
	sEvent GIO_RIO_SET_OUTPUT;

	sEvent GPCS_RPCS_UPDATE_SETTING;
	sEvent GPCS_RPCS_UPDATE_TEACHPOINT;
	sEvent GPCS_RPCS_BYPASS_DOOR;

	sEvent GGUI_RSEQ_DRY_RUN_MODE;
	sEvent GGUI_RSEQ_CHECK_SEQUENCE;

	sEvent RSEQ_GGUI_UPDATE_MOTION_CHART;

	sEvent RTHD_RMAIN_MAIN_SEQ_END;
	sEvent RTHD_RMAIN_IO_SCAN_END;
	sEvent RTHD_RMAIN_IO_OPREATION_END;
	sEvent RTHD_RMAIN_TEACH_POINT_END;
	sEvent RTHD_RMAIN_MANUAL_END;
	sEvent RTHD_RMAIN_MAINTENANCE_END;
	sEvent RTHD_RMAIN_STATE_END;
	sEvent RTHD_RMAIN_AUTO_OPERATION_END;
	sEvent RTHD_RMAIN_DUMMY_END;
};

class RPlatform_API SharedMemoryGeneral
{
public:
	int State;
	int nStateCall;
	int nStatePrevious;
	int ManualID;
	int AlarmID;
	int MaintenanceID;
	
	#pragma region TeachPointProtection
	int updateMessage;
	char MessageStatus[1000];
	//char MessageStatus[512];
	#pragma endregion
	int nLoginAuthority;

	int nTeachPointMotorType;
	int nTeachPointCardNo;
	int nTeachPointAxis;
	int nTeachPointIndexNo;
	signed long nMotorMovePosition;
	int nMotorEncoderPosition;
	double dMotorMovePosition;
	double dMotorEncoderPosition;

	int nMotorForwardLimit;
	int nMotorReverseLimit;
	int nMotorHomeSensor;
	int nMotorOnStatus;
	int nMotorMovingStatus;
	int nMotorAlarmOn;
	int nMotorSpeedPercent;
	int nMotorAccelerationPercent;


};

SharedMemorySetting *smSetting = NULL;
SharedMemoryTeachPoint *smTeachPoint = NULL;
SharedMemoryProduction *smProduction = NULL;
SharedMemoryCustomize *smCustomize = NULL;
SharedMemoryIO *smIO = NULL;
SharedMemoryModuleStatus *smModuleStatus = NULL;
SharedMemoryEvent *smEvent = NULL;
SharedMemoryGeneral *smGeneral = NULL;

#endif
