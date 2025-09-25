//////////////////////////////////////////////////////////////////
//
// GalilController.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 9/29/2017 8:43:39 PM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////
#ifndef GALILCONTROLLER_H
#define GALILCONTROLLER_H
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

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define GalilController_API __declspec(dllexport)
  #else
    #define GalilController_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define GalilController_API __declspec(dllexport)
  #else
    #define GalilController_API __declspec(dllimport)
  #endif
#endif


// Add DEFINES Here
//TCP/IP
//*** define for the shutdown Handler
#define SHUTDOWN_PRIORITY	(RT_PRIORITY_MAX - 2)

#define	MAX_PACKET_SIZE		65535
#define DEFAULT_PACKET_SIZE 1024
#define MAX_TCP_LIMIT		1460
#define MAX_UDP_LIMIT		1472

// Number of times to loop before calculating
// and printing timing information
#define TIME_LOOP_SIZE 500


#define WFD_RECV_DATA 1
#define WFD_SEND_DATA 2

// Timeout = 1.5 seconds
#define RECV_TIMEOUT 10000
#define SEND_TIMEOUT 10000

#define RECV_BUF_ERROR		0
#define RECV_BUF_TIMEOUT	-1

// Kill request messages
#define KILL_REQUEST (-1)
#define KILL_ACK	 (-2)

//Default Family
#define DEFAULT_FAMILY     PF_INET // Accept either IPv4 or IPv6
//
// Function prototypes
//

// This class is exported from the GalilController.dll
class GalilController_API CGalilController
{
public:
	int m_nErrorCode;
	LPSTR m_strErrorMsg;
	bool m_bConnected;
private:
	int	iSocketType,
		iNagle,
		iLargePackets;
				
	int Family;

	#define TCP_PROVIDE_TIMESTAMPS 13

	SOCKET m_clientMC;
	char*	m_IPAddress;
	char m_chArrIPAddress[256];
	int m_nSocketType;
	int m_nPort;
	int m_nConnection;
	int m_nLargePackets;
	//1.0.0.0a
	int m_bEnableDebugLog;
	//--
public:
    CGalilController(void);
	~CGalilController(void);
    
	//acceleration, deceleration, speed & position are according to pulse

	int CGalilController::Initialize();
	//1.0.0.0a Charles
	int CGalilController::Initialize(bool enableDebugLog);
	//--
	int CGalilController::Connect(char *IPAddress, int PortNo);
	int CGalilController::Disconnect();
	int CGalilController::Reset();
	int CGalilController::Reset(char *axis);
	int CGalilController::MotorOff(char *axis);
	int CGalilController::MotorOn(char *axis);
	int CGalilController::MotorHomeAsychronous(char *axis);
	int CGalilController::MotorStop(char *axis);	
	int CGalilController::MoveRelativeAsychronous(char *axis, MotorProfile motionProfile);
	int CGalilController::MoveRelativeAsychronous(char *axis, double positionValue);
	//int CGalilController::MoveRelativeAsychronous(char *axis, signed long positionValue);
	int CGalilController::MoveAbsoluteAsychronous(char *axis, MotorProfile motionProfile);
	int CGalilController::MoveAbsoluteAsychronous(char *axis, double positionValue);
	//int CGalilController::MoveAbsoluteAsychronous(char *axis, signed long positionValue);
	//int CGalilController::StopMovement(char *axis);
	bool CGalilController::InPosition(char *axis);
	bool CGalilController::IsForwardLimitOn(char *axis);
	bool CGalilController::IsReverseLimitOn(char *axis);
	bool CGalilController::IsHomeSensorOn(char *axis);
	bool CGalilController::IsMotorOn(char *axis);
	bool CGalilController::IsMotorMoving(char *axis);
	int CGalilController::SetSpeed(char *axis, double speed);
	int CGalilController::SetAcceleration(char *axis, double accelerationValue);
	int CGalilController::SetDeceleration(char *axis, double delerationValue);
	//Percentage:1-100
	int CGalilController::SetCAM(char *axis, double percentageValue);
	//Not supported
	int CGalilController::SetJerkTime(char *axis, double jerkTime);
	//--
	signed long CGalilController::GetEncoderPosition(char *axis);
	int CGalilController::GetEncoderPosition(char *axis, double* positionValue);
	signed long CGalilController::GetCommandPosition(char *axis);
	int CGalilController::GetCommandPosition(char *axis, double* positionValue);
	int CGalilController::GetSpeed(char *axis, double* speedValue);
	int CGalilController::GetAcceleration(char *axis, double* accelerationValue);
	int CGalilController::GetDeceleration(char *axis, double* decelerationValue);
	int CGalilController::GetCAM(char *axis, double* percentage);

	int CGalilController::SetVariable(char *name, signed long value);
	int CGalilController::SendCommand(char *Command);
	int CGalilController::SendReceiveCommand(char *Command, char *Receive);
	int CGalilController::SendReceiveCommandWithClearBuffer(char *Command, char *Receive);

private:
	int CGalilController::ConnectTCPIP();
	int CGalilController::DisconnectTCPIP();
	int CGalilController::SendBuffer(SOCKET s, char *Buffer, int Length, int iFragmentBuffer, int iTimeOut);
	int CGalilController::RecvBuffer(SOCKET s, char *Buffer, int MaxLength, int iTimeOut);
	bool CGalilController::WaitForData(SOCKET pSocket, long iTimeOut, long WaitType );
	bool CGalilController::IsAccelerationValid(int accelerationValue);
	bool CGalilController::IsDecelerationValid(int decelerationValue);
	bool CGalilController::IsSpeedValid(int speedValue);
	bool CGalilController::IsPositionValid(int positionValue);
};
extern GalilController_API int nGalilController;

 
#endif