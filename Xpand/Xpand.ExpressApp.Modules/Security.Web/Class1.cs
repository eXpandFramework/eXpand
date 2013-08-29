using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.Security.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web {
    public class Class1:XpandLogonParameters {
        // Fields...
        private string _propertyName;

        public string PropertyName {
            get { return _propertyName; }
            set {
                _propertyName = value;
            }
        }
        
    }
}
