#pragma once
#ifndef __CPLATFORMSHAREVARIABLES_H_INCLUDED__ 
#define __CPLATFORMSHAREVARIABLES_H_INCLUDED__

#ifndef __CPLATFORMSHAREDMEMORY_H_INCLUDED__ 
#include "CPlatformSharedMemory.h"
#endif

#ifndef __CLOGGER_H_INCLUDED__
#include "CLogger.h"
#endif

#include "RPlatform.h"

#define HIGHESTPRIORITY  (RT_PRIORITY_MIN + 3)
#define HIGHPRIORITY     (RT_PRIORITY_MIN + 2)

class RPlatform_API CPlatformShareVariables
{
private:
	CRITICAL_SECTION csDisplayLog;
public:
	#pragma region Timer Variables
	LARGE_INTEGER  m_lnPeriod_100us;
	LARGE_INTEGER  m_lnPeriod_1ms;
	LARGE_INTEGER m_lnPeriod_10ms;
	LARGE_INTEGER  m_lnPeriod_100ms;
	LARGE_INTEGER  m_lnPeriod_500ms;
	LARGE_INTEGER  m_lnPeriod_1s;
	LARGE_INTEGER lnClockStartDebug, lnClockEndDebug, lnClockSpanDebug;
	LONGLONG m_TimeCount = 10000; //10000000
	#pragma endregion

	#pragma region ErrorCode
	int Error_Exception = -1;
	int Error_Setting = 1;
	int Error_Disable = 2;
	int Error_Motor = 3;
	int Error_MotionTimeOut = 4;
	int Error_MotionStartTimeOut = 5;
	int Error_MotorBusy = 6;
	int Error_MotorNotHomed = 7;
	int Error_SendCommand = 8;
	int Error_Abort = 9;
	int Error_MotorPositionOutOfLimit = 10;
	int Error_MotorNotSafeToMove = 11;

	#pragma endregion

	#pragma region Timeout
	long HOMING_TIMEOUT = 60000;//60000
	long MOTION_TIMEOUT = 30000;
	long COMMAND_TIMEOUT = 10000;
	long CYLINDER_TIMEOUT = 5000;
	long VISION_TIMEOUT = 5000;//60000;
	#pragma endregion

	CPlatformShareVariables();
	~CPlatformShareVariables();
	virtual int RTFCNDCL CPlatformShareVariables::SetPlatformShareVariables(CPlatformShareVariables *platformShareVariables);
	void CPlatformShareVariables::SetAlarm(int alarmID);
	bool CPlatformShareVariables::UpdateMessageToGUIAndLog(char *message, ...);
	int CPlatformShareVariables::triggerUpdateMessage();
};

CPlatformShareVariables *m_cPlatformShareVariables = new CPlatformShareVariables();
#endif