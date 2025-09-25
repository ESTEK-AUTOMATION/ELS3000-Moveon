#pragma once
#ifndef __CPLATFORMMOTORCONTROL_H_INCLUDED__ 
#define __CPLATFORMMOTORCONTROL_H_INCLUDED__

#ifndef __MOTIONLIBRARY_H_INCLUDED__
#include "MotionLibrary.h"
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

class RPlatform_API CPlatformMotorControl
{
public:
	CPlatformMotorControl();
	~CPlatformMotorControl();
	virtual int CPlatformMotorControl::SetPlatformMotorControl(CPlatformMotorControl *platformMotorControl);
	virtual int CPlatformMotorControl::LoadMotorSetting();
};

CMotionLibrary *m_cMotion = new CMotionLibrary();
CPlatformMotorControl *m_cPlatformMotorControl = new CPlatformMotorControl();
#endif