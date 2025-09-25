#ifndef _VMIO_LIBRARY
#define _VMIO_LIBRARY

#if defined VMIO_EXPORTS
#define VMIOEXP __declspec(dllexport)
#else
#define VMIOEXP __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

#include <windows.h>
#include "vmio_typedef.h"


	// System General Libraries
	VI32 VMIOEXP VMIO_open(VI32 * iBoardTotal, VI32 iMode);
	VI32 VMIOEXP VMIO_close(void);
	VI32 VMIOEXP VMIO_start_bus(VI32 iBoardID, VI32 iBusNo);
	VI32 VMIOEXP VMIO_stop_bus(VI32 iBoardID, VI32 iBusNo);
	VI32 VMIOEXP VMIO_set_bus_parameter(VI32 iBoardID, VI32 iBusNo, VI32 iParamCode, VI32 iParam);
	VI32 VMIOEXP VMIO_get_bus_parameter(VI32 iBoardID, VI32 iBusNo, VI32 iParamCode, VI32 *iParam);
	VI32 VMIOEXP VMIO_get_bus_status(VI32 iBoardID, VI32 iBusNo, VI32 *iBusStatus);
	VI32 VMIOEXP VMIO_get_bus_cycle_time(VI32 iBoardID, VI32 iBusNo, VI32 * iCycleTime);
	VI32 VMIOEXP VMIO_system_reset(VI32 iBoardID, VI32 iBusNo);
	VI32 VMIOEXP VMIO_firmware_revision(VI32 iBoardID, VI32 *iRev);
	VI32 VMIOEXP VMIO_board_revision(VI32 iBoardID, VI32 *iRev);
	VI32 VMIOEXP VMIO_dll_revision(VI32 *iRev);
	VI32 VMIOEXP VMIO_get_module_name(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VI32 *iModuleName);
	VI32 VMIOEXP VMIO_get_module_online_status(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VI32 *iOnline);
	VI32 VMIOEXP VMIO_get_module_error(VI32 iBoardID, VI32 iBusNo, VU64 *uiErrBoard);
	

	// IO Features Libraries
	VI32 VMIOEXP VMIO_d_get_output(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 *uiDO);
	VI32 VMIOEXP VMIO_d_set_output(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 uiDO);
	VI32 VMIOEXP VMIO_d_get_input(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 *uiDI);
	VI32 VMIOEXP VMIO_d_set_output_bit(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VI32 iBitNo, VI32 iBitStatus);
	VI32 VMIOEXP VMIO_d_get_input_bit(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VI32 iBitNo, VI32 *iBitStatus);

	//--------------- The following section of definition is added for new remote configurable IO ---------------//
	VI32 VMIOEXP VMIO_retrieve_module_configuration(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo);
	VI32 VMIOEXP VMIO_get_input_inverse_setting(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 *uiStatus);
	VI32 VMIOEXP VMIO_get_io_pin_config(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VI32 iGroupNo, VU32 *uiPinConfig);
	VI32 VMIOEXP VMIO_get_input_latch_setting(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 *uiStatus);
	VI32 VMIOEXP VMIO_clear_latch(VI32 iBoardID, VI32 iBusNo, VI32 iModuleNo, VU32 uiStatus);

#ifdef __cplusplus
}
#endif	// VMIO_EXPORTS

#endif	// _VMIO_LIBRARY