using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PyroPatchViewer
{
    internal class ByteConversion
    {
        /// <summary>
        /// Convert byte to a visible text char.
        /// </summary>
        /// <remarks>
        /// Code from : https://github.com/pleonex/tinke/blob/master/Be.Windows.Forms.HexBox/ByteCharConverters.cs
        /// </remarks>
        public static char ByteToChar(byte value)
        {
            return value > 0x1F && !(value > 0x7E && value < 0xA0) ? (char)value : '.';
        }

        /// <summary>
        /// Convert bytes to a visible text string.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>String showing the visible Ascii characters.</returns>
        public static string BytesToString(byte[] bytes)
        {
            char[] charArray = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                charArray[i] = ByteToChar(bytes[i]);
            }
            return new string(charArray);

        }

        /// <summary>
        /// Converts 0-15 into 0-F. From Microsoft.
        /// </summary>
        /// <param name="i">Value from 0-15.</param>
        /// <returns>Char of 0-F.</returns>
        public static char GetHexChar(int value)
        {
            if (value < 0 || value >= 16)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (value < 10)
            {
                return (char)(value + '0');
            }
            return (char)(value - 10 + 'A');
        }

        /// <summary>
        /// Convert a byte into a two-character hex string.
        /// </summary>
        /// <param name="singleByte">Byte to convert (value 0-255).</param>
        /// <returns>A two-character hex string.</returns>
        public static string ByteToHexString(byte singleByte)
        {
            //char[] hexChars = "0123456789ABCDEF".ToCharArray();
            //char[] output = new char[2];
            //output[0] = hexChars[singleByte / 16];
            //output[1] = hexChars[singleByte % 16];
            //return new string(new char[] { hexChars[singleByte / 16], hexChars[singleByte % 16] });
            //return BitConverter.ToString(new byte[] { singleByte });
            char[] charArray = new char[2];
            charArray[0] = GetHexChar(singleByte / 16);
            charArray[1] = GetHexChar(singleByte % 16);
            return new string(charArray);
        }

        /// <summary>
        /// Convert a byte array into a string of hex values. From Microsoft.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <param name="separationChar">Char which separates each hex byte (such as "FF").</param>
        /// <returns>String of hex values separated by separationChar.</returns>
        public static string BytesToHexString(byte[] bytes, char? separationChar = ' ')
        {
            return BytesToHexString(bytes, 0, bytes.Length, separationChar);
        }

        /// <summary>
        /// Convert a byte array into a string of hex values. From Microsoft.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <param name="startIndex">Index of the first byte to convert.</param>
        /// <param name="length">Number of bytes to convert.</param>
        /// <param name="separationChar">Char which separates each hex byte (such as "FF").</param>
        /// <returns>String of hex values separated by separationChar representing the byte array.</returns>
        public static string BytesToHexString(byte[] bytes, int startIndex, int length, char? separationChar = ' ')
        {
            //return BitConverter.ToString(bytes).Replace('-', separationChar);
            //This code is the same as what powers the BitConverter, except with less error checking.
            int charIncrement = separationChar == null ? 2 : 3; //if there is no separation char, only use two chars per byte rather than three
            int charArrayLength = length * charIncrement;
            char[] charArray = new char[charArrayLength];
            int index = startIndex;
            for (int i = 0; i < charArrayLength; i += charIncrement)
            {
                byte b = bytes[index++];
                charArray[i] = GetHexChar(b / 16);
                charArray[i + 1] = GetHexChar(b % 16);
                if (separationChar != null)
                    charArray[i + 2] = (char)separationChar;
            }
            return new string(charArray, 0, charArray.Length - (separationChar == null ? 0 : 1)); //remove the last separation char, if one exists
        }

        /// <summary>
        /// Convert an integer into a string of hex vaules.
        /// </summary>
        /// <param name="value">Integer to convert.</param>
        /// <param name="separationChar">Char which separates each hex byte (such as "FF").</param>
        /// <returns>String of hex values separated by separationChar representing the integer.</returns>
        public static string IntToHexString(int value, int length = 4, char? separationChar = null)
        {
            return BytesToHexString(IntToBytes(value), 0, length, separationChar);
        }

        /// <summary>
        /// Convert up to 4 bytes into an integer.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="useLittleEndianFormat">Little Endian means a number like 256 is stored in hex as x0010, with the most significant byte on the right.</param>
        /// <returns></returns>
        public static int BytesToInt(byte[] bytes, bool useLittleEndianFormat = false)
        {
            if (BitConverter.IsLittleEndian != useLittleEndianFormat) { Array.Reverse(bytes); }
            return BitConverter.ToInt32(bytes.ToArray());
        }

        /// <summary>
        /// Convert a single byte into an integer.
        /// </summary>
        /// <param name="singleByte"></param>
        /// <returns></returns>
        public static int ByteToInt(byte singleByte)
        {
            return singleByte;
        }

        /// <summary>
        /// Convert up to 8 bytes into a long integer.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="useLittleEndianFormat">Little Endian means a number like 256 is stored in hex as x0010, with the most significant byte on the right.</param>
        /// <returns></returns>
        public static long BytesToLong(byte[] bytes, bool useLittleEndianFormat = false)
        {
            if (BitConverter.IsLittleEndian != useLittleEndianFormat) { Array.Reverse(bytes); }
            return BitConverter.ToInt64(bytes.ToArray());
        }

        /// <summary>
        /// Convert an integer into up to 4 bytes.
        /// </summary>
        /// <param name="integer">Integer to convert.</param>
        /// <param name="length">Number of bytes to output.</param>
        /// <param name="useLittleEndianFormat">Little Endian means a number like 256 is stored in hex as x0010, with the most significant byte on the right.</param>
        /// <returns></returns>
        public static byte[] IntToBytes(int integer, int length = 4, bool useLittleEndianFormat = false)
        {
            byte[] bytes = BitConverter.GetBytes(integer);
            return HandleEndianFormat(bytes, length, useLittleEndianFormat);
        }

        /// <summary>
        /// Convert a long integer into up to 8 bytes.
        /// </summary>
        /// <param name="integer">Long integer to convert.</param>
        /// <param name="length">Number of bytes to output.</param>
        /// <param name="useLittleEndianFormat">Little Endian means a number like 256 is stored in hex as x0010, with the most significant byte on the right.</param>
        /// <returns></returns>
        public static byte[] LongToBytes(long longInteger, int length = 8, bool useLittleEndianFormat = false)
        {
            byte[] bytes = BitConverter.GetBytes(longInteger);
            return HandleEndianFormat(bytes, length, useLittleEndianFormat);
        }


        //--------------------------------------------------------------
        // Helper Functions
        //--------------------------------------------------------------

        /// <summary>
        /// Adjust the ordering and number of bytes based upon the Endian Format.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length">Number of bytes to return.</param>
        /// <param name="useLittleEndianFormat">Little Endian means a number like 256 is stored in hex as x0010, with the most significant byte on the right.</param>
        /// <returns></returns>
        private static byte[] HandleEndianFormat(byte[] bytes, int length, bool useLittleEndianFormat)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (useLittleEndianFormat)
                {
                    return bytes.Take(length).ToArray();
                }
                else
                {
                    return bytes.Take(length).Reverse().ToArray();
                }
            }
            else
            {
                if (useLittleEndianFormat)
                {
                    return bytes.TakeLast(length).Reverse().ToArray();
                }
                else
                {
                    return bytes.TakeLast(length).ToArray();
                }
            }
        }
    }
}
