#include "CCustomerMain.h"

CCustomerMain::CCustomerMain()
{
}

CCustomerMain::~CCustomerMain()
{
}

int CCustomerMain::Initialize()
{
	SetProductClasses(m_cLogger, m_cMotion, m_cIO);
	m_cCustomerThread->SetProductThread(m_cCustomerThread);
	m_cCustomerSequence->SetProductSequence(m_cCustomerSequence);
	m_cCustomerStateControl->SetProductStateControl(m_cCustomerStateControl);
	m_cCustomerMotorControl->SetProductMotorControl(m_cCustomerMotorControl);
	//m_cCustomerMotorControl->SetClass(m_cMotion);	
	m_cCustomerIOControl->SetProductIOControl(m_cCustomerIOControl);
	m_cCustomerShareVariables->SetProductShareVariables(m_cCustomerShareVariables);
	
	CProductMain::Initialize();
	
	return 0;
}

int CCustomerMain::CreateShareMemory()
{
	m_cLogger->WriteLog("RtCreate share memory from customer\n");
	HANDLE hsm = NULL;
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemorySetting), "SharedMemorySetting", (LPVOID*)&smCustomerSetting)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemorySetting), "SharedMemorySetting", (LPVOID*)&smCustomerSetting)) == NULL)
#endif
	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 1);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryTeachPoint), "SharedMemoryTeachPoint", (LPVOID*)&smCustomerTeachPoint)) == NULL)
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryTeachPoint), "SharedMemoryTeachPoint", (LPVOID*)&smCustomerTeachPoint)) == NULL)
#endif
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 2);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryProduction), "SharedMemoryProduction", (LPVOID*)&smCustomerProduction)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryProduction), "SharedMemoryProduction", (LPVOID*)&smCustomerProduction)) == NULL)
#endif	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 3);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryCustomize), "SharedMemoryCustomize", (LPVOID*)&smCustomerCustomize)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryCustomize), "SharedMemoryCustomize", (LPVOID*)&smCustomerCustomize)) == NULL)
#endif	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 4);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryIO), "SharedMemoryIO", (LPVOID*)&smCustomerIO)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryIO), "SharedMemoryIO", (LPVOID*)&smCustomerIO)) == NULL)
#endif	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 5);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryModuleStatus), "SharedMemoryModuleStatus", (LPVOID*)&smCustomerModuleStatus)) == NULL)
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryModuleStatus), "SharedMemoryModuleStatus", (LPVOID*)&smCustomerModuleStatus)) == NULL)
#endif
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 6);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryEvent), "SharedMemoryEvent", (LPVOID*)&smCustomerEvent)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryEvent), "SharedMemoryEvent", (LPVOID*)&smCustomerEvent)) == NULL)
#endif	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 7);
		ExitProcess(2);
	}
#if UNDER_WIN
	if ((hsm = RtCreateSharedMemory(PAGE_READWRITE, FALSE, sizeof(CustomerSharedMemoryGeneral), "SharedMemoryGeneral", (LPVOID*)&smCustomerGeneral)) == NULL)	
#else
	if ((hsm = RtCreateSharedMemory(SHM_MAP_ALL_ACCESS, (DWORD)0, (DWORD)sizeof(CustomerSharedMemoryGeneral), "SharedMemoryGeneral", (LPVOID*)&smCustomerGeneral)) == NULL)
#endif	
	{
		m_cLogger->WriteLog("RtCreate share memory error = %d\n", 8);
		ExitProcess(2);
	}
	smProductSetting = smCustomerSetting;
	smProductTeachPoint = smCustomerTeachPoint;
	smProductProduction = smCustomerProduction;
	smProductCustomize= smCustomerCustomize;
	smProductIO = smCustomerIO;
	smProductModuleStatus = smCustomerModuleStatus;
	smProductEvent = smCustomerEvent;
	smProductGeneral = smCustomerGeneral;

	smSetting = smCustomerSetting;
	smTeachPoint = smCustomerTeachPoint;
	smProduction = smCustomerProduction;
	smCustomize = smCustomerCustomize;
	smIO = smCustomerIO;
	smModuleStatus = smCustomerModuleStatus;
	smEvent = smCustomerEvent;
	smGeneral = smCustomerGeneral;

	SetProductShareMemory(smCustomerSetting, smCustomerTeachPoint, smCustomerProduction, smCustomerCustomize, smCustomerIO
		, smCustomerModuleStatus, smCustomerEvent, smCustomerGeneral);
	return 0;
}

int CCustomerMain::LoadSetting()
{
	CProductMain::LoadSetting();

	return 0;
}

int CCustomerMain::CreateAllThread()
{
	DWORD   dwSuspendCount;							// suspend count
	// for event server thread code
	HANDLE  hSequenceThread_high;					// handle for child thread
	HANDLE  hIOScanThread_high;						// handle for child thread
	HANDLE  hIOOperationThread_min;					// handle for child thread
	HANDLE  hTeachPointThread_min;					// handle for child thread
	HANDLE  hManualThread_min;						// handle for child thread	
	HANDLE  hMaintenanceThread_min;					// handle for child thread
	HANDLE  hAutoOperationThread_Low;				// handle for child thread
	HANDLE  hStateThread_min;						// handle for child thread
	HANDLE  hDummyThread_Low;						// handle for child thread


	HANDLE  hMC1Thread_high;						// handle for child thread
	HANDLE  hMC2Thread_high;						// handle for child thread
	HANDLE  hMC3Thread_high;						// handle for child thread
	HANDLE  hMC4Thread_high;						// handle for child thread
	HANDLE  hMC5Thread_high;						// handle for child thread
	HANDLE  hMC6Thread_high;						// handle for child thread
	HANDLE  hMC7Thread_high;						// handle for child thread

	HANDLE hPickAndPlace1YAxisMotorThread_high;	// handle for child thread
	HANDLE hInputTrayTableXAxisMotorThread_high;	// handle for child thread
	HANDLE hInputTrayTableYAxisMotorThread_high;	// handle for child thread
	HANDLE hInputTrayTableZAxisMotorThread_high;	// handle for child thread
	HANDLE hPickAndPlace2YAxisMotorThread_high;	// handle for child thread
	HANDLE hOutputTrayTableXAxisMotorThread_high;	// handle for child thread
	HANDLE hOutputTrayTableYAxisMotorThread_high;	// handle for child thread
	HANDLE hOutputTrayTableZAxisMotorThread_high;	// handle for child thread
	HANDLE hInputVisionModuleMotorThread_high;	// handle for child thread
	HANDLE hS2VisionModuleMotorThread_high;	// handle for child thread
	HANDLE hS1VisionModuleMotorThread_high;	// handle for child thread
	HANDLE hS3VisionModuleMotorThread_high;	// handle for child thread

	HANDLE hPickAndPlace1XAxisMotorThread_high;	// handle for child thread

	HANDLE hPickAndPlace2XAxisMotorThread_high;	// handle for child thread
	HANDLE hPickAndPlace1ZAxisMotorThread_high;	// handle for child thread
	HANDLE hPickAndPlace2ZAxisMotorThread_high;	// handle for child thread
	HANDLE hPickAndPlace1ThetaAxisMotorThread_high;	// handle for child thread
	HANDLE hPickAndPlace2ThetaAxisMotorThread_high;	// handle for child thread

	HANDLE hInputVisionThread_high;	// handle for child thread
	HANDLE hS2VisionThread_high;	// handle for child thread
	HANDLE hOutputVisionThread_high;	// handle for child thread
	HANDLE hBottomVisionThread_high;	// handle for child thread
	HANDLE hS1VisionThread_high;	// handle for child thread
	HANDLE hS3VisionThread_high;	// handle for child thread

	HANDLE hPickAndPlaceSequenceThread_high;	// handle for child thread
	HANDLE hPickAndPlace2SequenceThread_high;
	HANDLE hInputTrayTableSequenceThread_high;	// handle for child thread
	HANDLE hOutputAndRejectTrayTableSequenceThread_high;	// handle for child thread
	
	HANDLE hCheckRejectReplaceThread_high;	// handle for child thread
	//HANDLE hCheckRejectRemoveThread_high;	// handle for child thread
	HANDLE hCheckRejectRemoveThread_high;	// handle for child thread

	//timeBeginPeriod(1);

	// Event code: create a child thread high
	hSequenceThread_high = RtCreateThread(0, 0, SequenceThread, NULL, CREATE_SUSPENDED, 0);
	if (hSequenceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread SequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hSequenceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority SequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	//GetExitCodeThread(hSequenceThread_high, 0);

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hSequenceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread SequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hIOScanThread_high = RtCreateThread(0, 0, IOScanThread, NULL, CREATE_SUSPENDED, 0);
	if (hIOScanThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread IOScanThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hIOScanThread_high,
		HIGHESTPRIORITY + 1) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority IOScanThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	//mif (SetThreadIdealProcessor(hIOScanThread_high, 2) == false)
		//if (RtSetProcessAffinityMask(hIOScanThread_high,
		//	mask) == FALSE)
	{
		m_cLogger->WriteLog("RtSetProcessAffinityMask hIOScanThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hIOScanThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hIOScanThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hIOOperationThread_min = RtCreateThread(0, 0, IOOperationThread, NULL, CREATE_SUSPENDED, 0);
	if (hIOOperationThread_min == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread IOOperationThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hIOOperationThread_min,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority IOOperationThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hIOOperationThread_min);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread IOOperationThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hTeachPointThread_min = RtCreateThread(0, 0, TeachPointThread, NULL, CREATE_SUSPENDED, 0);
	if (hTeachPointThread_min == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread TeachPointThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hTeachPointThread_min,
		RT_PRIORITY_MIN) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority TeachPointThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hTeachPointThread_min);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread TeachPointThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hManualThread_min = RtCreateThread(0, 0, ManualThread, NULL, CREATE_SUSPENDED, 0);
	if (hManualThread_min == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread ManualThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hManualThread_min,
		RT_PRIORITY_MIN) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority ManualThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hManualThread_min);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread ManualThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hMaintenanceThread_min = RtCreateThread(0, 0, MaintenanceThread, NULL, CREATE_SUSPENDED, 0);
	if (hMaintenanceThread_min == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread MaintenanceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hMaintenanceThread_min,
		RT_PRIORITY_MIN) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority MaintenanceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hMaintenanceThread_min);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread MaintenanceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hAutoOperationThread_Low = RtCreateThread(0, 0, AutoOperationThread, NULL, CREATE_SUSPENDED, 0);
	if (hAutoOperationThread_Low == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hAutoOperationThread_Low error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hAutoOperationThread_Low,
		//RT_PRIORITY_MIN) == FALSE)
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hAutoOperationThread_Low error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hAutoOperationThread_Low);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hAutoOperationThread_Low error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hStateThread_min = RtCreateThread(0, 0, StateThread, NULL, CREATE_SUSPENDED, 0);
	if (hStateThread_min == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread StateThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hStateThread_min,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority StateThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hStateThread_min);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread StateThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hDummyThread_Low = RtCreateThread(0, 0, DummyThread, NULL, CREATE_SUSPENDED, 0);
	if (hDummyThread_Low == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread DummyThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hDummyThread_Low,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority DummyThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hDummyThread_Low);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread DummyThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	#pragma endregion

	#pragma region Motion Controller
	if (smCustomerCustomize->EnableMotionController1)
	{
		// Event code: create a child thread high
		hMC1Thread_high = RtCreateThread(0, 0, MC1Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC1Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC1Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC1Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC1Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC1Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC1Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	}

	if (smCustomerCustomize->EnableMotionController2)
	{
		// Event code: create a child thread high
		hMC2Thread_high = RtCreateThread(0, 0, MC2Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC2Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC2Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC2Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC2Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC2Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC2Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	}

	if (smCustomerCustomize->EnableMotionController3)
	{
		// Event code: create a child thread high
		hMC3Thread_high = RtCreateThread(0, 0, MC3Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC3Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC3Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC3Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC3Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC3Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC3Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	}

	if (smCustomerCustomize->EnableMotionController4)
	{
		// Event code: create a child thread high
		hMC4Thread_high = RtCreateThread(0, 0, MC4Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC4Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC4Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC4Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC4Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC4Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC4Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	}
	//if (smCustomerCustomize->EnableMotionController5)
	//{
		// Event code: create a child thread high
		hMC5Thread_high = RtCreateThread(0, 0, MC5Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC5Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC5Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC5Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC5Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC5Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC5Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	//}
	//if (smCustomerCustomize->EnableMotionController6)
	//{
		// Event code: create a child thread high
		hMC6Thread_high = RtCreateThread(0, 0, MC6Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC6Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC6Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC6Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC6Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC6Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC6Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	//}
	//if (smCustomerCustomize->EnableMotionController7)
	//{
		// Event code: create a child thread high
		hMC7Thread_high = RtCreateThread(0, 0, MC7Thread, m_cMotion, CREATE_SUSPENDED, 0);
		if (hMC7Thread_high == NULL)
		{
			m_cLogger->WriteLog("RtCreateThread MC7Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code; set the priority of the child thread high
		if (RtSetThreadPriority(hMC7Thread_high,
			HIGHPRIORITY) == FALSE)
		{
			m_cLogger->WriteLog("RtSetThreadPriority MC7Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}

		// Event code: resume the child thread high
		dwSuspendCount = RtResumeThread(hMC7Thread_high);
		if (dwSuspendCount == 0xFFFFFFFF)
		{
			m_cLogger->WriteLog("RtResumeThread MC7Thread error = %d\n", GetLastError());
			// TO DO:  your exception code here
			// ExitProcess(1);
		}
	//}
	#pragma endregion

	#pragma region Fuji Control
	// Event code: create a child thread high
	hPickAndPlace1YAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace1YAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace1YAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hPickAndPlace1YAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace1YAxisMotorThread_high,
		HIGHPRIORITY + 1) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hPickAndPlace1YAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace1YAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hPickAndPlace1YAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hInputTrayTableXAxisMotorThread_high = RtCreateThread(0, 0, InputTrayTableXAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hInputTrayTableXAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hInputTrayTableXAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputTrayTableXAxisMotorThread_high,
		HIGHPRIORITY + 1) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hInputTrayTableXAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputTrayTableXAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hInputTrayTableXAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hInputTrayTableYAxisMotorThread_high = RtCreateThread(0, 0, InputTrayTableYAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hInputTrayTableYAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hInputTrayTableYAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputTrayTableYAxisMotorThread_high,
		HIGHPRIORITY + 1) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hInputTrayTableYAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputTrayTableYAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hInputTrayTableYAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hInputTrayTableZAxisMotorThread_high = RtCreateThread(0, 0, InputTrayTableZAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hInputTrayTableZAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hInputTrayTableZAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputTrayTableZAxisMotorThread_high,
		HIGHPRIORITY + 1) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hInputTrayTableZAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputTrayTableZAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hInputTrayTableZAxisMotorThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace2YAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace2YAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace2YAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace2YAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace2YAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace2YAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace2YAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace2YAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: create a child thread high
	hOutputTrayTableXAxisMotorThread_high = RtCreateThread(0, 0, OutputTrayTableXAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hOutputTrayTableXAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread OutputTrayTableXAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hOutputTrayTableXAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority OutputTrayTableXAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hOutputTrayTableXAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread OutputTrayTableXAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: create a child thread high
	hOutputTrayTableYAxisMotorThread_high = RtCreateThread(0, 0, OutputTrayTableYAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hOutputTrayTableYAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread OutputTrayTableYAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hOutputTrayTableYAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority OutputTrayTableYAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hOutputTrayTableYAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread OutputTrayTableYAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hOutputTrayTableZAxisMotorThread_high = RtCreateThread(0, 0, OutputTrayTableZAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hOutputTrayTableZAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread OutputTrayTableZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hOutputTrayTableZAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority OutputTrayTableZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hOutputTrayTableZAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread OutputTrayTableZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hInputVisionModuleMotorThread_high = RtCreateThread(0, 0, InputVisionModuleMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hInputVisionModuleMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread InputVisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputVisionModuleMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority InputVisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputVisionModuleMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread InputVisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hS2VisionModuleMotorThread_high = RtCreateThread(0, 0, S2VisionModuleMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hS2VisionModuleMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread S2VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS2VisionModuleMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority S2VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS2VisionModuleMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread S2VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hS1VisionModuleMotorThread_high = RtCreateThread(0, 0, S1VisionModuleMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hS1VisionModuleMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread S1VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS1VisionModuleMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority S1VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS1VisionModuleMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread S1VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hS3VisionModuleMotorThread_high = RtCreateThread(0, 0, S3VisionModuleMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hS3VisionModuleMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread S3VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS3VisionModuleMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority S3VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS3VisionModuleMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread S3VisionModuleMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace1XAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace1XAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace1XAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace1XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace1XAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace1XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace1XAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace1XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace2XAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace2XAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace2XAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace2XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace2XAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace2XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace2XAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace2XAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace1ZAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace1ZAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace1ZAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace1ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace1ZAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace1ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace1ZAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace1ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace2ZAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace2ZAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace2ZAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace2ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace2ZAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace2ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace2ZAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace2ZAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace1ThetaAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace1ThetaAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace1ThetaAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace1ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace1ThetaAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace1ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace1ThetaAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace1ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace2ThetaAxisMotorThread_high = RtCreateThread(0, 0, PickAndPlace2ThetaAxisMotorThread, m_cMotion, CREATE_SUSPENDED, 0);
	if (hPickAndPlace2ThetaAxisMotorThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlace2ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace2ThetaAxisMotorThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace2ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace2ThetaAxisMotorThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace2ThetaAxisMotorThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	#pragma endregion
	#pragma region Vision
	// Event code: create a child thread high
	hInputVisionThread_high = RtCreateThread(0, 0, InputVisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hInputVisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread InputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputVisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority InputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputVisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread InputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hS2VisionThread_high = RtCreateThread(0, 0, S2VisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hS2VisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread hS2VisionThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS2VisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority hS2VisionThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS2VisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread hS2VisionThread_high error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hOutputVisionThread_high = RtCreateThread(0, 0, OutputVisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hOutputVisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread OutputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hOutputVisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority OutputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hOutputVisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread OutputVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	hBottomVisionThread_high = RtCreateThread(0, 0, BottomVisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hBottomVisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread BottomVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hBottomVisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority BottomVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hBottomVisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread BottomVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hS1VisionThread_high = RtCreateThread(0, 0, S1VisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hS1VisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread SidewallLeftVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS1VisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority SidewallLeftVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS1VisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread SidewallLeftVisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	hS3VisionThread_high = RtCreateThread(0, 0, S3VisionThread, NULL, CREATE_SUSPENDED, 0);
	if (hS3VisionThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread S3VisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hS3VisionThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority S3VisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hS3VisionThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread S3VisionThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	#pragma endregion
	#pragma region Sequence
	// Event code: create a child thread high
	hPickAndPlaceSequenceThread_high = RtCreateThread(0, 0, PickAndPlaceSequenceThread, NULL, CREATE_SUSPENDED, 0);
	if (hPickAndPlaceSequenceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlaceSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlaceSequenceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlaceSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlaceSequenceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlaceSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hPickAndPlace2SequenceThread_high = RtCreateThread(0, 0, PickAndPlace2SequenceThread, NULL, CREATE_SUSPENDED, 0);
	if (hPickAndPlace2SequenceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread PickAndPlaceSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hPickAndPlace2SequenceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority PickAndPlace2SequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hPickAndPlace2SequenceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread PickAndPlace2SequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: create a child thread high
	hInputTrayTableSequenceThread_high = RtCreateThread(0, 0, InputTrayTableSequenceThread, NULL, CREATE_SUSPENDED, 0);
	if (hInputTrayTableSequenceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread InputTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hInputTrayTableSequenceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority InputTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hInputTrayTableSequenceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread InputTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	// Event code: create a child thread high
	hOutputAndRejectTrayTableSequenceThread_high = RtCreateThread(0, 0, OutputAndRejectTrayTableSequenceThread, NULL, CREATE_SUSPENDED, 0);
	if (hOutputAndRejectTrayTableSequenceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread OutputAndRejectTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hOutputAndRejectTrayTableSequenceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority OutputAndRejectTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hOutputAndRejectTrayTableSequenceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread OutputAndRejectTrayTableSequenceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	#pragma endregion
	hCheckRejectReplaceThread_high = RtCreateThread(0, 0, CheckRejectReplaceThread, NULL, CREATE_SUSPENDED, 0);
	if (hCheckRejectReplaceThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread CheckRejectReplaceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hCheckRejectReplaceThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority CheckRejectReplaceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hCheckRejectReplaceThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread CheckRejectReplaceThread error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	//hCheckRejectRemoveThread_high = RtCreateThread(0, 0, CheckRejectRemoveThread, NULL, CREATE_SUSPENDED, 0);
	//if (hCheckRejectRemoveThread_high == NULL)
	//{
	//	m_cLogger->WriteLog("RtCreateThread CheckRejectRemoveThread error = %d\n", GetLastError());
	//	// TO DO:  your exception code here
	//	// ExitProcess(1);
	//}
	//// Event code; set the priority of the child thread high
	//if (RtSetThreadPriority(hCheckRejectRemoveThread_high,
	//	HIGHPRIORITY) == FALSE)
	//{
	//	m_cLogger->WriteLog("RtSetThreadPriority CheckRejectRemoveThread error = %d\n", GetLastError());
	//	// TO DO:  your exception code here
	//	// ExitProcess(1);
	//}
	//// Event code: resume the child thread high
	//dwSuspendCount = RtResumeThread(hCheckRejectRemoveThread_high);
	//if (dwSuspendCount == 0xFFFFFFFF)
	//{
	//	m_cLogger->WriteLog("RtResumeThread CheckRejectRemoveThread error = %d\n", GetLastError());
	//	// TO DO:  your exception code here
	//	// ExitProcess(1);
	//}
	hCheckRejectRemoveThread_high = RtCreateThread(0, 0, CheckRejectRemoveThread, NULL, CREATE_SUSPENDED, 0);
	if (hCheckRejectRemoveThread_high == NULL)
	{
		m_cLogger->WriteLog("RtCreateThread CheckRejectRemoveThread1 error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code; set the priority of the child thread high
	if (RtSetThreadPriority(hCheckRejectRemoveThread_high,
		HIGHPRIORITY) == FALSE)
	{
		m_cLogger->WriteLog("RtSetThreadPriority CheckRejectRemoveThread1 error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}
	// Event code: resume the child thread high
	dwSuspendCount = RtResumeThread(hCheckRejectRemoveThread_high);
	if (dwSuspendCount == 0xFFFFFFFF)
	{
		m_cLogger->WriteLog("RtResumeThread CheckRejectRemoveThread1 error = %d\n", GetLastError());
		// TO DO:  your exception code here
		// ExitProcess(1);
	}

	m_cLogger->WriteLog("All Thread create done\n");
	return 0;
}

int CCustomerMain::Run()
{
	CProductMain::Run();
	return 0;
}

bool CCustomerMain::IsAllThreadEnd()
{
	if (m_cProductMain->IsAllThreadEnd() == true)
	{
		//timeEndPeriod(1);
		return true;
	}
	else
	{
		return false;
	}	
}

int CCustomerMain::OnAllThreadEndTimeout()
{
	return m_cProductMain->OnAllThreadEndTimeout();
}

ULONG RTFCNDCL SequenceThread(void * nContext)
{
	return m_cCustomerThread->SequenceThread(nContext);
}

ULONG RTFCNDCL IOScanThread(void * nContext)
{
	return m_cCustomerThread->IOScanThread(nContext);
}

ULONG RTFCNDCL IOOperationThread(void * nContext)
{
	return m_cCustomerThread->IOOperationThread(nContext);
}

ULONG RTFCNDCL TeachPointThread(void * nContext)
{
	return m_cCustomerThread->TeachPointThread(nContext);
}

ULONG RTFCNDCL ManualThread(void * nContext)
{
	return m_cCustomerThread->ManualThread(nContext);
}

ULONG RTFCNDCL MaintenanceThread(void * nContext)
{
	return m_cCustomerThread->MaintenanceThread(nContext);
}

ULONG RTFCNDCL AutoOperationThread(void * nContext)
{
	return m_cCustomerThread->AutoOperationThread(nContext);
}

ULONG RTFCNDCL StateThread(void * nContext)
{
	return m_cCustomerStateControl->RunState();
}

ULONG RTFCNDCL DummyThread(void * nContext)
{
	//return m_cCustomerThread->DummyThread(nContext);
	return 0;
}

ULONG RTFCNDCL MC1Thread(void * nContext)
{
	return m_cCustomerThread->MC1Thread(nContext);
}

ULONG RTFCNDCL MC2Thread(void * nContext)
{
	return m_cCustomerThread->MC2Thread(nContext);
}

ULONG RTFCNDCL MC3Thread(void * nContext)
{
	return m_cCustomerThread->MC3Thread(nContext);
}

ULONG RTFCNDCL MC4Thread(void * nContext)
{
	return m_cCustomerThread->MC4Thread(nContext);
}

ULONG RTFCNDCL MC5Thread(void * nContext)
{
	return m_cCustomerThread->MC5Thread(nContext);
}

ULONG RTFCNDCL MC6Thread(void * nContext)
{
	return m_cCustomerThread->MC6Thread(nContext);
}

ULONG RTFCNDCL MC7Thread(void * nContext)
{
	return m_cCustomerThread->MC7Thread(nContext);
}

ULONG RTFCNDCL PickAndPlace1YAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace1YAxisMotorThread(nContext);
}
ULONG RTFCNDCL InputTrayTableXAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->InputTrayTableXAxisMotorThread(nContext);
}
ULONG RTFCNDCL InputTrayTableYAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->InputTrayTableYAxisMotorThread(nContext);
}
ULONG RTFCNDCL InputTrayTableZAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->InputTrayTableZAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace2YAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace2YAxisMotorThread(nContext);
}
ULONG RTFCNDCL OutputTrayTableXAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->OutputTrayTableXAxisMotorThread(nContext);
}
ULONG RTFCNDCL OutputTrayTableYAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->OutputTrayTableYAxisMotorThread(nContext);
}
ULONG RTFCNDCL OutputTrayTableZAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->OutputTrayTableZAxisMotorThread(nContext);
}
ULONG RTFCNDCL InputVisionModuleMotorThread(void * nContext)
{
	return m_cCustomerThread->InputVisionModuleMotorThread(nContext);
}
ULONG RTFCNDCL S2VisionModuleMotorThread(void * nContext)
{
	return m_cCustomerThread->S2VisionModuleMotorThread(nContext);
}
ULONG RTFCNDCL S1VisionModuleMotorThread(void * nContext)
{
	return m_cCustomerThread->S1VisionModuleMotorThread(nContext);
}
ULONG RTFCNDCL S3VisionModuleMotorThread(void * nContext)
{
	return m_cCustomerThread->S3VisionModuleMotorThread(nContext);
}

ULONG RTFCNDCL PickAndPlace1XAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace1XAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace2XAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace2XAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace1ZAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace1ZAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace2ZAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace2ZAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace1ThetaAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace1ThetaAxisMotorThread(nContext);
}
ULONG RTFCNDCL PickAndPlace2ThetaAxisMotorThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace2ThetaAxisMotorThread(nContext);
}

ULONG RTFCNDCL InputVisionThread(void * nContext)
{
	return m_cCustomerThread->InputVisionThread(nContext);
}
ULONG RTFCNDCL S2VisionThread(void * nContext)
{
	return m_cCustomerThread->S2VisionThread(nContext);
}

ULONG RTFCNDCL OutputVisionThread(void * nContext)
{
	return m_cCustomerThread->OutputVisionThread(nContext);
}

ULONG RTFCNDCL BottomVisionThread(void * nContext)
{
	return m_cCustomerThread->BottomVisionThread(nContext);
}
ULONG RTFCNDCL S1VisionThread(void * nContext)
{
	return m_cCustomerThread->S1VisionThread(nContext);
}
ULONG RTFCNDCL S3VisionThread(void * nContext)
{
	return m_cCustomerThread->S3VisionThread(nContext);
}

ULONG RTFCNDCL PickAndPlaceSequenceThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlaceSequenceThread(nContext);
}

ULONG RTFCNDCL PickAndPlace2SequenceThread(void * nContext)
{
	return m_cCustomerThread->PickAndPlace2SequenceThread(nContext);
}

ULONG RTFCNDCL InputTrayTableSequenceThread(void * nContext)
{
	return m_cCustomerThread->InputTrayTableSequenceThread(nContext);
}

ULONG RTFCNDCL OutputAndRejectTrayTableSequenceThread(void * nContext)
{
	return m_cCustomerThread->OutputAndRejectTrayTableSequenceThread(nContext);
}

ULONG RTFCNDCL CheckRejectReplaceThread(void * nContext)
{
	return m_cCustomerThread->CheckRejectReplaceThread(nContext);
}
//ULONG RTFCNDCL CheckRejectRemoveThread(void * nContext)
//{
//	return m_cCustomerThread->CheckRejectRemoveThread(nContext);
//}
ULONG RTFCNDCL CheckRejectRemoveThread(void * nContext)
{
	return m_cCustomerThread->CheckRejectRemoveThread(nContext);
}

