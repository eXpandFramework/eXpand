using System;

namespace ProcessAsUser {
    class Program {
        static void Main(string[] args) {
            
            if (args.Length!=4)
                throw new ArgumentException( "UserName, Passowrd, TestExecutorPath,TestExecutorArgs");
            ProcessAsUser.Launch("Tolis", "4450@p0m", @"C:\eXpandFrameWork\eXpand\Demos\Modules\AuditTrail\AuditTrailTester.Win\bin\EasyTest\TestExecutor.v13.2.exe", @"C:\eXpandFrameWork\eXpand\Demos\Modules\AuditTrail\AuditTrailTester.Module\FunctionalTests\AutoTest.ets");
            ProcessAsUser.Launch(args[0],args[1],args[2],args[3]);
        }
    }
}
