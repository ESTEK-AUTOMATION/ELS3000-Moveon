#pragma once
#ifndef __CPRODUCTSHAREVARIABLES_H_INCLUDED__ 
#define __CPRODUCTSHAREVARIABLES_H_INCLUDED__

#ifndef __CPLATFORMSHAREVARIABLES_H_INCLUDED__ 
#include "CPlatformShareVariables.h"
#endif

#ifndef __CPRODUCTSHAREDMEMORY_H_INCLUDED__ 
#include "CProductSharedMemory.h"
#endif

#include "RProduct.h"

LONGLONG m_TurretDelay = 140;

class RProduct_API CProductShareVariables : public CPlatformShareVariables
{
public:
	CProductShareVariables();
	~CProductShareVariables();
	virtual int RTFCNDCL CProductShareVariables::SetProductShareVariables(CProductShareVariables *productShareVariables);
};
CProductShareVariables *m_cProductShareVariables = new CProductShareVariables();
#endif

