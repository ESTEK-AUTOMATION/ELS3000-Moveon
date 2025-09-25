#pragma once
#ifndef __CPLATFORMMAIN_H_INCLUDED__ 
#define __CPLATFORMMAIN_H_INCLUDED__

#ifndef __CPLATFORMTHREAD_H_INCLUDED__ 
#include "CPlatformThread.h"
#endif

#ifndef __CPLATFORMSEQUENCE_H_INCLUDED__ 
#include "CPlatformSequence.h"
#endif

#ifndef __CPLATFORMSTATECONTROL_H_INCLUDED__ 
#include "CPlatformStateControl.h"
#endif

#ifndef __CPLATFORMMOTORCONTROL_H_INCLUDED__ 
#include "CPlatformMotorControl.h"
#endif

#ifndef __CPLATFORMIOCONTROL_H_INCLUDED__ 
#include "CPlatformIOControl.h"
#endif

#ifndef __CPLATFORMSHAREVARIABLES_H_INCLUDED__ 
#include "CPlatformShareVariables.h"
#endif

#ifndef __CPLATFORMSHAREDMEMORY_H_INCLUDED__ 
#include "CPlatformSharedMemory.h"
#endif

#include "RPlatform.h"

class RPlatform_API CPlatformMain
{
public:
	CPlatformMain();
	~CPlatformMain();

	int CPlatformMain::SetShareMemory(SharedMemorySetting *sharedMemorySetting
		, SharedMemoryTeachPoint *sharedMemoryTeachPoint
		, SharedMemoryProduction *sharedMemoryProduction
		, SharedMemoryCustomize *sharedMemoryCustomize
		, SharedMemoryIO *sharedMemoryIO
		, SharedMemoryModuleStatus *sharedMemoryModuleStatus
		, SharedMemoryEvent *sharedMemoryEvent
		, SharedMemoryGeneral *sharedMemoryGeneral
	);
	virtual int CPlatformMain::SetPlatformClasses(CLogger *cLogger, CMotionLibrary *cMotionLibrary, CIO *cIO);
	virtual int CPlatformMain::RunApplication();
	virtual int CPlatformMain::Initialize();
	virtual int CPlatformMain::CreateShareMemory();
	virtual int CPlatformMain::ResetVariables();
	virtual int CPlatformMain::LoadSetting();
	virtual int CPlatformMain::CreateAllThread();
	virtual int CPlatformMain::Run();
	virtual bool CPlatformMain::IsAllThreadEnd();
	virtual int CPlatformMain::OnAllThreadEndTimeout();
	
	
};

CPlatformMain *m_cPlatformMain = new CPlatformMain();

#endif