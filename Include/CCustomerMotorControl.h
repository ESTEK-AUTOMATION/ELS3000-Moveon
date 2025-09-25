#pragma once
#ifndef __CCUSTOMERMOTORCONTROL_H_INCLUDED__ 
#define __CCUSTOMERMOTORCONTROL_H_INCLUDED__

#ifndef __CPRODUCTMOTORCONTROL_H_INCLUDED__ 
#include "CProductMotorControl.h"
#endif

#ifndef __CCUSTOMERIOCONTROL_H_INCLUDED__
#include "CCustomerIOControl.h"
#endif

#ifndef __CCUSTOMERSHAREVARIABLES_H_INCLUDED__ 
#include "CCustomerShareVariables.h"
#endif

#ifndef __CCUSTOMERSHAREDMEMORY_H_INCLUDED__ 
#include "CCustomerSharedMemory.h"
#endif

#include "RCustomer.h"

class RCustomer_API CCustomerMotorControl : public CProductMotorControl
{
public:
	CCustomerMotorControl();
	~CCustomerMotorControl();
};

CCustomerMotorControl *m_cCustomerMotorControl = new CCustomerMotorControl();
#endif