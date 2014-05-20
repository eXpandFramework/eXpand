using System;

namespace ProcessAsUser {
    class Program {
        static void Main(string[] args) {
            
            if (args.Length!=2)
                throw new ArgumentException( "Args count=" +args.Length+ "Expected args--> ExePath,ExeArgs");
            var userName = GetVariable("ProcessAsUserUserName");
            var password = GetVariable("ProcessAsUserPassword");
            ProcessAsUser.Launch(userName,password,args[2],args[3],Environment.Exit);
        }

        private static string GetVariable(string variableName){
            var variable = Environment.GetEnvironmentVariable(variableName);
            if (variable == null) throw new ArgumentNullException(variableName);
            return variable;
        }
    }
}
