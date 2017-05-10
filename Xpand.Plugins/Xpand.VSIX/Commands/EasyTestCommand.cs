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
        public bool Successed { get; set; }
    }

    public class EasyTestCommand:VSCommand {
        private EasyTestCommand(EventHandler invokeHandler, CommandID commandID) : base(invokeHandler, commandID){
            this.EnableForDXSolution().EnableForActiveFile(".ets", ".inc");
        }

        public static void Init(){
            var runEasyTest = new EasyTestCommand((sender, args) => RunTest(false),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidRunEasyTest));
            runEasyTest.BindCommand("Text Editor::Alt+T");
            var debugEasyTest = new EasyTestCommand((sender, args) => RunTest(true),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidDebugEasyTest));
            debugEasyTest.BindCommand("Text Editor::Alt+D");
        }

        private static readonly DTE2 _dte=DteExtensions.DTE;

        public static event EventHandler<LastBuildStatusArgs> QueryLastBuildStatus;

        public static void RunTest(bool debug) {
            _dte.InitOutputCalls("RunTest");
            Task.Factory.StartNew(() => RunTestCore(debug),CancellationToken.None,TaskCreationOptions.None,TaskScheduler.Current);
        }

        private static void RunTestCore(bool debug) {
            var lastBuildStatusArgs = new LastBuildStatusArgs();
            OnQueryLastBuildStatus(lastBuildStatusArgs);
            try {
                _dte.WriteToOutput("Building EasyTest/Debug Configuration");
                if (_dte.Solution.BuildSolution()) {
                    var activeFileName = _dte.ActiveDocument.FullName;
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
                            _dte.WriteToOutput(messageElement.Value);
                        }
                        else
                            _dte.WriteToOutput("EasyTest Passed!");
                    }
                }
                else {
                    _dte.WriteToOutput("EasyTest build failed");
                }
            }
            catch (Exception e) {
                _dte.WriteToOutput(e.ToString());
            }
        }

        private static string WriteToOutput(bool debug,  string activeFileName){
            string debugSwitch = null;
            if (debug){
                debugSwitch = " -d:" + ((TextSelection) _dte.ActiveDocument.Selection).CurrentLine;
                _dte.WriteToOutput("Debug will start at line " + ((TextSelection) _dte.ActiveDocument.Selection).CurrentLine);
            }
            else{
                _dte.WriteToOutput("EasyTesting " + activeFileName);
            }
            return debugSwitch;
        }

        private static string GetTestExecutorPath() {
            
            var testExecutorPath = OptionClass.Instance.TestExecutorPath;
            if (string.IsNullOrWhiteSpace(testExecutorPath)) {
                var version = _dte.Solution.GetDXVersion();
                var dxRootDirectory = _dte.Solution.GetDXRootDirectory();
                return Path.Combine(dxRootDirectory + @"\Tools\eXpressAppFramework\EasyTest", "TestExecutor." + version + ".exe");
            }
            return testExecutorPath;
        }

        protected static void OnQueryLastBuildStatus(LastBuildStatusArgs e){
            QueryLastBuildStatus?.Invoke(null, e);
        }
    }
}
