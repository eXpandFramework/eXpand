using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Logic.TypeConverters {
    public class StringToModelViewConverter : TypeConverter {
        readonly IModelApplication _modelApplication;

        public StringToModelViewConverter(IModelApplication modelApplication) {
            _modelApplication = modelApplication;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            IModelView modelView = _modelApplication.Views.SingleOrDefault(view => view.Id == value.ToString());
            if (modelView == null)
                throw new NullReferenceException(value.ToString());
            return modelView;
        }
    }
}