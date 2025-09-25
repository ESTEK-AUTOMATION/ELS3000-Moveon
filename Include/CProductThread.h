#pragma once
#ifndef __CPRODUCTTHREAD_H_INCLUDED__ 
#define __CPRODUCTTHREAD_H_INCLUDED__

#ifndef __CPLATFORMTHREAD_H_INCLUDED__ 
#include "CPlatformThread.h"
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

class RProduct_API CProductThread : public CPlatformThread
{
private:
	LONGLONG  lManualThreadConvert1msTo100ns = 10000;
	LARGE_INTEGER lnDelayIn100ns;
	signed long m_slManualThreadPitchValue_um = 1250;//1250
	bool m_bManualThreadMoveTop = true;
	bool m_bManualThreadMoveTopX = true;
	bool m_bManualThreadMoveTopY = true;

	int m_nTurretIndexNo = 0;
	int m_nFlipperIndexNo = 0;

	int nManuaThreadSequenceNo = 0;
	bool bManualThreadStart = true;

public:
	LARGE_INTEGER
		lnClockStartTriggerStartButton,
		lnClockStartTriggerStopButton,
		lnClockStartTriggerResetButton,
		lnClockPeriodTriggerStartButton,
		lnClockPeriodTriggerStopButton,
		lnClockPeriodTriggerResetButton,
		lnClockPeriodTriggerOffCanisterPurge,
		lnClockPeriodTriggerOffTurretPurgeBinPurge,
		lnClockPeriodTriggerOffFlipperRejectBinPurge,
		lnClockStartTakeTHKValue,
		lnClockPeriodTakeTHKValue;
	LARGE_INTEGER lnClockStartFeeder;
	LARGE_INTEGER lnClockResetButtonEnd;
	LARGE_INTEGER lnClockResetButtonSpan;
	bool bIsTriggerOffCanisterPurge = false;
	bool bIsTriggerOffTurretPurgeBinPurge = false;
	bool bIsTriggerOffFlipperRejectBinPurge = false;

	LARGE_INTEGER lnInputLoadingUnloadingSequenceSoftTraySensorOffClockStart, lnInputLoadingUnloadingSequenceSoftTraySensorOffClockEnd, lnInputLoadingUnloadingSequenceSoftTraySensorOffClockSpan;
	LARGE_INTEGER lnOutputLoadingUnloadingSequenceSoftTraySensorOffClockStart, lnOutputLoadingUnloadingSequenceSoftTraySensorOffClockEnd, lnOutputLoadingUnloadingSequenceSoftTraySensorOffClockSpan;
	LARGE_INTEGER lnSortingLoadingUnloadingSequenceSoftTraySensorOffClockStart, lnSortingLoadingUnloadingSequenceSoftTraySensorOffClockEnd, lnSortingLoadingUnloadingSequenceSoftTraySensorOffClockSpan;
	LARGE_INTEGER lnRejectLoadingUnloadingSequenceSoftTraySensorOffClockStart, lnRejectLoadingUnloadingSequenceSoftTraySensorOffClockEnd, lnRejectLoadingUnloadingSequenceSoftTraySensorOffClockSpan;
	CProductThread();
	~CProductThread();

	virtual int CProductThread::SetProductThread(CProductThread *productThread);
	ULONG RTFCNDCL CProductThread::IOScanThread(void * nContext) override;


	ULONG CProductThread::MC1Thread(void * nContext);
	ULONG CProductThread::MC2Thread(void * nContext);
	ULONG CProductThread::MC3Thread(void * nContext);
	ULONG CProductThread::MC4Thread(void * nContext);
	ULONG CProductThread::MC5Thread(void * nContext);
	ULONG CProductThread::MC6Thread(void * nContext);
	ULONG CProductThread::MC7Thread(void * nContext);

	ULONG CProductThread::PickAndPlace1YAxisMotorThread(void * nContext);
	ULONG CProductThread::InputTrayTableXAxisMotorThread(void * nContext);
	ULONG CProductThread::InputTrayTableYAxisMotorThread(void * nContext);
	ULONG CProductThread::InputTrayTableZAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace2YAxisMotorThread(void * nContext);
	ULONG CProductThread::OutputTrayTableXAxisMotorThread(void * nContext);
	ULONG CProductThread::OutputTrayTableYAxisMotorThread(void * nContext);
	ULONG CProductThread::OutputTrayTableZAxisMotorThread(void * nContext);
	ULONG CProductThread::InputVisionModuleMotorThread(void * nContext);
	ULONG CProductThread::S2VisionModuleMotorThread(void * nContext);
	ULONG CProductThread::S1VisionModuleMotorThread(void * nContext);
	ULONG CProductThread::S3VisionModuleMotorThread(void * nContext);

	ULONG CProductThread::PickAndPlace1XAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace2XAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace1ZAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace2ZAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace1ThetaAxisMotorThread(void * nContext);
	ULONG CProductThread::PickAndPlace2ThetaAxisMotorThread(void * nContext);

	//ULONG CProductThread::PickAndPlacePickupHead1ZAxisMotorThread(void * nContext);
	//ULONG CProductThread::PickAndPlacePickupHead1ThetaAxisMotorThread(void * nContext);
	//ULONG CProductThread::PickAndPlacePickupHead2ZAxisMotorThread(void * nContext);
	//ULONG CProductThread::PickAndPlacePickupHead2ThetaAxisMotorThread(void * nContext);

	ULONG CProductThread::InputVisionThread(void * nContext);
	ULONG CProductThread::S2VisionThread(void * nContext);
	ULONG CProductThread::OutputVisionThread(void * nContext);
	ULONG CProductThread::BottomVisionThread(void * nContext);
	ULONG CProductThread::S1VisionThread(void * nContext);
	ULONG CProductThread::S3VisionThread(void * nContext);

	//ULONG CProductThread::InputLoadingUnloadingSequenceThread(void * nContext);
	//ULONG CProductThread::OutputLoadingUnloadingSequenceThread(void * nContext);
	//ULONG CProductThread::SortingLoadingUnloadingSequenceThread(void * nContext);
	//ULONG CProductThread::RejectLoadingUnloadingSequenceThread(void * nContext);
	ULONG CProductThread::PickAndPlaceSequenceThread(void * nContext);
	ULONG CProductThread::PickAndPlace2SequenceThread(void * nContext);
	ULONG CProductThread::InputTrayTableSequenceThread(void * nContext);
	ULONG CProductThread::OutputAndRejectTrayTableSequenceThread(void * nContext);
	//ULONG CProductThread::InputSequenceThread(void * nContext);
	//ULONG CProductThread::OutputSequenceThread(void * nContext);
	//ULONG CProductThread::SortingSequenceThread(void * nContext);
	//ULONG CProductThread::RejectSequenceThread(void * nContext);

	ULONG CProductThread::CheckRejectReplaceThread(void * nContext);
	//ULONG CProductThread::CheckRejectRemoveThread(void * nContext);
	ULONG CProductThread::CheckRejectRemoveThread(void * nContext);

	int CProductThread::SequenceThreadInitializeSequence() override;
	int CProductThread::SequenceThreadHomeSequence() override;
	int CProductThread::SequenceThreadSetupSequence() override;
	int CProductThread::SequenceThreadJobSequence() override;
	int CProductThread::SequenceThreadEndingSequence() override;
	int CProductThread::SequenceThreadMaintenanceSequence() override;

	int CProductThread::IOScanThreadSetOutputBeforeExitSoftwareAndDisconnect() override;

	int CProductThread::IOOperationThreadOnStart() override;
	int CProductThread::IOOperationThreadOnTriggerOutput() override;
	int CProductThread::IOOperationThreadWhileInJobMode() override;
	int CProductThread::IOOperationThreadInAllMode() override;

	int CProductThread::OnUpdateSetting() override;

	int CProductThread::OnTeachPointThreadMotorOff(int axis) override;
	int CProductThread::OnTeachPointThreadMotorOn(int axis) override;
	int CProductThread::OnTeachPointThreadMotorHome(int axis) override;
	int CProductThread::OnTeachPointThreadMotorMoveRelative(int axis, signed long position) override;
	int CProductThread::OnTeachPointThreadMotorMoveAbsolute(int axis, signed long position) override;
	int CProductThread::OnTeachPointThreadMotorSetSpeedAndAcceleration(int axis, int speedPercent, int acceleration, int deceleration) override;
	int CProductThread::OnTeachPointThreadMotorSemiAutoTeach(int axis, int number) override;
	int CProductThread::ManualThreadSwitch(int number) override;
};
CProductThread *m_cProductThread = new CProductThread();
#endif