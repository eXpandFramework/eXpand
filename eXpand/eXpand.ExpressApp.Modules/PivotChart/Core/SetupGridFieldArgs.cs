using System;
using DevExpress.XtraPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Core {
    public class SetupGridFieldArgs : EventArgs
    {
        readonly PivotGridFieldBase _pivotGridField;
        readonly Type _memberType;
        readonly string _displayFormat;


        public SetupGridFieldArgs(PivotGridFieldBase pivotGridField, Type memberType, string displayFormat)
        {
            _pivotGridField = pivotGridField;
            _memberType = memberType;
            _displayFormat = displayFormat;
        }

        public Type MemberType
        {
            get { return _memberType; }
        }

        public string DisplayFormat
        {
            get { return _displayFormat; }
        }

        public PivotGridFieldBase PivotGridField
        {
            get { return _pivotGridField; }
        }
    }
}