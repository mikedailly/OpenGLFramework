// **********************************************************************************************************************
// 
// Copyright (c)2020, Ogre Games Ltd. All Rights reserved.
// 
// Date				Version		Comment
// ----------------------------------------------------------------------------------------------------------------------
// 26/12/2020		V1.0        1st version
// 
// **********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    // Untested

    public class WinNative
    {


        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public bool CheckOpenAL()
        {
            IntPtr p = LoadLibrary("openal32.dll");
            if (p == IntPtr.Zero){
                return false;
            }
            return true;
        }


        [DllImport("Advapi32.dll", EntryPoint = "RegOpenKeyExW", CharSet = CharSet.Unicode)]
        static extern int RegOpenKeyEx(IntPtr hKey, [In] string lpSubKey, int ulOptions, int samDesired, out IntPtr phkResult);
        [DllImport("Advapi32.dll", EntryPoint = "RegQueryValueExW", CharSet = CharSet.Unicode)]
        static extern int RegQueryValueEx(IntPtr hKey, [In] string lpValueName, IntPtr lpReserved, out int lpType, [Out] byte[] lpData, ref int lpcbData);
        [DllImport("advapi32.dll")]
        static extern int RegCloseKey(IntPtr hKey);

        static public readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(-2147483648);
        static public readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
        static public readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
        static public readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);
        static public readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
        static public readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
        static public readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);

        public const int KEY_READ = 0x20019;
        public const int KEY_WRITE = 0x20006;
        public const int KEY_QUERY_VALUE = 0x0001;
        public const int KEY_SET_VALUE = 0x0002;
        public const int KEY_WOW64_64KEY = 0x0100;
        public const int KEY_WOW64_32KEY = 0x0200;

        public const int REG_NONE = 0;
        public const int REG_SZ = 1;
        public const int REG_EXPAND_SZ = 2;
        public const int REG_BINARY = 3;
        public const int REG_DWORD = 4;
        public const int REG_DWORD_BIG_ENDIAN = 5;
        public const int REG_LINK = 6;
        public const int REG_MULTI_SZ = 7;
        public const int REG_RESOURCE_LIST = 8;
        public const int REG_FULL_RESOURCE_DESCRIPTOR = 9;
        public const int REG_RESOURCE_REQUIREMENTS_LIST = 10;
        public const int REG_QWORD = 11;

        static void RegTest(string[] args)
        {
            IntPtr key;
            int error;
            if ((error = RegOpenKeyEx(HKEY_CURRENT_USER, @"Software\Test", 0, KEY_READ | KEY_WOW64_32KEY, out key)) != 0)
            {
                return;
                //throw new Win32Exception(error);
            }

            try
            {
                Console.WriteLine(RegQueryValue(key, "String"));
                Console.WriteLine(RegQueryValue(key, "ExpString"));
                foreach (string str in (string[])RegQueryValue(key, "MultiSz"))
                    Console.WriteLine(str);
                Console.WriteLine(RegQueryValue(key, "Dword"));
                Console.WriteLine(RegQueryValue(key, "Qword"));
                Console.WriteLine(RegQueryValue(key, "NoValue", "'NoValue' doesn't exist"));
            }
            finally
            {
                RegCloseKey(key);
            }
        }

        static object RegQueryValue(IntPtr key, string value)
        {
            return RegQueryValue(key, value, null);
        }

        static object RegQueryValue(IntPtr key, string value, object defaultValue)
        {
            int error, type = 0, dataLength = 0xfde8;
            int returnLength = dataLength;
            byte[] data = new byte[dataLength];
            while ((error = RegQueryValueEx(key, value, IntPtr.Zero, out type, data, ref returnLength)) == 0xea)
            {
                dataLength *= 2;
                returnLength = dataLength;
                data = new byte[dataLength];
            }
            if (error == 2)
                return defaultValue; // value doesn't exist
            if (error != 0)
            {
                return -1;
                //throw new Win32Exception(error);
            }

            switch (type)
            {
                case REG_NONE:
                case REG_BINARY:
                    return data;
                case REG_DWORD:
                    return (((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                case REG_DWORD_BIG_ENDIAN:
                    return (((data[3] | (data[2] << 8)) | (data[1] << 16)) | (data[0] << 24));
                case REG_QWORD:
                    {
                        uint numLow = (uint)(((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                        uint numHigh = (uint)(((data[4] | (data[5] << 8)) | (data[6] << 16)) | (data[7] << 24));
                        return (long)(((ulong)numHigh << 32) | (ulong)numLow);
                    }
                case REG_SZ:
                    return Encoding.Unicode.GetString(data, 0, returnLength);
                case REG_EXPAND_SZ:
                    return Environment.ExpandEnvironmentVariables(Encoding.Unicode.GetString(data, 0, returnLength));
                case REG_MULTI_SZ:
                    {
                        var strings = new List<string>();
                        string packed = Encoding.Unicode.GetString(data, 0, returnLength);
                        int start = 0;
                        int end = packed.IndexOf('\0', start);
                        while (end > start)
                        {
                            strings.Add(packed.Substring(start, end - start));
                            start = end + 1;
                            end = packed.IndexOf('\0', start);
                        }
                        return strings.ToArray();
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        }
}
