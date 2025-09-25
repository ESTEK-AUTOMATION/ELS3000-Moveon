#pragma once
#ifndef __CPLATFORMIOCONTROL_H_INCLUDED__ 
#define __CPLATFORMIOCONTROL_H_INCLUDED__

#ifndef __CIO_H_INCLUDED__
#include "IO.h"
#endif

#ifndef __CPLATFORMSHAREVARIABLES_H_INCLUDED__ 
#include "CPlatformShareVariables.h"
#endif

#ifndef __CPLATFORMSHAREDMEMORY_H_INCLUDED__ 
#include "CPlatformSharedMemory.h"
#endif

class RPlatform_API CPlatformIOControl
{
public:
	CPlatformIOControl();
	~CPlatformIOControl();
	virtual int RTFCNDCL CPlatformIOControl::SetPlatformIOControl(CPlatformIOControl *platformIOControl);
	virtual void CPlatformIOControl::SetOutputBeforeExitSoftware();
	virtual bool CPlatformIOControl::SetTowerLightStartHome();
	virtual bool CPlatformIOControl::SetTowerLightHoming();
	virtual bool CPlatformIOControl::SetTowerLightRunning();
	virtual bool CPlatformIOControl::SetTowerLightIdle();
	virtual bool CPlatformIOControl::SetTowerLightAlarm();
	virtual bool CPlatformIOControl::IsSetTowerLightAmber();
};

CIO *m_cIO = new CIO();
CPlatformIOControl *m_cPlatformIOControl = new CPlatformIOControl();
#endif