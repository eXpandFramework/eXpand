using System;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Helpers;

namespace Xpand.EasyTest.Commands {
    public static class Extensions {
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

        public static void RegisterCommands(this IRegisterCommand registerCommand) {
            var dictionary = new Dictionary<Type, string>{
                {typeof (FillDateTimeValueCommand), FillDateTimeValueCommand.Name},
                {typeof (XpandCompareScreenshotCommand), XpandCompareScreenshotCommand.Name},
                {typeof (KillWindowCommand), KillWindowCommand.Name},
                {typeof (SendKeysCommand), SendKeysCommand.Name},
                {typeof (KillFocusCommand), KillFocusCommand.Name}
            };
            foreach (var keyValuePair in dictionary) {
                registerCommand.RegisterCommand(keyValuePair.Value, keyValuePair.Key);
            }
        }

    }
}