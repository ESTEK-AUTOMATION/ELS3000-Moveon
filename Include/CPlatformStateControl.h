#pragma once
#ifndef __CPLATFORMSTATECONTROL_H_INCLUDED__ 
#define __CPLATFORMSTATECONTROL_H_INCLUDED__

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

class RPlatform_API CPlatformStateControl
{
public:

	int m_nStateSequence = 0;

	#pragma region State variables
	static const int InitializeStartState = 1;
	static const int InitializingState = 2;
	static const int InitializeDoneState = 3;
	static const int DisableStartState = 4;
	static const int DisablingState = 5;
	static const int DisableDoneState = 6;
	static const int ShutDownStartState = 7;
	static const int ShutDowningState = 8;
	static const int ShutDownDoneState = 9;
	static const int IdleStartState = 10;
	static const int IdlingState = 11;
	static const int IdleDoneState = 12;
	static const int PreHomeStartState = 13;
	static const int PreHomingState = 14;
	static const int PreHomeDoneState = 15;
	static const int HomeStartState = 16;
	static const int HomingState = 17;
	static const int HomeDoneState = 18;
	static const int PostHomeStartState = 19;
	static const int PostHomingState = 20;
	static const int PostHomeDoneState = 21;
	static const int PreProductionStartState = 22;
	static const int PreProductioningState = 23;
	static const int PreProductionDoneState = 24;
	static const int ProductionStartState = 25;
	static const int ProductioningState = 26;
	static const int ProductionDoneState = 27;
	static const int PostProductionStartState = 28;
	static const int PostProductioningState = 29;
	static const int PostProductionDoneState = 30;
	static const int PreSetupStartState = 31;
	static const int PreSetupingState = 32;
	static const int PreSetupDoneState = 33;
	static const int SetupStartState = 34;
	static const int SetupingState = 35;
	static const int SetupDoneState = 36;
	static const int PostSetupStartState = 37;
	static const int PostSetupingState = 38;
	static const int PostSetupDoneState = 39;
	static const int AlarmStartState = 40;
	static const int AlarmingState = 41;
	static const int AlarmDoneState = 42;
	static const int PauseStartState = 43;
	static const int PausingState = 44;
	static const int PauseDoneState = 45;
	static const int ResumeStartState = 46;
	static const int ResumingState = 47;
	static const int ResumeDoneState = 48;
	static const int AbortStartState = 49;
	static const int AbortingState = 50;
	static const int AbortDoneState = 51;
	#pragma endregion

	CPlatformStateControl();
	~CPlatformStateControl();

	virtual int RTFCNDCL CPlatformStateControl::SetPlatformStateControl(CPlatformStateControl *platformStateControl);

	int CPlatformStateControl::RunState();
	//bool CPlatformStateControl::IsStateCanResume();
	bool CPlatformStateControl::IsStateCanAlarm(int nState);
	bool CPlatformStateControl::IsStateCanPause(int nState);
	bool CPlatformStateControl::IsStateCanAbort(int nState);
	bool CPlatformStateControl::IsCurrentStateCanTriggerResume();
	bool CPlatformStateControl::IsCurrentStateCanTriggerPause();
	bool CPlatformStateControl::IsStateShutdown();

	virtual int CPlatformStateControl::OnChangeIdleStartStateToIdlingState();
	virtual int CPlatformStateControl::OnChangeIdlingStateToIdleDoneState();
	virtual int CPlatformStateControl::OnChangePreHomeStartStateToPreHomingState();
	virtual int CPlatformStateControl::OnChangePostHomingStateToHomeDoneState();
	virtual int CPlatformStateControl::OnChangePreProductionStartStateToPreProductioningState();
	virtual int CPlatformStateControl::OnChangePostProductioningStateToPostProductionDoneState();
	virtual int CPlatformStateControl::OnChangePreSetupStartStateToPreSetupingState();
	virtual int CPlatformStateControl::OnAlarmStartState();
	virtual int CPlatformStateControl::OnChangePausingStateToPauseDoneState();
	virtual int CPlatformStateControl::OnCheckResumeStartStateReady();
	virtual int CPlatformStateControl::OnResumeDoneStateDoneForOtherState();
	virtual int CPlatformStateControl::OnResumeDoneStateDoneForHomeState();
	virtual int CPlatformStateControl::OnResumeDoneStateDoneForProductionState();
	//1.0.0.0h Charles
	virtual int CPlatformStateControl::OnResumeDoneStateDoneForSetupState();
	//--
	virtual bool CPlatformStateControl::OnCheckShuttingDownStateDone();
	virtual int CPlatformStateControl::OnTriggerResetButton();
};

CPlatformStateControl *m_cPlatformStateControl = new CPlatformStateControl();

#endif