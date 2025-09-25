//////////////////////////////////////////////////////////////////
//
// KingstarController.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 5/25/2021 3:56:09 PM
// User: Charles Chong
//
//////////////////////////////////////////////////////////////////
#ifndef KINGSTARCONTROLLER_H
#define KINGSTARCONTROLLER_H
#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS

#include <SDKDDKVer.h>

//#include <stdio.h>
//#include <string.h>
//#include <ctype.h>
//#include <conio.h>
//#include <stdlib.h>
//#include <math.h>
//#include <errno.h>
#include <windows.h>
#include <tchar.h>
//#include <rtapi.h>      // RTX64 APIs that can be used in real-time or Windows applications.
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

#ifdef INSTALL_KINGSTAR_DRIVER
#include "ksmotion.h"
#include "ksapi.h"
#endif

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define KingstarController_API __declspec(dllexport)
  #else
    #define KingstarController_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define KingstarController_API __declspec(dllexport)
  #else
    #define KingstarController_API __declspec(dllimport)
  #endif
#endif


// Add DEFINES Here
// This class is exported from the KingstarController.dll
class KingstarController_API CKingstarController
{

public:
	int m_nErrorCode;
	LPSTR m_strErrorMsg;
	bool m_bConnected;

public:
    CKingstarController(void);
	~CKingstarController(void);
    // TODO: add your methods here.

	int CKingstarController::KSInitialize();
	int CKingstarController::KSInitialize(bool debug, bool online);
	int CKingstarController::KSConfigure(bool debug, bool online, double cycleTime, int mode, bool enableAxisInput);
	int CKingstarController::KSConnect();
	int CKingstarController::KSDisconnect();
	int CKingstarController::KSReadInputByte(int Index, int ByteOffset, BYTE* Value);
	int CKingstarController::KSReadInputWord(int Index, int ByteOffset, WORD* Value);
	int CKingstarController::KSWriteOutputByte(int Index, int ByteOffset, BYTE Value);
	int CKingstarController::KSWriteOutputWord(int Index, int ByteOffset, WORD Value);

	int CKingstarController::KSTest();
};
extern KingstarController_API int nKingstarController;

#endif