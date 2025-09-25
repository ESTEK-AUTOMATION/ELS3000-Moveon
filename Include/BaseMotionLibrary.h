#ifndef BASEMOTIONLIBRARY_H
#define BASEMOTIONLIBRARY_H

#pragma once
#ifndef MOTOR_H
#include "Motor.h"
#endif

#ifdef INSTALL_GALIL_DRIVER
#ifndef GALILCONTROLLER_H
#include "GalilController.h"
#endif
#endif

#ifdef INSTALL_ETEL_DRIVER
#ifndef ETELCONTROLLER_H
#include "EtelController.h"
#endif
#endif

#ifdef INSTALL_KINGSTAR_DRIVER
#ifndef KINGSTARCONTROLLER_H
#include "KingstarController.h"
#endif
#endif

#ifndef __IMOTIONLIBRARY_H_INCLUDED__
#include "imotionlibrary.h"
#endif

class BaseMotionLibrary :
	public iMotionLibrary
{
public:
	BaseMotionLibrary(void);
	virtual ~BaseMotionLibrary(void);
	
#pragma region variable
	int m_nErrorCode;
	LPSTR m_strErrorMsg;
	bool m_bOnline;
	MotorConfiguration m_MotorConfiguration[MOTORQUANTITY];
	//MotorConfiguration* m_MotorConfiguration(nullptr);
	OfflineParameter m_MotorOfflineStatus[MOTORQUANTITY];
#pragma endregion

#pragma region method
public:	
	virtual int Configure(bool Online);
	virtual int Initialize();
	virtual int GetLastErrorCode();
	virtual LPCTSTR GetErrorMsg(int errorCode);
	virtual void ClearError();

	virtual int Initialize(bool online, MotorConfiguration* motorConfiguration, int motorQuantity);
	virtual int MoveRelative(LPSTR axis, MotorProfile motionProfile);
	virtual int MoveAbsolute(LPSTR axis, MotorProfile motionProfile);
	virtual int StopMovement(LPSTR axis);
	virtual bool InPosition(LPSTR axis);
	virtual bool IsForwardLimitOn(LPSTR axis);
	virtual bool IsReverseLimitOn(LPSTR axis);
	virtual bool IsHomeSensorOn(LPSTR axis);

#pragma endregion
};

#pragma region Error Codes
#define SUCCESS = 0;
#define ERROR_INVALID_MOTOR_CONFIGURATION 1;
#define ERROR_INVALID_CONTROLLER 2;
#define ERROR_INVALID_CONTROLLER_NO 3;
#define ERROR_INVALID_MOTOR_AXIS 4;
#define ERROR_FUNCTION_NOT_SUPPORTED 5;

#pragma endregion
#endif