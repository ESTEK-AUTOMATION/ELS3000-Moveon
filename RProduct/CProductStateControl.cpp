#include "CProductStateControl.h"

CProductStateControl::CProductStateControl()
{
}

CProductStateControl::~CProductStateControl()
{
}

int CProductStateControl::SetProductStateControl(CProductStateControl *productStateControl)
{
	m_cProductStateControl = productStateControl;
	m_cProductStateControl->SetPlatformStateControl(productStateControl);
	return 0;
}

int CProductStateControl::OnChangeIdleStartStateToIdlingState()
{
	CPlatformStateControl::OnChangeIdleStartStateToIdlingState();
	return 0;
}

int CProductStateControl::OnChangePreHomeStartStateToPreHomingState()
{
	CPlatformStateControl::OnChangePreHomeStartStateToPreHomingState();	
	return 0;
}

int CProductStateControl::OnChangePostHomingStateToHomeDoneState()
{
	CPlatformStateControl::OnChangePostHomingStateToHomeDoneState();
	return 0;
}

int CProductStateControl::OnChangePreProductionStartStateToPreProductioningState()
{
	CPlatformStateControl::OnChangePreProductionStartStateToPreProductioningState();
	return 0;
}

int CProductStateControl::OnChangePostProductioningStateToPostProductionDoneState()
{
	CPlatformStateControl::OnChangePostProductioningStateToPostProductionDoneState();
	return 0;
}

int CProductStateControl::OnChangePreSetupStartStateToPreSetupingState()
{
	CPlatformStateControl::OnChangePreSetupStartStateToPreSetupingState();
	return 0;
}

int CProductStateControl::OnAlarmStartState()
{
	CPlatformStateControl::OnAlarmStartState();
	m_cProductIOControl->IsAlarmState = true;
	return 0;
}

int CProductStateControl::OnChangePausingStateToPauseDoneState()
{
	CPlatformStateControl::OnChangePausingStateToPauseDoneState();
	if (!m_cProductIOControl->IsAlarmState)
	{
		m_cProductIOControl->SetTowerLightRed(false);
		m_cProductIOControl->SetTowerLightAmber(true);
		m_cProductIOControl->SetTowerLightGreen(false);
		m_cProductIOControl->SetTowerLightBuzzer(false);
	}
	return 0;
}

int CProductStateControl::OnCheckResumeStartStateReady()
{
	LARGE_INTEGER lnClockStart, lnClockStart2, lnClockEnd, lnClockSpan;

	if (CPlatformStateControl::OnCheckResumeStartStateReady() == 0 && smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false)
	{
		
		RtGetClockTime(CLOCK_FASTEST, &lnClockStart2);
	}
	else if (CPlatformStateControl::OnCheckResumeStartStateReady() == 0 && smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == true)
	{
		smGeneral->State = ResumingState;
		//break;
		m_cProductIOControl->IsAlarmState = false;
		m_cProductIOControl->SetTowerLightRed(false);
		m_cProductIOControl->SetTowerLightAmber(false);
		m_cProductIOControl->SetTowerLightGreen(true);
		m_cProductIOControl->SetTowerLightBuzzer(false);
		return 0;
	}
	else
	{
		m_cProductShareVariables->SetAlarm(5003);
		//break;
		return 2;
	}
	while (smProductEvent->ExitRTX.Set == false)
	{
		RtGetClockTime(CLOCK_FASTEST, &lnClockEnd);
		lnClockSpan.QuadPart = lnClockEnd.QuadPart - lnClockStart2.QuadPart;
		if (lnClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT - 1000)
		{
			m_cProductShareVariables->SetAlarm(5003);
			m_cLogger->WriteLog("Resume: Door lock timeout %ums.\n", lnClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//break;
			return 3;
		}
		else if (true
			&& (CPlatformStateControl::OnCheckResumeStartStateReady() == 0 /*&& smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == true*/)
			)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_100ms);
			//RtSleepFt(&m_cProductShareVariables->m_lnPeriod_100ms);
			m_cLogger->WriteLog("Resume: Door lock\n");
			//smGeneral->State = ResumingState;
			//break;
			m_cProductIOControl->IsAlarmState = false;
			m_cProductIOControl->SetTowerLightRed(false);
			m_cProductIOControl->SetTowerLightAmber(false);
			m_cProductIOControl->SetTowerLightGreen(true);
			m_cProductIOControl->SetTowerLightBuzzer(false);
			return 0;
		}
		RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
	}
}

int CProductStateControl::OnResumeDoneStateDoneForOtherState()
{
	CPlatformStateControl::OnResumeDoneStateDoneForOtherState();
	return 0;
}

int CProductStateControl::OnResumeDoneStateDoneForHomeState()
{
	CPlatformStateControl::OnResumeDoneStateDoneForHomeState();
	return 0;
}

int CProductStateControl::OnResumeDoneStateDoneForProductionState()
{
	CPlatformStateControl::OnResumeDoneStateDoneForProductionState();
	return 0;
}

bool CProductStateControl::OnCheckShuttingDownStateDone()
{
	if (CPlatformStateControl::OnCheckShuttingDownStateDone()

		&& (smProductCustomize->EnableMotionController1 == false || (smProductCustomize->EnableMotionController1 == true && smProductEvent->RTHD_RMAIN_MC1_END.Set == true))
		&& (smProductCustomize->EnableMotionController2 == false || (smProductCustomize->EnableMotionController2 == true && smProductEvent->RTHD_RMAIN_MC2_END.Set == true))
		&& (smProductCustomize->EnableMotionController3 == false || (smProductCustomize->EnableMotionController3 == true && smProductEvent->RTHD_RMAIN_MC3_END.Set == true))
		&& (smProductCustomize->EnableMotionController4 == false || (smProductCustomize->EnableMotionController4 == true && smProductEvent->RTHD_RMAIN_MC4_END.Set == true))
		&& (smProductCustomize->EnableMotionController5 == false || (smProductCustomize->EnableMotionController5 == true && smProductEvent->RTHD_RMAIN_MC5_END.Set == true))
		&& (smProductCustomize->EnableMotionController6 == false || (smProductCustomize->EnableMotionController6 == true && smProductEvent->RTHD_RMAIN_MC6_END.Set == true))
		&& (smProductCustomize->EnableMotionController7 == false || (smProductCustomize->EnableMotionController7 == true && smProductEvent->RTHD_RMAIN_MC7_END.Set == true))
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_INPUT_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S2_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_BOTTOM_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S3_VISION_END.Set == true

		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE2_SEQUENCE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REPLACE_END.Set==true
		//&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set==true
		&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set==true
		)
	{
		return true;
	}
	else
		return false;
}

int CProductStateControl::OnTriggerResetButton()
{
	m_cProductIOControl->SetTowerLightBuzzer(false);
	return 0;
}