using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;

namespace Xpand.Persistent.Base.General {
    public interface IPropertyEditor{
        event EventHandler ValueRead;
        event EventHandler<EventArgs> ControlCreated;
        void SetValue(string value);
        object ControlValue { get; }
        IMemberInfo MemberInfo { get; }
    }

    public interface IChooseFromListCollectionEditor : IPropertyEditor,IDependentPropertyEditor {
        
    }
}