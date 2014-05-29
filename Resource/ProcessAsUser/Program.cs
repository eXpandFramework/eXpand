using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace ProcessAsUser {
    class Program {
        static void Main(string[] args){
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


