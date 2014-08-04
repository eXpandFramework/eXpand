using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Helpers;

namespace Xpand.EasyTest.Commands {
    public interface IXpandTestWinAdapter : IXpandTestAdapter {
         
    }
    public interface IXpandTestAdapter {
    }

    public static class Extensions {
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

        public static bool IsWinAdapter(this ICommandAdapter instance){
            return Adapter is IXpandTestWinAdapter;
        }

        public static Command SynchWith(this Command instance, Command command) {
            instance.Parameters.AddRange(command.Parameters);
            instance.Parameters.MainParameter = command.Parameters.MainParameter;
            return instance;
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
        // Fields...
        private static IXpandTestAdapter _adapter;

        public static IXpandTestAdapter Adapter {
            get { return _adapter; }
        }
        
        public static void RegisterCommands(this IRegisterCommand registerCommand,IXpandTestAdapter applicationAdapter){
            _adapter = applicationAdapter;
            var dictionary = new Dictionary<Type, string>{
                {typeof (FillDateTimeValueCommand), FillDateTimeValueCommand.Name},
                {typeof (HideCursorCommand), HideCursorCommand.Name},
                {typeof (KillFocusCommand), KillFocusCommand.Name},
                {typeof (KillWindowCommand), KillWindowCommand.Name},
                {typeof (SendKeysCommand), SendKeysCommand.Name},
                {typeof (XpandFillFormCommand), XpandFillFormCommand.Name},
                {typeof (XpandCompareScreenshotCommand), XpandCompareScreenshotCommand.Name},
            };
            foreach (var keyValuePair in dictionary) {
                registerCommand.RegisterCommand(keyValuePair.Value, keyValuePair.Key);
            }
        }

    }
}