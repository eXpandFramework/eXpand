using System;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    [AttributeUsage(AttributeTargets.Property)]
    public class VisibleInExcelMapAttribute:Attribute{
        public VisibleInExcelMapAttribute(bool visible){
            Visible = visible;
        }

        public bool Visible{ get; }
    }
}