#pragma once
#ifndef __CPLATFORMTHREAD_H_INCLUDED__ 
#define __CPLATFORMTHREAD_H_INCLUDED__

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

class RPlatform_API CPlatformThread
{
public:
	LARGE_INTEGER lnClockStart, lnClockEnd, lnClockSpan, lnClockStart2;

	CPlatformThread();
	~CPlatformThread();

	virtual int RTFCNDCL CPlatformThread::SetPlatformThread(CPlatformThread *platformThread);
	virtual ULONG RTFCNDCL CPlatformThread::SequenceThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::IOScanThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::IOOperationThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::TeachPointThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::ManualThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::MaintenanceThread(void * nContext);
	virtual ULONG RTFCNDCL CPlatformThread::AutoOperationThread(void * nContext);
	//virtual ULONG RTFCNDCL CPlatformThread::StateThread(void * nContext);

	virtual int CPlatformThread::SequenceThreadInitializeSequence();
	virtual int CPlatformThread::SequenceThreadHomeSequence();
	virtual int CPlatformThread::SequenceThreadSetupSequence();
	virtual int CPlatformThread::SequenceThreadJobSequence();
	virtual int CPlatformThread::SequenceThreadEndingSequence();
	virtual int CPlatformThread::SequenceThreadMaintenanceSequence();

	virtual int CPlatformThread::IOScanThreadSetOutputBeforeExitSoftwareAndDisconnect();

	virtual int CPlatformThread::IOOperationThreadOnStart();
	virtual int CPlatformThread::IOOperationThreadOnTriggerOutput();
	virtual int CPlatformThread::IOOperationThreadWhileInJobMode();
	virtual int CPlatformThread::IOOperationThreadInAllMode();

	virtual int CPlatformThread::OnUpdateSetting();

	virtual int CPlatformThread::OnTeachPointThreadMotorOff(int axis);
	virtual int CPlatformThread::OnTeachPointThreadMotorOn(int axis);
	virtual int CPlatformThread::OnTeachPointThreadMotorHome(int axis);
	virtual int CPlatformThread::OnTeachPointThreadMotorMoveRelative(int axis, signed long position);
	virtual int CPlatformThread::OnTeachPointThreadMotorMoveAbsolute(int axis, signed long position);
	virtual int CPlatformThread::OnTeachPointThreadMotorSetSpeedAndAcceleration(int axis, int speedPercent, int acceleration, int deceleration);
	virtual int CPlatformThread::OnTeachPointThreadMotorSemiAutoTeach(int axis, int number);
	virtual int CPlatformThread::ManualThreadSwitch(int number);
};
CPlatformThread *m_cPlatformThread = new CPlatformThread();
#endif