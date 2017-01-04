using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EH.CRC
{
    class CRC_DSP
    {
        private const UInt16 CRC16_POLY = 0x1021;		// CCITT polynomial: x^16 + x^12 + x^5 + 1
        private const UInt16 CRC16_START = 0x55aa;		// start value for shift-register
        private static UInt16 CRC16_shift;

        public static void CRC16_init()											// clear shift-register
        {
            CRC16_shift = CRC16_START;
        }


        public static UInt16 CRC16_nextData(UInt16 data)									// calc CRC of a data-stream
        {
            UInt16 i;
            for (i = 0; i < 16; i++)
            {
                if ((CRC16_shift & 0x8000) != (data & 0x8000))		// compare MSBs
                    CRC16_shift = (UInt16)((UInt16)(CRC16_shift << 1) ^ CRC16_POLY);		// if not equal: shift and XOR
                else
                    CRC16_shift = (UInt16)(CRC16_shift << 1);					// if equal: shift 
                data = (UInt16)(data << 1);										// next data-bit -> MSB
            }
            return CRC16_shift;
        }



        public static UInt16 CRC16_block(UInt16[] data, UInt32 count, UInt32 offset)				// calc CRC of data-block
        {
            UInt32 i;

            CRC16_init();
            for (i = 0; i < count; i++) CRC16_nextData(data[i + offset]);
            return CRC16_shift;
        }
    }
}
