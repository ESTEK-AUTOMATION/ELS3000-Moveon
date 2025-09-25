#pragma once
#ifndef __CCUSTOMERIOCONTROL_H_INCLUDED__ 
#define __CCUSTOMERIOCONTROL_H_INCLUDED__

#ifndef __CPRODUCTIOCONTROL_H_INCLUDED__ 
#include "CProductIOControl.h"
#endif

#ifndef __CCUSTOMERSHAREVARIABLES_H_INCLUDED__ 
#include "CCustomerShareVariables.h"
#endif

#ifndef __CCUSTOMERSHAREDMEMORY_H_INCLUDED__ 
#include "CCustomerSharedMemory.h"
#endif

#include "RCustomer.h"

class RCustomer_API CCustomerIOControl : public CProductIOControl 
{
public:
	CCustomerIOControl();
	~CCustomerIOControl();
};

CCustomerIOControl *m_cCustomerIOControl = new CCustomerIOControl();
#endif