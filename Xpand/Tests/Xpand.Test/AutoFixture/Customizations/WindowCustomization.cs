
using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Xpand.Test.AutoFixture.Customizations {
    public class WindowCustomization :ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new WindowSpecimentBuilder());
        }
    }

    public class WindowSpecimentBuilder :  ISpecimenBuilder{

        public object Create(object request, ISpecimenContext context){
            var type = request as Type;
            if (typeof(Window).IsAssignableFrom(type)){
                var application = context.Create<XafApplication>();
                return application.CreateWindow(TemplateContext.ApplicationWindow, new List<Controller>(), true);
            }
            return new NoSpecimen();
        }

    }
}
