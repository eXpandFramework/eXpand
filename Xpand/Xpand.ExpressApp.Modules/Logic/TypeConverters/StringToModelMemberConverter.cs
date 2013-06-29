using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Logic.TypeConverters {
    public class StringToModelMemberConverter : TypeConverter {
        readonly IModelApplication _modelApplication;

        public StringToModelMemberConverter(IModelApplication modelApplication) {
            _modelApplication = modelApplication;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            var modelMember = _modelApplication.BOModel.GetClass(destinationType).AllMembers.FirstOrDefault(member => member.Name == value.ToString());
            if (modelMember == null)
                throw new NullReferenceException(value.ToString());
            return modelMember;
        }
    }
}