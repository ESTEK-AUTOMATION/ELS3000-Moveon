#pragma once
#ifndef __CCUSTOMERSEQUENCE_H_INCLUDED__ 
#define __CCUSTOMERSEQUENCE_H_INCLUDED__

#ifndef __CPRODUCTSEQUENCE_H_INCLUDED__ 
#include "CProductSequence.h"
#endif

#ifndef __CCUSTOMERSTATECONTROL_H_INCLUDED__ 
#include "CCustomerStateControl.h"
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

class RCustomer_API CCustomerSequence : public CProductSequence
{
public:
	CCustomerSequence();
	~CCustomerSequence();
};
CCustomerSequence *m_cCustomerSequence = new CCustomerSequence();
#endif