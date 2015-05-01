using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.EasyTest.Commands;
using Xpand.Utils.Helpers;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest {
    public interface IXpandTestWinAdapter : IXpandTestAdapter {
         
    }
    public interface IXpandTestAdapter {
    }

    public static class Extensions {
        private static readonly string[] _navigationControlPossibleNames = { "ViewsNavigation.Navigation", "Navigation" };

        public static ITestControl GetNavigationTestControl(this ICommandAdapter adapter) {
            string controlNames = "";
            for (int i = 0; i < _navigationControlPossibleNames.Length; i++) {
                if (adapter.IsControlExist(TestControlType.Action, _navigationControlPossibleNames[i])) {
                    try {
                        var testControl = adapter.CreateTestControl(TestControlType.Action,
                            _navigationControlPossibleNames[i]);
                        var gridBaseInterface = testControl.GetInterface<IGridBase>();
                        int itemsCount = gridBaseInterface.GetRowCount();
                        if (itemsCount > 0) {
                            return testControl;
                        }
                    }
                    catch (WarningException) {
                    }
                }
                controlNames += (i <= _navigationControlPossibleNames.Length)
                    ? _navigationControlPossibleNames[i] + " or "
                    : _navigationControlPossibleNames[i];
            }
            throw new WarningException(string.Format("Cannot find the '{0}' control", controlNames));
        }

        public static void Execute(this CopyFileCommand copyFileCommand,ICommandAdapter adapter,TestParameters testParameters,string sourceFile,string destinationFile){
            var sourceParameter = new Parameter("Source", sourceFile, true, new PositionInScript(0));
            var destinationParameter = new Parameter("Destination", destinationFile, true, new PositionInScript(0));
            copyFileCommand.ParseCommand(testParameters, sourceParameter, destinationParameter);
            copyFileCommand.Execute(adapter);
        }

        public static string GetXpandPath(string directory) {
            var directoryInfo = new DirectoryInfo(directory);
            while (!File.Exists(Path.Combine(directoryInfo.FullName, "Xpand.build"))) {
                directoryInfo = directoryInfo.Parent;
                if (directoryInfo == null)
                    throw new ArgumentNullException();
            }
            return directoryInfo.FullName;
        }

        public static string GetPlatformSuffixedPath(this ICommandAdapter adapter,string fileName){
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName) + "";
            var suffix = adapter.IsWinAdapter() ? ".win" : ".web";
            if (!fileNameWithoutExtension.ToLower().EndsWith(suffix)) {
                var directoryName = Path.GetDirectoryName(fileName) + "";
                fileName = string.Format("{0}{1}{2}", fileNameWithoutExtension, suffix, Path.GetExtension(fileName));
                fileName =Path.Combine(directoryName, fileName);
            }
            return fileName;
        }

        public static bool IsWinCommand(this Command instance){
            return Adapter is IXpandTestWinAdapter;
        }

        public static bool IsWinAdapter(this ICommandAdapter instance){
            return Adapter is IXpandTestWinAdapter;
        }

        public static string GetBinPath(this TestParameters testParameters){
            return testParameters.GetAlias("WinAppBin", "WebAppBin").Value;
        }

        public static TestAlias GetAlias(this TestParameters testParameters,string name,string webName=null){
            string aliasName = IsWinAdapter(null) ? name : webName??name;
            var options = testParameters.LoadOptions();
            return options.Aliases.Cast<TestAlias>().First(@alias => alias.Name == aliasName);
        }

        public static Options LoadOptions(this TestParameters testParameters) {
            var configPath = Path.Combine(testParameters.ScriptsPath, "config.xml");
            var optionsStream = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Options.LoadOptions(optionsStream, null, null, Path.GetDirectoryName(configPath));
        }

        public static Command ParseCommand(this Command command,TestParameters testParameters,params Parameter[] parameters){
            string[] strings = parameters.Select(parameter =>" "+ parameter.Name + " = " + parameter.Value).ToArray();
            var commandName = "*"+command.GetType().Name.Replace("Command","");
            var scriptLines = new ScriptStringList(new[] { commandName }.Concat(strings).ToArray());
            var commandCreationParam = new CommandCreationParam(scriptLines, 0, testParameters);
            command.ParseCommand(commandCreationParam);
            return command;
        }

        public static Command SynchWith(this Command instance, Command command) {
            instance.Parameters.AddRange(command.Parameters);
            instance.Parameters.MainParameter = command.Parameters.MainParameter;
            instance.Parameters.ExtraParameter = command.Parameters.ExtraParameter;
            return instance;
        }

        public static T ParameterValue<T>(this TestApplication application, string parameterName){
            return application.ParameterValue(parameterName, default(T));
        }

        public static T ParameterValue<T>(this TestApplication application, string parameterName, T defaultValue) {
            var paramValue = application.FindParamValue(parameterName);
            T result;
            if (!XpandConvert.TryToChange(paramValue, out result)) {
                throw new EasyTestException(string.Format("Cannot retrieve the '{0}' attribute's value for the '{1}' application", parameterName, application.Name));
            }
            return result;
        }

        public static T ParameterValue<T>(this Command command, string parameterName){
            return command.ParameterValue(parameterName, default(T));
        }

        public static T ParameterValue<T>(this Command command, string parameterName,T defaultValue) {
            T result = defaultValue;
            Parameter parameter = command.Parameters[parameterName];
            if (parameter != null){
                if (!XpandConvert.TryToChange(parameter.Value, out result)) {
                    throw new CommandException(string.Format("'{0}' value is incorrect", parameterName), command.StartPosition);    
                }
            }
            return result;
        }
        
        private static IXpandTestAdapter _adapter;

        public static IXpandTestAdapter Adapter {
            get { return _adapter; }
        }

        public static string WindowText(this IntPtr intPtr) {
            int length = Win32Declares.Window.GetWindowTextLength(intPtr);
            var sb = new StringBuilder(length + 1);
            Win32Declares.Window.GetWindowText(intPtr, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void RegisterCommands(this IRegisterCommand registerCommand,IXpandTestAdapter applicationAdapter){
            _adapter = applicationAdapter;
            var dictionary = new Dictionary<Type, string>{
                {typeof (FillDateTimeValueCommand), FillDateTimeValueCommand.Name},
                {typeof (CreatePermissionCommand), CreatePermissionCommand.Name},
                {typeof (ChangeUserPasswordCommand), ChangeUserPasswordCommand.Name},
                {typeof (NavigateCommand), NavigateCommand.Name},
                {typeof (SaveAndCloseCommand), SaveAndCloseCommand.Name},
                {typeof (HideCursorCommand), HideCursorCommand.Name},
                {typeof (KillFocusCommand), KillFocusCommand.Name},
                {typeof (XpandDeleteFileCommand), XpandDeleteFileCommand.Name},
                {typeof (KillWindowCommand), KillWindowCommand.Name},
                {typeof (XpandProcessRecordCommand), XpandProcessRecordCommand.Name},
                {typeof (SqlCommand), SqlCommand.Name},
                {typeof (SqlDropDatabaseCommand), SqlDropDatabaseCommand.Name},
                {typeof (SendKeysCommand), SendKeysCommand.Name},
                {typeof (MouseCommand), MouseCommand.Name},
                {typeof (UseModelCommand), UseModelCommand.Name},
                {typeof (SetEnvironmentVariableCommand), SetEnvironmentVariableCommand.Name},
                {typeof (XpandHandleDialogCommand), XpandHandleDialogCommand.Name},
                {typeof (XpandFillFormCommand), XpandFillFormCommand.Name},
                {typeof (XpandAutoTestCommand), XpandAutoTestCommand.Name},
                {typeof (XpandCheckFieldValuesCommand), XpandCheckFieldValuesCommand.Name},
                {typeof (LogonCommand), LogonCommand.Name},
                {typeof (XpandCheckFileExistsCommand), XpandCheckFileExistsCommand.Name},
                {typeof (ResizeWindowCommand), ResizeWindowCommand.Name},
                {typeof (FocusWindowCommand), FocusWindowCommand.Name},
                {typeof (XpandCompareScreenshotCommand), XpandCompareScreenshotCommand.Name},
                {typeof (XpandSelectRecordsCommand), XpandSelectRecordsCommand.Name},
                {typeof (ScreenCaptureCommand), ScreenCaptureCommand.Name},
                {typeof (StopCommand), StopCommand.Name},
            };
            foreach (var keyValuePair in dictionary) {
                registerCommand.RegisterCommand(keyValuePair.Value, keyValuePair.Key);
            }
        }

    }
}