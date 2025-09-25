#pragma once
#ifndef __CCUSTOMERSTATECONTROL_H_INCLUDED__ 
#define __CCUSTOMERSTATECONTROL_H_INCLUDED__

#ifndef __CPRODUCTSTATECONTROL_H_INCLUDED__ 
#include "CProductStateControl.h"
#endif

#ifndef __CCUSTOMERMOTORCONTROL_H_INCLUDED__ 
#include "CCustomerMotorControl.h"
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

class RCustomer_API CCustomerStateControl : public CProductStateControl
{
public:
	CCustomerStateControl();
	~CCustomerStateControl();
};
CCustomerStateControl *m_cCustomerStateControl = new CCustomerStateControl();
#endif