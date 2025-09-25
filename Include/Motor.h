#pragma once


#ifndef MOTOR_H
#define MOTOR_H
#include "EstekLibrary.h"

#define INSTALL_KINGSTAR_DRIVER 1
#define INSTALL_ETEL_DRIVER 1
#define INSTALL_GALIL_DRIVER 1

#ifndef MOTORQUANTITY
	#define MOTORQUANTITY 32
#endif

#pragma region struct
//public:
	struct MotorProfile
    {
	public:
        int Index;
        char PositionName[256];
        int Position;
        char ProfileName[256];
        int ProfileNo;
        int Speed;
        int Acceleration;
        int Deceleration;
        char Unit[256];
        char Axis[256];
        int AxisNo;
        char Module[256];
        char Description[256];
        char Remark[256];
        bool EnableTeaching;
        bool Visible;
    };
	struct MotorConfiguration
    {
	public:
		char Controller[256];
		char ControllerDetails[256];
		bool Enable;
		bool Online;
		char Axis[256];		
        double UnitInOnePulse;//micron or degree
        char UnitName[256];
        int AxisInPositionToleranceInUnitPulse;
    };

struct OfflineParameter
    {
	public:
        char Axis[256];
        bool IsMotorOn;
        bool IsMotorHome;
        bool IsStop;
        int CommandPulse;
        int EncoderPulse;
    };
#pragma endregion
#endif