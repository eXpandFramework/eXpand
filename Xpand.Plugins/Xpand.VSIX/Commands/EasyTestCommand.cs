using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands{
    public class LastBuildStatusArgs : EventArgs {
        public bool Succeeded { get; set; }
    }

    public class EasyTestCommand:VSCommand {
        private EasyTestCommand(EventHandler invokeHandler, CommandID commandID,string name) : base(invokeHandler, commandID){
            this.EnableForDXSolution().EnableForActiveFile(".ets", ".inc");
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            var unused = new EasyTestCommand((sender, args) => RunTest(false),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidRunEasyTest),$"Run{nameof(EasyTestCommand)}");
            // ReSharper disable once ObjectCreationAsStatement
            new EasyTestCommand((sender, args) => RunTest(true),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidDebugEasyTest),$"Debug{nameof(EasyTestCommand)}");
        }

        private static readonly DTE2 DTE=DteExtensions.DTE;

        public static event EventHandler<LastBuildStatusArgs> QueryLastBuildStatus;

        public static void RunTest(bool debug) {
            DTE.InitOutputCalls("RunTest");
            Task.Factory.StartNew(() => RunTestCore(debug),CancellationToken.None,TaskCreationOptions.None,TaskScheduler.Current);
        }

        private static void RunTestCore(bool debug) {
            var lastBuildStatusArgs = new LastBuildStatusArgs();
            OnQueryLastBuildStatus(lastBuildStatusArgs);
            try {
                DTE.WriteToOutput("Building EasyTest/Debug Configuration");
                if (DTE.Solution.BuildSolution()) {
                    var activeFileName = DTE.ActiveDocument.FullName;
                    var testLogPath = Path.Combine(Path.GetDirectoryName(activeFileName) + "", "Testslog.xml");
                    if (File.Exists(testLogPath))
                        File.Delete(testLogPath);
                    var debugSwitch = WriteToOutput(debug, activeFileName);

                    var testExecutorPath = GetTestExecutorPath();
                    if (!File.Exists(testExecutorPath))
                        throw new FileNotFoundException(
                            "Use plugin options to assign a valid path for the standalond TestExecutor. Or leave it blank for auto detection.");
                    var processStartInfo = new ProcessStartInfo(testExecutorPath) {
                        Arguments = $@"""{activeFileName}""{debugSwitch}",
                        UseShellExecute = debug,
                        RedirectStandardOutput = !debug,
                        CreateNoWindow = !debug
                    };

                    var process = System.Diagnostics.Process.Start(processStartInfo);
                    Debug.Assert(process != null, "process != null");
                    process.WaitForExit();
                    if (File.Exists(testLogPath)) {
                        var document = XDocument.Load(File.OpenRead(testLogPath));
                        var errorElement =
                            document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Error");
                        if (errorElement != null) {
                            var messageElement = errorElement.Descendants("Message").First();
                            DTE.WriteToOutput(messageElement.Value);
                        }
                        else
                            DTE.WriteToOutput("EasyTest Passed!");
                    }
                }
                else {
                    DTE.WriteToOutput("EasyTest build failed");
                }
            }
            catch (Exception e) {
                DTE.WriteToOutput(e.ToString());
            }
        }

        private static string WriteToOutput(bool debug,  string activeFileName){
            string debugSwitch = null;
            if (debug){
                debugSwitch = " -d:" + ((TextSelection) DTE.ActiveDocument.Selection).CurrentLine;
                DTE.WriteToOutput("Debug will start at line " + ((TextSelection) DTE.ActiveDocument.Selection).CurrentLine);
            }
            else{
                DTE.WriteToOutput("EasyTesting " + activeFileName);
            }
            return debugSwitch;
        }

        private static string GetTestExecutorPath() {
            
            var testExecutorPath = OptionClass.Instance.TestExecutorPath;
            if (string.IsNullOrWhiteSpace(testExecutorPath)) {
                var version = DTE.Solution.GetDXVersion();
                var dxRootDirectory = DTE.Solution.GetDXRootDirectory();
                return Path.Combine(dxRootDirectory + @"\Tools\eXpressAppFramework\EasyTest", "TestExecutor." + version + ".exe");
            }
            return testExecutorPath;
        }

        protected static void OnQueryLastBuildStatus(LastBuildStatusArgs e){
            QueryLastBuildStatus?.Invoke(null, e);
        }
    }
}
