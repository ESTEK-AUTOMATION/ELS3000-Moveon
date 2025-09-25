//////////////////////////////////////////////////////////////////
//
// WinRTXConversion.cpp - cpp file
//
// This file was generated using the RTX64 RTDLL Template for Visual Studio.
//
// Created: 4/26/2021 1:32:55 PM
// User: User-PC
//
//////////////////////////////////////////////////////////////////

#pragma once
//This define will deprecate all unsupported Microsoft C-runtime functions when compiled under RTSS.
//If using this define, #include <rtapi.h> should remain below all windows headers
//#define UNDER_RTSS_UNSUPPORTED_CRT_APIS
#ifndef __WINRTXCONVERSION_H_INCLUDED__
#define __WINRTXCONVERSION_H_INCLUDED__

#ifndef _INC_SDKDDKVER
#include <SDKDDKVer.h>
#endif

#include <stdio.h>
//#include <string.h>
//#include <ctype.h>
//#include <conio.h>
//#include <stdlib.h>
//#include <math.h>
//#include <errno.h>
#ifndef _WINDOWS_
#include <windows.h>
#endif
#ifndef _INC_TCHAR
#include <tchar.h>
#endif
#ifndef _RTAPI_H_
#ifdef UNDER_WIN
#define _RTAPI_H_
#define RTAPI __stdcall
#define RTFCNDCL __stdcall

#define	RtGetLastError		GetLastError
#define	RtSetLastError		SetLastError
#define RtCreateThread		CreateThread
#define	RtExitThread		ExitThread
#define	RtGetCurrentThread	GetCurrentThread
#define	RtResumeThread		ResumeThread
#define	RtSuspendThread		SuspendThread
#define	RtTerminateThread	TerminateThread
#define	RtExitProcess		ExitProcess
#define	RtGetStdHandle		GetStdHandle
#define	RtSleep				Sleep
#define	RtSleepEx			SleepEx
#ifdef UNICODE
#define	RtWriteConsole		WriteConsoleW
#define	RtGetCommandLine	GetCommandLineW
#else
#define	RtWriteConsole		WriteConsoleA
#define	RtGetCommandLine	GetCommandLineA
#endif // !UNICODE

#define	RtPrintf printf
#define RtSetThreadPriority SetThreadPriority
#define RT_PRIORITY_MAX 127
#define RT_PRIORITY_MIN 0
#define CLOCK_FASTEST	0xFFFF	// Fastest available clock & timer
#define RtGetClockTime GetClockTime
#define RtSleepFt SleepFt
#define RtCreateSharedMemory WdCreateSharedMemory
#define RtCloseHandle CloseHandle

typedef unsigned long CLOCK, *PCLOCK;

#define CLOCK_1		1	// System Clock
#define CLOCK_2		2	// Real-time HAL Clock
#define CLOCK_3		3
#define CLOCK_FASTEST	0xFFFF	// Fastest available clock & timer
#define CLOCK_SYSTEM	CLOCK_1

#if (!defined(_NTDDK_)&& !defined(__RTX_H__))
typedef enum _MEMORY_CACHING_TYPE
{
	MmNonCached,
	MmCached,
	MmWriteCombined
} MEMORY_CACHING_TYPE;
#endif

#if ( !defined(_NTDDK_) && !defined(_NTHAL_H_) && !defined(__RTX_H__) )
//
// Define the I/O bus interface types.
//
typedef enum _INTERFACE_TYPE {
	InterfaceTypeUndefined = -1,
	Internal,
	Isa,
	Eisa,
	MicroChannel,
	TurboChannel,
	PCIBus,
	VMEBus,
	NuBus,
	PCMCIABus,
	CBus,
	MPIBus,
	MPSABus,
	ProcessorInternal,
	InternalPowerBus,
	PNPISABus,
	MaximumInterfaceType
}INTERFACE_TYPE, *PINTERFACE_TYPE;


typedef enum _KINTERRUPT_MODE
{
	LevelSensitive,
	Latched
}KINTERRUPT_MODE;

#endif

#if ( !defined(_NTDDK_) )
//
// Define types of bus information.
//
typedef enum _BUS_DATA_TYPE
{
	ConfigurationSpaceUndefined = -1,
	Cmos,
	EisaConfiguration,
	Pos,
	CbusConfiguration,
	PCIConfiguration,
	VMEConfiguration,
	NuBusConfiguration,
	PCMCIAConfiguration,
	MPIConfiguration,
	MPSAConfiguration,
	PNPISAConfiguration,
	MaximumBusDataType
}BUS_DATA_TYPE, *PBUS_DATA_TYPE;

#pragma warning (push)
#pragma warning( disable : 4214 )
#pragma warning( disable : 4201 )
//
//Define the format of the PCI Slot parameter.
//
typedef struct _PCI_SLOT_NUMBER {
	union {
		struct {
			ULONG   DeviceNumber : 5;
			ULONG   FunctionNumber : 3;
			ULONG   Reserved : 24;
		} bits;
		ULONG   AsULONG;
	} u;
} PCI_SLOT_NUMBER, *PPCI_SLOT_NUMBER;
#pragma warning (pop)

#define PCI_TYPE0_ADDRESSES             6
#define PCI_TYPE1_ADDRESSES             2


//
// Define the standard PCI configuration information.
//
typedef struct _PCI_COMMON_CONFIG {
	USHORT  VendorID;                   // (ro)
	USHORT  DeviceID;                   // (ro)
	USHORT  Command;                    // Device control
	USHORT  Status;
	UCHAR   RevisionID;                 // (ro)
	UCHAR   ProgIf;                     // (ro)
	UCHAR   SubClass;                   // (ro)
	UCHAR   BaseClass;                  // (ro)
	UCHAR   CacheLineSize;              // (ro+)
	UCHAR   LatencyTimer;               // (ro+)
	UCHAR   HeaderType;                 // (ro)
	UCHAR   BIST;                       // Built in self test

	union {
		struct _PCI_HEADER_TYPE_0 {
			ULONG   BaseAddresses[PCI_TYPE0_ADDRESSES];
			ULONG   CIS;
			USHORT  SubVendorID;
			USHORT  SubSystemID;
			ULONG   ROMBaseAddress;
			ULONG   Reserved2[2];

			UCHAR   InterruptLine;      //
			UCHAR   InterruptPin;       // (ro)
			UCHAR   MinimumGrant;       // (ro)
			UCHAR   MaximumLatency;     // (ro)
		} type0;


	} u;

	UCHAR   DeviceSpecific[192];

} PCI_COMMON_CONFIG, *PPCI_COMMON_CONFIG;


#define PCI_COMMON_HDR_LENGTH (FIELD_OFFSET (PCI_COMMON_CONFIG, DeviceSpecific))

#define PCI_MAX_DEVICES                     32
#define PCI_MAX_FUNCTION                    8

#define PCI_INVALID_VENDORID                0xFFFF


//
// Bit encodings for  PCI_COMMON_CONFIG.HeaderType
//
#define PCI_MULTIFUNCTION                   0x80
#define PCI_DEVICE_TYPE                     0x00
#define PCI_BRIDGE_TYPE                     0x01


//
// Bit encodings for PCI_COMMON_CONFIG.Command
//
#define PCI_ENABLE_IO_SPACE                 0x0001
#define PCI_ENABLE_MEMORY_SPACE             0x0002
#define PCI_ENABLE_BUS_MASTER               0x0004
#define PCI_ENABLE_SPECIAL_CYCLES           0x0008
#define PCI_ENABLE_WRITE_AND_INVALIDATE     0x0010
#define PCI_ENABLE_VGA_COMPATIBLE_PALETTE   0x0020
#define PCI_ENABLE_PARITY                   0x0040  // (ro+)
#define PCI_ENABLE_WAIT_CYCLE               0x0080  // (ro+)
#define PCI_ENABLE_SERR                     0x0100  // (ro+)
#define PCI_ENABLE_FAST_BACK_TO_BACK        0x0200  // (ro)


//
// Bit encodings for PCI_COMMON_CONFIG.Status
//
#define PCI_STATUS_FAST_BACK_TO_BACK        0x0080  // (ro)
#define PCI_STATUS_DATA_PARITY_DETECTED     0x0100
#define PCI_STATUS_DEVSEL                   0x0600  // 2 bits wide
#define PCI_STATUS_SIGNALED_TARGET_ABORT    0x0800
#define PCI_STATUS_RECEIVED_TARGET_ABORT    0x1000
#define PCI_STATUS_RECEIVED_MASTER_ABORT    0x2000
#define PCI_STATUS_SIGNALED_SYSTEM_ERROR    0x4000
#define PCI_STATUS_DETECTED_PARITY_ERROR    0x8000


//
// Bit encodes for PCI_COMMON_CONFIG.u.type0.BaseAddresses
//
#define PCI_ADDRESS_IO_SPACE                0x00000001  // (ro)
#define PCI_ADDRESS_MEMORY_TYPE_MASK        0x00000006  // (ro)
#define PCI_ADDRESS_MEMORY_PREFETCHABLE     0x00000008  // (ro)

#define PCI_TYPE_32BIT      0
#define PCI_TYPE_20BIT      2
#define PCI_TYPE_64BIT      4


//
// Bit encodes for PCI_COMMON_CONFIG.u.type0.ROMBaseAddresses
//
#define PCI_ROMADDRESS_ENABLED              0x00000001


//
// Reference notes for PCI configuration fields:
//
// ro   these field are read only.  changes to these fields are ignored
//
// ro+  these field are intended to be read only and should be initialized
//      by the system to their proper values.  However, driver may change
//      these settings.
//
#endif // _NTDDK_

#else
#include <rtapi.h>
#endif
#endif

#ifdef UNDER_RTSS
#include <rtssapi.h>    // RTX64 APIs that can only be used in real-time applications.
#endif // UNDER_RTSS
#ifdef UNDER_WRTSS
#define RtSleepFt SleepFt
#endif
#if defined UNDER_RTSS
  #if defined RTX64_EXPORTS
    #define WinRTXConversion_API __declspec(dllexport)
  #else
    #define WinRTXConversion_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define WinRTXConversion_API __declspec(dllexport)
  #else
    #define WinRTXConversion_API __declspec(dllimport)
  #endif
#endif


// Add DEFINES Here
// This class is exported from the WinRTXConversion.dll
class WinRTXConversion_API CWinRTXConversion
{
public:
    CWinRTXConversion(void);
    // TODO: add your methods here.
};
extern WinRTXConversion_API int nWinRTXConversion;

// function prototype for the RTDLL exported function
WinRTXConversion_API BOOL RTAPI GetClockTime(CLOCK Clock, PLARGE_INTEGER pTime);
WinRTXConversion_API VOID RTAPI SleepFt(PLARGE_INTEGER pSleepTime);
WinRTXConversion_API HANDLE	RTAPI WdCreateSharedMemory(DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCSTR lpName, VOID** location);
#endif