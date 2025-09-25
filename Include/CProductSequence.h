#pragma once
#ifndef __CPRODUCTSEQUENCE_H_INCLUDED__ 
#define __CPRODUCTSEQUENCE_H_INCLUDED__

#ifndef __CPLATFORMSEQUENCE_H_INCLUDED__ 
#include "CPlatformSequence.h"
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

#ifndef __PRODUCTSEQUENCE_H_INCLUDED__ 
#include "ProductSequence.h"
#endif
#include <vector>
class RProduct_API CProductSequence : public CPlatformSequence
{
	typedef struct InputUnitInfo
	{
		bool IsUnitPicked;
	};
	typedef struct FiducialFeedback
	{
		signed long XOffset;
		signed long YOffset;
		signed long ThetaOffset;
		int NoOfVerification;
		int Result;
	};
	typedef struct Position
	{
		signed long X_um;
		signed long Y_um;
		signed long Theta_um;
	};

	typedef struct ThetaResult
	{
		int firstNoOfUnit;
		int secondNoOfUnit;
		signed long ThetaOffset_mDegree;
	};
	LONGLONG  lSetupSequenceConvert1msTo100ns = 10000;
	LARGE_INTEGER  lnSetupSequenceDelayIn100ns;
public:
	//LARGE_INTEGER lnClockStart, lnClockEnd, lnClockSpan, lnClockStart2;
	//LARGE_INTEGER lnTrayTableSequenceClockStart, lnTrayTableSequenceClockEnd, lnTrayTableSequenceClockSpan, lnTrayTableSequenceClockStart2, lnTrayTableSequenceClockStart3;

	LARGE_INTEGER lnUPHClockStart, lnUPHClockEnd, lnUPHClockSpan;
	LARGE_INTEGER lnUPHClockPnP1Start, lnUPHClockPnP1End, lnUPHClockPnP1Span;
	LARGE_INTEGER lnUPHClockPnP2Start, lnUPHClockPnP2End, lnUPHClockPnP2Span;
	StationResult sClearResult;
	bool m_bNeedToMovePickAndPlace1XYAxis = false;
	bool m_bNeedToMovePickAndPlace2XYAxis = false;
	HomingSequenceNo nCase;
	bool m_bHighSpeed = false;
	CProductSequence();
	~CProductSequence();

	virtual int CProductSequence::SetProductSequence(CProductSequence *productSequence);
	int CProductSequence::SwitchInitializeSequence(int sequenceNo) override;
	int CProductSequence::SwitchHomeSequence(int sequenceNo) override;
	int CProductSequence::SwitchSetupSequence(int sequenceNo) override;
	int CProductSequence::SwitchJobSequence(int sequenceNo) override;
	int CProductSequence::SwitchEndingSequence(int sequenceNo) override;
	int CProductSequence::SwitchMaintenanceSequence(int sequenceNo) override;

	//int CProductSequence::InputLoadingUnloadingSequence();
	//int CProductSequence::OutputLoadingUnloadingSequence();
	int CProductSequence::PickAndPlace1Sequence();
	int CProductSequence::PickAndPlace2Sequence();
	//int CProductSequence::InputSequence();
	//int CProductSequence::OutputSequence();

	//int CProductSequence::TrayTableSequence();
	int CProductSequence::InputTrayTableSequence();
	int CProductSequence::OutputAndRejectTrayTableSequence();

	virtual int CProductSequence::ResetVariablesBeforeHome();
	virtual int CProductSequence::ResetVariablesBeforeSetup();
	virtual int CProductSequence::InitializeBeforeHome();
	virtual bool CProductSequence::IsReadyToHomeOrSetup();
	virtual bool CProductSequence::IsReadyToSetup();
	virtual int CProductSequence::SetUpBeforeHome();
	//virtual bool CProductSequence::IsSetUpDoneBeforeHome();
	virtual bool  CProductSequence::IsReadyToMove();
	virtual bool  CProductSequence::IsReadyToMoveProduction();

	bool CProductSequence::IsUnitReadyToPickOnInputTable();
	bool CProductSequence::IsNeedToPickUnitAndUnitPresent();
	bool CProductSequence::IsNeedToPickUnitButUnitNotPresent();
	bool CProductSequence::IsTotalPickedUnitIsSufficient();
	int CProductSequence::TotalUnitOnPUHAndModule();

	bool CProductSequence::IsPickAndPlaceModuleReadyToPickAtInput();
	bool CProductSequence::IsPickAndPlaceModuleReadyToPlaceAtOutput();
	bool CProductSequence::IsInputVisionReadyToGetOffset();
	bool CProductSequence::IsOutputVisionReadyToGetOffset();
	bool CProductSequence::IsCurrentPickupHeadNoUnit(int nNoOfPickupHead);
	bool CProductSequence::IsPickupHeadNoUnit();
	bool CProductSequence::IsInputTableNoUnit();
	bool CProductSequence::IsPickAndPlace1YAxisSaveToMoveCurve();
	bool CProductSequence::IsPickAndPlace1YAxisSaveToMoveCurveOutput();
	bool CProductSequence::IsPickAndPlace2YAxisSaveToMoveCurve();
	bool CProductSequence::IsPickAndPlace2YAxisSaveToMoveCurveOutput();
	int CProductSequence::GetNewXYOffsetFromThetaCorretion(double thetaCorrection_mDegree, int PickupHeadNo, double* newXOffset, double* newYOffset, bool AddToSleeve, bool relativeToSleeve);
	int CProductSequence::GetNewXYOffsetFromThetaCorretionMaintainance(double thetaCorrection_mDegree, int PickupHeadNo, double* newXOffset, double* newYOffset, bool relativeToSleeve);
	int CProductSequence::GetNewXYOffsetFromThetaCorrectionBasic(double thetaCorrection_mDegree, double XOffset, double YOffset, double* newXOffset, double* newYOffset);
	int CProductSequence::GetNewXYOffsetFromLookUpTable1(double thetaCorrection_mDegree, double* newXOffset, double* newYOffset);
	int CProductSequence::GetNewXYOffsetFromLookUpTable2(double thetaCorrection_mDegree, double* newXOffset, double* newYOffset);
	int CProductSequence::GetAllStationResult(int PickupHead);
	int CProductSequence::GetInputStationResult(int PickupHead);
	int CProductSequence::ResetInputStationResult();
	int CProductSequence::ResetAllStationResult();
	int CProductSequence::IsOutputNeedWriteReport();
};
CProductSequence *m_cProductSequence = new CProductSequence();
#endif