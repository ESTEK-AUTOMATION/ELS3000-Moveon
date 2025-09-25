#pragma once
#ifndef __CCUSTOMERMAIN_H_INCLUDED__ 
#define __CCUSTOMERMAIN_H_INCLUDED__

#ifndef __CPRODUCTMAIN_H_INCLUDED__ 
#include "CProductMain.h"
#endif

#ifndef __CCUSTOMERTHREAD_H_INCLUDED__ 
#include "CCustomerThread.h"
#endif

#ifndef __CCUSTOMERSEQUENCE_H_INCLUDED__ 
#include "CCustomerSequence.h"
#endif

#ifndef __CCUSTOMERSTATECONTROL_H_INCLUDED__ 
#include "CCustomerStateControl.h"
#endif

#ifndef __CCUSTOMERMOTORCONTROL_H_INCLUDED__ 
#include "CCustomerMotorControl.h"
#endif

#ifndef __CCUSTOMERIOCONTROL_H_INCLUDED__
#include "CCustomerIOControl.h"
#endif

#ifndef __CCUSTOMERSHAREVARIABLES_H_INCLUDED__ 
#include "CCustomerShareVariables.h"
#endif

#ifndef __CCUSTOMERSHAREDMEMORY_H_INCLUDED__ 
#include "CCustomerSharedMemory.h"
#endif

#include "RCustomer.h"

class RCustomer_API CCustomerMain : public CProductMain
{
public:
	CCustomerMain();
	~CCustomerMain();

	int CCustomerMain::Initialize() override;
	int CCustomerMain::CreateShareMemory() override;
	int CCustomerMain::LoadSetting() override;
	int CCustomerMain::CreateAllThread() override;
	int CCustomerMain::Run() override;

	bool CCustomerMain::IsAllThreadEnd() override;
	int CCustomerMain::OnAllThreadEndTimeout() override;
	
};

ULONG RTFCNDCL SequenceThread(void * nContext);
ULONG RTFCNDCL IOScanThread(void * nContext);
ULONG RTFCNDCL IOOperationThread(void * nContext);
ULONG RTFCNDCL TeachPointThread(void * nContext);
ULONG RTFCNDCL ManualThread(void * nContext);
ULONG RTFCNDCL MaintenanceThread(void * nContext);
ULONG RTFCNDCL AutoOperationThread(void * nContext);
ULONG RTFCNDCL StateThread(void * nContext);
ULONG RTFCNDCL DummyThread(void * nContext);

ULONG RTFCNDCL MC1Thread(void * nContext);
ULONG RTFCNDCL MC2Thread(void * nContext);
ULONG RTFCNDCL MC3Thread(void * nContext);
ULONG RTFCNDCL MC4Thread(void * nContext);
ULONG RTFCNDCL MC5Thread(void * nContext);
ULONG RTFCNDCL MC6Thread(void * nContext);
ULONG RTFCNDCL MC7Thread(void * nContext);

ULONG RTFCNDCL PickAndPlace1YAxisMotorThread(void * nContext);
ULONG RTFCNDCL InputTrayTableXAxisMotorThread(void * nContext);
ULONG RTFCNDCL InputTrayTableYAxisMotorThread(void * nContext);
ULONG RTFCNDCL InputTrayTableZAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace2YAxisMotorThread(void * nContext);
ULONG RTFCNDCL OutputTrayTableXAxisMotorThread(void * nContext);
ULONG RTFCNDCL OutputTrayTableYAxisMotorThread(void * nContext);
ULONG RTFCNDCL OutputTrayTableZAxisMotorThread(void * nContext);
ULONG RTFCNDCL InputVisionModuleMotorThread(void * nContext);
ULONG RTFCNDCL S2VisionModuleMotorThread(void * nContext);
ULONG RTFCNDCL S1VisionModuleMotorThread(void * nContext);
ULONG RTFCNDCL S3VisionModuleMotorThread(void * nContext);

ULONG RTFCNDCL PickAndPlace1XAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace2XAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace1ZAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace2ZAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace1ThetaAxisMotorThread(void * nContext);
ULONG RTFCNDCL PickAndPlace2ThetaAxisMotorThread(void * nContext);

ULONG RTFCNDCL InputVisionThread(void * nContext);
ULONG RTFCNDCL S2VisionThread(void * nContext);
ULONG RTFCNDCL OutputVisionThread(void * nContext);
ULONG RTFCNDCL BottomVisionThread(void * nContext);
ULONG RTFCNDCL S1VisionThread(void * nContext);
ULONG RTFCNDCL S3VisionThread(void * nContext);
ULONG RTFCNDCL PickAndPlaceSequenceThread(void * nContext);
ULONG RTFCNDCL PickAndPlace2SequenceThread(void * nContext);
ULONG RTFCNDCL InputTrayTableSequenceThread(void * nContext);
ULONG RTFCNDCL OutputAndRejectTrayTableSequenceThread(void * nContext);
ULONG RTFCNDCL CheckRejectReplaceThread(void * nContext);
//ULONG RTFCNDCL CheckRejectRemoveThread(void * nContext);
ULONG RTFCNDCL CheckRejectRemoveThread(void * nContext);

CCustomerMain *m_cCustomerMain = new CCustomerMain();
#endif