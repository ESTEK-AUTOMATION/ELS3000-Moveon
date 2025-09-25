//////////////////////////////////////////////////////////////////
//
// RCustomerRTDLLExportFunction.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 12/26/2019 8:38:47 PM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////
    
#include "RCustomer.h"

// An exported variable.
RCustomer_API int nRCustomer = 0;

// Exported RTDLL function
RCustomer_API
int 
RTAPI
Toggle(int argc, TCHAR * argv[])
{
    return 0;
}


// This is the constructor of a class that has been exported.
// see RCustomer.h for the class definition
CRCustomer::CRCustomer()
{
        return;
}

