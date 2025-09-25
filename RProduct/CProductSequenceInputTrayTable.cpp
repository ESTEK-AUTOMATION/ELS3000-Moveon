#include "CProductSequence.h"
#include <vector>

int CProductSequence::InputTrayTableSequence()
{
	int nError = 0;
	InputTrayTableSequenceNo nCase;
	int nSequenceNo = nCase.WaitingToReceiveEventStartInputTrayTableSequence;
	int nSequenceNo_Cont = 999;
	LARGE_INTEGER lnInputTrayTableSequenceClockStart, lnInputTrayTableSequenceClockEnd, lnInputTrayTableSequenceClockSpan, lnInputTrayTableSequenceClockStart2, lnInputTrayTableSequenceClockSpan4, lnInputTrayTableSequenceClockStart4, lnDelayIn100ns;
	LONGLONG  lConvert1msTo100ns = 10000;
	int InputVisionSequence;
	int nCurrentS1Vision, nCurrentInputVision;
	int nCurrentInputResultCounter, nCurrentS2ResultCounter;
	int nTotalInputVisionSnap, nTotalS2VisionSnap, nCurrentInputVisionDone, nCurrentS2VisionDone;
	int nCurrentEmptyCount;
	int PurgingCounter;
	bool bPreProductioning;
	bool bIsInputVisionRequireAdditionalMoveAndSnap = false;
	bool IsInputTableVacuumOn;
	bool IsLastUnitWithoutPicking = false;
	m_cLogger->WriteLog("InputTrayTableseq: Start Input tray table sequence\n");
	RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart);

	while (smProductEvent->ExitRTX.Set == false)
	{
		if (smProductEvent->GGUI_RSEQ_CHECK_SEQUENCE.Set == true)
		{
			m_cLogger->WriteLog("InputTrayTableseq: %u\n", nSequenceNo);
		}

		if (smProductEvent->GPCS_RSEQ_ABORT.Set == true)
		{
			m_cLogger->WriteLog("InputTrayTableseqAbort: %u\n", nSequenceNo);
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

		case nCase.WaitingToReceiveEventStartInputTrayTableSequence:
			//event not sure
			if (smProductEvent->RSEQ_RINT_SEQUENCE_START.Set == true)
			{
				smProductEvent->RSEQ_RINT_SEQUENCE_START.Set = false;
				if (smProductProduction->bInputContinue == false)
				{
					//smProductProduction->nCurrentInputTrayNo = 0;
					smProductProduction->nCurrentInputLotQuantityRun = 0;
					smProductProduction->nCurrentInputLotTrayNoRun = 0;
					smProductProduction->nTotalInputUnitDone = 0;
				}
				bPreProductioning = true;
				IsInputTableVacuumOn = false;
				nCurrentEmptyCount = 0; 
				smProductProduction->bolIsLastUnitTo1EndTray = false;
				smProductProduction->bolIsLastUnitTo2EndTray = false;
				//smProductProduction->nEdgeCoordinateX = 1;
				//smProductProduction->nEdgeCoordinateY = 1;
				InputVisionSequence = 1;
				nCurrentInputVision = 0;
				//nCurrentS1Vision = 0;
				PurgingCounter = 0;
				m_cLogger->WriteLog("InputTrayTableseq: Come in.\n");
				m_cLogger->WriteLog("InputTrayTableseq: Input Lot Quantity = %u.\n",smProductProduction->nInputLotQuantity);
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray No Quantity = %u.\n",smProductProduction->nInputLotTrayNo);
				nSequenceNo = nCase.IsInputTrayTableZAxisAtDownPosition;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart);
			}
			break;

		case nCase.IsInputTrayTableZAxisAtDownPosition:
			if (m_cProductMotorControl->IsInputTrayTableXAxisMotorSafeToMove() == false || m_cProductMotorControl->IsInputTrayTableYAxisMotorSafeToMove() == false)
			{
				m_cProductShareVariables->SetAlarm(43010);
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Not At Down Position");
				break;
			}
			else
			{
				nSequenceNo = nCase.IsInputTrayTableReady;
			}
			if (smProductEvent->JobStop.Set == true)
			{
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set = true;
				nSequenceNo = nCase.EndOfSequence;
				break;
			}
			break;

		case nCase.IsInputTrayTableReady:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				if (smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set == false)
				{
					if (m_cProductIOControl->IsInputLoadingStackerPresentSensorOn() == true)
					{
						if (smProductSetting->EnableInputTableVacuum == true)
						{
							nSequenceNo = nCase.StartSetInputTableVacuumOn;
						}
						else
						{
							nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPosition;
						}
					}
					else
					{
						m_cProductShareVariables->SetAlarm(5402);
						m_cLogger->WriteLog("InputTrayTableseq: Tray not present on input loading stacker.\n");
						break;
					}
				}
				else
				{
					m_cProductShareVariables->SetAlarm(5510);
					m_cLogger->WriteLog("InputTrayTableSeq: Sensor malfunction or software sequence problem.\n");
				}
			}
			else if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set = true;
				smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = false;
				smProductProduction->nPreviousInputTrayNo = smProductProduction->nCurrentInputTrayNo;
				smProductProduction->nCurrentInputTrayNo++;
				smProductProduction->nCurrentInputUnitOnTray = 0;
				smProductProduction->nCurrentRejectQuantityBasedOnInputTray = 0;
				if (smProductEvent->RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH.Set == false && smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set==false)
				{
					smProductProduction->nEdgeCoordinateX = 1;
					smProductProduction->nEdgeCoordinateY = 1;
				}
				if (smProductEvent->RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH.Set==true)
				{
					smProductEvent->RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH.Set = false;
				}
				if (smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set == true)
				{
					smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set = false;
				}
				nCurrentEmptyCount = 0;
				smProductProduction->nInputRunningState = 1;
				smProductEvent->GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_INPUT_SEND_TRAYNO.Set = true;
				
				//smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
				//smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
				if (bPreProductioning == true)
				{
					smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set = false;
					smProductProduction->nCurrentInputTrayNumberAtBottom = 1;
					smProductProduction->nCurrentInputTrayNumberAtS2S3 = 1;
					smProductProduction->nCurrentInputTrayNumberAtOutput = 1;
					smProductProduction->nPNPRunningState = 1;
					smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_BTM_SEND_TRAYNO.Set = true;
					smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
					smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S1_SEND_TRAYNO.Set = true;
					smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S3_SEND_TRAYNO.Set = true;
				}
				else 
				{
					
					//smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set = false;
					//smProductEvent->RTHD_GMAIN_BTM_SEND_TRAYNO.Set = true;
					//smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
					//smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
					//smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set = false;
					//smProductEvent->RTHD_GMAIN_S1_SEND_TRAYNO.Set = true;
					//smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set = false;
					//smProductEvent->RTHD_GMAIN_S3_SEND_TRAYNO.Set = true;
				}
				
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendVisionInputTrayNoDone;
			}
			break;
#pragma region Loading
		case nCase.StartSetInputTableVacuumOn:
			m_cProductIOControl->SetInputTrayTableVacuumOn(true);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsSetInputTableVacuumOnDone;
			break;
		case nCase.IsSetInputTableVacuumOnDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Table Vacuum On Done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				IsInputTableVacuumOn = true;
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPosition;
			}
			break;
		case nCase.StartMoveInputTrayTableXYAxisToLoadingPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
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
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableXYAxisToLoadingPositionDone;
			break;
		case nCase.IsMoveInputTrayTableXYAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Y Axis Motor move load position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (true
					&& m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cProductShareVariables->SetAlarm(5505);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Present Before Loading Tray %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					break;
				}
				
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToLoadingPosition;
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
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableseq: Door Get Trigger %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis move load position Timeout %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Axis Motor Move Loading position timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Y Axis Motor Move Loading position timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToLoadingPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableXYAxisToLoadingPosition:
			
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
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Y Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Y Axis stop Timeout %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Y Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToLoadingPosition;
			}
			break;
		case nCase.StartMoveInputTrayTableZAxisToLoadingPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorMoveLoadDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorMoveLoad.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveInputTrayTableZAxisToLoadingPositionDone;
			}
			break;
		case nCase.IsMoveInputTrayTableZAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveLoadDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor move loading position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoading;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableseq: Door Get Trigger %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToLoadingPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Move Loading timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToLoadingPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToLoadingPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToLoadingPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToLoadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToLoadingPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToLoadingPosition;
			}
			break;

		case nCase.CheckTrayPresentSensorBeforeLoading:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Present Checking before loading done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.InputLoadingStackerUnlockCylinderUnlock;
				}
				else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockSpan);
						m_cProductShareVariables->SetAlarm(5510);
						m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Tray Not Present %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorBeforeLoading;
				}
			}
			break;

		case nCase.InputLoadingStackerUnlockCylinderUnlock:
			m_cProductIOControl->SetInputLoadingStackerUnlockCylinderOn(true);
			if (smProductSetting->EnableInputTableVacuum == true)
			{
				m_cProductIOControl->SetInputTrayTableVacuumOn(true);
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsInputLoadingStackerUnlockCylinderUnlock;
			break;

		case nCase.IsInputLoadingStackerUnlockCylinderUnlock:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsInputLoadingStackerUnlockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Loading Stacker Cylinder Unlock done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToSingulationPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsInputLoadingStackerUnlockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5404);
					m_cLogger->WriteLog("InputTrayTableseq: Input Loading Stacker Cylinder Unlock Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.InputLoadingStackerUnlockCylinderUnlock;
			}
			break;

		case nCase.StartMoveInputTrayTableZAxisToSingulationPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableZAxisToSingulationPositionDone;
			break;
		case nCase.IsMoveInputTrayTableZAxisToSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == true))
				)
			{
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor move singulation position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableseq: Door Get Trigger %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Move Singulation timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToSingulationPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToSingulationPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToSingulationPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToSingulationPosition;
			}
			break;

		case nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (true
					&& m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true
					)
				{
					smProductEvent->InputTrayTableZAxisMotorChangeSlowSpeedDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorChangeSlowSpeed.Set = true;
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Present Checking After Move To Singulation Position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.InputLoadingStackerUnlockCylinderLock;
				}
				else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->CYLINDER_TIMEOUT)
				{
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockSpan);
						m_cProductShareVariables->SetAlarm(5510);
						m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Tray Not Present %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					}

					nSequenceNo = nCase.CheckTrayPresentSensorAfterMoveToSingulationPosition;
				}
			}
			break;

		case nCase.InputLoadingStackerUnlockCylinderLock:
			if (smProductEvent->InputTrayTableZAxisMotorChangeSlowSpeedDone.Set == true)
			{
				m_cProductIOControl->SetInputLoadingStackerUnlockCylinderOn(false);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsInputLoadingStackerUnlockCylinderLock;
			}
			break;

		case nCase.IsInputLoadingStackerUnlockCylinderLock:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsInputLoadingStackerLockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Loading Stacker Cylinder lock done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisTo_SecondSingulationPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsInputLoadingStackerLockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5403);
					m_cLogger->WriteLog("InputTrayTableseq: Input Loading Stacker Cylinder lock Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.InputLoadingStackerUnlockCylinderLock;
			}
			break;

		case nCase.StartMoveInputTrayTableZAxisTo_SecondSingulationPosition:

			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > 200)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorMoveSecondSingulationDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorMoveSecondSingulation.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveInputTrayTableZAxisTo_SecondSingulationPositionDone;
			}
			break;

		case nCase.IsMoveInputTrayTableZAxisTo_SecondSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSecondSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor move second singulation position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsTrayAvailableAfterSecondSingulationPosition;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableseq: Door Get Trigger %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisTo_SecondSingulationPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Move Second Singulation timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisTo_SecondSingulationPosition;
			}
			break;

		case nCase.StopMoveInputTrayTableZAxisTo_SecondSingulationPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisTo_SecondSingulationPositionDone;
			break;

		case nCase.IsStopMoveInputTrayTableZAxisTo_SecondSingulationPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisTo_SecondSingulationPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisTo_SecondSingulationPosition;
			}
			break;

		case nCase.IsTrayAvailableAfterSecondSingulationPosition:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > smProductSetting->DelayForCheckingInputOutputTableZAtSecondSigulationTrayAvalable_ms)
			{
				if ((m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false || m_cProductIOControl->IsInputTrayTiltSensorOn() == false) && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					m_cProductShareVariables->SetAlarm(5510);
					//nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent;
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Tray Not Present %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

				}
				else
				{
					smProductEvent->InputTrayTableZAxisMotorChangeNormalSpeedDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorChangeNormalSpeed.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPosition;
				}
			}
			break;

		case nCase.StartMoveInputTrayTableZAxisToDownPosition:
			if (smProductEvent->InputTrayTableZAxisMotorChangeNormalSpeedDone.Set == true)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorMoveDown.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsMoveInputTrayTableZAxisToDownPositionDone;
			}
			
			break;
		case nCase.IsMoveInputTrayTableZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set==false)
			{

				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				m_cProductShareVariables->SetAlarm(5510);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent;
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Tray Not Present %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				break;
			}
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor move down position done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsInputTrayTableReady;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableseq: Door Get Trigger %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Move down timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToDownPosition;
			}
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckIsTrayPresentOnInputTrayTableDuringDown;
				RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1s);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis stop Timeout %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionAfterTrayNotPresent;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			break;
		case nCase.CheckIsTrayPresentOnInputTrayTableDuringDown:
			if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set==false)
			{
				//if()if time >preset,then
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
				lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
				if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > (LONGLONG)smProductProduction->TrayPresentSensorOffTimeBeforeAlarm_ms)
				{
					m_cProductShareVariables->SetAlarm(5510);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Tray Not Present %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					//nSequenceNo = nCase.StopMoveInputTrayTableZAxisDownAtLoading;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					break;
				}
			}
			else if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true || smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set==true)

			{
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToDownPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionDone;
			break;

		case nCase.IsStopMoveInputTrayTableZAxisToDownPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor stop done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis stop Timeout %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table Z Axis Motor Stop timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToDownPosition;
			}
			break;
		case nCase.IsSendVisionInputTrayNoDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (( smProductSetting->EnableVision == true  && bPreProductioning == true
				&& ((smProductEvent->GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true) || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true) || smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableS1Vision == true && smProductCustomize->EnableS1VisionModule == true) || smProductSetting->EnableS1Vision == false || smProductCustomize->EnableS1VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true) || smProductSetting->EnableS2Vision == false || smProductCustomize->EnableS2VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
				)||smProductSetting->EnableVision == false)
			{
				if (bPreProductioning == true)
					bPreProductioning = false;
				nSequenceNo = nCase.StartSendInputVisionRowAndColumnBeforeToFirstPosition;
			}
			else if (bPreProductioning == false && 
				smProductSetting->EnableVision == true
				&& ((smProductEvent->GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true) || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
			{
				nSequenceNo = nCase.StartSendInputVisionRowAndColumnBeforeToFirstPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6135);
					smProductEvent->GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_INPUT_SEND_TRAYNO.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6135);
					smProductEvent->GMAIN_RTHD_S3_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S3_SEND_TRAYNO.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6135);
					smProductEvent->GMAIN_RTHD_S1_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S1_SEND_TRAYNO.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6135);
					smProductEvent->GMAIN_RTHD_S2_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S2_SEND_TRAYNO.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6135);
					smProductEvent->GMAIN_RTHD_BTM_SEND_TRAYNO_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_BTM_SEND_TRAYNO.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				break;
			}
			break;
#pragma endregion
#pragma region Pre Production
		case nCase.StartSendInputVisionRowAndColumnBeforeToFirstPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			smProductProduction->bAllowInputSnap = true;
			smProductProduction->InputTableResult[0].InputTrayNo = smProductProduction->nCurrentInputTrayNo;
			smProductProduction->InputTableResult[0].InputColumn = smProductProduction->nEdgeCoordinateY;
			smProductProduction->InputTableResult[0].InputRow = smProductProduction->nEdgeCoordinateX;
			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
			}
			
			m_cLogger->WriteLog("InputTrayTableSeq: Start Send Input Vision Row and column Before to first position.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.StartMoveInputTrayTableXYVisionZAxisToFirstPosition;
			break;
		case nCase.StartMoveInputTrayTableXYVisionZAxisToFirstPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation
				|| smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
			{
				break;
			}
			//smProductProduction->InputTrayTableXAxisMovePosition = smProductTeachPoint->InputTrayTableXAxisAtInputTrayTableCenterPosition + signed long(smProductSetting->DeviceXPitchInput*4.5)-signed long(smProductSetting->DeviceXPitchInput * (smProductProduction->nEdgeCoordinateX-1));
			//smProductProduction->InputTrayTableYAxisMovePosition = smProductTeachPoint->InputTrayTableYAxisAtInputTrayTableCenterPosition + signed long(smProductSetting->DeviceYPitchInput*9.5) - signed long(smProductSetting->DeviceYPitchInput * (smProductProduction->nEdgeCoordinateY - 1));
		
			smProductProduction->InputTrayTableXAxisMovePosition = smProductTeachPoint->InputTrayTableXAxisAtInputTrayTableCenterPosition + signed long(smProductSetting->DeviceXPitchInput * ((double)(smProductSetting->NoOfDeviceInRowInput - 1) / 2));
			smProductProduction->InputTrayTableYAxisMovePosition = smProductTeachPoint->InputTrayTableYAxisAtInputTrayTableCenterPosition + signed long(smProductSetting->DeviceYPitchInput * ((double)(smProductSetting->NoOfDeviceInColInput - 1) / 2));

			smProductProduction->InputTableResult[0].InputTrayNo = smProductProduction->nCurrentInputTrayNo;
			smProductProduction->InputTableResult[0].InputColumn = smProductProduction->nEdgeCoordinateY;
			smProductProduction->InputTableResult[0].InputRow = smProductProduction->nEdgeCoordinateX;

			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableInputVisionMotor == true)
			{
				smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorMoveFocusPosition.Set = true;
				smProductProduction->InputVisionModuleCurrentPosition = smProductTeachPoint->InputVisionZAxisAtInputVisionFocusPosition;
			}
			m_cProductIOControl->SetInputVisionLightingExtendCylinder(true);
			m_cLogger->WriteLog("InputTrayTableSeq: Start Input Tray Table X Y move to first position , Input Vision Module Z Axis Move to focus position done.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableXYVisionZAxisToFirstPositionDone;
			break;
		case nCase.IsMoveInputTrayTableXYVisionZAxisToFirstPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set == true))
				&& (m_cProductIOControl->IsInputVisionLightingExtendSensor() == true)
				//&& ((smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				//	|| (smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y move to first position done, Input Vision Module Z Axis Move to focus position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				smProductProduction->nEdgeCoordinateX = 1;
				smProductProduction->nEdgeCoordinateY = 1;
				smProductProduction->InputTrayTableXIndexPosition = smProductProduction->InputTrayTableXAxisMovePosition;
				smProductProduction->InputTrayTableYIndexPosition = smProductProduction->InputTrayTableYAxisMovePosition;
				
				nSequenceNo = nCase.IsSendInputVisionRowAndColumnBeforeToFirstPositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
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
				if (smProductCustomize->EnableInputVisionMotor == true)
				{
					smProductEvent->InputVisionModuleMotorStopDone.Set = false;
					smProductEvent->StartInputVisionModuleMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableXYVisionZAxisToFirstPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y move to first position timeout and Input Vision Module Z Axis Move to focus position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				//{
				//	m_cProductShareVariables->SetAlarm(6109);
				//	m_cLogger->WriteLog("InputTrayTableSeq: Input Vision receive column and row timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//	nSequenceNo = nCase.StartSendInputVisionRowAndColumn;
				//}
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Axis Motor Move to first position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition;
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Y Axis Motor Move to first position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition;
				}
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Z Axis Motor Move to focus position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition;
				}
				if (m_cProductIOControl->IsInputVisionLightingExtendSensor() == false)
				{
					m_cProductShareVariables->SetAlarm(5524);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Lighitng Extend Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition;
				}
			}
			break;
		case nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition:
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
			if (smProductCustomize->EnableInputVisionMotor == true)
			{
				smProductEvent->InputVisionModuleMotorStopDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableXYVisionZAxisToFirstPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableXYVisionZAxisToFirstPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis and Input Vision Module Z Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableXYVisionZAxisToFirstPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis and Input Vision Module Z Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48008);
					m_cLogger->WriteLog("Input Vision Z Axis Motor Stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYVisionZAxisToFirstPosition;
			}
			break;
		case nCase.IsSendInputVisionRowAndColumnBeforeToFirstPositionDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Send Input Vision Row and column after to first position Done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartInputVisionSOV;				
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendInputVisionRowAndColumnBeforeToFirstPositionDone;
					nSequenceNo = nCase.DelayAfterReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				else if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				{
					m_cProductShareVariables->SetAlarm(6109);
					m_cLogger->WriteLog("InputTrayTableSeq:  Send Input Vision Row and column after to first position timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				
			}
			break;
		case nCase.StartSendInputVisionRowAndColumn:
			if (smProductEvent->JobStop.Set == true)
			{
				if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == false)
				{
					m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
					m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					}
					else
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
					}
				}
				break;
			}
			smProductProduction->InputTableResult[0].InputTrayNo = smProductProduction->nCurrentInputTrayNo;
			smProductProduction->InputTableResult[0].InputColumn = smProductProduction->nEdgeCoordinateY;
			smProductProduction->InputTableResult[0].InputRow = smProductProduction->nEdgeCoordinateX;

			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;

			}
			m_cLogger->WriteLog("InputTrayTableSeq: Start Send Input Vision Row and column.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsSendInputVisionRowAndColumnDone;
			break;
		case nCase.IsSendInputVisionRowAndColumnDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true &&
				((smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
					|| (smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Receives Row and Column Done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartInputVisionSOV;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendInputVisionRowAndColumnDone;
					nSequenceNo = nCase.DelayAfterReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				else
				{
					m_cProductShareVariables->SetAlarm(6109);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision receive column and row timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StartSendInputVisionRowAndColumn;
				}				
			}
			break;

		case nCase.CheckTotalOutputUnitsDone:
			if (smProductEvent->JobStop.Set == true)
			{
				if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == false)
				{
					m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
					m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					}
					else
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
					}
				}
				break;
			}
			
			if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
			{
				nSequenceNo = nCase.SetUnitReadyToBePicked;
				break;
			}
			nSequenceNo = nCase.SetUnitReadyToBePicked;
			break;
	
		case nCase.CheckCurrentInputLotDone:
			if (smProductSetting->EnableCountDownByInputQuantity == true)
			{
				if (smProductProduction->nCurrentInputLotQuantityRun >= smProductProduction->nInputLotQuantity && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
				{
					m_cLogger->WriteLog("InputTrayTableSeq: Current Input Lot Reach Quantity.\n");
					smProductEvent->RMAIN_RTHD_CURRENT_INPUT_LOT_DONE.Set = true;
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				else
				{
					nSequenceNo = nCase.StartInputVisionSOV;
				}
			}
			else if (smProductSetting->EnableCountDownByInputTrayNo == true)
			{
				nSequenceNo = nCase.StartInputVisionSOV;
			}
			break;
		case nCase.StartInputVisionSOV:
			if (smProductEvent->JobStop.Set == true)
			{
				if (smProductEvent->RINP_RPNP_REQUIRE_PURGING.Set == false)
				{
					m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
					m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					}
					else
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
					}
				}
				break;
			}
			if (m_cProductIOControl->IsInputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				m_cProductShareVariables->SetAlarm(6101);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsInputVisionEndOfVision() == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				m_cProductShareVariables->SetAlarm(6102);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision busy.\n");
				break;
			}
#pragma region Offset
			smProductProduction->InputTableResult[0].InputXOffset_um = 0;
			smProductProduction->InputTableResult[0].InputYOffset_um = 0;
			smProductProduction->InputTableResult[0].InputThetaOffset_mDegree = 0;
#pragma endregion
			//if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation
			//	|| smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
			//{
			//	break;
			//}
			//if (smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set == false)
			//{
			//	break;
			//}
			if (smProductProduction->bAllowInputSnap == false)
			{
				break;
			}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{

				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_FAIL.Set = false;
			}

			lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeInputVisionSnap_ms;
			RtSleepFt(&lnDelayIn100ns);
			smProductEvent->RTHD_GMAIN_GET_INP_VISION_XYTR_START.Set = true;
			m_cProductIOControl->SetInputVisionSOV(true);
			m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Start Get XY Theta.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsInputVisionDone;
			break;
		
		case nCase.IsInputVisionDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			lnInputTrayTableSequenceClockSpan4.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart4.QuadPart;
			if (true
				&& ((m_cProductIOControl->IsInputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set == true && smProductProduction->InputTableResult[0].InputResult != 3 && smProductProduction->InputTableResult[0].InputResult != 5
					&& smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
					|| smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
				)
			{
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("InputTrayTableSeq: Job Pause.\n");
					}
				}
				if (smProductProduction->bolIsLastUnitTo1EndTray == true)
				{
					smProductProduction->bolIsLastUnitTo1EndTray = false;
				}
				else if (smProductProduction->bolIsLastUnitTo2EndTray == true)
				{
					smProductProduction->bolIsLastUnitTo2EndTray = false;
				}
				m_cProductIOControl->SetInputVisionSOV(false);
				smProductProduction->nCurrentLowYieldAlarmQuantity++;
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Get XY Theta done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[Timer],InputTrayTableSeq: Move Offset And Input Inspection done %ums.\n", lnInputTrayTableSequenceClockSpan4.QuadPart / m_cProductShareVariables->m_TimeCount);
		
				if(smProductEvent->ReviewMode.Set == true)
				{
					if (smProductProduction->InputTableResult[0].InputResult == 1)
					{
						smProductProduction->CurrentInputVisionContinuousFailCount = 0;
					}
					else if (smProductProduction->InputTableResult[0].InputResult != 1)
					{
						smProductProduction->CurrentInputVisionContinuousFailCount++;
					}

					//if (smProductSetting->InputVisionContinuousFailCountToTriggerAlarm != 0)
					{
						if (smProductProduction->CurrentInputVisionContinuousFailCount >= 1 /*(int)smProductSetting->InputVisionContinuousFailCountToTriggerAlarm*/)
						{
							m_cProductShareVariables->SetAlarm(6124);
						}
					}
				}
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductProduction->InputTableResult[0].UnitPresent = 1;
					smProductProduction->InputTableResult[0].InputResult = 1;
					smProductProduction->InputTableResult[0].InputXOffset_um = 0;
					smProductProduction->InputTableResult[0].InputYOffset_um = 0;
					smProductProduction->InputTableResult[0].InputThetaOffset_mDegree = 0;
				}
				smProductProduction->bPNP2AllowInputSnap = false;
				smProductProduction->bPNP1AllowInputSnap = false;
				smProductProduction->bAllowInputSnap = false;
				smProductProduction->CurrentInputVisionLoopNo = 0;
				smProductProduction->nContinuouslyEmptyPocket = 0;
				nSequenceNo = nCase.IsInputAdditionalRequiredMoveAndSnap;
			}
			else if (m_cProductIOControl->IsInputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set == true 
				&& (smProductProduction->InputTableResult[0].InputResult == 3 || smProductProduction->InputTableResult[0].InputResult == 5)
				&& smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				m_cProductIOControl->SetInputVisionSOV(false);
				smProductProduction->nCurrentLowYieldAlarmQuantity++;
				if (smProductProduction->InputTableResult[0].InputResult == 3)
				{
					smProductProduction->nContinuouslyEmptyPocket++;
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Missing Unit or Empty Pocket %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				else if (smProductProduction->InputTableResult[0].InputResult == 5)
				{
					smProductProduction->nCurrentInputQuantity++;
					smProductProduction->nContinuouslyEmptyPocket = 0;
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision 2DID Fail To Inspect %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				smProductProduction->InputTrayTableXIndexPosition = smProductProduction->InputTrayTableXAxisMovePosition;
				smProductProduction->InputTrayTableYIndexPosition = smProductProduction->InputTrayTableYAxisMovePosition;
				nSequenceNo = nCase.SetNextUnitInput;

			}
			else if (m_cProductIOControl->IsInputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_FAIL.Set == true
				&& smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true && smProductSetting->EnableVision == true)
			{
				m_cProductIOControl->SetInputVisionSOV(false);
				m_cProductShareVariables->SetAlarm(6105);
				m_cLogger->WriteLog("InputVisionseq: Input Vision get XY Theta fail %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				InputVisionSequence = 1;
				//smProductEvent->RINPV_GMAIN_INP_VISION_RESET_EOV.Set = true;
				//nSequenceNo = nCase.StartSendInputVisionRowAndColumn;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
				nSequenceNo = nCase.IsSendInputVisionRowAndColumnBeforeToNextPositionDone;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				{
					m_cProductIOControl->SetInputVisionSOV(false);
					m_cProductShareVariables->SetAlarm(6103);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Get XY Theta timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				//smProductEvent->RINPV_GMAIN_INP_VISION_RESET_EOV.Set = true;
				InputVisionSequence = 1;
				//nSequenceNo = nCase.StartSendInputVisionRowAndColumn;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsSendInputVisionRowAndColumnBeforeToNextPositionDone;
			}
			break;

		case nCase.IsInputAdditionalRequiredMoveAndSnap:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;

			bIsInputVisionRequireAdditionalMoveAndSnap = false;
			for (int i = smProductProduction->CurrentInputVisionLoopNo; i < 10; i++)
			{
				if (smProductSetting->InputVision[smProductProduction->CurrentInputVisionLoopNo].Enable == true)
				{
					//smProductProduction->nSidewallRightVisionAdditionalSnapNo = i + 1;
					bIsInputVisionRequireAdditionalMoveAndSnap = true;
					smProductProduction->CurrentInputVisionLoopNo = i;
					break;
				}
			}
			if (bIsInputVisionRequireAdditionalMoveAndSnap == false)//complete Input Vision
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap Done.\n");
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nCurrentInputResultCounter = 0;
				nSequenceNo = nCase.IsInputVisionAdditionalEOVDone;
			}
			else
			{
				if (smProductProduction->CurrentInputVisionLoopNo == 0)
				{
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap.\n");
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.InputVisionReadyForAdditionalFocusPosition;
				}
				else
				{ 
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap.\n");
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					nSequenceNo = nCase.StartMoveInputVisionZAxisToAdditionalPosition;
				}
			}
			break;

		case nCase.InputVisionReadyForAdditionalFocusPosition:
			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START.Set = true;
			}
			m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.StartMoveInputVisionZAxisToAdditionalPosition;
			break;

		case nCase.StartMoveInputVisionZAxisToAdditionalPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			
			if (smProductCustomize->EnableInputVisionMotor)
			{
				smProductProduction->InputVisionModuleMovePosition = smProductProduction->InputVisionModuleCurrentPosition
					+ (signed long)smProductSetting->InputVision[smProductProduction->CurrentInputVisionLoopNo].FocusOffset_um;
				smProductEvent->InputVisionModuleMotorMoveDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorMove.Set = true;
			}
			m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Additional Move And Snap.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputVisionZAxisToAdditionalPositionDone;
			break;

		case nCase.IsMoveInputVisionZAxisToAdditionalPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveDone.Set == true))
				)
			{
				smProductProduction->InputVisionModuleCurrentPosition = smProductProduction->InputVisionModuleMovePosition;
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Module Z Axis move to additional position done %ums\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsSendInputVisionRowAndColumnToAdditionalPositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputVisionMotor)
				{
					smProductEvent->InputVisionModuleMotorStopDone.Set = false;
					smProductEvent->StartInputVisionModuleMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Get Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputVisionZAxisToAdditionalPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48002);
					m_cLogger->WriteLog("Input Vision Z Axis Motor move to additional position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

					nSequenceNo = nCase.StopMoveInputVisionAxisToAdditionalPosition;
				}
			}
			break;
		case nCase.StopMoveInputVisionAxisToAdditionalPosition:
			if (smProductCustomize->EnableInputVisionMotor)
			{
				smProductEvent->InputVisionModuleMotorStopDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputVisionZAxisToAdditionalPositionDone;
			break;
		case nCase.IsStopMoveInputVisionZAxisToAdditionalPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Module Z Axis stop done %ums\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputVisionZAxisToAdditionalPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Z Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48002);
					m_cLogger->WriteLog("Input Vision Z Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputVisionAxisToAdditionalPosition;
			}
			break;

		case nCase.IsSendInputVisionRowAndColumnToAdditionalPositionDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductEvent->GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision receive row and column after to additional position done %ums\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartInputVisionAdditionalSOV;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE.Set == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				{
					m_cProductShareVariables->SetAlarm(6109);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision receive column and row after to additional position timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);

					smProductEvent->GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START.Set = true;

					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
			}
			break;

		case nCase.StartInputVisionAdditionalSOV:

			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			if (m_cProductIOControl->IsInputVisionReadyOn() == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				m_cProductShareVariables->SetAlarm(6101);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision not ready.\n");
				break;
			}
			if (m_cProductIOControl->IsInputVisionGrabDone() == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				m_cProductShareVariables->SetAlarm(6102);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision busy.\n");
				break;
			}
			//if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation
			//	|| smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
			//{
			//	break;
			//}
			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set = false;
				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_FAIL.Set = false;
			}
			else
			{
				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set = true;
				smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_FAIL.Set = false;
			}

			lnDelayIn100ns.QuadPart = lConvert1msTo100ns * (LONGLONG)smProductSetting->DelayBeforeInputVisionSnap_ms;
			RtSleepFt(&lnDelayIn100ns);

			smProductEvent->RTHD_GMAIN_GET_INP_VISION_XYTR_START.Set = true;
			m_cProductIOControl->SetInputVisionSOV(true);
			m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Start snap for inspection.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsInputVisionAdditionalStart;
			break;

		case nCase.IsInputVisionAdditionalStart:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((m_cProductIOControl->IsInputVisionGrabDone() == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Vision start of vision inspection %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsInputVisionAdditionalGrabDone;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6104);
				m_cProductIOControl->SetInputVisionSOV(false);
				m_cLogger->WriteLog("InputTableseq: Input Vision start of vision inspection timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartInputVisionAdditionalSOV;
			}
			break;

		case nCase.IsInputVisionAdditionalGrabDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((m_cProductIOControl->IsInputVisionGrabDone() == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
			{
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("InputTrayTableSeq: Job Pause.\n");
					}
				}
				m_cProductIOControl->SetInputVisionSOV(false);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision grab for inspection done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nCurrentInputVision++;
				//nCurrentInputVisionDone++;
				smProductProduction->CurrentInputVisionLoopNo++;
				nSequenceNo = nCase.IsInputAdditionalRequiredMoveAndSnap;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6103);
				m_cProductIOControl->SetInputVisionSOV(false);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision grab for inspection timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartSendInputVisionRowAndColumn;
			}
			break;

		case nCase.IsInputVisionAdditionalEOVDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((m_cProductIOControl->IsInputVisionEndOfVision() == true && smProductEvent->GMAIN_RTHD_GET_INP_VISION_XYTR_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
			{
				if (smProductEvent->CycleMode.Set == true)
				{
					if (m_cProductStateControl->IsCurrentStateCanTriggerPause() == true)
					{
						smProductEvent->StartPause.Set = true;
						smProductEvent->JobPause.Set = true;
						m_cLogger->WriteLog("InputTrayTableSeq: Job Pause.\n");
					}
				}
				m_cProductIOControl->SetInputVisionSOV(false);
				if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true)
				{
					smProductSetting->InputVision[nCurrentInputVision].Result = 1;
				}
				if (nCurrentInputResultCounter == 0)
				{
					smProductProduction->InputTableResult[0].InputResult = smProductSetting->InputVision[nCurrentInputVision].Result;
					nCurrentInputResultCounter++;
				}
				else if (nCurrentInputResultCounter > 0)
				{
					if (smProductSetting->InputVision[nCurrentInputVision].Result != 1 && smProductSetting->InputVision[nCurrentInputVision].Result != 0)
					{
						smProductProduction->InputTableResult[0].InputResult = smProductSetting->InputVision[nCurrentInputVision].Result;
					}
				}
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision snap for inspection done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nCurrentInputVisionDone++;
				nSequenceNo = nCase.CheckInputUnitPresent;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				//m_cProductShareVariables->SetAlarm(6103);
				//m_cProductIOControl->SetInputVisionSOV(false);
				//m_cLogger->WriteLog("InputTrayTableSeq: Input Vision End of vision snap for inspection timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//InputVisionSequence = 3;
				//smProductEvent->RINPV_GMAIN_INP_VISION_RESET_EOV.Set = true;
				//if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				//{
				//	smProductEvent->GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE.Set = false;
				//	smProductEvent->RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START.Set = true;
				//}
				//RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				//nSequenceNo = nCase.IsInputVisionAdditionalRowAndColumnDone;
			}
			break;

		case nCase.CheckInputUnitPresent:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
			}
			if (smProductProduction->InputTableResult[0].UnitPresent == 1)
			{
				nSequenceNo = nCase.AdjustOffsetInput;
				//smProductEvent->RINP_ROUT_INPUT_RESULT_DONE.Set = true;
				m_cLogger->WriteLog("InputTrayTableseq: Unit Present.\n");
			}
			else if (smProductProduction->InputTableResult[0].UnitPresent == 0)
			{
				smProductProduction->InputTrayTableXIndexPosition = smProductProduction->InputTrayTableXAxisMovePosition;
				smProductProduction->InputTrayTableYIndexPosition = smProductProduction->InputTrayTableYAxisMovePosition;
				nSequenceNo = nCase.SetNextUnitInput;
				m_cLogger->WriteLog("InputTrayTableseq: Empty Pocket, Move Next Unit.\n");
				smProductEvent->RTHD_GMAIN_UPDATE_INPUT_MAPPING.Set = true;
			}
			break;

		case nCase.AdjustOffsetInput:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
			}
			smProductProduction->InputTrayTableXAxisMovePosition = smProductProduction->InputTrayTableXIndexPosition - smProductProduction->InputTableResult[0].InputXOffset_um;
			smProductProduction->InputTrayTableYAxisMovePosition = smProductProduction->InputTrayTableYIndexPosition - smProductProduction->InputTableResult[0].InputYOffset_um;
			m_cLogger->WriteLog("InputTrayTableseq: X move position %u.\n", smProductProduction->InputTrayTableXAxisMovePosition);
			m_cLogger->WriteLog("InputTrayTableseq: Y move position %u.\n", smProductProduction->InputTrayTableYAxisMovePosition);
			nSequenceNo = nCase.StartMoveOffsetInput;
			break;

		case nCase.StartMoveOffsetInput:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
			}
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorMove.Set = true;
			}
			m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveOffsetInputDone;
			break;

		case nCase.IsMoveOffsetInputDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == true))
				&& (m_cProductIOControl->IsInputVisionLightingRetractSensor() == true)
				)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Y Axis Move offset done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[Timer], InputTrayTableseq: Input Tray Table X Y Axis Move offset and Extend Lighting done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				//nSequenceNo = nCase.SetUnitReadyToBePicked;
				smProductProduction->InputTrayTableXIndexPosition = smProductProduction->InputTrayTableXAxisMovePosition;
				smProductProduction->InputTrayTableYIndexPosition = smProductProduction->InputTrayTableYAxisMovePosition;
				nSequenceNo = nCase.CheckTotalOutputUnitsDone;

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
				//m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveOffsetInput;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table X Y Axis Move offset Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Move offset Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Move offset Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (m_cProductIOControl->IsInputVisionLightingRetractSensor() == false)
				{
					m_cProductShareVariables->SetAlarm(5522);
					m_cLogger->WriteLog("Input Vision Lighting Retract Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StartMoveOffsetInput;
			}
			break;
		
#pragma endregion

#pragma region Input Tray Sequence
		case nCase.StartSetInputTableVacuumOff:
			m_cProductIOControl->SetInputTrayTableVacuumOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsSetInputTableVacuumOffDone;
			break;
		case nCase.IsSetInputTableVacuumOffDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Table Vacuum Off Done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				IsInputTableVacuumOn = false;
				nSequenceNo = nCase.SetUnitReadyToBePicked;
			}
			break;
		case nCase.SetUnitReadyToBePicked:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			for (int i = 0; i < 10; i++)
			{
				if (smProductSetting->InputVision[i].Enable == true && (smProductSetting->InputVision[i].Result != 1 || smProductSetting->InputVision[i].Result != 0))
				{
					if (smProductProduction->InputTableResult[0].InputResult != 1)
						smProductProduction->InputTableResult[0].InputResult = smProductSetting->InputVision[i].Result;
					break;
				}
			}
			m_cProductIOControl->SetInputTrayTableVacuumOn(false);
			smProductProduction->nCurrentInputQuantity++;
			smProductEvent->RINT_RSEQ_INPUT_VISION_DONE.Set = true;
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			smProductEvent->RINT_RSEQ_INPUT_UNIT_PICK_START.Set = true;
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.WaitingUnitToBePicked;
			break;
		case nCase.WaitingUnitToBePicked:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set == true/* && smProductEvent->RSEQ_RINP_PNP_AWAY_FROM_INP_STATION_DONE.Set == true*/)
			{
				//m_cProductIOControl->SetInputVisionLightingExtendCylinder(true);
				if (smProductSetting->EnableInputTableVacuum == true)
				{
					m_cProductIOControl->SetInputTrayTableVacuumOn(true);
				}
				smProductEvent->RPNP_RINT_INPUT_UNIT_PICKED_DONE.Set = false;
				nSequenceNo = nCase.SetNextUnitInput;
			}
			break;
#pragma endregion

		case nCase.SetNextUnitInput:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			ResetInputStationResult();
			smProductProduction->InputTableResult[0].InputResult = 0;
			{
				if (smProductProduction->InputTableResult[0].InputRow % 2 != 0) //odd
				{
					smProductProduction->nEdgeCoordinateY++;

					smProductProduction->InputTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchInput);

					if (smProductProduction->nEdgeCoordinateY > smProductSetting->NoOfDeviceInColInput)
					{
						smProductProduction->nEdgeCoordinateY--;
						smProductProduction->nEdgeCoordinateX++;
						if (smProductProduction->nEdgeCoordinateX > smProductSetting->NoOfDeviceInRowInput)
						{
							smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = true;
							//smProductProduction->nCurrentInputUnitOnTray = 0;
							smProductProduction->LastUnit = true;
							//smProductProduction->InputTableResult[0].bolLastUnitInTray = false;
							nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
							break;
						}
						
						//if (smProductProduction->nEdgeCoordinateX == smProductSetting->NoOfDeviceInRowInput
						//	&& smProductProduction->nEdgeCoordinateY == smProductSetting->NoOfDeviceInColInput)
						//{
						//	smProductProduction->InputTableResult[0].bolLastUnitInTray = true;
						//	if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation)
						//	{
						//		smProductProduction->bolIsLastUnitTo1EndTray = true;
						//	}
						//	else if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
						//	{
						//		smProductProduction->bolIsLastUnitTo2EndTray = true;
						//	}
						//	
						//}
						smProductProduction->InputTrayTableXIndexPosition -= (signed long)(smProductSetting->DeviceXPitchInput);
						smProductProduction->InputTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchInput);
					}
					if (smProductProduction->nContinuouslyEmptyPocket >= smProductSetting->ContinuouslyEmptyPocket && smProductSetting->ContinuouslyEmptyPocket > 0)
					{
						smProductProduction->nContinuouslyEmptyPocket = 0;
						smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = true;
						//smProductProduction->nCurrentInputUnitOnTray = 0;
						smProductProduction->LastUnit = true;
						//smProductProduction->InputTableResult[0].bolLastUnitInTray = false;
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
						break;
					}

				}
				else if (smProductProduction->InputTableResult[0].InputRow % 2 == 0) //even
				{
					smProductProduction->nEdgeCoordinateY--;

					smProductProduction->InputTrayTableYIndexPosition += (signed long)(smProductSetting->DeviceYPitchInput);

					if (smProductProduction->nEdgeCoordinateY < 1)
					{
						smProductProduction->nEdgeCoordinateY++;
						smProductProduction->nEdgeCoordinateX++;
						if (smProductProduction->nEdgeCoordinateX > smProductSetting->NoOfDeviceInRowInput)
						{
							smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = true;
							//smProductProduction->nCurrentInputUnitOnTray = 0;
							smProductProduction->LastUnit = true;
							//smProductProduction->InputTableResult[0].bolLastUnitInTray = false;
							nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
							break;
						}
						
						//if (smProductProduction->nEdgeCoordinateX == smProductSetting->NoOfDeviceInRowInput
						//	&& smProductProduction->nEdgeCoordinateY == 1)
						//{
						//	smProductProduction->InputTableResult[0].bolLastUnitInTray = true;
						//	if (smProductProduction->PickAndPlace1CurrentStation == PickAndPlaceCurrentStation.InputStation)
						//	{
						//		smProductProduction->bolIsLastUnitTo1EndTray = true;
						//	}
						//	else if (smProductProduction->PickAndPlace2CurrentStation == PickAndPlaceCurrentStation.InputStation)
						//	{
						//		smProductProduction->bolIsLastUnitTo2EndTray = true;
						//	}
						//}
						smProductProduction->InputTrayTableYIndexPosition -= (signed long)(smProductSetting->DeviceYPitchInput);
						smProductProduction->InputTrayTableXIndexPosition -= (signed long)(smProductSetting->DeviceXPitchInput);

					}
					if (smProductProduction->nContinuouslyEmptyPocket >= smProductSetting->ContinuouslyEmptyPocket && smProductSetting->ContinuouslyEmptyPocket > 0)
					{
						smProductProduction->nContinuouslyEmptyPocket = 0;
						smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set = true;
						//smProductProduction->nCurrentInputUnitOnTray = 0;
						smProductProduction->LastUnit = true;
						//smProductProduction->InputTableResult[0].bolLastUnitInTray = false;
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
						break;
					}
				}

				smProductProduction->InputTrayTableXAxisMovePosition = smProductProduction->InputTrayTableXIndexPosition;
				smProductProduction->InputTrayTableYAxisMovePosition = smProductProduction->InputTrayTableYIndexPosition;
			}
			nSequenceNo = nCase.StartSendInputVisionRowAndColumnBeforeToNextPosition;
			break;

		case nCase.StartSendInputVisionRowAndColumnBeforeToNextPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}

			smProductProduction->InputTableResult[0].InputTrayNo = smProductProduction->nCurrentInputTrayNo;
			smProductProduction->InputTableResult[0].InputColumn = smProductProduction->nEdgeCoordinateY;
			smProductProduction->InputTableResult[0].InputRow = smProductProduction->nEdgeCoordinateX;

			if (smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
			{
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
			}
			m_cLogger->WriteLog("InputTrayTableSeq: Start send input vision row and column before to next position.\n");
			nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToNextPosition;
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart4);
			break;
		case nCase.StartMoveInputTrayTableXYAxisToNextPosition:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			if (smProductCustomize->EnableInputTrayTableXAxisMotor == true)
			{
				smProductEvent->InputTrayTableXAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableXAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableInputTrayTableYAxisMotor == true)
			{
				smProductEvent->InputTrayTableYAxisMotorMoveDone.Set = false;
				smProductEvent->StartInputTrayTableYAxisMotorMove.Set = true;
			}
			if (smProductCustomize->EnableInputVisionMotor == true)
			{
				smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorMoveFocusPosition.Set = true;
				smProductProduction->InputVisionModuleCurrentPosition = smProductTeachPoint->InputVisionZAxisAtInputVisionFocusPosition;
			}
			m_cLogger->WriteLog("InputTrayTableSeq: Start Input Tray Table X Y Axis Move to next position, input vision move to focus position.\n");
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableXYAxisToNextPositionDone;
			break;
		case nCase.IsMoveInputTrayTableXYAxisToNextPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == true))
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to next position done, input vision move to focus position done done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[Timer],InputTrayTableSeq: Input Tray Table X Y Axis Move to next position done, input vision move to focus position done done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsSendInputVisionRowAndColumnBeforeToNextPositionDone;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
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
				if (smProductCustomize->EnableInputVisionMotor == true)
				{
					smProductEvent->InputVisionModuleMotorStopDone.Set = false;
					smProductEvent->StartInputVisionModuleMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToNextPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to next position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveDone.Set == false)
				{
m_cProductShareVariables->SetAlarm(41002);
m_cLogger->WriteLog("Input Tray Table X Axis Motor Move to next position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToNextPosition;
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Move to next position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToNextPosition;
				}
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorMoveFocusPositionDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Z Axis Motor Move to focus position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToNextPosition;
				}

			}
			break;
		case nCase.StopMoveInputTrayTableXYAxisToNextPosition:

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
			if (smProductCustomize->EnableInputVisionMotor == true)
			{
				smProductEvent->InputVisionModuleMotorStopDone.Set = false;
				smProductEvent->StartInputVisionModuleMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToNextPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableXYAxisToNextPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputVisionMotor == false || (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis and input vision stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToNextPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputVisionMotor == true && smProductEvent->InputVisionModuleMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(48008);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Z Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToNextPosition;
			}
			break;
		case nCase.IsSendInputVisionRowAndColumnBeforeToNextPositionDone:
			if (smProductEvent->JobStop.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == true && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				|| (smProductSetting->EnableVision == false || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false))
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Send input vision row and column after to next position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				m_cLogger->WriteLog("[TImer],InputTrayTableSeq: Send input vision row and column after to next position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.CheckEventToEnsurePickAndPlaceHaveMoveAway;
				//nSequenceNo = nCase.StartInputVisionSOV;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set == true)
				{
					nSequenceNo_Cont = nCase.IsSendInputVisionRowAndColumnBeforeToNextPositionDone;
					nSequenceNo = nCase.DelayAfterReceivedNAK;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				else if (smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set == false && smProductSetting->EnableVision == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true)
				{
					m_cProductShareVariables->SetAlarm(6109);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Vision receive column and row timeout after to next position %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
					smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			break;

		case nCase.CheckEventToEnsurePickAndPlaceHaveMoveAway:
			if (smProductProduction->bAllowInputSnap == true)
			{
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(true);
				nSequenceNo = nCase.CheckEnsureInputLightingCylinderExtend;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				//nSequenceNo = nCase.StartInputVisionSOV;
			}
			break;

		case nCase.CheckEnsureInputLightingCylinderExtend:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (m_cProductIOControl->IsInputVisionLightingExtendSensor() == true)
			{
				nSequenceNo = nCase.StartInputVisionSOV;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(5524);
				m_cLogger->WriteLog("InputTrayTableSeq: Input Vision Not In Extend Position.\n");
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				RtSleep(100);
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(true);
				nSequenceNo = nCase.CheckEnsureInputLightingCylinderExtend;
			}
			break;

#pragma region Unloading
		case nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition:
			if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				m_cProductShareVariables->SetAlarm(5510);
				m_cLogger->WriteLog("InputTrayTableSeq: Tray not present on input tray table during unloading.\n");
				break;

			}
			else
			{
				if (m_cProductIOControl->IsInputUnloadingStackerFullSensorOn() == true)
				{
					m_cProductShareVariables->SetAlarm(5405);
					m_cLogger->WriteLog("InputTrayTableSeq: Tray full on input unloading stacker.\n");
					break;
				}
			}
			if (smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set == true)
			{
				smProductEvent->RINT_RSEQ_INPUT_FIRST_UNIT.Set = false;
			}
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

			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableXYAxisToUnloadingPositionDone;
			break;
		case nCase.IsMoveInputTrayTableXYAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to Unloading position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToSingulationPositionForUnloading;
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
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToUnloadingPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to Unloading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Move to Unloading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Move to Unloading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToUnloadingPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableXYAxisToUnloadingPosition:
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
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToUnloadingPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableXYAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Move stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Move stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToUnloadingPosition;
			}
			break;
		case nCase.StartMoveInputTrayTableZAxisToSingulationPositionForUnloading:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveSingulation.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableZAxisToSingulationPositionForUnloadingDone;
			break;
		case nCase.IsMoveInputTrayTableZAxisToSingulationPositionForUnloadingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Move to Singulation position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.InputUnloadingStackerUnlockCylinderUnlockForUnloading;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionForUnloadingDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Move to Singulation position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveSingulationDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("Input Tray Table Z Axis Motor Move to Singulation position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToSingulationPositionForUnloading;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToSingulationPositionForUnloading:

			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionForUnloadingDone;
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToSingulationPositionForUnloadingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToSingulationPositionForUnloading;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("Input Tray Table Z Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToSingulationPositionForUnloading;
			}
			break;
		case nCase.InputUnloadingStackerUnlockCylinderUnlockForUnloading:
			m_cProductIOControl->SetInputUnloadingStackerUnlockCylinderOn(true);
			m_cProductIOControl->SetInputTrayTableVacuumOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsInputUnloadingStackerUnlockCylinderUnlockForUnloading;
			break;
		case nCase.IsInputUnloadingStackerUnlockCylinderUnlockForUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsInputUnloadingStackerUnlockSensorOn() == true
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Unloading Stacker unlock cylinder done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToUnloadingPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsInputUnloadingStackerUnlockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5417);
					m_cLogger->WriteLog("Input Unloading Stacker unlock cylinder Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.InputUnloadingStackerUnlockCylinderUnlockForUnloading;
			}
			break;
		case nCase.StartMoveInputTrayTableZAxisToUnloadingPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveUnload.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableZAxisToUnloadingPositionDone;
			break;
		case nCase.IsMoveInputTrayTableZAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Move to Unloading position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.InputUnloadingStackerUnlockCylinderLockForUnloading;
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}

				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToUnloadingPositionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Motor Move to unload position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToUnloadingPosition;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToUnloadingPosition:
			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}

			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToUnloadingPositionDone;
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToUnloadingPositionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToUnloadingPosition;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToUnloadingPosition;
			}
			break;
		case nCase.InputUnloadingStackerUnlockCylinderLockForUnloading:
			m_cProductIOControl->SetInputUnloadingStackerUnlockCylinderOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsInputUnloadingStackerUnlockCylinderLockForUnloading;
			break;
		case nCase.IsInputUnloadingStackerUnlockCylinderLockForUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == true
				/*|| smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == true*/
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Unloading Stacker lock cylinder done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (IsInputTableVacuumOn == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPositionAfterUnloading;
				}
				else
				{
					nSequenceNo = nCase.StartSetInputTableVacuumOffDuringUnloading;
				}
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (m_cProductIOControl->IsInputUnloadingStackerLockSensorOn() == false)
				{
					m_cProductShareVariables->SetAlarm(5407);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Unloading Stacker lock cylinder Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.InputUnloadingStackerUnlockCylinderLockForUnloading;
			}
			break;
		case nCase.StartSetInputTableVacuumOffDuringUnloading:
			m_cProductIOControl->SetInputTrayTableVacuumOn(false);
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsSetInputTableVacuumOffDuringUnloadingDone;
			break;
		case nCase.IsSetInputTableVacuumOffDuringUnloadingDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount >= (LONGLONG)smProductSetting->DelayForCheckingInputTableVacuumOnOffCompletelyBeforeNextStep_ms)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Table Vacuum Off Done during unloading %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				IsInputTableVacuumOn = false;
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPositionAfterUnloading;
			}
			break;
		case nCase.StartMoveInputTrayTableZAxisToDownPositionAfterUnloading:

			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorMoveDown.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableZAxisToDownPositionDoneAfterUnloading;
			break;
		case nCase.IsMoveInputTrayTableZAxisToDownPositionDoneAfterUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveDownDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Move to down position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				
				if (true
					&& m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == true
					)
				{
					m_cProductShareVariables->SetAlarm(5505);
					m_cLogger->WriteLog("InputTrayTableseq: Input Tray Present After Unloading done %ums.", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					break;
				}	
				
				
				if (smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set == true)
				{
					smProductProduction->nCurrentInputLotTrayNoRun++;
				}
				m_cLogger->WriteLog("InputTrayTableSeq: Current Input Tray No Quantity Run = %d.\n", smProductProduction->nCurrentInputLotTrayNoRun);
				//if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 0 && smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 0)
				//{
				//	smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
				//}
				if (smProductProduction->nCurrentInputUnitOnTray == 0)
				{
					//smProductProduction->WriteReportTrayNo = smProductProduction->nCurrentInputTrayNo;
					smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
					smProductEvent->RMAIN_RTHD_CHANGE_MAPPING.Set = true;
					//smProductEvent->RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY.Set = true;
					m_cLogger->WriteLog("InputTrayTableSeq: No Unit is picked, update mapping.\n");
				}
				else
				{
					smProductEvent->RTHD_GMAIN_LOT_UNLOAD_COMPLETE.Set = true;
					smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtInput].IsLastUnit = 1;
					m_cLogger->WriteLog("InputTrayTableSeq: Last Unit.\n");
				}
				if (smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == true)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					if (smProductEvent->RMAIN_RTHD_CURRENT_INPUT_LOT_DONE.Set == true)
					{
						smProductEvent->RMAIN_RTHD_CURRENT_INPUT_LOT_DONE.Set = false;
						nSequenceNo = nCase.IsInputLoadingStackerEmpty;
					}
					else if (smProductEvent->RINP_RSEQ_INPUT_TRAY_FULL.Set == true)
					{
						if (smProductEvent->JobStop.Set == false)
						{
							smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set = false;
							smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDTRAY.Set = true;
							smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = true;
							smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = false;
							RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
							nSequenceNo = nCase.IsSendVisionInputS2EndTrayDone;
						}
						else
						{
							nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
						}
					}
					else
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					}
				}
			}
			else if (IsReadyToMoveProduction() == false)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
				{
					smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
					smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
				}

				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionDoneAfterUnloading;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorMoveUnloadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43002);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Motor Move to unload position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}

				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToDownPositionAfterUnloading;
			}
			break;
		case nCase.StopMoveInputTrayTableZAxisToDownPositionAfterUnloading:

			if (smProductCustomize->EnableInputTrayTableZAxisMotor == true)
			{
				smProductEvent->InputTrayTableZAxisMotorStopDone.Set = false;
				smProductEvent->StartInputTrayTableZAxisMotorStop.Set = true;
			}
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableZAxisToDownPositionDoneAfterUnloading;
			break;
		case nCase.IsStopMoveInputTrayTableZAxisToDownPositionDoneAfterUnloading:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableZAxisMotor == false || (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableZAxisToDownPositionAfterUnloading;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				if (smProductCustomize->EnableInputTrayTableZAxisMotor == true && smProductEvent->InputTrayTableZAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(43008);
					m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table Z Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableZAxisToDownPositionAfterUnloading;
			}
			break;
		case nCase.IsSendVisionInputS2EndTrayDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true 
				&& (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set == true && smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableInputVision==true && smProductCustomize->EnableInputVisionModule==true))
				||smProductSetting->EnableVision==false
				)
			{
				//if (smProductEvent->RTHD_RMAIN_REACH_TOTAL_UNIT_DONE.Set == false)
				//{
					nSequenceNo = nCase.IsInputLoadingStackerEmpty;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set == false)
				{
					m_cProductShareVariables->SetAlarm(6139);
					smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDTRAY.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				break;
			}
			break;
		
		case nCase.IsInputLoadingStackerEmpty:
			smProductEvent->RINT_RSEQ_HEAD_AND_OUTPUT_FULL.Set = false;
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = false;
				
			//if (smProductEvent->JobStop.Set == true)
			//{
			//	m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
			//	if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
			//	{
			//		nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			//	}
			//	else
			//	{
			//		nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
			//	}
			//	break;
			//}
			//if (smProductSetting->EnableCountDownByInputQuantity == true)
			//{
			//	if (smProductProduction->nCurrentInputLotQuantityRun >= smProductProduction->nInputLotQuantity && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			//	{
			//		m_cLogger->WriteLog("InputTrayTableSeq: Input Lot Done, Count by Input Unit Quantity.\n");
			//		if (smProductProduction->nLotIDNumber >= 2)
			//		{
			//			m_cLogger->WriteLog("InputTrayTableSeq: Maximum 3 lots, proceed to end lot");
			//			m_cProductShareVariables->SetAlarm(5534);
			//			smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
			//			smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
			//			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			//			nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			//			break;
			//		}
			//		else
			//		{
			//			smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtInput].IsLastUnitForCurrentLot = 1;
			//			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			//			smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = true;
			//			nSequenceNo = nCase.IsCurrentLotAllProcessed;
			//			break;
			//		}
			//	}
			//}
			//else if (smProductSetting->EnableCountDownByInputTrayNo == true)
			//{
			//	if (smProductProduction->nCurrentInputLotTrayNoRun >= smProductProduction->nInputLotTrayNo && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			//	{
			//		m_cLogger->WriteLog("InputTrayTableSeq: Input Lot Done, Count by Input Tray No Quantity.\n");
			//		if (smProductProduction->nLotIDNumber >= 2)
			//		{
			//			m_cLogger->WriteLog("InputTrayTableSeq: Maximum 3 lots, proceed to end lot");
			//			m_cProductShareVariables->SetAlarm(5534);
			//			smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
			//			smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
			//			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			//			nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			//			break;
			//		}
			//		else
			//		{
			//			smProductProduction->PickAndPlacePickUpHeadStationResult[smProductProduction->nCurrentPickupHeadAtInput].IsLastUnitForCurrentLot = 1;
			//			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			//			smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = true;
			//			nSequenceNo = nCase.IsCurrentLotAllProcessed;
			//			break;
			//		}
			//	}
			//}
			smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = true;
			if (m_cProductIOControl->IsInputLoadingStackerPresentSensorOn() == false && smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
			{
				if (smProductEvent->JobStop.Set == true)
				{
					m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
					m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
					if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					}
					else
					{
						nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
					}
					break;
				}
				if (smProductEvent->GMAIN_RTHD_ENDLOT.Set == true)
				{
					smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
					smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
					smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					m_cLogger->WriteLog("InputTrayTableseq: EndLot.\n");
					m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					//nSequenceNo = nCase.WaitPickAndPlaceToPlaceDuringPostProduction;
					break;
				}
				else
				{
					smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = false;
					m_cProductShareVariables->SetAlarm(5525);
					m_cLogger->WriteLog("InputTrayTableseq: Tray not present on input loading stacker.\n");
					break;
				}
			}
			else
			{
				if (smProductEvent->GMAIN_RTHD_ENDLOT.Set == true)
				{
					smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
					smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
					smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
					m_cLogger->WriteLog("InputTrayTableseq: EndLot.\n");
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
					//nSequenceNo = nCase.WaitPickAndPlaceToPlaceDuringPostProduction;
					break;
				}
				else
				{
					smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
					if (smProductEvent->GGUI_RSEQ_DRY_RUN_MODE.Set == false)
					{
						if (smProductSetting->EnableInputTableVacuum == true)
						{
							nSequenceNo = nCase.StartSetInputTableVacuumOn;
						}
						else
						{
							nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPosition;
						}
					}
					else
					{
						nSequenceNo = nCase.IsInputTrayTableReady;
					}
					break;
				}
			}
			break;
		case nCase.IsCurrentLotAllProcessed:
			smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
			if (smProductEvent->RMAIN_RTHD_CURRENT_LOT_ALL_PROCESSED.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Current Lot All Processed.");
				//smProductEvent->RMAIN_RTHD_UPDATE_MES_LOT_DATA.Set=true;
				nSequenceNo = nCase.IsNewLotOrEndLot;
			}
			break;
		case nCase.IsNewLotOrEndLot:
			smProductEvent->RMAIN_RTHD_NEW_OR_END_LOT_CONDITION.Set = true;
			if (smProductEvent->RINT_RSEQ_SWITCH_NEXT_INPUT_LOT.Set==true)
			{
				smProductEvent->RINT_RSEQ_SWITCH_NEXT_INPUT_LOT.Set = false;
				smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
				smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = false;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = false;
				smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = false;
				smProductEvent->RMAIN_RTHD_NEW_OR_END_LOT_CONDITION.Set = false;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("InputTrayTableSeq: User enter new lot.");
				m_cLogger->WriteLog("InputTrayTableseq: Input Lot Quantity = %u.\n", smProductProduction->nInputLotQuantity);
				nSequenceNo = nCase.IsUpdateDataDone;
				break;
			}
			else if (smProductEvent->GMAIN_RTHD_ENDLOT.Set == true)
			{
				smProductProduction->IsUpdateMESAgain = true;
				smProductEvent->RMAIN_RTHD_NEW_OR_END_LOT_CONDITION.Set = false;
				smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
				smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = false;
				smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
				smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = false;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				m_cLogger->WriteLog("InputTrayTableseq: EndLot.\n");
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				//nSequenceNo = nCase.WaitPickAndPlaceToPlaceDuringPostProduction;
				break;
			}
			else if (smProductEvent->JobStop.Set == true)
			{
				m_cProductIOControl->SetInputVisionLightingExtendCylinder(false);
				smProductProduction->IsUpdateMESAgain = true;
				smProductEvent->RMAIN_RTHD_LAST_UNIT_FOR_LOT_AND_WAIT.Set = false;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
				smProductEvent->RMAIN_RTHD_ENDLOT_CONDITION.Set = false;
				smProductEvent->RMAIN_RTHD_NEW_OR_END_LOT_CONDITION.Set = false;
				smProductEvent->RMAIN_RPNP_RUN_PNP_BEFORE_FINISH_LOT.Set = false;
				m_cLogger->WriteLog("InputTrayTableseq: User stop sequence.\n");
				if (m_cProductIOControl->IsInputTrayTableTrayPresentSensorOn() == false)
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
				}
				else
				{
					nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
				}
				break;
			}
			m_cProductShareVariables->SetAlarm(5533);
			break;
		case nCase.IsUpdateDataDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			if (smProductEvent->RMAIN_RTHD_UPDATE_MES_DATA_DONE.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Update MES data done.");
				smProductProduction->nInputRunningState = 0;
				smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
				smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S2_SEND_ENDLOT.Set = true;
				smProductEvent->GMAIN_RTHD_BTM_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_BTM_SEND_ENDLOT.Set = true;
				smProductProduction->nPNPRunningState = 0;
				smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S3_SEND_ENDLOT.Set = true;
				smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S1_SEND_ENDLOT.Set = true;
				smProductProduction->nOutputRunningState = 0;
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDLOT.Set = true;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDLOT.Set = true;
				
				nSequenceNo = nCase.IsSendEndLotDone;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			break;
		case nCase.IsSendEndLotDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& ((smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true) || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true) || smProductSetting->EnableS2Vision == false || smProductCustomize->EnableS2VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true) || smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_BTM_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)				
				|| smProductSetting->EnableVision == false)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Send Vision end lot done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_INPUT_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_S2_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S2_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_BOTTOM_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_S3_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S3_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_S1_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_S1_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_NEWLOT.Set = true;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_NEWLOT_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_NEWLOT.Set = true;
				nSequenceNo = nCase.IsSendVisionInputS2NewLot;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6011);
				m_cLogger->WriteLog("InputTrayTableSeq: Send Vision end lot timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S2_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S3_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S1_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_BTM_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_BTM_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_BTM_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_OUTPUT_SEND_ENDLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_REJECT_SEND_ENDLOT.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				break;
			}
			break;
		case nCase.IsSendVisionInputS2NewLot:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if ((smProductSetting->EnableVision == true
				&& ((smProductEvent->GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableInputVision == true && smProductCustomize->EnableInputVisionModule == true) || smProductSetting->EnableInputVision == false || smProductCustomize->EnableInputVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S2_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableS2Vision == true && smProductCustomize->EnableS2VisionModule == true) || smProductSetting->EnableS2Vision == false || smProductCustomize->EnableS2VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S3_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableS3Vision == true && smProductCustomize->EnableS3VisionModule == true) || smProductSetting->EnableS3Vision == false || smProductCustomize->EnableS3VisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_S1_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableBottomVision == true && smProductCustomize->EnableBottomVisionModule == true) || smProductSetting->EnableBottomVision == false || smProductCustomize->EnableBottomVisionModule == false)
				&& ((smProductEvent->GMAIN_RTHD_REJECT_SEND_NEWLOT_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
				)
				|| smProductSetting->EnableVision == false)
			{
				smProductProduction->nCurrentInputLotQuantityRun = 0;
				smProductProduction->nCurrentInputLotTrayNoRun = 0;
				smProductProduction->nCurrentLotGoodQuantity=0;
				smProductProduction->nCurrentLotNotGoodQuantity=0;
				smProductProduction->nCurrentInputTrayNo = 0;
				smProductProduction->nCurrentBottomStationTrayNo = 0;
				smProductProduction->nCurrentS3StationTrayNo = 0;
				smProductProduction->nOutputRunningState = 1;
				smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;
				smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
				m_cLogger->WriteLog("InputTrayTableSeq: Send Vision new lot done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.IsSendOutputTrayNoDone;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
			{
				m_cProductShareVariables->SetAlarm(6010);
				if (smProductEvent->GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_INPUT_SEND_NEWLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S2_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S2_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S2_SEND_NEWLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_BOTTOM_SEND_NEWLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S3_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S3_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S3_SEND_NEWLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_S1_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_S1_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_S1_SEND_NEWLOT.Set = true;
				}
				if (smProductEvent->GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE.Set == false)
				{
					smProductEvent->GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE.Set = false;
					smProductEvent->RTHD_GMAIN_OUTPUT_SEND_NEWLOT.Set = true;
				}
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			}
			break;
			case nCase.IsSendOutputTrayNoDone:
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
				lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
				if ((smProductSetting->EnableVision == true
					&& ((smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
					&& ((smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set == true && smProductSetting->EnableOutputVision == true && smProductCustomize->EnableOutputVisionModule == true) || smProductSetting->EnableOutputVision == false || smProductCustomize->EnableOutputVisionModule == false)
					) || smProductSetting->EnableVision == false) //&&smProductEvent->GMAIN_RTHD_OUTPUT_POST_ALIGNMENT_SEND_ENDTRAY_DONE.Set == true
				{
					m_cLogger->WriteLog("InputTrayTableSeq: Output Send TrayNo Done &ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					nSequenceNo = nCase.IsInputLoadingStackerEmpty;
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->COMMAND_TIMEOUT)
				{
					m_cProductShareVariables->SetAlarm(6835);
					m_cLogger->WriteLog("InputTrayTableSeq: Output Send TrayNo timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
					if (smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set == false)
					{
						smProductEvent->GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_OUTPUT_SEND_TRAYNO.Set = true;
					}
					if (smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set == false)
					{
						smProductEvent->GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE.Set = false;
						smProductEvent->RTHD_GMAIN_REJECT_SEND_TRAYNO.Set = true;
					}
					RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				}
				break;
#pragma endregion
#pragma region Post Production
		//case nCase.SendVisionInputEndLotDuringJobStop:
		//	if (smProductProduction->PickAndPlacePickUpHeadStationResult[0].UnitPresent == 1 || smProductProduction->PickAndPlacePickUpHeadStationResult[1].UnitPresent == 1)
		//	{
		//		smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_S2_SEND_ENDLOT.Set = true;
		//		//S1 Station
		//		smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_S1_SEND_ENDLOT.Set = false;
		//		smProductEvent->GMAIN_RTHD_SETUP_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_SETUP_SEND_ENDLOT.Set = false;
		//		smProductEvent->GMAIN_RTHD_SWLEFT_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_SWLEFT_SEND_ENDLOT.Set = false;
		//		smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_SWRIGHT_SEND_ENDLOT.Set = false;
		//		//S3 Station
		//		smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_S3_SEND_ENDLOT.Set = false;
		//		smProductEvent->GMAIN_RTHD_SWFRONT_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_SWFRONT_SEND_ENDLOT.Set = false;
		//		smProductEvent->GMAIN_RTHD_SWREAR_SEND_ENDLOT_DONE.Set = true;
		//		smProductEvent->RTHD_GMAIN_SWREAR_SEND_ENDLOT.Set = false;
		//	}
		//	else
		//	{
		//		smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_S2_SEND_ENDLOT.Set = true;
		//		//S1 Station
		//		smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_S1_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_SETUP_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_SETUP_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_SWLEFT_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_SWLEFT_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_SWRIGHT_SEND_ENDLOT.Set = true;
		//		//S3 Station
		//		smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_S3_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_SWFRONT_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_SWFRONT_SEND_ENDLOT.Set = true;
		//		smProductEvent->GMAIN_RTHD_SWREAR_SEND_ENDLOT_DONE.Set = false;
		//		smProductEvent->RTHD_GMAIN_SWREAR_SEND_ENDLOT.Set = true;
		//	}
		//	smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
		//	nSequenceNo = nCase.WaitPickAndPlaceToPlaceDuringPostProduction;
		//	break;
		//case nCase.WaitPickAndPlaceToPlaceDuringPostProduction: //currently used for checking send vision end lot done or not
		//	RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
		//	lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
		//	if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == true && smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set == true
		//		&& smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set == true && smProductEvent->GMAIN_RTHD_SETUP_SEND_ENDLOT_DONE.Set == true
		//		&& smProductEvent->GMAIN_RTHD_SWLEFT_SEND_ENDLOT_DONE.Set == true && smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set == true
		//		&& smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set == true && smProductEvent->GMAIN_RTHD_SWFRONT_SEND_ENDLOT_DONE.Set == true
		//		&& smProductEvent->GMAIN_RTHD_SWREAR_SEND_ENDLOT_DONE.Set == true)
		//	{
		//		nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
		//	}
		//	else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
		//	{
		//		if (smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_INPUT_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_S2_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_S2_SEND_ENDLOT.Set = true;
		//		}

		//		//S1 STATION
		//		if (smProductEvent->GMAIN_RTHD_SETUP_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_SETUP_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_SETUP_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_S1_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_S1_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_SWLEFT_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_SWLEFT_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_SWLEFT_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_SWRIGHT_SEND_ENDLOT.Set = true;
		//		}
		//		//S3 STATION
		//		if (smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_S3_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_S3_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_SWFRONT_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_SWFRONT_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_SWFRONT_SEND_ENDLOT.Set = true;
		//		}
		//		if (smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set == false)
		//		{
		//			smProductEvent->GMAIN_RTHD_SWRIGHT_SEND_ENDLOT_DONE.Set = false;
		//			smProductEvent->RTHD_GMAIN_SWRIGHT_SEND_ENDLOT.Set = true;
		//		}
		//		RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
		//		break;
		//	}
		//	break;
		case nCase.IsUnloadTrayOutOrRemainTray:
			m_cProductShareVariables->SetAlarm(5535);
			m_cLogger->WriteLog("InputTrayTableSeq: Remain Units on input tray.\n");
			smProductEvent->RMAIN_RTHD_IS_REMAINTRAY.Set = false;
			smProductEvent->RMAIN_RTHD_IS_UNLOADTRAY.Set = false;
			smProductEvent->RMAIN_RTHD_IS_REMAIN_OR_UNLOADTRAY.Set = true;
			nSequenceNo = nCase.IsUnloadTrayOutOrRemainTrayDone;
		break;
		case nCase.IsUnloadTrayOutOrRemainTrayDone:
			if (smProductEvent->RMAIN_RTHD_IS_REMAINTRAY.Set == true)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Remain Tray for next lot.");
				smProductEvent->RMAIN_RTHD_IS_REMAINTRAY.Set = false;
				smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set = true;
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
				smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set = true;
				nSequenceNo = nCase.EndOfSequence;
			}
			else if (smProductEvent->RMAIN_RTHD_IS_UNLOADTRAY.Set == true)
			{
				smProductEvent->RMAIN_RTHD_IS_UNLOADTRAY.Set = false;
				smProductEvent->RMAIN_RTHD_THR_IS_REMAINTRAY.Set = false;
				m_cLogger->WriteLog("InputTrayTableSeq: Proceed to Unload Tray.");
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToUnloadingPosition;
			}
			break;
		case nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsMoveInputTrayTableXYAxisToLoadingPositionDuringPostProductionDone;
			break;
		case nCase.IsMoveInputTrayTableXYAxisToLoadingPositionDuringPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to loading position done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				smProductEvent->RINT_RSEQ_INPUT_UNIT_READY_TO_BE_PICKED.Set = true;
				smProductEvent->RINT_RSEQ_LOADING_STACKER_EMPTY.Set = true;
				smProductEvent->RINT_RSEQ_POST_PRODUCTION_DONE.Set = true;
				nSequenceNo = nCase.EndOfSequence;
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
				m_cProductShareVariables->SetAlarm(60501);
				m_cLogger->WriteLog("InputTrayTableSeq: Door Triggered %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProductionDone;
				break;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis Move to loading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41002);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor Move to loading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorMoveLoadDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42002);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor Move to loading position Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			}
			break;
		case nCase.StopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction:
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
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
			nSequenceNo = nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProductionDone;
			break;
		case nCase.IsStopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProductionDone:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (true
				&& (smProductCustomize->EnableInputTrayTableXAxisMotor == false || (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == true))
				&& (smProductCustomize->EnableInputTrayTableYAxisMotor == false || (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == true))
				)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis stop done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				nSequenceNo = nCase.StartMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			}
			else if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->MOTION_TIMEOUT)
			{
				m_cLogger->WriteLog("InputTrayTableSeq: Input Tray Table X Y Axis stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				if (smProductCustomize->EnableInputTrayTableXAxisMotor == true && smProductEvent->InputTrayTableXAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(41008);
					m_cLogger->WriteLog("Input Tray Table X Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				if (smProductCustomize->EnableInputTrayTableYAxisMotor == true && smProductEvent->InputTrayTableYAxisMotorStopDone.Set == false)
				{
					m_cProductShareVariables->SetAlarm(42008);
					m_cLogger->WriteLog("Input Tray Table Y Axis Motor stop Timeout %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
				}
				nSequenceNo = nCase.StopMoveInputTrayTableXYAxisToLoadingPositionDuringPostProduction;
			}
			break;

		case nCase.DelayAfterReceivedNAK:
			RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
			lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart2.QuadPart;
			if (lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount > m_cProductShareVariables->VISION_TIMEOUT)
			{
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_NAK.Set = false;
				smProductEvent->GMAIN_RTHD_INP_VISION_GET_RC_DONE.Set = false;
				smProductEvent->RTHD_GMAIN_SEND_INP_VISION_RC_START.Set = true;
				RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockStart2);
				nSequenceNo = nSequenceNo_Cont;
			}
			break;
#pragma endregion
		default:
			return -1;
			m_cLogger->WriteLog("InputTrayTableSeq: return -1.\n");
			break;
		}
		//End of sequence
		if (nSequenceNo == 999)
		{
			nSequenceNo = 0;
		}
		RtSleepFt(&m_cProductShareVariables->m_lnPeriod_1ms);
	}
	RtGetClockTime(CLOCK_FASTEST, &lnInputTrayTableSequenceClockEnd);
	lnInputTrayTableSequenceClockSpan.QuadPart = lnInputTrayTableSequenceClockEnd.QuadPart - lnInputTrayTableSequenceClockStart.QuadPart;
	m_cLogger->WriteLog("InputTrayTableseq: Input Tray Table sequence done %ums.\n", lnInputTrayTableSequenceClockSpan.QuadPart / m_cProductShareVariables->m_TimeCount);
	return 0;
}