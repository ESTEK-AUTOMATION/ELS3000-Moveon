//////////////////////////////////////////////////////////////////
//
// RtxApp.cpp - cpp file
//
// This file was generated using the RTX64 Application Template for Visual Studio.
//
// Created: 12/26/2019 8:36:31 PM 
// User: Estek-CharlesChong
//
//////////////////////////////////////////////////////////////////

#include "RtxApp.h"
   

int _tmain(int argc, _TCHAR * argv[])
{
     
    //
    // TO DO:  your program code here
    //
  
	CCustomerMain *m_cCustomerMain = new CCustomerMain();
	m_cCustomerMain->RunApplication();
	delete m_cCustomerMain;

    return 0;
}
