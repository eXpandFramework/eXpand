using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Xpand.Xpo;

namespace Xpand.Tests.Xpand.Xpo {
    public class When_ {
        static object _o;
        static MyEnum _myEnum;

        enum MyEnum {
            Val1,
            Val2
        }

        Establish context = () => {
            _myEnum = MyEnum.Val1;
        };

        Because of = () => {
            _o = XpandReflectionHelper.ChangeType(null, typeof(MyEnum?));
        };

        It should_should = () => _o.ShouldBeNull();
    }
}
