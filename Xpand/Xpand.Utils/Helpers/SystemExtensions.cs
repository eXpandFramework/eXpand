using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace Xpand.Utils.Helpers {
    public static class SystemExtensions {
        public static string AggregateErrors(this CompilerResults compilerResults) {
            return string.Join(Environment.NewLine, compilerResults.Errors.Cast<CompilerError>().Select(error =>
                $"({error.Line},{error.Column}): {error.ErrorText}"));
        }
    }
}
