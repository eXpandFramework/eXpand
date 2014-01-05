//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DevExpress.Persistent.Base.General;

//namespace EFDemo.Module.Data {
//    public partial class PropertyDescriptor : IPropertyDescriptor {
//        private Type valueType;
//        public PropertyDescriptor() {
//        }
//        public PropertyDescriptor(String name, Type valueType) {
//            Name = name;
//            this.valueType = valueType;
//            Code = name.Substring(0, Math.Min(4, name.Length));
//        }
//        public Type ValueType {
//            get { return valueType; }
//        }
//        /*[FieldSize(4)]
//        [RuleRequiredField("PropertyDescriptor Code required", "Save", "")]
//        [RuleUniqueValue("PropertyDescriptor Code is unique", "Save", "The 'Code' property must have a unique value")]
//        public String Code {
//            get { return code; }
//            set { code = value; }
//        }*/
//    }
//}
