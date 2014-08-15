using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ProcessAsUser {
    class Program {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        static void Main(string[] args){
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener("processAsUser.log"));
            Trace.TraceInformation("Hello");
            if (args.Length!=2)
                throw new ArgumentException( "Args count=" +args.Length+ "Expected args--> ExePath,ExeArgs");

            var registryKey = Registry.LocalMachine.CreateSubKey(@"Software\Xpand\ProcessAsUser");
            if (registryKey != null){
                var userName =(string) registryKey.GetValue("UserName","");
                Trace.TraceInformation("Username="+userName);
                var password = (string) registryKey.GetValue("Password");
                if (!string.IsNullOrEmpty(userName)&&!string.IsNullOrEmpty(password)){
                    ProcessAsUser.Launch(userName,password,args[0],args[1]);
                }
                else{
                    Environment.Exit(200);
                }
            }
            else
                Environment.Exit(200);
            
            }
        
        }
    }


