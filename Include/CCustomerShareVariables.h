#pragma once
#ifndef __CCUSTOMERSHAREVARIABLES_H_INCLUDED__ 
#define __CCUSTOMERSHAREVARIABLES_H_INCLUDED__

#ifndef __CPRODUCTSHAREVARIABLES_H_INCLUDED__ 
#include "CProductShareVariables.h"
#endif

#ifndef __CCUSTOMERSHAREDMEMORY_H_INCLUDED__ 
#include "CCustomerSharedMemory.h"
#endif

#include "RCustomer.h"

class RCustomer_API CCustomerShareVariables : public CProductShareVariables
{
public:
	CCustomerShareVariables();
	~CCustomerShareVariables();
};

CCustomerShareVariables *m_cCustomerShareVariables = new CCustomerShareVariables();
#endif