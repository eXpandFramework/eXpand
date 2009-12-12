using System.Collections;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Tests.eXpand.WorldCreator {
    public interface IDummyString {
        string StringPropertyName { get; set; }
        User ReferencePropertyName {get;set;}
    }
    public interface INotSupported
    {
        IList PropertyName { get; set; }
    }

}