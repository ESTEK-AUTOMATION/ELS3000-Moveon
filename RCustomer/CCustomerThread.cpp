#include "CCustomerThread.h"

CCustomerThread::CCustomerThread()
{
}

CCustomerThread::~CCustomerThread()
{
}
int CCustomerThread::IOOperationThreadWhileInJobMode()
{
	int nError = 0;

	CProductThread::IOOperationThreadWhileInJobMode();

	lnClockSpan.QuadPart = lnClockEnd.QuadPart - lnClockStartTriggerStopButton.QuadPart;
	if (m_cCustomerIOControl->IsStopButtonPressed() == true && (lnClockEnd.QuadPart - lnClockPeriodTriggerStopButton.QuadPart) / m_cProductShareVariables->m_TimeCount > 100 && lnClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 1500)
	{
		RtGetClockTime(CLOCK_FASTEST, &lnClockStartTriggerStopButton);
		m_cLogger->WriteLog("User press stop button.\n");

		if (m_cCustomerStateControl->IsCurrentStateCanTriggerPause() == true)
			smCustomerEvent->StartPause.Set = true;
	}
	else if (m_cCustomerIOControl->IsStopButtonPressed() == false)
	{
		RtGetClockTime(CLOCK_FASTEST, &lnClockPeriodTriggerStopButton);
	}
	if (smCustomerEvent->RMAIN_RTHD_ALARM_ASSIST_START.Set == true)
	{
		smCustomerEvent->RMAIN_RTHD_ALARM_ASSIST_START.Set = false;
		m_cProductIOControl->SetTowerLightRed(false);
		m_cProductIOControl->SetTowerLightAmber(true);
		m_cProductIOControl->SetTowerLightGreen(false);
		m_cProductIOControl->SetTowerLightBuzzer(true);
	}
	if (smCustomerEvent->RMAIN_RTHD_ALARM_MESSAGE_START.Set == true)
	{
		smCustomerEvent->RMAIN_RTHD_ALARM_MESSAGE_START.Set = false;
		m_cProductIOControl->SetTowerLightRed(false);
		m_cProductIOControl->SetTowerLightAmber(true);
		m_cProductIOControl->SetTowerLightGreen(false);
		m_cProductIOControl->SetTowerLightBuzzer(true);
	}
	if (smCustomerEvent->RMAIN_RTHD_ALARM_FAILURE_START.Set == true)
	{
		smCustomerEvent->RMAIN_RTHD_ALARM_FAILURE_START.Set = false;
		m_cProductIOControl->SetTowerLightRed(true);
		m_cProductIOControl->SetTowerLightAmber(true);
		m_cProductIOControl->SetTowerLightGreen(false);
		m_cProductIOControl->SetTowerLightBuzzer(false);
	}
	return nError;
}