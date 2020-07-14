﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace epi.videoProcessor.crestron.wp401
{

    public static class XSigHelper
    {
        /// <summary>
        /// Sends the xSig Clear Byte
        /// </summary>
        /// <returns></returns>
        public static string ClearData()
        {
            var array = new byte[1];
            array[0] = 0xFC;

            return Encoding.GetEncoding(28591).GetString(array, 0, 1);
        }

        /// <summary>
        /// Converts a string to a xSig formatted string of the correct encoding type
        /// </summary>
        /// <param name="index">The location on the xSig where you want to value to occur.  Starts at 0.</param>
        /// <param name="value">The value to be sent to the xSig</param>
        /// <returns>string to pass to s+</returns>
        public static string CreateByteString(int index, string value)
        {
            value = !string.IsNullOrEmpty(value) ? value : "";
            var xSigStr = "";
            const string indexHeader = "11001";
            const int maxIndex = 1024;
            const int toBase = 2;
            const byte delimiter = 255;

            if (index >= maxIndex) return xSigStr;
            var indexBinary = (indexHeader + Convert.ToString(index, toBase).PadLeft(10, '0')).Insert(8, "0");

            Console.WriteLine("index binary = {0}", indexBinary);

            var array01 = (BitConverter.GetBytes(Convert.ToInt16(indexBinary, 2))).Reverse().ToArray();
            var array02 = Encoding.GetEncoding(28591).GetBytes(value);
            byte[] array03 = { delimiter };

            var dataArray = array01.Concat(array02).Concat(array03).ToArray();

            foreach (var item in dataArray)
            {
                Console.WriteLine("{0:X2}", item);
            }

            xSigStr = Encoding.GetEncoding(28591).GetString(dataArray, 0, dataArray.Length);

            return xSigStr;
        }

        /// <summary>
        /// Converts a string to a xSig formatted string of the correct encoding type
        /// </summary>
        /// <param name="index">The location on the xSig where you want to value to occur.  Starts at 0.</param>
        /// <param name="value">The value to be sent to the xSig</param>
        /// <returns>string to pass to s+</returns>
        public static string CreateByteString(int index, bool value)
        {
            var xSigStr = "";
            const int toBase = 2;
            const int maxIndex = 4096;

            if (index >= maxIndex) return xSigStr;
            var indexBinary = (Convert.ToString(index, toBase).PadLeft(12, '0')).Insert(5, "0");

            var myData = value ? 0 : 32; //Sets the bit for digital - 0 == high and 1 == low
            myData += 128;
            myData = myData << 8;
            myData += (Convert.ToInt16(indexBinary, 2));

            byte[] dataArray = { (byte)((myData & 0xFF00) >> 8), (byte)(myData & 0x00FF) };

            xSigStr = Encoding.GetEncoding(28591).GetString(dataArray, 0, dataArray.Length);

            return xSigStr;
        }

        /// <summary>
        /// Converts a string to a xSig formatted string of the correct encoding type
        /// </summary>
        /// <param name="index">The location on the xSig where you want to value to occur.  Starts at 0.</param>
        /// <param name="value">The value to be sent to the xSig</param>
        /// <returns>string to pass to s+</returns>
        public static string CreateByteString(int index, int value)
        {
            var xSigStr = "";
            const int toBase = 2;
            const int maxIndex = 1024;

            if (index >= maxIndex) return xSigStr;
            var binaryValue = ((Convert.ToString(value, toBase).PadLeft(16, '0')).Insert(2, "00")).Insert(11, "0");
            var binaryIndex = (Convert.ToString(index, toBase).PadLeft(10, '0')).Insert(3, "0");

            var dataPacket = ("11" + binaryValue.Insert(3, binaryIndex));

            var dataArray = BitConverter.GetBytes(Convert.ToInt32(dataPacket, 2));



            xSigStr = Encoding.GetEncoding(28591).GetString(dataArray.Reverse().ToArray(), 0, dataArray.Length);

            return xSigStr;
        }

        /// <summary>
        /// Converts xSig data to a type usable by C#
        /// </summary>
        /// <param name="data">Data from an xSig symbol in Simpl</param>
        /// <returns>An object of type XSigData that contains data about the xSig bytes</returns>
        public static XSigData ReadXsigBytes(string data)
        {
            var myReturn = new XSigData();

            const int stringByte = 200;
            const int intByte = 192;
            const int boolByte = 128;

            var dataArray = Encoding.GetEncoding(28591).GetBytes(data);

            foreach (var item in dataArray)
            {
                Console.WriteLine("{0:X2}", item);
            }

            var headerByte = (int)dataArray[0];

            //is String
            if ((headerByte & stringByte) == stringByte)
            {
                myReturn.SigType = eSigType.SigString;


                var index = (dataArray[0] << 8) + dataArray[1];


                var myIndex = Convert.ToString(index, 2).PadLeft(16, '0');
                myIndex = myIndex.Remove(0, 5).Remove(3, 1);
                myReturn.Index = Convert.ToInt16(myIndex, 2);

                myReturn.XString = dataArray.Length > 3 ? Encoding.GetEncoding(28591).GetString(dataArray, 2, dataArray.Length - 3) : "";
            }

                //is Analog
            else if ((headerByte & intByte) == intByte)
            {
                if (dataArray.Length != 4) return myReturn;
                myReturn.SigType = eSigType.SigInt;

                var packetInfo = dataArray.Select((t, i) => (int)(t << (24 - (i * 8)))).Sum();

                var index = (dataArray[0] << 8) + dataArray[1];

                var supplement = (dataArray[2] << 8) + dataArray[3];

                var myIndex = Convert.ToString(index, 2).PadLeft(16, '0');
                var newIndex = myIndex.Remove(0, 5).Remove(3, 1);
                myReturn.Index = Convert.ToInt16(newIndex, 2);

                var mySupplement = Convert.ToString(supplement, 2).PadLeft(16, '0');

                var fullPacket = myIndex + mySupplement;
                var myAnalog = fullPacket.Remove(0, 2).Remove(2, 13).Remove(9, 1);

                myReturn.XInt = Convert.ToInt16(myAnalog, 2);
            }

                //is Digital
            else if ((headerByte & boolByte) == boolByte)
            {
                if (dataArray.Length != 2) return myReturn;
                myReturn.SigType = eSigType.SigBool;
                var index = 0;

                for (var i = 0; i < 2; i++)
                {
                    index += (int)(dataArray[i] << (8 - (i * 8)));
                }

                var myIndex = Convert.ToString(index, 2).PadLeft(16, '0');
                var myData = myIndex[2];
                myIndex = myIndex.Remove(0, 3).Remove(5, 1);
                myReturn.Index = Convert.ToInt32(myIndex, 2);

                myReturn.XBool = myData == '0' ? true : false;
            }
            else
                myReturn.SigType = eSigType.SigNone;

            return myReturn;

        }



    }

    public class XSigData
    {

        public int Index;
        public string XString;
        public int XInt;
        public bool XBool;
        public eSigType SigType;

        public XSigData()
        {
            this.SigType = eSigType.SigNone;
        }

    }

    public enum eSigType
    {
        SigString,
        SigInt,
        SigBool,
        SigNone
    };
}
