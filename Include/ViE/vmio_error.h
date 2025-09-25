#ifndef VMIO_ERR
 #define VMIO_ERR

#define	VMIO_SUCCESS      0x00        //Success with no error

#define	VERR_DRIVER_FAILED       -1   //Driver failed to open
#define	VERR_TIMEOUT             -2   //Function response time out
#define	VERR_NO_BOARD            -3   //No board detected
#define	VERR_INVALID_PARAM       -4   //Invalid parameter been passing to function
#define	VERR_DUPLICATE_BOARD_ID  -5   //Duplicate board ID in the same system (for manual mode only)
#define	VERR_DOUBLE_INITIALED    -6   //The board have already been initialed
#define	VERR_NO_MODULE           -7   //No remote module detected in the system
#define VERR_BOARD_ID_OVERFLOW   -8   //Exceed 16 board per system
#define VERR_SYSTEM				 -9   //System Error
#define VERR_MODULE_COMM	     -10  //Module Communication Error
#define VERR_DRIVER_MEMORY		 -11  //Driver Memory Error
#define VERR_API_NOT_AVAILABLE	 -12  //API cannot operate at this moment
#define VERR_OPERATION_FAILED	 -13  //Operation Failed
#define VERR_BOARD_NOT_INITIALED -14  //Board not yet initialize
#define VERR_API_NOT_SUPPORTED	 -15  //API not supported for the model

#endif	//VMIO_ERR


