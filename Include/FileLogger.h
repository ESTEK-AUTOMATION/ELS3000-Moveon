//////////////////////////////////////////////////////////////////
//
// FileLogger.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 2/28/2018 9:27:44 AM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////

#pragma once
#ifndef __FILELOGGER_H_INCLUDED__
#define __FILELOGGER_H_INCLUDED__
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS

#ifndef _INC_SDKDDKVER
#include <SDKDDKVer.h>
#endif

//#include <stdio.h>
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
#ifndef _ARRAY_
#include <array>
#endif
#ifndef _FSTREAM_
#include <fstream>
#endif
#ifndef _STRING_
#include <string>
#endif
#ifndef _LIST_
#include <list>
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

using namespace std;

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define FileLogger_API __declspec(dllexport)
  #else
    #define FileLogger_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define FileLogger_API __declspec(dllexport)
  #else
    #define FileLogger_API __declspec(dllimport)
  #endif
#endif

typedef struct LogStatus{
	char chArrLogMsg[1024];
	char chArrDate[256];
	char chArrTime[256];
	bool SendMsgDone;
}LogStatus, *PLogStatus;

typedef struct StringLogStatus {
	string strLogMsg;
	char chArrDate[256];
	char chArrTime[256];
}StringLogStatus, *PStringLogStatus;

typedef struct ThreadInformation {
	list<StringLogStatus> listStrLogStatus;
	string strLogMsg;
	bool m_bStartEndThread;
	bool m_bThreadEnd;
	bool m_bKillThread;
}ThreadInformation, *PThreadInformation;


// This class is exported from the FileLogger.dll
class FileLogger_API CFileLogger
{
private:
	int m_nErrorCode;
	int m_nNoOfDayToKeepLog;
    char* m_strErrorMsg;
	CRITICAL_SECTION csLogFile;
	LARGE_INTEGER lnClockStart, lnClockEnd, lnClockSpan, lnClockStart2;

	PTP_WORK m_work;
	PTP_POOL m_pool;
	PTP_CLEANUP_GROUP m_cleanupgroup;
	UINT m_rollback;
	LogStatus m_logStatus;
	StringLogStatus m_strLogStatus;
	string m_strLogMsg;
	ThreadInformation m_ThreadInformation;
public:
	CFileLogger(void);
	~CFileLogger();
    // TODO: add your methods here.
	int RTFCNDCL CFileLogger::Initialize();
	int RTFCNDCL CFileLogger::Initialize(char* directory, int noOfLogToKeep);
	int RTFCNDCL CFileLogger::Shutdown();
	bool RTFCNDCL CFileLogger::IsShutdown();	
	int RTFCNDCL CFileLogger::Write(char* message, ...);
	int RTFCNDCL CFileLogger::WriteWithDateTime(char* message, ...);
private:
	int CFileLogger::Dispose();
	int CFileLogger::CleanUp(int);
	int CFileLogger::CleanOldLog(int noOfLogToKeep);
	int CFileLogger::WriteToFile(LogStatus logStatus);
	int CFileLogger::WriteToFile(StringLogStatus stringLogStatus);
	
};
extern FileLogger_API int nFileLogger;

// function prototype for the RTDLL exported function
 
VOID
CALLBACK
LogFileCallback(
    PTP_CALLBACK_INSTANCE Instance,
    PVOID                 Parameter,
    PTP_WORK              Work
    );

ULONG LoggerThread(void * nContext);
#endif
