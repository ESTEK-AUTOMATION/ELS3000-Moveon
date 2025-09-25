#pragma once
#ifndef __CCUSTOMERSHAREDMEMORY_H_INCLUDED__ 
#define __CCUSTOMERSHAREDMEMORY_H_INCLUDED__

#ifndef __CPRODUCTSHAREDMEMORY_H_INCLUDED__ 
#include "CProductSharedMemory.h"
#endif

#include "RCustomer.h"

class RCustomer_API CustomerSharedMemorySetting : public ProductSharedMemorySetting
{
};

class RCustomer_API CustomerSharedMemoryTeachPoint : public ProductSharedMemoryTeachPoint
{
};

class RCustomer_API CustomerSharedMemoryProduction : public ProductSharedMemoryProduction
{
};

class RCustomer_API CustomerSharedMemoryCustomize : public ProductSharedMemoryCustomize
{
};

class RCustomer_API CustomerSharedMemoryIO : public ProductSharedMemoryIO
{
};

class RCustomer_API CustomerSharedMemoryModuleStatus : public ProductSharedMemoryModuleStatus
{
};

class RCustomer_API CustomerSharedMemoryEvent : public ProductSharedMemoryEvent
{
};

class RCustomer_API CustomerSharedMemoryGeneral : public ProductSharedMemoryGeneral
{
};


CustomerSharedMemorySetting *smCustomerSetting = NULL;
CustomerSharedMemoryTeachPoint *smCustomerTeachPoint = NULL;
CustomerSharedMemoryProduction *smCustomerProduction = NULL;
CustomerSharedMemoryCustomize *smCustomerCustomize = NULL;
CustomerSharedMemoryIO *smCustomerIO = NULL;
CustomerSharedMemoryModuleStatus *smCustomerModuleStatus = NULL;
CustomerSharedMemoryEvent *smCustomerEvent = NULL;
CustomerSharedMemoryGeneral *smCustomerGeneral = NULL;

#endif

