#pragma once
#ifndef __CCUSTOMERTHREAD_H_INCLUDED__ 
#define __CCUSTOMERTHREAD_H_INCLUDED__

#ifndef __CPRODUCTTHREAD_H_INCLUDED__ 
#include "CProductThread.h"
#endif

#ifndef __CCUSTOMERSEQUENCE_H_INCLUDED__ 
#include "CCustomerSequence.h"
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

class RCustomer_API CCustomerThread : public CProductThread
{
public:
	CCustomerThread();
	~CCustomerThread();
	int CCustomerThread::IOOperationThreadWhileInJobMode() override;
};
CCustomerThread *m_cCustomerThread = new CCustomerThread();
#endif