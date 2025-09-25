#ifndef VMIO_DEF
  #define VMIO_DEF

// Remote Module type define
#define	DEV_D8IO	0x10
#define	DEV_D16IO	0x13
#define	DEV_D32I	0x16
#define	DEV_D32O	0x17
#define	DEV_D20IO	0x18		// new added for remote configurable IO
#define	DEV_UNKNOWN	0x00

// VMIO Board Model type
#define BOARD_DAQES2GPCI64C	0x0060


// MNET BUS ID
#define VMIO_BUS_IO      0x00
#define VMIO_BUS_MOTION  0x01

// MNET Controller Parameter ID define
#define	VMIO_TRANSFER_RATE	0x1

// MNET Controller Parameter value define (for VMIO_TRANSFER_RATE)
#define	VTR_2_5_MEGA_BIT_PSEC	0x00	// 2.5Mbps
#define	VTR_5_MEGA_BIT_PSEC	0x01	// 5Mbps
#define	VTR_10_MEGA_BIT_PSEC	0x02	// 10Mbps
#define	VTR_20_MEGA_BIT_PSEC	0x03	// 20Mbps -DEFAULT

//--------------- The following section of definition is added for new remote configurable IO ---------------//
#define VMIO_AUTO_BOARD_ID	0x00
#define VMIO_MANUAL_BOARD_ID	0x01

// R20IO IO pins group definition
#define R20IO_ICOM			0x00
#define R20IO_PCOM			0x01
//--------------- End ---------------//

#endif	// VMIO_DEF
