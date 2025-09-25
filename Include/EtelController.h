//////////////////////////////////////////////////////////////////
//
// EtelController.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 9/29/2017 8:43:39 PM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////
#ifndef ETELCONTROLLER_H
#define ETELCONTROLLER_H
#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS

#ifndef _INC_SDKDDKVER
#include <SDKDDKVer.h>
#endif
#include <stdio.h>
//#include <string.h>
//#include <ctype.h>
//#include <conio.h>
//#include <stdlib.h>
//#include <math.h>
//#include <errno.h>
//#include <winsock2.h>
#ifndef _WS2TCPIP_H_
#include <ws2tcpip.h>
#endif
#ifndef _WINDOWS_
#include <windows.h>
#endif
#ifndef _INC_TCHAR
#include <tchar.h>
#endif

#ifndef __WINRTXCONVERSION_H_INCLUDED__
#include "WinRTXConversion.h"
#endif
#ifndef _RTAPI_H_
#ifndef UNDER_WIN
#include <rtapi.h>      // RTX64 APIs that can be used in real-time or Windows applications.
#endif
#endif
#ifdef UNDER_RTSS
#include <rtssapi.h>    // RTX64 APIs that can only be used in real-time applications.
#endif // UNDER_RTSS

#ifndef MOTOR_H
#include "Motor.h"
#endif

#ifdef INSTALL_ETEL_DRIVER
#ifndef _DSA40_H
#include "dsa40.h"
#endif
#endif

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define EtelController_API __declspec(dllexport)
  #else
    #define EtelController_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define EtelController_API __declspec(dllexport)
  #else
    #define EtelController_API __declspec(dllimport)
  #endif
#endif


// Add DEFINES Here
//
// Function prototypes
//

// This class is exported from the GalilController.dll
class EtelController_API CEtelController
{
public:
	int m_nErrorCode;
	LPSTR m_strErrorMsg;
	bool m_bConnected;
	long MoveTimeout = 30000;
	long HomingTimeout = 30000;

	#pragma region Etel Control Variables
#ifdef INSTALL_ETEL_DRIVER
	DSA_STATUS EtelDriveStatus = { sizeof(DSA_STATUS) };
#endif
	#pragma endregion
private:
#ifdef INSTALL_ETEL_DRIVER
	DSA_MASTER *ultimet = NULL;
	DSA_DRIVE *(ptrArrDSA_DRIVE[MOTORQUANTITY]);
#endif

	double CAM_Full = 1;

	int m_bEnableDebugLog;

public:
	CEtelController(void);
	~CEtelController(void);
    // TODO: add your methods here.
	
	int CEtelController::Initialize();
	int CEtelController::Initialize(bool enableDebugLog);
	//int CEtelController::Connect(char *IPAddress, int PortNo);
	//Connect master and all axis until maximum no of axis
	int CEtelController::Connect();
	int CEtelController::Connect(char *axis);
	//int CEtelController::Disconnect();
	int CEtelController::Disconnect(char *axis);
	//Reset master and all axis until maximum no of axis
	int CEtelController::Reset();
	int CEtelController::ResetMaster();
	int CEtelController::Reset(char *axis);
	int CEtelController::MotorOff(char *axis);
	int CEtelController::MotorOn(char *axis);
	int CEtelController::MotorHome(char *axis);
	int CEtelController::MotorHomeAsychronous(char *axis);
	int CEtelController::MotorStop(char *axis);
	int CEtelController::MoveRelative(char *axis, MotorProfile motionProfile);
	//positionValue: meter or turn
	int CEtelController::MoveRelative(char *axis, double positionValue);
	int CEtelController::MoveRelativeAsychronous(char *axis, MotorProfile motionProfile);
	//positionValue: meter or turn
	int CEtelController::MoveRelativeAsychronous(char *axis, double positionValue);
	int CEtelController::MoveAbsolute(char *axis, MotorProfile motionProfile);
	//positionValue: meter or turn
	int CEtelController::MoveAbsolute(char *axis, double positionValue);
	int CEtelController::MoveAbsoluteAsychronous(char *axis, MotorProfile motionProfile);
	//positionValue: meter or turn
	int CEtelController::MoveAbsoluteAsychronous(char *axis, double positionValue);
	int CEtelController::WaitMovementComplete(char *axis);
	int CEtelController::WaitMovementComplete(char *axis, long timeout);
	bool CEtelController::InPosition(char *axis);
	bool CEtelController::IsForwardLimitOn(char *axis);
	bool CEtelController::IsReverseLimitOn(char *axis);
	bool CEtelController::IsHomeSensorOn(char *axis);
	bool CEtelController::IsMotorOn(char *axis);
	bool CEtelController::IsMotorMoving(char *axis);
	bool CEtelController::IsMotorError(char *axis);
	//speedValue: meter/s or turn/s
	int CEtelController::SetSpeed(char *axis, double speedValue);
	//accelerationValue: meter/s2 or turn/s2
	int CEtelController::SetAcceleration(char *axis, double accelerationValue);
	//decelerationValue: meter/s2 or turn/s2
	int CEtelController::SetDeceleration(char *axis, double decelerationValue);
	//100: percentage == 1.0
	int CEtelController::SetCAM(char *axis, double percentage);
	//jerkTime: s
	int CEtelController::SetJerkTime(char *axis, double jerkTime);
	//positionValue: meter or turn
	int CEtelController::GetEncoderPosition(char *axis, double* positionValue);
	//positionValue: meter or turn
	int CEtelController::GetCommandPosition(char *axis, double* positionValue);
	//speedValue: meter/s or turn/s
	int CEtelController::GetSpeed(char *axis, double* speedValue);
	//accelerationValue: meter/s2 or turn/s2
	int CEtelController::GetAcceleration(char *axis, double* accelerationValue);
	//decelerationValue: meter/s2 or turn/s2
	int CEtelController::GetDeceleration(char *axis, double* decelerationValue);
	//100: percentage == 1.0
	int CEtelController::GetCAM(char *axis, double* percentage);
	//jerkTime: s
	int CEtelController::GetJerkTime(char *axis, double* jerkTime);
	//currentValue: A
	int CEtelController::GetCurrentValueInA(char *axis, double* currentValue);

private:
	bool CEtelController::IsSuccessful(int errorCode);
	int CEtelController::ConnectMaster();
	int CEtelController::DisconnectMaster();
	int CEtelController::ReConnectMaster();
	bool CEtelController::IsAccelerationValid(int accelerationValue);
	bool CEtelController::IsDecelerationValid(int decelerationValue);
	bool CEtelController::IsSpeedValid(int speedValue);
	bool CEtelController::IsPositionValid(int positionValue);
};
extern EtelController_API int nEtelController;
 
#endif