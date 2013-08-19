using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.Logic.TypeConverters {
    public class StringToModelViewConverter : TypeConverter {
        readonly IModelApplication _modelApplication;

        public StringToModelViewConverter() {
        }

        public StringToModelViewConverter(IModelApplication modelApplication) {
            _modelApplication = modelApplication;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof (string);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(CaptionHelper.ApplicationModel.Views.ToArray());
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            IModelView modelView = _modelApplication.Views.SingleOrDefault(view => view.Id == value.ToString());
            if (modelView == null)
                throw new NullReferenceException(value.ToString());
            return modelView;
        }
    }
}