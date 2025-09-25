//////////////////////////////////////////////////////////////////
//
// RtxLibrary.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 1/11/2018 6:02:20 PM
// User: Estek
//
//////////////////////////////////////////////////////////////////

#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS

#include <SDKDDKVer.h>

//#include <stdio.h>
#include <string.h>
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

#include <string>
#include <map>
#include "CCustomerSharedMemory.h"

using namespace std;

#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define RtxLibrary_API __declspec(dllexport)
  #else
    #define RtxLibrary_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define RtxLibrary_API __declspec(dllexport)
  #else
    #define RtxLibrary_API __declspec(dllimport)
  #endif
#endif

#define DllExport extern "C" __declspec(dllexport)

// Add DEFINES Here
// This class is exported from the RtxLibrary.dll
class RtxLibrary_API CRtxLibrary
{
public:
    CRtxLibrary(void);
    // TODO: add your methods here.
};
extern RtxLibrary_API int nRtxLibrary;


// function prototype for the RTDLL exported function
RtxLibrary_API
int 
RTAPI
Toggle(int argc, TCHAR * argv[]);
 

DllExport RtxLibrary_API long RunUtility(PCHAR lpCmd);
DllExport RtxLibrary_API int InitializeSharedMemory(void);
DllExport RtxLibrary_API int ResetShareMemoryEvent();
DllExport RtxLibrary_API int ReleaseSharedMemory();

DllExport RtxLibrary_API void SetShareMemorySettingBool(LPSTR settingName, bool enable);
DllExport RtxLibrary_API int GetShareMemorySettingBool(LPSTR settingName);
DllExport RtxLibrary_API int GetShareMemorySettingArrayBool(LPSTR settingName, int arrayNo);
DllExport RtxLibrary_API void SetShareMemorySettingArrayBool(LPSTR settingName, int arrayNo, bool enable);

DllExport RtxLibrary_API int GetShareMemorySettingArray(LPSTR parameterName, int arrayNo, LPSTR resultName);
DllExport RtxLibrary_API void SetShareMemorySettingArray(LPSTR parameterName, int arrayNo, LPSTR resultName, int parameterValue);

DllExport RtxLibrary_API signed long GetShareMemorySettingLong(LPSTR settingName);
DllExport RtxLibrary_API void SetShareMemorySettingLong(LPSTR settingName, long value);
DllExport RtxLibrary_API signed long GetShareMemorySettingArrayLong(LPSTR settingName, int arrayNo);
DllExport RtxLibrary_API void SetShareMemorySettingArrayLong(LPSTR settingName, int arrayNo, long value);
DllExport RtxLibrary_API int GetShareMemorySettingUInt(LPSTR settingName);
DllExport RtxLibrary_API void SetShareMemorySettingUInt(LPSTR settingName, unsigned int value);
DllExport RtxLibrary_API double GetShareMemorySettingDouble(LPSTR generalName);
DllExport RtxLibrary_API void SetShareMemorySettingDouble(LPSTR generalName, double value);

DllExport RtxLibrary_API signed long GetShareMemoryTeachPointLong(LPSTR teachPointName);
DllExport RtxLibrary_API void SetShareMemoryTeachPointLong(LPSTR teachPointName, signed long teachPointValue);

DllExport RtxLibrary_API int GetShareMemoryProductionBool(LPSTR parameterName);
DllExport RtxLibrary_API void SetShareMemoryProductionBool(LPSTR parameterName, bool enable);

DllExport RtxLibrary_API int GetShareMemoryProductionInt(LPSTR parameterName);
DllExport RtxLibrary_API void SetShareMemoryProductionInt(LPSTR parameterName, int parameterValue);

DllExport RtxLibrary_API void GetShareMemoryProductionString(LPSTR generalName, char *stringWord);
DllExport RtxLibrary_API void SetShareMemoryProductionString(LPSTR generalName, LPSTR message);

DllExport RtxLibrary_API signed long GetShareMemoryProductionLong(LPSTR parameterName);
DllExport RtxLibrary_API void SetShareMemoryProductionLong(LPSTR parameterName, signed long parameterValue);
DllExport RtxLibrary_API int GetShareMemoryProductionArray(LPSTR parameterName, int arrayNo, LPSTR resultName);
DllExport RtxLibrary_API void SetShareMemoryProductionArray(LPSTR parameterName, int arrayNo, LPSTR resultName, int parameterValue);
DllExport RtxLibrary_API int GetShareMemoryProductionPickUpHeadResult(int stationNo, LPSTR resultName);
DllExport RtxLibrary_API void SetShareMemoryProductionPickUpHeadResult(int stationNo, LPSTR resultName, int resultValue);
DllExport RtxLibrary_API double GetShareMemoryProductionDoubleArray(LPSTR parameterName, int arrayNo, LPSTR resultName);
DllExport RtxLibrary_API void SetShareMemoryProductionDoubleArray(LPSTR parameterName, int arrayNo, LPSTR resultName, double parameterValue);
DllExport RtxLibrary_API int GetShareMemoryProductionPatternResultInt(int patternNo, int resultNo, LPSTR parameterName, LPSTR resultName, int resultValue);
DllExport RtxLibrary_API void SetShareMemoryProductionPatternResultInt(int patternNo, int resultNo, LPSTR parameterName, LPSTR resultName, int resultValue);
DllExport RtxLibrary_API void GetShareMemoryProductionPatternResultString(int patternNo, int resultNo, LPSTR parameterName, LPSTR resultName, char *resultValue, int resultLength);
DllExport RtxLibrary_API void SetShareMemoryProductionPatternResultString(int patternNo, int resultNo, LPSTR parameterName, LPSTR resultName, LPSTR resultValue, int resultLength);

DllExport RtxLibrary_API int GetShareMemoryProductionArrayBool(LPSTR parameterName, int arrayNo);
DllExport RtxLibrary_API void SetShareMemoryProductionArrayBool(LPSTR parameterName, int arrayNo, bool enable);

DllExport RtxLibrary_API double GetShareMemoryProductionDouble(LPSTR parameterName);
DllExport RtxLibrary_API void SetShareMemoryProductionDouble(LPSTR parameterName, double value);

DllExport RtxLibrary_API int GetShareMemoryCustomizeBool(LPSTR customizeName);
DllExport RtxLibrary_API void SetShareMemoryCustomizeBool(char* customizeName, bool enable);

DllExport RtxLibrary_API int GetShareMemoryModuleStatusBool(LPSTR moduleStatusName);
DllExport RtxLibrary_API void SetShareMemoryModuleStatusBool(LPSTR moduleStatusName, bool enable);

DllExport RtxLibrary_API int GetShareMemoryIOArrayInt(LPSTR IOName, int arrayNo);
DllExport RtxLibrary_API void SetShareMemoryIOArrayInt(LPSTR IOName, int arrayNo, int IOValue);

DllExport RtxLibrary_API int GetShareMemoryEvent(LPSTR eventName);
DllExport RtxLibrary_API void SetShareMemoryEvent(LPSTR eventName, bool state);

DllExport RtxLibrary_API int GetShareMemoryGeneralInt(LPSTR generalName);
DllExport RtxLibrary_API void SetShareMemoryGeneralInt(LPSTR generalName, int value);
DllExport RtxLibrary_API double GetShareMemoryGeneralDouble(LPSTR generalName);
DllExport RtxLibrary_API void SetShareMemoryGeneralDouble(LPSTR generalName, double value);
DllExport RtxLibrary_API void GetShareMemoryGeneralString(LPSTR generalName, char *stringWord, int len);
DllExport RtxLibrary_API void SetShareMemoryGeneralString(LPSTR generalName, LPSTR message);

DllExport RtxLibrary_API void TimerHandler(PVOID    context);
DllExport RtxLibrary_API LPSTR TestPassAndReturnString(LPSTR generalName);
DllExport RtxLibrary_API void TestPassAndReturnString1(LPSTR generalName, char *stringWord, int len);

int MapShareMemory();
int MapSettingBoolPlatform(map<string, bool*> *mapSettingBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingBoolProduct(map<string, bool*> *mapSettingBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingBoolCustomer(map<string, bool*> *mapSettingBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArrayBoolPlatform(map<string, bool*> *mapSettingArrayBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArrayBoolProduct(map<string, bool*> *mapSettingArrayBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArrayBoolCustomer(map<string, bool*> *mapSettingArrayBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingSignedLongPlatform(map<string, signed long*> *mapSettingSignedLongBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingSignedLongProduct(map<string, signed long*> *mapSettingSignedLongBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingSignedLongCustomer(map<string, signed long*> *mapSettingSignedLongBool, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArraySignedLongPlatform(map<string, signed long*> *mapSettingArraySignedLong, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArraySignedLongProduct(map<string, signed long*> *mapSettingArraySignedLong, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingArraySignedLongCustomer(map<string, signed long*> *mapSettingArraySignedLong, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingUnsignedIntPlatform(map<string, unsigned int*> *mapSettingUnsignedInt, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingUnsignedIntProduct(map<string, unsigned int*> *mapSettingUnsignedInt, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingUnsignedIntCustomer(map<string, unsigned int*> *mapSettingUnsignedInt, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingDoublePlatform(map<string, double*> *mapSettingDouble, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingDoubleProduct(map<string, double*> *mapSettingDouble, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapSettingDoubleCustomer(map<string, double*> *mapSettingDouble, CustomerSharedMemorySetting *customerSharedMemorySetting);

int MapSettingVisionInfoIntProduct(map<string, int*> *mapSettingVisionInfoInt, CustomerSharedMemorySetting *customerSharedMemorySetting);
int MapProductionArrayDoubleProduct(map<string, double*> *mapProductionArrayDouble, CustomerSharedMemoryProduction *smCustomerProduction);


int MapTeachPointSignedLongPlatform(map<string, signed long*> *mapTeachPointSignedLong, CustomerSharedMemoryTeachPoint *smCustomerTeachPoint);
int MapTeachPointSignedLongProduct(map<string, signed long*> *mapTeachPointSignedLong, CustomerSharedMemoryTeachPoint *smCustomerTeachPoint);
int MapTeachPointSignedLongCustomer(map<string, signed long*> *mapTeachPointSignedLong, CustomerSharedMemoryTeachPoint *smCustomerTeachPoint);
int MapProductionStationResultIntPlatform(map<string, int*> *mapProductionStationResultInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionStationResultIntProduct(map<string, int*> *mapProductionStationResultInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionStationResultIntCustomer(map<string, int*> *mapProductionStationResultInt, CustomerSharedMemoryProduction *smCustomerProduction);

int MapProductionIntPlatform(map<string, int*> *mapProductionInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionIntProduct(map<string, int*> *mapProductionInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionIntCustomer(map<string, int*> *mapProductionInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionStringProduct(map<string, char*> *mapProductionString, CustomerSharedMemoryProduction *customerProduction);
int MapProductionSignedLongPlatform(map<string, signed long*> *mapProductionSignedLong, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionSignedLongProduct(map<string, signed long*> *mapProductionSignedLong, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionSignedLongCustomer(map<string, signed long*> *mapProductionSignedLong, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionArrayIntPlatform(map<string, int*> *mapProductionArrayInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionArrayIntProduct(map<string, int*> *mapProductionArrayInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionArrayIntCustomer(map<string, int*> *mapProductionArrayInt, CustomerSharedMemoryProduction *smCustomerProduction);
int MapProductionBoolProduct(map<string, bool*> *mapProductionBool, CustomerSharedMemoryProduction *customerProduction);
int MapProductionArrayBoolProduct(map<string, bool*> *mapProductionArrayBool, CustomerSharedMemoryProduction *customerProduction);
int MapProductionDoubleProduct(map<string, double*> *mapProductionDouble, CustomerSharedMemoryProduction *customerProduction);
int MapCustomizeBoolPlatform(map<string, bool*> *mapCustomizeBool, CustomerSharedMemoryCustomize *smCustomerCustomize);
int MapCustomizeBoolProduct(map<string, bool*> *mapCustomizeBool, CustomerSharedMemoryCustomize *smCustomerCustomize);
int MapCustomizeBoolCustomer(map<string, bool*> *mapCustomizeBool, CustomerSharedMemoryCustomize *smCustomerCustomize);
int MapIOArrayUnsignedLongPlatform(map<string, unsigned long*> *mapIOArrayUnsignedLong, CustomerSharedMemoryIO *smCustomerIO);
int MapIOArrayUnsignedLongProduct(map<string, unsigned long*> *mapIOArrayUnsignedLong, CustomerSharedMemoryIO *smCustomerIO);
int MapIOArrayUnsignedLongCustomer(map<string, unsigned long*> *mapIOArrayUnsignedLong, CustomerSharedMemoryIO *smCustomerIO);
int MapEventPlatform(map<string, sEvent*> *mapEvent, CustomerSharedMemoryEvent *smCustomerEvent);
int MapEventProduct(map<string, sEvent*> *mapEvent, CustomerSharedMemoryEvent *smCustomerEvent);
int MapEventCustomer(map<string, sEvent*> *mapEvent, CustomerSharedMemoryEvent *smCustomerEvent);
int MapGeneralIntPlatform(map<string, int*> *mapGeneralInt, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralIntProduct(map<string, int*> *mapGeneralInt, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralIntCustomer(map<string, int*> *mapGeneralInt, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralSignedLongPlatform(map<string, signed long*> *mapGeneralSignedlong, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralSignedLongProduct(map<string, signed long*> *mapGeneralSignedlong, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralSignedLongCustomer(map<string, signed long*> *mapGeneralSignedlong, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralDoublePlatform(map<string, double*> *mapGeneralDouble, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralDoubleProduct(map<string, double*> *mapGeneralDouble, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralDoubleCustomer(map<string, double*> *mapGeneralDouble, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralStringPlatform(map<string, char*> *mapGeneralString, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneraStringProduct(map<string, char*> *mapGeneralString, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralStringCustomer(map<string, char*> *mapGeneralString, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralLargeIntegerPlatform(map<string, LARGE_INTEGER*> *mapGeneralLargeInteger, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneraLargeIntegerProduct(map<string, LARGE_INTEGER*> *mapGeneralLargeInteger, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapGeneralLargeIntegerCustomer(map<string, LARGE_INTEGER*> *mapGeneralLargeInteger, CustomerSharedMemoryGeneral *smCustomerGeneral);
int MapModuleStatusBoolProduct(map<string, bool*> *mapModuleStatusBool, CustomerSharedMemoryModuleStatus *customerModuleStatus);
int MapProductionPatternRecognizationIntProduct(map<string, int*> *mapProductionPatternRecognizationInt, CustomerSharedMemoryProduction * smCustomerProduction);
int MapProductionPatternRecognizationStringProduct(map<string, char*> *mapProductionPatternRecognizationString, CustomerSharedMemoryProduction *smCustomerProduction);
