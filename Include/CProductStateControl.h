#pragma once
#ifndef __CPRODUCTSTATECONTROL_H_INCLUDED__ 
#define __CPRODUCTSTATECONTROL_H_INCLUDED__

#ifndef __CPLATFORMSTATECONTROL_H_INCLUDED__ 
#include "CPlatformStateControl.h"
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

class RProduct_API CProductStateControl : public CPlatformStateControl
{
public:
	CProductStateControl();
	~CProductStateControl();

	virtual int CProductStateControl::SetProductStateControl(CProductStateControl *productStateControl);
	int CProductStateControl::OnChangeIdleStartStateToIdlingState() override;
	int CProductStateControl::OnChangePreHomeStartStateToPreHomingState() override;
	int CProductStateControl::OnChangePostHomingStateToHomeDoneState() override;
	int CProductStateControl::OnChangePreProductionStartStateToPreProductioningState() override;
	int CProductStateControl::OnChangePostProductioningStateToPostProductionDoneState() override;
	int CProductStateControl::OnChangePreSetupStartStateToPreSetupingState() override;
	int CProductStateControl::OnAlarmStartState() override;
	int CProductStateControl::OnChangePausingStateToPauseDoneState() override;
	int CProductStateControl::OnCheckResumeStartStateReady() override;
	int CProductStateControl::OnResumeDoneStateDoneForOtherState() override;
	int CProductStateControl::OnResumeDoneStateDoneForHomeState() override;
	int CProductStateControl::OnResumeDoneStateDoneForProductionState() override;
	bool CProductStateControl::OnCheckShuttingDownStateDone() override;
	int CProductStateControl::OnTriggerResetButton() override;
};
CProductStateControl *m_cProductStateControl = new CProductStateControl();
#endif