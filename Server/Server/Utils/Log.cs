using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WX.Utils;

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

    private static LogLevel m_logLevel = LogLevel.Info;
    private static bool m_isOutputToFile;

    public static void SetLevel(LogLevel level)
    {
        m_logLevel = level;
    }

    public static void SetOuputToFile(bool isOutputToFile)
    {
        m_isOutputToFile = isOutputToFile;
    }

    public static void LogInfo(string msg)
    {
        if (m_logLevel >= LogLevel.Info)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            LogMsg(msg, m_isOutputToFile);
        }
    }

    public static void LogWarning(string msg)
    {
        if (m_logLevel >= LogLevel.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            LogMsg(msg, m_isOutputToFile);
        }
    }

    public static void LogError(string msg)
    {
        if (m_logLevel >= LogLevel.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            LogMsg(msg, m_isOutputToFile);
        }
    }

    private static void LogMsg(string msg, bool isLogDetail)
    {
        var nowTime = GetRealTime();
        if (isLogDetail)
        {
            Console.WriteLine(nowTime);
        }

        Console.WriteLine(msg);
        var info = GetRunInfo();
        if (isLogDetail)
        {
            foreach (var t in info)
            {
                Console.WriteLine(t);
            }
        }

        if (m_isOutputToFile)
        {
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
    }

    public static void LogExpection(Exception e)
    {
        var t = e.ToString();
        Console.WriteLine(t);
        var fs = File.Open(m_logFilename, FileMode.Append, FileAccess.Write);
        var sw = new StreamWriter(fs, Encoding.UTF8);
        sw.WriteLine(t);
        sw.Flush();
        fs.Flush();
        sw.Close();
        fs.Close();
    }

    private static string[] GetRunInfo()
    {
        StackTrace st = new(3, true);
        var frames = st.GetFrames();
        var info = new string[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            var sf = frames[i];
            var line = sf.GetFileLineNumber();
            var filename = sf.GetFileName();
            var funcName = sf.GetMethod()?.DeclaringType?.FullName;
            info[i] = $"  at {funcName} in {filename}:line {line}";
        }

        return info;
    }

    private static string GetRealTime()
    {
        return DateTime.Now.ToString("yyyy-M-d HH:mm:ss");
    }
}