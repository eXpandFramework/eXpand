using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.CodeRush.Diagnostics.Commands;
using DevExpress.CodeRush.Menus;
using EnvDTE;
using Microsoft.Win32;
using Xpand.CodeRush.Plugins.Extensions;
using Process = System.Diagnostics.Process;

namespace Xpand.CodeRush.Plugins{
    class LastBuildStatusArgs:EventArgs{
        public bool Successed { get; set; }
    }
    class EasyTest{
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        private IMenuButton[] _menuButtons;
        private bool _lockButtonEnableState;
        public event EventHandler<LastBuildStatusArgs> QueryLastBuildStatus;
        public void RunTest(bool debug){
            _dte.InitOutputCalls("RunTest");
            Task.Factory.StartNewNow(() => RunTestCore(debug));
        }

        private void RunTestCore(bool debug){
            var lastBuildStatusArgs = new LastBuildStatusArgs();
            OnQueryLastBuildStatus(lastBuildStatusArgs);
            try{
                ChangeButtonsEnableState(false);
                _lockButtonEnableState = true;
                var uniqueName =
                    DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Solution.FindStartUpProject().UniqueName;
                _dte.WriteToOutput("Building EasyTest/Debug Configuration");
                DevExpress.CodeRush.Core.CodeRush.Solution.Active.SolutionBuild.BuildProject("EasyTest", uniqueName,
                    true);
                if (lastBuildStatusArgs.Successed){
                    var activeFileName = DevExpress.CodeRush.Core.CodeRush.Documents.ActiveFileName;
                    var testLogPath = Path.Combine(Path.GetDirectoryName(activeFileName) + "", "Testslog.xml");
                    if (File.Exists(testLogPath))
                        File.Delete(testLogPath);
                    string debugSwitch = null;
                    if (debug){
                        debugSwitch = " -d:" + DevExpress.CodeRush.Core.CodeRush.Caret.Line;
                        _dte.WriteToOutput("Debug will start at line " + DevExpress.CodeRush.Core.CodeRush.Caret.Line);
                    }
                    else{
                        _dte.WriteToOutput("EasyTesting "+activeFileName);
                    }

                    var testExecutorPath = GetTestExecutorPath();
                    Log.Send("TestExecutorpath", testExecutorPath);
                    if (!File.Exists(testExecutorPath))
                        throw new FileNotFoundException(
                            "Use plugin options to assign a valid path for the standalond TestExecutor. Or leave it blank for auto detection.");
                    var processStartInfo = new ProcessStartInfo(testExecutorPath){
                        Arguments = string.Format(@"""{0}""{1}", activeFileName, debugSwitch),
                        UseShellExecute = debug,
                        RedirectStandardOutput = !debug,
                        CreateNoWindow = !debug
                    };

                    var process = Process.Start(processStartInfo);
                    Debug.Assert(process != null, "process != null");
                    process.WaitForExit();
                    var document = XDocument.Load(File.OpenRead(testLogPath));
                    var errorElement =
                        document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Error");
                    if (errorElement != null){
                        var messageElement = errorElement.Descendants("Message").First();
                        _dte.WriteToOutput(messageElement.Value);
                    }
                    else
                        _dte.WriteToOutput("EasyTest Passed!");
                }
                else{
                    _dte.WriteToOutput("EasyTest build failed");
                }
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
            finally{
                _lockButtonEnableState = false;
                ChangeButtonsEnableState(true);
            }
        }

        private string GetTestExecutorPath(){
            var testExecutorPath = Options.ReadString(Options.TestExecutorPath);
            if (string.IsNullOrWhiteSpace(testExecutorPath)){
                var version = DevExpress.CodeRush.Core.CodeRush.Solution.Active.GetDXVersion();
                var registryKey = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432node\DevExpress\Components\"+version);
                if (registryKey != null)
                    return Path.Combine(registryKey.GetValue("RootDirectory")+ @"\Tools\eXpressAppFramework\EasyTest", "TestExecutor." +version+ ".exe");
            }
            return testExecutorPath;
        }

        public void CreateButtons(){
            _menuButtons = new IMenuButton[2];
            if (DevExpress.CodeRush.Core.CodeRush.Menus.Bars.Exists("EasyTest")){
                var menuBar = DevExpress.CodeRush.Core.CodeRush.Menus.Bars["EasyTest"];
                if (menuBar != null){
                    var menuButton = menuBar.AddButton();
                    _menuButtons[0]=menuButton;
                    menuButton.Caption = "R";
                    menuButton.Enabled = false;
                    menuButton.TooltipText = "Run";
                    menuButton.Click += (sender, args) => RunTest(false);
                    menuButton.Visible = true;
                    menuButton = menuBar.AddButton();
                    _menuButtons[1] = menuButton;
                    menuButton.Enabled = false;
                    menuButton.Caption = "D";
                    menuButton.TooltipText = "Debug";
                    menuButton.Click += (sender, args) => RunTest(true);
                    menuButton.Visible = true;
                }
            }
        }

        protected virtual void OnQueryLastBuildStatus(LastBuildStatusArgs e){
            var handler = QueryLastBuildStatus;
            if (handler != null) handler(this, e);
        }

        public void ChangeButtonsEnableState(bool enabled){
            if (_lockButtonEnableState)
                return;
            foreach (var menuButton in _menuButtons){
                menuButton.Enabled = enabled;
            }
        }
    }
}