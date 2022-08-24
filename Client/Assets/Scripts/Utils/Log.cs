using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace WX
{
    static class Log
    {
        internal enum LogLevel
        {
            Error = 0,
            Warning = 1,
            Info = 2
        }

        private static readonly string m_logPath = Environment.CurrentDirectory + "/Log/";
        private static readonly string m_logFilename = m_logPath + "Log.txt";
        private static bool m_isOutputToFile = false;

        private static LogLevel m_logLevel = LogLevel.Info;

        public static void SetLevel(LogLevel level)
        {
            m_logLevel = level;
        }

        public static void LogInfo(string msg)
        {
            if (m_logLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.Log(msg);
                if (m_isOutputToFile)
                {
                    OutPutToFile(msg);
                }
            }
        }

        public static void LogWarning(string msg)
        {
            if (m_logLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(msg);
                if (m_isOutputToFile)
                {
                    OutPutToFile(msg);
                }
            }
        }

        public static void LogError(string msg)
        {
            if (m_logLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.LogError(msg);
                if (m_isOutputToFile)
                {
                    OutPutToFile(msg);
                }
            }
        }

        private static void OutPutToFile(string msg)
        {
            var nowTime = GetRealTime();
            var info = GetRunInfo();

            var path = Environment.CurrentDirectory + "/Log/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filename = path + "Log.txt";
            var fs = File.Open(filename, FileMode.Append, FileAccess.Write);
            var sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(nowTime);
            sw.WriteLine(msg);
            foreach (var t in info)
            {
                sw.WriteLine(t);
            }

            sw.Close();
            fs.Close();
        }

        public static void LogExpection(Exception e)
        {
            var t = e.ToString();
            UnityEngine.Debug.LogException(e);
            if (m_isOutputToFile)
            {
                var fs = File.Open(m_logFilename, FileMode.Append, FileAccess.Write);
                var sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(t);
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private static string[] GetRunInfo()
        {
            StackTrace st = new StackTrace(3, true);
            var frames = st.GetFrames();
            var info = new string[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                var sf = frames[i];
                var line = sf.GetFileLineNumber();
                var filename = sf.GetFileName();
                var funcName = sf.GetMethod().DeclaringType.FullName;
                info[i] = $"  at {funcName} in {filename}:line {line}";
            }

            return info;
        }

        private static string GetRealTime()
        {
            return DateTime.Now.ToString("yyyy-M-d HH:mm:ss");
        }
    }
}