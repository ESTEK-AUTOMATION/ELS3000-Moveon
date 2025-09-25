#include "CProductMain.h"

CProductMain::CProductMain()
{
}

CProductMain::~CProductMain()
{
}

int CProductMain::SetProductShareMemory(ProductSharedMemorySetting *sharedProductMemorySetting
	, ProductSharedMemoryTeachPoint *sharedProductMemoryTeachPoint
	, ProductSharedMemoryProduction *sharedProductMemoryProduction
	, ProductSharedMemoryCustomize *sharedProductMemoryCustomize
	, ProductSharedMemoryIO *sharedProductMemoryIO
	, ProductSharedMemoryModuleStatus *sharedProductMemoryModuleStatus
	, ProductSharedMemoryEvent *sharedProductMemoryEvent
	, ProductSharedMemoryGeneral *sharedProductMemoryGeneral
)
{
	smProductSetting = sharedProductMemorySetting;
	smProductTeachPoint = sharedProductMemoryTeachPoint;
	smProductProduction = sharedProductMemoryProduction;
	smProductCustomize = sharedProductMemoryCustomize;
	smProductIO = sharedProductMemoryIO;
	smProductModuleStatus = sharedProductMemoryModuleStatus;
	smProductEvent = sharedProductMemoryEvent;
	smProductGeneral = sharedProductMemoryGeneral;

	smSetting = sharedProductMemorySetting;
	smTeachPoint = sharedProductMemoryTeachPoint;
	smProduction = sharedProductMemoryProduction;
	smCustomize = sharedProductMemoryCustomize;
	smIO = sharedProductMemoryIO;
	smModuleStatus = sharedProductMemoryModuleStatus;
	smEvent = sharedProductMemoryEvent;
	smGeneral = sharedProductMemoryGeneral;

	SetShareMemory(sharedProductMemorySetting, sharedProductMemoryTeachPoint, sharedProductMemoryProduction, sharedProductMemoryCustomize
		, sharedProductMemoryIO, sharedProductMemoryModuleStatus, sharedProductMemoryEvent, sharedProductMemoryGeneral);
	return 0;
}

int CProductMain::SetProductClasses(CLogger *cLogger, CMotionLibrary *cMotionLibrary, CIO *cIO)
{
	m_cLogger = cLogger;
	m_cMotion = cMotionLibrary;
	m_cIO = cIO;

	SetPlatformClasses(cLogger, cMotionLibrary, cIO);
	return 0;
}

int CProductMain::Initialize()
{
	CPlatformMain::Initialize();
	RtPrintf("override Initialize From Product\n");

	return 0;
}

int CProductMain::CreateShareMemory()
{
	return 0;
}

int CProductMain::ResetVariables()
{
	CPlatformMain::ResetVariables();
	RtPrintf("override ResetVariables From Product\n");

	smProductEvent->StartInitializeMotionController1.Set = false;
	smProductEvent->InitializeMotionController1Done.Set = false;
	smProductEvent->StartInitializeMotionController2.Set = false;
	smProductEvent->InitializeMotionController2Done.Set = false;
	smProductEvent->StartInitializeMotionController3.Set = false;
	smProductEvent->InitializeMotionController3Done.Set = false;
	smProductEvent->StartInitializeMotionController4.Set = false;
	smProductEvent->InitializeMotionController4Done.Set = false;

#pragma region Thread
	smProductEvent->RTHD_RMAIN_MC1_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC2_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC3_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC4_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC5_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC6_END.Set = false;
	smProductEvent->RTHD_RMAIN_MC7_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set = false;

	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set = false;

	smProductEvent->RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END.Set = false;

	smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_RIGHT_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_FRONT_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_REAR_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END.Set = false;

	smProductEvent->RTHD_RMAIN_INPUT_VISION_END.Set = false;
	smProductEvent->RTHD_RMAIN_OUTPUT_VISION_END.Set = false;
	smProductEvent->RTHD_RMAIN_BOTTOM_VISION_END.Set = false;
	smProductEvent->RTHD_RMAIN_S2_VISION_END.Set = false;
	smProductEvent->RTHD_RMAIN_S3_VISION_END.Set = false;

	smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_END.Set = false;
	smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END.Set = false;
	smProductEvent->RTHD_RMAIN_PICK_AND_PLACE2_SEQUENCE_END.Set = false;
	smProductEvent->RTHD_RMAIN_CHECK_REJECT_REPLACE_END.Set = false;
	//smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set = false;
	smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set = false;
#pragma endregion

	m_cProductSequence->ResetVariablesBeforeHome();

	return 0;
}

int CProductMain::LoadSetting()
{
	CPlatformMain::LoadSetting();
	m_cProductMotorControl->LoadMotorSetting();
	return 0;
}

int CProductMain::CreateAllThread()
{
	CPlatformMain::CreateAllThread();
	m_cLogger->WriteLog("Create product thread done\n", GetLastError());
	return 0;
}

bool CProductMain::IsAllThreadEnd()
{
	if (m_cPlatformMain->IsAllThreadEnd()

		&& (smProductCustomize->EnableMotionController1 == false || (smProductCustomize->EnableMotionController1 == true && smProductEvent->RTHD_RMAIN_MC1_END.Set == true))
		&& (smProductCustomize->EnableMotionController2 == false || (smProductCustomize->EnableMotionController2 == true && smProductEvent->RTHD_RMAIN_MC2_END.Set == true))
		&& (smProductCustomize->EnableMotionController3 == false || (smProductCustomize->EnableMotionController3 == true && smProductEvent->RTHD_RMAIN_MC3_END.Set == true))
		&& (smProductCustomize->EnableMotionController4 == false || (smProductCustomize->EnableMotionController4 == true && smProductEvent->RTHD_RMAIN_MC4_END.Set == true))
		&& (smProductCustomize->EnableMotionController5 == false || (smProductCustomize->EnableMotionController5 == true && smProductEvent->RTHD_RMAIN_MC5_END.Set == true))
		&& (smProductCustomize->EnableMotionController6 == false || (smProductCustomize->EnableMotionController6 == true && smProductEvent->RTHD_RMAIN_MC6_END.Set == true))
		&& (smProductCustomize->EnableMotionController7 == false || (smProductCustomize->EnableMotionController7 == true && smProductEvent->RTHD_RMAIN_MC7_END.Set == true))

		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END.Set == true

		&& smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_RIGHT_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_FRONT_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_REAR_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END.Set == true
		
		&& smProductEvent->RTHD_RMAIN_INPUT_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S2_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_VISION_END.Set == true

		&& smProductEvent->RTHD_RMAIN_BOTTOM_VISION_END.Set == true
		//&& smProductEvent->RTHD_RMAIN_S2_VISION_END.Set == true
		&& smProductEvent->RTHD_RMAIN_S3_VISION_END.Set == true

		&& smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_PICK_AND_PLACE2_SEQUENCE_END.Set == true
		&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REPLACE_END.Set==true
		//&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set==true
		&& smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set==true
		)
	{
		return true;
	}
	else
		return false;
}

int CProductMain::OnAllThreadEndTimeout()
{
	m_cPlatformMain->OnAllThreadEndTimeout();

	if (smProductEvent->RTHD_RMAIN_MC1_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC1_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC2_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC2_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC3_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC3_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC4_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC4_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC5_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC5_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC6_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC6_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_MC7_END.Set == false
		)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_MC7_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_1_Y_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_TRAY_TABLE_X_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_TRAY_TABLE_Y_AXIS_MOTOR_END thread not end.\n");
	}

	if (smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_TRAY_TABLE_Z_AXIS_MOTOR_END thread not end.\n");
	}

	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_2_Y_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_OUTPUT_TRAY_TABLE_X_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_OUTPUT_TRAY_TABLE_Y_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_OUTPUT_TRAY_TABLE_Z_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_VISION_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_S2_VISION_MODULE_MOTOR_END thread not end.\n");
	}

	if (smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_SIDE_WALL_VISION_LEFT_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_RIGHT_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_SIDE_WALL_VISION_RIGHT_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_FRONT_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_SIDE_WALL_VISION_FRONT_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_SIDE_WALL_VISION_REAR_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_SIDE_WALL_VISION_REAR_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_S3_VISION_MODULE_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_1_X_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_2_X_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_1_Z_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_2_Z_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_1_T_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_2_T_AXIS_MOTOR_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_INPUT_VISION_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_VISION_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_S2_VISION_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_S2_VISION_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_OUTPUT_VISION_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_OUTPUT_VISION_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_BOTTOM_VISION_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_BOTTOM_VISION_END thread not end.\n");
	}
	//if (smProductEvent->RTHD_RMAIN_S2_VISION_END.Set == false)
	//{
	//	m_cLogger->WriteLog("RTHD_RMAIN_S2_VISION_END thread not end.\n");
	//}
	if (smProductEvent->RTHD_RMAIN_S3_VISION_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_S3_VISION_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_INPUT_TRAY_TABLE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_INPUT_TRAY_TABLE_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_OUTPUT_TRAY_TABLE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_OUTPUT_TRAY_TABLE_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_PICK_AND_PLACE2_SEQUENCE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_PICK_AND_PLACE_SEQUENCE_END thread not end.\n");
	}
	if (smProductEvent->RTHD_RMAIN_CHECK_REJECT_REPLACE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_CHECK_REJECT_REPLACE_END thread not end.\n");
	}
	//if (smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set == false)
	//{
	//	m_cLogger->WriteLog("RTHD_RMAIN_CHECK_REJECT_REMOVE_END thread not end.\n");
	//}
	if (smProductEvent->RTHD_RMAIN_CHECK_REJECT_REMOVE_END.Set == false)
	{
		m_cLogger->WriteLog("RTHD_RMAIN_CHECK_REJECT_REMOVE_END thread not end.\n");
	}
	
	return 0;
}