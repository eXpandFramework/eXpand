using System;
using System.Diagnostics;
using System.Globalization;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ImportWizard.Win.Properties;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard{
    public class StringValueMapper{
        public virtual void MapValueToObjectProperty(XPObjectSpace objectSpace, XPMemberInfo prop, string value,
            ref IXPSimpleObject newObj){
            object convertedValue = null;

            //if simple property
            if (prop.ReferenceType == null){
                bool isNullable = prop.MemberType.IsGenericType &&
                                  prop.MemberType.GetGenericTypeDefinition().IsNullableType();

                if (prop.MemberType == null) return;


                convertedValue = isNullable ? MapStringToNullable(prop, value) : MapStringToValueType(prop, value);
            }

                //if referenced property
            else if (prop.ReferenceType != null)
                MapStringToReferenceType(objectSpace, prop, value);

            if (convertedValue != null)
                convertedValue = AppllyDoubleValueRounding(convertedValue);

            prop.SetValue(newObj, convertedValue);
        }

        /// <summary>
        ///     Defines how string is converted into NOT nullable value type
        /// </summary>
        /// <param name="prop">property that needs the converted value</param>
        /// <param name="value">string value to be converted</param>
        /// <param name="numberFormatInfo">Number formatting info</param>
        /// <returns>Converted value</returns>
        protected virtual object MapStringToValueType(XPMemberInfo prop, string value,
            NumberFormatInfo numberFormatInfo = null){
            object result = null;

            if (prop.MemberType.IsEnum)
                result = Enum.Parse(prop.MemberType, value);

            else if (prop.MemberType == typeof (char))
                result = Convert.ChangeType(ImportUtils.GetQString(value), prop.MemberType);

            else if (prop.StorageType == typeof (int)){
                int number;
                if (value != String.Empty && Int32.TryParse(value, out number))
                    result = number;
                else
                    result = 0;
            }
            else if (prop.MemberType == typeof (Guid))
                result = new Guid(ImportUtils.GetQString(value));
            else if (prop.StorageType == typeof (DateTime)){
                if (value != string.Empty){
                    //Include validate
                    DateTime dt = DateTime.FromOADate(Convert.ToDouble(value));
                    result = dt;
                }
            }
            else if (prop.MemberType == typeof (double)){
                double number;

                if (Double.TryParse(value, NumberStyles.Number,
                    numberFormatInfo ?? new NumberFormatInfo{NumberDecimalSeparator = "."}, out number))
                    result = number;
            }
            else if (prop.MemberType == typeof (bool)){
                if (value != string.Empty &&
                    (value.Length == 1 || value.ToLower() == @"true" || value.ToLower() == @"false")) {
                    bool truefalse;
                    if (value.ToLower() == @"true" || value.ToLower() == @"false")
                        truefalse = Convert.ToBoolean(value);
                    else
                        truefalse = Convert.ToBoolean(Convert.ToInt32(value));
                    result = truefalse;
                }
            }
            else
                result = Convert.ChangeType(value, prop.MemberType);
            return result;
        }

        /// <summary>
        ///     Defines how string is converted into NULLABLE VALUE (like int?) type
        /// </summary>
        /// <param name="prop">property that needs the converted value</param>
        /// <param name="value">string value to be converted</param>
        /// <param name="numberFormatInfo">Number formatting info</param>
        /// <returns>Converted value</returns>
        protected virtual object MapStringToNullable(XPMemberInfo prop, string value,
            NumberFormatInfo numberFormatInfo = null){
            object result = null;

            //TODO: Test this !!!
            if (prop.MemberType.IsEnum)
                result = Enum.Parse(prop.StorageType, value, true);

            else if (prop.StorageType == typeof (int)){
                int number;
                if (value != String.Empty && Int32.TryParse(value, out number))
                    result = number;
            }
            else if (prop.StorageType == typeof (DateTime)){
                if (value != string.Empty){
                    //Include validate
                    DateTime dt = DateTime.FromOADate(Convert.ToDouble(value));
                    result = dt;
                }
            }
            else if (prop.StorageType == typeof (double)){
                double number;
                if (Double.TryParse(value, NumberStyles.Number,
                    numberFormatInfo ?? new NumberFormatInfo{NumberDecimalSeparator = "."}, out number))
                    result = number;
            }
            else
                result = Convert.ChangeType(value, prop.StorageType);
            return result;
        }

        /// <summary>
        ///     Specifies double value rounding rules.
        ///     Override for customisation
        /// </summary>
        /// <param name="convertedValue">Raw value</param>
        /// <returns>rounded value</returns>
        protected virtual object AppllyDoubleValueRounding(object convertedValue){
            if (convertedValue is double)
                convertedValue = Math.Round((double) convertedValue, 2, MidpointRounding.ToEven);
            return convertedValue;
        }

        /// <summary>
        ///     Specifies the rules and actions how to convert string to a referenced type
        /// </summary>
        /// <param name="objectSpace">OS used to lookup refecenced object</param>
        /// <param name="prop">property that needs the converted value</param>
        /// <param name="value">string value to be converted</param>
        protected virtual object MapStringToReferenceType(XPObjectSpace objectSpace, XPMemberInfo prop, string value){
            //if other referenced type
            if (prop.MemberType.IsSubclassOf(typeof (XPBaseObject))){
                string text = value;
                Type type = prop.MemberType;
                try{
                    XPBaseObject mval = Helper.GetXpObjectByKeyValue(objectSpace, text, type);
                    return objectSpace.GetObject(mval);
                }
                catch (Exception e){
                    Trace.TraceWarning(Resources.RefTypeConversionError, value, prop.MemberType.Name, e);
                }
            }
            return null;
        }
    }
}