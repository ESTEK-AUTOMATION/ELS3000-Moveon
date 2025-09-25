//////////////////////////////////////////////////////////////////
//
// RProduct.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 12/26/2019 8:38:29 PM
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////

#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS

#include <SDKDDKVer.h>

//#include <stdio.h>
//#include <string.h>
//#include <ctype.h>
//#include <conio.h>
//#include <stdlib.h>
//#include <math.h>
//#include <errno.h>
#include <windows.h>
#include <tchar.h>
#ifndef __WINRTXCONVERSION_H_INCLUDED__
#include "WinRTXConversion.h"
#endif
#ifndef _RTAPI_H_
#ifndef UNDER_WIN
#include <rtapi.h>      // RTX64 APIs that can be used in real-time or Windows applications.
#endif
#endif
#ifdef UNDER_RTSS
#include <rtssapi.h>    // RTX64 APIs that can only be used in real-time applications.
#endif // UNDER_RTSS

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define RProduct_API __declspec(dllexport)
  #else
    #define RProduct_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define RProduct_API __declspec(dllexport)
  #else
    #define RProduct_API __declspec(dllimport)
  #endif
#endif


// Add DEFINES Here
// This class is exported from the RProduct.dll
class RProduct_API CRProduct
{
public:
    CRProduct(void);
    // TODO: add your methods here.
};
extern RProduct_API int nRProduct;


// function prototype for the RTDLL exported function
//RProduct_API
//int 
//RTAPI
//Toggle(int argc, TCHAR * argv[]);
 
