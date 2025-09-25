#pragma once
#ifndef __CPLATFORMSEQUENCE_H_INCLUDED__ 
#define __CPLATFORMSEQUENCE_H_INCLUDED__

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

class RPlatform_API CPlatformSequence
{
public:
	LARGE_INTEGER lnClockStart, lnClockEnd, lnClockSpan, lnClockStart2;
	LARGE_INTEGER lnInitializeSequenceClockStart, lnInitializeSequenceClockEnd, lnInitializeSequenceClockSpan, lnInitializeSequenceClockStart2;
	LARGE_INTEGER lnHomeSequenceClockStart, lnHomeSequenceClockEnd, lnHomeSequenceClockSpan, lnHomeSequenceClockStart2;
	LARGE_INTEGER lnSetupSequenceClockStart, lnSetupSequenceClockEnd, lnSetupSequenceClockSpan, lnSetupSequenceClockStart2;
	LARGE_INTEGER lnJobSequenceClockStart, lnJobSequenceClockEnd, lnJobSequenceClockSpan, lnJobSequenceClockStart2;
	LARGE_INTEGER lnEndingSequenceClockStart, lnEndingSequenceClockEnd, lnEndingSequenceClockSpan, lnEndingSequenceClockStart2;
	LARGE_INTEGER lnMaintenanceSequenceClockStart, lnMaintenanceSequenceClockEnd, lnMaintenanceSequenceClockSpan, lnMaintenanceSequenceClockStart2;

	CPlatformSequence();
	~CPlatformSequence();

	int CPlatformSequence::SetPlatformSequence(CPlatformSequence *platformSequence);
	int CPlatformSequence::SetPlatformVariables(LARGE_INTEGER *plnClockStart, LARGE_INTEGER *plnClockEnd, LARGE_INTEGER *plnClockSpan, LARGE_INTEGER *plnClockStart2
		, LARGE_INTEGER *plnInitializeSequenceClockStart, LARGE_INTEGER *plnInitializeSequenceClockEnd, LARGE_INTEGER *plnInitializeSequenceClockSpan, LARGE_INTEGER *plnInitializeSequenceClockStart2
		, LARGE_INTEGER *plnHomeSequenceClockStart, LARGE_INTEGER *plnHomeSequenceClockEnd, LARGE_INTEGER *plnHomeSequenceClockSpan, LARGE_INTEGER *plnHomeSequenceClockStart2
		, LARGE_INTEGER *plnSetupSequenceClockStart, LARGE_INTEGER *plnSetupSequenceClockEnd, LARGE_INTEGER *plnSetupSequenceClockSpan, LARGE_INTEGER *plnSetupSequenceClockStart2
		, LARGE_INTEGER *plnJobSequenceClockStart, LARGE_INTEGER *plnJobSequenceClockEnd, LARGE_INTEGER *plnJobSequenceClockSpan, LARGE_INTEGER *plnJobSequenceClockStart2
		, LARGE_INTEGER *plnEndingSequenceClockStart, LARGE_INTEGER *plnEndingSequenceClockEnd, LARGE_INTEGER *plnEndingSequenceClockSpan, LARGE_INTEGER *plnEndingSequenceClockStart2
		, LARGE_INTEGER *plnMaintenanceSequenceClockStart, LARGE_INTEGER *plnMaintenanceSequenceClockEnd, LARGE_INTEGER *plnMaintenanceSequenceClockSpan, LARGE_INTEGER *plnMaintenanceSequenceClockStart2);
	int CPlatformSequence::InitializeSequence();
	int CPlatformSequence::HomeSequence();
	int CPlatformSequence::SetupSequence();
	int CPlatformSequence::JobSequence();
	int CPlatformSequence::EndingSequence();
	int CPlatformSequence::MaintenanceSequence();
	virtual int CPlatformSequence::SwitchInitializeSequence(int sequenceNo);
	virtual int CPlatformSequence::SwitchHomeSequence(int sequenceNo);
	virtual int CPlatformSequence::SwitchSetupSequence(int sequenceNo);
	virtual int CPlatformSequence::SwitchJobSequence(int sequenceNo);
	virtual int CPlatformSequence::SwitchEndingSequence(int sequenceNo);
	virtual int CPlatformSequence::SwitchMaintenanceSequence(int sequenceNo);

};

CPlatformSequence *m_cPlatformSequence = new CPlatformSequence();

#endif