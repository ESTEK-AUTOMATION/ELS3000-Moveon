///////////////////////////////////////////////////////////////////////////////
//
//
//			Copyright (c) 2011 - 2018 IntervalZero, Inc.  All rights reserved.
//
//
//File: RtssApi.h
//
//Abstract:
//
//		This file defines the functions which only RTSS applications can use.
//	RTSS applications can use a superset of the RtApi functionality.  The total
//  RTSS API is declared in RtApi.h and RtssApi.h.
//
///////////////////////////////////////////////////////////////////////////////

#pragma once

#ifndef _RTSSAPI_H_
#define _RTSSAPI_H_

#ifndef RTAPI
# define RTAPI
#endif

#ifndef RTFCNDCL
# define RTFCNDCL
#endif

#ifdef __cplusplus
extern "C" {
#endif

#include "RtLicensing.h"

///////////////////////////////////////////////////////////////////////////////
//
//Interrupt functions.
//
///////////////////////////////////////////////////////////////////////////////

typedef enum _MSI_CAPABILITY {
    NotMsiMsixCapable,
    MsiCapable,
    MsixCapable,
    MsiMsixCapable
} MSI_CAPABILITY, *PMSI_CAPABILITY;

//
// This function queries a specified PCI device's MSI/MSIX capability
//
MSI_CAPABILITY RTAPI RtQueryPciMsiCapability(ULONG BusNumber, PCI_SLOT_NUMBER SlotNumber);

//
// This function gets a specified PCI device's MSIX free messages
//
ULONG RTAPI RtGetPciMsixFreeMessages(
    ULONG BusNumber, 
    PCI_SLOT_NUMBER SlotNumber,
    ULONG *TotalMessagesInCapability,
    BYTE *FreeMessageIdBitMap);

//
// This function queries the number of RTSS processor vectors (IDT vectors) currently available (free count)
//
ULONG RTAPI RtQueryProcessorVectorFreeCount(ULONG ProcessorVectorFreeCount[]);

///////////////////////////////////////////////////////////////////////////////
//
//Local memory functions.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//Local memory functions.
//
///////////////////////////////////////////////////////////////////////////////

// This function provides the ability to determine whether Subsystem and RTSS processes default to use Local Memory
BOOL    RTAPI RtIsDefaultLocalMemory();

// mspace is an opaque type representing an independent
// region of space that supports external or internal allocation of per RTSS process.
typedef void* mspace;

// Supported allocation option for Flags parameter in RtAllocateLocalMemoryEx
#define RTALLOC_NOT_ZERO_MEMORY             0x00000001  

// This function allocates memory from the Local Memory Pool in the external allocation space of current process
// Note that the memory is zeroed only if control register, ZeroMemoryAtAllocation, is set,
// and the Flags parameter without RTALLOC_NOT_ZERO_MEMORY option.
PVOID	RTAPI RtAllocateLocalMemoryEx(ULONG Size, ULONG Flags);

///////////////////////////////////////////////////////////////////////////////
//
//Physical memory mapping functions.
//
///////////////////////////////////////////////////////////////////////////////

PVOID	RTAPI RtAllocateLockedMemory(ULONG Size);
BOOL	RTAPI RtFreeLockedMemory(PVOID pVirtualAddress);


///////////////////////////////////////////////////////////////////////////////
//
//Thread functions.
//
///////////////////////////////////////////////////////////////////////////////
// This function retrieves timing information for the specified RTSS thread. This function 
// can only be called in RTSS processes.
BOOL	RTAPI RtGetThreadTimes(HANDLE hThread,	LPFILETIME lpCreationTime, LPFILETIME lpExitTime, LPFILETIME lpExecuteTime);

// Return value if function RtGetThreadTimeQuantumEx fails.
#define INVALID_QUANTUM 0x0

// This function gets the current time quantum, in milliseconds, for the specified thread.
DWORD	RTAPI RtGetThreadTimeQuantum(HANDLE hThread);

// This function sets the time quantum for the specified thread.
BOOL	RTAPI RtSetThreadTimeQuantum(HANDLE hThread, DWORD dwQuantumInMS);

// This function gets the current time quantum, in microseconds, for the specified realtime thread. This function
// can only be called in RTSS processes.
ULONG64	RTAPI RtGetThreadTimeQuantumEx(HANDLE hThread);

// This function sets the time quantum, in microseconds, for the specified realtime thread. This function
// can only be called in RTSS processes.
BOOL	RTAPI RtSetThreadTimeQuantumEx(HANDLE hThread, ULONG64 ulQuantumInUS);

//This function opens an existing thread object. This function can only be called in RTSS processes.
HANDLE RTAPI RtOpenThread(DWORD	dwDesiredAccess, BOOL bInheritHandle,	DWORD dwThreadId);

//This function retrieves the thread stack parameters.
BOOL RTAPI RtGetThreadStack(HANDLE hThread, PDWORD StackSize, PDWORD StackAvailable, PULONG_PTR StackBaseAddress, PULONG_PTR StackLimitAddress, PULONG_PTR CurrentStackTopAddress);

// RDT resource type bit offset for the returnning value of pRDTResourceIdBitMap parameter in RtGetRDTCapability
#define RDT_RESOURCE_BITOFFSET_L3_CAT     1
#define RDT_RESOURCE_BITOFFSET_L2_CAT     2
#define RDT_RESOURCE_BITOFFSET_MBA        3

// Get system Resource Director Technology (RDT) allocation capability
BOOL RTAPI RtGetRDTCapability(ULONG *pRDTResourceBitMap, ULONG *pNumberOfCLOS);

// Macro for nCLOS parameter in RtSetThreadCLOS to reset back to thread's implicit/default CLOS
#define DEFAULT_CLOS                  0xFFFF

// Overwrite thread’s implicit/default CLOS, which is based on its priority
BOOL RTAPI RtSetThreadCLOS(HANDLE hThread, ULONG nCLOS);

// Get thread’s explicit CLOS (if set by user) or implicit/default CLOS.
BOOL RTAPI RtGetThreadCLOS(HANDLE hThread, ULONG *pCLOS);

///////////////////////////////////////////////////////////////////////////////
//
//Time functions.
//
///////////////////////////////////////////////////////////////////////////////

// This function gets the current value for RTSS time.
VOID	RTAPI RtGetRtssTimeAsFileTime(LPFILETIME pTime);
// This function sets the new value for RTSS time.
VOID	RTAPI RtSetRtssTimeAsFileTime(LPFILETIME pTime);

///////////////////////////////////////////////////////////////////////////////
//
//Waitable Timer functions.
//
///////////////////////////////////////////////////////////////////////////////

#if !defined(_WINDEF_)
typedef VOID (RTFCNDCL *PTIMERAPCROUTINE)(LPVOID lpArgToCompletionRoutine, DWORD dwTimerLowValue, DWORD dwTimerHighValue);
#endif


HANDLE	RTAPI RtCreateWaitableTimerA(LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCSTR lpTimerName);
HANDLE	RTAPI RtCreateWaitableTimerW(LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCWSTR lpTimerName);


#ifdef UNICODE
#define RtCreateWaitableTimer  RtCreateWaitableTimerW
#else
#define RtCreateWaitableTimer  RtCreateWaitableTimerA
#endif // !UNICODE


HANDLE	RTAPI RtOpenWaitableTimerA(DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpTimerName);
HANDLE	RTAPI RtOpenWaitableTimerW(DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpTimerName);


#ifdef UNICODE
#define RtOpenWaitableTimer  RtOpenWaitableTimerW
#else
#define RtOpenWaitableTimer  RtOpenWaitableTimerA
#endif // !UNICODE


BOOL	RTAPI RtSetWaitableTimer(HANDLE hWaitableTimer, const LARGE_INTEGER *lpDueTime, LONG lPeriod, PTIMERAPCROUTINE pfnCompletionRoutine, LPVOID lpArgToCompletionRoutine, BOOL fResume);
BOOL	RTAPI RtCancelWaitableTimer(HANDLE hWaitableTimer);
BOOL	RTAPI RtSetWaitableTimerFt(HANDLE hWaitableTimer, const LARGE_INTEGER *lpDueTime, const LARGE_INTEGER *lpPeriod, PTIMERAPCROUTINE pfnCompletionRoutine, LPVOID lpArgToCompletionRoutine, BOOL fResume);

///////////////////////////////////////////////////////////////////////////////
//
//Spin Lock and Fast Semaphore functions.
//
///////////////////////////////////////////////////////////////////////////////

typedef struct _RTSPIN_LOCK
{
	volatile ULONG	Lock;
	volatile ULONG	EFlags;
	PVOID			pOwner;

}RTSPIN_LOCK, *PRTSPIN_LOCK;

BOOLEAN RTAPI RtInitializeSpinlock(PRTSPIN_LOCK pSpinlock);

VOID RTAPI RtAcquireSpinlock(PRTSPIN_LOCK pSpinlock);

VOID RTAPI RtReleaseSpinlock(PRTSPIN_LOCK pSpinlock);

typedef struct _RTFAST_SEMAPHORE
{
	// Thread synchronization spin lock for accessing the structure memebers
	RTSPIN_LOCK		ListLock;
	// List of current waiters for a fast semaphore  
	LIST_ENTRY		List;
	// Count of waiters for a fast semaphore
	volatile ULONG	Count;
	// The number of available resources for a fast semaphore
	volatile ULONG	CountToWake;
	// The number of concurrent fast semaphore waiters that cause a simultaneous release.
	ULONG			CountAutoWake;
	// Holds a unique ID used by the Monitoring Framework.
	ULONGLONG       UniqueID;       
} RTFAST_SEMAPHORE, *PRTFAST_SEMAPHORE;

// This function initializes a fast semaphore object.
BOOL RTAPI RtInitializeFastSemaphore(PRTFAST_SEMAPHORE pSemaphore, ULONG releaseCount);
// This function returns the number of threads waiting on this fast semaphore. This information is useful 
// when preparing for a manual release of the fast semaphore. 
BOOL RTAPI RtGetCountFastSemaphore(const PRTFAST_SEMAPHORE pSemaphore, PULONG pCount);
// This function places the thread into a busy wait state until the fast semaphore is released.
BOOL RTAPI RtAcquireFastSemaphore(PRTFAST_SEMAPHORE pSemaphore);
// This function releases all threads waiting on the fast semaphore. Ensures that all waiting threads enter 
// the ready-to-run state. Ensures that no new threads enter the busy wait state until all current waiters 
// have become runnable.
BOOL RTAPI RtReleaseAllFastSemaphore(PRTFAST_SEMAPHORE pSemaphore);
// This function releases the specified count of threads waiting on the fast semaphore. If the number of 
// waiting threads is less than the specified count, RtReleaseFastSemaphore will store the remaining count 
// value and continue to release new threads that call RtAcquireFastSemaphore on this fast semaphore.
BOOL RTAPI RtReleaseFastSemaphore(PRTFAST_SEMAPHORE pSemaphore, ULONG count);

///////////////////////////////////////////////////////////////////////////////
//
//Processor number
//
///////////////////////////////////////////////////////////////////////////////

// This function returns the system-assigned number of the current processor on which the caller is running.
ULONG RTAPI RtGetCurrentProcessorNumber(VOID);


///////////////////////////////////////////////////////////////////////////////
//
//Rt Trace functions.
//
///////////////////////////////////////////////////////////////////////////////

#define PERF_MEASURE_HALISR_ENTEREXIT     0x00000001
#define PERF_MEASURE_HAL_CLOCKTICK        0x00000002
#define PERF_MEASURE_HAL_INTERNAL         0x00000004
#define PERF_MEASURE_RTSS_TIMERTICK       0x00000008
#define PLATFORM_CONFIG_UP_PIC            0x00000001
#define PLATFORM_CONFIG_UP_APIC           0x00000002
#define PLATFORM_CONFIG_MP_DEDICATED      0x00000004
#define PLATFORM_CONFIG_MP_SHARED         0x00000008


ULONG	RTAPI RtStartPerfMeasure(ULONG perfMeasureType,				//control code of operation to perform (input)
								 ULONG_PTR *ioBuffer,				//buffer to pass data (output)
								 ULONG ioBufferSize,				//buffer size (input)
								 ULONG_PTR *ioBufferWrite,			//pointer to write index (output)
								 LONGLONG *lPerfCyclesPerSecond,	//pointer to pass PerfCyclesPerSecond (output)
								 ULONG *platformConfig,				//pointer to pass platform configuration (output)
								 ULONG *rtssProcessorNumber			//pointer to pass RTSS processor number (output)
								);


ULONG	RTAPI RtStopPerfMeasure(ULONG perfMeasureType,	//control code of operation to perform (input)
								ULONG *ioBuffer			//buffer to pass data (output)
							   );


///////////////////////////////////////////////////////////////////////////////
//
//WFSO/WFMO Ex functions.
//
///////////////////////////////////////////////////////////////////////////////

// This function allows a thread to wait on an object to be signaled with high granularity of time-out interval.
DWORD RTAPI RtWaitForSingleObjectEx(
    			HANDLE hHandle,
	            PULARGE_INTEGER  lpWaitTimedOut);

// This function allows a thread to wait on one of multiple objects to be signaled with high granularity of 
// time-out interval.
DWORD RTAPI RtWaitForMultipleObjectsEx(
	DWORD			 dwCount,
    CONST HANDLE	 *lpHandles,
	BOOL			 bWaitAll,
    PULARGE_INTEGER  lpWaitTimedOut);


///////////////////////////////////////////////////////////////////////////////
//
//Get process freeze status.
//
///////////////////////////////////////////////////////////////////////////////

BOOL
RTAPI
RtGetProcessFreeze(
	DWORD		dwProcessId,
    BOOL        *bFreeze
	);


///////////////////////////////////////////////////////////////////////////////
//
//Get enabled XState features on x64 processors
//
///////////////////////////////////////////////////////////////////////////////

//
// Enum type for RtGetEnabledXStateFeature parameter - XSateBit
//
#if ( !defined(_XSTATE_BIT_) )
#define _XSTATE_BIT_
typedef enum _XSTATE_BIT {
	XStateUndefined = -1,    // -1
    XStateFPU,               // 0 - x87 FPU
    XStateSSE,               // 1 - SSE
    XStateAVX,               // 2 - AVX/AVX2/AVX512
	XStateMPX,               // 3 - MPX
    XStatePT,                // 4 - PT
    XStateMaximum            // Invalid
} XSTATE_BIT, *PXSTATE_BIT;
#endif

#if ( !defined(_ENABLED_HIGHEST_FEATURE_) )
#define _ENABLED_HIGHEST_FEATURE_
typedef enum _ENABLED_HIGHEST_FEATURE {
    NotSupported = -1,
    x87FPU,
	SSE,
	SSE2,
	SSE3,
    SSSE3,
	SSE4_1,
	SSE4_2,
	AVX,
	AVX2,
	AVX512,
	MPX,
	PT
} ENABLED_HIGHEST_FEATURE, *PENABLED_HIGHEST_FEATURE;
#endif

BOOL 
RTAPI 
// This function returns the highest feature for a specified XState feature set supported on the running 
// processor and enabled by RTX64.
RtGetEnabledXStateFeature(
	XSTATE_BIT XSateBit, 
	PENABLED_HIGHEST_FEATURE HighestFeature
	);

LONG
RTAPI
__RtGetAllObjects(
	PUCHAR	Buffer,
	LONG	Length
	);

LONG
RTAPI
__RtQueryAllMSpaces(
    PUCHAR	Buffer,
    LONG	Length
);

BOOL
RTAPI
__RtGetAllMSpaceFragmentStart(
    PUCHAR  *TopBuffer,
    ULONG  *Length
);

BOOL
RTAPI
__RtGetAllMSpaceFragmentEnd(
    PUCHAR  TopBuffer,
    ULONG  Length
);

BOOL RTAPI RtGetFlushTLBTickMod(ULONG  *pTickMod);
BOOL RTAPI RtSetFlushTLBTickMod(ULONG  TickMod);


///////////////////////////////////////////////////////////////////////////////
//
// Process Local Storage (PLS) functions.
//
///////////////////////////////////////////////////////////////////////////////

//
// Maximum number of PLS values per process. Must be in 32 bit multiples.
//
#define	PLS_NINDEX		160

DWORD
RTAPI
RtPlsAlloc(
    VOID
    );

BOOL
RTAPI
RtPlsFree(
    DWORD  dwPlsIndex
    );

LPVOID
RTAPI
RtPlsGetValue(
    DWORD  dwPlsIndex
    );

BOOL
RTAPI
RtPlsSetValue(
    DWORD   dwPlsIndex,
    LPVOID  lpPlsValue
    );


/////////////////////////////////////////////////////////////////
//
// Low-level RTSS lock (e.g. mutex), not through object layer
//
/////////////////////////////////////////////////////////////////

//
// Low-level synchronization events.
//
typedef struct _RTLOCKEVENT
{
    LONG		State;			// non-0 if signaled
    LIST_ENTRY	WaitList;		// FIFO list of waiting threads
} RTLOCKEVENT, *PRTLOCKEVENT;

//
// Low-level locks (e.g. mutexes).
//
typedef struct _RTLOCK {
    RTLOCKEVENT	Event;			// Event to wait for if lock is locked
    PVOID	    OwnerThread;	// Thread holding the lock or NULL
    LONG		Count;			// Nested acquisition count
} RTLOCK, *PRTLOCK;

VOID
RTAPI
RtInitLock(
    PRTLOCK	Lock
    );

BOOL
RTAPI
RtLockLock(
    PRTLOCK	Lock
    );

VOID
RTAPI
RtUnlockLock(
    PRTLOCK	Lock
    );

/////////////////////////////////////////////////////////////////
//
// Watchdog functionality
//
/////////////////////////////////////////////////////////////////

BOOL
RTAPI
RtResetWatchdog(KAFFINITY affinity);

/////////////////////////////////////////////////////////////////
//
// Sleep functionality.
//
/////////////////////////////////////////////////////////////////

// This function suspends the current thread for the specified time with the granularity of the RTSS timer.
VOID RTAPI RtSleepFt(PLARGE_INTEGER pSleepTime);

// This function suspends execution of the current thread for a specified time period.
ULONG RTAPI RtSleepFtEx(PLARGE_INTEGER pSleepTime, BOOL bAlertable);

//
// Virtual network functions
//
BOOL RtNotifyVNetDriver(ULONG type, PVOID data);


///////////////////////////////////////////////////////////////////////////////
//
//RTSS component functions.
//
///////////////////////////////////////////////////////////////////////////////

//Types of RTSS components
typedef enum _RTSSCOMPONENT {
	RtTCPIP,
	RtNal
} RTSSCOMPONENT, *PRTSSCOMPONENT;

//
// This enumeration represents the status of a RTX64 runtime component.
//
typedef enum _RT_COMPONENT_STATUS
{
	RT_COMPONENT_STATUS_UNKNOWN = 0,
	RT_COMPONENT_STATUS_STOPPED,         // The component is stopped.
	RT_COMPONENT_STATUS_STARTED,         // The component is started.
	RT_COMPONENT_STATUS_STOPPING,        // The component is stopping.
	RT_COMPONENT_STATUS_STARTING,        // The component is starting.
	RT_COMPONENT_STATUS_TRANSITIONING,   // The component is either stopping or starting.
	RT_COMPONENT_STATUS_NOT_CONFIGURED,  // The component is not configured or not in use.
	RT_COMPONENT_STATUS_ERROR,           // The component is in an error state.
} RT_COMPONENT_STATUS, *PRT_COMPONENT_STATUS;

//
// This function queries the current status of specified RTSS component
//
BOOL RTAPI RtQueryComponent(RTSSCOMPONENT Component, PRT_COMPONENT_STATUS Status);

//
// This function starts specified RTSS component
//
BOOL RTAPI RtStartComponent(RTSSCOMPONENT Component);

#ifdef __cplusplus
}
#endif

#endif
