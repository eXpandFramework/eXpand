using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.Xpo;

namespace Xpand.Tests.Xpand.Xpo {
    public class DynamicCalcPropertyObject : BaseObject {
        public DynamicCalcPropertyObject(Session session)
            : base(session) {
        }
        private int _age;
        public int Age {
            get {
                return _age;
            }
            set {
                SetPropertyValue("Age", ref _age, value);
            }
        }

    }

    public class When_a_calculated_member_is_requested : With_In_Memory_DataStore {
        static string _stringAge;
        static DynamicCalcPropertyObject _dynamicCalcPropertyObject;

        Establish context = () => {
            _dynamicCalcPropertyObject = XPObjectSpace.CreateObject<DynamicCalcPropertyObject>();
            _dynamicCalcPropertyObject.ClassInfo.CreateCalculabeMember("StringAge", typeof(string), "Concat(Age,'0')");
            _dynamicCalcPropertyObject.Age = 2;
            XPObjectSpace.CommitChanges();
        };

        Because of = () => {
            _stringAge = _dynamicCalcPropertyObject.GetMemberValue("StringAge") as string;
        };

        It should_return_the_calculated_value = () => _stringAge.ShouldEqual("20");
    }
}
