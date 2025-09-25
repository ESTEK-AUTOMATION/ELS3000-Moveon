#include "CProductSequence.h"
#include <vector>

int CProductSequence::PickAndPlace2Sequence()
{
	int nError = 0;
	PickAndPlaceSequenceNo nCase;
	int nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceSequence;
	int nSequenceNo_Cont = 999;
	int nHeadNo = 2;
	int nS2S3Cycle = 1;

	bool bPrintLogForFirstTime = false;

	bool bIsS1VisionRequireAdditionalMoveAndSnap = false;
	bool bIsS2VisionRequireAdditionalMoveAndSnap = false;

	bool bTopVisionInspect = false;
	bool bAlignerVisionInspect = false;
	bool bBottomVisionInspect = false;
	bool bSidewallTopBottomVisionInspect = false;
	bool bOutputVisionInspect = false;

	bool bSidewallLeftVisionSendRowColumnDone = false;
	bool bSidewallRightVisionSendRowColumnDone = false;
	bool bSidewallFrontVisionSendRowColumnDone = false;
	bool bSidewallRearVisionSendRowColumnDone = false;
	bool bS3VisionSendRowColumnDone = false;

	bool bCycleStopDone = false;
	int nPreviousSequence = 0;
	int nRetry = 0;
	int nCurrentBottomResultCounter, nCurrentSWLResultCounter, nCurrentSWRResultCounter, nCurrentSWREARResultCounter, nCurrentSWFResultCounter, nCurrentS3ResultCounter;
	bool Retry = false;

	signed long slBottomPickAndPlaceXAxisMove;
	signed long slBottomPickAndPlaceYAxisMove;
	signed long slS3PickAndPlaceXAxisMove;
	signed long slS3PickAndPlaceYAxisMove;

	signed long nSidewallLeftVisionYAxisMove;
	signed long nSidewallRightVisionYAxisMove;
	signed long nSidewallTopVisionYAxisMove;
	signed long nSidewallBottomVisionYAxisMove;
	signed long ThicknessDifference = 0;
	bool IsSidewallLeftVisonWantToMove;
	bool IsSidewallRightVisionWantToMove;
	bool IsSidewallTopVisionWantToMove;
	bool IsSidewallBottomVisionWantToMove;

	int nBottomEndTrayRetryNo = 0;
	int nBottomNewTrayRetryNo = 0;
	int nS2S3EndTrayRetryNo = 0;
	int nS2S3NewTrayRetryNo = 0;

	bool IsBottomVisionAdditionalAllDone = false;
	LARGE_INTEGER lnPnPSequenceClockStart, lnPnPSequenceClockEnd, lnPnPSequenceClockSpan, lnPnPSequenceClockSpan4, lnPnPSequenceClockStart2, lnPnPSequenceClockStart3, lnPnPSequenceClockStart4, lnDelayIn100ns;
	LARGE_INTEGER lnPnPSequenceClockStartCycle;
	LONGLONG  lConvert1msTo100ns = 10000;

	m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PnP sequence\n", nHeadNo);
	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart);

	while (smProductEvent->ExitRTX.Set == false)
	{
		if (smProductEvent->GGUI_RSEQ_CHECK_SEQUENCE.Set == true)
		{
			m_cLogger->WriteLog("PickAndPlaceSeq%d: %u\n", nHeadNo, nSequenceNo);
		}

		if (smProductEvent->GPCS_RSEQ_ABORT.Set == true)
		{
			m_cLogger->WriteLog("PickAndPlaceSeqAbort%d: %u\n", nHeadNo, nSequenceNo);
			return 0;
		}

		if (smProductEvent->Alarm.Set == true || smProductEvent->JobPause.Set == true)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
			continue;
		}

		if (smProductEvent->RMAIN_GMAIN_LOW_YIELD_ALARM_TRIGGER.Set == true)
		{
			smProductProduction->nCurrentLowYieldAlarmQuantity = 0;
			smProductEvent->RMAIN_GMAIN_LOW_YIELD_ALARM_TRIGGER.Set = false;
		}

		if (smProductEvent->JobStart.Set == false)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
			continue;
		}

		if (smProductEvent->JobStep.Set == true)
		{
			if (smProductEvent->JobStart.Set == true)
			{
				smProductEvent->JobStart.Set = false;
			}
			else
			{
				RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
				continue;
			}
		}
		switch (nSequenceNo)
		{
		case nCase.WaitingToReceiveEventStartPickAndPlaceSequence:
			if (smProductEvent->RSEQ_RPNP2_SEQUENCE_START.Set == true)
			{
				lnPnPSequenceClockStartCycle.QuadPart = 0;
				smProductEvent->RSEQ_RPNP2_SEQUENCE_START.Set = false;
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = false;
				//smProductProduction->bPNP2AllowS2S3Snap = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Sequence.\n", nHeadNo);
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyPosition:
			if (smProductEvent->RSEQ_RPNP2_STANDBY_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_STANDBY_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace To Standby Position.\n", nHeadNo);
				nSequenceNo = nCase.SelectPickAndPlaceStandbyStation;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.SelectPickAndPlaceStandbyStation:
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Select PickAndPlace Standby Station.\n", nHeadNo);
			nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToStandbyPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToStandbyPosition:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Pick Or Place Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToStandbyPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Pick Or Place Position.\n", nHeadNo);
			}
			break;

		case nCase.StartMovePickAndPlaceXYAxisToStandbyPosition:
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor)// && smProductSetting->EnablePH[0])
			{
				if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.BottomStation)
				{
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition;
					smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMoveToS1Position.Set = true;
					smProductEvent->PickAndPlace2YAxisMotorMoveToS1PositionDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveToS1Position.Set = true;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To S1 Position.\n", nHeadNo);
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation)
				{
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisParkingPosition;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisStandbyPosition;
					//smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
					//smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
					smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Parking Position.\n", nHeadNo);
					nSequenceNo = nCase.IsMovePickAndPlaceYAxisToParkingPositionDone;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.S3Station)
				{
					smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set = false;
					smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT.Set = true;
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS2AndS3VisionPosition;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS2AndS3VisionPosition;
					smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMoveToS3Position.Set = true;
					smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveToS3Position.Set = true;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To S3 Position.\n", nHeadNo);
				}
				else
				{
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Unknown Standby Position.\n", nHeadNo);
					break;
				}
			}
			nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToStandbyPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceXYAxisToStandbyPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& ((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.BottomStation && smProductEvent->PickAndPlace2YAxisMotorMoveToS1PositionDone.Set == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set == true)
					|| (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.S3Station && smProductEvent->PickAndPlace2YAxisMotorMoveToS3PositionDone.Set == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS3PositionDone.Set == true))
				&& smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false || smProductSetting->EnablePH[1] == false)
			{
				if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.BottomStation)
				{
					smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.BottomStation;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To S1 Position done.\n", nHeadNo);
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.S3Station)
				{
					smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.S3Station;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To S3 Position done.\n", nHeadNo);
				}
				else
				{
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Unknown Standby Position.\n", nHeadNo);
					break;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlaceToStandbyPositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
				{
					smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
				}
				if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
				{
					smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Not Safe To Move or Door Get Trigger %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToStandbyPositionDone;
				break;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToStandbyPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXYAxisToStandbyPosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToStandbyPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYAxisToStandbyPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToStandbyPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToStandbyPosition;
			}
			break;
		case nCase.IsMovePickAndPlaceYAxisToParkingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToParkingPosition;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
				{
					smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Not Safe To Move or Door Get Trigger %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				nSequenceNo = nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone;
				break;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(44002);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToParkingPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceYAxisToParkingPosition:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToStandbyPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(44008);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToParkingPosition;
			}
			break;
		case nCase.StartMovePickAndPlaceXAxisToParkingPosition:
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceXAxisToParkingPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsMovePickAndPlaceXAxisToParkingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
			{
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlaceToStandbyPositionDone;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
				{
					smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Not Safe To Move or Door Get Trigger %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				nSequenceNo = nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone;
				break;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(56002);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToParkingPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXAxisToParkingPosition:
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToParkingPosition;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(56008);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToParkingPosition;
			}
			break;
		case nCase.SetEventPickAndPlaceToStandbyPositionDone:
			smProductEvent->RPNP2_RSEQ_STANDBY_DONE.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace To Standby Position Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickOrPlacePosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToPickOrPlacePosition:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set == true
				|| smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = false;
				smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Pick Or Place Position.\n", nHeadNo);
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToPickOrPlacePosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			//{
			//	smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set = true;
			//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
			//	nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			//}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToPickOrPlacePosition:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Pick Or Place Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToPickOrPlacePosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Pick Or Place Position.\n", nHeadNo);
			}
			break;

		case nCase.StartMovePickAndPlaceXYAxisToPickOrPlacePosition:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
			{
				if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.InputStation)
				{
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisInputPosition + (signed long)smProductSetting->PickingCenterXOffsetInput;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisInputPosition + (signed long)smProductSetting->PickingCenterYOffsetInput;
					smProductEvent->PickAndPlace2XAxisMotorMoveToInputPositionDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMoveToInputPosition.Set = true;
					smProductEvent->PickAndPlace2YAxisMotorMoveToInputPositionDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveToInputPosition.Set = true;
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.OutputStation)
				{
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisOutputPosition;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisOutputPosition;
					smProductEvent->PickAndPlace2XAxisMotorMoveToOutputPositionDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMoveToOutputPosition.Set = true;
					smProductEvent->PickAndPlace2YAxisMotorMoveToOutputPositionDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveToOutputPosition.Set = true;
				}
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move Pick And Place XY Axis To Pick Or Place Position.\n", nHeadNo);
			if (smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true)
			{
				nSequenceNo = nCase.CheckIsPickAndPlaceYAxisReadyToMovePositionCurve;
			}
			else
			{
				nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickOrPlacePositionDone;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.CheckIsPickAndPlaceYAxisReadyToMovePositionCurve:
			if (IsPickAndPlace2YAxisSaveToMoveCurve() == true)
			{
				if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.InputStation)
				{
					if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
					{
						smProductEvent->PickAndPlace2YAxisMotorMoveToInputPositionCurveDone.Set = false;
						smProductEvent->StartPickAndPlace2YAxisMotorMoveToInputPositionCurve.Set = true;
					}
					smProductProduction->bPNP2AllowS2S3Snap = true;
					nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickOrPlacePositionDone;
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.OutputStation)
				{
					if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
					{
						smProductEvent->PickAndPlace1YAxisMotorMoveToOutputPositionCurveDone.Set = false;
						smProductEvent->StartPickAndPlace1YAxisMotorMoveToOutputPositionCurve.Set = true;
					}
					nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickOrPlacePositionDone;
				}
			}
			break;

		case nCase.IsMovePickAndPlaceXYAxisToPickOrPlacePositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& (((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.InputStation && smProductEvent->PickAndPlace2XAxisMotorMoveToInputPositionDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveToInputPositionDone.Set == true)
					&& ((smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveToInputPositionCurveDone.Set == true) || smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == false))
					|| ((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.OutputStation && smProductEvent->PickAndPlace2XAxisMotorMoveToOutputPositionDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveToOutputPositionDone.Set == true)
						&& ((smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveToOutputPositionCurveDone.Set == true) || smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == false))
					)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.InputStation)
				{
					smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = false;
					if (smProductSetting->EnablePH[0] == false)
					{
						smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
					}
					smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.InputStation;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Input Position done.\n", nHeadNo);
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Pick Position done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = true;
				}
				else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.OutputStation)
				{
					smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.OutputStation;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
					smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = false;
					smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Output Position done.\n", nHeadNo);
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Place Position done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = true;//for first time standby without unit
					smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = true;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Pick Or Place Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlaceToPickOrPlacePositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Pick Or Place Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToPickOrPlacePosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXYAxisToPickOrPlacePosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Pick Or Place Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToPickOrPlacePositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYAxisToPickOrPlacePositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Pick Or Place Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToPickOrPlacePosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Pick Or Place Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToPickOrPlacePosition;
			}
			break;

		case nCase.SetEventPickAndPlaceToPickOrPlacePositionDone:
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace To Pick Or Place Position Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_MOVE_TO_INPUT_STATION_START.Set = false;
				smProductProduction->nCurrentPickupHeadAtInput = 1;
				//if (smProductProduction->bolIsLastUnitTo2EndTray == true)
				//{
				//	smProductProduction->bolIsLastUnitTo2EndTray = false;
				//}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Pick Position.\n", nHeadNo);
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//notify end so that go post production
			else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToPickPosition:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Pick Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Pick Position.\n", nHeadNo);
			}
			break;

		case nCase.StartMovePickAndPlaceXYAxisToPickPosition:
		{
			double dCalculatedXPos = 0;
			double dCalculatedYPos = 0;
			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisInputPosition;
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisInputPosition;
			if(m_bHighSpeed)
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 200000, 500, 10000000, 10000000);
			else
			{	//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 500, 250000, 250000);
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 800000, 500, 8500000, 8500000);
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000); //1200000--> 600000 27Nov2024 For Testing Puspose //back to 1200000
			}
			
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Set Path 4 Speed = %d.\n", nHeadNo, nError);
			if (smProductProduction->PickAndPlace2XAxisMovePosition > smProductTeachPoint->PickAndPlace2XAxisInputPosition + (signed long)2000 ||
				smProductProduction->PickAndPlace2XAxisMovePosition < smProductTeachPoint->PickAndPlace2XAxisInputPosition - (signed long)2000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Position is out of Pick And Place 2 X Input Station Limit\n");
				m_cProductShareVariables->SetAlarm(56010);
				break;
			}
			if (smProductProduction->PickAndPlace2YAxisMovePosition > smProductTeachPoint->PickAndPlace2YAxisInputPosition + (signed long)2000 ||
				smProductProduction->PickAndPlace2YAxisMovePosition < smProductTeachPoint->PickAndPlace2YAxisInputPosition - (signed long)2000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Position is out of Pick And Place 2 Y Input Station Limit\n");
				m_cProductShareVariables->SetAlarm(44010);
				break;
			}

			if (smProductSetting->EnableSafetyPnPMovePickStation == true)
			{
				nSequenceNo = nCase.MoveYToStanbyBeforeMoveXToInput;
				break;
			}

			if (smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true)
			{
				if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
				{
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis motor is disabled");
					break;
				}

				if (smProductModuleStatus->IsPickAndPlace2XAxisMotorHome == false)
				{
					m_cProductShareVariables->SetAlarm(56005);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis motor not homing yet.");
					break;
				}
				if (m_cProductIOControl->IsPickAndPlace2XAxisMotorReady() == false)
				{
					m_cProductShareVariables->SetAlarm(56001);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor not ready.");
					break;
				}
				if (m_cProductMotorControl->IsPickAndPlace2XAxisMotorSafeToMove() == false)
				{
					m_cProductShareVariables->SetAlarm(56010);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor not Safe To Move.");
					break;
				}

				if (m_cProductIOControl->IsPickAndPlace2XAxisMotorInPosition() == false)
				{
					m_cProductShareVariables->SetAlarm(56006);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor is busy.");
					break;
				}

				if (smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				{
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis motor is disabled.\n");
					break;
				}
				if (smProductModuleStatus->IsPickAndPlace2YAxisMotorHome == false)
				{
					m_cProductShareVariables->SetAlarm(44005);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis motor not homing yet.\n");
					break;
				}
				if (m_cProductIOControl->IsPickAndPlace2YAxisMotorReady() == false)
				{
					m_cProductShareVariables->SetAlarm(44001);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor not ready.\n");
					break;
				}
				if (m_cProductMotorControl->IsPickAndPlace2YAxisMotorSafeToMove() == false)
				{
					m_cProductShareVariables->SetAlarm(44010);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor not Safe To Move.\n");
					break;
				}

				if (m_cProductIOControl->IsPickAndPlace2YAxisMotorInPosition() == false)
				{
					m_cProductShareVariables->SetAlarm(44006);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor is busy.");
					break;
				}
			}

			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			}
			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;// +smProductSetting->UnitPlacementRotationOffsetInput;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace XY Theta Axis To Pick Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2XAxisInputPosition %lf.\n", (double)(smProductTeachPoint->PickAndPlace2XAxisInputPosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisInputPosition %lf.\n", (double)(smProductTeachPoint->PickAndPlace2YAxisInputPosition));
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisStandbyPosition %lf.\n", (double)(smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition));
			m_cLogger->WriteLog("InputThetaOffset %lf.\n", (double)smProductProduction->InputThetaOffset);

			smProductEvent->RPNP2_ROUT_THREAD_AWAY_OUTPUT.Set = true;
			if (smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true)
			{
				nSequenceNo = nCase.CheckIsPickAndPlaceYAxisReadyMoveToPickCurvePosition;
			}
			else
			{
				nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickPositionDone;
			}
			//nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		}
		break;

		case nCase.MoveYToStanbyBeforeMoveXToInput:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			}
			//smProductProduction->bPNP2AllowS2S3Snap = true;
			nSequenceNo = nCase.IsMoveYToStanbyBeforeMoveXToInputDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMoveYToStanbyBeforeMoveXToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				}

				smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;// +smProductSetting->UnitPlacementRotationOffsetInput;
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
					smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
				}

				nSequenceNo = nCase.CheckIsPickAndPlaceXAxisReadyMoveToPickCurvePositionForS2S3VisionInspection;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Stanby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveYToStanbyBeforeMoveXToInput;
			}
			break;

		case nCase.StopMoveYToStanbyBeforeMoveXToInput:
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Stanby Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveYToStanbyBeforeMoveXToInputDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMoveYToStanbyBeforeMoveXToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Stanby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.MoveYToStanbyBeforeMoveXToInput;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Stanby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveYToStanbyBeforeMoveXToInput;
			}
			break;

		case nCase.CheckIsPickAndPlaceXAxisReadyMoveToPickCurvePositionForS2S3VisionInspection:
			if (smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set == false)
			{
				if (m_cProductMotorControl->ReadPickAndPlace2XAxisMotorEncoder() > (smProductTeachPoint->PickAndPlace2XAxisOutputPosition + (signed long)50000))
					smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
			}
			if (IsPickAndPlace2YAxisSaveToMoveCurve() == true)
			{				
				smProductProduction->bPNP2AllowS2S3Snap = true;
				nSequenceNo = nCase.IsMoveXAndThetaToInputDone;
			}
			break;

		case nCase.IsMoveXAndThetaToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductSetting->EnablePH[1]
				&& ((smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true))
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				nSequenceNo = nCase.MoveYToInputAfterMoveXToInput;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X And Theta Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveXAndThetaToInput;
			}
			break;
		case nCase.StopMoveXAndThetaToInput:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;

			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X And Theta Axis To Pick Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveXAndThetaToInputDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMoveXAndThetaToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X And Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				}

				smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;// +smProductSetting->UnitPlacementRotationOffsetInput;
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
					smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
				}

				nSequenceNo = nCase.IsMoveXAndThetaToInputDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X And Theta Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveXAndThetaToInput;
			}
			break;

		case nCase.MoveYToInputAfterMoveXToInput:

			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveCurve.Set = true;
			}
			nSequenceNo = nCase.IsMoveYToInputAfterMoveXToInputDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMoveYToInputAfterMoveXToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& ((smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true)
					&& ((smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set == true) || smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == false))
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
				smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = false;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.InputStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = false;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XY Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("PickAndPlace1XAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(0, 0, 0)));
				m_cLogger->WriteLog("PickAndPlace1YAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace1ZAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2ZAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace1ThetaAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2ThetaAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace1XAxisLockPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorLockPosition(0, 0, 0)));
				//nSequenceNo = nCase.IsPostProductionDone;
				nSequenceNo = nCase.IsInputRequirePurging;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveYToInputAfterMoveXToInput;
			}
			break;

		case nCase.StopMoveYToInputAfterMoveXToInput:
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Pick Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveYToInputAfterMoveXToInputDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMoveYToInputAfterMoveXToInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.MoveYToInputAfterMoveXToInput;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveYToInputAfterMoveXToInput;
			}
			break;

		case nCase.CheckIsPickAndPlaceYAxisReadyMoveToPickCurvePosition:
			if(smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set == false)
			{
				if (m_cProductMotorControl->ReadPickAndPlace2XAxisMotorEncoder() > (smProductTeachPoint->PickAndPlace2XAxisOutputPosition + (signed long)50000))
					smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
			}
			if (IsPickAndPlace2YAxisSaveToMoveCurve() == true)
			{
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveCurve.Set = true;
				}
				smProductProduction->bPNP2AllowS2S3Snap = true;
				nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToPickPositionDone;
			}
			break;

		case nCase.IsMovePickAndPlaceXYAxisToPickPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& ((smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true)
					&& ((smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set == true) || smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == false))
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) < smProductProduction->PickAndPlace2XAxisMovePosition - 200
					|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) > smProductProduction->PickAndPlace2XAxisMovePosition + 200)
				{
					m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition when Input not going position %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
					m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition when Input not going position %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
					nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToPickPosition;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
				smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
				smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = false;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.InputStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = false;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_INPUT_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XY Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
				m_cLogger->WriteLog("PickAndPlace2YAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2YAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace2ZAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2ZAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace2ThetaAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace2ThetaAxisMotorEncoder()));
				m_cLogger->WriteLog("PickAndPlace2XAxisLockPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorLockPosition1(1, 0, 0)));
				//nSequenceNo = nCase.IsPostProductionDone;
				nSequenceNo = nCase.IsInputRequirePurging;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Theta Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToPickPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXYAxisToPickPosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Pick Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToPickPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYAxisToPickPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Theta Axis To Pick Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Theta Axis To Pick Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToPickPosition;
			}
			break;
		case nCase.IsInputRequirePurging:
			if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == true)
			{
				nSequenceNo = nCase.StartMovePickAndPlaceZToPurgingPosition;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Input Unit Requires Purging.\n", nHeadNo);
				break;
			}
			else
			{
				nSequenceNo = nCase.IsPostProductionDone;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Input Unit Do Not Require Purging, Proceed to Check Output Post Result.\n", nHeadNo);
				break;
			}
			break;
		case nCase.StartMovePickAndPlaceZToPurgingPosition:
			//if (smProductEvent->JobStop.Set == true)
			//{
			//	nSequenceNo = nCase.IsPostProductionDone;
			//	smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set = false;
			//	break;
			//}
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition - (signed long)3000;
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMove.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceZToPurgingPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);//for calculate placement time
			break;
		case nCase.IsMovePickAndPlaceZToPurgingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true && smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Purging Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlaceReleaseAndOffValveDone;
				//if (smProductEvent->JobStop.Set == true)
				//{
				//	nSequenceNo = nCase.IsPostProductionDone;
				//	break;
				//}
				smProductEvent->PickAndPlace2ZAxisReleaseAndOffValveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisReleaseAndOffValve.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Purging Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZToPurgingPosition;
			}
			break;
		case nCase.StopMovePickAndPlaceZToPurgingPosition:
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceZToPurgingPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMovePickAndPlaceZToPurgingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Z Axis To Purging Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZToPurgingPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Z Axis To Purging Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZToPurgingPosition;
			}
			break;

		case nCase.IsPickAndPlaceReleaseAndOffValveDone:
			//if (smProductEvent->JobStop.Set == true)
			//{
			//	nSequenceNo = nCase.IsPostProductionDone;
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->PickAndPlace2ZAxisReleaseAndOffValveDone.Set == true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Pickup Head Release And Off Valve Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPostProductionDone;
				break;
				//return nError;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Pickup Head Release And Off Valve Timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->PickAndPlace2ZAxisReleaseAndOffValveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisReleaseAndOffValve.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsPostProductionDone:
			//check post vision done, if not done, need go back output station to pickup again.
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_SKIP.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_SKIP.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Post Vision Result Pass After PickAndPlace1 Place.\n", nHeadNo);
				if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
				{
					if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == false)
					{
						nSequenceNo = nCase.IsTrayTableReadyToPick;
					}
					else
					{
						nSequenceNo = nCase.StartMovePickAndPlaceYZToStandbyPositionAfterPurging;
					}

				}
				else if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.DisableStation)
					nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_START.Set == true)//place fail
			{
				smProductEvent->RSEQ_RPNP2_REMOVE_UNIT_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Post Vision Result Fail After PickAndPlace1 Place.\n", nHeadNo);
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
				break;
			break;

		case nCase.StartMovePickAndPlaceYZToStandbyPositionAfterPurging:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition;
				smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMove.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYZToStandbyPositionAfterPurgingDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsMovePickAndPlaceYZToStandbyPositionAfterPurgingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true) || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& ((smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
				)
			{
				//smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y And Z Axis To Standby Position Done After Purging %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set = false;
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y And Z Axis To Standby Position Done After Purging timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYZToStandbyPositionAfterPurging;
			}
			break;
		case nCase.StopMovePickAndPlaceYZToStandbyPositionAfterPurging:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceYZToStandbyPositionAfterPurgingDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMovePickAndPlaceYZToStandbyPositionAfterPurgingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (
				((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& ((smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
				)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y And Z Axis To Standby Position Done After Purging %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYZToStandbyPositionAfterPurging;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y And Z Axis To Standby Position After Purging timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYZToStandbyPositionAfterPurging;
			}
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToOutputPositionToPick:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Output Position To Pick.\n", nHeadNo);
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			//{
			//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
			//	nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			//}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToOutputPositionToPick:
			if (/*smProductEvent->RPNP1_RSEQ_MOVE_STANDBY_DONE.Set==*/true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Output Position To Pick.\n", nHeadNo);
				if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == true)
				{
					nSequenceNo = nCase.StartMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick;
				}
				else
				{
					nSequenceNo = nCase.StartMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Output Position To Pick.\n", nHeadNo);
			}
			break;
		case nCase.StartMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPickDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPickDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done before To Output Position To Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done before To Output Position To Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick;
			}
			break;
		case nCase.StopMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPickDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPickDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y Axis To Standby Position Done before To Output Position To Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y Axis To Standby Position Done before To Output Position To Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYToStandbyPositionBeforeOutputPositionToPick;
			}
			break;
		case nCase.StartMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition;
				smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMove.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPickDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPickDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true) || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& ((smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
				)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y And Z Axis To Standby Position Before Moving To Output Done After Purging %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y And Z Axis To Standby Position Before Moving To Output Done After Purging timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick;
			}
			break;
		case nCase.StopMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPickDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPickDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (
				((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& ((smProductCustomize->EnablePickAndPlace2Module && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
					|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
				)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y And Z Axis To Standby Position Before Moving To Output Done After Purging %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y And Z Axis To Standby Position Before Moving To Output After Purging timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYZToStandbyPositionBeforeOutputPositionToPick;
			}
			break;
		case nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPositionToPick:
		{
			double dCalculatedOutputXOffset = 0;
			double dCalculatedOutputYOffset = 0;

			double dCalculatedOutputVisionXOffset = 0;
			double dCalculatedOutputVisionYOffset = 0;

			//GetNewXYOffsetFromThetaCorretion(-(double)smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree_Post + (double)smProductSetting->PickUpHeadOutputCompensationThetaOffset[0]
			//	, (double)smProductProduction->OutputTableResult[0].OutputXOffset_um_Post - (double)(smProductSetting->PickUpHeadRotationXOffset[1])
			//	, (double)smProductProduction->OutputTableResult[0].OutputYOffset_um_Post - (double)(smProductSetting->PickUpHeadRotationYOffset[1])
			//	, &dCalculatedOutputVisionXOffset, &dCalculatedOutputVisionYOffset);

			//GetNewXYOffsetFromThetaCorretion((double)smProductSetting->PickUpHeadOutputCompensationThetaOffset[1]
			//	, (double)smProductSetting->PickUpHeadOutputCompensationXOffset[1]
			//	, (double)smProductSetting->PickUpHeadOutputCompensationYOffset[1]
			//	, &dCalculatedOutputXOffset, &dCalculatedOutputYOffset);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisOutputPosition
				+ signed long(dCalculatedOutputVisionXOffset + (smProductSetting->PickUpHeadRotationXOffset[1]))
				- dCalculatedOutputXOffset;

			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisOutputPosition
				+ signed long(dCalculatedOutputVisionYOffset + (smProductSetting->PickUpHeadRotationYOffset[1]))
				- dCalculatedOutputYOffset;
			if (m_bHighSpeed)
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1200000, 500, 10000000, 10000000);
			else
			{	//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 500, 250000, 250000);
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 700000, 500, 8500000, 8500000);
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000); //700000--> 350000 27Nov2024 For Testing Puspose //back to 700000
			}
			if (smProductProduction->PickAndPlace2XAxisMovePosition > smProductTeachPoint->PickAndPlace2XAxisOutputPosition + (signed long)2000 ||
				smProductProduction->PickAndPlace2XAxisMovePosition < smProductTeachPoint->PickAndPlace2XAxisOutputPosition - (signed long)2000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Position is out of Pick And Place 2 X Output Station Limit\n");
				m_cProductShareVariables->SetAlarm(56010);
				break;
			}
			if (smProductProduction->PickAndPlace2YAxisMovePosition > smProductTeachPoint->PickAndPlace2YAxisOutputPosition + (signed long)2000 ||
				smProductProduction->PickAndPlace2YAxisMovePosition < smProductTeachPoint->PickAndPlace2YAxisOutputPosition - (signed long)2000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Position is out of Pick And Place 2 Y Output Station Limit\n");
				m_cProductShareVariables->SetAlarm(44010);
				break;
			}
			if (smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true)
			{
				if (smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
				{
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis motor is disabled");
					break;
				}

				if (smProductModuleStatus->IsPickAndPlace2XAxisMotorHome == false)
				{
					m_cProductShareVariables->SetAlarm(56005);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis motor not homing yet.");
					break;
				}
				if (m_cProductIOControl->IsPickAndPlace2XAxisMotorReady() == false)
				{
					m_cProductShareVariables->SetAlarm(56001);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor not ready.");
					break;
				}
				if (m_cProductMotorControl->IsPickAndPlace2XAxisMotorSafeToMove() == false)
				{
					m_cProductShareVariables->SetAlarm(56010);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor not Safe To Move.");
					break;
				}

				if (m_cProductIOControl->IsPickAndPlace2XAxisMotorInPosition() == false)
				{
					m_cProductShareVariables->SetAlarm(56006);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 X Axis Motor is busy.");
					break;
				}

				if (smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				{
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis motor is disabled.\n");
					break;
				}
				if (smProductModuleStatus->IsPickAndPlace2YAxisMotorHome == false)
				{
					m_cProductShareVariables->SetAlarm(44005);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis motor not homing yet.\n");
					break;
				}
				if (m_cProductIOControl->IsPickAndPlace2YAxisMotorReady() == false)
				{
					m_cProductShareVariables->SetAlarm(44001);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor not ready.\n");
					break;
				}
				if (m_cProductMotorControl->IsPickAndPlace2YAxisMotorSafeToMove() == false)
				{
					m_cProductShareVariables->SetAlarm(44010);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor not Safe To Move.\n");
					break;
				}

				if (m_cProductIOControl->IsPickAndPlace2YAxisMotorInPosition() == false)
				{
					m_cProductShareVariables->SetAlarm(44006);
					m_cProductShareVariables->UpdateMessageToGUIAndLog("Pick And Place 2 Y Axis Motor is busy.");
					break;
				}
			}
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				//smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisOutputPosition;
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				//smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisOutputPosition;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			}

			smProductProduction->PickAndPlace2ThetaAxisMovePosition = (smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition - smProductSetting->OutputVisionUnitThetaOffset - smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree_Post - smProductSetting->PickUpHeadOutputCompensationThetaOffset[1]);
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition2 + (signed long)smProductSetting->UnitThickness_um;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace XYZTheta Axis To Output Position.\n", nHeadNo);
			if (smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true)
			{
				nSequenceNo = nCase.CheckIsPickAndPlaceYAxisReadyToMoveCurveOutputPositionToPick;
			}
			else
			{
				nSequenceNo = nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick;
			}
			//nSequenceNo = nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		}
		break;

		case nCase.CheckIsPickAndPlaceYAxisReadyToMoveCurveOutputPositionToPick:
			if (IsPickAndPlace2YAxisSaveToMoveCurveOutput() == true)
			{
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
				{
					smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorMoveCurve.Set = true;
				}
				nSequenceNo = nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick;
			}
			break;
		case nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true)
					&& ((smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveCurveDone.Set == true) || smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set == false))
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				//smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = true;
				smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = false;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.OutputStation;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = true;
				smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set = false;
				smProductEvent->RPNP2_RSEQ_Y_MOVE_STANDBY.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYXTheta Axis To Output Position Done To Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsTrayTableReadyToPickAfterFail;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Output Position To Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPositionToPick;
			}
			break;

		case nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPositionToPick:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYTheta Axis To Output Position To Pick.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYZThetaAxisToOutputPositionDoneToPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Output Position Done To Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Output Position To Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPositionToPick;
			}
			break;

		case nCase.IsTrayTableReadyToPickAfterFail:
			if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				nSequenceNo = nCase.SetEventPickAndPlacePickProcessDoneAfterPick;
			}
			else if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 && 
				((smProductProduction->OutputTableResult[0].OutputUnitPresent == 1 && smProductEvent->ROUT_RSEQ_OUTPUT_FIRST_UNIT.Set == false) || (smProductEvent->ROUT_RSEQ_OUTPUT_FIRST_UNIT.Set == true && smProductProduction->OutputTableResult[0].RejectUnitPresent == 1)))
			{
				smProductProduction->nCurrentPickAndPlace2PickingRetry = 0;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Tray Table Ready To Place position to pick.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePositionToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if ((smProductProduction->OutputTableResult[0].OutputUnitPresent == 0 && smProductEvent->ROUT_RSEQ_OUTPUT_FIRST_UNIT.Set == false) || (smProductEvent->ROUT_RSEQ_OUTPUT_FIRST_UNIT.Set == true && smProductProduction->OutputTableResult[0].RejectUnitPresent == 0))
			{
				nSequenceNo = nCase.SetEventPickAndPlacePickProcessDoneAfterPick;
			}
			break;

		case nCase.StartMovePickAndPlaceZAxisToDownPlacePositionToPick:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPicking.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Axis To Down Place Position To Pick.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToDownPlacePositionDoneToPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceZAxisToDownPlacePositionDoneToPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Place Position Done After Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlaceZAxisPlaceUnitDoneToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				//m_cProductShareVariables->SetAlarm(12027);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Place Position After Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPlacePositionToPick;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToDownPlacePositionToPick:
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position To Pick.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToDownPlacePositionDoneToPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToDownPlacePositionDoneToPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position Done To Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisOffVacuumForOutputPickFail;
				//nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPositionAfterPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position To Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPlacePositionToPick;
			}
			break;

		case nCase.StartPickAndPlaceZAxisOffVacuumForOutputPickFail:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2ZAxisOffVacuumDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisOffVacuum.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis Off Vacuum due to output pick fail.\n", nHeadNo);
			nSequenceNo = nCase.IsPickAndPlaceZAxisOffVacuumForOutputPickFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsPickAndPlaceZAxisOffVacuumForOutputPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOffVacuumDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Vacuum due to Output pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisValveOnForOutputPickFail;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Vacuum due to Output pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisOffVacuumForOutputPickFail;
			}
			break;
		case nCase.StartPickAndPlaceZAxisValveOnForOutputPickFail:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2ZAxisOnValveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisOnValve.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis On Valve due to Output pick fail.\n", nHeadNo);
			nSequenceNo = nCase.IsPickAndPlaceZAxisValveOnForOutputPickFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsPickAndPlaceZAxisValveOnForOutputPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOnValveDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis On Valve due to Output pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsDelayForPurgingReachedBeforeOffValveForOutputPickFail;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis On Valve due to Output pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisValveOnForOutputPickFail;
			}
			break;
		case nCase.IsDelayForPurgingReachedBeforeOffValveForOutputPickFail:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= smProductSetting->DelayForPickupHeadPurgeAtDownPositionWhenPickFail)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis Off Valve due to Output pick fail %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				{
					smProductEvent->PickAndPlace2ZAxisOffValveDone.Set = false;
					smProductEvent->StartPickAndPlace2ZAxisOffValve.Set = true;
				}
				nSequenceNo = nCase.IsPickAndPlaceZAxisValveOffForOutputPickFailDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;
		case nCase.IsPickAndPlaceZAxisValveOffForOutputPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOffValveDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Valve due to Output pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlaceZAxisPlaceUnitDoneToPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Valve due to Output pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				{
					smProductEvent->PickAndPlace2ZAxisOffValveDone.Set = false;
					smProductEvent->StartPickAndPlace2ZAxisOffValve.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsPickAndPlaceZAxisPlaceUnitDoneToPick:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Place Unit Done To Pick.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPositionAfterPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartMovePickAndPlaceZAxisToUpPositionAfterPick:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition;
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPosition.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Axis To Up Position After Pick.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToUpPositionDoneAfterPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceZAxisToUpPositionDoneAfterPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up Position Done After Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//if (smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone.Set == false)
				//{
				//	nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePositionToPick;
				//}
				//else
				{
					nSequenceNo = nCase.IsPickAndPlacePickZAxisAtUpPositionDoneAfterPick;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up Position After Picktimeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpPositionAfterPick;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToUpPositionAfterPick:
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up Position After Pick.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToUpPositionDoneAfterPick;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToUpPositionDoneAfterPick:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up Position Done After Pick %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPositionAfterPick;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up And Theta To Orientation Position After Pick timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpPositionAfterPick;
			}
			break;

		case nCase.IsPickAndPlacePickZAxisAtUpPositionDoneAfterPick:
		{
			double dblReturnValue;
			//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			if (dblReturnValue <= smProductSetting->PickUpHead2Pressure)
			{
				smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductProduction->nCurrrentTotalUnitDoneByLot--;
					smProductProduction->nCurrentTotalUnitDone--;
					smProductProduction->nCurrentLotGoodQuantity--;
					smProductProduction->OutputQuantity--;
				}
				if (smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionForPickingDone.Set == true)
				{
					m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis At Up Position Done After Pick.\n", nHeadNo);
					nSequenceNo = nCase.SetEventPickAndPlacePickProcessDoneAfterPick;
				}
				else
				{
					m_cProductShareVariables->SetAlarm(12033);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis contains unit After Fail Picking At Output.\n", nHeadNo);
					nSequenceNo = nCase.IsUnitRemovedAfterFailPickForOutput;
				}
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				//m_cProductShareVariables->SetAlarm(11014);
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis fail pick unit during reject.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePositionToPick;
				if (smProductSetting->EnablePickupHeadRetryPickingNo == true)
				{
					//if (smProductSetting->PickupHeadRetryPickingNo != 0)
					{
						if (smProductProduction->nCurrentPickAndPlace2PickingRetry < smProductSetting->PickupHeadRetryPickingNo)
						{
							m_cProductShareVariables->SetAlarm(12014);
							smProductProduction->nCurrentPickAndPlace2PickingRetry++;
							nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePositionToPick;
						}
						else
						{
							m_cProductShareVariables->SetAlarm(12031);
							smProductEvent->RPNP_RSEQ_BYPASS_PICK_FAIL_REJECT.Set = true;
							if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
							{
								smProductProduction->nCurrrentTotalUnitDoneByLot--;
								smProductProduction->nCurrentTotalUnitDone--;
								smProductProduction->nCurrentLotGoodQuantity--;
								smProductProduction->OutputQuantity--;
							}
							smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
							//smProductEvent->RPNP1_RSEQ_Y_MOVE_STANDBY.Set = false;
							nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
						}
					}
				}
				else
				{
					m_cProductShareVariables->SetAlarm(12032);
					smProductEvent->RPNP_RSEQ_BYPASS_PICK_FAIL_REJECT.Set = true;
					if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
					{
						smProductProduction->nCurrrentTotalUnitDoneByLot--;
						smProductProduction->nCurrentTotalUnitDone--;
						smProductProduction->nCurrentLotGoodQuantity--;
						smProductProduction->OutputQuantity--;
					}
					smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
					//smProductEvent->RPNP1_RSEQ_Y_MOVE_STANDBY.Set = false;
					nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;

				}
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;
		}
		case nCase.IsUnitRemovedAfterFailPickForOutput:
		{
			double dblReturnValue;
			//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			if (dblReturnValue > smProductSetting->PickUpHead2Pressure)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
				smProductEvent->RPNP_RSEQ_BYPASS_PICK_FAIL_REJECT.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Unit Removed From PickAndPlace after Fail Picking At Output.\n", nHeadNo);
				nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
			}
			else
			{
				m_cProductShareVariables->SetAlarm(12033);
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
			}
			break;
		}
		case nCase.SetEventPickAndPlacePickProcessDoneAfterPick:
			//smProductProduction->PickAndPlacePickUpHeadStationResult[1] = smProductProduction->OutputTableResult[0];
			//smProductProduction->OutputTableResult[0] = sClearResult;
			//smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set = true;
			if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductProduction->nCurrrentTotalUnitDoneByLot--;
					smProductProduction->nCurrentTotalUnitDone--;
					smProductProduction->nCurrentLotGoodQuantity--;
					smProductProduction->OutputQuantity--;
				}
			}
			smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PICKED_DONE.Set = true;
			//smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			//smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = true;
			//smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PICKED_DONE.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Process Done.\n", nHeadNo);
			nSequenceNo = nCase.IsEventRejectTrayTableReady;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsEventRejectTrayTableReady:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true)
			{
				//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Reject Tray Table Ready.\n");
				nSequenceNo = nCase.IsTrayTableReadyToPlace;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnJobSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d:: Reject Tray Table Ready timeout.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsTrayTableReadyToPick:
			//check post vision done, if not done, need go back output station to pickup again.
			////if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true || smProductEvent->JobStop.Set == true || smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set == true)//input out of unit and tray
			////{
			////	//nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
			////	nSequenceNo = nCase.IsEventPickAndPlaceDisable;
			////	break;
			////}
			////else 
			if (smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set == true || smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true)//output quantity + head quantity full
			{
				//nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
				nSequenceNo = nCase.IsEventPickAndPlaceDisable;
				break;
			}
			else if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				smProductProduction->nCurrentPickAndPlace2PickingRetry = 0;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_PICK_START.Set = false;
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				nSequenceNo = nCase.IsPickAndPlacePickUnitAtUpPositionDone;
				//nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
			}
			else if (smProductEvent->RINT_RSEQ_INPUT_UNIT_PICK_START.Set == true && smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set == false)
			{
				if (lnPnPSequenceClockStartCycle.QuadPart != 0)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
					lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStartCycle.QuadPart;
					smProductProduction->nTime[19] = (int)(lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductProduction->nTime[19] = smProductProduction->nTime[0];
					m_cLogger->WriteLog("PickAndPlaceSeq%d: UPH cycle time=%lfms.\n", nHeadNo, (double)smProductProduction->nTime[0]);
					if (smProductSetting->EnablePH[0] == true)
					{
						//smProductProduction->nTime[0] = (smProductProduction->nTime[9] + smProductProduction->nTime[19]) / 4;
						int nTimeMoveToInputAndProcess1_1_2 = smProductProduction->nTime[1] + smProductProduction->nTime[2];
						int nTimeMoveToInputAndProcess2_11_12 = smProductProduction->nTime[11] + smProductProduction->nTime[12];
						int nTimeMoveToBottomAndProcess1_3_4 = smProductProduction->nTime[3] + smProductProduction->nTime[4];
						int nTimeMoveToBottomAndProcess2_13_14 = smProductProduction->nTime[13] + smProductProduction->nTime[14];
						int nTimeMoveToS3AndProcess1_5_6 = smProductProduction->nTime[5] + smProductProduction->nTime[6];
						int nTimeMoveToS3AndProcess2_15_16 = smProductProduction->nTime[15] + smProductProduction->nTime[16];
						int nTimeMoveToOutputAndProcess1_7_8 = smProductProduction->nTime[7] + smProductProduction->nTime[8];
						int nTimeMoveToOutputAndProcess2_17_18 = smProductProduction->nTime[17] + smProductProduction->nTime[18];

						int nTimeMoveToInputAndProcess1AndMoveToS3AndProcess2;
						int nTimeMoveToBottomAndProcess1AndMoveToOutputAndProcess2;
						int nTimeMoveToS3AndProcess1AndMoveToInputAndProcess2;
						int nTimeMoveToOutputAndProcess1MoveToBottomAndProcess2;

						if (nTimeMoveToInputAndProcess1_1_2 > nTimeMoveToS3AndProcess2_15_16)
							nTimeMoveToInputAndProcess1AndMoveToS3AndProcess2 = nTimeMoveToInputAndProcess1_1_2;
						else
							nTimeMoveToInputAndProcess1AndMoveToS3AndProcess2 = nTimeMoveToS3AndProcess2_15_16;

						if (nTimeMoveToBottomAndProcess1_3_4 > nTimeMoveToOutputAndProcess2_17_18)
							nTimeMoveToBottomAndProcess1AndMoveToOutputAndProcess2 = nTimeMoveToBottomAndProcess1_3_4;
						else
							nTimeMoveToBottomAndProcess1AndMoveToOutputAndProcess2 = nTimeMoveToOutputAndProcess2_17_18;

						if (nTimeMoveToS3AndProcess1_5_6 > nTimeMoveToInputAndProcess2_11_12)
							nTimeMoveToS3AndProcess1AndMoveToInputAndProcess2 = nTimeMoveToS3AndProcess1_5_6;
						else
							nTimeMoveToS3AndProcess1AndMoveToInputAndProcess2 = nTimeMoveToInputAndProcess2_11_12;

						if (nTimeMoveToOutputAndProcess1_7_8 > nTimeMoveToBottomAndProcess2_13_14)
							nTimeMoveToOutputAndProcess1MoveToBottomAndProcess2 = nTimeMoveToOutputAndProcess1_7_8;
						else
							nTimeMoveToOutputAndProcess1MoveToBottomAndProcess2 = nTimeMoveToBottomAndProcess2_13_14;

						smProductProduction->nTime[0] = (nTimeMoveToInputAndProcess1AndMoveToS3AndProcess2 + nTimeMoveToBottomAndProcess1AndMoveToOutputAndProcess2
							+ nTimeMoveToS3AndProcess1AndMoveToInputAndProcess2 + nTimeMoveToOutputAndProcess1MoveToBottomAndProcess2) / 2;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: UPH cycle time=%lfms.\n", nHeadNo, (double)smProductProduction->nTime[0]);
					}
					else
					{
						smProductProduction->nTime[0] = smProductProduction->nTime[19];
						m_cLogger->WriteLog("PickAndPlaceSeq%d: UPH cycle time=%lfms.\n", nHeadNo, (double)smProductProduction->nTime[0]);
					}
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStartCycle);
				smProductProduction->nCurrentPickAndPlace2PickingRetry = 0;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_PICK_START.Set = false;
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Tray Table Ready To Pick.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToDownPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);
			}
			break;

		case nCase.StartMovePickAndPlaceThetaAxisToDownPosition:
			//need add theta offset
			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition - smProductProduction->InputTableResult[0].InputThetaOffset_mDegree;// +smProductSetting->UnitPlacementRotationOffsetInput;
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;
			smProductProduction->dbPreviousOffset2X = 0;
			smProductProduction->dbPreviousOffset2Y = 0;

			smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um = 0;

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisInputPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisInputPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			//if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0])
			//{
			//	smProductEvent->PickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set = false;
			//	smProductEvent->StartPickAndPlace1ZAxisMotorMoveToInputTraySoftlandingPosition.Set = true;
			//}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To Down Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceThetaAxisToDownPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceThetaAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true
				&& smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true)
				//&& (smProductEvent->PickAndPlace1ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Down Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Down Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisToDownPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceThetaAxisToDownPosition:
			//smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set = false;
			//smProductEvent->StartPickAndPlace1ThetaAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Down Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceThetaAxisToDownPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceThetaAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				//&& (smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Down Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToDownPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Down Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisToDownPosition;
			}
			break;

		case nCase.StartMovePickAndPlaceZAxisToDownPosition:
			//need add theta offset
			//smProductProduction->PickAndPlace1ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace1ThetaAxisStandbyPosition + smProductSetting->UnitPlacementRotationOffsetInput + smProductProduction->InputTableResult[0].InputThetaOffset_mDegree;
			//if (smProductCustomize->EnablePickAndPlace1Module == true && smProductSetting->EnablePH[0])
			//{
			//	smProductEvent->PickAndPlace1ThetaAxisMotorMoveDone.Set = false;
			//	smProductEvent->StartPickAndPlace1ThetaAxisMotorMove.Set = true;
			//}
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPosition.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Theta Axis To Down Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToDownPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set == true)
				//&& (smProductEvent->PickAndPlace1ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlacePickUnitDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 3000)
			{
				//m_cProductShareVariables->SetAlarm(12027);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToDownPosition:
			//smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set = false;
			//smProductEvent->StartPickAndPlace1ThetaAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Theta Axis To Down Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToDownPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
				//&& (smProductEvent->PickAndPlace1ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisOffVacuumForPickFail;
				//nSequenceNo = nCase.IsPickAndPlacePickUnitDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPosition;
			}
			break;

		case nCase.StartPickAndPlaceZAxisOffVacuumForPickFail:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2ZAxisOffVacuumDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisOffVacuum.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis Off Vacuum due to input pick fail.\n", nHeadNo);
			nSequenceNo = nCase.IsPickAndPlaceZAxisOffVacuumForPickFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsPickAndPlaceZAxisOffVacuumForPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOffVacuumDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Vacuum due to input pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisValveOnForPickFail;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Vacuum due to input pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisOffVacuumForPickFail;
			}
			break;
		case nCase.StartPickAndPlaceZAxisValveOnForPickFail:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
			{
				smProductEvent->PickAndPlace2ZAxisOnValveDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisOnValve.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis On Valve due to input pick fail.\n", nHeadNo);
			nSequenceNo = nCase.IsPickAndPlaceZAxisValveOnForPickFailDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsPickAndPlaceZAxisValveOnForPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOnValveDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis On Valve due to input pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsDelayForPurgingReachedBeforeOffValveForPickFail;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis On Valve due to input pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPickAndPlaceZAxisValveOnForPickFail;
			}
			break;
		case nCase.IsDelayForPurgingReachedBeforeOffValveForPickFail:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= smProductSetting->DelayForPickupHeadPurgeAtDownPositionWhenPickFail)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Start PickAndPlace Z Axis Off Valve due to input pick fail %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				{
					smProductEvent->PickAndPlace2ZAxisOffValveDone.Set = false;
					smProductEvent->StartPickAndPlace2ZAxisOffValve.Set = true;
				}
				nSequenceNo = nCase.IsPickAndPlaceZAxisValveOffForPickFailDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;
		case nCase.IsPickAndPlaceZAxisValveOffForPickFailDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->PickAndPlace2ZAxisOffValveDone.Set == true && smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Valve due to input pick fail done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlacePickUnitDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Off Valve due to input pick fail timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] == true)
				{
					smProductEvent->PickAndPlace2ZAxisOffValveDone.Set = false;
					smProductEvent->StartPickAndPlace2ZAxisOffValve.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsPickAndPlacePickUnitDone:
			if (true)
			{
				slBottomPickAndPlaceXAxisMove = 0;
				slBottomPickAndPlaceYAxisMove = 0;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Unit Done.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition:
			//Set theta position
			//smProductSetting->SetupVisionUnitThetaOffset = 90000;
			//if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			//{
			//	smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
			//	smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			//}
			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition + (signed long)smProductSetting->UnitThickness_um;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Axis To Up And Theta To Orientation Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToUpAndThetaToOrientationPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceZAxisToUpAndThetaToOrientationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			lnPnPSequenceClockSpan4.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart4.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up And Theta To Orientation Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Picking Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlacePickUnitAtUpPositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up And Theta To Orientation Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition:
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up And Theta To Orientation Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToUpAndThetaToOrientationPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToUpAndThetaToOrientationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up And Theta To Orientation Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up And Theta To Orientation Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition;
			}
			break;

		case nCase.IsPickAndPlacePickUnitAtUpPositionDone:
			if (smProductEvent->PickAndPlace2ZAxisMotorMoveToInputTraySoftlandingPositionDone.Set == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				smProductProduction->nCurrentInputLotQuantityRun++;
				smProductProduction->nCurrentInputUnitOnTray++;
				smProductProduction->nCurrentTotalInputUnitDone++;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
				lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
				if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount < 100)
				{
					break;
				}
				double dblReturnValue;
				signed long lngEncoderPositionZ = 0;
				smProductProduction->PickAndPlacePickUpHeadStationResult[1] = smProductProduction->InputTableResult[0];
				GetInputStationResult(nHeadNo);
				ResetInputStationResult();
				m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
				m_cProductMotorControl->THKReadEncoderValue(1, 0, &lngEncoderPositionZ);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis At Up Position Done After Pick.\n", nHeadNo);
				if ((lngEncoderPositionZ > smProductProduction->PickAndPlace2ZAxisMovePosition + 5 || lngEncoderPositionZ < smProductProduction->PickAndPlace2ZAxisMovePosition - 5 )&& smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(11036);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Not At Up Position.\n", nHeadNo);
					nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpAndThetaToOrientationPosition;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;

				}
				if (dblReturnValue > smProductSetting->PickUpHead2Pressure && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
					//m_cProductShareVariables->SetAlarm(12018);

					if (smProductSetting->EnablePickupHeadRetryPickingNo == true)
					{
						//if (smProductSetting->PickupHeadRetryPickingNo != 0)
						{
							if (smProductProduction->nCurrentPickAndPlace2PickingRetry < smProductSetting->PickupHeadRetryPickingNo)
							{
								//m_cProductShareVariables->SetAlarm(12014);
								smProductProduction->nCurrentPickAndPlace2PickingRetry++;
								nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToDownPosition;
								break;
							}
							else
							{
								//m_cProductShareVariables->SetAlarm(12017);
								smProductProduction->nCurrentInputLotQuantityRun++;
								smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
								if (smProductSetting->EnableCountDownByInputQuantity == true)
								{
									smProductProduction->nCurrentLotNotGoodQuantity++;
								}
								smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
								smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE.Set = false;
								smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START.Set = true;
								nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
								break;
							}
						}
					}
					else
					{
						smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
						smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE.Set = false;
						smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace missing unit After Pick.\n", nHeadNo);
						if (smProductSetting->EnableCountDownByInputQuantity == true)
						{
							smProductProduction->nCurrentLotNotGoodQuantity++;
						}
					}
					
				}
				else
				{
					//smProductProduction->PickAndPlacePickUpHeadStationResult[1] = smProductProduction->InputTableResult[0];
					smProductProduction->UpdateMappingProgressHead = 1;
					smProductEvent->RTHD_GMAIN_UPDATE_IN_PROGRESS_MAPPING.Set = true;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Current Pressure after pick at input station %lf.\n", nHeadNo, dblReturnValue);
				nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis fail pick unit.\n", nHeadNo);
				if (smProductSetting->EnablePickupHeadRetryPickingNo == true)
				{
					//if (smProductSetting->PickupHeadRetryPickingNo != 0)
					{
						if (smProductProduction->nCurrentPickAndPlace2PickingRetry < smProductSetting->PickupHeadRetryPickingNo)
						{
							//m_cProductShareVariables->SetAlarm(12014);
							smProductProduction->nCurrentPickAndPlace2PickingRetry++;
							nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToDownPosition;
						}
						else
						{
							//m_cProductShareVariables->SetAlarm(12017);
							smProductProduction->nCurrentInputLotQuantityRun++;
							smProductProduction->PickAndPlacePickUpHeadStationResult[1] = smProductProduction->InputTableResult[0];
							smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
							
							if (smProductSetting->EnableCountDownByInputQuantity == true)
							{
								smProductProduction->nCurrentLotNotGoodQuantity++;
							}
							smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
							smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE.Set = false;
							smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START.Set = true;
							nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
						}
					}
				}
				else
				{
					//m_cProductShareVariables->SetAlarm(12026);
					smProductProduction->nCurrentInputLotQuantityRun++;
					smProductProduction->PickAndPlacePickUpHeadStationResult[1] = smProductProduction->InputTableResult[0];
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
					if (smProductSetting->EnableCountDownByInputQuantity == true)
					{
						smProductProduction->nCurrentLotNotGoodQuantity++;
					}
					smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
					smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE.Set = false;
					smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START.Set = true;
					nSequenceNo = nCase.SetEventPickAndPlacePickProcessDone;
				}
			}
			break;	
		case nCase.SetEventPickAndPlacePickProcessDone:
			if (smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set == false && smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == false && smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set == false)//output quantity + head quantity full
			{
				smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set = true;
			}
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_INPUT_STATION_DONE.Set = true;
			smProductEvent->RPNP2_ROUT_THREAD_AWAY_INPUT.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Process Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToBottomAndS1Position;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToBottomAndS1Position:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_BOTTOM_STATION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_MOVE_TO_BOTTOM_STATION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Bottom Position.\n", nHeadNo);
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo != smProductProduction->nCurrentInputTrayNumberAtBottom)
				{
					smProductProduction->nCurrentInputTrayNumberAtBottom = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo;
					nSequenceNo = nCase.SetBottomEndTrayAfterProgress;
				}
				else
				{
					nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToBottomAndS1Position;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			//{
			//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
			//	nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			//}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToBottomAndS1Position:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Bottom Position.\n", nHeadNo);
				nSequenceNo = nCase.IsBottomS1VisionReady;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Bottom Position.\n", nHeadNo);
			}
			break;

		case nCase.IsBottomS1VisionReady:
			smProductProduction->nCurrentPickupHeadAtS1 = 1;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{

				if ((m_cProductIOControl->IsBottomVisionReadyOn() == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6901);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision not ready.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsBottomVisionEndOfVision() == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6902);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision busy.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsS1VisionReadyOn() == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6301);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision not ready.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsS1VisionEndOfVision() == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6302);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision busy.\n", nHeadNo);
					break;
				}
			}

			if (smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START.Set = true;
				smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START.Set = false;
				smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationBottom = 1;
			nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToBottomPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.StartMovePickAndPlaceXYAxisToBottomPosition:
		{
			if(m_bHighSpeed)
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 700000, 500, 10000000, 10000000);
			else
			{	//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 500, 250000, 250000);
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 500000, 500, 8500000, 8500000);
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000);  //700000--> 350000 27Nov2024 For Testing Puspose //back to 700000
			}
			double dCalculatedXPos = 0;
			double dCalculatedYPos = 0;
			
			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition;
				//+ signed long(dCalculatedXPos + (smProductSetting->PickUpHeadRotationXOffset[1]));

			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition;
				//+ signed long(dCalculatedYPos + (smProductSetting->PickUpHeadRotationYOffset[1]));

			slBottomPickAndPlaceXAxisMove = smProductProduction->PickAndPlace2XAxisMovePosition;
			slBottomPickAndPlaceYAxisMove = smProductProduction->PickAndPlace2YAxisMovePosition;
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition1 + +(signed long)smProductSetting->UnitThickness_um;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor 
				&& smProductCustomize->EnablePickAndPlace2YAxisMotor 
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
				smProductEvent->S1VisionModuleMotorMoveFocusPositionDone.Set = false;
				smProductEvent->StartS1VisionModuleMotorMoveFocusPosition.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace XY Axis To Bottom Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			nSequenceNo = nCase.IsMovePickAndPlaceXYZThetaAxisToBottomPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		}
		break;

		case nCase.IsMovePickAndPlaceXYZThetaAxisToBottomPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor 
					&& smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				&& (smProductEvent->S1VisionModuleMotorMoveFocusPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) < smProductProduction->PickAndPlace2XAxisMovePosition - 200
					|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) > smProductProduction->PickAndPlace2XAxisMovePosition + 200)
				{
					m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition when bottom not going position %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
					m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition when bottom not going position %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
					nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToBottomPosition;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 100, 2500000, 2500000);
				smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = true;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.BottomStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_BOTTOM_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Bottom Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				{
					double dblReturnValue;
					m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
					if (dblReturnValue <= smProductSetting->PickUpHead2Pressure)
					{
						if (smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
						{
							smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains unit but unitpresent = 0, set event pick_fail.\n", nHeadNo);
						}
						else
						{
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains unit, unitpresent = 0 and event pick_fail == true.\n", nHeadNo);
						}
					}
					else
					{
						if (smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true)
						{
							smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = false;
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace pick fail unit but no units, reset pick_fail event.\n", nHeadNo);
						}
						else
						{
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains no unit, unitpresent = 0 and pick_fail event == false.\n", nHeadNo);
						}
					}
				}
				else if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					double dblReturnValue;
					m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Current Pressure at bottom station %lf.\n", nHeadNo, dblReturnValue);
					if (dblReturnValue > smProductSetting->PickUpHead2Pressure)
					{
						smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
						m_cProductShareVariables->SetAlarm(12018);
						smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_S1_MISSING_UNIT.Set = true;
					}
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Bottom Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Bottom Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
				m_cLogger->WriteLog("PickAndPlace2YAxisCurrentEncoderPosition %lf.\n", (double)(m_cProductMotorControl->ReadPickAndPlace1YAxisMotorEncoder()));
				nSequenceNo = nCase.IsBottomVisionReceiveRCDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 100, 2500000, 2500000);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Bottom Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToBottomPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXYAxisToBottomPosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Theta Axis To Bottom Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToBottomPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYAxisToBottomPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Theta Axis To Bottom Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToBottomPosition;//is vision offset position safe to directly move to input station
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis Theta To Bottom Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToBottomPosition;
			}
			break;

		case nCase.IsBottomVisionReceiveRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
				{
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StartBottomVisionSOV;
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsBottomVisionReceiveRCDone;
						nSequenceNo = nCase.DelayAfterBottomReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						nSequenceNo = nCase.IsBottomVisionReadyForRetry;
						m_cProductShareVariables->SetAlarm(6907);
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision get RC timeout.\n", nHeadNo);
					}					
				}
			}
			else
			{
				nSequenceNo = nCase.StartBottomVisionSOV;
			}
			break;

		case nCase.IsBottomVisionReadyForRetry:
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((m_cProductIOControl->IsS1VisionReadyOn() == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6901);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision not ready.\n", nHeadNo);
					break;
				}
				if ((m_cProductIOControl->IsS1VisionEndOfVision() == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6902);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision busy.\n", nHeadNo);
					break;
				}
			}

			if (smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
			}

			nSequenceNo = nCase.IsBottomVisionReceiveRCDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.StartBottomVisionSOV:
			smProductProduction->CurrentBottomVisionLoopNo = -1;
			if (smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1)
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				//smProductProduction->IsS1VisionFistSnap = true;
				smProductEvent->RBTMV_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RBTMV_START_VISION.Set = true;
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RBTMV_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RBTMV_START_VISION.Set = false;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Bottom Vision SOV.\n", nHeadNo);
			nSequenceNo = nCase.IsBottomVisionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsBottomVisionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductEvent->RBTMV_RSEQ_VISION_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableBottomVisionModule == false || smProductSetting->EnableBottomVision == false))
			{
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				smProductProduction->CurrentBottomVisionLoopNo = 0;
				//nSequenceNo = nCase.StartMoveAllSidewallAndS3ToFocusPosition;
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um = 0;
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um = 0;
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree = 0;
				}
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um > 500 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um < -500)
				{
					m_cProductShareVariables->SetAlarm(6911);
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um = 0;
					smProductProduction->CurrentBottomVisionLoopNo = -1;
					nSequenceNo = nCase.StartBottomVisionSOV;
				}
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um > 500 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um < -500)
				{
					m_cProductShareVariables->SetAlarm(6912);
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um = 0;
					smProductProduction->CurrentBottomVisionLoopNo = -1;
					nSequenceNo = nCase.StartBottomVisionSOV;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Bottom Vision Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nSequenceNo = nCase.IsRequireAdditionalMoveAndSnap;

				smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
				smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;

				nSequenceNo = nCase.StartMoveOffsetByBottomResult;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Bottom Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RBTMV_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RBTMV_START_VISION_RETEST.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				//nSequenceNo = nCase.StartBottomVisionSOV;
			}
			break;

		case nCase.StartMoveOffsetByBottomResult:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;
			smProductProduction->dbPreviousOffset2X = 0;
			smProductProduction->dbPreviousOffset2Y = 0;
			smProductProduction->dbCurrentOffset2Theta = 0;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO SETUP ANGLE  %lf\n", nHeadNo, (double)smProductSetting->SetupVisionUnitThetaOffset);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision X Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Y Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Theta Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition
				- (signed long)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree + (signed long)(smProductSetting->SetupVisionUnitThetaOffset);

			
			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION X POS %d.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION Y POS %d.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationBottom = 2;

			if (smProductCustomize->EnablePickAndPlace1Module == true
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To Bottom Offset Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			nSequenceNo = nCase.IsMoveOffsetByBottomResultDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMoveOffsetByBottomResultDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor 
					&& smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace1Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Bottom Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Bottom Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
				smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;

				nSequenceNo = nCase.IsSendSetupRCDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;
				//nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 50000, 100, 2500000, 2500000);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Axis To Bottom Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveOffsetByBottomResult;
			}
			break;

		case nCase.StopMoveOffsetByBottomResult:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Bottom When Offset Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveOffsetByBottomResultDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMoveOffsetByBottomResultDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor 
					&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace1YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Bottom Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOffsetByBottomResult;//is vision offset position safe to directly move to input station
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Axis Theta To Bottom Offset Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveOffsetByBottomResult;
			}
			break;

		case nCase.StartMoveOffsetBySetupOffset:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;
			smProductProduction->dbPreviousOffset2X = 0;
			smProductProduction->dbPreviousOffset2Y = 0;
			smProductProduction->dbCurrentOffset2Theta = 0;
			
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO SETUP ANGLE  %lf\n", nHeadNo, (double)smProductSetting->SetupVisionUnitThetaOffset);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision X Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Y Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Theta Offset %lf.\n", nHeadNo, smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2ThetaAxisMovePosition += (signed long)(smProductSetting->SetupVisionUnitThetaOffset);

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION X POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION Y POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			if (smProductCustomize->EnablePickAndPlace2Module == true
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To Setup Offset Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace1XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace1YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace1ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			nSequenceNo = nCase.IsMoveOffsetBySetupOffsetDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMoveOffsetBySetupOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Setup Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To Setup Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				nSequenceNo = nCase.StartSendSetupRC;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				//nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 50000, 100, 2500000, 2500000);

				smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Axis To Setup Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveOffsetBySetupOffset;
			}

			break;

		case nCase.StopMoveOffsetBySetupOffset:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Setup When Offset Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveOffsetBySetupOffsetDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMoveOffsetBySetupOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Setup Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOffsetBySetupOffset;//is vision offset position safe to directly move to input station
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Axis Theta To Setup Offset Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveOffsetBySetupOffset;
			}
			break;

		case nCase.StartSendSetupRC:
			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationBottom = 2;
			nSequenceNo = nCase.IsSendSetupRCDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);//setup inspection
			break;

		case nCase.IsSendSetupRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
				{
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Setup Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
					nSequenceNo = nCase.DelayBeforeSideWall1InspectionGetUnitHigh;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsSendSetupRCDone;
						nSequenceNo = nCase.DelayAfterSetupReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						m_cProductShareVariables->SetAlarm(6307);
						if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = true;

							smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = false;

							smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set = true;
						}
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Setup Vision get RC timeout.\n", nHeadNo);
					}					
				}
			}
			/*else
			{
				nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
			}*/
			break;

		case nCase.DelayBeforeSideWall1InspectionGetUnitHigh:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS1VisionSnap_ms)
			{
				nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartSideWall1InspectionGetUnitHigh:
			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true /*&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1*/)
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				/*smProductProduction->IsS1VisionFistSnap = true;
				smProductEvent->RS1V_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RS1V_START_VISION.Set = true;*/
				m_cProductIOControl->SetS1VisionSOV(true);
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RS1V_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RS1V_START_VISION.Set = false;
				m_cProductIOControl->SetS1VisionSOV(true); //Temp
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S1 Setup Vision SOV.\n", nHeadNo);
			nSequenceNo = nCase.IsSideWall1InspectionGetUnitHighDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsSideWall1InspectionGetUnitHighDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			lnPnPSequenceClockSpan4.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart4.QuadPart;
			if ((m_cProductIOControl->IsS1VisionEndOfVision() == true && smProductEvent->RMAIN_RSTPV_GET_VISION_RESULT_DONE.Set == true
				&& smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
			{
				smProductProduction->CurrentS1VisionRetryCount = 0;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				smProductProduction->CurrentBottomVisionLoopNo = 0;
				//nSequenceNo = nCase.StartMoveAllSidewallAndS3ToFocusPosition;
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].S1ZOffset_um = 0;
					smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree = 0;

				}
				//if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupZOffset_um > 500 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupZOffset_um < -500)
				//{
				//	m_cProductShareVariables->SetAlarm(6315);
				//	smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupZOffset_um = 0;
				//	//smProductProduction->CurrentBottomVisionLoopNo = -1;
				//	nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
				//}
				//if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree > 1500 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree < -1500)
				//{
				//	m_cProductShareVariables->SetAlarm(6315);
				//	smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree = 0;
				//	//smProductProduction->CurrentBottomVisionLoopNo = -1;
				//	nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
				//}
				m_cProductIOControl->SetS1VisionSOV(false);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Setup Vision Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S1 Setup Vision Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nSequenceNo = nCase.IsRequireAdditionalMoveAndSnap;
				smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
				smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;

				nSequenceNo = nCase.CheckSetupResultReturn;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Setup Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cProductIOControl->SetS1VisionSOV(false);
				smProductEvent->RS1V_GMAIN_S1_VISION_RESET_EOV.Set = true;
				if (smProductProduction->CurrentS1VisionRetryCount < 3)
				{
					smProductProduction->CurrentS1VisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentS1VisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6305);
				}


				//nSequenceNo = nCase.StartSideWall1InspectionGetUnitHigh;
				nSequenceNo = nCase.StartSendSetupRC;
			}
			break;

		case nCase.CheckSetupResultReturn:
			if (smProductProduction->nCurrentSetupFailValue >= smProductSetting->SetupVisionContinuousFailCountToTriggerAlarm)
			{
				smProductProduction->nCurrentSetupFailValue = 0;
				m_cProductShareVariables->SetAlarm(7019);
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisForS1Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisForS1Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartMovePickAndPlaceThetaAxisForS1Position:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO SETUP OFFSET ANGLE %lf + S1 THETA POS  %lf\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree, (double)smProductSetting->S1VisionUnitThetaOffset);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision X Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Y Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Theta Offset %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductProduction->PickAndPlace2ThetaAxisMovePosition - (signed long)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupThetaOffset_mDegree + (signed long)smProductSetting->S1VisionUnitThetaOffset;

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION X POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION Y POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To S1 Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceThetaAxisForS1PositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceThetaAxisForS1PositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S1 Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S1 Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				nSequenceNo = nCase.IsS1AdditionalRequiredMoveAndSnap;
				smProductProduction->CurrentS1VisionLoopNo = 0;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{

				smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S1 Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisForS1Position;
			}
			break;

		case nCase.StopMovePickAndPlaceThetaAxisForS1Position:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To Down Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceThetaAxisForS1Position;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceThetaAxisForS1Position:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisForS1Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisForS1Position;
			}
			break;

		case nCase.StartSendS1RC:
			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationBottom = 3;
			nSequenceNo = nCase.IsSendS1RCDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);
			break;

		//case nCase.IsSendS1RCDone:
		//	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
		//	lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
		//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
		//	{
		//		if ((smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
		//			|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
		//		{
		//			//smProductProduction->CurrentS1VisionLoopNo = 0;
		//			m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		//			m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S1 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		//			smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
		//			smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
		//			nSequenceNo = nCase.StartMoveS1VisionThetaAxisToAdditionalPosition;
		//		}
		//		else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//		{
		//			nSequenceNo = nCase.StartSendS1RC;
		//			m_cProductShareVariables->SetAlarm(6307);
		//			m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get RC timeout.\n", nHeadNo);
		//		}
		//	}
		//	else
		//	{
		//		if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
		//		{
		//			m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get empty unit.\n", nHeadNo);
		//			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		//			nSequenceNo = nCase.SetEventPickAndPlaceProcessAtBottomPositionDone;
		//		}
		//		//nSequenceNo = nCase.IsS1AdditionalRequiredMoveAndSnap;
		//	}
		//	break;

		case nCase.DelayAfterS1AdditionalGrabDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterS1VisionGrabDone_ms)
			{
				nSequenceNo = nCase.IsS1AdditionalRequiredMoveAndSnap;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsS1AdditionalRequiredMoveAndSnap:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			bIsS1VisionRequireAdditionalMoveAndSnap = false;
			for (int i = smProductProduction->CurrentS1VisionLoopNo; i < 10; i++)
			{
				if (smProductSetting->S1Vision[smProductProduction->CurrentS1VisionLoopNo].Enable == true)
				{
					//smProductProduction->nSidewallRightVisionAdditionalSnapNo = i + 1;
					bIsS1VisionRequireAdditionalMoveAndSnap = true;
					smProductProduction->CurrentS1VisionLoopNo = i;
					break;
				}
			}
			if (bIsS1VisionRequireAdditionalMoveAndSnap == false)//complete Input Vision
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap Done.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				//if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].bolLastUnitInTray == true || smProductProduction->bolIsLastUnitTo2EndTray == true)
				//{
				//	nSequenceNo = nCase.SetBottomEndTrayAfterProgress;
				//}
				//else
				//{
				smProductEvent->S1VisionModuleMotorMoveRetractPositionDone.Set = false;
				smProductEvent->StartS1VisionModuleMotorMoveRetractPosition.Set = true;
				nSequenceNo = nCase.IsS1ModuleMoveToRetractPosition;
				//nSequenceNo = nCase.SetEventPickAndPlaceProcessAtBottomPositionDone;
			}
			else
			{

				m_cLogger->WriteLog("InputTrayTableSeq: S1 Vision Additional Move And Snap.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				nSequenceNo = nCase.StartMoveS1VisionThetaAxisToAdditionalPosition;

			}
			break;

		case nCase.IsS1ModuleMoveToRetractPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->S1VisionModuleMotorMoveRetractPositionDone.Set == true && m_cProductIOControl->IsS1RetractSensor() == true)
			{
				nSequenceNo = nCase.SetEventPickAndPlaceProcessAtBottomPositionDone;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				smProductEvent->S1VisionModuleMotorMoveRetractPositionDone.Set = false;
				smProductEvent->StartS1VisionModuleMotorMoveRetractPosition.Set = true;
				nSequenceNo = nCase.IsS1ModuleMoveToRetractPosition;
			}
			break;

		case nCase.StartMoveS1VisionThetaAxisToAdditionalPosition:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO SW1_%d ANGLE  %lf\n", nHeadNo, (int)smProductProduction->CurrentS1VisionLoopNo + 1, (double)smProductSetting->S1Vision[smProductProduction->CurrentS1VisionLoopNo].ThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision X Offset %d.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Y Offset %d.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)Bottom Vision Theta Offset %d.\n", nHeadNo, (double)smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2ThetaAxisMovePosition += (signed long)smProductSetting->S1Vision[smProductProduction->CurrentS1VisionLoopNo].ThetaOffset_mDegree;

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, false, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION X POS %d.\n", nHeadNo, (int)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT BOTTOM VISION Y POS %d.\n", nHeadNo, (int)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %d\n", nHeadNo, (int)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %d\n", nHeadNo, (int)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %d.\n", nHeadNo, (int)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %d.\n", nHeadNo, (int)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %d.\n", nHeadNo, (int)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductProduction->nS1VisionAdditionalSnapNo++;
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationBottom = 3;

			if (smProductCustomize->EnablePickAndPlace2Module == true
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To S1 Offset Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace1ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			nSequenceNo = nCase.IsMoveS1VisionThetaAxisToAdditionalPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMoveS1VisionThetaAxisToAdditionalPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S1 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S1 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);				
				smProductProduction->S1VisionModuleMoveThetaCurrentPosition = smProductProduction->PickAndPlace2ThetaAxisMovePosition;
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				smProductProduction->dbPreviousOffset2Y = smProductProduction->dbCurrentOffset2Y;
				nSequenceNo = nCase.IsSendS1RCDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;

				//nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 50000, 100, 2500000, 2500000);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Axis To S1 Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveS1VisionThetaAxisToAdditionalPosition;
			}
			break;

		case nCase.StopMoveS1VisionThetaAxisToAdditionalPosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 When Offset Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMoveS1VisionThetaAxisToAdditionalPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMoveS1VisionThetaAxisToAdditionalPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveS1VisionThetaAxisToAdditionalPosition;//is vision offset position safe to directly move to input station
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Axis Theta To S1 Offset Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMoveS1VisionThetaAxisToAdditionalPosition;
			}
			break;

		case nCase.StartS1SnapPositionNumber:
			smProductEvent->GMAIN_RTHD_SW1_SEND_SNAP_POS_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_SW1_SEND_SNAP_POS.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Send SW1 snap number %d.\n", nHeadNo, smProductProduction->CurrentS1VisionLoopNo);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			nSequenceNo = nCase.IsS1SnapPositionNumberDone;
			break;

		case nCase.IsS1SnapPositionNumberDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->GMAIN_RTHD_SW1_SEND_SNAP_POS_DONE.Set == true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Send SW1 snap number Done %d.\n", nHeadNo, smProductProduction->CurrentS1VisionLoopNo);
				//nSequenceNo = nCase.StartS1AdditionalInspectSOV;
				nSequenceNo = nCase.DelayBeforeS1AdditionalInspectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Send SW1 snap number Fail %d.\n", nHeadNo, smProductProduction->CurrentS1VisionLoopNo);
				nSequenceNo = nCase.StartS1SnapPositionNumber;
			}
			break;

		case nCase.IsSendS1RCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
				{
					//smProductProduction->CurrentS1VisionLoopNo = 0;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S1 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
					smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
					//nSequenceNo = nCase.StartS1AdditionalInspectSOV;
					nSequenceNo = nCase.DelayBeforeS1AdditionalInspectSOV;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsSendS1RCDone;
						nSequenceNo = nCase.DelayAfterSW1ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						//nSequenceNo = nCase.StartSendS1RC;
						m_cProductShareVariables->SetAlarm(6307);
						if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductProduction->nS1VisionAdditionalSnapNo++;
							smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = true;

							smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = false;

							smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = true;
						}
						smProductProduction->nCurrentInspectionStationBottom = 3;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get RC timeout.\n", nHeadNo);
					}
					
				}
			}
			//else
			//{
			//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
			//	{
			//		m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 Vision get empty unit.\n", nHeadNo);
			//		RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			//		nSequenceNo = nCase.SetEventPickAndPlaceProcessAtBottomPositionDone;
			//	}
			//	//nSequenceNo = nCase.IsS1AdditionalRequiredMoveAndSnap;
			//}
			break;

		case nCase.DelayBeforeS1AdditionalInspectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS1VisionSnap_ms)
			{
				nSequenceNo = nCase.StartS1AdditionalInspectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartS1AdditionalInspectSOV:
			if (smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true && smProductSetting->EnableVision == true 
				&& (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0))
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				/*smProductProduction->IsS1VisionFistSnap = false;
				smProductEvent->RS1V_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RS1V_START_VISION.Set = true;*/
				m_cProductIOControl->SetS1VisionSOV(true);
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RS1V_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RS1V_START_VISION.Set = false;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S1 FAT1 And Glu Vision SOV.\n", nHeadNo);
			nSequenceNo = nCase.CheckS1GrabdoneAfterSOV;
			//nSequenceNo = nCase.IsS1AdditionalInspectSOVDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.CheckS1GrabdoneAfterSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS1VisionGrabDone() == false || lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 500)
			{
				nSequenceNo = nCase.IsS1AdditionalInspectSOVDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 FAT1 And Glu Vision started (Grabdone false).\n", nHeadNo);
			}
			break;

		case nCase.IsS1AdditionalInspectSOVDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((m_cProductIOControl->IsS1VisionGrabDone() && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true
				&& smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS1VisionModule == false || smProductSetting->EnableS1Vision == false))
			{
				smProductProduction->CurrentS1VisionRetryCount = 0;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S1 inspection done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cProductIOControl->SetS1VisionSOV(false);
				smProductProduction->CurrentS1VisionLoopNo++;
				//nSequenceNo = nCase.IsS1AdditionalRequiredMoveAndSnap;
				nSequenceNo = nCase.DelayAfterS1AdditionalGrabDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S1 FA1 adn Glue Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = true;
				smProductEvent->GMAIN_RS1V_GET_VISION_RESULT_DONE.Set = false;
				m_cProductIOControl->SetS1VisionSOV(false);
				smProductEvent->RS1V_GMAIN_S1_VISION_RESET_EOV.Set = true;
				if (smProductProduction->CurrentS1VisionRetryCount < 3)
				{
					smProductProduction->CurrentS1VisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentS1VisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6305);
				}
				//nSequenceNo = nCase.StartS1AdditionalInspectSOV;
				nSequenceNo = nCase.IsSendS1RCDone;
			}
			break;

		case nCase.SetBottomEndTrayAfterProgress:
			nBottomEndTrayRetryNo = 0;
			smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_BTM_SEND_ENDTRAY.Set = true;
			smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_S1_SEND_ENDTRAY.Set = true;
			nSequenceNo = nCase.IsBottomEndTrayAfterProgressDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsBottomEndTrayAfterProgressDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& (smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set == true && smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true))
				|| smProductSetting->EnableVision == false
				)
			{
				nSequenceNo = nCase.SetBottomNewTrayAfterProgress;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (nBottomEndTrayRetryNo <= 3)
				{
					if (smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set == false)
					{
						nBottomEndTrayRetryNo++;
						smProductEvent->GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_BTM_SEND_ENDTRAY.Set = true;
						nSequenceNo = nCase.IsBottomEndTrayAfterProgressDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						break;
					}
					if (smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set == false)
					{
						nBottomEndTrayRetryNo++;
						smProductEvent->GMAIN_RTHD_S1_SEND_ENDTRAY_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_S1_SEND_ENDTRAY.Set = true;
						nSequenceNo = nCase.IsBottomEndTrayAfterProgressDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						break;
					}
				}
				else
				{
					m_cProductShareVariables->SetAlarm(6938);
					nSequenceNo = nCase.SetBottomEndTrayAfterProgress;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
			}
			break;

		case nCase.SetBottomNewTrayAfterProgress:
			nBottomNewTrayRetryNo = 0;
			smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_BTM_SEND_TRAYNO.Set = true;
			smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_S1_SEND_TRAYNO.Set = true;
			nSequenceNo = nCase.IsBottomNewTrayAfterProgressDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsBottomNewTrayAfterProgressDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& (smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set == true && smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true))
				|| smProductSetting->EnableVision == false
				)
			{
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToBottomAndS1Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (nBottomNewTrayRetryNo <= 3)
				{
					if (smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set == false)
					{
						nBottomNewTrayRetryNo++;
						smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_BTM_SEND_TRAYNO.Set = true;
						nSequenceNo = nCase.IsBottomNewTrayAfterProgressDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						break;
					}
					if (smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set == false)
					{
						nBottomNewTrayRetryNo++;
						smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_S1_SEND_TRAYNO.Set = true;
						nSequenceNo = nCase.IsBottomNewTrayAfterProgressDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						break;
					}
				}
				else
				{
					m_cProductShareVariables->SetAlarm(6935);
					nSequenceNo = nCase.SetBottomNewTrayAfterProgress;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
			}
			break;

		case nCase.SetEventPickAndPlaceProcessAtBottomPositionDone:
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_BOTTOM_STATION_DONE.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Process At Bottom Position Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToS2S3Position;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToS2S3Position:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_S3_STATION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_MOVE_TO_S3_STATION_START.Set = false;
				smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To S3 Position.\n", nHeadNo);
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo != smProductProduction->nCurrentInputTrayNumberAtS2S3)
				{
					smProductProduction->nCurrentInputTrayNumberAtS2S3 = smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputTrayNo;
					nSequenceNo = nCase.SetS2S3EndTrayAfterProgress;
				}
				else
				{
					nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToS2S3Position;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			//{
			//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
			//	nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			//}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToS2S3Position:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set == true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To S3 Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
				smProductProduction->nCurrentPickupHeadAtS3 = 1; 
				smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
				smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
				smProductProduction->PickAndPlace2ThetaAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2ThetaAxisMovePosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module RESET S2 S3 Vision Result Not Done.\n", nHeadNo);
			}
			break;

		case nCase.IsS2S3VisionReady:
			
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((m_cProductIOControl->IsS3VisionReadyOn() == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6701);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision not ready.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsS3VisionEndOfVision() == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6702);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision busy.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsS2VisionReadyOn() == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6201);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision not ready.\n", nHeadNo);
					break;
				}

				if ((m_cProductIOControl->IsS2VisionEndOfVision() == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
				{

				}
				else
				{
					m_cProductShareVariables->SetAlarm(6202);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision busy.\n", nHeadNo);
					break;
				}
			}

			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
			}

			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
			}
			bS3VisionSendRowColumnDone = true;
			smProductProduction->nCurrentInspectionStationS2S3 = 1;
			nSequenceNo = nCase.IsS2S3VisionRCDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		

		case nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;
			smProductProduction->dbPreviousOffset2X = 0;
			smProductProduction->dbPreviousOffset2Y = 0;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO S2S3 FACET ANGLE  %lf\n", nHeadNo, (double)smProductSetting->S2FacetVisionUnitThetaOffset);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT X SLEEVE OFFSET %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SelveXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT Y SLEEVE OFFSET %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SelveYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);
			
			smProductProduction->PickAndPlace2ThetaAxisMovePosition += (signed long)smProductSetting->S2FacetVisionUnitThetaOffset;

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, true, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS2AndS3VisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS2AndS3VisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT S2 S3 VISION X POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT S2 S3 VISION Y POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition2
				+ smProductSetting->UnitThickness_um;
			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
			}

			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
			}
			bS3VisionSendRowColumnDone = true;
			smProductProduction->nCurrentInspectionStationS2S3 = 1;
			if (smProductCustomize->EnablePickAndPlace2Module == true
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To S2 S3 Offset Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			nSequenceNo = nCase.IsMovePickAndPlaceXYThetaAxisToS2S3PositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceXYThetaAxisToS2S3PositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor 
					&& smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)				
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) < smProductProduction->PickAndPlace2XAxisMovePosition - 200
					|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) > smProductProduction->PickAndPlace2XAxisMovePosition + 200)
				{
					m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition when S2S3 not going position %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
					m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition when S2S3 not going position %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
					nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
				//smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set = true;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.S3Station;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				smProductProduction->dbPreviousOffset2Y = smProductProduction->dbCurrentOffset2Y;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_S3_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYTheta Axis To S3 Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("PickAndPlace2XAxisLockPosition %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorLockPosition1(1, 0, 0)));
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				{
					double dblReturnValue;
					m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
					if (dblReturnValue <= smProductSetting->PickUpHead1Pressure)
					{
						if (smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == false)
						{
							smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = true;
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains unit but unitpresent = 0, set event pick_fail.\n", nHeadNo);
						}
						else
						{
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains unit, unitpresent = 0 and event pick_fail == true.\n", nHeadNo);
						}
					}
					else
					{
						if (smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true)
						{
							smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = false;
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace pick fail unit but no units, reset pick_fail event.\n", nHeadNo);
						}
						else
						{
							m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Contains no unit, unitpresent = 0 and pick_fail event == false.\n", nHeadNo);
						}
					}
				}
				else if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					double dblReturnValue;
					m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Current Pressure at S2 S3 station %lf.\n", nHeadNo, dblReturnValue);
					if (dblReturnValue > -40)
					{
						smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
						m_cProductShareVariables->SetAlarm(11018);
						smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_S1_MISSING_UNIT.Set = true;
					}
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S2 S3 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S2 S3 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//m_cProductIOControl->SetSideWall2_3VisionLighitngExtendCylinder(true);
				nSequenceNo = nCase.IsS2S3VisionRCDone;
				//nSequenceNo = nCase.StartSendS2FirstSnap;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYTheta Axis To S3 Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYThetaAxisToS2S3Position;
			}
			break;

		case nCase.IsS2S3VisionRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0
				|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true
					&& smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false
						|| smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
				{
					smProductProduction->CurrentS2VisionRCRetryCount = 0;
					smProductProduction->CurrentS3VisionRCRetryCount = 0;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StartSendS2FirstSnap;
					nSequenceNo = nCase.DelayBeforeSendS2FirstSnap;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					//nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsS2S3VisionRCDone;
						nSequenceNo = nCase.DelayAfterSW2_PARTING_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else if (smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsS2S3VisionRCDone;
						nSequenceNo = nCase.DelayAfterSW3_PARTING_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						//nSequenceNo = nCase.IsS2S3VisionReady;
					//m_cProductShareVariables->SetAlarm(6207);
						if (smProductProduction->CurrentS3VisionRCRetryCount < 3)
						{
							smProductProduction->CurrentS3VisionRCRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS3VisionRCRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6207);
						}
						if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = false;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
						}

						if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

							smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = false;

							smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
						}
						m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC timeout.\n", nHeadNo);
						nSequenceNo = nCase.IsS2S3VisionRCDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}					
				}
			}
			/*else
			{
				nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
			}*/
			break;

		case nCase.StopMovePickAndPlaceXYThetaAxisToS2S3Position:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYTheta Axis To S3 Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYThetaAxisToS2S3PositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYThetaAxisToS2S3PositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				&& smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Pick Position For Retry Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Pick Position For Retry timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYThetaAxisToS2S3Position;
			}
			break;

		case nCase.DelayBeforeSendS2FirstSnap:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms)
			{
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
				smProductEvent->RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE.Set = false;

				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
				smProductEvent->GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE.Set = false;
				nSequenceNo = nCase.StartSendS2FirstSnap;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartSendS2FirstSnap:
			//if (m_cProductIOControl->IsSW2N3VisionLightingUpSensor() == true)
			//{
				if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true
					&& smProductSetting->EnableVision == true /*&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1*/)
				{

					if (smProductProduction->bPNP1AllowS2S3Snap == true || smProductEvent->GMAIN_RTHD_ENDLOT.Set == true || smProductSetting->EnablePH[0] == false)
					{
						smProductProduction->bPNP1AllowS2S3Snap = false;
						//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
						/*smProductProduction->IsS2VisionFistSnap = true;
						smProductEvent->RS2V_RSEQ_VISION_DONE.Set = false;
						smProductEvent->RSEQ_RS2V_START_VISION.Set = true;*/

						if (smProductSetting->EnableS2S3BothSnapping == true)
						{
							//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms;
							//RtSleepFt(&lnDelayIn100ns);
							m_cProductIOControl->SetS2VisionSOV(true);

							//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms;
							//RtSleepFt(&lnDelayIn100ns);
							m_cProductIOControl->SetS3VisionSOV(true);
							m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S2 and S3 Vision SOV.\n", nHeadNo);
						}
						else
						{
							//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms;
							//RtSleepFt(&lnDelayIn100ns);
							m_cProductIOControl->SetS2VisionSOV(true);
							m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S2 Vision SOV.\n", nHeadNo);
						}

						
						nSequenceNo = nCase.IsSendS2FirstSnapProgress;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}

				}
				else
				{
					//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
					smProductEvent->RS2V_RSEQ_VISION_DONE.Set = true;
					smProductEvent->RSEQ_RS2V_START_VISION.Set = false;

					m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S2 Vision SOV.\n", nHeadNo);
					nSequenceNo = nCase.IsSendS2FirstSnapProgress;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
			//}
			//else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			//{
			//	m_cProductShareVariables->SetAlarm(5524);
			//	//m_cProductIOControl->SetSideWall2_3VisionLighitngExtendCylinder(true);
			//	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: SW2_SW3 Lighting Cylinder Extend timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//	nSequenceNo = nCase.StartSendS2FirstSnap;
			//}
			
			break;

		case nCase.IsSendS2FirstSnapProgress:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS2VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 1_2.\n");
				}
				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 1_2.\n");
				}				
			}
			if ((smProductSetting->EnableS2S3BothSnapping == false && m_cProductIOControl->IsS2VisionGrabDone() == false) || 
				(smProductSetting->EnableS2S3BothSnapping == true && m_cProductIOControl->IsS2VisionGrabDone() == false && m_cProductIOControl->IsS3VisionGrabDone() == false) 
				|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 500)
			{
				bPrintLogForFirstTime = false;
				nSequenceNo = nCase.IsSendS2FirstSnapDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsSendS2FirstSnapDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS2VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 2_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 2_2.\n");
				}				
			}
			if (((((smProductSetting->EnableS2S3BothSnapping == false && m_cProductIOControl->IsS2VisionGrabDone() == true)) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50)
				|| ((smProductSetting->EnableS2S3BothSnapping == true && m_cProductIOControl->IsS2VisionGrabDone() == true && m_cProductIOControl->IsS3VisionGrabDone() == true))) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50)) 
				&& smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
			{
				bPrintLogForFirstTime = false;
				smProductProduction->CurrentS2VisionRetryCount = 0;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				//smProductProduction->CurrentBottomVisionLoopNo = 0;
				smProductProduction->CurrentS2VisionLoopNo = 0;
				smProductProduction->CurrentS2FacetSnapTimes = 0;
				smProductProduction->CurrentS3FacetSnapTimes = 0;
				m_cProductIOControl->SetS2VisionSOV(false);
				m_cProductIOControl->SetS3VisionSOV(false);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision Parting done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductSetting->EnableS2S3BothSnapping == true)
				{
					//nSequenceNo = nCase.IsS2S3AdditionalRequiredMoveAndSnap;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
					//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE.Set = false;

					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
					//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE.Set = false;

					nSequenceNo = nCase.DelayAfterS2S3AdditionalGrabDone;
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 S3 Both Snap done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				else
				{
					//nSequenceNo = nCase.StartSendS3FirstSnap;
					nSequenceNo = nCase.DelayBeforeSendS3FirstSnap;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);

					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 Snap done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->bPNP1AllowS2S3Snap = true;
				if (smProductSetting->EnableS2S3BothSnapping == true)
				{
					if (m_cProductIOControl->IsS2VisionGrabDone() == false && m_cProductIOControl->IsS3VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS2VisionSOV(false);
						m_cProductIOControl->SetS3VisionSOV(false);
						smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
						smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6205);
						}

						smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

						smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;

						smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

						smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
					}
					else if (m_cProductIOControl->IsS2VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS2VisionSOV(false);
						smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6205);
						}
						smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

						smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
					}
					else if (m_cProductIOControl->IsS3VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS3VisionSOV(false);
						smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6705);
						}
						smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

						smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
					}
				}
				else
				{
					m_cProductIOControl->SetS2VisionSOV(false);
					smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
					if (smProductProduction->CurrentS2VisionRetryCount < 3)
					{
						smProductProduction->CurrentS2VisionRetryCount++;
					}
					else
					{
						smProductProduction->CurrentS2VisionRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6205);
					}

					smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;

					smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
				}

				//nSequenceNo = nCase.StartSendS2FirstSnap;
				nSequenceNo = nCase.IsS2S3VisionRCDone;
			}
			break;

		case nCase.DelayBeforeSendS3FirstSnap:
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
				lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

				if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms)
				{
					nSequenceNo = nCase.StartSendS3FirstSnap;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				break;

		case nCase.StartSendS3FirstSnap:
			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true 
				&& smProductSetting->EnableVision == true /*&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1*/)
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				/*smProductProduction->IsS3VisionFistSnap = true;
				smProductEvent->RS3V_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RS3V_START_VISION.Set = true;*/
				//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms;
				//RtSleepFt(&lnDelayIn100ns);
				m_cProductIOControl->SetS3VisionSOV(true);
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RS3V_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RS3V_START_VISION.Set = false;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S3 Vision SOV.\n", nHeadNo);
			nSequenceNo = nCase.IsSendS3FirstSnapProgress;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsSendS3FirstSnapProgress:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS3VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 3_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 3_2.\n");
				}				
			}
			if (m_cProductIOControl->IsS3VisionGrabDone() == false || lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 500)
			{
				bPrintLogForFirstTime = false;
				nSequenceNo = nCase.IsSendS3FirstSnapDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;


		case nCase.IsSendS3FirstSnapDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS3VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 4_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 4_2.\n");
				}				
			}
			if (((m_cProductIOControl->IsS3VisionGrabDone() == true) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50) 
				&& smProductSetting->EnableS3Vision == true	&& smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
			{
				bPrintLogForFirstTime = false;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				m_cProductIOControl->SetS3VisionSOV(false);
				smProductProduction->CurrentS2VisionLoopNo = 0;
				//smProductProduction->CurrentBottomVisionLoopNo = 0;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision Parting done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S3 Vision Parting done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nSequenceNo = nCase.IsS2S3AdditionalRequiredMoveAndSnap;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
				//smProductEvent->RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE.Set = false;

				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE.Set = false;
				//smProductEvent->GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE.Set = false;

				nSequenceNo = nCase.DelayAfterS2S3AdditionalGrabDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cProductIOControl->SetS3VisionSOV(false);
				smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
				if (smProductProduction->CurrentS2VisionRetryCount < 3)
				{
					smProductProduction->CurrentS2VisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentS2VisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6705);
				}
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;


				nSequenceNo = nCase.IsS3VisionRCDone;
			}
			break;

		case nCase.IsS3VisionRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0
				|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductSetting->EnableVision == true
					&& smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
				{
					smProductProduction->CurrentS3VisionRCRetryCount = 0;

					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StartSendS3FirstSnap;
					nSequenceNo = nCase.DelayBeforeSendS3FirstSnap;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					//nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsS3VisionRCDone;
						nSequenceNo = nCase.DelayAfterSW3_PARTING_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						if (smProductProduction->CurrentS3VisionRCRetryCount < 3)
						{
							smProductProduction->CurrentS3VisionRCRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS3VisionRCRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6207);
						}

						if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = false;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
						}

						m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC timeout.\n", nHeadNo);
						nSequenceNo = nCase.IsS3VisionRCDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}					
				}
			}
			break;

		case nCase.StartSendS2S3CheckingRC:
			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
			}
			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationS2S3 = 2;
			nSequenceNo = nCase.IsSendS2S3CheckingRCDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		//case nCase.IsSendS2S3CheckingRCDone:
		//	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
		//	lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
		//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
		//	{
		//		if ((smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true
		//			&& smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true)
		//			|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false
		//				|| smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
		//		{
		//			m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
		//			nS2S3Cycle = 1;
		//			smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
		//			smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
		//			nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;
		//		}
		//		else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
		//		{
		//			nSequenceNo = nCase.StartSendS2S3CheckingRC;
		//			m_cProductShareVariables->SetAlarm(6207);
		//			m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision get RC timeout.\n", nHeadNo);
		//		}
		//	}
		//	else
		//	{
		//		//smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
		//		//smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
		//		//nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;
		//	}
		//	break;

		case nCase.DelayAfterS2S3AdditionalGrabDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterS2S3VisionGrabDone_ms)
			{			
				nSequenceNo = nCase.IsS2S3AdditionalRequiredMoveAndSnap;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsS2S3AdditionalRequiredMoveAndSnap:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			bIsS2VisionRequireAdditionalMoveAndSnap = false;
			for (int i = smProductProduction->CurrentS2VisionLoopNo; i < 10; i++)
			{
				if (smProductSetting->S2FacetVision[smProductProduction->CurrentS2VisionLoopNo].Enable == true)
				{
					//smProductProduction->nSidewallRightVisionAdditionalSnapNo = i + 1;
					bIsS2VisionRequireAdditionalMoveAndSnap = true;
					smProductProduction->CurrentS2VisionLoopNo = i;
					break;
				}
			}
			if (bIsS2VisionRequireAdditionalMoveAndSnap == false)//complete Input Vision
			{
				m_cLogger->WriteLog("PickAndPlaceSeq: S2 and S3 Vision Additional Move And Snap Done.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				//m_cProductIOControl->SetSideWall2_3VisionLighitngExtendCylinder(false);
				//if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].bolLastUnitInTray == true || smProductProduction->bolIsLastUnitTo2EndTray == true)
				//{
				//	nSequenceNo = nCase.SetS2S3EndTrayAfterProgress;
				//}
				//else
				//{
				//	nSequenceNo = nCase.IsS2S3VisionAdditionalEOVDone;
				//}
				nSequenceNo = nCase.IsS2S3VisionAdditionalEOVDone;
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq: S2 and S3 Vision Additional Move And Snap.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				smProductProduction->nCurrentPickupHeadAtS3 = 1;
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;
			}
			break;

		case nCase.StartMovePickAndPlaceThetaAxisToNextPosition:
			smProductProduction->dbCurrentOffset2X = 0;
			smProductProduction->dbCurrentOffset2Y = 0;

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)THETA START ROTATE TO S2S3 FACET %d ANGLE  %lf\n", nHeadNo, (int)smProductProduction->CurrentS2VisionLoopNo, (double)smProductSetting->S2FacetVision[smProductProduction->CurrentS2VisionLoopNo].ThetaOffset_mDegree);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT ROW %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputRow);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT INPUT COLUMN %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputColumn);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT X SLEEVE OFFSET %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SelveXOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)UNIT Y SLEEVE OFFSET %d.\n", nHeadNo, (int)smProductProduction->PickAndPlacePickUpHeadStationResult[1].SelveYOffset_um);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)BEFORE MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
			smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;

			smProductProduction->PickAndPlace2ThetaAxisMovePosition += (signed long)smProductSetting->S2FacetVision[smProductProduction->CurrentS2VisionLoopNo].ThetaOffset_mDegree;

			GetNewXYOffsetFromThetaCorretion(smProductProduction->PickAndPlace2ThetaAxisMovePosition - smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition,
				nHeadNo, &smProductProduction->dbCurrentOffset2X, &smProductProduction->dbCurrentOffset2Y, true, false);

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS2AndS3VisionPosition - (signed long)(smProductProduction->dbCurrentOffset2X);
			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS2AndS3VisionPosition - (signed long)(smProductProduction->dbCurrentOffset2Y);

			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT S2 S3 VISION X POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)TEACH POINT S2 S3 VISION Y POS %lf.\n", nHeadNo, (double)smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION X OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2X);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION Y OFFSET %lf\n", nHeadNo, (double)smProductProduction->dbCurrentOffset2Y);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE X POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2XAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE Y POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2YAxisMovePosition);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: (-)AFTER CALCULATION MOVE T POS %lf.\n", nHeadNo, (double)smProductProduction->PickAndPlace2ThetaAxisMovePosition);

			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = false;

				smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
			}
			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = true;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = false;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
			}
			smProductProduction->nCurrentInspectionStationS2S3 = 2;

			if (smProductCustomize->EnablePickAndPlace2Module == true
				&& smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2ThetaAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Theta Axis To S2 and S3 Offset Position.\n", nHeadNo);
			m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2YAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2YAxisMovePosition));
			m_cLogger->WriteLog("PickAndPlace2ThetaAxisMovePosition %lf.\n", (double)(smProductProduction->PickAndPlace2ThetaAxisMovePosition));
			nSequenceNo = nCase.IsMovePickAndPlaceThetaAxisToNextPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceThetaAxisToNextPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorMoveDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S2 and S3 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace Theta Axis To S2 and S3 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->dbPreviousOffset2X = smProductProduction->dbCurrentOffset2X;
				smProductProduction->dbPreviousOffset2Y = smProductProduction->dbCurrentOffset2Y;
				//nSequenceNo = nCase.StartS2AdditionalInspectSOV;
				nSequenceNo = nCase.IsSendS2S3CheckingRCDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				//nError = m_cProductMotorControl->AgitoMotorSpeed(0, 0, 50000, 100, 2500000, 2500000);
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Axis To S2 and S3 Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisToNextPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceThetaAxisToNextPosition:
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 When Offset Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopPickAndPlaceThetaAxisToNextPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopPickAndPlaceThetaAxisToNextPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Theta Axis To S1 Offset Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;//is vision offset position safe to directly move to input station
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Axis Theta To S1 Offset Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceThetaAxisToNextPosition;
			}
			break;

		

		case nCase.StartS2S3SnapPositionNumber:
			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true
				&& smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true
				&& smProductSetting->EnableVision == true)
			{
				smProductEvent->GMAIN_RTHD_SW2_FACET_SEND_SNAP_POS_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SW2_FACET_SEND_SNAP_POS.Set = true;
				smProductEvent->GMAIN_RTHD_SW3_FACET_SEND_SNAP_POS_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SW3_FACET_SEND_SNAP_POS.Set = true;
			}
			else if (smProductSetting->EnableS2Vision == false || smProductCustomize->EnableS2VisionModule == false
				|| smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false
				|| smProductSetting->EnableVision == false)
			{
				smProductEvent->GMAIN_RTHD_SW2_FACET_SEND_SNAP_POS_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SW2_FACET_SEND_SNAP_POS.Set = true;
				smProductEvent->GMAIN_RTHD_SW3_FACET_SEND_SNAP_POS_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SW3_FACET_SEND_SNAP_POS.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Send SW2 and SW3 snap number %d.\n", nHeadNo, smProductProduction->CurrentS2VisionLoopNo);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			nSequenceNo = nCase.IsS2S3SnapPositionNumberDone;
			break;

		case nCase.IsS2S3SnapPositionNumberDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductEvent->GMAIN_RTHD_SW2_FACET_SEND_SNAP_POS_DONE.Set == true && smProductEvent->GMAIN_RTHD_SW3_FACET_SEND_SNAP_POS_DONE.Set == true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Send SW2 and SW3 snap number Done %d.\n", nHeadNo, smProductProduction->CurrentS2VisionLoopNo);
				//nSequenceNo = nCase.StartS2AdditionalInspectSOV;
				nSequenceNo = nCase.DelayBeforeS2AdditionalInspectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Send SW2 and SW3 snap number Fail %d.\n", nHeadNo, smProductProduction->CurrentS2VisionLoopNo);
				nSequenceNo = nCase.StartS2S3SnapPositionNumber;
			}
			break;

		case nCase.IsSendS2S3CheckingRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0 || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true
					&& smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false
						|| smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
				{
					smProductProduction->CurrentS2VisionRCRetryCount = 0;
					smProductProduction->CurrentS3VisionRCRetryCount = 0;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nS2S3Cycle = 1;
					//smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
					//smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
					//nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;
					//nSequenceNo = nCase.StartS2AdditionalInspectSOV;
					nSequenceNo = nCase.DelayBeforeS2AdditionalInspectSOV;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsSendS2S3CheckingRCDone;
						nSequenceNo = nCase.DelayAfterSW2_FACET_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else if (smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsSendS2S3CheckingRCDone;
						nSequenceNo = nCase.DelayAfterSW3_FACET_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						//nSequenceNo = nCase.StartSendS2S3CheckingRC;
						if (smProductProduction->CurrentS3VisionRCRetryCount < 3)
						{
							smProductProduction->CurrentS3VisionRCRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS3VisionRCRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6207);
						}
						m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision get RC timeout.\n", nHeadNo);
						if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

							smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = false;

							smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = true;
						}
						if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = false;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
						}
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					
				}
			}
			else
			{
				//smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2XAxisMovePosition;
				//smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove = smProductProduction->PickAndPlace2YAxisMovePosition;
				//nSequenceNo = nCase.StartMovePickAndPlaceThetaAxisToNextPosition;
			}
			break;

		case nCase.DelayBeforeS2AdditionalInspectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms)
			{
				nSequenceNo = nCase.StartS2AdditionalInspectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartS2AdditionalInspectSOV:
			if (smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true 
				&& smProductSetting->EnableVision == true /*&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1*/)
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				/*smProductProduction->IsS2VisionFistSnap = false;
				smProductEvent->RS2V_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RS2V_START_VISION.Set = true;*/
				if (smProductSetting->EnableS2S3BothSnapping == true)
				{
					//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms;
					//RtSleepFt(&lnDelayIn100ns);
					m_cProductIOControl->SetS2VisionSOV(true);

					//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms;
					//RtSleepFt(&lnDelayIn100ns);
					m_cProductIOControl->SetS3VisionSOV(true);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S2 and S3 Vision SOV.\n", nHeadNo);
				}
				else
				{
					//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS2VisionSnap_ms;
					//RtSleepFt(&lnDelayIn100ns);
					m_cProductIOControl->SetS2VisionSOV(true);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S2 Vision SOV.\n", nHeadNo);
				}
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RS2V_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RS2V_START_VISION.Set = false;
			}
			
			nSequenceNo = nCase.IsS2AdditionalInspectSOVProgress;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);
			break;

		case nCase.IsS2AdditionalInspectSOVProgress:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS2VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 5_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 5_2.\n");
				}				
			}
			if ((smProductSetting->EnableS2S3BothSnapping == false && m_cProductIOControl->IsS2VisionGrabDone() == false) 
				|| (smProductSetting->EnableS2S3BothSnapping == true && m_cProductIOControl->IsS2VisionGrabDone() == false && m_cProductIOControl->IsS3VisionGrabDone()== false) 
				|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 500)
			{
				bPrintLogForFirstTime = false;
				nSequenceNo = nCase.IsS2AdditionalInspectSOVDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsS2AdditionalInspectSOVDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS2VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 6_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 6_2.\n");
				}				
			}
			if ((
				(
				((smProductSetting->EnableS2S3BothSnapping == true && m_cProductIOControl->IsS2VisionGrabDone() == true && m_cProductIOControl->IsS3VisionGrabDone() == true)) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50)
					|| ((smProductSetting->EnableS2S3BothSnapping == false && m_cProductIOControl->IsS2VisionGrabDone() == true)) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50)
					)
				&& smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS2VisionModule == false || smProductSetting->EnableS2Vision == false))
			{
				bPrintLogForFirstTime = false;
				smProductProduction->CurrentS2VisionRetryCount = 0;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				//smProductProduction->CurrentBottomVisionLoopNo = 0;
				m_cProductIOControl->SetS2VisionSOV(false);
				m_cProductIOControl->SetS3VisionSOV(false);
				
				if (smProductSetting->EnableS2S3BothSnapping == true)
				{
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 and S3 Vision GrabDone Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.IsS2S3AdditionalRequiredMoveAndSnap;
					nSequenceNo = nCase.DelayAfterS2S3AdditionalGrabDone;
					smProductProduction->CurrentS2VisionLoopNo++;
				}
				else
				{
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 Vision GrabDone Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StartS3AdditionalInspectSOV;
					nSequenceNo = nCase.DelayBeforeS3AdditionalInspectSOV;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				//nSequenceNo = nCase.StartS3AdditionalInspectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cProductIOControl->SetS2VisionSOV(false);
				m_cProductIOControl->SetS3VisionSOV(false);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (smProductSetting->EnableS2S3BothSnapping == true)
				{
					if (m_cProductIOControl->IsS2VisionGrabDone() == false && m_cProductIOControl->IsS3VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS2VisionSOV(false);
						m_cProductIOControl->SetS3VisionSOV(false);
						smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
						smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6205);
						}
						smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

						smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;

						smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

						smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
					}
					else if (m_cProductIOControl->IsS2VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS2VisionSOV(false);
						smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6205);
						}
						smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

						smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
					}
					else if (m_cProductIOControl->IsS3VisionGrabDone() == false)
					{
						m_cProductIOControl->SetS3VisionSOV(false);
						smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
						if (smProductProduction->CurrentS2VisionRetryCount < 3)
						{
							smProductProduction->CurrentS2VisionRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS2VisionRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6705);
						}
						smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

						smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
					}
				}
				else
				{
					m_cProductIOControl->SetS2VisionSOV(false);
					smProductEvent->RS2V_GMAIN_S2_VISION_RESET_EOV.Set = true;
					if (smProductProduction->CurrentS2VisionRetryCount < 3)
					{
						smProductProduction->CurrentS2VisionRetryCount++;
					}
					else
					{
						smProductProduction->CurrentS2VisionRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6205);
					}
					smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;

					smProductEvent->RMAIN_RS2V_GET_VISION_RESULT_DONE.Set = false;
				}

				//nSequenceNo = nCase.StartS2AdditionalInspectSOV;
				nSequenceNo = nCase.IsSendS2S3CheckingRCDone;
			}
			break;

		case nCase.DelayBeforeS3AdditionalInspectSOV:
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
				lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;

				if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms)
				{
					nSequenceNo = nCase.StartS3AdditionalInspectSOV;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				}
				break;

		case nCase.StartS3AdditionalInspectSOV:
			if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true 
				&& smProductSetting->EnableVision == true /*&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1*/)
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = false;
				/*smProductProduction->IsS3VisionFistSnap = false;
				smProductEvent->RS3V_RSEQ_VISION_DONE.Set = false;
				smProductEvent->RSEQ_RS3V_START_VISION.Set = true;*/
				//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeS3VisionSnap_ms;
				//RtSleepFt(&lnDelayIn100ns);
				m_cProductIOControl->SetS3VisionSOV(true);
			}
			else
			{
				//smProductEvent->GMAIN_RBTMV_GET_VISION_RESULT_DONE.Set = true;
				smProductEvent->RS3V_RSEQ_VISION_DONE.Set = true;
				smProductEvent->RSEQ_RS3V_START_VISION.Set = false;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start S3 Vision SOV.\n", nHeadNo);
			nSequenceNo = nCase.IsS3AdditionalInspectSOVProgress;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);
			break;

		case nCase.IsS3AdditionalInspectSOVProgress:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS3VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 7_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 7_2.\n");
				}				
			}
			if (m_cProductIOControl->IsS3VisionGrabDone() == false || /*(smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true && */lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 500)
			{
				bPrintLogForFirstTime = false;
				nSequenceNo = nCase.IsS3AdditionalInspectSOVDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.IsS3AdditionalInspectSOVDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsS3VisionGrabDone() == false)
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: false Shanyu 8_2.\n");
				}				
			}
			else
			{
				if (bPrintLogForFirstTime == false)
				{
					bPrintLogForFirstTime = true;
					m_cLogger->WriteLog("[I/O]: IsInputVisionGrabDone: true Shanyu 8_2.\n");
				}				
			}
			if (((m_cProductIOControl->IsS3VisionGrabDone() == true) //|| lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= 50) 
				&& smProductSetting->EnableS3Vision == true	&& smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
				|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
			{
				bPrintLogForFirstTime = false;
				smProductProduction->CurrentS2VisionRetryCount = 0;
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq%d: Job Pause.\n", nHeadNo);
					}
				}
				m_cProductIOControl->SetS3VisionSOV(false);
				smProductProduction->CurrentS2VisionLoopNo++;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision Facet Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S3 Vision grabdone Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nSequenceNo = nCase.IsS2S3AdditionalRequiredMoveAndSnap;
				nSequenceNo = nCase.DelayAfterS2S3AdditionalGrabDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart3);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cProductIOControl->SetS3VisionSOV(false);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: S3 Vision timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cProductIOControl->SetS3VisionSOV(false);
				smProductEvent->RS3V_GMAIN_S3_VISION_RESET_EOV.Set = true;
				if (smProductProduction->CurrentS2VisionRetryCount < 3)
				{
					smProductProduction->CurrentS2VisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentS2VisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6705);
				}
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

				smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;

				nSequenceNo = nCase.IsS3VisionAditionRCDone;
			}
			break;

		case nCase.IsS3VisionAditionRCDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0
				|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				if ((smProductSetting->EnableVision == true
					&& smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductCustomize->EnableS3VisionModule == false || smProductSetting->EnableS3Vision == false))
				{
					smProductProduction->CurrentS3VisionRCRetryCount = 0;

					m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StartS3AdditionalInspectSOV;
					nSequenceNo = nCase.DelayBeforeS3AdditionalInspectSOV;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					//nSequenceNo = nCase.StartMovePickAndPlaceXYThetaAxisToS2S3Position;
				}
				else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
				{
					if (smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set == true)
					{
						nSequenceNo_Cont = nCase.IsS3VisionAditionRCDone;
						nSequenceNo = nCase.DelayAfterSW3_FACET_ReceivedNAK;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}
					else
					{
						if (smProductProduction->CurrentS3VisionRCRetryCount < 3)
						{
							smProductProduction->CurrentS3VisionRCRetryCount++;
						}
						else
						{
							smProductProduction->CurrentS3VisionRCRetryCount = 0;
							m_cProductShareVariables->SetAlarm(6207);
						}

						if (smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true && smProductSetting->EnableVision == true)
						{
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = false;
						}
						else
						{
							smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = false;

							smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set = true;
						}

						m_cLogger->WriteLog("PickAndPlaceSeq%d: S2 and S3 Vision get RC timeout.\n", nHeadNo);
						nSequenceNo = nCase.IsS3VisionAditionRCDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					}					
				}
			}
			break;

		case nCase.IsS2S3VisionAdditionalEOVDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((m_cProductIOControl->IsS3VisionEndOfVision() == true 
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[0].Enable == 1) || smProductSetting->S2FacetVision[0].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[1].Enable == 1) || smProductSetting->S2FacetVision[1].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[2].Enable == 1) || smProductSetting->S2FacetVision[2].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[3].Enable == 1) || smProductSetting->S2FacetVision[3].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[4].Enable == 1) || smProductSetting->S2FacetVision[4].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[5].Enable == 1) || smProductSetting->S2FacetVision[5].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[6].Enable == 1) || smProductSetting->S2FacetVision[6].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[7].Enable == 1) || smProductSetting->S2FacetVision[7].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[8].Enable == 1) || smProductSetting->S2FacetVision[8].Enable == 0)
				&& ((smProductEvent->GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[9].Enable == 1) || smProductSetting->S2FacetVision[9].Enable == 0)
				&& smProductSetting->EnableVision == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true
				&& m_cProductIOControl->IsS2VisionEndOfVision() == true
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[0].Enable == 1) || smProductSetting->S2FacetVision[0].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[1].Enable == 1) || smProductSetting->S2FacetVision[1].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[2].Enable == 1) || smProductSetting->S2FacetVision[2].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[3].Enable == 1) || smProductSetting->S2FacetVision[3].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[4].Enable == 1) || smProductSetting->S2FacetVision[4].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[5].Enable == 1) || smProductSetting->S2FacetVision[5].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[6].Enable == 1) || smProductSetting->S2FacetVision[6].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[7].Enable == 1) || smProductSetting->S2FacetVision[7].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[8].Enable == 1) || smProductSetting->S2FacetVision[8].Enable == 0)
				&& ((smProductEvent->RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE.Set == true && smProductSetting->S2FacetVision[9].Enable == 1) || smProductSetting->S2FacetVision[9].Enable == 0)
				&& smProductSetting->EnableVision == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true
				//&& smProductProduction->CurrentS2FacetSnapTimes == smProductProduction->CurrentS2S3FacetTotalSnap
				//&& smProductProduction->CurrentS3FacetSnapTimes == smProductProduction->CurrentS2S3FacetTotalSnap
				)
				/*&& smProductEvent->GMAIN_RS3V_GET_VISION_RESULT_DONE.Set == true*/ 
				|| smProductSetting->EnableVision == false || smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false)
			{
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("PickAndPlaceSeq: Job Pause.\n");
					}
				}
				smProductProduction->CurrentS2FacetSnapTimes = 0;
				smProductProduction->CurrentS3FacetSnapTimes = 0;
				smProductProduction->nCurrentS2FacetVisionRetryCount = 0;
				m_cProductIOControl->SetS2VisionSOV(false);
				m_cProductIOControl->SetS3VisionSOV(false);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 S3 Vision EOV Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->nCurrentPickupHeadAtS3 = 1;
				smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set = true;
				
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				{
					smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set = true;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Input Vision snap for inspection done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nCurrentInputVisionDone++;
				nSequenceNo = nCase.SetEventPickAndPlaceProcessAtS3PositionDone;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductProduction->nCurrentS2FacetVisionRetryCount < smProductSetting->S2FacetVisionRetryCountAfterFail)
				{
					smProductProduction->nCurrentS2FacetVisionRetryCount++;
					smProductProduction->PickAndPlace2XAxisMovePosition = smProductProduction->PickAndPlace2XAxisMovePosition_BeforeMove;
					smProductProduction->PickAndPlace2YAxisMovePosition = smProductProduction->PickAndPlace2YAxisMovePosition_BeforeMove;
					smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductProduction->PickAndPlace2ThetaAxisMovePosition_BeforeMove;
					smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE.Set = false;
					smProductEvent->RMAIN_GMAIN_RESET_S2S3_VISION_RESULT.Set = true;
					nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToS2S3Position;

				}
				else
				{
					smProductProduction->CurrentS2FacetSnapTimes = 0;
					smProductProduction->CurrentS3FacetSnapTimes = 0;
					smProductProduction->nCurrentS2FacetVisionRetryCount = 0;
					m_cProductIOControl->SetS2VisionSOV(false);
					m_cProductIOControl->SetS3VisionSOV(false);
					m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: S2 S3 Vision EOV Timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductProduction->nCurrentPickupHeadAtS3 = 1;
					smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set = true;
					if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
					{
						smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set = true;
					}

					m_cLogger->WriteLog("PickAndPlaceSeq%d: Continue For Next Unit %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nCurrentInputVisionDone++;
					nSequenceNo = nCase.SetEventPickAndPlaceProcessAtS3PositionDone;
				}
			}
			break;

		case nCase.SetS2S3EndTrayAfterProgress:
			nS2S3EndTrayRetryNo = 0;
			smProductEvent->RTHD_GMAIN_S2_SEND_ENDTRAY.Set = true;
			smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_S3_SEND_ENDTRAY.Set = true;
			smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set = false;
			nSequenceNo = nCase.IsS2S3EndTrayAfterProgressDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsS2S3EndTrayAfterProgressDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& (smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true)
				&& (smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true))
				|| smProductSetting->EnableVision == false
				)
			{
				nSequenceNo = nCase.SetS2S3NewTrayAfterProgress;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (nS2S3EndTrayRetryNo <= 3)
				{

					nS2S3EndTrayRetryNo++;
					smProductEvent->RTHD_GMAIN_S2_SEND_ENDTRAY.Set = true;
					smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S3_SEND_ENDTRAY.Set = true;
					smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set = false;
					nSequenceNo = nCase.IsS2S3EndTrayAfterProgressDone;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;

				}
				else
				{
					if (smProductEvent->GMAIN_RTHD_S2_SEND_ENDTRAY_DONE.Set == false)
					{
						m_cProductShareVariables->SetAlarm(6237);
					}
					if (smProductEvent->GMAIN_RTHD_S3_SEND_ENDTRAY_DONE.Set == false)
					{
						m_cProductShareVariables->SetAlarm(6737);
					}
					nSequenceNo = nCase.SetS2S3EndTrayAfterProgress;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
			}
			break;

		case nCase.SetS2S3NewTrayAfterProgress:
			nS2S3NewTrayRetryNo = 0;
			smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
			smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_S3_SEND_TRAYNO.Set = true;
			nSequenceNo = nCase.IsS2S3NewTrayAfterProgressDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsS2S3NewTrayAfterProgressDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& (smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true)
				&& (smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true))
				|| smProductSetting->EnableVision == false
				)
			{
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToS2S3Position;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (nS2S3NewTrayRetryNo <= 3)
				{
					if (smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set == false)
					{
						nS2S3NewTrayRetryNo++;
						smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
						smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_S3_SEND_TRAYNO.Set = true;
						nSequenceNo = nCase.IsS2S3NewTrayAfterProgressDone;
						RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
						break;
					}
				}
				else
				{
					if (smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set == false)
					{
						m_cProductShareVariables->SetAlarm(6236);
					}
					if (smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set == false)
					{
						m_cProductShareVariables->SetAlarm(6735);
					}
					nSequenceNo = nCase.SetS2S3NewTrayAfterProgress;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
			}
			break;

		case nCase.SetEventPickAndPlaceProcessAtS3PositionDone:
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_S3_STATION_DONE.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Process At S3 Position Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToOutputPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToOutputPosition:
			if (smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set == true)
			{
				smProductProduction->nCurrentPickupHeadAtOutput = 1;
				smProductEvent->RSEQ_RPNP2_MOVE_TO_OUTPUT_STATION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Output Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			//else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			//{
			//	smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
			//	nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			//}
			break;
		case nCase.CheckPickAndPlaceModuleSafeToMoveToOutputPosition:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Output Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Output Position.\n", nHeadNo);
			}
			break;

		case nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPosition:
		{
			smProductSetting->OutputVisionUnitThetaOffset = 0;
			double dCalculatedBottomVisionResultXOffset = 0;
			double dCalculatedBottomVisionResultYOffset = 0;
			double dCalculatedBottomVisionXOffset = 0;
			double dCalculatedBottomVisionYOffset = 0;

			double dCalculatedOutputXOffset = 0;
			double dCalculatedOutputYOffset = 0;

			smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisOutputPosition - smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomXOffset_um;

			smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisOutputPosition - smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomYOffset_um;
			
			if (m_bHighSpeed)
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1200000, 500, 10000000, 10000000);
			else
			{	
				nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 1000000, 500, 10000000, 10000000);  //700000--> 350000 27Nov2024 For Testing Puspose //back to 700000
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 700000, 500, 8500000, 8500000);
				//nError = m_cProductMotorControl->AgitoMotorSpeed1(1, 0, 50000, 500, 250000, 250000);
			}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Set Path 3 Speed = %d.\n", nHeadNo, nError);
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				//smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisOutputPosition;
				smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				//smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisOutputPosition;
				smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMove.Set = true;
			}
			smProductProduction->PickAndPlace2ThetaAxisMovePosition = smProductTeachPoint->PickAndPlace2ThetaAxisStandbyPosition;
			smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition2;

			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPositionAndRotate.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace XYZTheta Axis To Output Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		}
		break;

		case nCase.IsMovePickAndPlaceXYZThetaAxisToOutputPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorMoveDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionAndRotateDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				if ((double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) < smProductProduction->PickAndPlace2XAxisMovePosition - 200
					|| (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)) > smProductProduction->PickAndPlace2XAxisMovePosition + 200)
				{
					m_cLogger->WriteLog("PickAndPlace2XAxisCurrentEncoderPosition when Output not going position %lf.\n", (double)(m_cProductMotorControl->ReadAgitoMotorPosition(1, 0, 0)));
					m_cLogger->WriteLog("PickAndPlace2XAxisMovePosition when Output not going position %lf.\n", (double)(smProductProduction->PickAndPlace2XAxisMovePosition));
					nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPosition;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					break;
				}
				double dblReturnValue;
				m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Current Pressure at output Station %lf.\n", nHeadNo, dblReturnValue);
				smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = false;
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.OutputStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				smProductEvent->RPNP2_RSEQ_MOVE_TO_OUTPUT_STATION_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Output Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Move PickAndPlace XYXTheta Axis To Output Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsTrayTableReadyToPlace;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XYZTheta Axis To Output Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPosition:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ThetaAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Output Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYZThetaAxisToOutputPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYZThetaAxisToOutputPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1]
				&& (smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
					|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false || smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
				&& (smProductEvent->PickAndPlace2ThetaAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Output Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYZThetaAxisToOutputPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XYZTheta Axis To Output Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYZThetaAxisToOutputPosition;
			}
			break;

		case nCase.IsTrayTableReadyToPlace:
			if (smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set == true)
			{
				//if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				//{
				//	nSequenceNo = nCase.IsPickAndPlacePickZAxisAtUpPositionDone;
				//	//nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
				//	break;
				//}
				if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1 && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					double dblReturnValue;
					m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Current Pressure at output Station before place %lf.\n", nHeadNo, dblReturnValue);
					if (dblReturnValue > -40)
					{
						smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
						m_cProductShareVariables->SetAlarm(12018);
						smProductEvent->RPNP_RSEQ_BYPASS_PLACE_FAIL.Set = true;
						smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set = true;
						//smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_MISSING_UNIT.Set = true;
						nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
						break;
					}
				}
				if (smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set == true)
				{
					nSequenceNo = nCase.IsUnitRemovedAfterPickFail;
				}
				else if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1)
				{
					smProductProduction->nCurrentPickAndPlace2PlacingRetry = 0;
					m_cLogger->WriteLog("PickAndPlaceSeq%d: Tray Table Ready To Place.\n", nHeadNo);
					nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePosition;
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
					RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart4);
				}
				else if (smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				{
					smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
					smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;
					nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
				}
			}
			break;
		case nCase.IsUnitRemovedAfterPickFail:
		{
			double dblReturnValue;
			//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			if (dblReturnValue > smProductSetting->PickUpHead2Pressure)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
				smProductEvent->RPNP2_RSEQ_BYPASS_PICK_FAIL.Set = false;
				smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace unit removed after fail picking.\n", nHeadNo);
				nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				m_cProductShareVariables->SetAlarm(12033);
			}
			break;
		}
		case nCase.StartMovePickAndPlaceZAxisToDownPlacePosition:
		{
			LONGLONG  lConvert1msTo100ns = 10000;
			lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforePickupHeadGoingDownForPlacement_ms;
			RtSleepFt(&lnDelayIn100ns);
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPosition.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Axis To Down Place Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToDownPlacePositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
		}
			break;

		case nCase.IsMovePickAndPlaceZAxisToDownPlacePositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveToOutputTraySoftlandingPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Place Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlaceZAxisPlaceUnitDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cProductShareVariables->SetAlarm(12030);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Down Place Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPlacePosition;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToDownPlacePosition:
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToDownPlacePositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToDownPlacePositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 10000)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Down Place Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToDownPlacePosition;
			}
			break;

		case nCase.IsPickAndPlaceZAxisPlaceUnitDone:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Z Axis Place Unit Done.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.StartMovePickAndPlaceZAxisToUpPosition:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2ZAxisMovePosition = smProductTeachPoint->PickAndPlace2ZAxisUpPosition + (signed long)smProductSetting->UnitThickness_um;
				smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2ZAxisMotorMoveUpPosition.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Start Move PickAndPlace Z Axis To Up Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceZAxisToUpPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceZAxisToUpPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			lnPnPSequenceClockSpan4.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart4.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorMoveUpPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TIMER],PickAndPlaceSeq%d: Placement Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsPickAndPlacePickZAxisAtUpPositionDone;			
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Z Axis To Up Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpPosition;
			}
			break;

		case nCase.StopMovePickAndPlaceZAxisToUpPosition:
			smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2ZAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceZAxisToUpPositionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceZAxisToUpPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductSetting->EnablePH[1] && smProductEvent->PickAndPlace2ZAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Z Axis To Up And Theta To Orientation Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceZAxisToUpPosition;
			}
			break;

		case nCase.IsPickAndPlacePickZAxisAtUpPositionDone:
		{
			double dblReturnValue;
			signed long lngEncoderPositionZ = 0;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount < 100)
			{
				break;
			}
			//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			m_cProductMotorControl->THKReadEncoderValue(1, 0, &lngEncoderPositionZ);
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis At Up Position Done After Place.\n", nHeadNo);
			if ((lngEncoderPositionZ > (smProductProduction->PickAndPlace2ZAxisMovePosition + 5) 
				|| lngEncoderPositionZ < (smProductProduction->PickAndPlace2ZAxisMovePosition - 5 ))&& smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				m_cProductShareVariables->SetAlarm(11036);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Not At Up Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToUpPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				break;

			}
			if (dblReturnValue > smProductSetting->PickUpHead2Pressure || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				//smProductProduction->OutputTableResult[0] = smProductProduction->PickAndPlacePickUpHeadStationResult[1];
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
				smProductProduction->PickUpHeadCount[1]++;
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductProduction->nCurrrentTotalUnitDoneByLot++;
					smProductProduction->nCurrentTotalUnitDone++;
					smProductProduction->nCurrentLotGoodQuantity++;
					smProductProduction->OutputQuantity++;
				}

				smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
				smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = false;
				smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = false;

				smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis At Up Position Done After place.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceYAxisToStandbyPositionAfterPlace;
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				//m_cProductShareVariables->SetAlarm(12015);
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Pick Z Axis fail place unit.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePosition;
				if (smProductSetting->EnablePickupHeadRetryPlacingNo == true)
				{
					//if (smProductSetting->PickupHeadRetryPickingNo != 0)
					{
						if (smProductProduction->nCurrentPickAndPlace2PlacingRetry < smProductSetting->PickupHeadRetryPlacingNo)
						{
							m_cProductShareVariables->SetAlarm(12015);
							smProductProduction->nCurrentPickAndPlace2PlacingRetry++;
							nSequenceNo = nCase.StartMovePickAndPlaceZAxisToDownPlacePosition;
						}
						else
						{
							m_cProductShareVariables->SetAlarm(12028);
							nSequenceNo = nCase.IsUnitRemovedAfterPlaceFail;
							break;
						}
					}
				}
				else
				{
					m_cProductShareVariables->SetAlarm(12029);
					nSequenceNo = nCase.IsUnitRemovedAfterPlaceFail;
					break;
				}
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;
		}
		break;
		case nCase.IsUnitRemovedAfterPlaceFail:
		{
			double dblReturnValue;
			//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			m_cProductMotorControl->THKReadPressureValue(1, &dblReturnValue);
			if (dblReturnValue > smProductSetting->PickUpHead2Pressure)
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
				nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
				smProductEvent->RPNP_RSEQ_BYPASS_PLACE_FAIL.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Unit is removed after PickAndPlace Pick Z Axis Fail placing.\n", nHeadNo);
				//RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 1;
				m_cProductShareVariables->SetAlarm(12029);
			}
			break;
		}
		case nCase.StartMovePickAndPlaceYAxisToStandbyPositionAfterPlace:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYAxisToStandbyPositionAfterPlaceDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceYAxisToStandbyPositionAfterPlaceDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done after Output replacement done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlacePlaceProcessDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position after Output replacement timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYAxisToStandbyPositionAfterPlace;
			}
			break;

		case nCase.SetEventPickAndPlacePlaceProcessDone:
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent = 0;
			//smProductProduction->PickAndPlacePickUpHeadStationResult[1] = sClearResult;
			//if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			//{
			//	if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			//	{
			//		smProductProduction->nCurrrentTotalUnitDoneByLot++;
			//		smProductProduction->nCurrentTotalUnitDone++;
			//		smProductProduction->OutputQuantity++;
			//	}
			//	smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set = true;
			//}
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].InputResult = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].BottomResult = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].S1Result = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].S2Result = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].S2PartingResult = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].S3Result = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].S3PartingResult = 0;
			smProductProduction->PickAndPlacePickUpHeadStationResult[1].SetupResult = 0;

			smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			smProductEvent->RPNP2_RSEQ_PROCESS_AT_OUTPUT_STATION_DONE.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Place Process Done.\n", nHeadNo);
			if (smProductEvent->RPNP2_RSEQ_Y_MOVE_STANDBY.Set == true)
			{
				smProductEvent->RPNP2_RSEQ_Y_MOVE_STANDBY.Set = false;
				nSequenceNo = nCase.StartMovePickAndPlaceYToStandbyPositionAfterOutputReplace;
			}
			else
			{
				nSequenceNo = nCase.IsEventPickAndPlaceDisable;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.StartMovePickAndPlaceYToStandbyPositionAfterOutputReplace:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYToStandbyPositionAfterOutputReplaceDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsMovePickAndPlaceYToStandbyPositionAfterOutputReplaceDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done after Output replacement done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsEventPickAndPlaceDisable;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position after Output replacement timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYToStandbyPositionAfterOutputReplace;
			}
			break;
		case nCase.StopMovePickAndPlaceYToStandbyPositionAfterOutputReplace:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			nSequenceNo = nCase.IsStopMovePickAndPlaceYToStandbyPositionAfterOutputReplaceDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;
		case nCase.IsStopMovePickAndPlaceYToStandbyPositionAfterOutputReplaceDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y Axis To Standby Position Done after Output replacement done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYToStandbyPositionAfterOutputReplace;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop PickAndPlace Y Axis To Standby Position Done after Output replacement timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYToStandbyPositionAfterOutputReplace;
			}
			break;

		case nCase.IsEventPickAndPlaceDisable:
			if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Parking Station.\n", nHeadNo);
				nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToParkingPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.InputStation)
			{
				//smProductEvent->PickAndPlace2YAxisMotorMoveCurve.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Input Station.\n", nHeadNo);
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set == true)
			{
				smProductEvent->RSEQ_RPNP2_POST_PRODUCTION_START.Set = false;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event PickAndPlace Standby for post production.\n", nHeadNo);
				nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction;
			}
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToParkingPosition:
			if (true)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Parking Position.\n", nHeadNo);
				nSequenceNo = nCase.StartMovePickAndPlaceYAxisToParkingPosition2;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Parking Position.\n", nHeadNo);
			}
			break;

		case nCase.StartMovePickAndPlaceYAxisToParkingPosition2:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)// && smProductSetting->EnablePH[0])
			{
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisParkingPosition;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisStandbyPosition;
				//smProductEvent->PickAndPlace1XAxisMotorMoveToParkingPositionDone.Set = false;
				//smProductEvent->StartPickAndPlace1XAxisMotorMoveToParkingPosition.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Parking Position.\n", nHeadNo);
			}
			nSequenceNo = nCase.IsMovePickAndPlaceYAxisToParkingPositionDone2;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceYAxisToParkingPositionDone2:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true && smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation)// && smProductSetting->EnablePH[0]
				//&& smProductEvent->PickAndPlace1YAxisMotorMoveToStandbyPositionDone.Set == true && smProductEvent->PickAndPlace1XAxisMotorMoveToS1PositionDone.Set == true
				//parking position
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToParkingPosition2;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
				{
					smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Not Safe To Move or Door Get Trigger %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone2;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				break;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(44002);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToParkingPosition2;
			}
			break;

		case nCase.StopMovePickAndPlaceYAxisToParkingPosition2:
			if (smProductCustomize->EnablePickAndPlace2YAxisMotor)
			{
				smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone2;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceYAxisToParkingPositionDone2:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2YAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYAxisToParkingPosition2;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(44008);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToParkingPosition2;
			}
			break;

		case nCase.StartMovePickAndPlaceXAxisToParkingPosition2:
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)// && smProductSetting->EnablePH[0])
			{
				smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Parking Position.\n", nHeadNo);
			}
			nSequenceNo = nCase.IsMovePickAndPlaceXAxisToParkingPositionDone2;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceXAxisToParkingPositionDone2:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true && smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
			{
				smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Parking Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlaceToParkingPositionDone;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
				{
					smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
					smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Not Safe To Move or Door Get Trigger %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone2;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
				break;
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(56002);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Parking Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToParkingPosition2;
			}
			break;

		case nCase.StopMovePickAndPlaceXAxisToParkingPosition2:
			if (smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Parking Position.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone2;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXAxisToParkingPositionDone2:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Parking Position Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToParkingPosition2;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(56008);
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Parking Position timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToParkingPosition2;
			}
			break;
		case nCase.SetEventPickAndPlaceToParkingPositionDone:
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace To Parking Position Done.\n", nHeadNo);
			nSequenceNo = nCase.WaitingToReceiveEventStartPickAndPlaceToPickPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.DelayAfterSetupReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_SETUP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_SETUP_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterBottomReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterSW1ReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S1_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S1_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterSW2_PARTING_ReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterSW2_FACET_ReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterSW3_PARTING_ReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterSW3_FACET_ReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.WaitingToReceiveEventStartPickAndPlaceToStandbyStationPostProduction:
			if (true)
			{
				if (smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set == true)
				{
					smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set = false;
				}
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Receive Event Start PickAndPlace To Standby Station Post Production.\n", nHeadNo);
				nSequenceNo = nCase.SelectPickAndPlaceStandbyStationPostProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			break;

		case nCase.SelectPickAndPlaceStandbyStationPostProduction:
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Select PickAndPlace Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.StartMovePickAndPlaceYAxisToStandbyStationPostProduction;
			//nSequenceNo = nCase.CheckPickAndPlaceModuleSafeToMoveToStandbyStationPostProduction;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.CheckPickAndPlaceModuleSafeToMoveToStandbyStationPostProduction:
			//if (m_cProductIOControl->IsSW2N3VisionLightingDownSensor() == true && m_cProductIOControl->IsSW2N3VisionLightingUpSensor() == false)
			//{
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Safe To Move To Standby Station Post Production.\n", nHeadNo);
			//	nSequenceNo = nCase.StartMovePickAndPlaceYAxisToStandbyStationPostProduction;
			//	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			//}
			//else
			//{
			//	m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace Module Not Safe To Move To Standby Station Post Production.\n", nHeadNo);
			//}
			break;

		case nCase.StartMovePickAndPlaceXYAxisToStandbyStationPostProduction:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisAtS1AndBottomVisionPosition;
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisAtS1AndBottomVisionPosition;
				smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToS1Position.Set = true;
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Position.\n", nHeadNo);
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceXYAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceXYAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true && smProductEvent->PickAndPlace2XAxisMotorMoveToS1PositionDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetEventPickAndPlaceToStandbyStationPostProductionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace XY Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.StopMovePickAndPlaceXYAxisToStandbyStationPostProduction:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXYAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXYAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor && smProductCustomize->EnablePickAndPlace2YAxisMotor && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true && smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXYAxisToStandbyStationPostProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace XY Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXYAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.StartMovePickAndPlaceYAxisToStandbyStationPostProduction:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2YAxisMotor)// && smProductSetting->EnablePH[1])
			{
				smProductProduction->PickAndPlace2YAxisMovePosition = smProductTeachPoint->PickAndPlace2YAxisStandbyPosition;
				smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2YAxisMotorMoveToStandbyPosition.Set = true;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Position.\n", nHeadNo);
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceYAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceYAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2YAxisMotor// && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorMoveToStandbyPositionDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false)// || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToStandbyStationPostProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace Y Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.StopMovePickAndPlaceYAxisToStandbyStationPostProduction:
			smProductEvent->PickAndPlace2YAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2YAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceYAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceYAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2YAxisMotor// && smProductSetting->EnablePH[1]
				&& smProductEvent->PickAndPlace2YAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false)// || smProductSetting->EnablePH[1] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceYAxisToStandbyStationPostProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace Y Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceYAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.StartMovePickAndPlaceXAxisToStandbyStationPostProduction:
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor)
			{
				smProductProduction->PickAndPlace2XAxisMovePosition = smProductTeachPoint->PickAndPlace2XAxisParkingPosition;
				smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set = false;
				smProductEvent->StartPickAndPlace2XAxisMotorMoveToParkingPosition.Set = true;
				smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.DisableStation;
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Parking Position.\n", nHeadNo);
			}
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Post Production Position.\n", nHeadNo);
			nSequenceNo = nCase.IsMovePickAndPlaceXAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsMovePickAndPlaceXAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if ((smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation && smProductEvent->PickAndPlace2XAxisMotorMoveToParkingPositionDone.Set == true)
				|| smProductCustomize->EnablePickAndPlace2Module == false || smProductCustomize->EnablePickAndPlace2XAxisMotor == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RPNP2_RSEQ_POST_PRODUCTION_DONE.Set = true;
				//if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.S3Station)
				//{
				//	smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.S3Station;
				//	smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				//}
				//else if (smProductProduction->PickAndPlace2StationToMove == PickAndPlaceCurrentStation.DisableStation)
				//{
					smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
					smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
				//}
				nSequenceNo = nCase.SetEventPickAndPlaceToStandbyStationPostProductionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Move PickAndPlace X Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.StopMovePickAndPlaceXAxisToStandbyStationPostProduction:
			smProductEvent->PickAndPlace2XAxisMotorStopDone.Set = false;
			smProductEvent->StartPickAndPlace2XAxisMotorStop.Set = true;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Standby Station Post Production.\n", nHeadNo);
			nSequenceNo = nCase.IsStopMovePickAndPlaceXAxisToStandbyStationPostProductionDone;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		case nCase.IsStopMovePickAndPlaceXAxisToStandbyStationPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
			lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart2.QuadPart;
			if (smProductCustomize->EnablePickAndPlace2Module == true && smProductCustomize->EnablePickAndPlace2XAxisMotor// && smProductSetting->EnablePH[0]
				&& smProductEvent->PickAndPlace2XAxisMotorStopDone.Set == true
				|| smProductCustomize->EnablePickAndPlace2Module == false)// || smProductSetting->EnablePH[0] == false)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Standby Station Post Production Done %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMovePickAndPlaceXAxisToStandbyStationPostProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			}
			else if (lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("PickAndPlaceSeq%d: Stop Move PickAndPlace X Axis To Standby Station Post Production timeout %ums.\n", nHeadNo, lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StopMovePickAndPlaceXAxisToStandbyStationPostProduction;
			}
			break;

		case nCase.SetEventPickAndPlaceToStandbyStationPostProductionDone:
			//smProductProduction->PickAndPlace2CurrentStation = PickAndPlaceCurrentStation.DisableStation;
			//smProductProduction->PickAndPlace2StationToMove = PickAndPlaceCurrentStation.UnknownStation;
			m_cLogger->WriteLog("PickAndPlaceSeq%d: PickAndPlace To Standby Station Post Production Done.\n", nHeadNo);
			smProductProduction->bPNP2AllowS2S3Snap = true;
			smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set = true;
			nSequenceNo = 999;
			RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockStart2);
			break;

		default:
			return -1;
			break;
		}
		//End of sequence
		if (nSequenceNo == 999)
		{
			nSequenceNo = 0;
		}
		RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
	}
	RtGetClockTime(CLOCK_FASTEST, &lnPnPSequenceClockEnd);
	lnPnPSequenceClockSpan.QuadPart = lnPnPSequenceClockEnd.QuadPart - lnPnPSequenceClockStart.QuadPart;
	m_cLogger->WriteLog("PickAndPlaceSeq: Pick And Place sequence done %ums.\n", lnPnPSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	return 0;
}