#include "CProductSequence.h"
#include <vector>

int CProductSequence::OutputAndRejectTrayTableSequence()
{
	int nError = 0;
	bool bolReLoadingOutputTray = false;
	bool bolReLoadingRejectTray = false;
	OutputTrayTableSequenceNo nCase;
	int nPreviousProcessRejectTrayNo = 0;
	bool CheckUnitIsFirst = true;
	int nSequenceNo = nCase.WaitingToReceiveEventStartOutputAndRejectTrayTableSequence;
	int nSequenceNo_Cont = 999;
	int nSequenceNo_Prev = -99999;
	bool TemporaryDisable = false;
	bool bPreProductioning;
	bool OutputResult;
	int AlarmStatus;//0 = pocket not empty during production, 1 = full
	int nCurrentOutputVision;
	int nCurrentOutputVisionDone;
	int OutputPostVisionRCSequence;
	LARGE_INTEGER lnOutputTrayTableSequenceClockStart, lnOutputTrayTableSequenceClockEnd, lnOutputTrayTableSequenceClockSpan, lnOutputTrayTableSequenceClockStart2, lnOutputTrayTableSequenceClockSpan4, lnOutputTrayTableSequenceClockStart4, lnDelayIn100ns;
	LONGLONG  lConvert1msTo100ns = 10000;
	m_cLogger->WriteLog("OutputTrayTableseq: Start output tray table sequence\n");
	RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart);

	while (smProductEvent->ExitRTX.Set == false)
	{
		//if (smProductEvent->GGUI_RSEQ_CHECK_SEQUENCE.Set == true)
		if (nSequenceNo != nSequenceNo_Prev)
		{
			nSequenceNo_Prev = nSequenceNo;
			m_cLogger->WriteLog("OutputTrayTableseq: nCase %u\n", nSequenceNo);
		}

		if (smProductEvent->GPCS_RSEQ_ABORT.Set == true)
		{
			m_cLogger->WriteLog("OutputTrayTableseqAbort: %u\n", nSequenceNo);
			return 0;
		}

		if (smProductEvent->Alarm.Set == true || smProductEvent->JobPause.Set == true)
		{
			RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
			continue;
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
		case nCase.WaitingToReceiveEventStartOutputAndRejectTrayTableSequence:
			//event not sure
			if (smProductEvent->RSEQ_ROUT_SEQUENCE_START.Set == true)
			{
				smProductEvent->RSEQ_ROUT_SEQUENCE_START.Set = false;
				smProductProduction->nCurrentProcessRejectTrayNo = 0;
				if (smProductProduction->bOutputContinue == false)
				{
					smProductProduction->nCurrentOutputTrayNo = 0;
					smProductProduction->nCurrentRejectTrayNo = 0;
				}
				OutputPostVisionRCSequence = 1;
				bPreProductioning = true;
				CheckUnitIsFirst = true;
				nCurrentOutputVision = 0;
				OutputResult = false;
				smProductProduction->IsPostDone = false;
				m_cLogger->WriteLog("OutputTrayTableseq: Come in.\n");
				nSequenceNo = nCase.IsOutputTrayTableZAxisAtDownPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart);
			}
			break;
		case nCase.IsOutputTrayTableZAxisAtDownPosition:
			if (m_cProductMotorControl->IsOutputTrayTableXAxisMotorSafeToMove() == false || m_cProductMotorControl->IsOutputTrayTableYAxisMotorSafeToMove() == false)
			{
				m_cProductShareVariables->SetAlarm(47010);
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Not At Down Position");
				break;
			}
			else
			{
				nSequenceNo = nCase.IsOutputTrayTableReady;
			}
			if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
				smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set = true;
				nSequenceNo = nCase.EndOfSequence;
			}
			break;

		case nCase.IsOutputTrayTableReady:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				if (m_cProductIOControl->IsOutputLoadingStackerPresentSensorOn() == true && m_cProductIOControl->IsRejectTrayPresentSensorOn() == true)
				{
					if (smProductSetting->EnableOutputTableVacuum == true)
					{
						m_cProductIOControl->SetOutputTrayTableVacuumOn(true);
					}
					//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToLoadingPosition;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.IsSetOutputTableVacuumOnDoneBeforeLoadingPosition;
				}
				else
				{
					if (m_cProductIOControl->IsOutputLoadingStackerPresentSensorOn() == false)
					{
						m_cProductShareVariables->SetAlarm(5410);
						m_cLogger->WriteLog("OutputTrayTableseq: Tray not present on output loading stacker.\n");
						break;
					}
					else if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == false)
					{
						m_cProductShareVariables->SetAlarm(5408);
						m_cLogger->WriteLog("OutputTrayTableseq: Reject Tray not present on output loading stacker.\n");
						break;
					}
				}
					
			}
			else if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = false;
				smProductProduction->nOutputEdgeCoordinateX = 1;
				smProductProduction->nOutputEdgeCoordinateY = 1;
				smProductProduction->nCurrentOutputUnitOnTray = 0;
				smProductProduction->nOutputRunningState = 1;
				smProductProduction->nCurrentOutputTrayNo++;
				smProductProduction->bIsOutputFirstUnit = true;
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendVisionOutputTrayNoDone;

			}
			break;

		case nCase.IsSetOutputTableVacuumOnDoneBeforeLoadingPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingOutputTableVacuumOnOffCompletelyBeforeNextStep_ms)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Table Vacuum On Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToLoadingPosition;
			}
			break;

		case nCase.IsSendVisionOutputTrayNoDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableOutputVision == true) || smProductSetting->EnableOutputVision == false)
				) || smProductSetting->EnableVision == false)
			{
				nSequenceNo = nCase.IsRejectTrayTableReady;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6835);
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				break;
			}
			break;

		case nCase.IsRejectTrayTableReady:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (TemporaryDisable == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false
				&& (m_cProductIOControl->IsRejectTrayPresentSensorOn() == false))
			{
				nSequenceNo = nCase.StartMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
			}
			else
			{
				if (smProductSetting->EnableOutputTableVacuum == true)
				{
					m_cProductIOControl->SetRejectTrayTableVacuumOn(true);
				}
				
				smProductProduction->nCurrentRejectUnitOnTray = 0;
				smProductProduction->nCurrentTotalRejectUnit = 0;
				smProductProduction->nRejectEdgeCoordinateX = 1;
				smProductProduction->nRejectEdgeCoordinateY = 1;
				smProductProduction->nCurrentRejectTrayNo++;
				smProductProduction->bIsRejectFirstUnit = true;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendVisionRejectTrayNoDone;
			}
			break;

		case nCase.IsSendVisionRejectTrayNoDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true &&
				smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set == true
				) || smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false)
			{
				//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToCenterPosition;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPosition;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6835);
				smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				break;
			}
			break;

#pragma region Loading
		case nCase.StartMoveOutputTrayTableXYAxisToLoadingPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMoveLoad.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMoveLoad.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToLoadingPositionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis Motor move load position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cProductShareVariables->SetAlarm(5506);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Before Loading Tray %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					break;
				}
				
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToLoadingPosition;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis move load position Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Axis Motor Move Load timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Y Axis Motor Move Load timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToLoadingPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToLoadingPosition:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToLoadingPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis stop Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Y Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToLoadingPosition;
			}
			break;
		case nCase.StartMoveOutputTrayTableZAxisToLoadingPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveLoad.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToLoadingPositionDone;
			break;
		case nCase.IsMoveOutputTrayTableZAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true 
					&& smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move loading position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoading;
			}
			else if (IsReadyToMoveProduction() == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move Loading timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToLoadingPosition;
			}
			break;
		case nCase.StopMoveOutputTrayTableZAxisToLoadingPosition:

			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDone;
			break;
		case nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToLoadingPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToLoadingPosition;
			}
			break;

		case nCase.CheckTrayPresentSensorBeforeLoading:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Checking before loading done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderUnlock;
				}
				else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
						m_cProductShareVariables->SetAlarm(5511);
						m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoading;
				}
			}
			break;

		case nCase.OutputLoadingStackerUnlockCylinderUnlock:
			m_cProductIOControl->SetOutputLoadingStackerUnlockCylinderOn(true);

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsOutputLoadingStackerUnlockCylinderUnlock;
			break;

		case nCase.IsOutputLoadingStackerUnlockCylinderUnlock:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputLoadingStackerUnlockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder Unlock done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputLoadingStackerUnlockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5412);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder Unlock Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderUnlock;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToSingulationPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToSingulationPositionDone;
			break;
		case nCase.IsMoveOutputTrayTableZAxisToSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true 
					&& smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move singulation load position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true 
					&& smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move Singulation load timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPosition;
			}
			break;
		case nCase.StopMoveOutputTrayTableZAxisToSingulationPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDone;
			break;
		case nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPosition;
			}
			break;

		case nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					smProductEvent->OutputTrayTableZAxisMotorChangeSlowSpeedDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorChangeSlowSpeed.Set = true;
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Checking After Move To Singulation Position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderLock;
				}
				else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
						m_cProductShareVariables->SetAlarm(5511);
						m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition;
				}
			}
			break;

		case nCase.OutputLoadingStackerUnlockCylinderLock:

			if (smProductEvent->OutputTrayTableZAxisMotorChangeSlowSpeedDone.Set == true)
			{
				m_cProductIOControl->SetOutputLoadingStackerUnlockCylinderOn(false);

				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsOutputLoadingStackerUnlockCylinderLock;
			}
			break;

		case nCase.IsOutputLoadingStackerUnlockCylinderLock:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder lock done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5411);
					m_cLogger->WriteLog("OutputTrayTableseq:  Output Loading Stacker Cylinder lock Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderLock;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPosition:

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveSecondSingulationDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveSecondSingulation.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisTo_SecondSingulationPositionDone;
			}
			break;

		case nCase.IsMoveOutputTrayTableZAxisTo_SecondSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSecondSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move second singulation position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsTrayAvailableAfterSecondSingulationPosition;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Input Tray Table Z Axis Motor Move Second Singulation timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPosition;
			}
			break;

		case nCase.IsTrayAvailableAfterSecondSingulationPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > smProductSetting->DelayForCheckingInputOutputTableZAtSecondSigulationTrayAvalable_ms)
			{
				if ((m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false || m_cProductIOControl->IsOutputTrayTiltSensorOn() == false) && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					m_cProductShareVariables->SetAlarm(5511);
					//nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent;
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				}
				else
				{
					smProductEvent->OutputTrayTableZAxisMotorChangeNormalSpeedDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorChangeNormalSpeed.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPosition;
				}
			}
			break;



		case nCase.StartMoveOutputTrayTableZAxisToDownPosition:
			if (smProductEvent->OutputTrayTableZAxisMotorChangeNormalSpeedDone.Set == true)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveDown.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToDownPositionDone;
			}
		
			break;

		case nCase.IsMoveOutputTrayTableZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cProductShareVariables->SetAlarm(5511);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresent;
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				break;
			}
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move down position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsOutputTrayTableReady;
			}
			else if (IsReadyToMoveProduction() == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move down timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPosition;
			}
			break;
		case nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresent:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1s);
				nSequenceNo = nCase.CheckIsTrayPresentOnOutputTrayTableDuringDown;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis stop Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresent;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;
		case nCase.CheckIsTrayPresentOnOutputTrayTableDuringDown:
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				//if()if time >preset,then
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
				lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
				if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductProduction->TrayPresentSensorOffTimeBeforeAlarm_ms)
				{
					m_cProductShareVariables->SetAlarm(5511);
					m_cLogger->WriteLog("OutputLoadingseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StopMoveInputTrayTableZAxisDownAtLoading;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					break;
				}
			}
			else if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)

			{
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToDownPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDone;
			break;
		case nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPosition;
			}
			break;
#pragma endregion

		case nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{

				smProductProduction->OutputTableResult[0].RejectColumn = smProductProduction->nRejectEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].RejectRow = smProductProduction->nRejectEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].RejectTrayNo = smProductProduction->nCurrentRejectTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			{

				smProductProduction->OutputTableResult[0].OutputColumn = smProductProduction->nOutputEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].OutputRow = smProductProduction->nOutputEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].OutputTrayNo = smProductProduction->nCurrentOutputTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
				{
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
				}

			}
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Send Row and Column before to first position.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPosition;
			break;

		case nCase.DelayAfterOutputReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterRejectReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterOutputPostReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.DelayAfterRejectPostReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START.Set = true;
				nSequenceNo = nSequenceNo_Cont;
			}
			break;

		case nCase.StartMoveOutputTrayTableXYAxisToFirstPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // REJECT TRAY 5
			{
				//first position
				//smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtRejectTrayCenterPosition + (signed long)(smProductSetting->DeviceXPitchOutput* ((((double)smProductSetting->NoOfDeviceInColOutput) / 2) - 0.5));
				//smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtRejectTrayCenterPosition - (signed long)(smProductSetting->DeviceYPitchOutput* ((((double)smProductSetting->NoOfDeviceInRowOutput / 2) - 0.5)));
							
				
				smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtRejectTrayCenterPosition - signed long(smProductSetting->DeviceXPitchOutput * ((double)(smProductSetting->NoOfDeviceInRowOutput - 1) / 2));
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtRejectTrayCenterPosition - signed long(smProductSetting->DeviceYPitchOutput * ((double)(smProductSetting->NoOfDeviceInColOutput - 1) / 2));

				smProductProduction->RejectTrayTableXIndexPosition = smProductProduction->OutputTrayTableXAxisMovePosition;
				smProductProduction->RejectTrayTableYIndexPosition = smProductProduction->OutputTrayTableYAxisMovePosition;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)// OUTPUT TRAY TABLE
			{
				//first position
				//smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtOutputTrayTableCenterPosition + (signed long)(smProductSetting->DeviceXPitchOutput* ((((double)smProductSetting->NoOfDeviceInColOutput) / 2) - 0.5));
				//smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtOutputTrayTableCenterPosition - (signed long)(smProductSetting->DeviceYPitchOutput* ((((double)smProductSetting->NoOfDeviceInRowOutput / 2) - 0.5)));
						
				smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtOutputTrayTableCenterPosition - signed long(smProductSetting->DeviceXPitchOutput * ((double)(smProductSetting->NoOfDeviceInRowOutput - 1) / 2));
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtOutputTrayTableCenterPosition - signed long(smProductSetting->DeviceYPitchOutput * ((double)(smProductSetting->NoOfDeviceInColOutput - 1) / 2));

				smProductProduction->OutputTrayTableXIndexPosition = smProductProduction->OutputTrayTableXAxisMovePosition;
				smProductProduction->OutputTrayTableYIndexPosition = smProductProduction->OutputTrayTableYAxisMovePosition;

			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToFirstPositionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToFirstPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to first position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
							

				//if (smProductSetting->EnableScanBarcodeOnOutputTray == true)
				//{
				//	nSequenceNo = nCase.StartScanBarcodeOnTray;
				//}
				//else
				{
					nSequenceNo = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDone;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				//nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToFirstPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToFirstPosition:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToFirstPosition;
			}
			break;

		case nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			smProductProduction->CurrentOutputVisionRCRetryCount = 0;
			if (true &&
				(((smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set == true && smProductProduction->nCurrentProcessRejectTrayNo == 0)
					|| (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set == true && smProductProduction->nCurrentProcessRejectTrayNo == 5))
					&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Receives Row and Column Done after to assigned position %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
					nSequenceNo = nCase.DelayBeforeOutputVisionSOV;
				else
					nSequenceNo = nCase.DelayBeforeOutputVisionRejectSOV;

				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);

			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDone;
					nSequenceNo = nCase.DelayAfterOutputReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDone;
					nSequenceNo = nCase.DelayAfterRejectReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				{
					

					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRCRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row after to assigned position timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
					{
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
					}
					else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
					{
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
					}

				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.ReadTableToBePlaced:
			smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = false;
			if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)//not sure
			{
				smProductProduction->OutputTableResult[0].BottomResult = 1;
				smProductProduction->OutputTableResult[0].InputResult = 1;
			}
			/*smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;
			smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = true;*/
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsReadTableToBePlacedDone;
			//if (smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			break;

		case nCase.IsReadTableToBePlacedDone:
			if (IsOutputNeedWriteReport() == true)
			{
				smProductProduction->nCurrentInputTrayNumberAtOutput = smProductProduction->OutputTableResult[0].InputTrayNo;
				smProductProduction->WriteReportTrayNo = smProductProduction->nCurrentInputTrayNumberAtOutput - 1;
				smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
			}
			nSequenceNo = nCase.CheckIsMissingUnit;
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			//if (smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set == true)
			//{
			//	smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;
			//	m_cLogger->WriteLog("OutputTrayTableSeq: Set table to be placed Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//	nSequenceNo = nCase.CheckIsMissingUnit;
			//}
			//else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableSeq: Set table to be placed timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//	smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;
			//	smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = true;
			//	RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			//}
			break;

		case nCase.CheckIsMissingUnit:
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // reject
			{
				if (smProductProduction->bIsRejectFirstUnit == true)
				{
					nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPosition;
				}
				else {
					if (smProductProduction->IsMissingPostReject == false)
					{
						nSequenceNo = nCase.SetNextUnitReject;
					}
					else if (smProductProduction->IsMissingPostReject == true)
					{
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
						nSequenceNo = nCase.IsSendOutputVisionOutputRowAndColumnDone;
					}
				}
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5) // output
			{
				if (smProductProduction->bIsOutputFirstUnit == true)
				{
					nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPosition;
				}
				else 
				{
					if (smProductProduction->IsMissingPostOutput == false)
					{
						nSequenceNo = nCase.SetNextUnitOutput;
					}
					else if (smProductProduction->IsMissingPostOutput == true)
					{
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
						nSequenceNo = nCase.IsSendOutputVisionOutputRowAndColumnDone;
					}
				}
			}
			break;


		case nCase.SetNextUnitOutput:
		{
			if (smProductProduction->OutputTableResult[0].OutputRow % 2 != 0)
			{
				smProductProduction->nOutputEdgeCoordinateY++;
				smProductProduction->OutputTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchOutput);
				if (smProductProduction->nOutputEdgeCoordinateY > smProductSetting->NoOfDeviceInColOutput)
				{
					smProductProduction->nOutputEdgeCoordinateY--;
					smProductProduction->nOutputEdgeCoordinateX++;
					if (smProductProduction->nOutputEdgeCoordinateX > smProductSetting->NoOfDeviceInRowOutput)
					{
						smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = true;
						smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = true;
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
						break;
					}
					smProductProduction->OutputTrayTableXIndexPosition += (signed long)(smProductSetting->DeviceXPitchOutput);
					smProductProduction->OutputTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchOutput);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Coordinate R_%u C_%u.\n", smProductProduction->nOutputEdgeCoordinateY , smProductProduction->nOutputEdgeCoordinateX);
				}
			}
			else if (smProductProduction->OutputTableResult[0].OutputRow % 2 == 0)
			{
				smProductProduction->nOutputEdgeCoordinateY--;
				smProductProduction->OutputTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchOutput);
				if (smProductProduction->nOutputEdgeCoordinateY < 1)
				{
					smProductProduction->nOutputEdgeCoordinateX++;
					smProductProduction->nOutputEdgeCoordinateY++;
					if (smProductProduction->nOutputEdgeCoordinateX > smProductSetting->NoOfDeviceInRowOutput)
					{
						//if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
						//{
						///*	smProductProduction->nOutputEdgeCoordinateX = 1;
						//	smProductProduction->nOutputEdgeCoordinateY = 1;
						//	smProductProduction->nCurrentProcessRejectTrayNo = 0;*/
						//	nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToDownPositionDuringProductionDone;
						//}
						//else
						//{
						//	smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = true;
						//	smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = true;
						//	nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
						//	
						//}
						smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = true;
						smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = true;
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
						break;
					}
					smProductProduction->OutputTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchOutput);
					smProductProduction->OutputTrayTableXIndexPosition += (signed long)(smProductSetting->DeviceXPitchOutput);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Coordinate R_%u C_%u.\n", smProductProduction->nOutputEdgeCoordinateY, smProductProduction->nOutputEdgeCoordinateX);
				}
			}
		}
		smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableXIndexPosition;
		smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableYIndexPosition;
		nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
		break;

		case nCase.SetNextUnitReject:
		{
			

			if (smProductProduction->OutputTableResult[0].RejectRow % 2 != 0)
			{
				smProductProduction->nRejectEdgeCoordinateY++;
				smProductProduction->RejectTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchOutput);
				if (smProductProduction->nRejectEdgeCoordinateY > smProductSetting->NoOfDeviceInColOutput)
				{
					smProductProduction->nRejectEdgeCoordinateY--;
					smProductProduction->nRejectEdgeCoordinateX++;
					if (smProductProduction->nRejectEdgeCoordinateX > smProductSetting->NoOfDeviceInRowOutput)
					{
						smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = true;
						smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = true;
						nSequenceNo = nCase.StartMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
						break;
					}
					smProductProduction->RejectTrayTableXIndexPosition += (signed long)(smProductSetting->DeviceXPitchOutput);
					smProductProduction->RejectTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchOutput);
					m_cLogger->WriteLog("OutputTrayTableSeq: Reject Tray Table Coordinate R_%u C_%u.\n", smProductProduction->nOutputEdgeCoordinateY, smProductProduction->nOutputEdgeCoordinateX);
				}
			}
			else if (smProductProduction->OutputTableResult[0].RejectRow % 2 == 0)
			{
				smProductProduction->nRejectEdgeCoordinateY--;
				smProductProduction->RejectTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchOutput);
				if (smProductProduction->nRejectEdgeCoordinateY < 1)
				{
					smProductProduction->nRejectEdgeCoordinateX++;
					smProductProduction->nRejectEdgeCoordinateY++;
					if (smProductProduction->nRejectEdgeCoordinateX > smProductSetting->NoOfDeviceInRowOutput)
					{
						smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = true;
						smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = true;
						nSequenceNo = nCase.StartMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
						break;
					}
					smProductProduction->RejectTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchOutput);
					smProductProduction->RejectTrayTableXIndexPosition += (signed long)(smProductSetting->DeviceXPitchOutput);
					m_cLogger->WriteLog("OutputTrayTableSeq: Reject Tray Table Coordinate R_%u C_%u.\n", smProductProduction->nOutputEdgeCoordinateY, smProductProduction->nOutputEdgeCoordinateX);
				}
			}
		}
		smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->RejectTrayTableXIndexPosition;
		smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->RejectTrayTableYIndexPosition;
		nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
		break;

		case nCase.StartSendOutputVisionOutputOrRejectRowAndColumn:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // reject
			{
				smProductProduction->OutputTableResult[0].RejectColumn = smProductProduction->nRejectEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].RejectRow = smProductProduction->nRejectEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].RejectTrayNo = smProductProduction->nCurrentRejectTrayNo;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5) // pass
			{
				smProductProduction->OutputTableResult[0].OutputColumn = smProductProduction->nOutputEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].OutputRow = smProductProduction->nOutputEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].OutputTrayNo = smProductProduction->nCurrentOutputTrayNo;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
				{
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
				}

			}
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Send Row and Column before to set position.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart4);
			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToSetPosition;
			break;

		case nCase.StartMoveOutputTrayTableXYAxisToSetPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToSetPositionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToSetPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to set position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				{
					nSequenceNo = nCase.IsSendOutputVisionOutputRowAndColumnDone;
					//nSequenceNo = nCase.DelayAfterMoveOutputTrayTableXYAxisToSetPositionDone;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToSetPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to set position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to set position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to set position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToSetPosition;
			}
			break;	

		case nCase.StopMoveOutputTrayTableXYAxisToSetPosition:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToSetPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToSetPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToSetPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToSetPosition;
			}
			break;

		case nCase.IsSendOutputVisionOutputRowAndColumnDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				(((smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set == true && smProductProduction->nCurrentProcessRejectTrayNo < 5)
					|| (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set == true && smProductProduction->nCurrentProcessRejectTrayNo == 5))
					&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Receives Row and Column Done after to assigned position %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)//output
				{
					nSequenceNo = nCase.DelayBeforeOutputVisionSOV;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)//reject
				{
					nSequenceNo = nCase.DelayBeforeOutputVisionRejectSOV;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{

				if (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterOutputReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterRejectReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				{					

					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRCRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row after to assigned position timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
					{
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
					}
					else if (smProductProduction->nCurrentProcessRejectTrayNo < 5)
					{
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
					}

				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

#pragma region retry RC
		case nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				smProductProduction->OutputTableResult[0].RejectColumn = smProductProduction->nRejectEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].RejectRow = smProductProduction->nRejectEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].RejectTrayNo = smProductProduction->nCurrentRejectTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			{
				smProductProduction->OutputTableResult[0].OutputColumn = smProductProduction->nOutputEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].OutputRow = smProductProduction->nOutputEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].OutputTrayNo = smProductProduction->nCurrentOutputTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo < 5)
				{
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
				}

			}
			else
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = true;
					smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = false;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo < 5)
				{
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = true;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = false;
				}
			}
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Send Row and Column.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			{
				nSequenceNo = nCase.IsRetrySendOutputVisionRowAndColumnDone;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				nSequenceNo = nCase.IsRetrySendOutputVisionRejectRowAndColumnDone;
			}

			break;

		case nCase.IsRetrySendOutputVisionRowAndColumnDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				((smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false))
				)
			{
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Receives Row and Column Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.DelayBeforeOutputVisionSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);

			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsRetrySendOutputVisionRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterOutputReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else
				{
					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRCRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
				}				
			}
			break;

		case nCase.IsRetrySendOutputVisionRejectRowAndColumnDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				((smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false))
				)
			{
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Reject %u Vision Receives Row and Column Done %ums.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo, lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.DelayBeforeOutputVisionRejectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				
				if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRCRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6807);
				}
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
			}
			break;
#pragma endregion
#pragma region Output SOV
		case nCase.DelayBeforeOutputVisionSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
						
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.StartOutputVisionSOV;
			}
			break;

		case nCase.StartOutputVisionSOV:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			smProductProduction->OutputTableResult[0].OutputXOffset_um = 0;
			smProductProduction->OutputTableResult[0].OutputYOffset_um = 0;
			smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree = 0;
			smProductProduction->OutputTableResult[0].OutputResult = 0;
			smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set = false;

			}
			smProductEvent->RTHD_GMAIN_GET_OUT_VISION_XYTR_START.Set = true;
			//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			//RtSleepFt(&lnDelayIn100ns);
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.DelayAfterOutputVisionSOV;
			break;

		case nCase.DelayAfterOutputVisionSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.IsOutputVisionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.IsOutputVisionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			lnOutputTrayTableSequenceClockSpan4.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart4.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false)
				)
			{

				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[Timer], OutputTrayTableSeq: Output Move offset and Inspection done %ums.\n", lnOutputTrayTableSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->OutputTableResult[0].OutputUnitPresent = 0;
					smProductProduction->OutputTableResult[0].OutputResult = 1;
					smProductProduction->OutputTableResult[0].OutputXOffset_um = 0;
					smProductProduction->OutputTableResult[0].OutputYOffset_um = 0;
					smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree = 0;
				}

				lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms;
				RtSleepFt(&lnDelayIn100ns);
				nSequenceNo = nCase.SetOutputPreProductionPosition;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);			
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6837);
				}
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				//m_cProductShareVariables->SetAlarm(6803);
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			break;
#pragma endregion
#pragma region Reject SOV

		case nCase.DelayBeforeOutputVisionRejectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.StartOutputVisionRejectSOV;
			}
			break;

		case nCase.StartOutputVisionRejectSOV:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				smProductProduction->OutputTableResult[0].RejectXOffset_um = 0;
				smProductProduction->OutputTableResult[0].RejectYOffset_um = 0;
				smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree = 0;
				smProductProduction->OutputTableResult[0].RejectResult = 0;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set = false;
				
			}
			smProductEvent->RTHD_GMAIN_GET_REJECT_VISION_XYTR_START.Set = true;

			//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			//RtSleepFt(&lnDelayIn100ns);
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta for reject tray %u.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.DelayAfterOutputVisionRejectSOV;
			break;

		case nCase.DelayAfterOutputVisionRejectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.IsOutputVisionRejectDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.IsOutputVisionRejectDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta for reject tray %u done %ums.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo, lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
					{
						smProductProduction->OutputTableResult[0].RejectUnitPresent = 0;
						smProductProduction->OutputTableResult[0].RejectResult = 1;
						smProductProduction->OutputTableResult[0].RejectXOffset_um = 0;
						smProductProduction->OutputTableResult[0].RejectYOffset_um = 0;
						smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree = 0;
					}

				}
				//if (smProductProduction->OutputTableResult[0].OutputResult == 5 && smProductProduction->OutputTableResult[0].OutputUnitPresent == 1)//UNIT PRESENT AT OUTPUT Alarm
				//{
				//	//alarm
				//	//nSequenceNo = nCase.SetNextUnitOutput;
				//}
				//else if (smProductProduction->OutputTableResult[0].OutputResult == 1 && smProductProduction->OutputTableResult[0].OutputUnitPresent == 0)//OUTPUT PASS
				//{
				//	//nSequenceNo = nCase.AdjustOffset;
				//}
				lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms;
				RtSleepFt(&lnDelayIn100ns);
				nSequenceNo = nCase.SetOutputPreProductionPosition;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				//nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				
				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums retry %lf.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount, smProductProduction->CurrentOutputVisionRetryCount);
				
				//nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			break;

		case nCase.SetOutputPreProductionPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)//reject
			{

				if (smProductProduction->OutputTableResult[0].RejectResult == 1)
				{
					smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = false;
					smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount = 0;

					smProductProduction->RejectTrayTableCurrentXPosition = smProductProduction->OutputTrayTableXAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].RejectXOffset_um);
					smProductProduction->RejectTrayTableCurrentYPosition = smProductProduction->OutputTrayTableYAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].RejectYOffset_um);
					smProductProduction->RejectTrayTableXIndexPosition = smProductProduction->RejectTrayTableCurrentXPosition;
					smProductProduction->RejectTrayTableYIndexPosition = smProductProduction->RejectTrayTableCurrentYPosition;
					if (bPreProductioning == true)
					{
						smProductProduction->nCurrentProcessRejectTrayNo = 5;

						nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPosition;
					}
					else
					{
						//smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
						nSequenceNo = nCase.SetTableToBePlaced;
					}
					//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToPreProductionPosition;
				}
				else if (smProductProduction->OutputTableResult[0].RejectResult == 5)
				{
					if (smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount = 0;
						m_cProductShareVariables->SetAlarm(60309);
					}		
						
					nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
					//nSequenceNo = nCase.SetTableToBePlaced;
				}
				else if (smProductProduction->OutputTableResult[0].RejectResult == 7)
				{
					nSequenceNo = nCase.SetNextUnitReject;
				}
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)//output
			{
				if (smProductProduction->OutputTableResult[0].OutputResult == 1)
				{
					smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = false;
					smProductProduction->OutputTrayTableCurrentXPosition = smProductProduction->OutputTrayTableXAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].OutputXOffset_um);
					smProductProduction->OutputTrayTableCurrentYPosition = smProductProduction->OutputTrayTableYAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].OutputYOffset_um);
					smProductProduction->OutputTrayTableXIndexPosition = smProductProduction->OutputTrayTableCurrentXPosition;
					smProductProduction->OutputTrayTableYIndexPosition = smProductProduction->OutputTrayTableCurrentYPosition;

					smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount = 0;
					if (bPreProductioning == true)
					{
						//smProductProduction->nCurrentProcessRejectTrayNo++;
						smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableCurrentXPosition;
						smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableCurrentYPosition;
						bPreProductioning = false;
						smProductEvent->ROUT_RSEQ_OUTPUT_VISION_DONE.Set = true;
						smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
						nSequenceNo = nCase.SetTableToBePlaced;
					}
					else
					{
						//smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
						nSequenceNo = nCase.SetTableToBePlaced;
					}
					//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToPreProductionPosition;
				}
				else if (smProductProduction->OutputTableResult[0].OutputResult == 5)
				{					

					if (smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionNotMatchUnitRetryCount = 0;
						m_cProductShareVariables->SetAlarm(60308);
					}
					nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
					//nSequenceNo = nCase.SetTableToBePlaced;
				}
				else if (smProductProduction->OutputTableResult[0].OutputResult == 7)
				{
					nSequenceNo = nCase.SetNextUnitOutput;
				}
			}
			break;

		case nCase.SetTableToBePlaced:
			//wait PNP move to output station, then check the result and move table for PNP to place
			//if (smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set == true && smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set == false)
			//{
			//	smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = false;
			//	if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			//	{
			//		smProductProduction->OutputTableResult[0].BottomResult = 1;
			//		smProductProduction->OutputTableResult[0].InputResult = 1;
			//		smProductProduction->OutputTableResult[0].SetupResult = 1;
			//		smProductProduction->OutputTableResult[0].S1Result = 1;
			//		smProductProduction->OutputTableResult[0].S2Result = 1;
			//		smProductProduction->OutputTableResult[0].S3Result = 1;
			//	}
			//	{
			//		smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = true;
			//		RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			//		nSequenceNo = nCase.IsSetTableToBePlacedDone;
			//	}
			//}
			if (smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set == true)
			{
				//
				smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
				//
				smProductEvent->RPNP_ROUT_MOVE_TRAY_READY.Set = false;
				smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = true;
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->OutputTableResult[0].BottomResult = 1;
					smProductProduction->OutputTableResult[0].InputResult = 1;
					smProductProduction->OutputTableResult[0].SetupResult = 1;
					smProductProduction->OutputTableResult[0].S1Result = 1;
					smProductProduction->OutputTableResult[0].S2Result = 1;
					smProductProduction->OutputTableResult[0].S3Result = 1;
					smProductProduction->nCurrentProcessRejectTrayNo = 5;
				}
				nSequenceNo = nCase.IsSetTableToBePlacedDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				//if (nPreviousProcessRejectTrayNo == smProductProduction->nCurrentProcessRejectTrayNo && CheckUnitIsFirst == false)
				//{
				//	smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
				//	nSequenceNo = nCase.SetTableToBePlaced;
				//	break;
				//}
				//m_cLogger->WriteLog("OutputTrayTableSeq: Set table to be placed Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // REJECT TRAY 5
				//{
				//	CheckUnitIsFirst = false;
				//	nPreviousProcessRejectTrayNo = 0;
				//	smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->RejectTrayTableCurrentXPosition;
				//	smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->RejectTrayTableCurrentYPosition;
				//}
				//else if (smProductProduction->nCurrentProcessRejectTrayNo == 5) // OUTPUT TRAY
				//{
				//	CheckUnitIsFirst = false;
				//	nPreviousProcessRejectTrayNo = 5;
				//	smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableCurrentXPosition;
				//	smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableCurrentYPosition;
				//}
				//nSequenceNo = nCase.StartPreMove;
			}
			else if (smProductEvent->SEQ_OUT_IS_UNLOADTRAY.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			{
				if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
				{
					if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
					else
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			else if (smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set == true)
			{
				smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set = false;
				m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
				if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
				{
					if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
					else
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			//else if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			break;

		case nCase.IsSetTableToBePlacedDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set == true)
			{
				smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;
				m_cLogger->WriteLog("OutputTrayTableSeq: Set table to be placed Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // REJECT TRAY 5
				{
					smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->RejectTrayTableCurrentXPosition; //+smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtS3].BottomXOffset_um;
					smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->RejectTrayTableCurrentYPosition; //+smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtS3].BottomYOffset_um;
					smProductProduction->OutputThetaOffset = smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 5) // OUTPUT TRAY
				{
					smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableCurrentXPosition; //+smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtS3].BottomXOffset_um;
					smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableCurrentYPosition; //+smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtS3].BottomYOffset_um;
					smProductProduction->OutputThetaOffset = smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree;
				}
				nSequenceNo = nCase.StartMoveOffset;
			}
			
			//else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableSeq: Set table to be placed timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			//	smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE.Set = false;
			//	smProductEvent->RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED.Set = true;
			//	RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			//}
			break;
#pragma endregion

		case nCase.AdjustReturnOffset://return offset when unit missing
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0) // reject
			{
				smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->RejectTrayTableXIndexPosition;
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->RejectTrayTableYIndexPosition;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)//output
			{
				smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableXIndexPosition;
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableYIndexPosition;
			}
			nSequenceNo = nCase.StartMoveReturnOffset;
			break;

		case nCase.StartMoveReturnOffset:
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveReturnOffsetDone;
			break;

		case nCase.IsMoveReturnOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move offset done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = true;
				//smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
				nSequenceNo = nCase.IsReadTableToBePlacedDone;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveReturnOffsetDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("ManualPickAndPlaceSeq: Output Tray Table X Y Axis Move Return offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move Return offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move Return offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveReturnOffset;
			}
			break;

		case nCase.StopMoveReturnOffset:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveReturnOffsetDone;
			break;

		case nCase.IsStopMoveReturnOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveReturnOffset;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveReturnOffset;
			}
			break;
			//end return offset

		case nCase.StartMoveOffset:
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOffsetDone;
			break;

		case nCase.IsMoveOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move offset done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUT_RSEQ_OUTPUT_UNIT_READY_TO_BE_PLACED.Set = true;
				//smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = true;
				nSequenceNo = nCase.WaitingUnitToBePlaced;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOffsetDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("ManualPickAndPlaceSeq: Output Tray Table X Y Axis Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOffset;
			}
			break;

		case nCase.StopMoveOffset:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOffsetDone;
			break;

		case nCase.IsStopMoveOffsetDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOffset;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOffset;
			}
			break;

		case nCase.WaitingUnitToBePlaced:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			if (smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set == true && smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set == false)//wait PNP to place
			{
				smProductEvent->RPNP_ROUT_OUTPUT_UNIT_PLACED_DONE.Set = false;
				

				smProductProduction->OutputTableResult[0].InputTrayNo = smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtOutput].InputTrayNo;
				smProductProduction->OutputTableResult[0].InputRow = smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtOutput].InputRow;
				smProductProduction->OutputTableResult[0].InputColumn = smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtOutput].InputColumn;
				//smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtOutput] = sClearResult;
				if (smProductProduction->nCurrentProcessRejectTrayNo == 0)//reject unit
				{
					smProductProduction->OutputTableResult[0].RejectUnitPresent = 1;
					smProductProduction->nCurrentTotalRejectUnit++;
					smProductProduction->nCurrentLotNotGoodQuantity++;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)//reject unit
				{
					smProductProduction->OutputTableResult[0].OutputUnitPresent = 1;
				}
				nSequenceNo = nCase.IsPickAndPlaceModuleAwayAfterPlace;
				//smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set = false;
				//smProductEvent->RTHD_GMAIN_START_GET_OUTPUT_STATION.Set = true;
			}
			else if (smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set == true && smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set == false)
			{
				smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set = false;
				nSequenceNo = nCase.SetTableToBePlaced;
				break;
			}
			else if (smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set == true && smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set == true)
			{
				smProductEvent->RPNP_ROUT_NO_MOVE_TRAY_READY.Set = false;
				smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_START_GET_OUTPUT_STATION.Set = true;
				nSequenceNo = nCase.WaitOutputTakeInfoDone;
				break;
			}
			else if (smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set == true)
			{
				smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set = false;
				m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
				if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
				{
					if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
					else
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			break;

		case nCase.StartPreMove:
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsPreMoveDone;
			break;

		case nCase.IsPreMoveDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				smProductEvent->RPNP_ROUT_SET_OUTPUT_TRAY_BY_RESULT_DONE.Set = true;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move offset done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.SetTableToBePlaced;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopPreMoveDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("ManualPickAndPlaceSeq: Output Tray Table X Y Axis Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move offset Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopPreMove;
			}
			break;

		case nCase.StopPreMove:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopPreMoveDone;
			break;

		case nCase.IsStopPreMoveDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartPreMove;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopPreMove;
			}
			break;

		case nCase.IsPickAndPlaceModuleAwayAfterPlace:

			//smProductEvent->ROUT_RSEQ_OUTPUT_POST_VISION_DONE.Set = true;
			//smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
			if (smProductEvent->RSEQ_ROUT_PNP_AWAY_FROM_OUTPUT_STATION_DONE.Set == true)
			{
				//nSequenceNo = nCase.SetEventToUpdateReport;
				nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
				//nSequenceNo = nCase.ReadTableToBePlaced;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}

			break;

#pragma region Output Post
		case nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				smProductProduction->OutputTableResult[0].RejectColumn = smProductProduction->nRejectEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].RejectRow = smProductProduction->nRejectEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].RejectTrayNo = smProductProduction->nCurrentRejectTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			{
				smProductProduction->OutputTableResult[0].OutputColumn = smProductProduction->nOutputEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].OutputRow = smProductProduction->nOutputEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].OutputTrayNo = smProductProduction->nCurrentOutputTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START.Set = true;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
				{
					smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START.Set = true;
				}

			}
			else
			{
				if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
				{
					smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE.Set = true;
					smProductEvent->RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START.Set = false;
				}
				else if (smProductProduction->nCurrentProcessRejectTrayNo < 5)
				{
					smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE.Set = true;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START.Set = false;
				}
			}

			m_cLogger->WriteLog("OutputTrayTableSeq: Post Output Vision Start Send Row and Column.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set = false;
			smProductEvent->RTHD_GMAIN_START_GET_OUTPUT_STATION.Set = true;
			if (smProductProduction->nCurrentProcessRejectTrayNo == 5)
			{
				nSequenceNo = nCase.IsSendPostOutputVisionRowAndColumnDone;
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				nSequenceNo = nCase.IsSendPostOutputVisionRejectRowAndColumnDone;
			}

			break;

		case nCase.IsSendPostOutputVisionRowAndColumnDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				((smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false))
				)
			{
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Post Output Vision Receives Row and Column Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.DelayBeforePostOutputVisionSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_OUT_POST_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendPostOutputVisionRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterOutputPostReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else
				{
					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRCRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Post Output Vision receive column and row timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
				}				
			}
			break;

		case nCase.IsSendPostOutputVisionRejectRowAndColumnDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				((smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false))
				)
			{
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Reject %u Vision Receives Row and Column Done %ums.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo, lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.DelayBeforePostOutputVisionRejectSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);

			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{				
				if (smProductEvent->GMAIN_RTHD_REJECT_POST_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendPostOutputVisionRejectRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterRejectPostReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else
				{
					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRCRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
				}				
			}
			break;

		case nCase.DelayBeforePostOutputVisionSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.StartPostOutputVisionSOV;
			}
			break;

		case nCase.StartPostOutputVisionSOV:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			smProductProduction->OutputTableResult[0].OutputXOffset_um_Post = 0;
			smProductProduction->OutputTableResult[0].OutputYOffset_um_Post = 0;
			smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree_Post = 0;
			smProductProduction->OutputTableResult[0].OutputResult_Post = 0;
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_FAIL.Set = false;

			}
			smProductEvent->RTHD_GMAIN_GET_OUT_POST_VISION_XYTR_START.Set = true;

			//lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			//RtSleepFt(&lnDelayIn100ns);
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.DelayAfterPostOutputVisionSOV;
			break;

		case nCase.DelayAfterPostOutputVisionSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.IsPostOutputVisionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.IsPostOutputVisionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false)
				)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					//smProductProduction->OutputTableResult[0].OutputUnitPresent_Post = 0;
					smProductProduction->OutputTableResult[0].OutputResult_Post = 1;
					smProductProduction->OutputTableResult[0].OutputXOffset_um_Post = 0;
					smProductProduction->OutputTableResult[0].OutputYOffset_um_Post = 0;
					smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree_Post = 0;
				}
				nSequenceNo = nCase.CheckPostVisionResult;
				break;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6837);
				}
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums. Retry %lf\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount, smProductProduction->CurrentOutputVisionRetryCount);
				
				nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
			}
			break;

		case nCase.DelayBeforePostOutputVisionRejectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.StartPostOutputVisionRejectSOV;
			}
			break;

		case nCase.StartPostOutputVisionRejectSOV:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				smProductProduction->OutputTableResult[0].RejectXOffset_um_Post = 0;
				smProductProduction->OutputTableResult[0].RejectYOffset_um_Post = 0;
				smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree_Post = 0;
				smProductProduction->OutputTableResult[0].RejectResult_Post = 0;
			}
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_FAIL.Set = false;

			}
			smProductEvent->RTHD_GMAIN_GET_REJECT_POST_VISION_XYTR_START.Set = true;

			///*lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			//RtSleepFt(&lnDelayIn100ns);*/
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta for reject tray %u.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.DelayAfterPostOutputVisionRejectSOV;
			break;

		case nCase.DelayAfterPostOutputVisionRejectSOV:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;

			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductSetting->DelayAfterOutputVisionSnap_ms)
			{
				nSequenceNo = nCase.IsPostOutputVisionRejectDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.IsPostOutputVisionRejectDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->OutputTableResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{

				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta for reject tray %u done %ums.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo, lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
					{
						//smProductProduction->OutputTableResult[0].RejectUnitPresent_Post = 0;
						smProductProduction->OutputTableResult[0].RejectResult_Post = 1;
						smProductProduction->OutputTableResult[0].RejectXOffset_um_Post = 0;
						smProductProduction->OutputTableResult[0].RejectYOffset_um_Post = 0;
						smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree_Post = 0;
					}

				}
				nSequenceNo = nCase.CheckPostVisionResult;
				break;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cProductShareVariables->SetAlarm(6837);
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				///*m_cProductIOControl->SetOutputVisionSOV(false);
				//m_cProductShareVariables->SetAlarm(6803);*/

				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}

				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
			}
			break;
#pragma endregion

		case nCase.CheckPostVisionResult:
			smProductProduction->IsPostDone = true;
			smProductProduction->OutputTableResult[0].UnitPresent = 0;
			if (smProductProduction->nCurrentProcessRejectTrayNo == 5)//output
			{
				if (smProductProduction->OutputTableResult[0].OutputResult_Post == 1) //Pass
				{
					smProductProduction->IsMissingPostOutput = false;
					smProductProduction->nCurrentOutputUnitOnTray++;
					smProductProduction->OutputQuantity++;
					smProductProduction->nCurrentOutputQuantity++;
					//smProductProduction->nCurrentLowYieldAlarmQuantity++;
					smProductProduction->bIsOutputFirstUnit = false;
					////Output Quantity (WC)
					//if (smProductProduction->nCurrentOutputUnitOnTray >= smProductSetting->TotalOutputUnitQuantity && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
					//{
					//	smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set = true;
					//	smProductProduction->PendingLastUnitOutput = false;
					//	m_cLogger->WriteLog("OutputTrayTableSeq: Reach total Output quantity.\n");
					//}
					//if ((smProductProduction->nCurrentOutputUnitOnTray == (smProductSetting->TotalOutputUnitQuantity - 1)) && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
					//{
					//	smProductProduction->LastUnitOutput = true;
					//	smProductProduction->PendingLastUnitOutput = true;
					//}
					nSequenceNo = nCase.WaitOutputTakeInfoDone;
				}
				else if (smProductProduction->OutputTableResult[0].OutputResult_Post == 5) //Missing Unit
				{
					smProductProduction->PendingLastUnitOutput = false;
					smProductProduction->OutputTableResult[0].OutputUnitPresent = 0;
					smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
					smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
					//smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_POST_MISSING_UNIT.Set = true;
					//smProductProduction->OutputQuantity--;
					//smProductProduction->nCurrentOutputUnitOnTray--;
					//m_cProductShareVariables->SetAlarm(60305);
					smProductProduction->IsMissingPostOutput = true;
					nSequenceNo = nCase.AdjustReturnOffset;
					break;
				}
				else if (smProductProduction->OutputTableResult[0].OutputResult_Post == 8) //Unit Tilted
				{
					smProductProduction->PendingLastUnitOutput = false;
					smProductProduction->OutputTableResult[0].OutputUnitPresent = 1;
					smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
					//smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_POST_MISSING_UNIT.Set = true;
					//smProductProduction->OutputQuantity--;
					//smProductProduction->nCurrentOutputUnitOnTray--;
					m_cProductShareVariables->SetAlarm(60304);
					//smProductProduction->IsMissingPostOutput = true;
					nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
					break;
				}
			}
			else if (smProductProduction->nCurrentProcessRejectTrayNo == 0)//reject
			{
				smProductProduction->PendingLastUnitOutput = false;
				if (smProductProduction->OutputTableResult[0].RejectResult_Post == 1) //pass
				{
					smProductProduction->nCurrentRejectUnitOnTray++;
					smProductProduction->OutputQuantity++;
					smProductProduction->nCurrentRejectQuantity++;
					smProductProduction->nCurrentRejectQuantityBasedOnInputTray++;
					//smProductProduction->nCurrentLowYieldAlarmQuantity++;
					smProductProduction->IsMissingPostReject = false;
					smProductProduction->bIsRejectFirstUnit = false;
					nSequenceNo = nCase.WaitOutputTakeInfoDone;
				}
				else if (smProductProduction->OutputTableResult[0].RejectResult_Post == 5) //missing unit
				{
					smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
					//m_cProductShareVariables->SetAlarm(60311);
					smProductProduction->OutputTableResult[0].RejectUnitPresent = 0;
					smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
					//smProductEvent->RMAIN_RTHD_UPDATE_MAP_AFTER_REJECT_POST_MISSING_UNIT.Set = true;
					smProductProduction->IsMissingPostReject = true;
					nSequenceNo = nCase.AdjustReturnOffset;
					break;
				}
				else if (smProductProduction->OutputTableResult[0].RejectResult_Post == 8) //unit tilted
				{
					smProductEvent->ROUT_RSEQ_OUTPUT_TABLE_READY.Set = false;
					m_cProductShareVariables->SetAlarm(60310);
					//smProductProduction->IsMissingPostReject = true;
					nSequenceNo = nCase.StartSendPostOutputVisionOutputOrRejectRowAndColumn;
					break;
				}
			}

			//smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set = false;
			//smProductEvent->RTHD_GMAIN_START_GET_OUTPUT_STATION.Set = true;
			break;

		case nCase.WaitOutputTakeInfoDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set == true)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision get Result done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				nSequenceNo = nCase.IsUpdateReportDOne;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6807);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision get Result timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RTHD_GMAIN_GET_OUTPUT_STATION_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_START_GET_OUTPUT_STATION.Set = true;

				nSequenceNo = nCase.WaitOutputTakeInfoDone;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);

			}
			break;

		case nCase.IsUpdateReportDOne:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			//m_cLogger->WriteLog("OutputTrayTableSeq: Output Report updated successfully %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
			smProductEvent->ROUT_RSEQ_OUTPUT_UPDATE_EVENT_DONE.Set = true;
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductEvent->ROUT_RSEQ_START_POST_PRODUCTION.Set == true)
			{
				if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
				{
					if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
					else
						nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			else
			{
				nSequenceNo = nCase.ReadTableToBePlaced;
			}
			break;

#pragma region Unloading
		case nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition:
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				m_cProductShareVariables->SetAlarm(5511);
				m_cLogger->WriteLog("OutputTrayTableSeq: Tray not present on output tray table during unloading.\n");
				break;

			}
			else
			{
				if (m_cProductIOControl->IsOutputUnloadingStackerFullSensorOn() == true)
				{
					m_cProductShareVariables->SetAlarm(5413);
					m_cLogger->WriteLog("OutputTrayTableSeq: Tray full on output unloading stacker.\n");
					break;
				}
			}
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToUnloadingPositionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to Unloading position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPositionForUnloading;
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to Unloading position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to Unloading position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to Unloading position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToUnloadingPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToUnloadingPosition:
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

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToUnloadingPosition;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToSingulationPositionForUnloading:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToSingulationPositionForUnloadingDone;
			
			break;

		case nCase.IsMoveOutputTrayTableZAxisToSingulationPositionForUnloadingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true 
					&& smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Move to Singulation unload position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.OutputUnloadingStackerUnlockCylinderUnlockForUnloading;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionForUnloadingDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Motor Move to Singulation unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPositionForUnloading;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToSingulationPositionForUnloading:

			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionForUnloadingDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionForUnloadingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPositionForUnloading;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("Output Tray Table Z Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPositionForUnloading;
			}
			break;

		case nCase.OutputUnloadingStackerUnlockCylinderUnlockForUnloading:
			m_cProductIOControl->SetOutputUnloadingStackerUnlockCylinderOn(true);
			m_cProductIOControl->SetOutputTrayTableVacuumOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			//nSequenceNo = nCase.IsOutputUnloadingStackerUnlockCylinderUnlockForUnloading;
			nSequenceNo = nCase.IsSetOutputTableVacuumOffnDoneBeforeUnloadingPosition;
			break;

		case nCase.IsSetOutputTableVacuumOffnDoneBeforeUnloadingPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingOutputTableVacuumOnOffCompletelyBeforeNextStep_ms)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Table Vacuum Off Done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				nSequenceNo = nCase.IsOutputUnloadingStackerUnlockCylinderUnlockForUnloading;
			}
			break;

		case nCase.IsOutputUnloadingStackerUnlockCylinderUnlockForUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputUnloadingStackerUnlockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Unloading Stacker unlock cylinder done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToUnloadingPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputUnloadingStackerUnlockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5418);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Unloading Stacker unlock cylinder Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.OutputUnloadingStackerUnlockCylinderUnlockForUnloading;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToUnloadingPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveUnload.Set = true;
				//smProductEvent->OutputTrayTableXAxisSetOffsetDone.Set = false;
				//smProductEvent->StartOutputTrayTableXAxisSetOffset.Set = true;
				//smProductEvent->OutputTrayTableYAxisSetOffsetDone.Set = false;
				//smProductEvent->StartOutputTrayTableYAxisSetOffset.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToUnloadingPositionDone;
			break;

		case nCase.IsMoveOutputTrayTableZAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Move to Unloading position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.OutputUnloadingStackerUnlockCylinderLockForUnloading;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToUnloadingPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Motor Move to unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToUnloadingPosition;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToUnloadingPosition:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToUnloadingPositionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToUnloadingPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToUnloadingPosition;
			}
			break;

		case nCase.OutputUnloadingStackerUnlockCylinderLockForUnloading:
			m_cProductIOControl->SetOutputUnloadingStackerUnlockCylinderOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsOutputUnloadingStackerUnlockCylinderLockForUnloading;
			break;

		case nCase.IsOutputUnloadingStackerUnlockCylinderLockForUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Unloading Stacker lock cylinder done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPositionAfterUnloading;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputUnloadingStackerLockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5411);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Unloading Stacker lock cylinder Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.OutputUnloadingStackerUnlockCylinderLockForUnloading;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToDownPositionAfterUnloading:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorMoveDown.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToDownPositionDoneAfterUnloading;
			break;

		case nCase.IsMoveOutputTrayTableZAxisToDownPositionDoneAfterUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Move to down position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cProductShareVariables->SetAlarm(5506);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Present After Unloading Done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					break;
				}	
				
				if ((smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true || smProductEvent->SEQ_OUT_IS_UNLOADTRAY.Set == true || smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set == true /*|| smProductEvent->JobStop.Set == true*/) && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0
					&& smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;

				}
				else /*if (smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set == true)*/
				{
					smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDTRAY.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					//nSequenceNo = nCase.IsOutputLoadingStackerEmpty;
					nSequenceNo = nCase.IsSendVisionOutputEndTrayDone;
				}
				break;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDoneAfterUnloading;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("Output Tray Table Z Axis Motor Move to down position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPositionAfterUnloading;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToDownPositionAfterUnloading:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;

			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDoneAfterUnloading;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDoneAfterUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPositionAfterUnloading;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Z Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPositionAfterUnloading;
			}
			break;
#pragma endregion
#pragma region Manual Unload
		case nCase.StartMoveRejectTrayTableXYAxisToManualLoadUnloadPosition:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveRejectTrayTableXYAxisToManualLoadUnloadPositionDone;
			break;

		case nCase.IsMoveRejectTrayTableXYAxisToManualLoadUnloadPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table XY axis move to manual load unload position to fill reject tray done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					smProductEvent->ROUT_RTHD_REMOVE_AND_REPLACE_REJECT_TRAY_DONE.Set = false;
					smProductEvent->ROUT_RTHD_REMOVE_AND_REPLACE_REJECT_TRAY_START.Set = true;					
				}
				
				m_cProductIOControl->SetRejectTrayTableVacuumOn(false);
				nSequenceNo = nCase.IsOperatorReplaceRejectTrayTableDone;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveRejectTrayTableXYAxisToManualLoadUnloadPositionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table XY axis move to manual load unload position to fill reject tray timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X axis move to manual load unload position to fill reject tray timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Y axis move to manual load unload position to fill reject tray timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
				break;
			}
			break;

		case nCase.StopMoveRejectTrayTableXYAxisToManualLoadUnloadPosition:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveRejectTrayTableXYAxisToManualLoadUnloadPositionDone;
			break;

		case nCase.IsStopMoveRejectTrayTableXYAxisToManualLoadUnloadPositionDone:
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table XY axis motor stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table XY axis motor stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X axis stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table Y axis stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveRejectTrayTableXYAxisToManualLoadUnloadPosition;
				break;
			}
			break;

		case nCase.IsOperatorReplaceRejectTrayTableDone:
			if (smProductEvent->ROUT_RTHD_REMOVE_AND_REPLACE_REJECT_TRAY_DONE.Set == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				bolReLoadingRejectTray = true;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDTRAY.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendVisionRejectEndTrayDone;
				break;
			}
			else
			{
				m_cProductShareVariables->SetAlarm(5429);
				break;
			}
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			break;

		case nCase.IsSendVisionRejectEndTrayDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true
				&& ((smProductProduction->nCurrentProcessRejectTrayNo == 0 && smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set == true)

					)
				) || smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
			{
				if (bolReLoadingRejectTray == true)
				{
					//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
					//{
					//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
					//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
					//	{
					//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
					//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
					//		else
					//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
					//	}
					//	else
					//	{
					//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
					//	}
					//	break;
					//}
					//else
					{
						/*smProductProduction->nRejectEdgeCoordinateX = 1;
						smProductProduction->nCurrentRejectUnitOnTray = 0;
						smProductProduction->nRejectEdgeCoordinateY = 1;
						smProductProduction->nCurrentRejectTrayNo++;
						smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
						RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);*/
						nSequenceNo = nCase.IsOperatorFillRejectTray;

						break;
					}
				}
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6857);
				if (bolReLoadingRejectTray == true)
				{
					smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDTRAY.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					break;
				}
			}
			break;

		case nCase.IsOperatorFillRejectTray:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == false && TemporaryDisable == false)
			{
				m_cProductShareVariables->SetAlarm(5512);
				m_cLogger->WriteLog("Tray not present on reject tray table.\n");
				break;
			}
			if (TemporaryDisable == true || (TemporaryDisable == false && m_cProductIOControl->IsRejectTrayPresentSensorOn() == true))
			{

				//nSequenceNo = nCase.IsRejectTrayTableReady;
				smProductProduction->nCurrentRejectUnitOnTray = 0;
				smProductProduction->nRejectEdgeCoordinateX = 1;
				smProductProduction->nRejectEdgeCoordinateY = 1;
				smProductProduction->nCurrentRejectTrayNo++;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendVisionRejectTrayNo1Done;
				break;
			}
			break;

		case nCase.IsSendVisionRejectTrayNo1Done:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true
				&& ((bolReLoadingRejectTray == true && smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set == true)
					))
				|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
			{
				//if (smProductSetting->EnableScanBarcodeOnOutputTray == true)
				//{
				//	nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPositionForBarcodeScan;
				//}
				//else
				{
					nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPositionDuringProduction;
				}
				break;
				//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPosition;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6835);
				if (bolReLoadingRejectTray == true)
				{
					smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					break;
				}

			}
			break;
#pragma endregion

		case nCase.IsSendVisionOutputEndTrayDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true
				&& smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set == true)
				|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
			{
				{

					nSequenceNo = nCase.IsOutputLoadingStackerEmpty;

				}
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6857);
				{
					smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDTRAY.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				break;
			}
			break;

		case nCase.IsOutputLoadingStackerEmpty:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0)
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputLoadingStackerPresentSensorOn() == true && m_cProductIOControl->IsRejectTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction;
			}
			else
			{
				if (m_cProductIOControl->IsOutputLoadingStackerPresentSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5410);
					m_cLogger->WriteLog("OutputTrayTableseq: Tray not present on output loading stacker.\n");
				}
				else if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5408);
					m_cLogger->WriteLog("OutputTrayTableseq: Reject Tray not present on output loading.\n");
				}
				break;
			}
			break;

		case nCase.StartMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0)
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMoveLoad.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMoveLoad.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToLoadingPositionDuringProductionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToLoadingPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis Motor move load position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cProductShareVariables->SetAlarm(5506);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Before Loading Tray %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					break;
				}

				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToLoadingPositionDuringProduction;
			}
			else if (IsReadyToMoveProduction() == false)
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

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis move load position Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Axis Motor Move Load timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Y Axis Motor Move Load timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToLoadingPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Y Axis stop Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table X Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Y Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToLoadingPositionDuringProduction;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToLoadingPositionDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveLoad.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToLoadingPositionDuringProductionDone;
			}
			break;

		case nCase.IsMoveOutputTrayTableZAxisToLoadingPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move loading position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoadingDuringPorduction;
			}
			else if (IsReadyToMoveProduction() == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}

				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move Loading timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToLoadingPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToLoadingPositionDuringProduction:

			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToLoadingPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToLoadingPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToLoadingPositionDuringProduction;
			}
			break;

		case nCase.CheckTrayPresentSensorBeforeLoadingDuringPorduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Checking before loading during production done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderUnlockDuringProduction;
				}
				else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
						m_cProductShareVariables->SetAlarm(5511);
						m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoadingDuringPorduction;
				}
			}
			break;

		case nCase.OutputLoadingStackerUnlockCylinderUnlockDuringProduction:
			m_cProductIOControl->SetOutputLoadingStackerUnlockCylinderOn(true);

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsOutputLoadingStackerUnlockCylinderUnlockDuringProduction;
			break;

		case nCase.IsOutputLoadingStackerUnlockCylinderUnlockDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputLoadingStackerUnlockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder Unlock done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputLoadingStackerUnlockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5412);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder Unlock Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderUnlockDuringProduction;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisToSingulationPositionDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveSingulation.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToSingulationPositionDuringProductionDone;
			}
			break;

		case nCase.IsMoveOutputTrayTableZAxisToSingulationPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true 
					&& smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move singulation position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPositionDuringProduction;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move Singulation timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToSingulationPositionDuringProduction:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToSingulationPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToSingulationPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToSingulationPositionDuringProduction;
			}
			break;

		case nCase.CheckTrayPresentSensorAfterMoveToSingulationPositionDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true
					)
				{
					smProductEvent->OutputTrayTableZAxisMotorChangeSlowSpeedDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorChangeSlowSpeed.Set = true;
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Present Checking After Move To Singulation Position during production done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderLockDuringProduction;
				}
				else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
						m_cProductShareVariables->SetAlarm(5511);
						m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPositionDuringProduction;
				}
			}
			break;


		case nCase.OutputLoadingStackerUnlockCylinderLockDuringProduction:
			if (smProductEvent->OutputTrayTableZAxisMotorChangeSlowSpeedDone.Set == true)
			{
				m_cProductIOControl->SetOutputLoadingStackerUnlockCylinderOn(false);

				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsOutputLoadingStackerUnlockCylinderLockDuringProduction;
			}
			break;

		case nCase.IsOutputLoadingStackerUnlockCylinderLockDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Loading Stacker Cylinder lock done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsOutputLoadingStackerLockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5411);
					m_cLogger->WriteLog("OutputTrayTableseq:  Output Loading Stacker Cylinder lock Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.OutputLoadingStackerUnlockCylinderLockDuringProduction;
			}
			break;

		case nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction:

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveSecondSingulationDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveSecondSingulation.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProductionDone;
			}
			break;

		case nCase.IsMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSecondSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move second singulation position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsTrayAvailableAfterSecondSingulationPositionDuringProduction;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Input Tray Table Z Axis Motor Move Second Singulation timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisTo_SecondSingulationPositionDuringProduction;
			}
			break;

		case nCase.IsTrayAvailableAfterSecondSingulationPositionDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > smProductSetting->DelayForCheckingInputOutputTableZAtSecondSigulationTrayAvalable_ms)
			{
				if ((m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false || m_cProductIOControl->IsOutputTrayTiltSensorOn() == false) && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					m_cProductShareVariables->SetAlarm(5511);
					//nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent;
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				}
				else
				{
					smProductEvent->OutputTrayTableZAxisMotorChangeNormalSpeedDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorChangeNormalSpeed.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPositionDuringProduction;
				}
			}
			break;


		case nCase.StartMoveOutputTrayTableZAxisToDownPositionDuringProduction:
			if (smProductEvent->OutputTrayTableZAxisMotorChangeNormalSpeedDone.Set == true)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorMoveDown.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveOutputTrayTableZAxisToDownPositionDuringProductionDone;
			}
			break;

		case nCase.IsMoveOutputTrayTableZAxisToDownPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				m_cProductShareVariables->SetAlarm(5511);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresentDuringProduction;
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				break;
			}
			if (true
				&& smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor move down position done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RMAIN_RTHD_OUTPUT_FULL.Set = false;
				smProductProduction->nOutputEdgeCoordinateX = 1;
				smProductProduction->nOutputEdgeCoordinateY = 1;
				smProductProduction->nCurrentOutputTrayNo++;
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;

				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendOutputVisionTrayNoDoneDuringProduction;

			}
			else if (IsReadyToMoveProduction() == false)
			{

				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableseq: Door Get Trigger %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47002);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Move down timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableZAxisToDownPositionDuringProduction:
			if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
			{
				smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
			}

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableZAxisToDownPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableZAxisToDownPositionDuringProduction;
			}
			break;



		case nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresentDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableZAxisMotor == false || (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor stop done %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1s);
				nSequenceNo = nCase.CheckIsTrayPresentOnOutputTrayTableDuringDownDuringProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis stop Timeout %ums.", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true && smProductEvent->OutputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(47008);
					m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table Z Axis Motor Stop timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableZAxisMotor == true)
				{
					smProductEvent->OutputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartOutputTrayTableZAxisMotorStop.Set = true;
				}
				nSequenceNo = nCase.IsStopMoveOutputTrayTableZAxisToDownPositionAfterTrayNotPresentDuringProduction;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.CheckIsTrayPresentOnOutputTrayTableDuringDownDuringProduction:
			if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				//if()if time >preset,then
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
				lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
				if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductProduction->TrayPresentSensorOffTimeBeforeAlarm_ms)
				{
					m_cProductShareVariables->SetAlarm(5511);
					m_cLogger->WriteLog("OutputLoadingseq: Output Tray Table Tray Not Present %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StopMoveInputTrayTableZAxisDownAtLoading;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
					break;
				}
			}
			else if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)

			{
				nSequenceNo = nCase.StartMoveOutputTrayTableZAxisToDownPositionDuringProduction;
			}
			break;

		case nCase.IsSendOutputVisionTrayNoDoneDuringProduction:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableOutputVision == true) || smProductSetting->EnableOutputVision == false)
				) || smProductSetting->EnableVision == false)
			{
				bolReLoadingOutputTray = true;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPositionDuringProduction;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6835);
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				break;
			}
			break;

		case nCase.StartSendOutputVisionOutputOrRejectRowAndColumnBeforeToFirstPositionDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (bolReLoadingRejectTray == true)
			{

				smProductProduction->OutputTableResult[0].RejectColumn = smProductProduction->nRejectEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].RejectRow = smProductProduction->nRejectEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].RejectTrayNo = smProductProduction->nCurrentRejectTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
			else if (bolReLoadingOutputTray == true)
			{

				smProductProduction->OutputTableResult[0].OutputColumn = smProductProduction->nOutputEdgeCoordinateY;
				smProductProduction->OutputTableResult[0].OutputRow = smProductProduction->nOutputEdgeCoordinateX;
				smProductProduction->OutputTableResult[0].OutputTrayNo = smProductProduction->nCurrentOutputTrayNo;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				if (bolReLoadingOutputTray == true)
				{
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
				}
				else if (bolReLoadingRejectTray == true)
				{
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
				}

			}
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Send Row and Column before to first position.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPositionDuringProduction;
			break;

		case nCase.StartMoveOutputTrayTableXYAxisToFirstPositionDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (bolReLoadingRejectTray == true) // REJECT TRAY 5
			{
				//first position

				smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtRejectTrayCenterPosition - signed long(smProductSetting->DeviceXPitchOutput * ((double)(smProductSetting->NoOfDeviceInRowOutput - 1) / 2));
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtRejectTrayCenterPosition - signed long(smProductSetting->DeviceYPitchOutput * ((double)(smProductSetting->NoOfDeviceInColOutput - 1) / 2));
				smProductProduction->RejectTrayTableXIndexPosition = smProductProduction->OutputTrayTableXAxisMovePosition;
				smProductProduction->RejectTrayTableYIndexPosition = smProductProduction->OutputTrayTableYAxisMovePosition;
			}
			else if (bolReLoadingOutputTray == true)// OUTPUT TRAY TABLE
			{
				//first position

				smProductProduction->OutputTrayTableXAxisMovePosition = smProductTeachPoint->OutputTrayTableXAxisAtOutputTrayTableCenterPosition - signed long(smProductSetting->DeviceXPitchOutput * ((double)(smProductSetting->NoOfDeviceInRowOutput - 1) / 2));
				smProductProduction->OutputTrayTableYAxisMovePosition = smProductTeachPoint->OutputTrayTableYAxisAtOutputTrayTableCenterPosition - signed long(smProductSetting->DeviceYPitchOutput * ((double)(smProductSetting->NoOfDeviceInColOutput - 1) / 2));
				smProductProduction->OutputTrayTableXIndexPosition = smProductProduction->OutputTrayTableXAxisMovePosition;
				smProductProduction->OutputTrayTableYIndexPosition = smProductProduction->OutputTrayTableYAxisMovePosition;

			}
			if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true)
			{
				smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true)
			{
				smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartOutputTrayTableYAxisMotorMove.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToFirstPositionDuringProductionDone;
			break;


		case nCase.IsMoveOutputTrayTableXYAxisToFirstPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to first position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				{
					nSequenceNo = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDuringProductionDone;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDuringProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to first position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToFirstPositionDuringProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToFirstPositionDuringProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDuringProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToFirstPositionDuringProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToFirstPositionDuringProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToFirstPositionDuringProduction;
			}
			break;

		case nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDuringProductionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				(((smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set == true && bolReLoadingRejectTray)
					|| (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set == true && bolReLoadingOutputTray))
					&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{
				smProductProduction->CurrentOutputVisionRCRetryCount = 0;
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Receives Row and Column Done after to assigned position %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (bolReLoadingOutputTray == true)
					nSequenceNo = nCase.StartOutputVisionSOVDuringProduction;
				else
					nSequenceNo = nCase.StartOutputVisionRejectSOVDuringProduction;

			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDuringProductionDone;
					nSequenceNo = nCase.DelayAfterOutputReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendOutputVisionOutputOrRejectRowAndColumnBeforeToAssignedPositionDuringProductionDone;
					nSequenceNo = nCase.DelayAfterRejectReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				}
				else if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
				{					

					if (smProductProduction->CurrentOutputVisionRCRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRCRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6807);
					}
					m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision receive column and row after to assigned position timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (bolReLoadingRejectTray == true)
					{
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_OUT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_OUT_VISION_RC_START.Set = true;
					}
					else if (bolReLoadingOutputTray == true)
					{
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_NAK.Set = false;
						smProductEvent->GMAIN_RTHD_REJECT_VISION_GET_RC_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_SEND_REJECT_VISION_RC_START.Set = true;
					}

				}
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.StartOutputVisionSOVDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			smProductProduction->OutputTableResult[0].OutputXOffset_um = 0;
			smProductProduction->OutputTableResult[0].OutputYOffset_um = 0;
			smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree = 0;
			smProductProduction->OutputTableResult[0].OutputResult = 0;
			smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 5;
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set = false;

			}
			smProductEvent->RTHD_GMAIN_GET_OUT_VISION_XYTR_START.Set = true;

			lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			RtSleepFt(&lnDelayIn100ns);
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsOutputVisionDuringProductionDone;
			break;

		case nCase.IsOutputVisionDuringProductionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false)
				)
			{

				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->OutputTableResult[0].OutputUnitPresent = 0;
					smProductProduction->OutputTableResult[0].OutputResult = 1;
					smProductProduction->OutputTableResult[0].OutputXOffset_um = 0;
					smProductProduction->OutputTableResult[0].OutputYOffset_um = 0;
					smProductProduction->OutputTableResult[0].OutputThetaOffset_mDegree = 0;
				}
				nSequenceNo = nCase.SetOutputPreProductionPositionDuringProduction;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_OUT_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cProductShareVariables->SetAlarm(6837);
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				//*m_cProductIOControl->SetOutputVisionSOV(false);
				//m_cProductShareVariables->SetAlarm(6803);*/
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			break;

		case nCase.StartOutputVisionRejectSOVDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (m_cProductIOControl->IsOutputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6801);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsOutputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				m_cProductShareVariables->SetAlarm(6802);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision busy.\n");
				break;
			}
#pragma region Offset
			if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
			{
				smProductProduction->OutputTableResult[0].RejectXOffset_um = 0;
				smProductProduction->OutputTableResult[0].RejectYOffset_um = 0;
				smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree = 0;
				smProductProduction->OutputTableResult[0].RejectResult = 0;
				smProductProduction->OutputTableResult[0].CurrentOutputTableNo = 0;
			}
#pragma endregion
			if (smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule)
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set = false;

			}
			smProductEvent->RTHD_GMAIN_GET_REJECT_VISION_XYTR_START.Set = true;

			lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeOutputVisionSnap_ms;
			RtSleepFt(&lnDelayIn100ns);
			m_cProductIOControl->SetOutputVisionSOV(true);
			m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Start Get XY Theta for reject tray %u.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo);
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsOutputVisionRejectDuringProductionDone;
			break;

		case nCase.IsOutputVisionRejectDuringProductionDone:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
			{

				smProductProduction->CurrentOutputVisionRetryCount = 0;
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Vision Get XY Theta for reject tray %u done %ums.\n", 5 - smProductProduction->nCurrentProcessRejectTrayNo, lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					if (smProductProduction->nCurrentProcessRejectTrayNo == 0)
					{
						smProductProduction->OutputTableResult[0].RejectUnitPresent = 0;
						smProductProduction->OutputTableResult[0].RejectResult = 1;
						smProductProduction->OutputTableResult[0].RejectXOffset_um = 0;
						smProductProduction->OutputTableResult[0].RejectYOffset_um = 0;
						smProductProduction->OutputTableResult[0].RejectThetaOffset_mDegree = 0;
					}

				}
				nSequenceNo = nCase.SetOutputPreProductionPositionDuringProduction;
			}
			else if (m_cProductIOControl->IsOutputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_REJECT_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableVision == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true)
			{
				m_cProductIOControl->SetOutputVisionSOV(false);
				m_cProductShareVariables->SetAlarm(6837);
				m_cLogger->WriteLog("OutputVisionseq: Output Vision get XY Theta fail %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				//nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > ((long)2000))
			{
				///*m_cProductIOControl->SetOutputVisionSOV(false);
				//m_cProductShareVariables->SetAlarm(6803);*/
				m_cProductIOControl->SetOutputVisionSOV(false);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

				if (smProductProduction->CurrentOutputVisionRetryCount < 3)
				{
					smProductProduction->CurrentOutputVisionRetryCount++;
				}
				else
				{
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					m_cProductShareVariables->SetAlarm(6803);
				}
				m_cLogger->WriteLog("OutputTrayTableseq: Output Vision get XY Theta timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;
				//nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
				nSequenceNo = nCase.StartSendOutputVisionOutputOrRejectRowAndColumn;
			}
			break;

		case nCase.SetOutputPreProductionPositionDuringProduction:
			//if (smProductEvent->JobStop.Set == true && smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0)
			//{
			//	m_cLogger->WriteLog("OutputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsOutputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		if ((m_cProductIOControl->IsRejectTrayPresentSensorOn() == true && smProductProduction->nCurrentRejectUnitOnTray > 0))
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			//		else
			//			nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			if (bolReLoadingRejectTray == true)//reject
			{
				bolReLoadingRejectTray = false;
				if (smProductProduction->OutputTableResult[0].RejectResult == 1)
				{
				
					smProductProduction->CurrentOutputVisionRetryCount = 0;
					smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = false;
					smProductProduction->RejectTrayTableCurrentXPosition = smProductProduction->OutputTrayTableXAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].RejectXOffset_um);
					smProductProduction->RejectTrayTableCurrentYPosition = smProductProduction->OutputTrayTableYAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].RejectYOffset_um);
					smProductProduction->RejectTrayTableXIndexPosition = smProductProduction->RejectTrayTableCurrentXPosition;
					smProductProduction->RejectTrayTableYIndexPosition = smProductProduction->RejectTrayTableCurrentYPosition;
					smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->RejectTrayTableCurrentXPosition;
					smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->RejectTrayTableCurrentYPosition;
					nSequenceNo = nCase.SetTableToBePlaced;

					//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToPreProductionPosition;
				}
				else if (smProductProduction->OutputTableResult[0].RejectResult == 5)
				{
					m_cProductIOControl->SetOutputVisionSOV(false);
					smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

					if (smProductProduction->CurrentOutputVisionRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6803);
					}
					nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
					//nSequenceNo = nCase.SetTableToBePlaced;
				}
			}
			else if (bolReLoadingOutputTray == true)//output
			{
				bolReLoadingOutputTray = false;
				if (smProductProduction->OutputTableResult[0].OutputResult == 1)
				{
					smProductEvent->RMAIN_RTHD_OUTPUT_OR_REJECT_FULL.Set = false;
					smProductProduction->OutputTrayTableCurrentXPosition = smProductProduction->OutputTrayTableXAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].OutputXOffset_um);
					smProductProduction->OutputTrayTableCurrentYPosition = smProductProduction->OutputTrayTableYAxisMovePosition - (signed long)(smProductProduction->OutputTableResult[0].OutputYOffset_um);
					smProductProduction->OutputTrayTableXIndexPosition = smProductProduction->OutputTrayTableCurrentXPosition;
					smProductProduction->OutputTrayTableYIndexPosition = smProductProduction->OutputTrayTableCurrentYPosition;
					smProductProduction->OutputTrayTableXAxisMovePosition = smProductProduction->OutputTrayTableCurrentXPosition;
					smProductProduction->OutputTrayTableYAxisMovePosition = smProductProduction->OutputTrayTableCurrentYPosition;
					nSequenceNo = nCase.SetTableToBePlaced;

					//nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToPreProductionPosition;
				}
				else if (smProductProduction->OutputTableResult[0].OutputResult == 5)
				{
					m_cProductIOControl->SetOutputVisionSOV(false);
					smProductEvent->ROUTV_GMAIN_OUT_VISION_RESET_EOV.Set = true;

					if (smProductProduction->CurrentOutputVisionRetryCount < 3)
					{
						smProductProduction->CurrentOutputVisionRetryCount++;
					}
					else
					{
						smProductProduction->CurrentOutputVisionRetryCount = 0;
						m_cProductShareVariables->SetAlarm(6803);
					}
					nSequenceNo = nCase.StartRetrySendOutputVisionOutputOrRejectRowAndColumn;
					//nSequenceNo = nCase.SetTableToBePlaced;
				}
			}
			break;


		case nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProductionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to Unloading position done during post production %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDTRAY.Set = true;
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to Unloading position during post production Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to Unloading position during post production Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to Unloading position during post production Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction:
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

			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop done during post production %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis stop Timeout during post production %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor stop Timeout during post production %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor stop Timeout during post production %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToUnloadingPositionDuringPostProduction;
			}
			break;

		case nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProductionDone;
			break;

		case nCase.IsMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to manual load unload position done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				{
					nSequenceNo = nCase.IsRejectTrayRemovedPostProduction;
				}
			}
			else if (IsReadyToMoveProduction() == false)
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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("OutputTrayTableSeq: Door Triggered %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProductionDone;
				break;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X Y Axis Move to manual load unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveManualLoadUnloadDone.Set == false)
				//{
				//	m_cProductShareVariables->SetAlarm(45002);
				//	m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to manual load unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//}
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45002);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor Move to manual load unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorMoveManualLoadUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46002);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor Move to manual load unload position Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			}
			break;

		case nCase.StopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProductionDone;
			break;

		case nCase.IsStopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
			lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableOutputTrayTableXAxisMotor == false || (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableOutputTrayTableYAxisMotor == false || (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X and Y Axis stop done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			}
			else if (lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("OutputTrayTableSeq: Output Tray Table X and Y Axis stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableOutputTrayTableXAxisMotor == true && smProductEvent->OutputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(45008);
					m_cLogger->WriteLog("Output Tray Table X Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableOutputTrayTableYAxisMotor == true && smProductEvent->OutputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(46008);
					m_cLogger->WriteLog("Output Tray Table Y Axis Motor stop Timeout %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveOutputTrayTableXYAxisToManualLoadUnloadPositionPostProduction;
			}
			break;

		case nCase.IsRejectTrayRemovedPostProduction:
			if (m_cProductIOControl->IsRejectTrayPresentSensorOn() == true)
			{
				m_cProductIOControl->SetRejectTrayTableVacuumOn(false);
				m_cProductShareVariables->SetAlarm(5526);
				smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_DONE.Set = false;
				smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_START.Set = true;
			}
			//else
			//{
			//	smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set = true;
			//	nSequenceNo = nCase.EndOfSequence;
			//	//smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_DONE.Set = true;
			//	//smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_START.Set = false;
			//}
			if (smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_DONE.Set == true)
			{
				smProductEvent->ROUT_RSEQ_POST_PRODUCTION_DONE.Set = true;
				smProductEvent->SEQ_OUT_IS_UNLOADTRAY.Set = false;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDTRAY.Set = true;
				nSequenceNo = nCase.EndOfSequence;
			}
			else
			{
				if (smProductEvent->ROUT_RTHD_REMOVE_REJECT_TRAY_DONE.Set == false)
				{
					m_cProductIOControl->SetRejectTrayTableVacuumOn(false);
					m_cProductShareVariables->SetAlarm(5526);
				}
			}
			break;



		default:
			return -1;
			m_cLogger->WriteLog("OutputTrayTableSeq: return -1.\n");
			break;
		}
		//End of sequence
		if (nSequenceNo == 999)
		{
			nSequenceNo = 0;
		}
		RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
	}
	RtGetClockTime(CLOCK_FASTEST, &lnOutputTrayTableSequenceClockEnd);
	lnOutputTrayTableSequenceClockSpan.QuadPart = lnOutputTrayTableSequenceClockEnd.QuadPart - lnOutputTrayTableSequenceClockStart.QuadPart;
	m_cLogger->WriteLog("OutputTrayTableseq: Output Tray Table sequence done %ums.\n", lnOutputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	return 0;
}