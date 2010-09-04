using System;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.PivotChart.PivotedProperty {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PivotedSortAttribute : Attribute
    {
        readonly string _propertyName;
        readonly SortDirection _sortDirection;
        readonly string _sortPropertyName;

        public PivotedSortAttribute(string propertyName, SortDirection sortDirection, string sortPropertyName)
        {
            _propertyName = propertyName;
            _sortDirection = sortDirection;
            _sortPropertyName = sortPropertyName;
        }

        public string SortPropertyName
        {
            get { return _sortPropertyName; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public SortDirection SortDirection
        {
            get { return _sortDirection; }
        }
    }
}