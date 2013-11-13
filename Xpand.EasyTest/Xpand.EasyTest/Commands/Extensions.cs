using System;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {
    public static class Extensions {
        public static void RegisterCommands(this IRegisterCommand registerCommand) {
            var dictionary = new Dictionary<Type, string>{{typeof (FillDateTimeValueCommand), "FillDateTimeValue"}};
            foreach (var keyValuePair in dictionary) {
                registerCommand.RegisterCommand(keyValuePair.Value, keyValuePair.Key);
            }
        }
    }
}