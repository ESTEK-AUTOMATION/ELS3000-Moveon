//////////////////////////////////////////////////////////////////
//
// MotionLibrary.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 9/18/2017 11:54:17 AM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////

#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS
#ifndef __MOTIONLIBRARY_H_INCLUDED__
#define __MOTIONLIBRARY_H_INCLUDED__
#ifndef _INC_SDKDDKVER
#include <SDKDDKVer.h>
#endif
#ifndef _WS2TCPIP_H_
#include <ws2tcpip.h>
#endif
#ifndef BASEMOTIONLIBRARY_H
#include "BaseMotionLibrary.h"
#endif
//#include <stdio.h>
//#include <string.h>
//#include <ctype.h>
//#include <conio.h>
//#include <stdlib.h>
//#include <math.h>
//#include <errno.h>
#include <string.h>
#ifndef _WINDOWS_
#include <windows.h>
#endif
#ifndef _INC_TCHAR
#include <tchar.h>
#endif
#ifndef _ARRAY_
#include <array>
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

//#ifndef GALILCONTROLLER_H
//#include "GalilController.h"
//#endif
//#ifndef ETELCONTROLLER_H
//#include "EtelController.h"
//#endif
//#ifndef KINGSTARCONTROLLER_H
//#include "KingstarController.h"
//#endif



#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define MotionLibrary_API __declspec(dllexport)
  #else
    #define MotionLibrary_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define MotionLibrary_API __declspec(dllexport)
  #else
    #define MotionLibrary_API __declspec(dllimport)
  #endif
#endif

struct testing{
public:
	LPSTR Controller;
	LPSTR ControllerDetails;
	bool Enable;
	bool Online;
	//string Axis;		
    double UnitInOnePulse;//micron or degree
    //string UnitName;
    int AxisInPositionToleranceInUnitPulse;
};

// This class is exported from the MotionLibrary.dll
//extern "C"
class MotionLibrary_API CMotionLibrary //: public aMotionLibrary
{
public:
	int m_nErrorCode;
    LPSTR m_strErrorMsg;
	bool m_bOnline;
	bool m_bEnableDebug;
	MotorConfiguration m_MotorConfiguration[MOTORQUANTITY];
#ifdef INSTALL_GALIL_DRIVER
	CGalilController *m_galilController1;
	CGalilController *m_galilController2;
	CGalilController *m_galilController3;
	CGalilController *m_galilController4;
	//1.0.0.1a Charles
	CGalilController *m_galilController5;
	CGalilController *m_galilController6;
	//--
#endif
#ifdef INSTALL_ETEL_DRIVER
	CEtelController *m_etelController;
#endif
#ifdef INSTALL_KINGSTAR_DRIVER
	CKingstarController *m_kingstarController;
#endif

private:
	int m_nNoOfMotorHasBeenConfigured;
public:
	
    CMotionLibrary(void);
	~CMotionLibrary(void);
    
	virtual int CMotionLibrary::Configure(bool Online);
	virtual int CMotionLibrary::Initialize();
	virtual int CMotionLibrary::GetLastErrorCode();
	virtual LPCTSTR CMotionLibrary::GetErrorMsg(int errorCode);
	virtual void CMotionLibrary::ClearError();
	virtual int CMotionLibrary::Initialize(bool online, MotorConfiguration* motorConfiguration, int motorQuantity);
	virtual int CMotionLibrary::Initialize(bool online, bool enableDebug, MotorConfiguration* motorConfiguration, int motorQuantity);
	virtual int CMotionLibrary::Reset(char *axis);
	virtual int CMotionLibrary::Connect(char *axis);
	virtual int CMotionLibrary::Disconnect(char *axis);
	virtual int CMotionLibrary::MotorOff(char *axis);
	virtual int CMotionLibrary::MotorOn(char *axis);
	virtual int CMotionLibrary::MotorHome(char *axis);
	virtual int CMotionLibrary::MotorHomeAsychronous(char *axis);
	virtual int CMotionLibrary::MotorStop(char *axis);
	//Galil not supported
	virtual int CMotionLibrary::MoveRelative(char *axis, MotorProfile motionProfile);
	//Galil not supported
	virtual int CMotionLibrary::MoveRelative(char *axis, signed long positionValue);
	//Galil:convert to pulse base on UnitInOnePulse
	virtual int CMotionLibrary::MoveRelativeAsychronous(char *axis, MotorProfile motionProfile);
	//Galil:base on UnitInOnePulse
	virtual int CMotionLibrary::MoveRelativeAsychronous(char *axis, double positionValue);
	//Galil not supported
	virtual int CMotionLibrary::MoveAbsolute(char *axis, MotorProfile motionProfile);	
	//Galil not supported
	virtual int CMotionLibrary::MoveAbsolute(char *axis, signed long positionValue);
	//Galil:convert to pulse base on UnitInOnePulse
	virtual int CMotionLibrary::MoveAbsoluteAsychronous(char *axis, MotorProfile motionProfile);
	//Galil:base on UnitInOnePulse
	virtual int CMotionLibrary::MoveAbsoluteAsychronous(char *axis, double positionValue);
	//Galil not supported
	int CMotionLibrary::WaitMovementComplete(char *axis);
	//Galil not supported
	int CMotionLibrary::WaitMovementComplete(char *axis, long timeout);
	virtual int CMotionLibrary::StopMovement(char *axis);
	//Galil not supported
	virtual bool CMotionLibrary::InPosition(char *axis);
	//Etel not supported
	virtual bool CMotionLibrary::IsForwardLimitOn(char *axis);
	//Etel not supported
	virtual bool CMotionLibrary::IsReverseLimitOn(char *axis);
	//Etel not supported
	virtual bool CMotionLibrary::IsHomeSensorOn(char *axis);
	bool CMotionLibrary::IsMotorOn(char *axis);
	bool CMotionLibrary::IsMotorMoving(char *axis);
	//Galil not supported
	bool CMotionLibrary::IsMotorError(char *axis);
	int CMotionLibrary::SetSpeed(char *axis, double speedValue);
	int CMotionLibrary::SetAcceleration(char *axis, double accelerationValue);
	int CMotionLibrary::SetDeceleration(char *axis, double decelerationValue);
	int CMotionLibrary::SetCAM(char *axis, double percentageValue);
	//Galil not supported, unit in s
	int CMotionLibrary::SetJerkTime(char *axis, double jerkTime);
	virtual signed long CMotionLibrary::GetEncoderPosition(char *axis);
	virtual int CMotionLibrary::GetEncoderPosition(char *axis, double* positionValue);
	virtual signed long CMotionLibrary::GetCommandPosition(char *axis);
	virtual int CMotionLibrary::GetCommandPosition(char *axis, double* positionValue);
	int CMotionLibrary::GetSpeed(char *axis, double* speedValue);
	int CMotionLibrary::GetAcceleration(char *axis, double* accelerationValue);
	int CMotionLibrary::GetDeceleration(char *axis, double* decelerationValue);
	int CMotionLibrary::GetCAM(char *axis, double* percentageValue);
	//Galil not supported
	int CMotionLibrary::GetJerkTime(char *axis, double* jerkTime);
	//Galil not supported
	int CMotionLibrary::GetCurrentValueInA(char *axis, double* currentValue);

	//Galil/Etel not supported
	int CMotionLibrary::GetInputByte(char *axis, int index, int byteOffset, BYTE* value);
	int CMotionLibrary::SetOutputByte(char *axis, int index, int byteOffset, BYTE value);
	int CMotionLibrary::GetInputWord(char *axis, int index, int byteOffset, WORD* value);
	int CMotionLibrary::SetOutputWord(char *axis, int index, int byteOffset, WORD value);

	//Etel not supported
	virtual int CMotionLibrary::SetControllerVariable(char *controllerDetails, char *name, signed long value);
	//Etel not supported
	virtual int CMotionLibrary::SendCommand(char *controllerDetails, char *command);
	//Etel not supported
	virtual int CMotionLibrary::SendReceiveCommand(char *controllerDetails, char *command, char *receive);
	int CMotionLibrary::Test(testing* motorConfiguration);
private:
	LPSTR CMotionLibrary::GetControllerDetailsControllerNo(char *ControllerDetailsIPAddress);
	LPSTR CMotionLibrary::GetControllerDetailsIPAddress(char *ControllerDetailsIPAddress);
	LPSTR CMotionLibrary::GetControllerDetailsMotorAxis(char *ControllerDetailsIPAddress);
	LPSTR CMotionLibrary::GetControllerDetails(char *ControllerDetailsIPAddress, int DetailNo);
};
extern MotionLibrary_API int nMotionLibrary;


// function prototype for the RTDLL exported function
//MotionLibrary_API
MotionLibrary_API
int 
RTAPI
Toggle(int argc, TCHAR * argv[]);

MotionLibrary_API
int 
RTAPI
Test2(int input);
#endif