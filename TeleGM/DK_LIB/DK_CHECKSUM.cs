using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmTelematics
{
    class DK_CHECKSUM
    {
        //OCU 전용 ----------------------------------------------------------------------
        public static ushort CRC_16_INIT = 0x0000;
        public static ushort CRC_16_L_SEED = 0xffff;
        public static int CRC_TAB_SIZE = 256;             /* 2^CRC_TAB_BITS      */
        public static ushort CRC_16_L_POLYNOMIAL = 0x8408;
        public static ushort CRC_16_L_OK = 0x0F47;
        public static ushort CRC_16_L_STEP_SEED = ((ushort)~((ushort)CRC_16_L_SEED));

        public static byte ESC_CHAR = 0x7D;
        public static byte CONTROL_CHAR = 0x7E;
        public static byte MASK_CHAR = 0x20;

        private static ushort[] crc16_table = {
                0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
                0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
                0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
                0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
                0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
                0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
                0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
                0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
                0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
                0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
                0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
                0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
                0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
                0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
                0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
                0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
                0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
                0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
                0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
                0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
                0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
                0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
                0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
                0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
                0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
                0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
                0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
                0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
                0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
                0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
                0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
                0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
                };
        //------------------------------------------------------------------------------

        public DK_CHECKSUM()
        {

        }

        public bool Gen9_CX(byte[] bData, ref byte bCX)
        {
            bCX = 0x00;

            if (bData.Length < 1) return false;

            for(int i = 0; i < bData.Length; i++)
            {
                bCX ^= bData[i];
            }

            return true;
        }

        //gen10 
        public bool Gen10_chksum(byte[] bData, ref byte bHigh, ref byte bLow, bool bSendRecv)
        {
            //bSendRecv 는 보낼때 받을때 체크섬 위치가 다름.

            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (bData.Length < 5) return false;

            int iStx = 0;
            int iEtx = 0;
            if (bSendRecv)
            {
                iStx = 4;
                iEtx = 2;
            }
            else
            {
                iStx = 6;
                iEtx = 3;
            }

            for (int i = iStx; i < bData.Length - iEtx; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }

        //gen9 
        public bool Gen9_chksum(byte[] bData, ref byte bHigh, ref byte bLow, bool bSendRecv)
        {
            //bSendRecv 는 보낼때 받을때 체크섬 위치가 다름.

            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (bData.Length < 5) return false;

            int iStx = 0;
            int iEtx = 0;
            if (bSendRecv)
            {
                iStx = 4;
                iEtx = 2;
            }
            else
            {
                iStx = 4;
                iEtx = 3;
            }

            for (int i = iStx; i < bData.Length - iEtx; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }

        //ccm , nad
        public bool Ccm_chksum(byte[] bData, ref byte bHigh, ref byte bLow, bool bSendRecv)
        {
            //bSendRecv 는 보낼때 받을때 체크섬 위치가 다름.

            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (bData.Length < 5) return false;

            int iStx = 0;
            int iEtx = 0;
            if (bSendRecv)
            {
                iStx = 4;
                iEtx = 2;
            }
            else
            {
                iStx = 6;
                iEtx = 3;
            }

            for (int i = iStx; i < bData.Length - iEtx; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }

        //tcp 
        public bool Tcp_chksum(byte[] bData, ref byte bHigh, ref byte bLow, bool bSendRecv)
        {
            //bSendRecv 는 보낼때 받을때 체크섬 위치가 다름.

            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (bData.Length < 5) return false;

            int iStx = 0;
            int iEtx = 0;
            if (bSendRecv)
            {
                iStx = 4;
                iEtx = 2;
            }
            else
            {
                iStx = 6;
                iEtx = 3;
            }

            for (int i = iStx; i < bData.Length - iEtx; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }

        //gen11 
        public bool Gen11_chksum(byte[] bData, ref byte bHigh, ref byte bLow, bool bSendRecv)
        {
            //bSendRecv 는 보낼때 받을때 체크섬 위치가 다름.

            ushort usSum = 0x0000;
            ushort usCRC0 = 0xFFFF;
            ushort usCRC1 = 0x0000;

            if (bData.Length < 5) return false;

            int iStx = 0;
            int iEtx = 0;
            if (bSendRecv)
            {
                iStx = 4;
                iEtx = 2;
            }
            else
            {
                iStx = 6;
                iEtx = 3;
            }

            for (int i = iStx; i < bData.Length - iEtx; i++)
            {
                usCRC0 ^= (ushort)(bData[i] & 0xFF);

                for (int j = 0; j < 8; j++)
                {
                    usCRC1 = (ushort)(((int)usCRC0 / 2) & 0x7FFF);

                    if ((int)(usCRC0 & 0x01) == 1)
                    {
                        usCRC0 = (ushort)(usCRC1 ^ 0xc659);
                    }
                    else
                    {
                        usCRC0 = usCRC1;
                    }
                }
            }

            usSum = usCRC0;

            bHigh = (byte)(usSum >> 8);
            bLow = (byte)(usSum & 0xFF);

            return true;

        }


        public bool XOR_High_Low(byte[] bData, ref byte bHigh, ref byte bLow)
        {

            short iRet = 0;
            short iShare = 0;
            short iRemainder = 0;

            for (int i = 0; i < bData.Length; i++)
            {
                iRet ^= (short)bData[i];
            }

            iShare = (short)(iRet / 16);
            iRemainder = (short)(iRet - (short)(iShare * 16));

            if (iShare < 10) bHigh = (byte)((byte)iShare + 0x30);
            else bHigh = (byte)((iShare - 10) + 0x41);

            if (iRemainder < 10) bLow = (byte)((byte)iRemainder + 0x30);
            else bLow = (byte)((iRemainder - 10) + 0x41);

            return true;
        }

        //OCU 프로토콜 해독

        public byte[] CRC16_DECODE(byte[] recvData)
        {
            byte[] convertData = new byte[recvData.Length];
            ushort convertLength = 0;

            for (int i = 0; i < recvData.Length; i++)
            {
                if (recvData[i] == ESC_CHAR)
                {
                    convertData[convertLength++] = (byte)(recvData[++i] ^ MASK_CHAR);
                }
                else if (recvData[i] == CONTROL_CHAR)
                {
                    convertData[convertLength++] = recvData[i];

                    break;
                }
                else
                {
                    convertData[convertLength++] = recvData[i];
                }
            }

            byte[] returnData = new byte[convertLength];

            for (int i = 0; i < convertLength; i++)
                returnData[i] = convertData[i];

            return returnData;
        }


        public byte[] CRC16_HDLC(byte[] recvData)
        {
            List<byte> tmpData = new List<byte>();
            tmpData.Clear();

            for (int i = 0; i < recvData.Length; i++)
            {
                if (recvData[i] == ESC_CHAR)
                {
                    tmpData.Add(ESC_CHAR);
                    tmpData.Add((byte)(recvData[i] ^ MASK_CHAR));
                }
                else if (recvData[i] == CONTROL_CHAR)
                {
                    tmpData.Add(ESC_CHAR);
                    tmpData.Add((byte)(recvData[i] ^ MASK_CHAR));

                }
                else
                {
                    tmpData.Add(recvData[i]);
                }
            }

            byte[] returnData = tmpData.ToArray();

            return returnData;
        }


        //OCU 프로토콜 캡슐
        public void CRC16_ENCODE(byte[] buf_ptr, int byte_len, ref byte bHigh, ref byte bLow)
        {
            ushort crc_16 = 0xffff;
            ushort fcsval = 0xffff;
            ushort share = 0x0000;
            ushort remainder = 0x0000;

            for (int i = 0; i < byte_len; ++i)
            {
                //crc_16 = (ushort)(crc16_table[(crc_16 ^ buf_ptr[i]) & 0x00ff] ^ (crc_16 >> 8));
                crc_16 = (ushort)(crc16_table[(crc_16 ^ buf_ptr[i]) & 0x00ff] ^ (crc_16 >> 8));
            }

            fcsval = (ushort)(~crc_16);

            if (fcsval > 0x00FF)
            {
                share = (ushort)(fcsval / 0x0100);
                remainder = (ushort)(fcsval % 0x0100);
            }
            else
            {
                share = (ushort)0x00;
                remainder = (ushort)fcsval;
            }

            bHigh = (byte)share;
            bLow = (byte)remainder;

        }


        public ushort crc_16_l_calc(byte[] buf_ptr, ushort len)
        {
            ushort data = 0x0000, crc_16 = 0xffff;

            //ASSERT(buf_ptr != NULL);

            /* Generate a CRC-16 by looking up the transformation in a table and
            ** XOR-ing it into the CRC, one byte at a time.
            */
            int i = 0;

            for (crc_16 = CRC_16_L_SEED; len >= 8; len -= 8, i++)
            {
                //crc_16 = crc_16_l_table[(crc_16 ^ buf_ptr[i]) & 0x00ff] ^ (crc_16 >> 8);
                crc_16 = (ushort)(crc16_table[(crc_16 ^ buf_ptr[i]) & 0x00ff] ^ (crc_16 >> 8));
            }

            /* Finish calculating the CRC over the trailing data bits
            **
            ** XOR the MS bit of data with the MS bit of the CRC.
            ** Shift the CRC and data left 1 bit.
            ** If the XOR result is 1, XOR the generating polynomial in with the CRC.
            */
            if (len != 0)
            {
                data = (ushort)(((ushort)(buf_ptr[i])) << (16 - 8)); /* Align data MSB with CRC MSB */

                while (len-- != 0)
                {
                    if (((crc_16 ^ data) & 0x01) != 0)
                    {   /* Is LSB of XOR a 1 */

                        crc_16 >>= 1;                   /* Right shift CRC         */
                        crc_16 ^= CRC_16_L_POLYNOMIAL;  /* XOR polynomial into CRC */

                    }
                    else
                    {

                        crc_16 >>= 1;                   /* Right shift CRC         */

                    }

                    data >>= 1;                       /* Right shift data        */
                }
            }

            return (ushort)(~crc_16);            /* return the 1's complement of the CRC */
        }





    }
}
