#include "CProductSequence.h"
#include <vector>
CProductSequence::CProductSequence()
{
}

CProductSequence::~CProductSequence()
{
}

int CProductSequence::SetProductSequence(CProductSequence *productSequence)
{
	m_cProductSequence = productSequence;
	m_cProductSequence->SetPlatformSequence(productSequence);
	return 0;
}

int CProductSequence::SwitchInitializeSequence(int sequenceNo)
{
	switch (sequenceNo)
	{
	case 0:
		if (smProductEvent->RTXSoftwareInitializeDone.Set == true)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnInitializeSequenceClockStart2);
			sequenceNo++;
		}
		break;

	case 1:
		sequenceNo++;
		break;

	case 2:
		sequenceNo++;
		break;

	case 3:
		m_cLogger->WriteLog("Initializeseq: Start Initialize Controller\n");
		if (smProductCustomize->EnableMotionController1 == true)
		{
			smProductEvent->InitializeMotionController1Done.Set = false;
			smProductEvent->StartInitializeMotionController1.Set = true;
		}
		if (smProductCustomize->EnableMotionController2 == true)
		{
			smProductEvent->InitializeMotionController2Done.Set = false;
			smProductEvent->StartInitializeMotionController2.Set = true;
		}
		if (smProductCustomize->EnableMotionController3 == true)
		{
			smProductEvent->InitializeMotionController3Done.Set = false;
			smProductEvent->StartInitializeMotionController3.Set = true;
		}
		sequenceNo++;
		break;

	case 4:
		RtGetClockTime(CLOCK_FASTEST, &lnInitializeSequenceClockEnd);
		lnInitializeSequenceClockSpan.QuadPart = lnInitializeSequenceClockEnd.QuadPart - lnInitializeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableMotionController1 == false || (smProductCustomize->EnableMotionController1 == true && smProductEvent->InitializeMotionController1Done.Set == true))
			&& (smProductCustomize->EnableMotionController2 == false || (smProductCustomize->EnableMotionController2 == true && smProductEvent->InitializeMotionController2Done.Set == true))
			&& (smProductCustomize->EnableMotionController3 == false || (smProductCustomize->EnableMotionController3 == true && smProductEvent->InitializeMotionController3Done.Set == true))
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnInitializeSequenceClockEnd);
			lnInitializeSequenceClockSpan.QuadPart = lnInitializeSequenceClockEnd.QuadPart - lnInitializeSequenceClockStart2.QuadPart;
			smProductEvent->RTHD_GMAIN_CONNECT_TO_CONTROLLER_DONE.Set = true;
			m_cLogger->WriteLog("Initializeseq: Initialize controller Done %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
		}
		else if (lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			//m_cLogger->WriteLog("Initializeseq: Controller initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if(((smProductCustomize->EnableEtelDriver_0_1 || smProductCustomize->EnableEtelDriver_2_3 || smProductCustomize->EnableEtelDriver_4_5 || smProductCustomize->EnableEtelDriver_6_7 || smProductCustomize->EnableEtelDriver_8_9) && smProductEvent->InitializeEtelDone.Set == false))
			//if (smProductCustomize->EnableMotionController1 == true && smProductEvent->InitializeMotionController1Done.Set == false)
			//	m_cLogger->WriteLog("Initializeseq: Controller1 initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if (smProductCustomize->EnableMotionController2 == true && smProductEvent->InitializeMotionController2Done.Set == false)
			//	m_cLogger->WriteLog("Initializeseq: Controller2 initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if (smProductCustomize->EnableMotionController3 == true && smProductEvent->InitializeMotionController3Done.Set == false)
			//	m_cLogger->WriteLog("Initializeseq: Controller3 initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if (smProductCustomize->EnableMotionController4 == true && smProductEvent->InitializeMotionController4Done.Set == false)
			//	m_cLogger->WriteLog("Initializeseq: Controller4 initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if (smProductCustomize->EnableMotionController5 == true && smProductEvent->InitializeMotionController5Done.Set == false)
			//	m_cLogger->WriteLog("Initializeseq: Controller5 initialize timeout %ums.\n", lnInitializeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//sequenceNo = 0;
			if (smProductCustomize->EnableMotionController1 == true && smProductEvent->InitializeMotionController1Done.Set == false)
			{
				smProductEvent->InitializeMotionController1Done.Set = false;
				smProductEvent->StartInitializeMotionController1.Set = true;
			}
			if (smProductCustomize->EnableMotionController2 == true && smProductEvent->InitializeMotionController2Done.Set == false)
			{
				smProductEvent->InitializeMotionController2Done.Set = false;
				smProductEvent->StartInitializeMotionController2.Set = true;
			}
			if (smProductCustomize->EnableMotionController3 == true && smProductEvent->InitializeMotionController3Done.Set == false)
			{
				smProductEvent->InitializeMotionController3Done.Set = false;
				smProductEvent->StartInitializeMotionController3.Set = true;
			}
		}
		break;

	default:
		return -1;
		break;
	}

	return sequenceNo;
}

int CProductSequence::SwitchSetupSequence(int sequenceNo)
{
	switch (sequenceNo)
	{
	case 0:
		//if(smProductEvent->StartSetup.Set == true)
	{
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		Sleep(100);
		smProductProduction->MessageForFirstTime_Setup = false;
		sequenceNo++;
	}
	break;

	case 1:
		if (IsReadyToSetup() == true)
		{
			//m_cProductMotorControl->AgitoMotorSpeed(0, 2000000, 400, 1000000, 1000000);
			if(m_bHighSpeed)
			{
				m_cProductMotorControl->AgitoMotorSpeed(0, 0, 1200000, 500, 10000000, 10000000);
				m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1200000, 500, 10000000, 10000000);
			}
			else
			{
				//700000-->350000 27 Nov 2024
				m_cProductMotorControl->AgitoMotorSpeed(0, 0, 1000000, 500, 10000000, 10000000);
				m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000);
			}
			smProductProduction->nCurrentInputQuantity = 0;
			smProductProduction->nCurrentOutputQuantity = 0;
			smProductProduction->nCurrentRejectQuantity = 0;
			smProductProduction->nCurrentRejectQuantityBasedOnInputTray = 0;
			smProductProduction->nCurrentLowYieldAlarmQuantity = 0;
			smProductEvent->RTHD_GMAIN_UPDATE_REAL_TIME_YEILD.Set = true;
			smProductEvent->RTHD_GMAIN_START_REAL_TIME_UPH.Set = true;
			//smProductProduction->nCurrentOutputQuantity = 0;
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
			lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
			m_cLogger->WriteLog("Setupseq: Ready setup done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
		}
		break;

	case 2:
		if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == true ||
			(smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false
				&& m_cProductIOControl->IsFrontDoorOpen() == false
				&& m_cProductIOControl->IsRearDoorOpen() == false
				&& m_cProductIOControl->IsLeftDoorOpen() == false
				&& m_cProductIOControl->IsRightDoorOpen() == false
				))
		{
			sequenceNo = 4;
		}
		else
		{
			if (m_cProductIOControl->IsFrontDoorOpen() == true)
			{
				m_cProductShareVariables->SetAlarm(5003);
			}
			if (m_cProductIOControl->IsRearDoorOpen() == true)
			{
				m_cProductShareVariables->SetAlarm(5004);
			}
			if (m_cProductIOControl->IsLeftDoorOpen() == true)
			{
				m_cProductShareVariables->SetAlarm(5005);
			}
			if (m_cProductIOControl->IsRightDoorOpen() == true)
			{
				m_cProductShareVariables->SetAlarm(5006);
			}
		}
		break;

	case 3:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			|| (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
			lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
			m_cLogger->WriteLog("Setupseq: Door lock done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Door lock timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo--;
		}
		break;

	case 4:
#pragma region Motor setting up

#pragma endregion
	{
		double dblReturnValue;
		if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true)
		{
			m_cProductMotorControl->THKReadPressureValue(0, &dblReturnValue);
			if (dblReturnValue <= smProductSetting->PickUpHead1Pressure)
			{
				m_cLogger->WriteLog("Setupseq: Pick And Place 1 Pickup Head Unit Present, Please Remove %ums.\n");
				m_cProductShareVariables->SetAlarm(60204);
				RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
				break;
			}
		}
		if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
		{
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			if (dblReturnValue <= smProductSetting->PickUpHead2Pressure)
			{				
				m_cLogger->WriteLog("Setupseq: Pick And Place 2 Pickup Head Unit Present, Please Remove %ums.\n");
				m_cProductShareVariables->SetAlarm(60205);
				RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
				break;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;
	}
	case 5:
		if (smProductCustomize->EnablePickAndPlace1Module == true)
		{
			smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorMoveUpPosition.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module == true)
		{
			smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPosition.Set = true;
		}
		sequenceNo++;
		m_cLogger->WriteLog("Setupseq: PNP Move to Up Position.\n");
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		break;
	case 6:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1 , 2 move up position done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true)
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Setupseq: Door Get Trigger %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
			sequenceNo++;
			break;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1 , 2 move up position Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				if (smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 1 Z Axis Motor move up timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true)
			{
				if (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 2 Z Axis Motor move up timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo++;
		}
		break;
	case 7:
		if (smProductCustomize->EnablePickAndPlace1Module == true)
		{
			smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module  == true)
		{
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;
	case 8:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1,2 stop done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1,2 stop Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				if (smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11010);
					m_cLogger->WriteLog("Setupseq: Pick And Place 1 Z Axis Motor Stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true)
			{
				if (smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12010);
					m_cLogger->WriteLog("Setupseq: Pick And Place 2 Z Axis Motor Stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo--;
		}
		break;
	case 9:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			m_cLogger->WriteLog("Setupseq: Motor setting sequence done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			//sequenceNo--;
		}
		break;

	case 10:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorMoveToStandbyPosition.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;
	case 11:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis move to standby position done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
			{
				smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Setupseq: Door Get Trigger %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
			sequenceNo++;
			break;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis move to standby position Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40002);
				m_cLogger->WriteLog("Setupseq: Pick And Place 1 Y Axis Motor move to standby position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44002);
				m_cLogger->WriteLog("Setupseq: Pick And Place 2 Y Axis Motor move to standby position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo++;
		}
		break;
	case 12:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 13:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis stop done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis stop Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 1 Y Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 2 Y Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo--;
		}
		break;
	case 14:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			if (smProductSetting->EnablePH[0] == true)
			{
				smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorMoveToS1Position.Set = true;
			}
			else
			{
				smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorMoveToParkingPosition.Set = true;
			}

		}
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			if (smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToS3Position.Set = true;
			}
			else
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
			}

		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;
	case 15:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true
				&& ((smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == true) || (smProductSetting->EnablePH[0] == false && smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true
				&& ((smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set == true) || (smProductSetting->EnablePH[1] == false && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true))))
			)
		{
			if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(0, 0, 0)) < smProductTeachPoint->PickAndPlace1XAxisAtS1AndBottomVisionPosition - 200
				|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(0, 0, 0)) > smProductTeachPoint->PickAndPlace1XAxisAtS1AndBottomVisionPosition + 200)
			{			
				RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
				break;
			}

			if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) < smProductTeachPoint->PickAndPlace2XAxisAtS2AndS3VisionPosition - 200
				|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) > smProductTeachPoint->PickAndPlace2XAxisAtS2AndS3VisionPosition + 200)
			{
				
				RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
				break;
			}

			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 X Axis move to standby position done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
			{
				smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
			{
				smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Setupseq: Door Get Trigger %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
			sequenceNo++;
			break;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 X Axis move to standby position Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
			{
				if (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(55002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 1 X Axis Motor move to S1 position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				else if (smProductSetting->EnablePH[0] == false && smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(55002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 1 X Axis Motor move to Parking position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
			{
				if (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(56002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 2 X Axis Motor move to S3 position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				else if (smProductSetting->EnablePH[1] == false && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(56002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 2 X Axis Motor move to Parking position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo++;
		}
		break;
	case 16:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 17:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 X Axis stop done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 X Axis stop Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(55008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 1 X Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(56008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 2 X Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo--;
		}
		break;
	case 18:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			if (smProductSetting->EnablePH[0] == true)
			{
				smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set = false;
				smProductEvent->StartPickAndPlace1YAxisMotorMoveToS1Position.Set = true;
			}
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			if (smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToS3Position.Set = true;
			}

		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;
	case 19:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true
				&& (smProductSetting->EnablePH[0] == false || (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true
				&& (smProductSetting->EnablePH[1] == false || (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis move to pre production standby position done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductSetting->EnablePH[0] == true)
			{
				smProductProduction->PickAndPlace1CurrentStation = PickAndPlaceCurrentStation.BottomStation;
			}
			else
			{
				smProductProduction->PickAndPlace1CurrentStation = PickAndPlaceCurrentStation.DisableStation;
			}
			if (smProductSetting->EnablePH[1] == true)
			{
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.S3Station;
			}
			else
			{
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
			}
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
			{
				if (smProductSetting->EnablePH[0] == true)
				{
					smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
				}
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
			{
				if (smProductSetting->EnablePH[1] == true)
				{
					smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
				}
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Setupseq: Door Get Trigger %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
			sequenceNo++;
			sequenceNo++;
			break;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis move to pre production standby position Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
			{
				if (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(40002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 1 Y Axis Motor move to S1 position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
			{
				if (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(44002);
					m_cLogger->WriteLog("Setupseq: Pick And Place 2 Y Axis Motor move to S3 position timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo++;
		}
		break;
	case 20:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			if (smProductSetting->EnablePH[0] == true)
			{
				smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
			}
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			if (smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 21:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true
				&& (smProductSetting->EnablePH[0] == false || (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true
				&& (smProductSetting->EnablePH[1] == false || (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis stop done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Pick And Place 1, 2 Y Axis stop Timeout %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 1 Y Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44008);
				m_cLogger->WriteLog("Setupseq: Pick And Place 2 Y Axis Motor stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo--;
		}
		break;

	case 22:
		//m_cProductIOControl->SetDiffuserActuatorBacklightOn(false);
		//m_cLogger->WriteLog("Setupseq: Set Diffuser Actuator Backlight Off.\n");
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 23:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingDiffuserActuatorOnOffCompletelyBeforeNextStep_ms)
		{
			m_cLogger->WriteLog("Setupseq: Diffuser Actuator Backlight is Off %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			smProductProduction->bIsDiffuserOn = false;
			sequenceNo++;
		}
		break;

	case 24:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
#pragma region Check if motor standby done

#pragma endregion
			)
		{
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
			RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		}
else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{

			sequenceNo--;
		}
		break;

	case 25:
#pragma region Stop motors

#pragma endregion
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 26:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true

			)
		{
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cProductShareVariables->SetAlarm(4008);
			m_cLogger->WriteLog("Setupseq: Etel, Input Table X, Y, Theta, Z Axis, Output Table X, Y, Input and Output Elevator, Pepper Pot, Aligner X, Y, Theta, Flipper stop timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo--;
		}
		break;

	case 27:
		m_cProductIOControl->SetPNP1VacuumValveOn(true);
		m_cProductIOControl->SetPNP2VacuumValveOn(true);
		sequenceNo++;
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		break;

	case 28:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (m_cProductIOControl->IsPNP1VacuumSwitchReady() == true && m_cProductIOControl->IsPNP2VacuumSwitchReady() == true)
		{
			m_cLogger->WriteLog("Setupseq: PNP 1 2 vacuum on Done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			if (m_cProductIOControl->IsPNP1VacuumSwitchReady() == false)
			{
				m_cProductShareVariables->SetAlarm(5015);
				m_cLogger->WriteLog("Pick And Place 1 Vacuum switch not ready.\n");
			}
			if (m_cProductIOControl->IsPNP2VacuumSwitchReady() == false)
			{
				m_cProductShareVariables->SetAlarm(5016);
				m_cLogger->WriteLog("Pick And Place 2 Vacuum switch not ready.\n");
			}
			sequenceNo--;
		}
		break;

	case 29:
		m_cProductIOControl->SetInputIonizerBlowerValveOn(true);
		m_cProductIOControl->SetOutputIonizerBlowerValveOn(true);
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 30:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			&& m_cProductIOControl->IsInputIonizerBlowerReady() == true && m_cProductIOControl->IsOutputIonizerBlowerReady() == true
			)
		{
			m_cLogger->WriteLog("Setupseq: Input Ionzier and output ionizer blower open Done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			if (m_cProductIOControl->IsInputIonizerBlowerReady() == false)
			{
				m_cProductShareVariables->SetAlarm(5012);
				m_cLogger->WriteLog("Input Ionizer Blower not ready.\n");
			}
			if (m_cProductIOControl->IsOutputIonizerBlowerReady() == false)
			{
				m_cProductShareVariables->SetAlarm(5013);
				m_cLogger->WriteLog("Output Ionizer Blower not ready.\n");
			}
			sequenceNo--;
		}
		break;

	case 31:
#pragma region Input

#pragma endregion

#pragma region Output

#pragma endregion
		sequenceNo++;
		break;

	case 32:

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 33:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_100ms);
			m_cLogger->WriteLog("Setupseq:Input and Output elevator casstte lock done.\n");
			sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
		{
			m_cLogger->WriteLog("Setupseq: Wait cylinder move timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo--;
		}
		break;

	case 34:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);


		for (int i = 0; i < 100; i++)
		{
			smProductProduction->nTime[i] = 0;
		}

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;

		m_cLogger->WriteLog("Setupseq: Parameters setting sequence done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		sequenceNo++;
		//sequenceNo=18;
		//smProductEvent->StartJob.Set = true;
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		break;

	case 35:
		//if (m_nTurretClearUnitCycle <= 4)
		//{
		//	

		//	smProductEvent->RIO_RIPT_OFF_TURRET_PURGE_BIN_PURGE_DONE.Set = false;
		//	smProductEvent->RIPT_RIO_OFF_TURRET_PURGE_BIN_PURGE_START.Set = true;

		//	smProductEvent->RIO_RIPT_OFF_FLIPPER_REJECT_PURGE_DONE.Set = false;
		//	smProductEvent->RIPT_RIO_OFF_FLIPPER_REJECT_PURGE_START.Set = true;
		//	sequenceNo++;
		//}
		//else if (m_nTurretClearUnitCycle <= 24)
		//{
		//	
		//	smProductEvent->RIO_RIPT_OFF_TURRET_PURGE_BIN_PURGE_DONE.Set = false;
		//	smProductEvent->RIPT_RIO_OFF_TURRET_PURGE_BIN_PURGE_START.Set = true;
		//	sequenceNo++;
		//}
		//else
		//{
		//	if (smProductEvent->RIO_RIPT_OFF_FLIPPER_REJECT_PURGE_DONE.Set == false)
		//		break;
		//	if (smProductEvent->RIO_RIPT_OFF_TURRET_PURGE_BIN_PURGE_DONE.Set == false)
		//		break;

		if (smProductSetting->EnableTeachUnitAtVision == true)
		{
			smProductEvent->GMAIN_RTHD_INP_VISION_NEED_LEARN_UNIT.Set = true;
			smProductEvent->GMAIN_RTHD_S2_VISION_NEED_LEARN_UNIT.Set = true;
			smProductEvent->GMAIN_RTHD_BOTTOM_VISION_NEED_LEARN_UNIT.Set = true;
			smProductEvent->GMAIN_RTHD_S1_VISION_NEED_LEARN_UNIT.Set = true;
			smProductEvent->GMAIN_RTHD_S3_VISION_NEED_LEARN_UNIT.Set = true;
			smProductEvent->GMAIN_RTHD_OUT_VISION_NEED_LEARN_UNIT.Set = true;
		}
		else
		{
			smProductEvent->GMAIN_RTHD_INP_VISION_NEED_LEARN_UNIT.Set = false;
			smProductEvent->GMAIN_RTHD_S2_VISION_NEED_LEARN_UNIT.Set = false;
			smProductEvent->GMAIN_RTHD_BOTTOM_VISION_NEED_LEARN_UNIT.Set = false;
			smProductEvent->GMAIN_RTHD_S1_VISION_NEED_LEARN_UNIT.Set = false;
			smProductEvent->GMAIN_RTHD_S3_VISION_NEED_LEARN_UNIT.Set = false;
			smProductEvent->GMAIN_RTHD_OUT_VISION_NEED_LEARN_UNIT.Set = false;
		}
		sequenceNo++;
		sequenceNo++;
		sequenceNo++;
		sequenceNo++;
		sequenceNo++;
		//}
		break;

	case 36:

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		break;

	case 37:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			//if (((lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount) - (LONGLONG)130) < 0)
			if (((lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount) - (LONGLONG)m_TurretDelay) < 0)
			{
				//RtSleep((LONGLONG)130 - (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount));
				//lnSetupSequenceDelayIn100ns.QuadPart = lSetupSequenceConvert1msTo100ns * ((LONGLONG)130 - (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount));
				lnSetupSequenceDelayIn100ns.QuadPart = lSetupSequenceConvert1msTo100ns * ((LONGLONG)m_TurretDelay - (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount));
				RtSleepFt(&lnSetupSequenceDelayIn100ns);
			}
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cProductShareVariables->SetAlarm(4008);
			m_cLogger->WriteLog("Setupseq: Turret and Flipper motor index timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
		}
		break;

	case 38:

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 39:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			m_cLogger->WriteLog("Setupseq: Turret and Flipper Motors Stop Done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{

			sequenceNo--;
		}
		break;

	case 40:

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 41:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
			//sequenceNo++;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cProductShareVariables->SetAlarm(4008);
			m_cLogger->WriteLog("Setupseq: Turret and Flipper motor index timeout %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
		}
		break;

	case 42:

		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockStart2);
		sequenceNo++;
		break;

	case 43:
		RtGetClockTime(CLOCK_FASTEST, &lnSetupSequenceClockEnd);
		lnSetupSequenceClockSpan.QuadPart = lnSetupSequenceClockEnd.QuadPart - lnSetupSequenceClockStart2.QuadPart;
		if (true
			)
		{
			m_cLogger->WriteLog("Setupseq: Turret and Flipper Motors Stop Done %ums.\n", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{

			sequenceNo--;
		}
		break;

	case 44:
		if (smProductGeneral->nLoginAuthority < 2)
		{
			//if (smProductProduction->nUnitHasBeenPlacedInCanister != 0)

		}
		sequenceNo = 999;
		break;

	default:
		return -1;
		break;
	}
	return sequenceNo;
}

int CProductSequence::SwitchEndingSequence(int sequenceNo)
{
	switch (sequenceNo)
	{

	case 0:
		if (smProductEvent->StartEnding.Set == true)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockStart2);
			sequenceNo++;
		}
		break;

	case 1:
		
		{
			sequenceNo++;
		}
		break;

	case 2:
		
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockStart2);
		sequenceNo++;
		break;

	case 3:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockEnd);
		lnEndingSequenceClockSpan.QuadPart = lnEndingSequenceClockEnd.QuadPart - lnEndingSequenceClockStart2.QuadPart;
		if (true
			)
		{
			sequenceNo++;
			sequenceNo++;
			sequenceNo++;
		}
		
		else if (lnEndingSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			
			sequenceNo--;
		}
		break;

	case 4:
		
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockStart2);
		sequenceNo++;
		break;

	case 5:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockEnd);
		lnEndingSequenceClockSpan.QuadPart = lnEndingSequenceClockEnd.QuadPart - lnEndingSequenceClockStart2.QuadPart;
		if (true
			//&& (smProductCustomize->EnableTransferArmMotor == false || (smProductCustomize->EnableTransferArmMotor == true && smProductEvent->TransferArmMotorStopDone.Set == true))
			)
		{
			sequenceNo--;
			sequenceNo--;
			sequenceNo--;
		}
		else if (lnEndingSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			
			sequenceNo--;
		}
		break;

	case 6:
		if (true
			//&& smProductEvent->InputModuleEndingDone.Set == true
			)
		{
			sequenceNo++;
		}
		break;

	case 7:
		if (true
			//&& smProductEvent->SortingModuleEndingDone.Set == true
			)
		{
			sequenceNo++;
		}
		break;

	case 8:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockStart2);

		
		sequenceNo++;
		break;

	case 9:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockEnd);
		lnEndingSequenceClockSpan.QuadPart = lnEndingSequenceClockEnd.QuadPart - lnEndingSequenceClockStart2.QuadPart;
		if (true
			)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_100ms);
			m_cLogger->WriteLog("Endingseq:Cassettes lock cylinder unlock done.\n");
			sequenceNo++;
		}
		else if (lnEndingSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
		{
			
			sequenceNo--;
		}
		break;

	case 10:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockStart2);

		

		sequenceNo++;
		break;

	case 11:
		RtGetClockTime(CLOCK_FASTEST, &lnEndingSequenceClockEnd);
		lnEndingSequenceClockSpan.QuadPart = lnEndingSequenceClockEnd.QuadPart - lnEndingSequenceClockStart2.QuadPart;
		//if (true
		//	&& (IsDoorLockSensorOffAndDoorUnlockSensorOn() == true)

		//	)
		//{
		RtSleepFt(&m_cProductShareVariables->m_lnPeriod_100ms);
		m_cLogger->WriteLog("Endingseq:Door unlock done.\n");
		sequenceNo = 999;
		//}
		//else if (lnEndingSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
		//{
		//	m_cProductShareVariables->SetAlarm(5425);
		//	//m_cLogger->WriteLog("Homeseq: Turret cover not at done position.\n");
		//	m_cLogger->WriteLog("Endingseq: Wait door unlock cylinder timeout %ums.\n", lnEndingSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		//	sequenceNo--;
		//}
		break;

	default:
		return -1;
		break;
	}
	return sequenceNo;
}

int CProductSequence::SwitchMaintenanceSequence(int sequenceNo)
{
	MaintenanceSequenceNo nCase;
	double dbCurrentOffset1X;
	double dbCurrentOffset1Y;
	double dbPreviousOffset1X;
	double dbPreviousOffset1Y;
	double dbCurrentOffset2X;
	double dbCurrentOffset2Y;
	double dbPreviousOffset2X;
	double dbPreviousOffset2Y;
	//int nCurrentX = 0;
	//int nCurrentY = 0;
	switch (sequenceNo)
	{
	case nCase.StartMaintenanceSequence:
		if (smProductEvent->JobMode.Set == true)
		{
			m_cLogger->WriteLog("MaintenanceSeq: Start.\n");
			sequenceNo = nCase.CheckWhichToMove;
		}
		break;

	case nCase.CheckWhichToMove:
		if (smProductProduction->StationSelectedForCalibration == 1)
		{
			m_cLogger->WriteLog("MaintenanceSeq: Turn PnP Theta.\n");
			sequenceNo = nCase.StartTurnThetaAxisToNextPosition;
		}
		else if (smProductProduction->StationSelectedForCalibration == 2)
		{
			m_cLogger->WriteLog("MaintenanceSeq: Move PnP To Bottom.\n");
			sequenceNo = nCase.StartRetractPnPYToStandbyPosition;
		}
		else if (smProductProduction->StationSelectedForCalibration == 3)
		{
			if (smProductGeneral->MaintenanceID == 1)
			{
				m_cLogger->WriteLog("MaintenanceSeq: Turn On Vacuum 1.\n");
				sequenceNo = nCase.StartVacuumOnPnp1;
			}
			else
			{
				m_cLogger->WriteLog("MaintenanceSeq: Turn On Vacuum 1.\n");
				sequenceNo = nCase.StartVacuumOffPnp1;
			}
			
		}
		else if (smProductProduction->StationSelectedForCalibration == 4)
		{
			if (smProductGeneral->MaintenanceID == 1)
			{
				m_cLogger->WriteLog("MaintenanceSeq: Turn On Vacuum 1.\n");
				sequenceNo = nCase.StartVacuumOnPnp2;
			}
			else
			{
				m_cLogger->WriteLog("MaintenanceSeq: Turn On Vacuum 1.\n");
				sequenceNo = nCase.StartVacuumOffPnp2;
			}
		}
		break;

	case nCase.StartRetractPnPYToStandbyPosition:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorMoveToStandbyPosition.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
		}
		sequenceNo = nCase.IsRetractPnPYToStandbyPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockStart2);
		break;

	case nCase.IsRetractPnPYToStandbyPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockEnd);
		lnMaintenanceSequenceClockSpan.QuadPart = lnMaintenanceSequenceClockEnd.QuadPart - lnMaintenanceSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true))
			)
		{
			m_cLogger->WriteLog("MaintenanceSeq: Move Y Axis move to standby position done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMovePnPXToBottomAndParkingPosition;
		}
		break;

	case nCase.StartMovePnPXToBottomAndParkingPosition:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			if (smProductGeneral->MaintenanceID == 1)
			{
				smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorMoveToS1Position.Set = true;
			}
			else
			{
				smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorMoveToParkingPosition.Set = true;
			}

		}
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			if (smProductGeneral->MaintenanceID == 2)
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToS1Position.Set = true;
			}
			else
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
			}

		}
		sequenceNo = nCase.IsMovePnPXToBottomAndParkingPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockStart2);
		break;

	case nCase.IsMovePnPXToBottomAndParkingPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockEnd);
		lnMaintenanceSequenceClockSpan.QuadPart = lnMaintenanceSequenceClockEnd.QuadPart - lnMaintenanceSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true
				&& ((smProductGeneral->MaintenanceID == 1 && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == true) || (smProductGeneral->MaintenanceID != 1 && smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true
				&& ((smProductGeneral->MaintenanceID == 2 && smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set == true) || (smProductGeneral->MaintenanceID != 2 && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("MaintenanceSeq: Move Y Axis move to standby position done %ums.", lnSetupSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMovePnPYToBottomAndParkingPosition;
		}
		break;

	case nCase.StartMovePnPYToBottomAndParkingPosition:
		if (smProductGeneral->MaintenanceID == 1)
		{
			smProductProduction->PickAndPlace1ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace1ThetaAxisStandbyPosition;
			smProductProduction->PickAndPlace1YAxisMovePosition = smProductTeachPoint->PickAndPlace1YAxisAtS1AndBottomVisionPosition;
			smProductProduction->PickAndPlace1ZAxisMovePosition = smProductTeachPoint->PickAndPlace1ZAxisUpPosition2
				+ smProductSetting->UnitThickness_um;

			smProductEvent->PickAndPlace1YAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorMoveUpPositionAndRotate.Set = true;
		}
		else if (smProductGeneral->MaintenanceID == 2)
		{
			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition;
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition2
				+ smProductSetting->UnitThickness_um;

			smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
		}
		sequenceNo = nCase.IsMovePnPYToBottomAndParkingPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockStart2);
		break;

	case nCase.IsMovePnPYToBottomAndParkingPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockEnd);
		lnMaintenanceSequenceClockSpan.QuadPart = lnMaintenanceSequenceClockEnd.QuadPart - lnMaintenanceSequenceClockStart2.QuadPart;
		if (true
			&& ((smProductGeneral->MaintenanceID == 1 && smProductEvent->PickAndPlace1YAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| (smProductGeneral->MaintenanceID == 2 && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)))
		{
			smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
			sequenceNo = 999; //End
		}
		break;

	case nCase.StartTurnThetaAxisToNextPosition:
		if (smProductEvent->JobStop.Set == true)
		{
			sequenceNo = 999;
			smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
			break;
		}
		
		if (smProductGeneral->MaintenanceID == 1)
		{
			smProductProduction->PickAndPlace1ThetaAxisMovePosition += smProductProduction->PnP1AngleMaintainance;
			GetNewXYOffsetFromThetaCorretionMaintainance(smProductProduction->PickAndPlace1ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace1ThetaAxisStandbyPosition,
				smProductGeneral->MaintenanceID, &smProductProduction->dbCurrentOffset1X, &smProductProduction->dbCurrentOffset1Y, false);

			smProductProduction->PickAndPlace1XAxisMovePosition = smProductTeachPoint->PickAndPlace1XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset1X);
			smProductProduction->PickAndPlace1YAxisMovePosition = smProductTeachPoint->PickAndPlace1YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset1Y);

			smProductProduction->PickAndPlace1ZAxisMovePosition = smProductTeachPoint->PickAndPlace1ZAxisUpPosition2
				+ smProductSetting->UnitThickness_um;

			smProductEvent->PickAndPlace1YAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace1XAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace1XAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorMoveUpPositionAndRotate.Set = true;

			smProductProduction->PnPCurrentAngle = smProductProduction->PickAndPlace1ThetaAxisMovePosition;

			m_cLogger->WriteLog("MaintenanceSeq%d: Maintainance.\n", 1);
			m_cLogger->WriteLog("PickAndPlace1XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace1XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace1YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace1YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace1ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace1ThetaAxisMovePosition));

			
		}
		else if (smProductGeneral->MaintenanceID == 2)
		{
			smProductProduction->PickAndPlace2ThetaAxisMovePosition += smProductProduction->PnP2AngleMaintainance;
			GetNewXYOffsetFromThetaCorretionMaintainance(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				smProductGeneral->MaintenanceID, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition2
				+ smProductSetting->UnitThickness_um;

			smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;

			smProductProduction->PnPCurrentAngle = smProductProduction->PickAndPlace2ThetaAxisMovePosition;

			m_cLogger->WriteLog("MaintenanceSeq%d: Maintainance.\n", 2);
			m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));

		}
		sequenceNo = nCase.IsTurnThetaAxisToNextPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockStart2);
		break;

	case nCase.IsTurnThetaAxisToNextPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnMaintenanceSequenceClockEnd);
		lnMaintenanceSequenceClockSpan.QuadPart = lnMaintenanceSequenceClockEnd.QuadPart - lnMaintenanceSequenceClockStart2.QuadPart;
		if (smProductGeneral->MaintenanceID == 1)
		{
			if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0]
				&& (smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductEvent->PickAndPlace1XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace1YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false)
				&& (smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace1Module == false || smProductSetting->EnablePH[0] == false)
			{
				smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
				sequenceNo = 999; //End
			}
		}
		else
		{
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
				sequenceNo = 999; //End
			}
		}
		break;

	case nCase.StartVacuumOnPnp1:
		m_cProductMotorControl->THKVacuumValveOn(0);
		smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
		sequenceNo = 999; //End
		break;

	case nCase.StartVacuumOffPnp1:
		m_cProductMotorControl->THKVacuumValveOff(0);
		smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
		sequenceNo = 999; //End
		break;

	case nCase.StartVacuumOnPnp2:
		m_cProductMotorControl->THKVacuumValveOn(1);
		smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
		sequenceNo = 999; //End
		break;

	case nCase.StartVacuumOffPnp2:
		m_cProductMotorControl->THKVacuumValveOff(1);
		smProductEvent->GMAIN_RMAIN_STOP_CALIBRATION.Set = true;
		sequenceNo = 999; //End
		break;
	
	default:
		return -1;
		break;
	}
	return sequenceNo;
}


#pragma region Application Method

#pragma region Input XY Table
#pragma endregion
int CProductSequence::GetNewXYOffsetFromThetaCorretion(double thetaCorrection_mDegree, int PickupHeadNo, double* newXOffset, double* newYOffset, bool AddToSleeve, bool relativeToSleeve)
{
	int nError = 0;
	double dCurrentTheta_miliDegree = 0;
	double dTargetTheta_miliDegree = 0;
	double XOffset = 0;
	double YOffset = 0;
	double XOffset1 = 0;
	double YOffset1 = 0;

	if (relativeToSleeve == true)
	{
		double InputSleeveAndShaftOffsetX = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputSleeveXOffset_um - (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputXOffset_um;
		double InputSleeveAndShaftOffsetY = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputSleeveYOffset_um - (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputYOffset_um;
		
		GetNewXYOffsetFromThetaCorrectionBasic((double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputThetaOffset_mDegree, InputSleeveAndShaftOffsetX, InputSleeveAndShaftOffsetY, &XOffset1, &YOffset1);
		XOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomXOffset_um + XOffset1 - (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
		YOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomYOffset_um + YOffset1 - (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);
	}
	else if (AddToSleeve)
	{
		XOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomXOffset_um + (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].SelveXOffset_um - (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
		YOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomYOffset_um + (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].SelveYOffset_um - (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);
	}
	else
	{
		XOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomXOffset_um - (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
		YOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomYOffset_um - (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);
	}


	if (XOffset == 0.0 && YOffset == 0.0)
	{
		*newXOffset = 0;
		*newYOffset = 0;
		return nError;
	}
	dCurrentTheta_miliDegree = atan2(YOffset, XOffset) / 3.1415926536*180000.0;
	dTargetTheta_miliDegree = dCurrentTheta_miliDegree - thetaCorrection_mDegree;

	if (dTargetTheta_miliDegree >= 0.0)
	{
		if ((dTargetTheta_miliDegree / 360000.0) > 1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree - ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}
	else
	{
		if ((dTargetTheta_miliDegree / 360000.0) < -1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree + ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}

	*newXOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * cos(dTargetTheta_miliDegree / 180000.0*3.1415926536)) + (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
	*newYOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * sin(dTargetTheta_miliDegree / 180000.0*3.1415926536)) + (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);

	if (*newXOffset < -1000.00 || *newXOffset > 1000.00)
		*newXOffset = 0;
	if (*newYOffset < -1000.00 || *newYOffset > 1000.00)
		*newYOffset = 0;
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorretion]: dCurrentTheta_miliDegree = %lf , dTargetTheta_miliDegree = %lf.\n", dCurrentTheta_miliDegree, dTargetTheta_miliDegree);
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorretion]: newXOffset = %lf , newYOffset = %lf.\n", *newXOffset, *newYOffset);
	return nError;
}

int CProductSequence::GetNewXYOffsetFromThetaCorretionMaintainance(double thetaCorrection_mDegree, int PickupHeadNo, double* newXOffset, double* newYOffset, bool relativeToSleeve)
{
	int nError = 0;
	double dCurrentTheta_miliDegree = 0;
	double dTargetTheta_miliDegree = 0;
	double XOffset = 0;
	double YOffset = 0;
	double XOffset1 = 0;
	double YOffset1 = 0;

	if (relativeToSleeve == true)
	{
		double InputSleeveAndShaftOffsetX = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputSleeveXOffset_um - (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputXOffset_um;
		double InputSleeveAndShaftOffsetY = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputSleeveYOffset_um - (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputYOffset_um;

		GetNewXYOffsetFromThetaCorrectionBasic((double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].InputThetaOffset_mDegree, InputSleeveAndShaftOffsetX, InputSleeveAndShaftOffsetY, &XOffset1, &YOffset1);
		XOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomXOffset_um + XOffset1 - (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
		YOffset = (double)smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHeadNo - 1].BottomYOffset_um + YOffset1 - (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);
	}
	else
	{
		XOffset = (double)smProductProduction->BottomXAxisoffsetMaintainance - (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
		YOffset = (double)smProductProduction->BottomYAxisoffsetMaintainance - (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);
	}


	if (XOffset == 0.0 && YOffset == 0.0)
	{
		*newXOffset = 0;
		*newYOffset = 0;
		return nError;
	}
	dCurrentTheta_miliDegree = atan2(YOffset, XOffset) / 3.1415926536*180000.0;
	dTargetTheta_miliDegree = dCurrentTheta_miliDegree - thetaCorrection_mDegree;

	if (dTargetTheta_miliDegree >= 0.0)
	{
		if ((dTargetTheta_miliDegree / 360000.0) > 1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree - ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}
	else
	{
		if ((dTargetTheta_miliDegree / 360000.0) < -1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree + ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}

	*newXOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * cos(dTargetTheta_miliDegree / 180000.0*3.1415926536)) + (double)(smProductSetting->PickUpHeadRotationXOffset[PickupHeadNo - 1]);
	*newYOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * sin(dTargetTheta_miliDegree / 180000.0*3.1415926536)) + (double)(smProductSetting->PickUpHeadRotationYOffset[PickupHeadNo - 1]);

	if (*newXOffset < -1000.00 || *newXOffset > 1000.00)
		*newXOffset = 0;
	if (*newYOffset < -1000.00 || *newYOffset > 1000.00)
		*newYOffset = 0;
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorretion]: dCurrentTheta_miliDegree = %lf , dTargetTheta_miliDegree = %lf.\n", dCurrentTheta_miliDegree, dTargetTheta_miliDegree);
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorretion]: newXOffset = %lf , newYOffset = %lf.\n", *newXOffset, *newYOffset);
	return nError;
}


int CProductSequence::GetNewXYOffsetFromThetaCorrectionBasic(double thetaCorrection_mDegree, double XOffset, double YOffset, double* newXOffset, double* newYOffset)
{
	int nError = 0;
	double dCurrentTheta_miliDegree = 0;
	double dTargetTheta_miliDegree = 0;

	if (XOffset == 0.0 && YOffset == 0.0)
	{
		*newXOffset = 0;
		*newYOffset = 0;
		return nError;
	}
	dCurrentTheta_miliDegree = atan2(YOffset, XOffset) / 3.1415926536*180000.0;
	dTargetTheta_miliDegree = dCurrentTheta_miliDegree + thetaCorrection_mDegree;

	if (dTargetTheta_miliDegree >= 0.0)
	{
		if ((dTargetTheta_miliDegree / 360000.0) > 1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree - ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}
	else
	{
		if ((dTargetTheta_miliDegree / 360000.0) < -1.0)
			dTargetTheta_miliDegree = dTargetTheta_miliDegree + ((double)((int)(dTargetTheta_miliDegree / 360000.0))) * 360000.0;
	}

	*newXOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * cos(dTargetTheta_miliDegree / 180000.0*3.1415926536));
	*newYOffset = ((sqrt(XOffset* XOffset + YOffset * YOffset)) * sin(dTargetTheta_miliDegree / 180000.0*3.1415926536));

	if (*newXOffset < -1000.00 || *newXOffset > 1000.00)
		*newXOffset = 0;
	if (*newYOffset < -1000.00 || *newYOffset > 1000.00)
		*newYOffset = 0;
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorrectionBasic]: dCurrentTheta_miliDegree = %lf , dTargetTheta_miliDegree = %lf.\n", dCurrentTheta_miliDegree, dTargetTheta_miliDegree);
	m_cLogger->WriteLog("[GetNewXYOffsetFromThetaCorrectionBasic]: newXOffset = %lf , newYOffset = %lf.\n", *newXOffset, *newYOffset);
	return nError;
}

int CProductSequence::GetNewXYOffsetFromLookUpTable1(double thetaCorrection_mDegree, double* newXOffset, double* newYOffset)
{
	if (thetaCorrection_mDegree >= 360000)
	{
		thetaCorrection_mDegree = thetaCorrection_mDegree - 360000;
	}
	else if (thetaCorrection_mDegree < 0)
	{
		thetaCorrection_mDegree = 360000 + thetaCorrection_mDegree;
	}
	thetaCorrection_mDegree = thetaCorrection_mDegree / 1000;
	for (int i = 0; i < 360; i++)
	{
		if (smProductProduction->PickAndPlaceLookUpTableData1[i].Angle <= thetaCorrection_mDegree)
		{
			smProductProduction->LookUpTableCollectDown1 = smProductProduction->PickAndPlaceLookUpTableData1[i];
		}
		if (smProductProduction->PickAndPlaceLookUpTableData1[i].Angle >= thetaCorrection_mDegree)
		{
			smProductProduction->LookUpTableCollectUp1 = smProductProduction->PickAndPlaceLookUpTableData1[i];
			break;
		}
	}
	if (smProductProduction->LookUpTableCollectDown1.Angle != smProductProduction->LookUpTableCollectUp1.Angle)
	{
		double ratio = thetaCorrection_mDegree - smProductProduction->LookUpTableCollectDown1.Angle;

		double interpolatedXOffset = smProductProduction->LookUpTableCollectDown1.XOffset + ratio * (smProductProduction->LookUpTableCollectUp1.XOffset - smProductProduction->LookUpTableCollectDown1.XOffset);
		double interpolatedYOffset = smProductProduction->LookUpTableCollectDown1.YOffset + ratio * (smProductProduction->LookUpTableCollectUp1.YOffset - smProductProduction->LookUpTableCollectDown1.YOffset);

		*newXOffset = interpolatedXOffset * -1;
		*newYOffset = interpolatedYOffset * -1;

	}
	else
	{
		*newXOffset = smProductProduction->LookUpTableCollectDown1.XOffset * -1;
		*newYOffset = smProductProduction->LookUpTableCollectDown1.YOffset * -1;
	}
	return 0;
}

int CProductSequence::GetNewXYOffsetFromLookUpTable2(double thetaCorrection_mDegree, double* newXOffset, double* newYOffset)
{
	if (thetaCorrection_mDegree >= 360000)
	{
		thetaCorrection_mDegree = thetaCorrection_mDegree - 360000;
	}
	else if (thetaCorrection_mDegree < 0)
	{
		thetaCorrection_mDegree = 360000 + thetaCorrection_mDegree;
	}
	thetaCorrection_mDegree = thetaCorrection_mDegree / 1000;
	for (int i = 0; i < 360; i++)
	{
		if (smProductProduction->PickAndPlaceLookUpTableData2[i].Angle <= thetaCorrection_mDegree)
		{
			smProductProduction->LookUpTableCollectDown2 = smProductProduction->PickAndPlaceLookUpTableData2[i];
		}
		if (smProductProduction->PickAndPlaceLookUpTableData2[i].Angle >= thetaCorrection_mDegree)
		{
			smProductProduction->LookUpTableCollectUp2 = smProductProduction->PickAndPlaceLookUpTableData2[i];
			break;
		}
	}
	if (smProductProduction->LookUpTableCollectDown2.Angle != smProductProduction->LookUpTableCollectUp2.Angle)
	{
		double ratio = thetaCorrection_mDegree - smProductProduction->LookUpTableCollectDown2.Angle;

		double interpolatedXOffset = smProductProduction->LookUpTableCollectDown2.XOffset + ratio * (smProductProduction->LookUpTableCollectUp2.XOffset - smProductProduction->LookUpTableCollectDown2.XOffset);
		double interpolatedYOffset = smProductProduction->LookUpTableCollectDown2.YOffset + ratio * (smProductProduction->LookUpTableCollectUp2.YOffset - smProductProduction->LookUpTableCollectDown2.YOffset);

		*newXOffset = interpolatedXOffset * -1;
		*newYOffset = interpolatedYOffset * -1;

	}
	else
	{
		*newXOffset = smProductProduction->LookUpTableCollectDown2.XOffset * -1;
		*newYOffset = smProductProduction->LookUpTableCollectDown2.YOffset * -1;
	}
	return 0;
}

bool CProductSequence::IsPickAndPlaceModuleReadyToPickAtInput()
{
	
	return false;
}
bool CProductSequence::IsPickAndPlaceModuleReadyToPlaceAtOutput()
{
	
	return false;
}
bool CProductSequence::IsInputVisionReadyToGetOffset() //Chekc is PnP module at there
{
	
	return false;
}
bool CProductSequence::IsOutputVisionReadyToGetOffset() //Chekc is PnP module at there
{
	
	return false;
}
bool CProductSequence::IsCurrentPickupHeadNoUnit(int nNoOfPickupHead)
{
	double pressurevalue = 0;
	m_cProductMotorControl->THKReadPressureValue(nNoOfPickupHead, &pressurevalue);
	if (pressurevalue > -25.0)
	{
		return true;
	}
	return false;
}
bool CProductSequence::IsPickupHeadNoUnit()
{
	return true;
}
bool CProductSequence::IsInputTableNoUnit()
{
	return true;
}
#pragma endregion

int CProductSequence::SwitchHomeSequence(int sequenceNo)
{
	HomingSequenceNo nCase;
	int nError = 0;
	bool HomePickupHeadFirst;
	bool TemporaryDisable = true;
	signed long lngPnP1ZAxisPos = 0;
	signed long lngPnP2ZAxisPos = 0;
	//int InOutTrayUnloadingNo = 0;
	//LARGE_INTEGER lnHomeSequenceClockEnd, lnHomeSequenceClockSpan, lnHomeSequenceClockStart2;
	switch (sequenceNo)
	{
	case nCase.CheckWhetherGetUnitOnPickAndPlace:
		if (smProductEvent->JobMode.Set == true)
		{
			double dblReturnValue;
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart);
			m_cLogger->WriteLog("Homeseq: Homing start.\n");
			if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true)
			{
				m_cProductMotorControl->THKReadPressureValue(0, &dblReturnValue);
				if (dblReturnValue <= smProductSetting->PickUpHead1Pressure)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
					lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart.QuadPart;
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Pickup Head Unit Present, Please Remove %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
					{
						smProductEvent->PickAndPlace1XAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1XAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
					{
						smProductEvent->PickAndPlace1YAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1YAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
					{
						smProductEvent->PickAndPlace2XAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2XAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
					{
						smProductEvent->PickAndPlace2YAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2YAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace1Module == true)
					{
						smProductEvent->PickAndPlace1THKMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1THKMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2Module == true)
					{
						smProductEvent->PickAndPlace2THKMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2THKMotorOff.Set = true;
					}
					//PickUpHeadNo = 0;
					m_cProductShareVariables->SetAlarm(60204);
					RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
					sequenceNo = nCase.IsPickAndPlace1XYMotorOffDone;
					break;
				}
			}
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart);
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
			{
				m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
				if (dblReturnValue <= smProductSetting->PickUpHead2Pressure)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
					lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart.QuadPart;
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Pickup Head Unit Present, Please Remove %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
					{
						smProductEvent->PickAndPlace1XAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1XAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
					{
						smProductEvent->PickAndPlace1YAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1YAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
					{
						smProductEvent->PickAndPlace2XAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2XAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
					{
						smProductEvent->PickAndPlace2YAxisMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2YAxisMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace1Module == true)
					{
						smProductEvent->PickAndPlace1THKMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace1THKMotorOff.Set = true;
					}
					if (smProductCustomize->EnablePickAndPlace2Module == true)
					{
						smProductEvent->PickAndPlace2THKMotorOffDone.Set = false;
						smProductEvent->StartPickAndPlace2THKMotorOff.Set = true;
					}
					//PickUpHeadNo = 1;
					m_cProductShareVariables->SetAlarm(60205);
					RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
					sequenceNo = nCase.IsPickAndPlace1XYMotorOffDone;
					break;
				}
			}
			sequenceNo = nCase.ResetVariablesBeforeHoming;
		}
		break;

	case nCase.IsPickAndPlace1XYMotorOffDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorOffDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorOffDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorOffDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorOffDone.Set == true))
			&&(smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1THKMotorOffDone.Set == true))
			&&(smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2THKMotorOffDone.Set == true))
			)
		{
			sequenceNo = nCase.CheckWhetherGetUnitOnPickAndPlace;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(55011);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace1XAxisMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorOff.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40011);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace1YAxisMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace1YAxisMotorOff.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(56011);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace2XAxisMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorOff.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44011);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace2YAxisMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorOff.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1THKMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(11025);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 THK Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace1THKMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace1THKMotorOff.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2THKMotorOffDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(12025);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 THK Motor Off timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				smProductEvent->PickAndPlace2THKMotorOffDone.Set = false;
				smProductEvent->StartPickAndPlace2THKMotorOff.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			}
		}
		break;

	case nCase.ResetVariablesBeforeHoming:
		ResetVariablesBeforeHome();
		smProductEvent->Homed.Set = true;

		HomePickupHeadFirst = false;
		if(m_bHighSpeed)
		{
			nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 1200000, 500, 10000000, 10000000);
			nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1200000, 500, 10000000, 10000000);
		}
		else
		{
			nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 1000000, 500, 10000000, 10000000);
			nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000);
		}
		sequenceNo = nCase.InitializeBeforeHoming;
		break;

	case nCase.InitializeBeforeHoming:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		InitializeBeforeHome();
		//m_cProductMotorControl->AgitoPositionEventTriggerDisable(0);
		m_cLogger->WriteLog("Homeseq: Initialize before home done.\n");
		sequenceNo = nCase.CheckMainPressureDoorAndMotorStatus;
		break;

	case nCase.CheckMainPressureDoorAndMotorStatus:
		RtSleep(150);
		if (IsReadyToHomeOrSetup() == true)
		{
			m_cLogger->WriteLog("Homeseq: Machine ready to home.\n");
			SetUpBeforeHome();
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.StartLockInputOutputStackerCylinderAndMoveBottomVisionBacklightUpCylinderDown;
		}
		else
		{
			m_cLogger->WriteLog("Homeseq: Machine Not Ready to home.\n");
			sequenceNo = nCase.InitializeBeforeHoming;
		}
		break;

	case nCase.StartLockInputOutputStackerCylinderAndMoveBottomVisionBacklightUpCylinderDown:
		m_cProductIOControl->SetInputLoadingStackerUnlockCylinderOn(false);
		m_cProductIOControl->SetInputUnloadingStackerUnlockCylinderOn(false);
		m_cProductIOControl->SetOutputLoadingStackerUnlockCylinderOn(false);
		m_cProductIOControl->SetOutputUnloadingStackerUnlockCylinderOn(false);
		//m_cProductIOControl->SetBottomVisionBacklightUpCylinderOn(false);
		m_cLogger->WriteLog("Homeseq: Start Input Output Stacker Cylinder Lock and Bottom Vision Backlight Up Cylinder move down.\n");
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsLockInputOutputStackerCylinderAndMoveBottomVisionBacklightUpCylinderDownDone;
		break;

	case nCase.IsLockInputOutputStackerCylinderAndMoveBottomVisionBacklightUpCylinderDownDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (m_cProductIOControl->IsInputLoadingStackerLockSensorOn() == true
			&& m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == true
			&& m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == true
			&& m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == true)
			//&& m_cProductIOControl->IsBottomVisionBacklightDownSensorOn() == true)
		{
			sequenceNo = nCase.StartInitializeMotionController;
			m_cLogger->WriteLog("Homeseq: Input Output Stacker Cylinder Lock and Bottom Vision Backlight Up Cylinder move down Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Output Stacker Cylinder Lock and Bottom Vision Backlight Up Cylinder move down timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (m_cProductIOControl->IsInputLoadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5403);
				m_cLogger->WriteLog("Homeseq: Input Loading Stacker Cylinder Lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			}
			if (m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5407);
				m_cLogger->WriteLog("Homeseq: Input Unloading Stacker Cylinder Lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5411);
				m_cLogger->WriteLog("Homeseq: Output Loading Stacker Cylinder Lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5415);
				m_cLogger->WriteLog("Homeseq: Output Unloading Stacker Cylinder Lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartLockInputOutputStackerCylinderAndMoveBottomVisionBacklightUpCylinderDown;

		}
		break;
	case nCase.StartInitializeMotionController:
		if (smProductCustomize->EnableMotionController1 == true)
		{
			smProductEvent->InitializeMotionController1Done.Set = false;
			smProductEvent->StartInitializeMotionController1.Set = true;
		}
		if (smProductCustomize->EnableMotionController2 == true)
		{
			smProductEvent->InitializeMotionController2Done.Set = false;
			smProductEvent->StartInitializeMotionController2.Set = true;
		}
		if (smProductCustomize->EnableMotionController3 == true)
		{
			smProductEvent->InitializeMotionController3Done.Set = false;
			smProductEvent->StartInitializeMotionController3.Set = true;
		}
		//if (smProductCustomize->EnableMotionController4 == true)
		//{
		//	smProductEvent->InitializeMotionController4Done.Set = false;
		//	smProductEvent->StartInitializeMotionController4.Set = true;
		//}

		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsInitializeMotionControllerDone;
		break;

	case nCase.IsInitializeMotionControllerDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableMotionController1 == false || (smProductCustomize->EnableMotionController1 == true && smProductEvent->InitializeMotionController1Done.Set == true))
			&& (smProductCustomize->EnableMotionController2 == false || (smProductCustomize->EnableMotionController2 == true && smProductEvent->InitializeMotionController2Done.Set == true))
			&& (smProductCustomize->EnableMotionController3 == false || (smProductCustomize->EnableMotionController3 == true && smProductEvent->InitializeMotionController3Done.Set == true))
			//&& (smProductCustomize->EnableMotionController4 == false || (smProductCustomize->EnableMotionController4 == true && smProductEvent->InitializeMotionController4Done.Set == true))
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
			lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
			m_cLogger->WriteLog("Homeseq: Initialize controller Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartPickAndPlaceZMotorHome;

		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			//Alarm
			m_cProductShareVariables->SetAlarm(4009);
			m_cLogger->WriteLog("Homeseq: Controller initialize timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableMotionController1 == true && smProductEvent->InitializeMotionController1Done.Set == false)
				m_cLogger->WriteLog("Homeseq: Controller1 initialize timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableMotionController2 == true && smProductEvent->InitializeMotionController2Done.Set == false)
				m_cLogger->WriteLog("Homeseq: Controller2 initialize timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableMotionController3 == true && smProductEvent->InitializeMotionController3Done.Set == false)
				m_cLogger->WriteLog("Homeseq: Controller3 initialize timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//if (smProductCustomize->EnableMotionController4 == true && smProductEvent->InitializeMotionController4Done.Set == false)
			//	m_cLogger->WriteLog("Homeseq: Controller4 initialize timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo = -1;
		}
		break;
	case nCase.StartPickAndPlaceZMotorHome:
		if (smProductCustomize->EnablePickAndPlace1Module)
		{
			smProductEvent->PickAndPlace1ZAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module)
		{
			smProductEvent->PickAndPlace2ZAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorHome.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsPickAndPlaceZMotorHomeDone;
		break;

	case nCase.IsPickAndPlaceZMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ZAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ZAxisMotorHomeDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Z Axis Home Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//sequenceNo++;
			//sequenceNo++;
			//sequenceNo++;
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsPickAndPlaceAndHomePosition;
		}
		else if (IsReadyToMove() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1Module)
			{
				smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module)
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopPickAndPlaceZMotorHomeDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Z Axis Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1Module && smProductEvent->PickAndPlace1ZAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(11006);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Pickup Head Z Axis Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductEvent->PickAndPlace2ZAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(12006);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Pickup Head Z Axis Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopPickAndPlaceZMotorHome;
		}
		break;

	case nCase.StartStopPickAndPlaceZMotorHome:
		if (smProductCustomize->EnablePickAndPlace1Module)
		{
			smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module)
		{
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopPickAndPlaceZMotorHomeDone;
		break;

	case nCase.IsStopPickAndPlaceZMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true
				&& smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true
				&& smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Z Axis Stop Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartPickAndPlaceZMotorHome;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{

			m_cLogger->WriteLog("Homeseq: Pick And Place Z Axis Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(11010);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Pickup Head Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(12010);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Pickup Head Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopPickAndPlaceZMotorHome;
		}
		break;

	case nCase.IsPickAndPlaceAndHomePosition:
		
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount < 100)
		{
			break;
		}
		m_cProductMotorControl->THKReadEncoderValue(0, 0, &lngPnP1ZAxisPos);
		m_cProductMotorControl->THKReadEncoderValue(1, 0, &lngPnP2ZAxisPos);
		if ((lngPnP1ZAxisPos > (5)
			|| lngPnP1ZAxisPos < (-5)
			|| lngPnP2ZAxisPos > (5)
			|| lngPnP2ZAxisPos < (-5)))
		{
			m_cProductShareVariables->SetAlarm(10024);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.StartPickAndPlaceZMotorHome;
			m_cLogger->WriteLog("Homeseq: Pick And Place Pickup Head Z Axis Motor Not At Up Position.\n");
			break;
		}
		else
		{
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.StartTableTrayZPickAndPlaceZAndAllVisionMotorHome;
			m_cLogger->WriteLog("Homeseq: Pick And Place Pickup Head Z Axis Motor At Up Position.\n");
		}
		break;


	case nCase.StartTableTrayZPickAndPlaceZAndAllVisionMotorHome:
		if (smProductCustomize->EnableInputTrayTableZAxisMotor)
		{
			smProductEvent->InputTrayTableZAxisMotorHomeDone.Set = false;
			smProductEvent->StartInputTrayTableZAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableZAxisMotor)
		{
			smProductEvent->OutputTrayTableZAxisMotorHomeDone.Set = false;
			smProductEvent->StartOutputTrayTableZAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnableInputVisionMotor)
		{
			smProductEvent->InputVisionModuleMotorHomeDone.Set = false;
			smProductEvent->StartInputVisionModuleMotorHome.Set = true;
		}
		if (smProductCustomize->EnableS2VisionMotor)
		{
			smProductEvent->S2VisionModuleMotorHomeDone.Set = false;
			smProductEvent->StartS2VisionModuleMotorHome.Set = true;
		}
		if (smProductCustomize->EnableS1VisionMotor)
		{
			smProductEvent->S1VisionModuleMotorHomeDone.Set = false;
			smProductEvent->StartS1VisionModuleMotorHome.Set = true;
		}
		if (smProductCustomize->EnableS3VisionMotor)
		{
			smProductEvent->S3VisionModuleMotorHomeDone.Set = false;
			smProductEvent->StartS3VisionModuleMotorHome.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsTableTrayZPickAndPlaceZAndAllVisionMotorHomeDone;
		break;

	case nCase.IsTableTrayZPickAndPlaceZAndAllVisionMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableS2VisionMotor == false || (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableS1VisionMotor == false || (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableS3VisionMotor == false || (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorHomeDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: All Table,Vision and Pick And Place Z Axis Home Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//sequenceNo++;
			//sequenceNo++;
			//sequenceNo++;
			sequenceNo = nCase.StartPickAndPlaceYMotorHome;
		}
		else if (IsReadyToMove() == false)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableInputVisionMotor)
			{
				smProductEvent->InputVisionModuleMotorStopDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorStop.Set = true;
			}
			if (smProductCustomize->EnableS2VisionMotor)
			{
				smProductEvent->S2VisionModuleMotorStopDone.Set = false;
				smProductEvent->StartS2VisionModuleMotorStop.Set = true;
			}
			if (smProductCustomize->EnableS1VisionMotor)
			{
				smProductEvent->S1VisionModuleMotorStopDone.Set = false;
				smProductEvent->StartS1VisionModuleMotorStop.Set = true;
			}
			if (smProductCustomize->EnableS3VisionMotor)
			{
				smProductEvent->S3VisionModuleMotorStopDone.Set = false;
				smProductEvent->StartS3VisionModuleMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopTableTrayZPickAndPlaceZAndAllVisionMotorHomeDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: All Table,Vision and Pick And Place Z Axis Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(43004);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(47004);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(48004);
				m_cLogger->WriteLog("Homeseq: Input Vision Module Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(49004);
				m_cLogger->WriteLog("Homeseq: S2 Vision Module Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(50004);
				m_cLogger->WriteLog("Homeseq: S1 Vision Module Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(54004);
				m_cLogger->WriteLog("Homeseq: S3 Vision Module Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopTableTrayZPickAndPlaceZAndAllVisionMotorHome;
		}
		break;

	case nCase.StartStopTableTrayZPickAndPlaceZAndAllVisionMotorHome:
		if (smProductCustomize->EnableInputTrayTableZAxisMotor)
		{
			smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
			smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableZAxisMotor)
		{
			smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
			smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableInputVisionMotor)
		{
			smProductEvent->InputVisionModuleMotorStopDone.Set = false;
			smProductEvent->StartInputVisionModuleMotorStop.Set = true;
		}
		if (smProductCustomize->EnableS2VisionMotor)
		{
			smProductEvent->S2VisionModuleMotorStopDone.Set = false;
			smProductEvent->StartS2VisionModuleMotorStop.Set = true;
		}
		if (smProductCustomize->EnableS1VisionMotor)
		{
			smProductEvent->S1VisionModuleMotorStopDone.Set = false;
			smProductEvent->StartS1VisionModuleMotorStop.Set = true;
		}
		if (smProductCustomize->EnableS3VisionMotor)
		{
			smProductEvent->S3VisionModuleMotorStopDone.Set = false;
			smProductEvent->StartS3VisionModuleMotorStop.Set = true;
		}
		
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopTableTrayZPickAndPlaceZAndAllVisionMotorHomeDone;
		break;

	case nCase.IsStopTableTrayZPickAndPlaceZAndAllVisionMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == true))
			&& (smProductCustomize->EnableS2VisionMotor == false || (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorStopDone.Set == true))
			&& (smProductCustomize->EnableS1VisionMotor == false || (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorStopDone.Set == true))
			&& (smProductCustomize->EnableS3VisionMotor == false || (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: All Table,Vision and Pick And Place Z Axis Stop Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartTableTrayZPickAndPlaceZAndAllVisionMotorHome;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{

			m_cLogger->WriteLog("Homeseq: All Table,Vision and Pick And Place Z Axis Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(43008);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(47008);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(48008);
				m_cLogger->WriteLog("Homeseq: Input Vision Module Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(49008);
				m_cLogger->WriteLog("Homeseq: S2 Vision Module Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(50008);
				m_cLogger->WriteLog("Homeseq: S1 Vision Module Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(54008);
				m_cLogger->WriteLog("Homeseq: S3 Vision Module Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopTableTrayZPickAndPlaceZAndAllVisionMotorHome;
		}
		break;
	case nCase.StartPickAndPlaceYMotorHome:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor)
		{
			smProductEvent->PickAndPlace1YAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
		{
			smProductEvent->PickAndPlace2YAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorHome.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsPickAndPlaceYMotorHomeDone;
		break;

	case nCase.IsPickAndPlaceYMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorHomeDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Y Axis home Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo = nCase.StartAllMotorSettingUp;
		}
		else if (IsReadyToMove() == false)
		{
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor)
			{
				smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("InputTableseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopPickAndPlaceYMotorHomeDone;
			break;
		}

		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{

			m_cLogger->WriteLog("Homeseq: Pick And Place Y Axis home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40004);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44004);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopPickAndPlaceYMotorHome;

		}
		break;
	case nCase.StartStopPickAndPlaceYMotorHome:
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor)
		{
			smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
		{
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopPickAndPlaceYMotorHomeDone;
		break;
	case nCase.IsStopPickAndPlaceYMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Y Axis stop Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

			sequenceNo = nCase.StartPickAndPlaceYMotorHome;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Pick And Place Y Axis stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40008);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44008);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartStopPickAndPlaceYMotorHome;
		}
	case nCase.StartAllMotorSettingUp:
		if (smProductCustomize->EnableInputTrayTableXAxisMotor)
		{
			smProductEvent->InputTrayTableXAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartInputTrayTableXAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableYAxisMotor)
		{
			smProductEvent->InputTrayTableYAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartInputTrayTableYAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableZAxisMotor)
		{
			smProductEvent->InputTrayTableZAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartInputTrayTableZAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableXAxisMotor)
		{
			smProductEvent->OutputTrayTableXAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartOutputTrayTableXAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableYAxisMotor)
		{
			smProductEvent->OutputTrayTableYAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartOutputTrayTableYAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableZAxisMotor)
		{
			smProductEvent->OutputTrayTableZAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartOutputTrayTableZAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableInputVisionMotor)
		{
			smProductEvent->InputVisionModuleMotorSettingUpDone.Set = false;
			smProductEvent->StartInputVisionModuleMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableS2VisionMotor)
		{
			smProductEvent->S2VisionModuleMotorSettingUpDone.Set = false;
			smProductEvent->StartS2VisionModuleMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableS1VisionMotor)
		{
			smProductEvent->S1VisionModuleMotorSettingUpDone.Set = false;
			smProductEvent->StartS1VisionModuleMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnableS3VisionMotor)
		{
			smProductEvent->S3VisionModuleMotorSettingUpDone.Set = false;
			smProductEvent->StartS3VisionModuleMotorSettingUp.Set = true;
		}

		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			smProductEvent->PickAndPlace1XAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace1XAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
		{
			smProductEvent->PickAndPlace1YAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace1YAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1Module == true)
		{
			smProductEvent->PickAndPlace1ZAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorSettingUp.Set = true;

			smProductEvent->PickAndPlace1ThetaAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace1ThetaAxisMotorSettingUp.Set = true;
		}

		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			smProductEvent->PickAndPlace2XAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
		{
			smProductEvent->PickAndPlace2YAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorSettingUp.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module == true)
		{
			smProductEvent->PickAndPlace2ZAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorSettingUp.Set = true;

			smProductEvent->PickAndPlace2ThetaAxisMotorSettingUpDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorSettingUp.Set = true;

		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsAllMotorSettingUpDone;
		break;
	case nCase.IsAllMotorSettingUpDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableS2VisionMotor == false || (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableS1VisionMotor == false || (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnableS3VisionMotor == false || (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ZAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ThetaAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ZAxisMotorSettingUpDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ThetaAxisMotorSettingUpDone.Set == true))
			)
		{
			m_cLogger->WriteLog("All Motor Setting up Done %ums\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartTableTrayXYPickAndPlaceXTMotorHome;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Table X, Y, Z Axis, Output Table X, Y, Z Axis,  all Vision, PnP setting up timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(41009);
				m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(42009);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(43009);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(45009);
				m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(46009);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(47009);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(48009);
				m_cLogger->WriteLog("Homeseq: Input Vision Module Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS2VisionMotor == true && smProductEvent->S2VisionModuleMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(49009);
				m_cLogger->WriteLog("Homeseq: S2 Vision Module Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS1VisionMotor == true && smProductEvent->S1VisionModuleMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(50009);
				m_cLogger->WriteLog("Homeseq: S1 Vision Module Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableS3VisionMotor == true && smProductEvent->S3VisionModuleMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(54009);
				m_cLogger->WriteLog("Homeseq: S3 Vision Module Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}

			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(55009);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(40009);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				if (smProductEvent->PickAndPlace1ZAxisMotorSettingUpDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11011);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Z Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductEvent->PickAndPlace1ThetaAxisMotorSettingUpDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11021);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Theta Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}

			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(56009);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorSettingUpDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(44009);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2Module == true)
			{
				if (smProductEvent->PickAndPlace2ZAxisMotorSettingUpDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12011);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Z Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductEvent->PickAndPlace2ThetaAxisMotorSettingUpDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12021);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Theta Axis Motor setting timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartAllMotorSettingUp;
		}
		break;
	case nCase.StartTableTrayXYPickAndPlaceXTMotorHome:
		if (smProductCustomize->EnableInputTrayTableXAxisMotor)
		{
			smProductEvent->InputTrayTableXAxisMotorHomeDone.Set = false;
			smProductEvent->StartInputTrayTableXAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableYAxisMotor)
		{
			smProductEvent->InputTrayTableYAxisMotorHomeDone.Set = false;
			smProductEvent->StartInputTrayTableYAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableXAxisMotor)
		{
			smProductEvent->OutputTrayTableXAxisMotorHomeDone.Set = false;
			smProductEvent->StartOutputTrayTableXAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableYAxisMotor)
		{
			smProductEvent->OutputTrayTableYAxisMotorHomeDone.Set = false;
			smProductEvent->StartOutputTrayTableYAxisMotorHome.Set = true;
		}

		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			smProductEvent->PickAndPlace1XAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace1XAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1Module)
		{
			smProductEvent->PickAndPlace1ThetaAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace1ThetaAxisMotorHome.Set = true;
		}

		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			smProductEvent->PickAndPlace2XAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorHome.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module)
		{
			smProductEvent->PickAndPlace2ThetaAxisMotorHomeDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorHome.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsTableTrayXYPickAndPlaceXTMotorHomeDone;
		break;

	case nCase.IsTableTrayXYPickAndPlaceXTMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ThetaAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorHomeDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ThetaAxisMotorHomeDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Table X, Y Axis, Output Table X, Y Axis, Pick And Place X, Theta Axis Home done Done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			smProductProduction->PickAndPlace1CurrentStation = PickAndPlaceCurrentStation.HomeStation;
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.HomeStation;
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			sequenceNo = nCase.CheckTrayPresentOnTrayTable;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnableInputTrayTableXAxisMotor)
			{
				smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor)
			{
				smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor)
			{
				smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor)
			{
				smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
			{
				smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace1Module)
			{
				smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1ThetaAxisMotorStop.Set = true;
			}

			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
			{
				smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module)
			{
				smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopTableTrayXYPickAndPlaceXTMotorHomeDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Table X, Y Axis, Output Table X, Y Axis, Pick And Place X, Theta Axis Home timeout timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(41004);
				m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(42004);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(45004);
				m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(46004);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(55004);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				if (smProductEvent->PickAndPlace1ThetaAxisMotorHomeDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11022);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Theta Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorHomeDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(56004);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2Module)
			{
				if (smProductEvent->PickAndPlace2ThetaAxisMotorHomeDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12022);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Theta Axis Motor Home timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopTableTrayXYPickAndPlaceXTMotorHome;
		}
		break;

	case nCase.StartStopTableTrayXYPickAndPlaceXTMotorHome:
		if (smProductCustomize->EnableInputTrayTableXAxisMotor)
		{
			smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
			smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableYAxisMotor)
		{
			smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
			smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableXAxisMotor)
		{
			smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
			smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableYAxisMotor)
		{
			smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
			smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
		}

		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
		{
			smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1Module)
		{

			smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1ThetaAxisMotorStop.Set = true;

		}

		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
		{
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module)
		{
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopTableTrayXYPickAndPlaceXTMotorHomeDone;
		break;

	case nCase.IsStopTableTrayXYPickAndPlaceXTMotorHomeDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Table X, Y Axis, Output Table X, Y Axis, Pick And Place X, Theta Axis stop done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartTableTrayXYPickAndPlaceXTMotorHome;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{

			m_cLogger->WriteLog("Homeseq: Input Table X, Y Axis, Output Table X, Y Axis, Pick And Place X, Theta Axis stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(41008);
				m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(42008);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(45008);
				m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(46008);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}

			if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(55008);
				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true)
			{
				if (smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11023);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Theta Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}

			if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(56008);
				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace2Module)
			{
				if (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12023);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Theta Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopTableTrayXYPickAndPlaceXTMotorHome;
		}
		break;

	case nCase.CheckTrayPresentOnTrayTable:
		if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true)
		{
			smProductProduction->InputTrayUnloadingNo = 1;
			sequenceNo = nCase.StartMoveTrayTableXYToUnload;
		}
		if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true)
		{
			smProductProduction->OutputTrayUnloadingNo = 1;
			sequenceNo = nCase.StartMoveTrayTableXYToUnload;
			break;
		}
		//if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && TemporaryDisable == false)
		//{
		//	smProductProduction->OutputTrayUnloadingNo = 1;
		//	sequenceNo = nCase.StartMoveTrayTableXYToUnload;
		//	break;
		//}
		if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false
			&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false
			&&(TemporaryDisable==true || 
			(TemporaryDisable == false
			/*	&& m_cProductIOControl->IsRejectTrayPresentSensorOn() == false*/
				)))
		{
			sequenceNo = nCase.StartMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp;
			break;
		}


	case nCase.StartMoveTrayTableXYToUnload:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorMoveUnload.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorMoveUnload.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMoveUnload.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMoveUnload.Set = true;
			}
		}
		sequenceNo = nCase.IsMoveTrayTableXYToUnloadDone;
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		break;

	case nCase.IsMoveTrayTableXYToUnloadDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == true))))
			)
		{
			RtSleep(500);
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table move Unload done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToSingulation;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
				{
					smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
				{
					smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
				{
					smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
				{
					smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
				}
			}

			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.IsStopMoveTrayTableXYToUnloadDone;
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table move Unload Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor move Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor move Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor move Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor move Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableXYToUnload;
		}
		break;

	case nCase.StartStopMoveTrayTableXYToUnload:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopMoveTrayTableXYToUnloadDone;
		break;

	case nCase.IsStopMoveTrayTableXYToUnloadDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table stop done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableXYToUnload;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Stop Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableXYToUnload;
		}
		break;
	case nCase.StartMoveTrayTableZToSingulation:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsMoveTrayTableZToSingulationDone;
		break;
	case nCase.IsMoveTrayTableZToSingulationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input And/Or Output Tray Table Move to Singulation done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartUnlockUnloadingStackerCylinder;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopMoveTrayTableZToSingulationDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Move to singulation Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Move To Singulation timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Move To Singulation timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToSingulation;
		}
		break;

	case nCase.StartStopMoveTrayTableZToSingulation:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopMoveTrayTableZToSingulationDone;
		break;

	case nCase.IsStopMoveTrayTableZToSingulationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table stop done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToSingulation;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Stop Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToSingulation;
		}
		break;
	case nCase.StartUnlockUnloadingStackerCylinder:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			m_cProductIOControl->SetInputUnloadingStackerUnlockCylinderOn(true);
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			m_cProductIOControl->SetOutputUnloadingStackerUnlockCylinderOn(true);
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsUnlockUnloadingStackerCylinderDone;
		break;
	case nCase.IsUnlockUnloadingStackerCylinderDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1 && m_cProductIOControl->IsInputUnloadingStackerUnlockSensorOn() == true))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1 && m_cProductIOControl->IsOutputUnloadingStackerUnlockSensorOn() == true))
			)
		{
			m_cLogger->WriteLog("Input and output unloading stacker cylinder unlock done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToUnloading;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Input and Output Unloading Stacker Cylinder unlock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1 && m_cProductIOControl->IsInputUnloadingStackerUnlockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5417);
				m_cLogger->WriteLog("Input Unloading Stacker Cylinder unlock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1 && m_cProductIOControl->IsOutputUnloadingStackerUnlockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5418);
				m_cLogger->WriteLog("Output Unloading Stacker Cylinder unlock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartUnlockUnloadingStackerCylinder;
		}
		break;
	case nCase.StartMoveTrayTableZToUnloading:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveUnload.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveUnload.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsMoveTrayTableZToUnloadingDone;
		break;
	case nCase.IsMoveTrayTableZToUnloadingDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input And/Or Output Tray Table Z Axis Move to Unloading position done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartLockUnloadingStackerCylinder;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopMoveTrayTableZToUnloadingDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Z Axis Move to unloading position Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Move To unloading timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Move To unloading timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToUnloading;
		}
		break;

	case nCase.StartStopMoveTrayTableZToUnloading:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopMoveTrayTableZToUnloadingDone;
		break;

	case nCase.IsStopMoveTrayTableZToUnloadingDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table stop done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToUnloading;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Stop Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToUnloading;
		}
		break;
	case nCase.StartLockUnloadingStackerCylinder:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			m_cProductIOControl->SetInputUnloadingStackerUnlockCylinderOn(false);
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			m_cProductIOControl->SetOutputUnloadingStackerUnlockCylinderOn(false);
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsLockUnloadingStackerCylinderDone;
		break;
	case nCase.IsLockUnloadingStackerCylinderDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1 && m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == true))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1 && m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == true))
			)
		{
			m_cLogger->WriteLog("Input and output unloading stacker cylinder lock done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//sequenceNo = nCase.StartMoveTrayTableZToDownPosition;
			sequenceNo = nCase.StartSetInputTableVacuumOffDuringUnloading;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Input and Output Unloading Stacker Cylinder lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1 && m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5407);
				m_cLogger->WriteLog("Input Unloading Stacker Cylinder lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1 && m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == false)
			{
				m_cProductShareVariables->SetAlarm(5415);
				m_cLogger->WriteLog("Output Unloading Stacker Cylinder lock timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			sequenceNo = nCase.StartLockUnloadingStackerCylinder;
		}
		break;
	case nCase.StartSetInputTableVacuumOffDuringUnloading:
		m_cProductIOControl->SetInputTrayTableVacuumOn(false);
		m_cProductIOControl->SetOutputTrayTableVacuumOn(false);
		m_cProductIOControl->SetInputTrayTableVacuumOn(false);
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsSetInputTableVacuumOffDuringUnloadingDone;
		break;
	case nCase.IsSetInputTableVacuumOffDuringUnloadingDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms)
		{
			m_cLogger->WriteLog("HomeSeq: Input Table Vacuum Off Done during unloading %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToDownPosition;
		}
		break;
	case nCase.StartMoveTrayTableZToDownPosition:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveDown.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveDown.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsMoveTrayTableZToDownPositionDone;
		break;
	case nCase.IsMoveTrayTableZToDownPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input And/Or Output Tray Table Move to Down position done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			smProductProduction->InputTrayUnloadingNo = 0;
			smProductProduction->OutputTrayUnloadingNo = 0;
			sequenceNo = nCase.StartMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopMoveTrayTableZToDownPositionDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Move to down position Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Move To down position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Move To Down position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToDownPosition;
		}
		break;

	case nCase.StartStopMoveTrayTableZToDownPosition:
		if (smProductProduction->InputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
		}
		if (smProductProduction->OutputTrayUnloadingNo == 1)
		{
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopMoveTrayTableZToDownPositionDone;
		break;

	case nCase.IsStopMoveTrayTableZToDownPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductProduction->InputTrayUnloadingNo == 0 || (smProductProduction->InputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))))
			&& (smProductProduction->OutputTrayUnloadingNo == 0 || (smProductProduction->OutputTrayUnloadingNo == 1
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table stop done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveTrayTableZToDownPosition;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input Tray Table and Output Tray Table Stop Timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductProduction->InputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("Homeseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductProduction->OutputTrayUnloadingNo == 1)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Homeseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveTrayTableZToDownPosition;
		}
		break;


	case nCase.StartMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp:
		if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
		{
			smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set = false;
			smProductEvent->StartInputTrayTableXAxisMotorMoveLoad.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
		{
			smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set = false;
			smProductEvent->StartInputTrayTableYAxisMotorMoveLoad.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
		{
			smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set = false;
			smProductEvent->StartOutputTrayTableXAxisMotorMoveManualLoadUnload.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
		{
			smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set = false;
			smProductEvent->StartOutputTrayTableYAxisMotorMoveManualLoadUnload.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1Module && smProductSetting->EnablePH[0] == true)
		{
			smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorMoveUpPosition.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] == true)
		{
			smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPosition.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUpDone;
		break;

	case nCase.IsMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUpDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && (smProductSetting->EnablePH[0] == false||(smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && (smProductSetting->EnablePH[1] == false||(smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input and Output Table X Y Axis move load Position and Pick And Place 1 , 2 move up position done %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = 999;
		}
		else if (IsReadyToMoveProduction() == false)
		{
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace1Module && smProductSetting->EnablePH[0] == true)
			{
				smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			m_cProductShareVariables->SetAlarm(60501);
			m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
			sequenceNo = nCase.IsStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUpDone;
			break;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input and Output Table X Y Axis move load and Pick And Place 1 , 2 move up position Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(41002);
				m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor Move Load timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(42002);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor Move Load timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}

			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(45002);
				m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor Move Manual Load Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(46002);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor Move Manual Load Unload timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true)
			{
				if (smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11002);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Z Axis Motor move up timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2Module&&smProductSetting->EnablePH[1] == true)
			{
				if (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12002);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Z Axis Motor move up timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp;
		}
		break;
	case nCase.StartStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp:
		if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
		{
			smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;
			smProductEvent->StartInputTrayTableXAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
		{
			smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;
			smProductEvent->StartInputTrayTableYAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
		{
			smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;
			smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
		{
			smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;
			smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace1Module && smProductSetting->EnablePH[0] == true)
		{
			smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = true;
		}
		if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] == true)
		{
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
		sequenceNo = nCase.IsStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUpDone;
		break;

	case nCase.IsStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUpDone:
		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
		lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
		if (true
			&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
			&& (smProductCustomize->EnablePickAndPlace1Module == false || (smProductCustomize->EnablePickAndPlace1Module == true && (smProductSetting->EnablePH[0] == false || (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == true))))
			&& (smProductCustomize->EnablePickAndPlace2Module == false || (smProductCustomize->EnablePickAndPlace2Module == true && (smProductSetting->EnablePH[1] == false || (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true))))
			)
		{
			m_cLogger->WriteLog("Homeseq: Input and Output Table X Y Axis and Pick And Place 1,2 stop done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			sequenceNo = nCase.StartMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp;
		}
		else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
		{
			m_cLogger->WriteLog("Homeseq: Input and Output Table X Y Axis and Pick And Place 1,2 stop Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(41008);
				m_cLogger->WriteLog("Homeseq: Input Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(42008);
				m_cLogger->WriteLog("Homeseq: Input Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}

			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(45008);
				m_cLogger->WriteLog("Homeseq: Output Tray Table X Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
			{
				m_cProductShareVariables->SetAlarm(46008);
				m_cLogger->WriteLog("Homeseq: Output Tray Table Y Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			}
			if (smProductCustomize->EnablePickAndPlace1Module == true&& smProductSetting->EnablePH[0] == true)
			{
				if (smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11010);
					m_cLogger->WriteLog("Homeseq: Pick And Place 1 Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			if (smProductCustomize->EnablePickAndPlace2Module==true&&smProductSetting->EnablePH[1] == true)
			{
				if (smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(12010);
					m_cLogger->WriteLog("Homeseq: Pick And Place 2 Z Axis Motor Stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
			}
			sequenceNo = nCase.StartStopMoveInputTrayTableXYToLoadAndOutputTrayTableXYToManualLoadUnloadAndMovePickZUp;
		}
		break;
	//case nCase.StartMovePNP12YAxisToStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set = false;
	//		smProductEvent->StartPickAndPlace1YAxisMotorMoveToStandbyPosition.Set = true;
	//	}
	//	if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
	//		smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsMovePNP12YAxisToStandbyPositionDone;
	//	break;
	//case nCase.IsMovePNP12YAxisToStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == true))
	//		&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis move to standby position done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		sequenceNo = nCase.StartMovePNP12XAxisToStandbyPosition;
	//	}
	//	else if (IsReadyToMove() == false)
	//	{
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//		{
	//			smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//		{
	//			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
	//		}
	//		m_cProductShareVariables->SetAlarm(60501);
	//		m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//		sequenceNo = nCase.IsStopMovePNP12YAxisToStandbyPositionDone;
	//		break;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis move to standby position Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(40002);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor move to standby position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);	
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(44002);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor move to standby position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		sequenceNo = nCase.StopMovePNP12YAxisToStandbyPosition;
	//	}
	//	break;
	//case nCase.StopMovePNP12YAxisToStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
	//		smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
	//	}
	//	if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
	//		smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsStopMovePNP12YAxisToStandbyPositionDone;
	//	break;

	//case nCase.IsStopMovePNP12YAxisToStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true))
	//		&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis stop done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		sequenceNo = nCase.StartMovePNP12YAxisToStandbyPosition;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis stop Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(40008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(44008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		sequenceNo = nCase.StopMovePNP12YAxisToStandbyPosition;
	//	}
	//	break;
	//case nCase.StartMovePNP12XAxisToStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[0] == true)
	//		{
	//			smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace1XAxisMotorMoveToS1Position.Set = true;
	//		}
	//		else
	//		{
	//			smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace1XAxisMotorMoveToParkingPosition.Set = true;
	//		}

	//	}
	//	if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[1] == true)
	//		{
	//			smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace2XAxisMotorMoveToS3Position.Set = true;
	//		}
	//		else
	//		{
	//			smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
	//		}

	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsMovePNP12XAxisToStandbyPositionDone;
	//	break;
	//case nCase.IsMovePNP12XAxisToStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true 
	//			&&((smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == true)|| (smProductSetting->EnablePH[0] == false && smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set == true))))
	//		&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true
	//			&& ((smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set == true) || (smProductSetting->EnablePH[1] == false && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true))))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 X Axis move to standby position done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		sequenceNo = nCase.StartMovePNP12YAxisToPreProductionStandbyPosition;
	//	}
	//	else if (IsReadyToMove() == false)
	//	{
	//		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
	//		{
	//			smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
	//		{
	//			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
	//		}
	//		m_cProductShareVariables->SetAlarm(60501);
	//		m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//		sequenceNo = nCase.IsStopMovePNP12XAxisToStandbyPositionDone;
	//		break;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 X Axis move to standby position Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
	//		{
	//			if(smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(55002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor move to S1 position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//			else if(smProductSetting->EnablePH[0] == false && smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(55002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor move to Parking position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
	//		{
	//			if (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(56002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor move to S3 position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//			else if (smProductSetting->EnablePH[1] == false && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(56002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor move to Parking position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//		}
	//		sequenceNo = nCase.StopMovePNP12XAxisToStandbyPosition;
	//	}
	//	break;
	//case nCase.StopMovePNP12XAxisToStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
	//		smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = true;
	//	}
	//	if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
	//	{
	//		smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
	//		smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsStopMovePNP12XAxisToStandbyPositionDone;
	//	break;

	//case nCase.IsStopMovePNP12XAxisToStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == true))
	//		&& (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 X Axis stop done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		sequenceNo = nCase.StartMovePNP12YAxisToStandbyPosition;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 X Axis stop Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true && smProductEvent->PickAndPlace1XAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(55008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 1 X Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(56008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 2 X Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		sequenceNo = nCase.StopMovePNP12XAxisToStandbyPosition;
	//	}
	//	break;
	//case nCase.StartMovePNP12YAxisToPreProductionStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[0] == true)
	//		{
	//			smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace1YAxisMotorMoveToS1Position.Set = true;
	//		}
	//	}
	//	if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[1] == true)
	//		{
	//			smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set = false;
	//			smProductEvent->StartPickAndPlace2YAxisMotorMoveToS3Position.Set = true;
	//		}

	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsMovePNP12YAxisToPreProductionStandbyPositionDone;
	//	break;
	//case nCase.IsMovePNP12YAxisToPreProductionStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true
	//			&& (smProductSetting->EnablePH[0] == false || (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set == true))))
	//		&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true
	//			&& (smProductSetting->EnablePH[1] == false ||(smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set == true))))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis move to pre production standby position done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductSetting->EnablePH[0]==true)
	//		{
	//			smProductProduction->PickAndPlace1CurrentStation = PickAndPlaceCurrentStation.BottomStation;
	//		}
	//		else
	//		{
	//			smProductProduction->PickAndPlace1CurrentStation = PickAndPlaceCurrentStation.DisableStation;
	//		}
	//		if (smProductSetting->EnablePH[1] == true)
	//		{
	//			smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.S3Station;
	//		}
	//		else
	//		{
	//			smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
	//		}
	//		smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.UnknownStation;
	//		smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
	//		sequenceNo = 999;
	//	}
	//	else if (IsReadyToMove() == false)
	//	{
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//		{
	//			if (smProductSetting->EnablePH[0] == true)
	//			{
	//				smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
	//				smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
	//			}
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//		{
	//			if (smProductSetting->EnablePH[1] == true)
	//			{
	//				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
	//				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
	//			}
	//		}
	//		m_cProductShareVariables->SetAlarm(60501);
	//		m_cLogger->WriteLog("Homeseq: Door Get Trigger %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//		sequenceNo = nCase.IsStopMovePNP12YAxisToPreProductionStandbyPositionDone;
	//		break;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis move to pre production standby position Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//		{
	//			if (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(40002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor move to S1 position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//		{
	//			if (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set == false)
	//			{
	//				m_cProductShareVariables->SetAlarm(44002);
	//				m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor move to S3 position timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//			}
	//		}
	//		sequenceNo = nCase.StopMovePNP12YAxisToPreProductionStandbyPosition;
	//	}
	//	break;
	//case nCase.StopMovePNP12YAxisToPreProductionStandbyPosition:
	//	if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[0] == true)
	//		{
	//			smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = true;
	//		}
	//	}
	//	if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	//	{
	//		if (smProductSetting->EnablePH[1] == true)
	//		{
	//			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
	//			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
	//		}
	//	}
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockStart2);
	//	sequenceNo = nCase.IsStopMovePNP12YAxisToPreProductionStandbyPositionDone;
	//	break;

	//case nCase.IsStopMovePNP12YAxisToPreProductionStandbyPositionDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnHomeSequenceClockEnd);
	//	lnHomeSequenceClockSpan.QuadPart = lnHomeSequenceClockEnd.QuadPart - lnHomeSequenceClockStart2.QuadPart;
	//	if (true
	//		&& (smProductCustomize->EnablePickAndPlace1YAxisMotor == false || (smProductCustomize->EnablePickAndPlace1YAxisMotor == true
	//			&& (smProductSetting->EnablePH[0] == false || (smProductSetting->EnablePH[0] == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true))))
	//		&& (smProductCustomize->EnablePickAndPlace2YAxisMotor == false || (smProductCustomize->EnablePickAndPlace2YAxisMotor == true
	//			&& (smProductSetting->EnablePH[1] == false || (smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true))))
	//		)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis stop done %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		sequenceNo = nCase.StartMovePNP12YAxisToPreProductionStandbyPosition;
	//	}
	//	else if (lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->HOMING_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Homeseq: Pick And Place 1, 2 Y Axis stop Timeout %ums.", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(40008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 1 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == false)
	//		{
	//			m_cProductShareVariables->SetAlarm(44008);
	//			m_cLogger->WriteLog("Homeseq: Pick And Place 2 Y Axis Motor stop timeout %ums.\n", lnHomeSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	//		}
	//		sequenceNo = nCase.StopMovePNP12YAxisToPreProductionStandbyPosition;
	//	}
	//	break;
	default:
		return -1;
		break;
	}
	return sequenceNo;
}

int CProductSequence::SwitchJobSequence(int sequenceNo)
{
	JobSequenceNo nJobSequenceNo;
	switch (sequenceNo)
	{
	case nJobSequenceNo.WaitingToReceiveEventStartJobSequence:
	{
		m_cLogger->WriteLog("Jobseq: Start WaitingToReceiveEventStartJobSequence.\n");
		
		smProductProduction->InputTableResult[0] = sClearResult;
		smProductProduction->OutputTableResult[0] = sClearResult;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0] = sClearResult;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1] = sClearResult;

		
		smProductProduction->InputTableResult[0].UnitPresent = 0;
		smProductProduction->InputTableResult[0].PlacementResult = 0;
		smProductProduction->InputTableResult[0].InputResult = 0;
		smProductProduction->InputTableResult[0].S2Result = 0;
		smProductProduction->InputTableResult[0].S2PartingResult = 0;
		smProductProduction->InputTableResult[0].S1Result = 0;
		smProductProduction->InputTableResult[0].SetupResult = 0;
		smProductProduction->InputTableResult[0].SetupThicknessResult = 0;
		smProductProduction->InputTableResult[0].BottomResult = 0;
		smProductProduction->InputTableResult[0].S3Result = 0;
		smProductProduction->InputTableResult[0].S3PartingResult = 0;
		smProductProduction->InputTableResult[0].OutputResult = 0;
		smProductProduction->InputTableResult[0].OutputResult_Post = 0;
		smProductProduction->InputTableResult[0].RejectResult = 0;
		smProductProduction->InputTableResult[0].RejectResult_Post = 0;

		smProductProduction->OutputTableResult[0].UnitPresent = 0;
		smProductProduction->OutputTableResult[0].PlacementResult = 0;
		smProductProduction->OutputTableResult[0].InputResult = 0;
		smProductProduction->OutputTableResult[0].S2Result = 0;
		smProductProduction->OutputTableResult[0].S2PartingResult = 0;
		smProductProduction->OutputTableResult[0].S1Result = 0;
		smProductProduction->OutputTableResult[0].SetupResult = 0;
		smProductProduction->OutputTableResult[0].SetupThicknessResult = 0;
		smProductProduction->OutputTableResult[0].BottomResult = 0;
		smProductProduction->OutputTableResult[0].S3Result = 0;
		smProductProduction->OutputTableResult[0].S3PartingResult = 0;
		smProductProduction->OutputTableResult[0].OutputResult = 0;
		smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
		smProductProduction->OutputTableResult[0].RejectResult = 0;
		smProductProduction->OutputTableResult[0].RejectResult_Post = 0;

		smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].PlacementResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].S2Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].S2PartingResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].S1Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].SetupResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].SetupThicknessResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].BottomResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].S3Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].S3PartingResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].OutputResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].OutputResult_Post = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].RejectResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[0].RejectResult_Post = 0;

		smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].PlacementResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].S2Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].S2PartingResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].S1Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThicknessResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].S3Result = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].S3PartingResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].OutputResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].OutputResult_Post = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].RejectResult = 0;
		smProductProduction->PickAndPlacePickUpHeadStationResult[1].RejectResult_Post = 0;

		//smProductEvent->RINT_RSEQ_INPUT_VISION_DONE.Set = true;
		//smProductEvent->ROUT_RSEQ_OUTPUT_VISION_DONE.Set = true;
		smProductEvent->RINT_RSEQ_INPUT_VISION_DONE.Set = false;
		smProductEvent->ROUT_RSEQ_OUTPUT_VISION_DONE.Set = false;
		smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = false;
		smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
		smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PICKED.Set = false;
		smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set = false;
		smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set = false;
		smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PICKED_DONE.Set = false;

		smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = false;

		smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
		smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = false;
		smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
		smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = false;

		smProductEvent->RSEQ_RPNP1_MOVE_TO_INPUT_STATION_START.Set = false;
		smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
		smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = false;
		smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;

		smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = false;

		smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set = false;

		smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = true;
		smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = true;

		smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = true;
		smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = true;

		smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = false;
		smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set = false;
		smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
		smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set = false;
		smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set = false;


		smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
		smProductEvent->RINT_RSEQ_INPUT_UNIT_PICK_START.Set = false;
		smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set = false;
		smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = false;

		smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_START.Set = false;
		smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_SKIP.Set = false;
		smProductEvent->RPNP1_RSEQ_REMOVE_UNIT_DONE.Set = true;
		smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_START.Set = false;
		smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_SKIP.Set = false;
		smProductEvent->RPNP2_RSEQ_REMOVE_UNIT_DONE.Set = true;

		smProductEvent->RPNP1_RSEQ_MOVE_STANDBY_DONE.Set = false;
		smProductEvent->RPNP2_RSEQ_MOVE_STANDBY_DONE.Set = false;
		smProductEvent->RSEQ_RPNP1_MOVE_STANDBY_START.Set = false;
		smProductEvent->RSEQ_RPNP2_MOVE_STANDBY_START.Set = false;

		smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set = false;
		if (smProductEvent->RMAIN_RTHD_CONTINUE_LOT.Set == false)
		{	
			smProductProduction->nCurrentTotalUnitDone = 0;
			smProductProduction->nCurrrentTotalUnitDoneByLot = 0;

			smProductProduction->nCurrentBottomStationTrayNo = 0;
			smProductProduction->nCurrentS3StationTrayNo = 0;
			smProductProduction->nCurrentTotalInputUnitDone = 0;
			smProductProduction->nCurrentLotGoodQuantity = 0;
			smProductProduction->nCurrentLotNotGoodQuantity = 0;
		}

		smProductProduction->OutputQuantity = 0;
		smProductEvent->ROUT_RSEQ_REJECT1_RESET.Set = false;
		smProductEvent->ROUT_RSEQ_REJECT2_RESET.Set = false;
		smProductEvent->ROUT_RSEQ_REJECT3_RESET.Set = false;
		smProductEvent->ROUT_RSEQ_REJECT4_RESET.Set = false;
		smProductEvent->ROUT_RSEQ_REJECT5_RESET.Set = false;

		smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = false;

		smProductEvent->RPNP1_RSEQ_Y_MOVE_STANDBY.Set = false;
		smProductEvent->RPNP2_RSEQ_Y_MOVE_STANDBY.Set = false;
		
		smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set = false;
		smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = false;

		smProductEvent->RPNP_RSEQ_BYPASS_PLACE_FAIL.Set = false;

		smProductEvent->RPNP_RSEQ_BYPASS_PICK_FAIL_REJECT.Set = false;

		smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = false;
		smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = false;
		smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = false;

		smProductProduction->CurrentInputVisionRetryCount = 0;
		smProductProduction->CurrentInputVisionContinuousFailCount = 0;
		smProductProduction->CurrentInputVisionRCRetryCount = 0;
		smProductProduction->CurrentS2VisionRetryCount = 0;
		smProductProduction->CurrentS2VisionContinuousFailCount = 0;
		smProductProduction->CurrentS2VisionRCRetryCount = 0;
		smProductProduction->CurrentS1VisionRetryCount = 0;
		smProductProduction->CurrentS1VisionContinuousFailCount = 0;
		smProductProduction->CurrentS1VisionRCRetryCount = 0;
		smProductProduction->CurrentBottomVisionRetryCount = 0;
		smProductProduction->CurrentBottomVisionContinuousFailCount = 0;
		smProductProduction->CurrentBottomVisionRCRetryCount = 0;
		smProductProduction->CurrentS3VisionRetryCount = 0;
		smProductProduction->CurrentS3VisionContinuousFailCount = 0;
		smProductProduction->CurrentS3VisionRCRetryCount = 0;
		smProductProduction->CurrentOutputVisionRetryCount = 0;
		smProductProduction->CurrentOutputVisionContinuousFailCount = 0;
		smProductProduction->CurrentOutputVisionRCRetryCount = 0;

		smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount = 0;

		smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = false;
		smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = true;

		smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set = false;
		smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set = false;
		smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set = false;
		smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set = false;
		smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set = false;

		smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set = false;
		smProductEvent->ROUT_RSEQ_OUTPUT_FIRST_UNIT.Set = false;
		smProductEvent->ROUT_RSEQ_PNP_PICK_FIRST_UNIT.Set = false;

		smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set = false;
		smProductEvent->RINT_RSEQ_SWITCH_NEXT_INPUT_LOT.Set = false;

		smProductEvent->RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_START.Set = false;
		smProductEvent->RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_FAIL.Set = false;
		smProductEvent->RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_DONE.Set = false;

		smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START.Set = false;
		smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE.Set = false;

		smProductEvent->RMAIN_RTHD_CURRENT_INPUT_LOT_DONE.Set = false;

		smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = false;
		smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = false;
		smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = false;
		smProductEvent->RMAIN_RTHD_UPDATE_MES_DATA.Set = false;
		smProductEvent->RMAIN_RTHD_UPDATE_MES_DATA_DONE.Set = false;

		smProductEvent->RMAIN_RTHD_NEW_OR_END_LOT_CONDITION.Set = false;

		smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = false;
		smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;

		smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST.Set = false;
		smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST_DONE.Set = false;

		smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START_NOMES.Set = false;
		smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE_NOMES.Set = false;

		smProductEvent->RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_START.Set = false;
		smProductEvent->RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_FAIL.Set = false;
		smProductEvent->RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_DONE.Set = false;

		smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set = false;

		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart);
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		sequenceNo++;
	}
	break;

	case nJobSequenceNo.SetEventStartInputAndOutputTableLoadingSequence:
		if (smProductEvent->JobStop.Set == true)
		{
			m_cLogger->WriteLog("Jobseq: User stop sequence.\n");
			////smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
			sequenceNo = 999;
		}
		else if (smProductSetting->EnablePH[0] == false &&
			smProductSetting->EnablePH[1] == false
			)
		{
			////smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
			m_cProductShareVariables->SetAlarm(4201);
			m_cLogger->WriteLog("Jobseq: No Pick and place head is enabled.\n");
			sequenceNo = 999;
		}
		else
		{
			//table sequence start
			if (smProductEvent->RMAIN_RTHD_CONTINUE_LOT.Set == true)
			{
				smProductProduction->bInputContinue = true;
				smProductProduction->bOutputContinue = true;
			}
			else
			{
				smProductProduction->bInputContinue = false;
				smProductProduction->bOutputContinue = false;
			}
			smProductEvent->RSEQ_RINT_SEQUENCE_START.Set = true;
			smProductEvent->RSEQ_ROUT_SEQUENCE_START.Set = true;
			smProductEvent->RSEQ_RPNP1_SEQUENCE_START.Set = true;
			smProductEvent->RSEQ_RPNP2_SEQUENCE_START.Set = true;

			m_bNeedToMovePickAndPlace1XYAxis = true;
			m_bNeedToMovePickAndPlace2XYAxis = true;
			sequenceNo = nJobSequenceNo.StartMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition;
		}
		break;


	case nJobSequenceNo.StartMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition:
		//check both Pnp position and move to standby
		if (smProductSetting->EnablePH[0] == true && smProductSetting->EnablePH[1] == true)
		{
			if (((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.BottomStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace1CurrentStation==PickAndPlaceCurrentStation.HomeStation))
				&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
					&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.HomeStation))
				)
			{
				if (m_bNeedToMovePickAndPlace1XYAxis == true)
				{
					m_bNeedToMovePickAndPlace1XYAxis = false;
					smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.BottomStation;
					smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
				}
				if (m_bNeedToMovePickAndPlace2XYAxis == true)
				{
					m_bNeedToMovePickAndPlace2XYAxis = false;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.S3Station;
					smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
				}
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 1 or 2 to standby position.\n");
				break;
			}
		}
		else if (smProductSetting->EnablePH[0] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor)
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.HomeStation)
				)
			{
				if (m_bNeedToMovePickAndPlace2XYAxis == true)
				{
					m_bNeedToMovePickAndPlace2XYAxis = false;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.DisableStation;
					smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
				}
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
			if ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.BottomStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.HomeStation)
				)
			{
				if (m_bNeedToMovePickAndPlace1XYAxis == true)
				{
					m_bNeedToMovePickAndPlace1XYAxis = false;
					smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.BottomStation;
					smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
				}
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 1 to standby position.\n");
				break;
			}
		}
		else if (smProductSetting->EnablePH[1] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor)
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.HomeStation)
				)
			{
				if (m_bNeedToMovePickAndPlace1XYAxis == true)
				{
					m_bNeedToMovePickAndPlace1XYAxis = false;
					smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.DisableStation;
					smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
				}
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.HomeStation||smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.BottomStation)
				)
			{
				if (m_bNeedToMovePickAndPlace2XYAxis == true)
				{
					m_bNeedToMovePickAndPlace2XYAxis = false;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.BottomStation;
					smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
					smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
				}
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
		}
		else {
			m_cProductShareVariables->SetAlarm(4201);
			m_cLogger->WriteLog("Jobseq: No Pick and place head is enabled.\n");
		}
		if (smProductCustomize->EnableS2VisionModule && smProductCustomize->EnableS2VisionMotor)
		{
			smProductEvent->S2VisionModuleMotorMoveFocusPositionDone.Set = false;
			smProductEvent->StartS2VisionModuleMotorMoveFocusPosition.Set = true;
		}
		if (smProductCustomize->EnableS3VisionModule && smProductCustomize->EnableS3VisionMotor)
		{
			smProductEvent->S3VisionModuleMotorMoveFocusPositionDone.Set = false;
			smProductEvent->StartS3VisionModuleMotorMoveFocusPosition.Set = true;
		}
		if (smProductCustomize->EnableS1VisionModule && smProductCustomize->EnableS1VisionMotor)
		{
			smProductEvent->S1VisionModuleMotorMoveRetractPositionDone.Set = false;
			smProductEvent->StartS1VisionModuleMotorMoveRetractPosition.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start move all Sidewall vision axis to focus position and pick and place head to standby position.\n");
		sequenceNo = nJobSequenceNo.IsMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (((smProductCustomize->EnableS2VisionModule && smProductCustomize->EnableS2VisionMotor && smProductEvent->S2VisionModuleMotorMoveFocusPositionDone.Set == true) || smProductCustomize->EnableS2VisionModule == false || smProductCustomize->EnableS2VisionMotor == false)
			&& ((smProductCustomize->EnableS3VisionModule && smProductCustomize->EnableS3VisionMotor && smProductEvent->S3VisionModuleMotorMoveFocusPositionDone.Set == true) || smProductCustomize->EnableS3VisionModule == false || smProductCustomize->EnableS3VisionMotor == false)
			&& ((smProductCustomize->EnableS1VisionModule && smProductCustomize->EnableS1VisionMotor && smProductEvent->S1VisionModuleMotorMoveRetractPositionDone.Set == true) || smProductCustomize->EnableS1VisionModule == false || smProductCustomize->EnableS1VisionMotor == false)
			&& ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor /*&& smProductSetting->EnablePH[0]*/ && smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace1Module == false || smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false)// || smProductSetting->EnablePH[0] == false)
			&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor /*&& smProductSetting->EnablePH[1]*/ && smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace2Module == false || smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)// || smProductSetting->EnablePH[1] == false)
			)
		{
			m_cLogger->WriteLog("Jobseq: Move all Sidewall vision axis to focus position and pick and place head to standby position done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputVisionDone;
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Move all Sidewall vision axis to focus position and pick and place head to standby position timeout.\n");
			if (((smProductCustomize->EnableS2VisionModule && smProductCustomize->EnableS2VisionMotor && smProductEvent->S2VisionModuleMotorMoveFocusPositionDone.Set == true) || smProductCustomize->EnableS2VisionModule == false || smProductCustomize->EnableS2VisionMotor == false) == false)
			{
				m_cLogger->WriteLog("Jobseq: Move S2 vision axis to focus position timeout.\n");
			}
			if (((smProductCustomize->EnableS3VisionModule && smProductCustomize->EnableS3VisionMotor && smProductEvent->S3VisionModuleMotorMoveFocusPositionDone.Set == true) || smProductCustomize->EnableS3VisionModule == false || smProductCustomize->EnableS3VisionMotor == false) == false)
			{
				m_cLogger->WriteLog("Jobseq: Move S3 vision axis to focus position timeout.\n");
			}
			if (((smProductCustomize->EnableS1VisionModule && smProductCustomize->EnableS1VisionMotor && smProductEvent->S1VisionModuleMotorMoveFocusPositionDone.Set == true) || smProductCustomize->EnableS1VisionModule == false || smProductCustomize->EnableS1VisionMotor == false) == false)
			{
				m_cLogger->WriteLog("Jobseq: Move S1 vision axis to focus position timeout.\n");
			}
			if (((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace1Module == false || smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false) == false)
			{
				m_bNeedToMovePickAndPlace1XYAxis = true;			
				m_cLogger->WriteLog("Jobseq: Move pick and place head 1 to standby position timeout.\n");
			}
			if (((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace2Module == false || smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false) == false)
			{
				m_bNeedToMovePickAndPlace2XYAxis = true;
				m_cLogger->WriteLog("Jobseq: Move pick and place head 2 to standby position timeout.\n");
			}
			sequenceNo = nJobSequenceNo.StopMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition;
		}
		break;

	case nJobSequenceNo.StopMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition:
		smProductEvent->S2VisionModuleMotorStopDone.Set = false;
		smProductEvent->StartS2VisionModuleMotorStop.Set = true;
		smProductEvent->S3VisionModuleMotorStopDone.Set = false;
		smProductEvent->StartS3VisionModuleMotorStop.Set = true;
		smProductEvent->S1VisionModuleMotorStopDone.Set = false;
		smProductEvent->StartS1VisionModuleMotorStop.Set = true;
		m_cLogger->WriteLog("Jobseq: Stop move all Sidewall vision axis to focus position and pick and place head to standby position.\n");
		sequenceNo = nJobSequenceNo.IsStopMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsStopMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (((smProductCustomize->EnableS2VisionModule && smProductCustomize->EnableS2VisionMotor && smProductEvent->S2VisionModuleMotorStopDone.Set == true) || smProductCustomize->EnableS2VisionModule == false || smProductCustomize->EnableS2VisionMotor == false)
			&& ((smProductCustomize->EnableS3VisionModule && smProductCustomize->EnableS3VisionMotor && smProductEvent->S3VisionModuleMotorStopDone.Set == true) || smProductCustomize->EnableS3VisionModule == false || smProductCustomize->EnableS3VisionMotor == false)
			&& ((smProductCustomize->EnableS1VisionModule && smProductCustomize->EnableS1VisionMotor && smProductEvent->S1VisionModuleMotorStopDone.Set == true) || smProductCustomize->EnableS1VisionModule == false || smProductCustomize->EnableS1VisionMotor == false)
			)
		{
			m_cLogger->WriteLog("Jobseq: Stop move all Sidewall vision axis to focus position and pick and place head to standby position done.\n");
			sequenceNo = nJobSequenceNo.StartMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Stop move all Sidewall vision axis to focus position and pick and place head to standby position timeout.\n");
			sequenceNo = nJobSequenceNo.StopMoveAllSidewallVisionAxisToFocusAndAllPickAndPlaceToStandbyPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventInputVisionDone:
		if (smProductEvent->RINT_RSEQ_INPUT_VISION_DONE.Set == true)//ready to pick
		{
			smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = false;
			smProductEvent->RINT_RSEQ_INPUT_VISION_DONE.Set = false;
			m_cLogger->WriteLog("Jobseq: Input vision done.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1Or2ToPickStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnitForCurrentLot==1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnitForCurrentLot==1)
		//{
		//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnitForCurrentLot == 1)
		//	{
		//		smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnitForCurrentLot = 0;
		//	}
		//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnitForCurrentLot == 1)
		//	{
		//		smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnitForCurrentLot = 0;
		//	}
		//	smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = true;
		//	m_cLogger->WriteLog("Jobseq: Empty Tray Before Job real start, set CURRENT_LOT_ALL_PROCESSED \n.");
		//}
		//else if ((smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1Or2ToPickStation:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.InputStation;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[0] == false && smProductSetting->EnablePH[1])
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.InputStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 Or 2 To Pick Station.\n");
		sequenceNo = nJobSequenceNo.IsEventOutputVisionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventOutputVisionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductEvent->ROUT_RSEQ_OUTPUT_VISION_DONE.Set == true)
		{
			smProductEvent->ROUT_RSEQ_OUTPUT_VISION_DONE.Set = false;
			m_cLogger->WriteLog("Jobseq: Output vision done.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1Or2ToPlaceStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if ((smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Output vision timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1Or2ToPlaceStation:
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1Or2ToPickStationDone;
		break;

	case nJobSequenceNo.IsEventPickAndPlace1Or2ToPickStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductSetting->EnablePH[0] == true && smProductSetting->EnablePH[1] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
				)
			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Pick Station done.\n");
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1Or2ToPlaceStationDone;
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
			else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace1 Or 2 To Pick Station timeout.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
		}
		else if (smProductSetting->EnablePH[0] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
				|| smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)

			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Pick Station done.\n");
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1Or2ToPlaceStationDone;
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
			else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Pick Station timeout.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
		}
		else if (smProductSetting->EnablePH[1] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)

			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace2 To Pick Station done.\n");
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1Or2ToPlaceStationDone;
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
			else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("Jobseq: PickAndPlace2 To Pick Station timeout.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			}
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1Or2ToPlaceStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductSetting->EnablePH[0] == true && smProductSetting->EnablePH[1] == true)
		{
			//if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set == true)
			//	|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
			//{
			//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 Or 2 To Place Station done.\n");
				sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			//}
			//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			//{
			//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 Or 2 To Place Station timeout.\n");
			//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
			//	break;
			//}
		}
		else if (smProductSetting->EnablePH[0] == true && smProductSetting->EnablePH[1] == false)
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
		else if (smProductSetting->EnablePH[0] == false && smProductSetting->EnablePH[1] == true)
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
		else
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 and 2 are disabled.\n");
		break;

	case nJobSequenceNo.IsEventInputTrayTableReady:
		if (smProductSetting->EnablePH[0] == false)
		{
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace1Place;
			break;
		}
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false)//Input tray not ready
		{
			break;
		}
		else
		{
			if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set==false /*&& smProductEvent->JobStop.Set == false*/)
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = false;
		}
		if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true)//for Endlot
		{
			smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace1Place;
			break;
		}
		if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.OutputStation)//if Pnp1 at Input Station
		{
			m_cLogger->WriteLog("Jobseq: Input Tray Table ready.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToPickStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)//if Pnp2 at Input Station
		{
			m_cLogger->WriteLog("Jobseq: Input Tray Table ready.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToPickStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToPickStation:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
		{
			if (smProductSetting->EnablePH[1] == true 
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				smProductEvent->PickAndPlace1YAxisMotorMoveCurve.Set = true;
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.InputStation;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Pick Station.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtPickStation;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);
		break;

	case nJobSequenceNo.IsEventPickAndPlace1AtPickStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
			)
		{
			//smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;

			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[1] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Pick Station done in %lfms nTime[1].\n", (double)smProductProduction->nTime[1]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);

			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace1Place;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 disabled, not At Pick Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace1Place;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Pick Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace1Place:
		if ((smProductProduction->OutputTableResult[0].OutputUnitPresent == 1 || smProductProduction->OutputTableResult[0].RejectUnitPresent == 1
				|| smProductProduction->OutputTableResult[0].OutputResult_Post == 3)
				&& smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set == false)
				break;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductProduction->OutputTableResult[0].OutputResult_Post == 1
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 0
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 3
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 5)
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result Pass After PickAndPlace1 Place.\n");
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_START.Set = false;
			smProductEvent->RPNP1_RSEQ_REMOVE_UNIT_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_SKIP.Set = true;
			//if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnit == 1)
			//{
			//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 update last unit.\n");
			//	smProductProduction->WriteReportTrayNo = smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputTrayNo;
			//	smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
			//	smProductEvent->RMAIN_RTHD_CHANGE_MAPPING.Set = true;
			//	smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnit = 0;
			//}
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnitForCurrentLot == 1)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[0].IsLastUnitForCurrentLot = 0;
				smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = true;
			}
			sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove;
			if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == true)
			{
				sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductProduction->OutputTableResult[0].OutputResult_Post == 7)//place fail, first unit
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result Fail After PickAndPlace1 Place.\n");
			//smProductEvent->RSEQ_RPNP2_MOVE_STANDBY_START.Set = true;
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_REMOVE_UNIT_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_SKIP.Set = false;
			smProductEvent->RSEQ_RPNP1_REMOVE_UNIT_START.Set = true;
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set = false;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result After PickAndPlace1 Place receive timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartOutputTrayTableMove:
		m_cLogger->WriteLog("Jobseq: Start Output Tray Table Move.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtS3Station;
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1PickProcessDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToOutputPositionDueToPostVisionFail:
		//different path
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
		{
			if(smProductSetting->EnableOutputVision2ndPostAlign==true)
			{
				if (smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set == false)
				{
					break;
				}
				else
				{
					smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set = false;
				}
			}
			if (smProductSetting->EnablePH[1] == true
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				smProductEvent->PickAndPlace1YAxisMotorMoveCurve.Set = true;
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.OutputStation;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_OUTPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Output Position Due To Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputPositionDueToPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace1AtOutputPositionDueToPostVisionFail:
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
			)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 process at Output Position done Due To Post Vision Fail done.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtOuputStationDueToPostVisionFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 disabled, not At Output Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtOuputStationDueToPostVisionFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//{
		//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Output Position Due To Post Vision Fail timeout.\n");
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1ProcessAtOuputStationDueToPostVisionFailDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At Output Station done due to post vision fail.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartRejectTrayTableMove;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtRejectPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, not process At Output Station.\n");
			sequenceNo = nJobSequenceNo.SetEventStartRejectTrayTableMove;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//{
		//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At Output Station timeout.\n");
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		break;

	case nJobSequenceNo.SetEventStartRejectTrayTableMove:
		//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
		m_cLogger->WriteLog("Jobseq: Start Reject Tray Table Move.\n");
		sequenceNo = nJobSequenceNo.IsEventRejectTrayTableReady;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventRejectTrayTableReady:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true)
		{
			smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Reject Tray Table Ready.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToRejectPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Reject Tray Table Ready timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToRejectPositionDueToPostVisionFail:
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_OUTPUT_STATION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtRejectPositionDueToPostVisionFail;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Reject Position Due To Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtRejectPositionDueToPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace1AtRejectPositionDueToPostVisionFail:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Reject Position Due To Post Vision Fail done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductSetting->EnablePH[0] == false))
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, Place Process not Done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Reject Position Due To Post Vision Fail timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToPickStationAfterPostVisionFail:
		//different path
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
		{
			if (smProductSetting->EnablePH[1] == true
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.OutputStation))
				smProductEvent->PickAndPlace1YAxisMotorMoveCurve.Set = true;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Pick Station After Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtPickStationPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace1AtPickStationPostVisionFail:
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
			)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Pick Station Post Vision Fail done.\n");
			sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 disabled, not At Pick Station.\n");
			sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//{
		//	m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Pick Station Post Vision Fail timeout.\n");
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1PickProcessDone:
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == true/*|| smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true
				|| smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set == true ||smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set==true || smProductEvent->JobStop.Set == true
				||smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set==true || smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set == true*/)
			&& smProductEvent->RPNP1_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[2] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			//smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set = false;
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Pick Process done in %lfms nTime[2].\n", (double)smProductProduction->nTime[2]);
			//m_cLogger->WriteLog("Jobseq: PickAndPlace1 Pick Process Done.\n");
			sequenceNo = nJobSequenceNo.IsEventBottomStationReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductSetting->EnablePH[0] == false))
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, Pick Process not Done.\n");
			sequenceNo = nJobSequenceNo.IsEventBottomStationReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Pick Process timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventBottomStationReady:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (true)
		{
			m_cLogger->WriteLog("Jobseq: Bottom Station Ready.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtS3Station;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Bottom Station Ready timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtS3Station:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station
			//|| smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation
			)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[15] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At S3 Station done in %lfms nTime[15].\n", (double)smProductProduction->nTime[15]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtS3StationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, not At S3 Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtS3StationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At S3 Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2ProcessAtS3StationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1) && smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true) //|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)
			&& smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set == true
			//|| smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation
			)
		{
			//smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = false;
			//smProductProduction->OutputTableResult[0].InputResult = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputResult;
			//smProductProduction->OutputTableResult[0].S2Result = smProductProduction->PickAndPlacePickUpHeadStationResult[1].S2Result;
			//smProductProduction->OutputTableResult[0].BottomResult = smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomResult;
			//smProductProduction->OutputTableResult[0].S1Result = smProductProduction->PickAndPlacePickUpHeadStationResult[1].S1Result;
			//smProductProduction->OutputTableResult[0].S3Result = smProductProduction->PickAndPlacePickUpHeadStationResult[1].S3Result;
			//smProductProduction->OutputTableResult[0].InputTrayNo = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo;
			//smProductProduction->OutputTableResult[0].InputRow = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow;
			//smProductProduction->OutputTableResult[0].InputColumn = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn;
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;

			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[16] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At S3 Station done in %lfms nTime[16].\n", (double)smProductProduction->nTime[16]);

			//m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At S3 Station done.\n");
			sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0) && smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)//|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)
			)
		{
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;

			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[16] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 No Process At S3 Station done in %lfms nTime[16] because no unit.\n", (double)smProductProduction->nTime[16]);

			sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[16] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, No Process At S3 Station done in %lfms nTime[16].\n", (double)smProductProduction->nTime[16]);

			sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At S3 Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventOutputTrayTableReady2:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductSetting->EnablePH[1] == true && smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true && (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 ||
			(smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0))
			&& smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)//output tray ready to place
		{
			//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready 2.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
			else
				sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);			
		}
		else if (smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready but PickAndPlace2 disable.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductSetting->EnablePH[1] == true && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 
			&& smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true && smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set != true)
		{
			//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Output Tray Table Not ready and no unit.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		//else if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& (smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == true || smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true))
		//{
		//	smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
		//	smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
		//	m_cLogger->WriteLog("Jobseq: Pick And Place Contains fail unit and need to move to output station to remove.\n");
		//	sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready 2 timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation:
		//or finish tray
		//if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.BottomStation;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_BOTTOM_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_BOTTOM_STATION_START.Set = true;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Bottom Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtBottomStation;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToOutputStation:
		//if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		//else 
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1)
			{
				//smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
			}
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.OutputStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = true;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtOutputStation;
			else
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtBottomStation;
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtBottomStation;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Output Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1PickProcessDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);
		break;

	//case nJobSequenceNo.IsEventPickAndPlace1PickProcessDone:
	//	if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
	//		/*&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1*/ && smProductEvent->RPNP1_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set == true)
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Pick Process Done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventBottomStationReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductSetting->EnablePH[0] == false))
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, Pick Process not Done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventBottomStationReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Pick Process timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventBottomStationReady:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if (true)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Bottom Station Ready.\n");
	//		sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Bottom Station Ready timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.SetEventStartPickAndPlace1ToBottomStation:
	//	if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
	//	{
	//		smProductEvent->RPNP1_RSEQ_MOVE_TO_BOTTOM_STATION_DONE.Set = false;
	//		smProductEvent->RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set = false;
	//		smProductEvent->RSEQ_RPNP1_MOVE_TO_BOTTOM_STATION_START.Set = true;
	//	}
	//	m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Bottom Station.\n");
	//	sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtBottomStation;
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	break;

	case nJobSequenceNo.IsEventPickAndPlace1AtBottomStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.BottomStation && smProductEvent->RPNP1_RSEQ_MOVE_TO_BOTTOM_STATION_DONE.Set == true)
			|| smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[3] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Bottom Station done in %lfms nTime[3].\n", (double)smProductProduction->nTime[3]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);

			//sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtOutputStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtBottomStationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 To Bottom Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1ProcessAtBottomStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			/*&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)*/ && smProductEvent->RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set == true)
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[4] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 process at Bottom Station done in %lfms nTime[4].\n", (double)smProductProduction->nTime[4]);

			sequenceNo = nJobSequenceNo.IsEventS3StationReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false /*|| smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0*/)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, not process At Bottom Station.\n");
			sequenceNo = nJobSequenceNo.IsEventS3StationReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At Bottom Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventS3StationReady:
		//if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == false)
		//	break;
		if (true)
		{

			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtOutputStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//{
		//	m_cLogger->WriteLog("Jobseq: S3 Station Ready timeout.\n");
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtOutputStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[17] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 at Output Station done in %lfms nTime[17].\n", (double)smProductProduction->nTime[17]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);

			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 disabled, not At Output Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Output Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[18] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 place process done in %lfms nTime[18].\n", (double)smProductProduction->nTime[18]);
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
			sequenceNo = nJobSequenceNo.IsEndLotOrAbort;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductSetting->EnablePH[1] == false))
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, Place Process not Done.\n");
			//sequenceNo = nJobSequenceNo.IsPickAndPlace2Disable;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Place Process timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEndLotOrAbort:
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false)
			break;
		if (smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true)
		{
			smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsPNP2PostProductionDone;
		}
		else
		{
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
		}
		break;

	case nJobSequenceNo.IsPNP2PostProductionDone:
		if (smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Post Production Done.\n");
			if (smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true)// check if PNP1 already done abort
			{
				sequenceNo = nJobSequenceNo.SetInputAndOutputPostProduction;
				break;
			}
			else
			{
				sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
			}
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station:
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false ||((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == false))//Input tray not ready or PNP standy not done during abort
		{
			break;
		}
		if (smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set == true)
		{
			smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = true;
			break;
		}
		else
		{
			smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = false;
		}
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set ==true)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.S3Station;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_S3_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_S3_STATION_START.Set = true;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
			else
				sequenceNo = nJobSequenceNo.IsPickAndPlace2Disable;
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			sequenceNo = nJobSequenceNo.IsPickAndPlace2Disable;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To S3 Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);
		break;


	//case nJobSequenceNo.IsEventPickAndPlace2AtOutputStation:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set == true)
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Output Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 disabled, not At Output Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Output Station timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventPickAndPlace2PlaceProcessDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
	//		&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Place Process Done.\n");
	//		sequenceNo = nJobSequenceNo.IsPickAndPlace2Disable;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductSetting->EnablePH[1] == false))
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, Place Process not Done.\n");
	//		sequenceNo = nJobSequenceNo.IsPickAndPlace2Disable;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Place Process timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	case nJobSequenceNo.IsPickAndPlace2Disable:
		if (smProductSetting->EnablePH[1] == false && smProductProduction->PickAndPlace2CurrentStation != PickAndPlaceCurrentStation.DisableStation)
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.DisableStation;
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToParkingStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductSetting->EnablePH[1] == true || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation)//not disable
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.InputStation;
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToParkingStation:
		if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor)
			)
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.DisableStation;
			smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Parking Station.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtParkingStationDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtParkingStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set == true)
		{
			smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Parking Station Done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Parking Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventInputTrayTableReady2:
		if (smProductSetting->EnablePH[1] == false)
		{
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace2Place;
			break;
		}
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false)//Input tray not ready
		{
			break;
		}
		else
		{
			if(smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set==false && smProductEvent->JobStop.Set == false)
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = false;
		}
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		//or finish tray
		if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true)//for Endlot
		{
			smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace2Place;
			break;
		}
		if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation)//if Pnp2 at Input Station
		{
			m_cLogger->WriteLog("Jobseq: Input Tray Table Ready 2.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToPickStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Input Tray Table Ready 2 timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToPickStation:
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
		{
			if (smProductSetting->EnablePH[0] == true
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = true;
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.InputStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Pick Station.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtPickStation;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtPickStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
			)
		{
			//smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;

			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[11] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Pick Station done in %lfms nTime[11].\n", (double)smProductProduction->nTime[11]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);

			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace2Place;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 disabled, not At Pick Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace2Place;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPostVisionResultPassAfterPickAndPlace2Place:
		if ((smProductProduction->OutputTableResult[0].OutputUnitPresent == 1 || smProductProduction->OutputTableResult[0].RejectUnitPresent == 1
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 3)
			&& smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set == false)
			break;		
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductProduction->OutputTableResult[0].OutputResult_Post == 1
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 0
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 3
			|| smProductProduction->OutputTableResult[0].OutputResult_Post == 5)
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result Pass After PickAndPlace2 Place.\n");
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_START.Set = false;
			smProductEvent->RPNP2_RSEQ_REMOVE_UNIT_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_SKIP.Set = true;
			//if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnit == 1)
			//{
			//	m_cLogger->WriteLog("Jobseq: PickAndPlace2 update last unit.\n");
			//	smProductProduction->WriteReportTrayNo = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo;
			//	smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
			//	smProductEvent->RMAIN_RTHD_CHANGE_MAPPING.Set = true;
			//	smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnit = 0;
			//}
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnitForCurrentLot == 1)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].IsLastUnitForCurrentLot = 0;
				smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = true;
			}
			sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove2;
			if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == true)
			{
				sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductProduction->OutputTableResult[0].OutputResult_Post == 7)//place fail
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result Fail After PickAndPlace2 Place.\n");
			//smProductEvent->RSEQ_RPNP1_MOVE_STANDBY_START.Set = true;
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_REMOVE_UNIT_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_SKIP.Set = false;
			smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_START.Set = true;
			smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set = false;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToOutputPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Post Vision Result After PickAndPlace		2 Place receive timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartOutputTrayTableMove2:
		m_cLogger->WriteLog("Jobseq: Start Output Tray Table Move 2.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtBottomStationDone;
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToOutputPositionDueToPostVisionFail:
		//different path
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
		{
			if (smProductSetting->EnableOutputVision2ndPostAlign == true)
			{
				if (smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set == false)
				{
					break;
				}
				else
				{
					smProductEvent->ROUT_RSEQ_OUTPUT_POST_ALIGNMENT_DONE.Set = false;
				}
			}
			if (smProductSetting->EnablePH[0] == true
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = true;
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.OutputStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Output Position Due To Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtOutputPositionDueToPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtOutputPositionDueToPostVisionFail:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set == true)
			)
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 To Output Position Due To Post Vision Fail done.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtOuputStationDueToPostVisionFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 disabled, not At Output Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtOuputStationDueToPostVisionFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Output Position Due To Post Vision Fail timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2ProcessAtOuputStationDueToPostVisionFailDone:
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)&& smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At Output Station done due to post vision fail.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartRejectTrayTableMove;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtRejectPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, not process At Output Station.\n");
			sequenceNo = nJobSequenceNo.SetEventStartRejectTrayTableMove2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//{
		//	m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At Output Station timeout.\n");
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		break;

	case nJobSequenceNo.SetEventStartRejectTrayTableMove2:
		//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
		m_cLogger->WriteLog("Jobseq: Start Reject Tray Table Move 2.\n");
		sequenceNo = nJobSequenceNo.IsEventRejectTrayTableReady2;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventRejectTrayTableReady2:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true)
		{
			smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Reject Tray Table Ready 2.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToRejectPositionDueToPostVisionFail;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Reject Tray Table Ready 2 timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToRejectPositionDueToPostVisionFail:
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtRejectPositionDueToPostVisionFail;
		}
		m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Reject Position Due To Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtRejectPositionDueToPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtRejectPositionDueToPostVisionFail:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Reject Position Due To Post Vision Fail.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductSetting->EnablePH[1] == false))
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, Place Process not Done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Reject Position Due To Post Vision Fail timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToPickStationAfterPostVisionFail:
		//different path
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
		{
			if (smProductSetting->EnablePH[0] == true
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation))// || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = true;
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.InputStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Pick Station After Post Vision Fail.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtPickStationPostVisionFail;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtPickStationPostVisionFail:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set == true)
			)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Pick Station Post Vision Fail done.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove2;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 disabled, not At Pick Station.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartOutputTrayTableMove2;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Pick Station Post Vision Fail timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	//case nJobSequenceNo.IsEventPickAndPlace1ProcessAtBottomStationDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
	//		&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP1_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set == true)
	//		//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At Bottom Station done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventS3StationReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false || smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, not process At Bottom Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventS3StationReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At Bottom Station timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventS3StationReady:
	//	if (true)
	//	{

	//		sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	//else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	//{
	//	//	m_cLogger->WriteLog("Jobseq: S3 Station Ready timeout.\n");
	//	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	//}
	//	break;

	//case nJobSequenceNo.SetEventStartPickAndPlace1ToS3Station:
	//	if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
	//	{
	//		smProductEvent->RPNP1_RSEQ_MOVE_TO_S3_STATION_DONE.Set = false;
	//		smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = false;
	//		smProductEvent->RSEQ_RPNP1_MOVE_TO_S3_STATION_START.Set = true;
	//	}
	//	m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To S3 Station.\n");
	//	sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtS3Station;
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	break;

	case nJobSequenceNo.IsEventPickAndPlace1AtS3Station:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_S3_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[5] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At S3 Station done in %lfms nTime[5].\n", (double)smProductProduction->nTime[5]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);

			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At S3 Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtS3StationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 disabled, not At S3 Station.\n");
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtS3StationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At S3 Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1ProcessAtS3StationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 && smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)
			&& smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set == true
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			//set vision result to output for tray selection
			//smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = false;
			//smProductProduction->OutputTableResult[0].InputResult = smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputResult;
			//smProductProduction->OutputTableResult[0].S2Result = smProductProduction->PickAndPlacePickUpHeadStationResult[0].S2Result;
			//smProductProduction->OutputTableResult[0].S1Result = smProductProduction->PickAndPlacePickUpHeadStationResult[0].S1Result;
			//smProductProduction->OutputTableResult[0].BottomResult = smProductProduction->PickAndPlacePickUpHeadStationResult[0].BottomResult;
			//smProductProduction->OutputTableResult[0].S3Result = smProductProduction->PickAndPlacePickUpHeadStationResult[0].S3Result;
			//smProductProduction->OutputTableResult[0].InputTrayNo = smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputTrayNo;
			//smProductProduction->OutputTableResult[0].InputRow = smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputRow;
			//smProductProduction->OutputTableResult[0].InputColumn = smProductProduction->PickAndPlacePickUpHeadStationResult[0].InputColumn;
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;

			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[6] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 process At S3 Station done in %lfms nTime[6].\n", (double)smProductProduction->nTime[6]);

			sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			//set vision result to output for tray selection
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[6] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 process At S3 Station done in %lfms nTime[6] because no unit.\n", (double)smProductProduction->nTime[6]);

				m_cLogger->WriteLog("Jobseq: No unit, PickAndPlace1 Process At S3 Station Done.\n");
				sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady;
				RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			//smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[6] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, process S3 Station done in %lfms nTime[6].\n", (double)smProductProduction->nTime[6]);

			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, not process At S3 Station.\n");
			sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At S3 Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventOutputTrayTableReady:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductSetting->EnablePH[0] == true && smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true 
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set==true || (smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0))
			 && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set==true)
		{
			//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
			else
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PickProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductSetting->EnablePH[0] == true && (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			&& smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set != true)
		{
			//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
			m_cLogger->WriteLog("Jobseq: Pick And Place 1 no unit.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PickProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		//else if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		//else if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& (smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == true || smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true))
		//{
		//	m_cLogger->WriteLog("Jobseq: PickAndPlace Contains fail unit, need to go to output station and remove it.\n");
		//	smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
		//	smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
		//	sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PickProcessDone;
		//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		//}
		else if (smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready but PickAndPlace1 disable.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2PickProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Output Tray Table Ready timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2PickProcessDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true /*|| smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true
				|| smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true 
				|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true || smProductEvent->JobStop.Set == true
				||smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true|| smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set == true*/)
			&& smProductEvent->RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[12] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			//smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = false;
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 process At Pick Station done in %lfms nTime[12].\n", (double)smProductProduction->nTime[12]);

			sequenceNo = nJobSequenceNo.IsEventBottomStationReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductSetting->EnablePH[1] == false))// || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[12] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, process At Pick Station done in %lfms nTime[12].\n", (double)smProductProduction->nTime[12]);

			sequenceNo = nJobSequenceNo.IsEventBottomStationReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Pick Process timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventBottomStationReady2:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (true)
		{
			m_cLogger->WriteLog("Jobseq: Bottom Station Ready 2.\n");
			//sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToBottomStation;
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Bottom Station Ready 2 timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation:
		//if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.OutputStation;
			smProductEvent->RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_MOVE_TO_OUTPUT_STATION_START.Set = true;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputStation;
			else
				sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToBottomStation;
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToBottomStation;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Output Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputStation;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);
		break;

	//case nJobSequenceNo.IsEventPickAndPlace2PickProcessDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
	//		&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true )&& smProductEvent->RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set == true)
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Pick Process Done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventBottomStationReady2;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && smProductSetting->EnablePH[1] == false))// || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, Pick Process not Done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventBottomStationReady2;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Pick Process timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventBottomStationReady2:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if (true)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Bottom Station Ready 2.\n");
	//		sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToBottomStation;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Bottom Station Ready 2 timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToBottomStation:
		//if ((smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->JobStop.Set == true)
		//	&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
		//	&& smProductEvent->RPNP1_RSEQ_BYPASS_PICK_FAIL.Set == false && smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
		//{
		//	smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
		//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = true;
		//	sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
		//}
		//else 
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set == true)
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.BottomStation;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_BOTTOM_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_BOTTOM_STATION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputStation;
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputStation;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To Bottom Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1ProcessAtS3StationDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);		
		break;

	//case nJobSequenceNo.IsEventPickAndPlace1ProcessAtS3StationDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
	//		&& (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP1_RSEQ_PROCESS_AT_S3_STATION_DONE.Set == true)
	//		//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
	//		)
	//	{
	//		//set vision result to output for tray selection
	//		smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At S3 Station Done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false || smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
	//	{
	//		smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, not process At S3 Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventOutputTrayTableReady;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace1 Process At S3 Station timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventOutputTrayTableReady:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if (smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true)
	//	{
	//		smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
	//		m_cLogger->WriteLog("Jobseq: Output Tray Table Ready.\n");
	//		sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (smProductSetting->EnablePH[0] == false)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Output Tray Table Ready but PickAndPlace1 disable.\n");
	//		sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: Output Tray Table Ready timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.SetEventStartPickAndPlace1ToOutputStation:
	//	if (smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
	//	{
	//		smProductEvent->RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = false;
	//		smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = false;
	//		smProductEvent->RSEQ_RPNP1_MOVE_TO_OUTPUT_STATION_START.Set = true;
	//	}
	//	m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Output Station.\n");
	//	sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtOutputStation;
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	break;

	case nJobSequenceNo.IsEventPickAndPlace1AtOutputStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[7] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Output Station done in %lfms nTime[7].\n", (double)smProductProduction->nTime[7]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1Start);

			//sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtBottomStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1PlaceProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 disabled, not At Output Station.\n");
			//sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtBottomStation;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace1PlaceProcessDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Bottom Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace1PlaceProcessDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace1XAxisMotor &&  smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0]
			&& smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductEvent->RPNP1_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set == true)
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[8] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Place process done in %lfms nTime[8].\n", (double)smProductProduction->nTime[8]);

			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Place Process Done.\n");
			//sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtBottomStation;
			sequenceNo = nJobSequenceNo.IsEndLotOrAbort2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if ((smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductSetting->EnablePH[0] == false))
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP1End);
			lnUPHClockPnP1Span.QuadPart = lnUPHClockPnP1End.QuadPart - lnUPHClockPnP1Start.QuadPart;
			smProductProduction->nTime[8] = (int)(lnUPHClockPnP1Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 is disable, Place process done in %lfms nTime[8].\n", (double)smProductProduction->nTime[8]);

			//sequenceNo = nJobSequenceNo.IsPickAndPlace1Disable;
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtBottomStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Place Process timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEndLotOrAbort2:
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false)
			break;

		if (smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true)
		{
			smProductEvent->RSEQ_RPNP1_POST_PRODUCTION_START.Set = true;
			sequenceNo = nJobSequenceNo.IsPNP1PostProductionDone;
		}
		else
		{
			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtBottomStation;
		}
		break;

	case nJobSequenceNo.IsPNP1PostProductionDone:
		if (smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Post Production Done.\n");
			if (smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)// check if PNP1 already done abort
			{
				sequenceNo = nJobSequenceNo.SetInputAndOutputPostProduction;
				break;
			}
			else
			{
				sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
			}
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2AtBottomStation:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.BottomStation)
			|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[13] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Bottom Station done in %lfms nTime[13].\n", (double)smProductProduction->nTime[13]);
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);

			sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtBottomStationDone;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Bottom Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsEventPickAndPlace2ProcessAtBottomStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
			/*&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)*/ && smProductEvent->RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set == true)
			//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
			)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[14] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 process At Bottom Station done in %lfms nTime[14].\n", (double)smProductProduction->nTime[14]);

			//sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
			sequenceNo = nJobSequenceNo.IsPickAndPlace1Disable;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false /*|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0*/)
		{
			RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2End);
			lnUPHClockPnP2Span.QuadPart = lnUPHClockPnP2End.QuadPart - lnUPHClockPnP2Start.QuadPart;
			smProductProduction->nTime[14] = (int)(lnUPHClockPnP2Span.QuadPart / m_cProductShareVariables->m_TimeCount);
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, process At Bottom Station done in %lfms nTime[14].\n", (double)smProductProduction->nTime[14]);

			//sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
			sequenceNo = nJobSequenceNo.IsPickAndPlace1Disable;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At Bottom Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.IsPickAndPlace1Disable:
		if (smProductSetting->EnablePH[0] == false && smProductProduction->PickAndPlace1CurrentStation != PickAndPlaceCurrentStation.DisableStation)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.DisableStation;
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 Disable.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace1ToParkingStation;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (smProductSetting->EnablePH[0] == true || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.InputStation;
			//sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace1ToParkingStation:
		if ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor)
			)
		{
			smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.DisableStation;
			smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
			smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace1 To Parking Station.\n");
		sequenceNo = nJobSequenceNo.IsEventPickAndPlace1AtParkingStationDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsEventPickAndPlace1AtParkingStationDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set == true)
		{
			smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Parking Station Done.\n");
			//sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: PickAndPlace1 At Parking Station timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;


	//case nJobSequenceNo.IsEventPickAndPlace2AtBottomStation:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.BottomStation)
	//		|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Bottom Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventPickAndPlace2ProcessAtBottomStationDone;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 At Bottom Station timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	//case nJobSequenceNo.IsEventPickAndPlace2ProcessAtBottomStationDone:
	//	RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
	//	lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
	//	if ((smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
	//		&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true) && smProductEvent->RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set == true)
	//		//|| smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
	//		)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At Bottom Station done.\n");
	//		sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 is disable, not process At Bottom Station.\n");
	//		sequenceNo = nJobSequenceNo.IsEventS3StationReady2;
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
	//	{
	//		m_cLogger->WriteLog("Jobseq: PickAndPlace2 Process At Bottom Station timeout.\n");
	//		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
	//	}
	//	break;

	case nJobSequenceNo.IsEventS3StationReady2:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set == false || ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == false))//Input tray not ready
		{
			break;
		}
		if (smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set == true)
		{
			smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = true;
			break;
		}
		else
		{
			smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = false;
		}
		if (true)
		{
			m_cLogger->WriteLog("Jobseq: S3 Station Ready 2.\n");
			sequenceNo = nJobSequenceNo.SetEventStartPickAndPlace2ToS3Station;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: S3 Station Ready 2 timeout.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetEventStartPickAndPlace2ToS3Station:
		if (smProductCustomize->EnablePickAndPlace2XAxisMotor &&  smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set==true)
		{
			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.S3Station;
			smProductEvent->RPNP2_RSEQ_MOVE_TO_S3_STATION_DONE.Set = false;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = false;
			smProductEvent->RSEQ_RPNP2_MOVE_TO_S3_STATION_START.Set = true;
			if ((smProductEvent->JobStop.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true) && smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true)
				sequenceNo = nJobSequenceNo.IsEventPickAndPlace2AtS3Station;
			else
				sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
		}
		else if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
		{
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
		}
		m_cLogger->WriteLog("Jobseq: Start PickAndPlace2 To S3 Station.\n");
		//sequenceNo = nJobSequenceNo.IsEventPickAndPlace1PlaceProcessDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		RtGetClockTime(CLOCK_FASTEST, &lnUPHClockPnP2Start);
		break;

	case nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition:
		if (smProductSetting->EnablePH[0] == true && smProductSetting->EnablePH[1] == true)
		{
			if (((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.BottomStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
					&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation))
				)
			{
				smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.BottomStation;
				smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.S3Station;
				smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 1 or 2 to standby position.\n");
				break;
			}
		}
		else if (smProductSetting->EnablePH[0] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor)
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation)
				)
			{
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.DisableStation;
				smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
			if ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0])
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.BottomStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation)
				)
			{
				smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.BottomStation;
				smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 1 to standby position.\n");
				break;
			}
		}
		else if (smProductSetting->EnablePH[1] == true)
		{
			if ((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor)
				&& (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.DisableStation)
				)
			{
				smProductProduction->PickAndPlace1StationToMove = PickAndPlaceCurrentStation.DisableStation;
				smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP1_STANDBY_START.Set = true;
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
				&& (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.OutputStation || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station || smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation)
				)
			{
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.BottomStation;
				smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = false;
				smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = true;
			}
			else
			{
				m_cLogger->WriteLog("Jobseq: Undefined pick and place head 2 to standby position.\n");
				break;
			}
		}
		else {
			m_cProductShareVariables->SetAlarm(4201);
			m_cLogger->WriteLog("Jobseq: No Pick and place head is enabled.\n");
		}
		m_cLogger->WriteLog("Jobseq: Start Move All PickAndPlace To Standby Position.\n");
		sequenceNo = nJobSequenceNo.IsMoveAllPickAndPlaceToStandbyPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsMoveAllPickAndPlaceToStandbyPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace1Module == false || smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false)
			&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace2Module == false || smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false))
		{
			m_cLogger->WriteLog("Jobseq: Move All PickAndPlace To Standby Position Done.\n");
			sequenceNo = nJobSequenceNo.IsEventInputTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 5000)
		{
			m_cLogger->WriteLog("Jobseq: Move All PickAndPlace To Standby Position timeout.\n");
			if (((smProductCustomize->EnablePickAndPlace1Module == true && smProductCustomize->EnablePickAndPlace1XAxisMotor && smProductCustomize->EnablePickAndPlace1YAxisMotor && smProductSetting->EnablePH[0] && smProductEvent->RPNP1_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace1Module == false || smProductCustomize->EnablePickAndPlace1XAxisMotor == false || smProductCustomize->EnablePickAndPlace1YAxisMotor == false || smProductSetting->EnablePH[0] == false) == false)
			{
				m_cLogger->WriteLog("Jobseq: Move pick and place head 1 to standby position timeout.\n");
			}
			if (((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1] && smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set == true) || smProductCustomize->EnablePickAndPlace2Module == false || smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false) == false)
			{
				m_cLogger->WriteLog("Jobseq: Move pick and place head 2 to standby position timeout.\n");
			}
			if (smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set==false)
			{
				smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set = true;
				m_cLogger->WriteLog("Jobseq: Trigger Reset Current Lot All Processed Event.\n");
			}
			if (smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set == true
				&& smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
			{
				smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
				smProductEvent->RMAIN_RTHD_CHANGE_MAPPING.Set = true;
				//smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
				//smProductProduction->PreviousInputEdgeCoordinateX = smProductProduction->nEdgeCoordinateX;
				//smProductProduction->PreviousInputEdgeCoordinateY = smProductProduction->nEdgeCoordinateX;
				//smProductProduction->WriteReportTrayNo = smProductProduction->nCurrentInputTrayNo;
				if (smProductSetting->EnableBarcodePrinter == true)
				{
					smProductEvent->RMAIN_RTHD_TRIGGER_BARCODE_PRINTER.Set = true;
				}
				//if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set==true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true)
				{
					smProductEvent->GMAIN_RTHD_ENDLOT.Set = true;
					
					smProductEvent->RMAIN_RTHD_CONTINUE_LOT.Set = false;
					smProductEvent->RMAIN_RTHD_START_RUNNING.Set = false;
					smProductProduction->nInputRunningState = 0;
					smProductProduction->nPNPRunningState = 0;
					smProductProduction->nOutputRunningState = 0;
					//if (smProductSetting->EnableCountDownByInputQuantity == true)
					//{
					//	if (smProductProduction->nCurrentInputLotQuantityRun < smProductProduction->nInputLotQuantity /*&& smProductProduction->nCurrentInputLotQuantityRun != 0 && smProductProduction->nEdgeCoordinateX <=10*/)
					//	{
					//		if (smProductProduction->nEdgeCoordinateX > 10)
					//		{
					//			smProductProduction->nEdgeCoordinateX = 1;
					//			smProductProduction->nEdgeCoordinateY = 1;
					//			//smProductProduction->nCurrentInputTrayNo++;
					//		}
					//		smProductEvent->RMAIN_GMAIN_SAVE_UNFINISHED_LOT.Set = true;
					//	}
					//}
					//else if (smProductSetting->EnableCountDownByInputTrayNo == true)
					//{
					//	if (smProductProduction->nCurrentInputLotTrayNoRun < smProductProduction->nInputLotTrayNo /*&& smProductProduction->nCurrentInputLotQuantityRun != 0 && smProductProduction->nEdgeCoordinateX <=10*/)
					//	{
					//		if (smProductProduction->nEdgeCoordinateX > 10)
					//		{
					//			smProductProduction->nEdgeCoordinateX = 1;
					//			smProductProduction->nEdgeCoordinateY = 1;
					//			//smProductProduction->nCurrentInputTrayNo++;
					//		}
					//		smProductEvent->RMAIN_GMAIN_SAVE_UNFINISHED_LOT.Set = true;
					//	}
					//}
					sequenceNo = nJobSequenceNo.EndOfSequence;
				}
				//else
				//{
				//	smProductEvent->RMAIN_RTHD_CONTINUE_LOT.Set = true;
				//	sequenceNo = nJobSequenceNo.SendVisionEndTrayDuringJobStop;
				//}
			}
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.StopMoveAllPickAndPlaceToStandbyPosition:
		m_cLogger->WriteLog("Jobseq: Stop Move All PickAndPlace To Standby Position.\n");
		sequenceNo = nJobSequenceNo.IsStopMoveAllPickAndPlaceToStandbyPositionDone;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		break;

	case nJobSequenceNo.IsStopMoveAllPickAndPlaceToStandbyPositionDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if (true)
		{
			m_cLogger->WriteLog("Jobseq: Stop Move All PickAndPlace To Standby Position Done.\n");
			sequenceNo = nJobSequenceNo.StartMoveAllPickAndPlaceToStandbyPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		{
			m_cLogger->WriteLog("Jobseq: Stop Move All PickAndPlace To Standby Position timeout.\n");
			sequenceNo = nJobSequenceNo.StopMoveAllPickAndPlaceToStandbyPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;

	case nJobSequenceNo.SetInputAndOutputPostProduction:
		//smProductEvent->RINT_RSEQ_START_POST_PRODUCTION.Set = true;
		smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set = true;
		sequenceNo = nJobSequenceNo.WaitingForPostProductionDone;
		break;

	case nJobSequenceNo.WaitingForPostProductionDone:
		if (smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set == true
			&& smProductEvent->RPNP1_RSEQ_POST_PRODUCTION_DONE.Set == true && smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set == true)
		{
			smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
			smProductEvent->RMAIN_RTHD_CHANGE_MAPPING.Set = true;
			smProductProduction->WriteReportTrayNo = smProductProduction->nCurrentInputTrayNo;
			smProductEvent->GMAIN_RTHD_ENDLOT.Set = true;
			smProductEvent->GMAIN_RTHD_ENDLOT_UPDATE_SUMMARY_DONE.Set = true;
			smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
			//smProductProduction->WriteReportTrayNo = smProductProduction->nCurrentInputTrayNo;
			if (smProductSetting->EnableBarcodePrinter == true)
			{
				smProductEvent->RMAIN_RTHD_TRIGGER_BARCODE_PRINTER.Set = true;
			}
			
			smProductEvent->RMAIN_RTHD_CONTINUE_LOT.Set = false;
			smProductEvent->RMAIN_RTHD_START_RUNNING.Set = false;
			smProductProduction->nInputRunningState = 0;
			smProductProduction->nPNPRunningState = 0;
			smProductProduction->nOutputRunningState = 0;
			//m_cProductIOControl->SetSideWall2_3VisionLighitngExtendCylinder(false);
			sequenceNo = nJobSequenceNo.EndOfSequence;
			//sequenceNo = nJobSequenceNo.SendVisionEndTrayDuringJobStop;
		}
		break;
	case nJobSequenceNo.SendVisionEndTrayDuringJobStop:
		smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDTRAY.Set = true;

		smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_BTM_SEND_ENDTRAY.Set = true;

		smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_S2_SEND_ENDTRAY.Set = true;

		smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_S1_SEND_ENDTRAY.Set = true;

		smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_S3_SEND_ENDTRAY.Set = true;

		smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDTRAY.Set = true;
		
		smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set = false;
		smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDTRAY.Set = true;
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		sequenceNo = nJobSequenceNo.IsSendVisionEndTrayDuringJobStopDone;
		break;
	case nJobSequenceNo.IsSendVisionEndTrayDuringJobStopDone:
		RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockEnd);
		lnJobSequenceClockSpan.QuadPart = lnJobSequenceClockEnd.QuadPart - lnJobSequenceClockStart2.QuadPart;
		if ((smProductSetting->EnableVision==true 
			&& ((smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set==true&& smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true) || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule==false )
			&& ((smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true) || smProductSetting->EnableS2Vision == false || smProductCustomize->EnableS2VisionModule == false)
			&& ((smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
			&& ((smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
			&& ((smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true) || smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false)
			&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
			&& ((smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
			)||smProductSetting->EnableVision==false) //&&smProductEvent->GMAIN_RTHD_OUTPUT_POST_ALIGNMENT_SEND_ENDTRAY_DONE.Set == true
		{
			m_cLogger->WriteLog("Jobseq: Send Endtray Done.\n");
			sequenceNo = nJobSequenceNo.EndOfSequence;
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
		{
			if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_BTM_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S2_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S1_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S3_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S1_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDTRAY.Set = true;
			}
			if (smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set == false)
			{
				smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDTRAY.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnJobSequenceClockStart2);
		}
		break;
	default:
		return -1;
		break;
	}
	return sequenceNo;
}

int CProductSequence::ResetVariablesBeforeHome()
{
	//smProductProduction->nCurrentInputTrayNo = 0;
	//smProductProduction->nCurrentOutputTrayNo = 0;

	smProductEvent->StartPickAndPlace1YAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToInputPosition.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToInputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToInputPositionCurve.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToInputPositionCurveDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToS1Position.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToS1PositionDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToS3Position.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToS3PositionDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToOutputPosition.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToOutputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToOutputPositionCurve.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToOutputPositionCurveDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveToStandbyPosition.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorMoveCurve.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorMoveCurveDone.Set = false;
	smProductEvent->StartPickAndPlace1YAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace1YAxisMotorStopDone.Set = false;

	smProductEvent->StartInputTrayTableXAxisMotorHome.Set = false;
	smProductEvent->InputTrayTableXAxisMotorHomeDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorSettingUp.Set = false;
	smProductEvent->InputTrayTableXAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorMoveLoad.Set = false;
	smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorMoveUnload.Set = false;
	smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorMoveCenter.Set = false;
	smProductEvent->InputTrayTableXAxisMotorMoveCenterDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorMove.Set = false;
	smProductEvent->InputTrayTableXAxisMotorMoveDone.Set = false;
	smProductEvent->StartInputTrayTableXAxisMotorStop.Set = false;
	smProductEvent->InputTrayTableXAxisMotorStopDone.Set = false;

	smProductEvent->StartInputTrayTableYAxisMotorHome.Set = false;
	smProductEvent->InputTrayTableYAxisMotorHomeDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorSettingUp.Set = false;
	smProductEvent->InputTrayTableYAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorMoveLoad.Set = false;
	smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorMoveUnload.Set = false;
	smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorMoveCenter.Set = false;
	smProductEvent->InputTrayTableYAxisMotorMoveCenterDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorMove.Set = false;
	smProductEvent->InputTrayTableYAxisMotorMoveDone.Set = false;
	smProductEvent->StartInputTrayTableYAxisMotorStop.Set = false;
	smProductEvent->InputTrayTableYAxisMotorStopDone.Set = false;

	smProductEvent->StartInputTrayTableZAxisMotorHome.Set = false;
	smProductEvent->InputTrayTableZAxisMotorHomeDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorSettingUp.Set = false;
	smProductEvent->InputTrayTableZAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorMoveDown.Set = false;
	smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorMoveLoad.Set = false;
	smProductEvent->InputTrayTableZAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorMoveSingulation.Set = false;
	smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorMoveUnload.Set = false;
	smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorMove.Set = false;
	smProductEvent->InputTrayTableZAxisMotorMoveDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorStop.Set = false;
	smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorChangeSlowSpeed.Set = false;
	smProductEvent->InputTrayTableZAxisMotorChangeSlowSpeedDone.Set = false;
	smProductEvent->StartInputTrayTableZAxisMotorChangeNormalSpeed.Set = false;
	smProductEvent->InputTrayTableZAxisMotorChangeNormalSpeedDone.Set = false;

	smProductEvent->StartPickAndPlace2YAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMoveToInputPosition.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveToInputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMoveToS1Position.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveToS1PositionDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMoveToS3Position.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMoveToOutputPosition.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveToOutputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;

	smProductEvent->StartOutputTrayTableXAxisMotorHome.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorHomeDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorSettingUp.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMoveLoad.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMoveUnload.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMoveCenter.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveCenterDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMoveRejectTrayCenter.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveRejectTrayCenterDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMoveManualLoadUnload.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
	smProductEvent->StartOutputTrayTableXAxisMotorStop.Set = false;
	smProductEvent->OutputTrayTableXAxisMotorStopDone.Set = false;

	smProductEvent->StartOutputTrayTableYAxisMotorHome.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorHomeDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorSettingUp.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMoveLoad.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMoveUnload.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMoveCenter.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveCenterDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMoveRejectTrayCenter.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveRejectTrayCenterDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMoveManualLoadUnload.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
	smProductEvent->StartOutputTrayTableYAxisMotorStop.Set = false;
	smProductEvent->OutputTrayTableYAxisMotorStopDone.Set = false;

	smProductEvent->StartOutputTrayTableZAxisMotorHome.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorHomeDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorSettingUp.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorMoveDown.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorMoveLoad.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorMoveSingulation.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorMoveUnload.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorMove.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorMoveDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorChangeSlowSpeed.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorChangeSlowSpeedDone.Set = false;
	smProductEvent->StartOutputTrayTableZAxisMotorChangeNormalSpeed.Set = false;
	smProductEvent->OutputTrayTableZAxisMotorChangeNormalSpeedDone.Set = false;

	smProductEvent->StartInputVisionModuleMotorHome.Set = false;
	smProductEvent->InputVisionModuleMotorHomeDone.Set = false;
	smProductEvent->StartInputVisionModuleMotorSettingUp.Set = false;
	smProductEvent->InputVisionModuleMotorSettingUpDone.Set = false;
	smProductEvent->StartInputVisionModuleMotorMoveFocusPosition.Set = false;
	smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set = false;
	smProductEvent->StartInputVisionModuleMotorMove.Set = false;
	smProductEvent->InputVisionModuleMotorMoveDone.Set = false;
	smProductEvent->StartInputVisionModuleMotorStop.Set = false;
	smProductEvent->InputVisionModuleMotorStopDone.Set = false;

	smProductEvent->StartS2VisionModuleMotorHome.Set = false;
	smProductEvent->S2VisionModuleMotorHomeDone.Set = false;
	smProductEvent->StartS2VisionModuleMotorSettingUp.Set = false;
	smProductEvent->S2VisionModuleMotorSettingUpDone.Set = false;
	smProductEvent->StartS2VisionModuleMotorMoveFocusPosition.Set = false;
	smProductEvent->S2VisionModuleMotorMoveFocusPositionDone.Set = false;
	smProductEvent->StartS2VisionModuleMotorMove.Set = false;
	smProductEvent->S2VisionModuleMotorMoveDone.Set = false;
	smProductEvent->StartS2VisionModuleMotorStop.Set = false;
	smProductEvent->S2VisionModuleMotorStopDone.Set = false;

	smProductEvent->StartS3VisionModuleMotorHome.Set = false;
	smProductEvent->S3VisionModuleMotorHomeDone.Set = false;
	smProductEvent->StartS3VisionModuleMotorSettingUp.Set = false;
	smProductEvent->S3VisionModuleMotorSettingUpDone.Set = false;
	smProductEvent->StartS3VisionModuleMotorMoveFocusPosition.Set = false;
	smProductEvent->S3VisionModuleMotorMoveFocusPositionDone.Set = false;
	smProductEvent->StartS3VisionModuleMotorMove.Set = false;
	smProductEvent->S3VisionModuleMotorMoveDone.Set = false;
	smProductEvent->StartS3VisionModuleMotorStop.Set = false;
	smProductEvent->S3VisionModuleMotorStopDone.Set = false;

	smProductEvent->StartPickAndPlace1XAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMoveToInputPosition.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveToInputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMoveToS1Position.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMoveToS3Position.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveToS3PositionDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMoveToOutputPosition.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveToOutputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMoveToParkingPosition.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorStopDone.Set = false;
	smProductEvent->StartPickAndPlace1XAxisMotorOff.Set = false;
	smProductEvent->PickAndPlace1XAxisMotorOffDone.Set = false;

	smProductEvent->StartPickAndPlace2XAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMoveToInputPosition.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveToInputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMoveToS1Position.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMoveToS3Position.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMoveToOutputPosition.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveToOutputPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
	smProductEvent->StartPickAndPlace2XAxisMotorOff.Set = false;
	smProductEvent->PickAndPlace2XAxisMotorOffDone.Set = false;

	smProductEvent->StartPickAndPlace1ZAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMoveUpPosition.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveUpPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMoveToInputTrayDownPosition.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveToInputTrayDownPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPosition.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMoveToOutputTrayDownPosition.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveToOutputTrayDownPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPosition.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveToOutputTraySoftlandingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace1ZAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace1ZAxisMotorStopDone.Set = false;


	smProductEvent->StartPickAndPlace2ZAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPosition.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMoveToInputTrayDownPosition.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveToInputTrayDownPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPosition.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMoveToOutputTrayDownPosition.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTrayDownPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPosition.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;


	smProductEvent->StartPickAndPlace1ThetaAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace1ThetaAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace1ThetaAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace1ThetaAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace1ThetaAxisMotorMoveStandbyPosition.Set = false;
	smProductEvent->PickAndPlace1ThetaAxisMotorMoveStandbyPositionDone.Set = false;
	smProductEvent->StartPickAndPlace1ThetaAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace1ThetaAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace1ThetaAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set = false;


	smProductEvent->StartPickAndPlace2ThetaAxisMotorHome.Set = false;
	smProductEvent->PickAndPlace2ThetaAxisMotorHomeDone.Set = false;
	smProductEvent->StartPickAndPlace2ThetaAxisMotorSettingUp.Set = false;
	smProductEvent->PickAndPlace2ThetaAxisMotorSettingUpDone.Set = false;
	smProductEvent->StartPickAndPlace2ThetaAxisMotorMoveStandbyPosition.Set = false;
	smProductEvent->PickAndPlace2ThetaAxisMotorMoveStandbyPositionDone.Set = false;
	smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = false;
	smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
	smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = false;
	smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
	return 0;
}

int CProductSequence::ResetVariablesBeforeSetup()
{
	smProductEvent->RSEQ_GGUI_UPDATE_MOTION_CHART.Set = false;

	//smProductEvent->RSEQ_RINPV_START_VISION.Set = false;
	//smProductEvent->RINPV_RSEQ_VISION_DONE.Set = true;
	//smProductEvent->RSEQ_RINPV_START_VISION_RETEST.Set = false;
	//smProductEvent->RINPV_RSEQ_VISION_RETEST_DONE.Set = true;

	smProductEvent->RSEQ_RBTMV_START_VISION.Set = false;
	smProductEvent->RBTMV_RSEQ_VISION_DONE.Set = true;
	smProductEvent->RSEQ_RBTMV_START_VISION_RETEST.Set = false;
	smProductEvent->RBTMV_RSEQ_VISION_RETEST_DONE.Set = true;

	smProductEvent->RSEQ_RS1V_START_VISION.Set = false;
	smProductEvent->RS1V_RSEQ_VISION_DONE.Set = true;
	smProductEvent->RSEQ_RS1V_START_VISION_RETEST.Set = false;
	smProductEvent->RS1V_RSEQ_VISION_RETEST_DONE.Set = true;

	smProductEvent->RSEQ_RS3V_START_VISION.Set = false;
	smProductEvent->RS3V_RSEQ_VISION_DONE.Set = true;
	smProductEvent->RSEQ_RS3V_START_VISION_RETEST.Set = false;
	smProductEvent->RS3V_RSEQ_VISION_RETEST_DONE.Set = true;

	//smProductEvent->RSEQ_ROUTV_START_VISION.Set = false;
	//smProductEvent->ROUTV_RSEQ_VISION_DONE.Set = true;
	//smProductEvent->RSEQ_ROUTV_START_VISION_RETEST.Set = false;
	//smProductEvent->ROUTV_RSEQ_VISION_RETEST_DONE.Set = true;

	//smProductEvent->RSEQ_RPOUTV_START_VISION.Set = false;
	//smProductEvent->RPOUTV_RSEQ_VISION_DONE.Set = true;
	//smProductEvent->RSEQ_RPOUTV_START_VISION_RETEST.Set = false;
	//smProductEvent->RPOUTV_RSEQ_VISION_RETEST_DONE.Set = true;
	return 0;
}

int CProductSequence::InitializeBeforeHome()
{
	m_cProductIOControl->SetInputIonizerBlowerValveOn(false);
	m_cProductIOControl->SetOutputIonizerBlowerValveOn(false);
	//m_cProductIOControl->SetPNP1VacuumValveOn(false);
	//m_cProductIOControl->SetPNP2VacuumValveOn(false);

	m_cProductIOControl->SetInputTrayTableVacuumOn(false);

	m_cProductIOControl->SetInputVisionSOV(false);
	m_cProductIOControl->SetInputVisionROV(false);
	m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
	//m_cProductIOControl->SetSideWall2_3VisionLighitngExtendCylinder(false);
	m_cProductIOControl->SetS2VisionSOV(false);
	m_cProductIOControl->SetS2VisionROV(false);
	m_cProductIOControl->SetS1VisionSOV(false);
	m_cProductIOControl->SetS1VisionROV(false);
	m_cProductIOControl->SetS1VisionSOV(false);
	m_cProductIOControl->SetS1VisionROV(false);
	m_cProductIOControl->SetS3VisionSOV(false);
	m_cProductIOControl->SetS3VisionROV(false);
	m_cProductIOControl->SetBottomVisionSOV(false);
	m_cProductIOControl->SetBottomVisionROV(false);
	m_cProductIOControl->SetOutputVisionSOV(false);
	m_cProductIOControl->SetOutputVisionROV(false);
	//if (smProductCustomize->EnablePickAndPlaceModule == true && smProductSetting->EnablePH[0] == true)
	//{
	//	m_cProductMotorControl->THKServoOn(0);
	//}
	if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true)
	{
		m_cProductMotorControl->THKVacuumValveOff(0);
		m_cProductMotorControl->THKReleaseValveOff(0);
	}
	if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
	{
		m_cProductMotorControl->THKVacuumValveOff(1);
		m_cProductMotorControl->THKReleaseValveOff(1);
	}
	return 0;
}

bool CProductSequence::IsReadyToMove()
{
	if (m_cProductIOControl->IsMainPressureSwitchReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5001);
		m_cLogger->WriteLog("Main pressure switch not ready.\n");
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsFrontDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5003);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRearDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5004);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsLeftDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5005);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRightDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5006);
		return false;
	}
	//if (m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder() + m_cProductMotorControl->ReadSidewallVisionFrontModuleMotorEncoder() > 20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 may collide with side wall vision front module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder() + m_cProductMotorControl->ReadSidewallVisionRightModuleMotorEncoder() > 20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 may collide with side wall vision right module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace1YAxisMotorEncoder() - m_cProductMotorControl->ReadSidewallVisionLeftModuleMotorEncoder() < -20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 1 may collide with side wall vision left module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace1YAxisMotorEncoder() - m_cProductMotorControl->ReadSidewallVisionRearModuleMotorEncoder() < -20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 1 may collide with side wall vision rear module.\n");
	//	return false;
	//}
	return true;
}
bool CProductSequence::IsReadyToMoveProduction()
{
	if (m_cProductIOControl->IsMainPressureSwitchReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5001);
		m_cLogger->WriteLog("Main pressure switch not ready.\n");
		return false;
	}

	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsFrontDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5003);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRearDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5004);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsLeftDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5005);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRightDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5006);
		return false;
	}
	//if (m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder() + m_cProductMotorControl->ReadSidewallVisionFrontModuleMotorEncoder() > 20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 may collide with side wall vision front module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder() + m_cProductMotorControl->ReadSidewallVisionRightModuleMotorEncoder() > 20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 may collide with side wall vision right module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace1YAxisMotorEncoder() - m_cProductMotorControl->ReadSidewallVisionLeftModuleMotorEncoder() < -20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 1 may collide with side wall vision left module.\n");
	//	return false;
	//}
	//if (m_cProductMotorControl->ReadPickAndPlace1YAxisMotorEncoder() - m_cProductMotorControl->ReadSidewallVisionRearModuleMotorEncoder() < -20000)
	//{
	//	m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 1 may collide with side wall vision rear module.\n");
	//	return false;
	//}
	return true;
}
bool CProductSequence::IsReadyToHomeOrSetup()
{
	if (m_cProductIOControl->IsMainPressureSwitchReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5001);
		m_cLogger->WriteLog("Main pressure switch not ready.\n");
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsFrontDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5003);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRearDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5004);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsLeftDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5005);
		return false;
	}
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false && m_cProductIOControl->IsRightDoorOpen() == true)
	{
		m_cProductShareVariables->SetAlarm(5006);
		return false;
	}

	if (smProductCustomize->EnablePickAndPlace1XAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Pick And Place 1 X Axis Motor.\n");
		if (m_cProductIOControl->IsPickAndPlace1YAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(55001);
			m_cLogger->WriteLog("Pick And Place 1 X Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnablePickAndPlace2XAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Pick And Place 2 X Axis Motor.\n");
		if (m_cProductIOControl->IsPickAndPlace2YAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(56001);
			m_cLogger->WriteLog("Pick And Place 2 X Axis Motor not ready.\n");
			return false;
		}
	}
	//if (smProductCustomize->EnablePickAndPlace1Module == true)
	//{
	//	m_cLogger->WriteLog("Enable Pick And Place 1 Z Axis Motor.\n");
	//	if (m_cProductIOControl->IsPickAndPlace1ZAxisMotorReady() == false)
	//	{
	//		m_cProductShareVariables->SetAlarm(55001);
	//		m_cLogger->WriteLog("Pick And Place 1 X Axis Motor not ready.\n");
	//		return false;
	//	}
	//}
	//if (smProductCustomize->EnablePickAndPlace2Module == true)
	//{
	//	m_cLogger->WriteLog("Enable Pick And Place 2 Z Axis Motor.\n");
	//	if (m_cProductIOControl->IsPickAndPlace2YAxisMotorReady() == false)
	//	{
	//		m_cProductShareVariables->SetAlarm(56001);
	//		m_cLogger->WriteLog("Pick And Place 2 X Axis Motor not ready.\n");
	//		return false;
	//	}
	//}
	if (smProductCustomize->EnablePickAndPlace1YAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Pick And Place 1 Y Axis Motor.\n");
		if (m_cProductIOControl->IsPickAndPlace1YAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(40001);
			m_cLogger->WriteLog("Pick And Place 1 Y Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Input Tray Table X Axis Motor.\n");
		if (m_cProductIOControl->IsOutputTrayTableXAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(41001);
			m_cLogger->WriteLog("Input Tray Table X Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Input Tray Table Y Axis Motor.\n");
		if (m_cProductIOControl->IsInputTrayTableYAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(42001);
			m_cLogger->WriteLog("Input Tray Table Y Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Input Tray Table Z Axis Motor.\n");
		if (m_cProductIOControl->IsInputTrayTableZAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(43001);
			m_cLogger->WriteLog("Input Tray Table Z Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnablePickAndPlace2YAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Pick And Place 2 Y Axis Motor.\n");
		if (m_cProductIOControl->IsPickAndPlace2YAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(44001);
			m_cLogger->WriteLog("Pick And Place 2 Y Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Output Tray Table X Axis Motor.\n");
		if (m_cProductIOControl->IsOutputTrayTableXAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(45001);
			m_cLogger->WriteLog("Output Tray Table X Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Output Tray Table Y Axis Motor.\n");
		if (m_cProductIOControl->IsOutputTrayTableYAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(46001);
			m_cLogger->WriteLog("Output Tray Table Y Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
	{
		m_cLogger->WriteLog("Enable Output Tray Table Z Axis Motor.\n");
		if (m_cProductIOControl->IsOutputTrayTableZAxisMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(47001);
			m_cLogger->WriteLog("Output Tray Table Z Axis Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputVisionMotor == true)
	{
		m_cLogger->WriteLog("Enable Input Vision Module Motor.\n");
		if (m_cProductIOControl->IsInputVisionModuleMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(48001);
			m_cLogger->WriteLog("Input Vision Module Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableS2VisionMotor == true)
	{
		m_cLogger->WriteLog("Enable S2 Vision Module Motor.\n");
		if (m_cProductIOControl->IsS2VisionModuleMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(49001);
			m_cLogger->WriteLog("S2 Vision Module Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableS1VisionMotor == true)
	{
		m_cLogger->WriteLog("Enable S1 Vision Module Motor.\n");
		if (m_cProductIOControl->IsS1VisionModuleMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(50001);
			m_cLogger->WriteLog("S1 Vision Module Motor not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableS3VisionMotor == true)
	{
		m_cLogger->WriteLog("Enable S3 Vision Module Motor.\n");
		if (m_cProductIOControl->IsS3VisionModuleMotorReady() == false)
		{
			m_cProductShareVariables->SetAlarm(54001);
			m_cLogger->WriteLog("S3 Vision Module Motor not ready.\n");
			return false;
		}
	}
	if (m_cProductIOControl->IsInputVisionLightingRetractSensor() != true || m_cProductIOControl->IsInputVisionLightingExtendSensor() == true)
	{
		m_cProductShareVariables->SetAlarm(5522);
		m_cLogger->WriteLog("Input Vision Lighing Retract Error.\n");
		return false;
	}
	//if (m_cProductIOControl->IsSW2N3VisionLightingDownSensor() != true || m_cProductIOControl->IsSW2N3VisionLightingUpSensor() == true)
	//{
	//	m_cProductShareVariables->SetAlarm(5523);
	//	m_cLogger->WriteLog("Side Wall 2 & 3 Vision Lighing Retract Error.\n");
	//	return false;
	//}
	return true;
}


bool CProductSequence::IsReadyToSetup()
{
	if (IsReadyToHomeOrSetup() == false)
		return false;
	/*if (smProductSetting->InputTrayType == 1 && m_cProductIOControl->IsInputJedecTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5012);
		m_cLogger->WriteLog("Input Jedec Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->InputTrayType == 2 && m_cProductIOControl->IsInputSoftTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5013);
		m_cLogger->WriteLog("Input Soft Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->OutputTrayType == 1 && m_cProductIOControl->IsOutputJedecTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5014);
		m_cLogger->WriteLog("Output Jedec Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->OutputTrayType == 2 && m_cProductIOControl->IsOutputSoftTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5015);
		m_cLogger->WriteLog("Output Soft Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->OutputTrayType == 3 && m_cProductIOControl->IsOutputSpecialCarrierTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5016);
		m_cLogger->WriteLog("Output Special Carrier Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->SortingTrayType == 1 && m_cProductIOControl->IsOutputJedecTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5014);
		m_cLogger->WriteLog("Output Jedec Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->SortingTrayType == 2 && m_cProductIOControl->IsOutputSoftTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5015);
		m_cLogger->WriteLog("Output Soft Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->SortingTrayType == 3 && m_cProductIOControl->IsOutputSpecialCarrierTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5016);
		m_cLogger->WriteLog("Output Special Carrier Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->RejectTrayType == 1 && m_cProductIOControl->IsOutputJedecTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5014);
		m_cLogger->WriteLog("Output Jedec Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->RejectTrayType == 2 && m_cProductIOControl->IsOutputSoftTrayTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5015);
		m_cLogger->WriteLog("Output Soft Tray Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductSetting->RejectTrayType == 3 && m_cProductIOControl->IsOutputSpecialCarrierTableIonizerBlowerReady() == false)
	{
		m_cProductShareVariables->SetAlarm(5016);
		m_cLogger->WriteLog("Output Special Carrier Table Ionizer Blower not ready.\n");
		return false;
	}
	if (smProductCustomize->EnableInputJedecTrayTableYAxisMotor == true)
	{
		if (m_cProductIOControl->IsInputJedecTrayTableYAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(40006);
			m_cLogger->WriteLog("Input Jedec Tray Table Y Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputSoftTrayTableYAxisMotor == true)
	{
		if (m_cProductIOControl->IsInputSoftTrayTableYAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(40006);
			m_cLogger->WriteLog("Input Soft Tray Table Y Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputJedecTrayTableYAxisMotor == true)
	{
		if (m_cProductIOControl->IsOutputJedecTrayTableYAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(40006);
			m_cLogger->WriteLog("Output Jedec Tray Table Y Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputSoftTrayTableYAxisMotor == true)
	{
		if (m_cProductIOControl->IsOutputSoftTrayTableYAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(40006);
			m_cLogger->WriteLog("Output Soft Tray Table Y Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputSpecialCarrierTableYAxisMotor == true)
	{
		if (m_cProductIOControl->IsOutputSpecialCarrierTableYAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(40006);
			m_cLogger->WriteLog("Output Special Carrier Table Y Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableInputVisionXAxisMotor == true)
	{
		if (m_cProductIOControl->IsInputVisionXAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(41006);
			m_cLogger->WriteLog("Input Vision X Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputVisionXAxisMotor == true)
	{
		if (m_cProductIOControl->IsOutputVisionXAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(41006);
			m_cLogger->WriteLog("Output Vision X Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnablePickAndPlacePitchChangerXAxisMotor == true)
	{
		if (m_cProductIOControl->IsPickAndPlaceModulePitchXAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(42006);
			m_cLogger->WriteLog("Pitch Changer X Axis Motor in use.\n");
			return false;
		}
	}
	if (smProductCustomize->EnablePickAndPlaceXAxisMotor == true)
	{
		if (m_cProductIOControl->IsPickAndPlaceModuleXAxisMotorInPosition() == false)
		{
			m_cProductShareVariables->SetAlarm(42006);
			m_cLogger->WriteLog("Pick And Place Module X Axis Motor in use.\n");
			return false;
		}
	}

	if (smProductCustomize->EnableInputVision == true)
	{
		if (m_cProductIOControl->IsInputVisionReadyOn() == false)
		{
			m_cProductShareVariables->SetAlarm(6101);
			m_cLogger->WriteLog("Input Vision Not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableBottomVision == true)
	{
		if (m_cProductIOControl->IsBottomVisionReadyOn() == false)
		{
			m_cProductShareVariables->SetAlarm(6301);
			m_cLogger->WriteLog("Bottom Vision Not ready.\n");
			return false;
		}
	}
	if (smProductCustomize->EnableOutputVision == true)
	{
		if (m_cProductIOControl->IsOutputVisionReadyOn() == false)
		{
			m_cProductShareVariables->SetAlarm(6701);
			m_cLogger->WriteLog("Output Vision Not ready.\n");
			return false;
		}
	}
	if (((smProductCustomize->EnableInputJedecTrayTableZAxisMotor && m_cProductIOControl->IsInputJedecTrayTableTrayPresentSensorOn() == true)
		|| (smProductCustomize->EnableInputSoftTrayTableZAxisMotor && (m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor1On() == true || m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor2On() == true
			|| m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor3On() == true || m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor4On() == true))
		|| (smProductCustomize->EnableOutputJedecTrayTableZAxisMotor &&m_cProductIOControl->IsOutputJedecTrayTableTrayPresentSensorOn() == true)
		|| (smProductCustomize->EnableOutputSoftTrayTableZAxisMotor && (m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor1On() == true || m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor2On() == true
			|| m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor3On() == true || m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor4On() == true))
		|| (smProductCustomize->EnableOutputSpecialCarrierTableZAxisMotor && m_cProductIOControl->IsOutputSpecialCarrierTableTrayPresentSensorOn() == true) && smProductCustomize->EnableAutoInputLoading == true))
	{
		if (m_cProductIOControl->IsInputJedecTrayTableTrayPresentSensorOn() == true)
		{
			m_cProductShareVariables->SetAlarm(5505);
		}
		else if ((m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor1On() == true || m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor2On() == true
			|| m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor3On() == true || m_cProductIOControl->IsInputSoftTrayTableTrayPresentSensor4On() == true))
		{
			m_cProductShareVariables->SetAlarm(5507);
		}
		else if (m_cProductIOControl->IsOutputJedecTrayTableTrayPresentSensorOn() == true)
		{
			m_cProductShareVariables->SetAlarm(5506);
		}
		else if ((m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor1On() == true || m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor2On() == true
			|| m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor3On() == true || m_cProductIOControl->IsOutputSoftTrayTableTrayPresentSensor4On() == true))
		{
			m_cProductShareVariables->SetAlarm(5508);
		}
		else if (m_cProductIOControl->IsOutputSpecialCarrierTableTrayPresentSensorOn() == true)
		{
			m_cProductShareVariables->SetAlarm(5509);
		}
		m_cLogger->WriteLog("Frame present at input table.\n");
		return false;
	}*/
	if (smProductSetting->EnablePH[0] == false
		&& smProductSetting->EnablePH[1] == false
		)
	{
		m_cProductShareVariables->SetAlarm(4201);
		m_cLogger->WriteLog("All Pickup head are disable.\n");
		return false;
	}
	return true;
}
int CProductSequence::SetUpBeforeHome()
{
	if (smProductEvent->GPCS_RPCS_BYPASS_DOOR.Set == false)
	{
	}

	//SetClamperCylinder(false);

	//m_cProductIOControl->SetOutputTableVacuumValve(false);
	//m_cProductIOControl->SetInputTableHotBlowerOn(false);
	return 0;
}

bool CProductSequence::IsPickAndPlace1YAxisSaveToMoveCurve()
{
	if (m_cProductMotorControl->ReadPickAndPlace1XAxisMotorEncoder() > (smProductTeachPoint->PickAndPlace1XAxisOutputPosition + (signed long)380000)
		&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true
			&& smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station)
			|| (smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)))
	{
		return true;
	}
	return false;
}
bool CProductSequence::IsPickAndPlace1YAxisSaveToMoveCurveOutput()
{
	if (m_cProductMotorControl->ReadPickAndPlace1XAxisMotorEncoder() < (smProductTeachPoint->PickAndPlace1XAxisOutputPosition + (signed long)5000)
		&& ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true
			&& smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.S3Station)
			|| (smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)))
	{
		return true;
	}
	return false;
}
bool CProductSequence::IsPickAndPlace2YAxisSaveToMoveCurve()
{
	if (m_cProductMotorControl->ReadPickAndPlace2XAxisMotorEncoder() > (smProductTeachPoint->PickAndPlace2XAxisOutputPosition + (signed long)380000)
		&& ((smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true
			&& smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.S3Station)
			|| (smProductCustomize->EnablePickAndPlace1Module == false || smProductSetting->EnablePH[0] == false)))
	{
		return true;
	}
	return false;
}
bool CProductSequence::IsPickAndPlace2YAxisSaveToMoveCurveOutput()
{
	if (m_cProductMotorControl->ReadPickAndPlace2XAxisMotorEncoder() < (smProductTeachPoint->PickAndPlace2XAxisOutputPosition + (signed long)5000)
		&& ((smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0] == true
			&& smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.S3Station)
			|| (smProductCustomize->EnablePickAndPlace1Module == false || smProductSetting->EnablePH[0] == false)))
	{
		return true;
	}
	return false;
}

int CProductSequence::GetAllStationResult(int PickupHead)
{
	for (int i = 0; i < 10; i++)
	{
		if (smProductSetting->S1Vision[i].Enable == true)
		{
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S1Result == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S1Result == 0)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S1Result = smProductSetting->S1Vision[i].Result;
			}
		}
	}
	//for (int i = 0; i < 10; i++)
	//{
	//	if (smProductSetting->S2Vision[i].Enable == true && (smProductSetting->S2Vision[i].Result != 1 && smProductSetting->S2Vision[i].Result != 0))
	//	{
	//		if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S2PartingResult == 1)
	//		{
	//			smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S2PartingResult = smProductSetting->S2Vision[i].Result;
	//			break;
	//		}
	//	}
	//}
	//for (int i = 0; i < 10; i++)
	//{
	//	if (smProductSetting->S3Vision[i].Enable == true && (smProductSetting->S3Vision[i].Result != 1 && smProductSetting->S3Vision[i].Result != 0))
	//	{
	//		if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S3PartingResult == 1)
	//		{
	//			smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S3PartingResult = smProductSetting->S3Vision[i].Result;
	//			break;
	//		}

	//	}
	//}
	for (int i = 0; i < 10; i++)
	{
		if (smProductSetting->S2FacetVision[i].Enable == true)
		{
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S2Result == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S2Result == 0)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S2Result = smProductSetting->S2FacetVision[i].Result;
			}
		}
	}
	for (int i = 0; i < 10; i++)
	{
		if (smProductSetting->S2FacetVision[i].Enable == true)
		{
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S3Result == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S3Result == 0)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].S3Result = smProductSetting->S3FacetVision[i].Result;
			}
		}
	}
	return 1;
}

int CProductSequence::GetInputStationResult(int PickupHead)
{
	for (int i = 0; i < 10; i++)
	{
		if (smProductSetting->InputVision[i].Enable == true)
		{
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].InputResult == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].InputResult == 0)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[PickupHead - 1].InputResult = smProductSetting->InputVision[i].Result;
			}
		}
	}
	return 0;
}

int CProductSequence::ResetInputStationResult()
{
	for (int i = 0; i < 10; i++)
	{
		smProductSetting->InputVision[i].Result = 0;
	}
	return 0;
}

int CProductSequence::ResetAllStationResult()
{
	for (int i = 0; i < 10; i++)
	{
		//smProductSetting->InputVision[i].Result = 0;
		smProductSetting->S1Vision[i].Result = 0;
		smProductSetting->S2Vision[i].Result = 0;
		smProductSetting->S2FacetVision[i].Result = 0;
		smProductSetting->S3Vision[i].Result = 0;
		smProductSetting->S2FacetVision[i].Result = 0;
	}
	return 0;
}

int CProductSequence::IsOutputNeedWriteReport()
{
	if (smProductProduction->nCurrentInputTrayNumberAtOutput != smProductProduction->OutputTableResult[0].InputTrayNo && smProductProduction->nCurrentInputTrayNumberAtOutput != 0)
	{
		return true;
	}
	return false;
}