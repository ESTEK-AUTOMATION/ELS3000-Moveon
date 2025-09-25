#pragma once
#ifndef __IMOTIONLIBRARY_H_INCLUDED__
#define __IMOTIONLIBRARY_H_INCLUDED__
//#include <Windows.h>

interface iMotionLibrary
{
public:
	int Configure();
	int Initialize();
	int GetLastErrorCode();
	LPCTSTR GetErrorMsg(int errorCode);
	void ClearError();
};

#endif