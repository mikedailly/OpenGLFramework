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
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utils
{
    class FileSystem
    {
        // ******************************************************************************
        /// <summary>
        ///     Load a file from the Android package
        /// </summary>
        /// <param name="_name">Name of file to load</param>
        /// <returns>
        ///     Buffer holding the file, or null for error/not found.
        /// </returns>
        // ******************************************************************************
        public static byte[] Load(string _name)
        {
            try
            {
                string filename = _name;    // Path.Combine("Assets", _name);
                byte[] data = System.IO.File.ReadAllBytes(filename);
                return data;
            }
            catch { }
            return null;
        }

        // ******************************************************************************
        /// <summary>
        ///     Load a text file from the Android package
        /// </summary>
        /// <param name="_name">Name of text file to load</param>
        /// <returns>
        ///     string holding the file, or null for error/not found.
        /// </returns>
        // ******************************************************************************
        public static string LoadTextFile(string _name)
        {
            byte[] b = Load(_name);
            if (b == null) return null;

            string t = Encoding.UTF8.GetString(b, 0, b.Length);
            return t;
        }

    }
}
