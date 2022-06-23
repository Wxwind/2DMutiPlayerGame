using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Proto2CSHelper : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="workingDir">运行目录</param>
    /// <param name="cmd">用来运行bat时，如果不使用shell则应使用文件的完整路径</param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    public static Process CreateShellExProcess(string workingDir, string cmd, string args)
    {
        var pStartInfo = new ProcessStartInfo(cmd);
        if (!String.IsNullOrEmpty(args))
        {
            pStartInfo.Arguments = args;
        }
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = false;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;
        pStartInfo.WorkingDirectory = workingDir;
        if (pStartInfo.UseShellExecute)
        {
            pStartInfo.RedirectStandardOutput = false;
            pStartInfo.RedirectStandardError = false;
            pStartInfo.RedirectStandardInput = false;
        }
        else
        {
            pStartInfo.RedirectStandardOutput = true; //获取或设置指示是否将应用程序的错误输出写入 StandardError 流中的值。
            pStartInfo.RedirectStandardError = true; //获取或设置指示是否将应用程序的错误输出写入 StandardError 流中的值。
            pStartInfo.RedirectStandardInput = true; //获取或设置指示应用程序的输入是否从 StandardInput 流中读取的值。
            pStartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            pStartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
        }
        var process = Process.Start(pStartInfo);
        if (pStartInfo.UseShellExecute == false)
        {
            if (process != null)
            {
                string output = "Proto2CS输出信息:" + process.StandardOutput.ReadToEnd();
                Debug.Log(output);
                string outError = process.StandardError.ReadToEnd();
                if (outError == "")
                {
                    Debug.Log("Proto2CS执行成功");
                }
                else Debug.LogError(outError);
            }
            else
            {
                Debug.Log("process is null");
            }
        }
        return process;
    }


    private static void RunBat(string path, string batFileName, string args = "")
    {
        string batFileFullPath = path + '\\' + batFileName;
        if (!File.Exists(batFileFullPath))
        {
            Debug.LogError($"{batFileName}文件不存在此目录中:{path}");
        }
        else
        {
            try
            {
                var p = CreateShellExProcess(path, batFileFullPath, args);
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }
    }

    [MenuItem("ProtoToCS/Build")]
    private static void Run()
    {
        //RunMyBat(Application.dataPath + "/../Proto2CS/","Proto2CS.bat");
        RunBat(System.Environment.CurrentDirectory + "\\Proto2CS", "Proto2CS.bat");
    }
}