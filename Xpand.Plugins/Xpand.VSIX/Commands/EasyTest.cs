using System;
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
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands{
    class LastBuildStatusArgs : EventArgs {
        public bool Successed { get; set; }
    }

    class EasyTest {
        private readonly DTE2 _dte=DteExtensions.DTE;
        public event EventHandler<LastBuildStatusArgs> QueryLastBuildStatus;
        public void RunTest(bool debug) {
            _dte.InitOutputCalls("RunTest");
            Task.Factory.StartNew(() => RunTestCore(debug),CancellationToken.None,TaskCreationOptions.None,TaskScheduler.Current);
        }

        private void RunTestCore(bool debug) {
            var lastBuildStatusArgs = new LastBuildStatusArgs();
            OnQueryLastBuildStatus(lastBuildStatusArgs);
            try {
                var uniqueName =_dte.Solution.FindStartUpProject().UniqueName;
                _dte.WriteToOutput("Building EasyTest/Debug Configuration");
                _dte.Solution.SolutionBuild.BuildProject("EasyTest", uniqueName,
                    true);
                if (_dte.Solution.SolutionBuild.LastBuildInfo==0) {
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

        private string WriteToOutput(bool debug,  string activeFileName){
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

        private string GetTestExecutorPath() {
            
            var testExecutorPath = OptionClass.Instance.TestExecutorPath;
            if (string.IsNullOrWhiteSpace(testExecutorPath)) {
                var version = _dte.Solution.GetDXVersion();
                var dxRootDirectory = _dte.Solution.GetDXRootDirectory();
                return Path.Combine(dxRootDirectory + @"\Tools\eXpressAppFramework\EasyTest", "TestExecutor." + version + ".exe");
            }
            return testExecutorPath;
        }

        protected virtual void OnQueryLastBuildStatus(LastBuildStatusArgs e) {
            var handler = QueryLastBuildStatus;
            handler?.Invoke(this, e);
        }

    }
}
