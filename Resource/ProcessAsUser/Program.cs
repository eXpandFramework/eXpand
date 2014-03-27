using System;

namespace ProcessAsUser {
    class Program {
        static void Main(string[] args) {
            
            if (args.Length!=4)
                throw new ArgumentException( "Args count=" +args.Length+ "Expected args--> UserName, Passowrd, TestExecutorPath,TestExecutorArgs");
            ProcessAsUser.Launch(args[0],args[1],args[2],args[3]);
        }
    }
}
