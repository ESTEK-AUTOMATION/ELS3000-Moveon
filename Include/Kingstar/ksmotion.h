///////////////////////////////////////////////////////////////////////////////
//
//
//			Copyright (c) 2011 - 2020 IntervalZero, Inc.  All rights reserved.
//
//
//File: ksmotion.h
//
//Abstract:
//
//		This file defines the KINGSTAR Motion API.
//		For Runtime features please import the ksapi.h file.
//
///////////////////////////////////////////////////////////////////////////////

#pragma once
#include "ksapi.h"

#ifdef __cplusplus
extern "C" {
#endif


///////////////////////////////////////////////////////////////////////////////
//
//	Enumerations.
//
///////////////////////////////////////////////////////////////////////////////
#define MODE_MANUAL					0x1
#define MODE_DIRECT_POSITION		0x2
#define MODE_DIRECT_VELOCITY		0x4
#define MODE_DIRECT_TORQUE			0x8
#define MODE_PID_VELOCITY			0x10
#define MODE_PID_TORQUE				0x20
#define MODE_INTERPOLATION_POSITION 0x40
#define MODE_INTERPOLATION_VELOCITY 0x80
#define MODE_INTERPOLATION_TORQUE	0x100
#define MODE_SLAVE_INTERPOLATION	0x200

#define AXIS_ERROR_COMMUNICATION	0x1
#define AXIS_ERROR_POSITION_LAG		0x2
#define AXIS_ERROR_COMMAND_JUMP		0x3
#define AXIS_ERROR_NO_READING		0x4

///////////////////////////////////////////////////////////////////////////////
//
//	Enumerations.
//
///////////////////////////////////////////////////////////////////////////////
typedef enum
{
	modeNotSet = -1,
	modeManual,
	modeDirectPos,
	modeDirectVel,
	modeDirectTor,
	modePidVel,
	modePidTor,
	modeMasterIntPos,
	modeMasterIntVel,
	modeMasterIntTor,
	modeSlaveInt,
} McControlMode;

typedef enum
{
	mcPositive = 0,
	mcNegative
} McPolarity;

typedef enum
{
	mcPrimary = 0,
	mcSecondary
} McEncoder;

typedef enum
{
	mcAborting = 0,
	mcBuffered,
	mcBlendingLow,
	mcBlendingPrevious,
	mcBlendingNext,
	mcBlendingHigh,
	mcCancel
} McBufferMode;

typedef enum
{
	mcPositiveDirection = 0,
	mcShortestWay,
	mcNegativeDirection,
	mcCurrentDirection
} McDirection;

typedef enum
{
	mcImmediately = 0,
	mcQueued
} McExecutionMode;

typedef enum
{
	mcConstantVelocity = 0,
	mcAccelerating,
	mcDecelerating,
	mcHalted
} McMotionState;

typedef enum
{
	// PLCopen Standard
	mcCommandedPosition = 1,
	mcSoftLimitPositive = 2,
	mcSoftLimitNegative = 3,
	mcEnableLimitPositive = 4,
	mcEnableLimitNegative = 5,
	mcEnablePosLagMonitoring = 6,
	mcMaxPositionLag = 7,
	mcMaxVelocitySystem = 8,
	mcMaxVelocityApplication = 9,
	mcActualVelocity = 10,
	mcCommandedVelocity = 11,
	mcMaxAccelerationSystem = 12,
	mcMaxAccelerationApplication = 13,
	mcMaxDecelerationSystem = 14,
	mcMaxDecelerationApplication = 15,
	mcMaxJerkSystem = 16,
	mcMaxJerkApplication = 17,

	// Axis Configuration
	mcAlias = 1001,
	mcInterpolationTime,
	mcAvailableControlModes,
	mcControlMode,
	mcHomeSwitchModuleType,
	mcHomeSwitchModuleIndex,
	mcHomeSwitchOffset,
	mcHomeSwitchInvert,
	mcPositiveLimitSwitchModuleType,
	mcPositiveLimitSwitchModuleIndex,
	mcPositiveLimitSwitchOffset,
	mcPositiveLimitSwitchInvert,
	mcNegativeLimitSwitchModuleType,
	mcNegativeLimitSwitchModuleIndex,
	mcNegativeLimitSwitchOffset,
	mcNegativeLimitSwitchInvert,
	mcSoftwareLimitPositive,
	mcSoftwareLimitNegative,
	mcEnablePositiveLimitSwitch,
	mcEnableNegativeLimitSwitch,
	mcEnableSoftwareLimitPositive,
	mcEnableSoftwareLimitNegative,
	mcEnableFollowingErrorMonitoring,
	mcCountPerUnitNumerator,
	mcCountPerUnitDenominator,
	mcCountPerUnitReverse,
	mcSecondaryCountPerUnitNumerator,
	mcSecondaryCountPerUnitDenominator,
	mcSecondaryCountPerUnitReverse,
	mcPosToVelRatio,
	mcTorquePolarityReverse,
	mcRealUnitConversion,
	mcActualFollowingError,
	mcEnablePositiveDirection,
	mcEnableNegativeDirection,

	// Motion and PID
	mcMotionProfileType,
	mcMinimumFollowingError,
	mcMaximumFollowingError,
	mcMaximumVelocity,
	mcAcceleration,
	mcDeceleration,
	mcJerk,
	mcJolt,
	mcFeedbackDelay,
	mcVelocityKP,
	mcVelocityKI,
	mcVelocityKILimitPercent,
	mcVelocityKD,
	mcVelocityKV,
	mcVelocityKAA,
	mcVelocityKAD,
	mcVelocityKJ,
	mcVelocityReducedGainsDelay,
	mcVelocityRgFactor,
	mcVelocityKIStoppedOnly,
	mcVelocityKDUseSecondEncoder,
	mcVelocityMinimumOutput,
	mcVelocityMaximumOutput,
	mcTorqueKP,
	mcTorqueKI,
	mcTorqueKILimitPercent,
	mcTorqueKD,
	mcTorqueKV,
	mcTorqueKAA,
	mcTorqueKAD,
	mcTorqueKJ,
	mcTorqueReducedGainsDelay,
	mcTorqueRgFactor,
	mcTorqueKIStoppedOnly,
	mcTorqueKDUseSecondEncoder,
	mcTorqueMinimumOutput,
	mcTorqueMaximumOutput,

	// Homing
	mcSlaveHomeOffset,
	mcSlaveHomingMode,
	mcSlaveControlLimit,
	mcCancelHome,
} McAxisParameter;

typedef enum
{
	mcCommandedValue = 0,
	mcSetValue,
	mcActualValue,
	mcSecondEncoderValue
} McSource;

typedef enum
{
	mcShortest = 0,
	mcCatchUp,
	mcSlowDown
} McSyncMode;

typedef enum
{
	profileUnitPerSecond = 0,
	profileDelayInSecond
} McProfileType;

typedef enum
{
	homingLatch = 0,
	homingSoft = 1,
	homingSensor = 2,
	homingSlave = 3,
	homingOnPosition = 4
} McHomingMode;

typedef enum
{
	camLinear = 1,
	camPoly5
} McCamInterpolationType;

typedef enum
{
	camAbsolute = 1,
	camRelative,
	camRampDistance,
	camRampTime
} McCamStartMode;

typedef enum
{
	mcAxisCoordSystem = 0,
	mcMachineCoordSystem,
	mcProductCoordSystem
} McCoordSystem;

typedef enum
{
	mcNone = 0,
	mcCornerDistance = 3,
	mcMaxCornerDeviation = 4
} McTransitionMode;

typedef enum
{
	mcBorder = 0,
	mcCenter,
	mcRadius
} McCircMode;

typedef enum
{
	mcShortPath = 0,
	mcLongPath
} McCircPathChoice;

///////////////////////////////////////////////////////////////////////////////
//
//	Structures.
//
///////////////////////////////////////////////////////////////////////////////
typedef struct
{
	int			PowerStatusModule;
	int			PowerStatusOffset;
	BOOL		PowerStatusInvert;
	int			AlarmModule;
	int			AlarmOffset;
	BOOL		AlarmInvert;
	int			PowerControlModule;
	int			PowerControlOffset;
	BOOL		PowerControlInvert;
	int			ResetModule;
	int			ResetOffset;
	BOOL		ResetInvert;
	int			EncoderModule;
	int			EncoderOffset;
	int			EncoderLength;
	int			TargetPositionModule;
	int			TargetPositionOffset;
	int			TargetPositionLength;
	int			TargetVelocityModule;
	int			TargetVelocityOffset;
	int			TargetVelocityLength;
	int			TargetTorqueModule;
	int			TargetTorqueOffset;
	int			TargetTorqueLength;
} McVirtualAxis;

typedef struct
{
	double		KP;            
	double		KI;            
	double		KILimitPercent;  
	double		KD;         
	double		KV;                
	double		KAA;           
	double		KAD;          
	double		KJ;			
	double		ReducedGainsDelay; 
	double		RgFactor;       
	BOOL		KIStoppedOnly;  
	BOOL		KDUseSecondEncoder;
	double		MinimumOutput;
	double		MaximumOutput;		
} McPidSettings;

typedef struct
{
	double		MinimumFollowingError;		
	double		MaximumFollowingError;		
	double		MaximumVelocity;
	double		Acceleration;
	double		Deceleration;
	double		Jerk;
	double		Jolt;
} McProfileSettings;

typedef struct
{
	int			TouchProbeId;
	BOOL		Software;
	BOOL		Edge;
	BOOL		IndexPulse;
	BOOL		AxisSwitch;
	int			Index;
	int			Offset;
} McProbeTrigger;

typedef struct
{
	int			TrackNumber;
	double		FirstOnPosition;
	double		LastOnPosition;
	int			AxisDirection;
	int			CamSwitchMode;
	double		Duration;
} McCamSwitch;

typedef struct
{
	BOOL		Axis;
	int 		Index;
	int 		Offset;
} McOutput;

typedef struct
{
	double		OnCompensation;
	double		OffCompensation;
	double		Hysteresis;
} McTrack;

typedef struct
{
	int			Length;
	BOOL		MasterAbsolute;
	BOOL		SlaveAbsolute;
	BOOL		Periodic;
	McCamInterpolationType InterpolationType;
} McCamTable;

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetAxisAvailableControlModes(int Index, int* Modes);
KS_API KsCommandStatus __stdcall SetAxisControlMode(int Index, McControlMode ControlMode);
KS_API KsError __stdcall SetVirtualAxis(int Index, McVirtualAxis axis);
KS_API KsCommandStatus __stdcall SetAxisInterpolation(int Index, double Cycle);
KS_API KsError __stdcall SetAxisCountsPerUnit(int Index, double Numerator, double Denominator, BOOL Reverse);
KS_API KsError __stdcall SetAxisSecondEncoderCountsPerUnit(int Index, double Numerator, double Denominator, BOOL Reverse);
KS_API KsError __stdcall SetAxisTorquePolarity(int Index, McPolarity Polarity);
KS_API KsError __stdcall SetAxisDirection(int Index, McPolarity Direction);
KS_API KsError __stdcall SetAxisEncoder(int Index, McEncoder Encoder);
KS_API KsError __stdcall EnableAxisUnitConversion(int Index, BOOL Active);
KS_API KsError __stdcall SetAxisHomeSwitch(int Index, BOOL AxisSwitch, int ModuleIndex, int BitOffset, BOOL Invert);
KS_API KsError __stdcall SetAxisPositiveLimitSwitch(int Index, BOOL AxisSwitch, int ModuleIndex, int BitOffset, BOOL Invert, BOOL enable);
KS_API KsError __stdcall SetAxisNegativeLimitSwitch(int Index, BOOL AxisSwitch, int ModuleIndex, int BitOffset, BOOL Invert, BOOL enable);
KS_API KsError __stdcall SetAxisMotionProfile(int Index, McProfileType ProfileType, McProfileSettings Settings);
KS_API KsError __stdcall SetAxisMotionProfileOverride(int Index, double VelocityFactor, double AccelerationFactor, double JerkFactor);
KS_API KsError __stdcall SetAxisVelocityPid(int Index, McPidSettings Settings);
KS_API KsError __stdcall SetAxisTorquePid(int Index, McPidSettings Settings);
KS_API KsCommandStatus __stdcall SetAxisPositionOffset(int Index, double Position, BOOL Relative, McExecutionMode ExecutionMode);
KS_API KsError __stdcall GetAxisParameter(int Index, McAxisParameter ParameterNumber, double* Value);
KS_API KsCommandStatus __stdcall SetAxisParameter(int Index, McAxisParameter ParameterNumber, double Value, McExecutionMode ExecutionMode);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Variable Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetAxisPosition(int Index, McSource Source, double* Position);
KS_API KsError __stdcall SetAxisPosition(int Index, double Position);
KS_API KsError __stdcall GetAxisVelocity(int Index, McSource Source, double* Velocity);
KS_API KsError __stdcall SetAxisVelocity(int Index, double Velocity);
KS_API KsError __stdcall GetAxisTorque(int Index, McSource Source, double* Torque);
KS_API KsError __stdcall SetAxisTorque(int Index, double Torque);
KS_API KsError __stdcall GetAxisTorqueOffset(int Index, double* Torque);
KS_API KsError __stdcall SetAxisTorqueOffset(int Index, double Torque);
KS_API KsError __stdcall GetAxisFollowingError(int Index, McSource Source, double* Error);
KS_API KsError __stdcall GetAxisDigitalInput(int Index, int InputNumber, BOOL* Value);
KS_API KsError __stdcall GetAxisDigitalOutput(int Index, int OutputNumber, BOOL* Value);
KS_API KsCommandStatus __stdcall SetAxisDigitalOutput(int Index, int OutputNumber, BOOL Value, McExecutionMode ExecutionMode);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Status Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetAxisState(int Index, AxisState* State);
KS_API KsError __stdcall GetAxisMotionState(int Index, McMotionState* State, McDirection* Direction);
KS_API KsError __stdcall GetAxisInfo(int Index, BOOL* HomeAbsSwitch, BOOL* LimitSwitchPos, BOOL* LimitSwitchNeg, BOOL* Simulation, BOOL* CommunicationReady, BOOL* ReadyForPowerOn, BOOL* PowerOn, BOOL* IsHomed, BOOL* AxisWarning);
KS_API KsError __stdcall GetAxisError(int Index, WORD* ErrorId);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall ResetAxis(int Index);
KS_API KsCommandStatus __stdcall PowerAxis(int Index, BOOL Enable, BOOL EnablePositiveDirection, BOOL EnableNegativeDirection);
KS_API KsCommandStatus __stdcall SetAxisTouchProbe(int Index, McProbeTrigger TriggerInput, BOOL WindowOnly, double firstPosition, double lastPosition, double* recordedPosition);
KS_API KsCommandStatus __stdcall SetAxisCamSwitch(int Index, int SwitchLength, McCamSwitch* Switches, int TrackLength, McOutput* Outputs, McTrack* Tracks, DWORD EnableMask, McSource ValueSource);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis PTP Motion Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall UpdateCommand(double Position, double Velocity, double EndVelocity, double Acceleration, double Deceleration, double Jerk, KsCommandStatus Status);

KS_API KsCommandStatus __stdcall HomeAxis(int Index, double Position, double Velocity, double EndVelocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction, McHomingMode HomingMode);
KS_API KsCommandStatus __stdcall StopAxis(int Index, double Deceleration, double Jerk);
KS_API KsCommandStatus __stdcall HaltAxis(int Index, double Deceleration, double Jerk, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall MoveAxisAbsolute(int Index, double Position, double Velocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall MoveAxisRelative(int Index, double Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall MoveAxisAdditive(int Index, double Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall MoveAxisVelocity(int Index, double Velocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall JogAxis(int Index, double Velocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction);
KS_API KsCommandStatus __stdcall InchAxis(int Index, double Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction);
KS_API KsCommandStatus __stdcall TestAxis(int Index, double Amplitude);
KS_API KsError __stdcall SimulateAxisAbsolute(int Index, double Position, double Velocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction, double* AccelerationTime, double* DecelerationTime, double* Duration);
KS_API KsError __stdcall SimulateAxisRelative(int Index, double Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, double* AccelerationTime, double* DecelerationTime, double* Duration);
KS_API KsCommandStatus __stdcall MoveAxisContinuousAbsolute(int Index, double Position, double Velocity, double EndVelocity, double Acceleration, double Deceleration, double Jerk, McDirection Direction, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall MoveAxisContinuousRelative(int Index, double Distance, double Velocity, double EndVelocity, double Acceleration, double Deceleration, double Jerk, McBufferMode BufferMode);

///////////////////////////////////////////////////////////////////////////////
//
//	Axis Synchronized Motion Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetCamTable(int Index, McCamTable* Table, int Length, double* MasterPositions, double* SlavePositions, double* PointParameters);
KS_API KsCommandStatus __stdcall SetCamTable(int Index, McCamTable Table, double* MasterPositions, double* SlavePositions, double* PointParameters);
KS_API KsCommandStatus __stdcall SetAxisCam(int Master, int Slave, BOOL Permanent, double MasterOffset, double SlaveOffset, double MasterScaling, double SlaveScaling, McCamStartMode StartMode, double StartParameter, McSource MasterValueSource, int Cam, McBufferMode BufferMode);
KS_API KsError __stdcall GetAxisCamInfo(int Index, BOOL* InSync, BOOL* EndOfProfile, int* RepetitionCount, int* RowIndex, double* SlaveTargetPosition, double* MinimumSlavePosition, double* MaximumSlavePosition);
KS_API KsError __stdcall SimulateAxisCam(int Index, double MasterPosition, double MasterOffset, double SlaveOffset, double MasterScaling, double SlaveScaling, int* RepetitionCount, int* RowIndex, double* SlaveTargetPosition, double* MinimumSlavePosition, double* MaximumSlavePosition);
KS_API KsCommandStatus __stdcall SetAxisGear(int Master, int Slave, BOOL Permanent, double Ratio, McSource MasterValueSource, double Acceleration, double Deceleration, double Jerk, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall SetAxisGearInPos(int Master, int Slave, BOOL Permanent, double Ratio, McSource MasterValueSource, double MasterSyncPosition, double SlaveSyncPosition, McSyncMode SyncMode, double MasterStartDistance, double Velocity, double Acceleration, double Deceleration, double Jerk, McBufferMode BufferMode);

///////////////////////////////////////////////////////////////////////////////
//
//	Group Configuration Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall AddAxisToGroup(int Index, int AxisIndex, int IndexInGroup);
KS_API KsError __stdcall RemoveAxisFromGroup(int Index, int IndexInGroup);
KS_API KsError __stdcall UngroupAllAxes(int Index);
KS_API KsError __stdcall GetGroupConfiguration(int Index, int IndexInGroup, McCoordSystem CoordSystem, int* AxisIndex);
KS_API KsError __stdcall GetGroupActualPosition(int Index, McCoordSystem CoordSystem, McSource Source, int BufferLength, double* Position);
KS_API KsError __stdcall GetGroupActualVelocity(int Index, McCoordSystem CoordSystem, McSource Source, int BufferLength, double* Velocity, double* PathVelocity);
KS_API KsError __stdcall GetGroupActualAcceleration(int Index, McCoordSystem CoordSystem, McSource Source, int BufferLength, double* Acceleration, double* PathAcceleration);

///////////////////////////////////////////////////////////////////////////////
//
//	Group Control Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall EnableGroup(int Index);
KS_API KsError __stdcall DisableGroup(int Index);
KS_API KsCommandStatus __stdcall SetGroupPositionOffset(int Index, int Length, double* Position, BOOL Relative, McCoordSystem CoordSystem, McExecutionMode ExecutionMode);
KS_API KsError __stdcall SetGroupMotionProfileOverride(int Index, double VelocityFactor, double AccelerationFactor, double JerkFactor);
KS_API KsCommandStatus __stdcall ResetGroup(int Index);

///////////////////////////////////////////////////////////////////////////////
//
//	Group Status Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsError __stdcall GetGroupState(int Index, AxisState* State);
KS_API KsError __stdcall GetGroupError(int Index, WORD* ErrorId);

///////////////////////////////////////////////////////////////////////////////
//
//	Group Motion Functions.
//
///////////////////////////////////////////////////////////////////////////////
KS_API KsCommandStatus __stdcall StopGroup(int Index, double Deceleration, double Jerk);
KS_API KsCommandStatus __stdcall HaltGroup(int Index, double Deceleration, double Jerk, McBufferMode BufferMode);
KS_API KsCommandStatus __stdcall JogGroup(int Index, int Length, double* Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem);
KS_API KsCommandStatus __stdcall InchGroup(int Index, int Length, double* Distance, double* Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem);
KS_API KsCommandStatus __stdcall MoveLinearAbsolute(int Index, int Length, double* Position, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveLinearRelative(int Index, int Length, double* Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveLinearAdditive(int Index, int Length, double* Distance, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveCircularAbsolute(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveCircularRelative(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveCircularAdditive(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveHelicalAbsolute(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Depth, double Pitch, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveHelicalRelative(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Depth, double Pitch, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);
KS_API KsCommandStatus __stdcall MoveHelicalAdditive(int Index, McCircMode CircMode, int Length, double* AuxPoint, double* EndPoint, McCircPathChoice PathChoice, double Depth, double Pitch, double Velocity, double Acceleration, double Deceleration, double Jerk, McCoordSystem CoordSystem, McBufferMode BufferMode, McTransitionMode TransitionMode, double* TransitionParameter);

#ifdef __cplusplus
}
#endif

