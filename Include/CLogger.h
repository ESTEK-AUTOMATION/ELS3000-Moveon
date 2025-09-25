#pragma once
#ifndef __CLOGGER_H_INCLUDED__
#define __CLOGGER_H_INCLUDED__

#ifndef __FILELOGGER_H_INCLUDED__
#include "FileLogger.h"
#endif

#if defined UNDER_RTSS
#if defined RTX64_EXPORTS
#define RT_API __declspec(dllexport)
#else
#define RT_API __declspec(dllimport)
#endif
#else
#if defined DLL64_EXPORTS
#define RT_API __declspec(dllexport)
#else
#define RT_API __declspec(dllimport)
#endif
#endif

class RT_API CLogger
{
private:
	CFileLogger *m_cFileLogger;
	CRITICAL_SECTION csWriteLog;	
public:
	CLogger();
	~CLogger();
	int CLogger::Initialize();
	int CLogger::WriteLog(char* message, ...);
	int CLogger::Shutdown();
	bool CLogger::IsShutDown();
};
CLogger *m_cLogger = new CLogger();
#endif