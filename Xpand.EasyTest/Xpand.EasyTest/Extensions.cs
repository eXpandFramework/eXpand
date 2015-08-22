using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.EasyTest.Framework.Loggers;
using DevExpress.Xpo.DB.Helpers;
using Xpand.EasyTest.Commands;
using Xpand.Utils.Helpers;

namespace Xpand.EasyTest {
    public enum ApplicationParams {
        PhysicalPath,
        UseIISExpress,
        UseModel,
        DefaultWindowSize,
        Url,
        DontRunWebDev,
        SingleWebDev,
        WaitDebuggerAttached,
        DontKillWebDev,
        DontRestartIIS,
        WebBrowserType,
        FileName,
        Model
    }

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

        public static string GetBinPath(this Command command) {
            return _application.ParameterValue<string>(ApplicationParams.PhysicalPath) ??
                   Path.GetDirectoryName(_application.ParameterValue<string>(ApplicationParams.FileName));
        }

        public static Options LoadOptions(string scriptsPath) {
            var configPath = Path.Combine(scriptsPath, "config.xml");
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

        public static void DeleteUserModel(this TestApplication testApplication) {
            var appPath = testApplication.ParameterValue<string>(ApplicationParams.FileName);
            var directoryName = File.Exists(appPath) ? Path.GetDirectoryName(appPath) + "" : testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath);
            foreach (var file in Directory.GetFiles(directoryName, "Model.user*.xafml").ToArray()) {
                File.Delete(file);
            }
        }

        public static T ParameterValue<T>(this TestApplication application, ApplicationParams applicationParams) {
            return application.ParameterValue(applicationParams, default(T));
        }

        public static T ParameterValue<T>(this TestApplication application, ApplicationParams applicationParams, T defaultValue) {
            var parameterValue = application.ParameterValue<T>(applicationParams.ToString());
            return Equals(default(T), parameterValue) ? defaultValue : parameterValue;
        }

        public static void ClearModel(this TestApplication application){
            var appPath = application.ParameterValue<string>(ApplicationParams.PhysicalPath) ?? Path.GetDirectoryName(application.ParameterValue<string>(ApplicationParams.FileName));
            File.WriteAllText(Path.Combine(appPath+"","Model.xafml"), @"<?xml version=""1.0"" ?><Application />");
        }

        public static void CopyModel(this TestApplication application){
            application.ClearModel();
            var appPath = application.ParameterValue<string>(ApplicationParams.PhysicalPath) ?? Path.GetDirectoryName(application.ParameterValue<string>(ApplicationParams.FileName));
            var modelFileName = GetModelFileName(application);
            var destFileName = Path.Combine(appPath+"", "Model.xafml");
            if (File.Exists(modelFileName)){
                File.Copy(modelFileName, destFileName, true);
            }
        }

        private static string GetModelFileName(TestApplication application){
            var model = application.ParameterValue<string>(ApplicationParams.Model);
            var logPath = Logger.Instance.GetLogger<FileLogger>().LogPath;
            return model!=null ? Path.Combine(logPath, model + ".xafml") : logPath;
        }

        public static void CreateParametersFile(this TestApplication application){
            application.DeleteParametersFile();
            var paramFile = application.GetParameterFile();
            var paramValue = application.ParameterValue<string>("Parameter");
            if (paramValue != null){
                using (var streamWriter = File.CreateText(paramFile)){
                    streamWriter.WriteLine(paramValue);
                }
            }
        }

        public static void DeleteParametersFile(this TestApplication application){
            var paramFile = application.GetParameterFile();
            if (File.Exists(paramFile))
                File.Delete(paramFile);
        }

        private static string GetParameterFile(this TestApplication application){
            var path = application.ParameterValue<string>(ApplicationParams.PhysicalPath) ??
                       Path.GetDirectoryName(application.ParameterValue<string>(ApplicationParams.FileName));
            return Path.Combine(path+"", "easytestparameters");
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
        private static TestApplication _application;

        public static IXpandTestAdapter Adapter {
            get { return _adapter; }
        }

        public static void Assign(this TestApplication application) {
            _application=application;    
        }

        public static void RegisterCommands(this IRegisterCommand registerCommand, IXpandTestAdapter applicationAdapter){
            _adapter = applicationAdapter;
            var dictionary = new Dictionary<Type, string>{
                {typeof (XpandCompareScreenshotCommand), XpandCompareScreenshotCommand.Name},
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
                {typeof (MouseDragDropCommand), MouseDragDropCommand.Name},
                {typeof (UseModelCommand), UseModelCommand.Name},
                {typeof (SetEnvironmentVariableCommand), SetEnvironmentVariableCommand.Name},
                {typeof (XpandHandleDialogCommand), XpandHandleDialogCommand.Name},
                {typeof (XpandFillFormCommand), XpandFillFormCommand.Name},
                {typeof (XpandFillRecordCommand), XpandFillRecordCommand.Name},
                {typeof (SaveFileDialogCommand), SaveFileDialogCommand.Name},
                {typeof (OpenFileDialogCommand), OpenFileDialogCommand.Name},
                {typeof (XpandAutoTestCommand), XpandAutoTestCommand.Name},
                {typeof (XpandCheckFieldValuesCommand), XpandCheckFieldValuesCommand.Name},
                {typeof (LogonCommand), LogonCommand.Name},
                {typeof (LogOffCommand), LogOffCommand.Name},
                {typeof (XpandCheckFileExistsCommand), XpandCheckFileExistsCommand.Name},
                {typeof (ResizeWindowCommand), ResizeWindowCommand.Name},
                {typeof (FocusWindowCommand), FocusWindowCommand.Name},
                {typeof (ScreenCaptureCommand), ScreenCaptureCommand.Name},
                {typeof (StopCommand), StopCommand.Name},
                {typeof (ToggleNavigationCommand), ToggleNavigationCommand.Name},
            };
            foreach (var keyValuePair in dictionary) {
                registerCommand.RegisterCommand(keyValuePair.Value, keyValuePair.Key);
            }
        }

        public static IDbConnection DbConnection(this TestDatabase database, string connectionString, string assemblyName, string typeName){
            return ReflectConnectionHelper.GetConnection(assemblyName,typeName,connectionString);
        }
    }
}