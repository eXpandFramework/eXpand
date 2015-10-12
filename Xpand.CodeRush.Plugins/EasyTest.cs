using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.CodeRush.Menus;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;
using Process = System.Diagnostics.Process;

namespace Xpand.CodeRush.Plugins{
    class LastBuildStatusArgs:EventArgs{
        public bool Successed { get; set; }
    }
    class EasyTest{
        private IMenuButton[] _menuButtons;
        private bool _lockButtonEnableState;
        public event EventHandler<LastBuildStatusArgs> QueryLastBuildStatus;
        public void RunTest(bool debug){
            Task.Factory.StartNew(() => RunTestCore(debug));
        }

        private void RunTestCore(bool debug){
            var lastBuildStatusArgs = new LastBuildStatusArgs();
            OnQueryLastBuildStatus(lastBuildStatusArgs);
            DTE dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
            try{
                ChangeButtonsEnableState(false);
                _lockButtonEnableState = true;
                var uniqueName =
                    DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Solution.FindStartUpProject().UniqueName;
                dte.WriteToOutput("Building EasyTest/Debug Configuration");
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
                        dte.WriteToOutput("Debug will start at line " + DevExpress.CodeRush.Core.CodeRush.Caret.Line);
                    }
                    else{
                        dte.WriteToOutput("EasyTesting "+activeFileName);
                    }
                    var testExecutorPath = Options.ReadString(Options.TestExecutorPath);
                    if (!File.Exists(testExecutorPath))
                        throw new FileNotFoundException(
                            "Use plugin options to assign a valid path for the standalond TestExecutor");
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
                        dte.WriteToOutput(messageElement.Value);
                    }
                    else
                        dte.WriteToOutput("EasyTest Passed!");
                }
                else{
                    dte.WriteToOutput("EasyTest build failed");
                }
            }
            catch (Exception e){
                dte.WriteToOutput(e.ToString());
            }
            finally{
                _lockButtonEnableState = false;
                ChangeButtonsEnableState(true);
            }
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