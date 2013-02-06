using DevExpress.ExpressApp.DC;

namespace FeatureCenter.Module.RuntimeFields.DC {
    [DomainComponent]
    public interface DomainComponentWithRuntimeMembers {
        string Name { get; set; }
        string Email { get; set; }
        string Twitter { get; set; }
        //string CalculatedProperty { get; }
        //int SumMethod(int val1, int val2);
    }

    // To use a Domain Component in an XAF application, the Component should be registered.
    // Override the ModuleBase.Setup method in the application's module and invoke the ITypesInfo.RegisterEntity method in it:
    //
    // public override void Setup(XafApplication application) {
    //     XafTypesInfo.Instance.RegisterEntity("MyComponent", typeof(DomainComponent1));
    //     base.Setup(application);
    // }

    //[DomainLogic(typeof(DomainComponentWithRuntimeMembers))]
    //public class DomainComponent1Logic {
    //    public static string Get_CalculatedProperty(DomainComponentWithRuntimeMembers instance) {
    //        // A "Get_" method is executed when getting a target property value. The target property should be readonly.
    //        // Use this method to implement calculated properties.
    //        return "";
    //    }
    //    public static void AfterChange_PersistentProperty(DomainComponentWithRuntimeMembers instance) {
    //        // An "AfterChange_" method is executed after a target property is changed. The target property should not be readonly. 
    //        // Use this method to refresh dependant property values.
    //    }
    //    public static void AfterConstruction(DomainComponentWithRuntimeMembers instance) {
    //        // The "AfterConstruction" method is executed only once, after an object is created. 
    //        // Use this method to initialize new objects with default property values.
    //    }
    //    public static int SumMethod(DomainComponentWithRuntimeMembers instance, int val1, int val2) {
    //        // You can also define custom methods.
    //        return val1 + val2;
    //    }
    //}
}
