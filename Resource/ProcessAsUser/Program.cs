using System;
using Microsoft.Win32;

namespace ProcessAsUser {
    class Program {
        static void Main(string[] args) {
            if (args.Length!=2)
                throw new ArgumentException( "Args count=" +args.Length+ "Expected args--> ExePath,ExeArgs");
                
            var registryKey = Registry.LocalMachine.CreateSubKey(@"Software\Xpand\ProcessAsUser");
            if (registryKey != null){
                var userName =(string) registryKey.GetValue("UserName","");
                var password = (string) registryKey.GetValue("Password");
                if (!string.IsNullOrEmpty(userName)&&!string.IsNullOrEmpty(password)){
                    ProcessAsUser.Launch(userName,password,args[0],args[1],Environment.Exit);
                }
                else{
                    Environment.Exit(255);
                }
            }
            else
                Environment.Exit(255);
            }
        }
    }


