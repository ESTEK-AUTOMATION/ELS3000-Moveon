#include "CProductShareVariables.h"

CProductShareVariables::CProductShareVariables()
{
	MOTION_TIMEOUT = 50000;//Default 30000
	VISION_TIMEOUT = 5000;//Default 5000
}

CProductShareVariables::~CProductShareVariables()
{
}

int RTFCNDCL CProductShareVariables::SetProductShareVariables(CProductShareVariables *productShareVariables)
{
	m_cProductShareVariables = productShareVariables;
	m_cProductShareVariables->SetPlatformShareVariables(productShareVariables);
	return 0;
}