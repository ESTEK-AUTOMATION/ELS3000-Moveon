#pragma once
#ifndef __CPRODUCTMAIN_H_INCLUDED__ 
#define __CPRODUCTMAIN_H_INCLUDED__

#ifndef NUMBER_IO_CARD
#define NUMBER_IO_CARD 21
#endif

#ifndef __CPLATFORMMAIN_H_INCLUDED__
#include "CPlatformMain.h"
#endif

#ifndef __CPRODUCTTHREAD_H_INCLUDED__ 
#include "CProductThread.h"
#endif

#ifndef __CPRODUCTSEQUENCE_H_INCLUDED__ 
#include "CProductSequence.h"
#endif

#ifndef __CPRODUCTSTATECONTROL_H_INCLUDED__ 
#include "CProductStateControl.h"
#endif

#ifndef __CPRODUCTMOTORCONTROL_H_INCLUDED__ 
#include "CProductMotorControl.h"
#endif

#ifndef __CPRODUCTIOCONTROL_H_INCLUDED__ 
#include "CProductIOControl.h"
#endif

#ifndef __CPRODUCTSHAREVARIABLES_H_INCLUDED__ 
#include "CProductShareVariables.h"
#endif

#ifndef __CPRODUCTSHAREDMEMORY_H_INCLUDED__ 
#include "CProductSharedMemory.h"
#endif

#include "RProduct.h"

class RProduct_API CProductMain : public CPlatformMain
{
public:
	CProductMain();
	~CProductMain();

	virtual int CProductMain::SetProductShareMemory(ProductSharedMemorySetting *sharedProductMemorySetting
		, ProductSharedMemoryTeachPoint *sharedProductMemoryTeachPoint
		, ProductSharedMemoryProduction *sharedProductMemoryProduction
		, ProductSharedMemoryCustomize *sharedProductMemoryCustomize
		, ProductSharedMemoryIO *sharedProductMemoryIO
		, ProductSharedMemoryModuleStatus *sharedProductMemoryModuleStatus
		, ProductSharedMemoryEvent *sharedProductMemoryEvent
		, ProductSharedMemoryGeneral *sharedProductMemoryGeneral
	);
	int CProductMain::SetProductClasses(CLogger *cLogger, CMotionLibrary *cMotionLibrary, CIO *cIO);
	int CProductMain::Initialize() override;
	int CProductMain::CreateShareMemory() override;
	
	int CProductMain::ResetVariables() override;
	int CProductMain::LoadSetting() override;
	int CProductMain::CreateAllThread() override;

	bool CProductMain::IsAllThreadEnd() override;
	int CProductMain::OnAllThreadEndTimeout() override;
};
CProductMain *m_cProductMain = new CProductMain();
#endif