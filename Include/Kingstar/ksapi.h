///////////////////////////////////////////////////////////////////////////////
//
//
//			Copyright (c) 2011 - 2020 IntervalZero, Inc.  All rights reserved.
//
//
//File: ksapi.h
//
//Abstract:
//
//		This file defines the KINGSTAR Runtime API.
//		For Motion features please import the ksmotion.h file.
//
///////////////////////////////////////////////////////////////////////////////

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
#include <rtapi.h>      // RTX64 APIs that can be used in real-time or Windows applications.

#ifdef UNDER_RTSS
#include <rtssapi.h>    // RTX64 APIs that can only be used in real-time applications.
#endif // UNDER_RTSS

#if defined UNDER_RTSS
  #if defined KINGSTAR_EXPORTS
    #define KS_API __declspec(dllexport)
  #else
    #define KS_API __declspec(dllimport)
  #endif
#else
  #if defined DLL64_EXPORTS
    #define KS_API __declspec(dllexport)
  #else
    #define KS_API __declspec(dllimport)
  #endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

///////////////////////////////////////////////////////////////////////////////
//
//	Defines.
//
///////////////////////////////////////////////////////////////////////////////
#define NOSTATUS					{0}

#define cycle100					0.0001
#define cycle125					0.000125
#define cycle250					0.000250
#define cycle500					0.0005
#define cycle1000					0.001

#define VAR_STATUS_WORD				0x1
#define VAR_ACTUAL_POSITION 		0x2
#define VAR_SECOND_ENCODER  		0x4
#define VAR_ACTUAL_VELOCITY			0x8
#define VAR_ACTUAL_TORQUE			0x10
#define VAR_TOUCH_PROBE_STATUS		0x20
#define VAR_MOP_DISPLAY				0x40
#define VAR_AXIS_INPUTS			0x80

#define VAR_CONTROL_WORD			0x1
#define VAR_TARGET_POSITION 		0x2
#define VAR_TARGET_VELOCITY  		0x4
#define VAR_TARGET_TORQUE			0x8
#define VAR_TORQUE_OFFSET			0x10
#define VAR_TOUCH_PROBE_CONTROL		0x20
#define VAR_MOP						0x40
#define VAR_AXIS_OUTPUTS			0x80
///////////////////////////////////////////////////////////////////////////////
//
//	Enumerations.
//
///////////////////////////////////////////////////////////////////////////////
typedef enum
{
	// Normal Status codes 0x0000
	errNoError = 0,
	errBusy = 1,

	// API Errors 0x1000
	errNullParameter = 0x1000,
	errWrongParameter = 0x1001,
	errSubsystemNotRunning = 0x1002,
	errLinkBusy = 0x1003,
	errNoLicense = 0x1004,
	errWrongEnvironment = 0x1005,
	errVariableUnavailable = 0x1006,
	errVariableSizeIncoherent = 0x1007,
	errUserCancelled = 0x1008,
	errTimeout = 0x1009,

	// Startup Errors 0x2000
	errNoNicSelected = 0x2000,
	errNicNotFound = 0x2001,
	errLinkDisconnected = 0x2002,
	errNoSlave = 0x2003,
	errUnknownSlave = 0x2004,
	errNoMemory = 0x2005,
	errNoFile = 0x2006,
	errIncorrectFormat = 0x2007,
	errWrongDeviceCount = 0x2008,
	errInitFailed = 0x2009,
	errPreOpFailed = 0x200A,
	errSafeOpFailed = 0x200B,
	errOpFailed = 0x200C,
	errDcFailed = 0x200D,
	errMdpFailed = 0x200E,
	errFailed = 0x200F,

	// Motion Errors 0x3000
	errWrongControlMode = 0x3000,
	errCommandBufferFull = 0x3001,
	errInvalidHandle = 0x3002,
	errWrongAxisState = 0x3003,
	errNoSensorDefined = 0x3004,
	errFollowingError = 0x3005,
	errMinimumLimit = 0x3006,
	errMaximumLimit = 0x3007,
	errCommandJump = 0x3008,

	// SDO Errors 0x4000
	errSdoToggleBit = 0x4000,
	errSdoTimeout = 0x4001,
	errSdoCommandSpecifier = 0x4002,
	errSdoOutOfMemory = 0x4003,
	errSdoUnsupportedAccess = 0x4004,
	errSdoWriteOnly = 0x4005,
	errSdoReadOnly = 0x4006,
	errSdoSubindexReadOnly = 0x4007,
	errSdoNoCompleteAccess = 0x4008,
	errSdoObjectTooLong = 0x4009,
	errSdoObjectInPdo = 0x400A,
	errSdoObjectNotExist = 0x400B,
	errSdoNoPdoMapping = 0x400C,
	errSdoPdoLengthExceeded = 0x400D,
	errSdoParameterIncompatible = 0x400E,
	errSdoInternalIncompatible = 0x400F,
	errSdoHardwareError = 0x4010,
	errSdoLengthIncorrect = 0x4011,
	errSdoLengthTooHigh = 0x4012,
	errSdoLengthTooLow = 0x4013,
	errSdoSubindexNotExist = 0x4014,
	errSdoValueOutOfRange = 0x4015,
	errSdoValueTooHigh = 0x4016,
	errSdoValueTooLow = 0x4017,
	errSdoMaxBelowMin = 0x4018,
	errSdoGeneralError = 0x4019,
	errSdoCannotTransfer = 0x401A,
	errSdoCannotTransferLocal = 0x401B,
	errSdoWrongState = 0x401C,
	errSdoDictionaryNotAvailable = 0x401D
} KsError;

typedef enum
{
	featureCycleTime = 0,
	featureServerLog,
	featureAutoRestart,
	featureAutoRepair,
	featureAutoConfig,
	featureDc,
	featureDcCheck,
	featureDcMasterShift,
	featureRedundancy,
	featureHotConnect,
	featureAccessMode,
	featureAxisInput,
	featureAxisOutput,
	featureSecondEncoder,
	featureTorqueOffset,
	featureSyncControlMode,
	featureActualVelocity,
	featureActualTorque,
	featureActualCurrent,
	featureFollowingError,
	featureTouchProbe,
	featureMaxTorque,
	featureMaxCurrent,
	featureProfilePosition
} KsFeature;

typedef enum
{
	ecatUnknown = -1,
	ecatOffline = 0,
	ecatInit = 1,
	ecatPreOP = 2,
	ecatBoot = 3,
	ecatSafeOP = 4,
	ecatOP = 8
} EthercatState;

typedef enum
{
	axisOffline = 0,
	axisCommunicationError,
	axisMotionError,
	axisDisabled,
	axisLocked,
	axisStandStill,
	axisHoming,
	axisDiscreteMotion,
	axisContinuousMotion,
	axisSynchronizedMotion
} AxisState;

typedef enum
{
	configEtherCAT = 0,
	configIoLink,
	configCANopen,
	configEl6695,
	configEsi,
	configSafety
} KsConfigurationType;

typedef enum
{
	accessPos = 0,
	accessVel,
	accessTor,
	accessPosVel,
	accessVelPos,
	accessPosVelTor,
	accessVelPosTor
} KsAccessMode;

typedef enum
{
	logEthercat = 0,
	logAxis,
	logGroup,
	logInput,
	logOutput
} KsLogSource;

typedef enum
{
	logActualPosition = 0,
	logActualVelocity,
	logActualTorque,
	logActualCurrent,
	logFollowingError,
	logSecondEncoder,
	logInterpolationPosition,
	logInterpolationVelocity,
	logInterpolationTorque,
	logInterpolationAcceleration,
	logInterpolationJerk,
	logDigitalInputs,
	logDigitalOutputs
} KsLogVariable;

typedef enum
{
	logBool = 0,
	logByte,
	logSInt,
	logWord,
	logInt,
	logDWord,
	logDInt,
	logFloat,
	logLWord,
	logLInt ,
	logDouble
} KsLogDataType;

typedef enum
{
	logImmediately = 0,
	logAboveLimit,
	logBelowLimit,
	logAboveAbs,
	logBelowAbs
} KsLogTriggerType;

typedef enum
{
	dcInput = 0,
	dcCurrent,
	dcOutput
} DcCycle;

///////////////////////////////////////////////////////////////////////////////
//
//	Structures.
//
///////////////////////////////////////////////////////////////////////////////
typedef struct {
	EthercatState	State;
	EthercatState	RequestedState;
	int				SlaveCount;
	int				IOCount;
	int				AxesCount;
	EthercatState	SlaveStates[256];
	EthercatState	IOStates[256];
	AxisState		AxesStates[256];
} SubsystemStatus;

typedef struct {
	USHORT			FixedAddress;
	char			Type[64];
	char			Name[64];
	UINT			DeviceType;
	UINT			VendorId;
	UINT			ProductCode;
	UINT			RevisionNo;
	UINT			SerialNo;
	USHORT			MailboxIn;
	USHORT			MailboxOut;
	BYTE			LinkStatus;
	BYTE			LinkPreset;
	BYTE			Flags;
	USHORT			StationAddress;
	USHORT			DlStatus;
	USHORT			AlStatus;
	USHORT			AlControl;
	USHORT			AlStatusCode;
	BYTE			LinkConnStatus;
	BYTE			LinkConnControl;
	USHORT			PortAddress[4];
	UINT			Crc[4];
	UINT			CyclicWc;
	UINT			NotPresent;
	UINT			AbnormalState;
} SlaveDiagnostics;

typedef struct {
	UINT			VendorId;
	UINT			ProductCode;
	UINT			RevisionNo;
	UINT			SerialNo;
	UINT			CyclicPacketLost;
	UINT			AcyclicPacketLost;
	int				SlaveCount;
	SlaveDiagnostics Slaves[256];
} SubsystemDiagnostics;

typedef struct
{
	CHAR			Name[64];
	DWORD			VendorId;
	DWORD			ProductCode;
	DWORD			RevisionNumber;
	DWORD			SerialNumber;
	int 			SlaveId;
	int				SlotId;
	USHORT			PhysAddress;
	USHORT			AliasAddress;
	USHORT			ExplicitId;
	EthercatState	State;
	EthercatState	RequestedState;
	int				InputLength;
	int				OutputLength;
	int				CycleTime;
	WORD			VariableIndexOffset;
} SlaveStatus;

typedef struct {
	WORD			Index;
	WORD			DataType;
	BYTE			MaxSubIndex;
	BYTE			ObjectCode;
	CHAR			Name[64];
} SdoObjectDescription;

typedef struct {
	WORD			Index;
	BYTE			SubIndex;
	BYTE			ValueInfo;
	WORD			DataType;
	WORD			BitLength;
	WORD			ObjectAccess;
	BYTE			Data[64];
} SdoEntryDescription;

typedef struct
{
	USHORT			Index;
	BYTE			SubIndex;
	INT				Length;
	BYTE			Data[8];
} SdoCommand;

typedef struct
{
	UCHAR			Revision;
	UCHAR			Spdu;
	USHORT			Control;
	INT				InputLength;
	INT				OutputLength;
} IoLinkSetting;

typedef struct
{
	BOOL			Enable;
	INT				Length;
	USHORT			Index;
	BYTE			Trigger;
} CanPdo;

typedef struct
{
	INT				RxPdoCount;
	CanPdo			RxPdos[4];
	INT				TxPdoCount;
	CanPdo			TxPdos[4];
	INT				SdoCommandCount;
	SdoCommand		SdoCommands[16];
} CanOpenSetting;

typedef struct
{
	INT				InputLength;
	INT				OutputLength;
	INT				SdoCommandCount;
	SdoCommand		SdoCommands[16];
} EcatMdpSetting;

typedef struct
{
	INT				ControllerIndex;
	BOOL			SafetyDataFirst;
	INT				SafetyInputLength;
	INT				SafetyOutputLength;
	INT				StandardInputLength;
	INT				StandardOutputLength;
} SafetySetting;

typedef struct
{
	KsLogSource		Source;
	INT				Index;
	KsLogVariable	Variable;
	INT				Offset;
	KsLogDataType	DataType;
} KsLogChannel;

typedef struct
{
	HANDLE		Handle;
	PVOID		Value;
	INT 		ValueLength;
	BOOL		Done;
	BOOL		InSync;
	BOOL		InVelocity;
	BOOL		Busy;
	BOOL		Active;
	BOOL		CommandAborted;
	BOOL		Error;
	KsError		ErrorId;
} KsCommandStatus;

typedef int(*AppCallback)(PVOID Context);

///////////////////////////////////////////////////////////////////////////////
//
//	Subsystem Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall SetCycleTime(double Cycle);
KS_API KsError __stdcall EnableServerLog(BOOL Active);
KS_API KsError __stdcall EnableAutoRestart(BOOL Active);
KS_API KsError __stdcall EnableAutoRepair(BOOL Active);
KS_API KsError __stdcall EnableAutoConfig(BOOL Active);
KS_API KsError __stdcall ConfigureDc(BOOL Active, BOOL CheckStatus, BOOL MasterShift, int ReferenceIndex);
KS_API KsError __stdcall EnableRedundancy(BOOL Active);
KS_API KsError __stdcall EnableHotConnect(BOOL Active);
KS_API KsError __stdcall GetFeatureStatus(KsFeature Feature, double* Status);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Variable Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall SetAxisAccessMode(KsAccessMode Mode);
KS_API KsError __stdcall EnableAxisInput(BOOL Active);
KS_API KsError __stdcall EnableAxisOutput(BOOL Active);
KS_API KsError __stdcall EnableSecondEncoder(BOOL Active);
KS_API KsError __stdcall EnableActualVelocity(BOOL Active);
KS_API KsError __stdcall EnableActualTorque(BOOL Active);
KS_API KsError __stdcall EnableActualCurrent(BOOL Active);
KS_API KsError __stdcall EnableTorqueOffset(BOOL Active);
KS_API KsError __stdcall EnableSynchronizedControlMode(BOOL Active);
KS_API KsError __stdcall EnableTouchProbe(BOOL Active);
KS_API KsError __stdcall EnableProfilePosition(BOOL Active);
KS_API KsError __stdcall EnableFollowingError(BOOL Active);
KS_API KsError __stdcall EnableMaxTorque(BOOL Active);
KS_API KsError __stdcall EnableMaxCurrent(BOOL Active);

///////////////////////////////////////////////////////////////////////////////
//
//	Modular slaves Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetConfiguredModuleCount(int* Count);
KS_API KsError __stdcall AddModuleConfiguration(int SlaveId, int ModuleId, KsConfigurationType ConfigurationType, VOID* Configuration);
KS_API KsError __stdcall GetModuleConfiguration(int ConfigurationIndex, int* SlaveId, int* ModuleId, KsConfigurationType* ConfigurationType, VOID* Configuration);
KS_API KsError __stdcall RemoveModuleConfiguration(int ConfigurationIndex);

///////////////////////////////////////////////////////////////////////////////
//
//	Simulated Hardware Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetConfiguredIoCount(int* Count);
KS_API KsError __stdcall SetConfiguredIoCount(int Count);
KS_API KsError __stdcall ConfigureIo(int Index, SlaveStatus Details);
KS_API KsError __stdcall GetConfiguredAxesCount(int* Count);
KS_API KsError __stdcall SetConfiguredAxesCount(int Count);
KS_API KsError __stdcall ConfigureAxis(int Index, SlaveStatus Details, int Resolution);

///////////////////////////////////////////////////////////////////////////////
//
//	Subsystem Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall Create(int Instance, int IdealProcessor);
KS_API KsError __stdcall Destroy();
KS_API KsError __stdcall Unlink();
KS_API KsCommandStatus __stdcall Start();
KS_API KsCommandStatus __stdcall StartFromConfiguration(char* Configuration);
KS_API KsCommandStatus __stdcall Stop();
KS_API KsCommandStatus __stdcall Restart();
KS_API KsCommandStatus __stdcall RequestState(EthercatState State);
KS_API KsError __stdcall GetStatus(SubsystemStatus* Status, SubsystemDiagnostics* Diagnostics);
KS_API KsError __stdcall RegisterCallback(AppCallback callback, PVOID context);
KS_API KsError __stdcall EnableAliases(BOOL Active);
KS_API KsError __stdcall GetAliasesStatus(BOOL* Active);
KS_API KsError __stdcall GetDCSystemTime(DcCycle Cycle, DWORD64* Time);
KS_API KsCommandStatus __stdcall Log(int Length, KsLogChannel* Channels, int TriggerChannel, double TriggerValue, KsLogTriggerType TriggerType, double Duration);
KS_API KsCommandStatus __stdcall StopLog();

///////////////////////////////////////////////////////////////////////////////
//
//	Heartbeat Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall SetHeartbeat(BOOL Enable, double Timeout);
KS_API KsError __stdcall PulseHeartbeat();

///////////////////////////////////////////////////////////////////////////////
//
//	Slave Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall RestartSlave(int SlaveId);
KS_API KsCommandStatus __stdcall RequestSlaveState(int SlaveId, EthercatState State);
KS_API KsError __stdcall GetSlaveById(int SlaveId, SlaveStatus* Details);
KS_API KsCommandStatus __stdcall ReadSlaveRegister(int SlaveId, int Offset, int Length, BYTE* Data);
KS_API KsCommandStatus __stdcall WriteSlaveRegister(int SlaveId, int Offset, int Length, BYTE* Data);
KS_API KsCommandStatus __stdcall ReadSlaveEEprom(int SlaveId, int Offset, DWORD* Data);
KS_API KsCommandStatus __stdcall WriteSlaveEEprom(int SlaveId, int Offset, DWORD Data);
KS_API KsCommandStatus __stdcall ReadSlaveAlias(int SlaveId, WORD* Alias);
KS_API KsCommandStatus __stdcall WriteSlaveAlias(int SlaveId, WORD Alias);

///////////////////////////////////////////////////////////////////////////////
//
//	I/O Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetIOByIndex(int Index, SlaveStatus* Details);
KS_API KsError __stdcall ReadIOAlias(int Index, int* Alias);
KS_API KsError __stdcall WriteIOAlias(int Index, int Alias);
KS_API KsError __stdcall GetIOBuffers(int Index, void** InputBuffer, void** OutputBuffer);
KS_API KsError __stdcall WriteOutputBit(int Index, int BitOffset, BOOL Value);
KS_API KsError __stdcall WriteOutputByte(int Index, int ByteOffset, BYTE Value);
KS_API KsError __stdcall WriteOutputWord(int Index, int ByteOffset, WORD Value);
KS_API KsError __stdcall WriteOutputDWord(int Index, int ByteOffset, DWORD Value);
KS_API KsError __stdcall ReadOutputBit(int Index, int BitOffset, BOOL* Value);
KS_API KsError __stdcall ReadOutputByte(int Index, int ByteOffset, BYTE* Value);
KS_API KsError __stdcall ReadOutputWord(int Index, int ByteOffset, WORD* Value);
KS_API KsError __stdcall ReadOutputDWord(int Index, int ByteOffset, DWORD* Value);
KS_API KsError __stdcall ForceInputBit(int Index, int BitOffset, BOOL Value);
KS_API KsError __stdcall ForceInputByte(int Index, int ByteOffset, BYTE Value);
KS_API KsError __stdcall ForceInputWord(int Index, int ByteOffset, WORD Value);
KS_API KsError __stdcall ForceInputDWord(int Index, int ByteOffset, DWORD Value);
KS_API KsError __stdcall ReadInputBit(int Index, int BitOffset, BOOL* Value);
KS_API KsError __stdcall ReadInputByte(int Index, int ByteOffset, BYTE* Value);
KS_API KsError __stdcall ReadInputWord(int Index, int ByteOffset, WORD* Value);
KS_API KsError __stdcall ReadInputDWord(int Index, int ByteOffset, DWORD* Value);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetAxisByIndex(int Index, SlaveStatus* Details, int* Resolution, DWORD* InputVariables, DWORD* OutputVariables);
KS_API KsError __stdcall ReadAxisAlias(int Index, int* Alias);
KS_API KsError __stdcall WriteAxisAlias(int Index, int Alias);
KS_API KsError __stdcall ReadAxisStatusWord(int Index, WORD* Value);
KS_API KsError __stdcall ReadAxisActualPosition(int Index, int* Value);
KS_API KsError __stdcall ReadAxisActualVelocity(int Index, int* Value);
KS_API KsError __stdcall ReadAxisActualTorque(int Index, short* Value);
KS_API KsError __stdcall ReadAxisActualCurrent(int Index, short* Value);
KS_API KsError __stdcall ReadAxisSecondEncoder(int Index, int* Value);
KS_API KsError __stdcall ReadAxisFollowingError(int Index, int* Value);
KS_API KsError __stdcall ReadAxisTouchProbeStatus(int Index, WORD* Value);
KS_API KsError __stdcall ReadAxisMopDisplay(int Index, BYTE* Value);
KS_API KsError __stdcall ReadAxisInputs(int Index, DWORD* Value);
KS_API KsError __stdcall ForceAxisInputs(int Index, DWORD Value);
KS_API KsError __stdcall ReadAxisCustomInput1(int Index, DWORD* Value);
KS_API KsError __stdcall ReadAxisCustomInput2(int Index, DWORD* Value);
KS_API KsError __stdcall WriteAxisControlWord(int Index, WORD Value);
KS_API KsError __stdcall WriteAxisTargetPosition(int Index, int Value);
KS_API KsError __stdcall WriteAxisTargetVelocity(int Index, int Value);
KS_API KsError __stdcall WriteAxisTargetTorque(int Index, short Value);
KS_API KsError __stdcall WriteAxisTorqueOffset(int Index, short Value);
KS_API KsError __stdcall WriteAxisMaxTorque(int Index, short Value);
KS_API KsError __stdcall WriteAxisMaxCurrent(int Index, short Value);
KS_API KsError __stdcall WriteAxisTouchProbeControl(int Index, WORD Value);
KS_API KsError __stdcall WriteAxisMop(int Index, BYTE Value);
KS_API KsError __stdcall ReadAxisOutputs(int Index, DWORD* Value);
KS_API KsError __stdcall WriteAxisOutputs(int Index, DWORD Value);
KS_API KsError __stdcall WriteAxisProfileVelocity(int Index, int Value);
KS_API KsError __stdcall WriteAxisProfileAcceleration(int Index, int Value);
KS_API KsError __stdcall WriteAxisProfileDeceleration(int Index, int Value);
KS_API KsError __stdcall WriteAxisCustomOutput1(int Index, DWORD Value);
KS_API KsError __stdcall WriteAxisCustomOutput2(int Index, DWORD Value);

///////////////////////////////////////////////////////////////////////////////
//
//	Command Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall WaitForCommand(double Timeout, BOOL AbortOnTimeout, KsCommandStatus Status);
KS_API KsCommandStatus __stdcall GetCommandStatus(KsCommandStatus Status);
KS_API KsCommandStatus __stdcall AbortCommand(KsCommandStatus Status);

///////////////////////////////////////////////////////////////////////////////
//
//	Mailbox Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall AoeReadSdoObject(int SlaveId, int Port, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall AoeWriteSdoObject(int SlaveId, int Port, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall AoeReadCommand(int SlaveId, int Port, int IndexGroup, int IndexOffset, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall AoeWriteCommand(int SlaveId, int Port, int IndexGroup, int IndexOffset, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall AoeWriteControlCommand(int SlaveId, int Port, WORD AoeState, WORD DeviceState, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall AoeReadWriteCommand(int SlaveId, int Port, int IndexGroup, int IndexOffset, BYTE* ReadValue, int ReadLength, BYTE* WriteValue, int WriteLength);

KS_API KsCommandStatus __stdcall CoeReadSdoObject(int SlaveId, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall CoeWriteSdoObject(int SlaveId, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall CoeReadSdoODList(int SlaveId, int ListType, BYTE* Data, int Length, int* LengthRead);
KS_API KsCommandStatus __stdcall CoeReadSdoObjectDescription(int SlaveId, SdoObjectDescription* Data);
KS_API KsCommandStatus __stdcall CoeReadSdoEntryDescription(int SlaveId, SdoEntryDescription* Data);

KS_API KsCommandStatus __stdcall EoeSetSlaveEoeIp(int SlaveId, unsigned long long Mac, DWORD Ip, DWORD Subnet, DWORD Gateway, DWORD Dns, char* DnsName);

KS_API KsCommandStatus __stdcall FoeReadFileToBuffer(int SlaveId, BOOL BootState, int NameLength, char* Name, DWORD Password, int BufferLength, BYTE* Buffer, int* ReadLength);
KS_API KsCommandStatus __stdcall FoeReadFileToFile(int SlaveId, BOOL BootState, int NameLength, char* Name, DWORD Password, char* File);
KS_API KsCommandStatus __stdcall FoeWriteFileFromBuffer(int SlaveId, BOOL BootState, int NameLength, char* Name, DWORD Password, int BufferLength, BYTE* Buffer);
KS_API KsCommandStatus __stdcall FoeWriteFileFromFile(int SlaveId, BOOL BootState, int NameLength, char* Name, DWORD Password, char* File);

KS_API KsCommandStatus __stdcall ReadIOSdoObject(int Index, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall WriteIOSdoObject(int Index, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall ReadAxisSdoObject(int Index, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);
KS_API KsCommandStatus __stdcall WriteAxisSdoObject(int Index, int ObIndex, int ObSubIndex, BOOL CompleteAccess, BYTE* Value, int Length);

///////////////////////////////////////////////////////////////////////////////
//
//	KINGSTAR Memory Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetMemoryPointer(void** ppMemory);
KS_API KsError __stdcall WriteMemoryBit(int Offset, int BitOffset, BOOL Value);
KS_API KsError __stdcall WriteMemoryByte(int Offset, BYTE Value);
KS_API KsError __stdcall WriteMemoryWord(int Offset, WORD Value);
KS_API KsError __stdcall WriteMemoryDWord(int Offset, DWORD Value);
KS_API KsError __stdcall WriteMemoryLWord(int Offset, unsigned long long Value);
KS_API KsError __stdcall ReadMemoryBit(int Offset, int BitOffset, BOOL* Value);
KS_API KsError __stdcall ReadMemoryByte(int Offset, BYTE* Value);
KS_API KsError __stdcall ReadMemoryWord(int Offset, WORD* Value);
KS_API KsError __stdcall ReadMemoryDWord(int Offset, DWORD* Value);
KS_API KsError __stdcall ReadMemoryLWord(int Offset, unsigned long long* Value);

#ifdef __cplusplus
}
#endif

