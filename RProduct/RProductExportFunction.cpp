//////////////////////////////////////////////////////////////////
//
// RProductRTDLLExportFunction.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 12/26/2019 8:38:29 PM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////
    
#include "RProduct.h"

// An exported variable.
RProduct_API int nRProduct = 0;

// Exported RTDLL function
RProduct_API
int 
RTAPI
Toggle(int argc, TCHAR * argv[])
{
    return 0;
}


// This is the constructor of a class that has been exported.
// see RProduct.h for the class definition
CRProduct::CRProduct()
{
        return;
}

