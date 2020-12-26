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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework.Utils
{
    public class Log
    {
        public static string FullLogFileName = "";
        public static string LogFileName = "raycast_editor.log";
        public static string LogTitle = "Raycast Editor logfile";

        public static int LogFileSize = 1024 * 64;
        public static StringBuilder LogFile;


        // *********************************************************************
        /// <summary>
        ///     Initialise the log
        /// </summary>
        // *********************************************************************
        private static void InitLog()
        {
            if (LogFile == null)
            {
                DateTime d = DateTime.Now;
                LogFile = new StringBuilder( LogFileSize );
                LogFile.AppendLine("************************* " + LogTitle + " "+ d.ToString()+" *************************");
                LogFile.AppendLine();
            }
        }

        // *********************************************************************
        /// <summary>
        ///     Flush buffer
        /// </summary>
        // *********************************************************************
        public static void Flush()
        {
            if (LogFile == null) return;

            if (FullLogFileName == ""){
                FullLogFileName = Path.Combine(Environment.CurrentDirectory, LogFileName);
            }


            StreamWriter f;
            if (!File.Exists(FullLogFileName))
            {
                f = File.CreateText(FullLogFileName);
                f.WriteLine();
                f.Close();
            }

            // flush buffer to disk
            string s = LogFile.ToString();
            f = File.AppendText(FullLogFileName);
            f.WriteLine(s);
            f.Close();
            LogFile.Clear();
        }


        // *********************************************************************
        /// <summary>
        ///     Writeln text to the logfile
        /// </summary>
        /// <param name="_text">text to save</param>
        /// <param name="_args">args</param>
        // *********************************************************************
        public static void Write(string _text, params object[] _args)
        {
            string s = string.Format(_text, _args);

            InitLog();
            if ((LogFile.Length + s.Length + 10) > LogFileSize) Flush();
            LogFile.Append(s);
            Debug.Write(s);
            Console.Write(s);
        }



        // *********************************************************************
        /// <summary>
        ///     Writeln text to the logfile
        /// </summary>
        /// <param name="_text">text to save</param>
        /// <param name="_args">args</param>
        // *********************************************************************
        public static void WriteLine(string _text, params object[] _args)
        {
            string s = string.Format(_text, _args);

            InitLog();
            if ((LogFile.Length + s.Length + 10) > LogFileSize) Flush();
            LogFile.AppendLine(s);
            Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }
}
