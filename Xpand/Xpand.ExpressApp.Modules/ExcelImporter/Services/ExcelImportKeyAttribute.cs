using System;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class ExcelImportKeyAttribute:Attribute {
        public ExcelImportKeyAttribute(string memberName) {
            MemberName = memberName;
        }

        public string MemberName { get; }
    }
}