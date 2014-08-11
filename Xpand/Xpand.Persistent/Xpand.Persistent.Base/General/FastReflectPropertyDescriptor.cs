using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace Xpand.Persistent.Base.General{
    public class FastReflectPropertyDescriptor:PropertyDescriptor{
        private readonly PropertyInfo _propertyInfo;

        public FastReflectPropertyDescriptor(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.GetCustomAttributes(typeof(Attribute),false).Cast<Attribute>().ToArray()){
            _propertyInfo = propertyInfo;
        }

        public override bool CanResetValue(object component){
            throw new NotImplementedException();
        }

        public override object GetValue(object component){
            return component.GetPropertyValue(Name);
        }

        public override void ResetValue(object component){
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value){
            _propertyInfo.SetValue(component, value,null);
        }

        public override bool ShouldSerializeValue(object component){
            throw new NotImplementedException();
        }

        public override Type ComponentType{
            get { throw new NotImplementedException(); }
        }

        public override bool IsReadOnly{
            get { throw new NotImplementedException(); }
        }

        public override string ToString(){
            return Name;
        }

        public override Type PropertyType{
            get { return _propertyInfo.PropertyType; }
        }
    }
}