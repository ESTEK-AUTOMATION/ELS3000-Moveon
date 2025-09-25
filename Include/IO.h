//////////////////////////////////////////////////////////////////
//
// IO.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 10/23/2017 9:55:23 AM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////

#pragma once
#ifndef __CIO_H_INCLUDED__ 
#define __CIO_H_INCLUDED__
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

#include "Vmio.h"
#include "vmio_define.h"
#include "vmio_typedef.h"
#include "vmio_error.h"

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define IO_API __declspec(dllexport)
  #else
    #define IO_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define IO_API __declspec(dllexport)
  #else
    #define IO_API __declspec(dllimport)
  #endif
#endif

// Add DEFINES Here
//1.0.0.3
//#define NUMBER_IO_CARD 12
#ifndef NUMBER_IO_CARD
#define NUMBER_IO_CARD 21
#endif // !NUMBER_IO_CARD

#pragma region Error Codes
#define SUCCESS_IO = 0;
#define ERROR_INVALID_IO_CONFIGURATION 1;
#define ERROR_INVALID_IO_CONTROLLER 2;
#define ERROR_INVALID_IO_CONTROLLER_NO 3;
#define ERROR_INVALID_IO_MODULE 4;
#define ERROR_IO_FUNCTION_NOT_SUPPORTED 5;

#pragma endregion

//--

struct IOConfiguration
{
public:
	char Controller[256];
	char ControllerDetails[256];
	bool Enable;
	bool Online;
	char IOName[256];
};

// This class is exported from the IO.dll
class IO_API CIO
{
public:
	int m_nErrorCode;
	LPSTR m_strErrorMsg;
	bool m_bOnline;
	bool m_bEnableDebug;
private:
	IOConfiguration m_IOConfiguration[NUMBER_IO_CARD];
	PCHAR BASE_ADDRESS0_ES2GPCI64C;
	ULONG BASE_ADDRESS1_ES2GPCI64C;
	ULONG BASE_ADDRESS2_ES2GPCI64C;
	ULONG PORT_OUTPUT;
	CRITICAL_SECTION csSetOutput;
	CRITICAL_SECTION csReadInput;
	unsigned long m_ulArrayInput[NUMBER_IO_CARD];
	unsigned long m_ulArrayOutput[NUMBER_IO_CARD];
	bool m_bStopScan;
	bool m_bIsCommunicationDisconnect;
	int m_nNoOfIOHasBeenConfigured;
public:	

    CIO(void);
	~CIO(void);
    // TODO: add your methods here.
	virtual int CIO::Initialize(bool online, bool enableDebug, IOConfiguration* ioConfiguration, int ioQuantity);
	int RTFCNDCL CIO::Connect();
	ULONG RTFCNDCL CIO::IOScanThread(unsigned long *ulArrayInput, unsigned long *ulArrayOutput);
	int RTFCNDCL CIO::InitializeIO(unsigned long ucInput[NUMBER_IO_CARD], unsigned long ucOutput[NUMBER_IO_CARD]);
	int RTFCNDCL CIO::InitializeIO();
	int RTFCNDCL CIO::Disconnect();
	int RTFCNDCL CIO::EnablePort();
	int RTFCNDCL CIO::DisablePort();
	int RTFCNDCL CIO::ReadAllOutput(unsigned long *ulArrayOutput);
	int RTFCNDCL CIO::ReadInput(int cardNo, int bit);
	int RTFCNDCL CIO::WriteOutput(int cardNo, int bit, bool trueToSet);
	int RTFCNDCL CIO::UpdateAllOutput(unsigned long *ulArrayOutput);
	int RTFCNDCL CIO::ReadOutput(int cardNo, int bit);
	int RTFCNDCL CIO::StopScan();
	int RTFCNDCL CIO::IsCommunicationDisconnect();
private:
	LPSTR CIO::GetControllerDetailsControllerNo(char *ControllerDetailsIPAddress);
	LPSTR CIO::GetControllerDetailsIPAddress(char *ControllerDetailsIPAddress);
	LPSTR CIO::GetControllerDetailsIOModule(char *ControllerDetailsIPAddress);
	LPSTR CIO::GetControllerDetails(char *ControllerDetailsIPAddress, int DetailNo);
};
extern IO_API int nIO;

#endif