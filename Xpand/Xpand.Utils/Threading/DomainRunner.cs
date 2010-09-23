using System;
using Xpand.Utils.Threading;

namespace Xpand.Utils.Threading
{
    public class DomainRunner {
        public static object RunInAppDomain(Delegate delg, AppDomain targetDomain, params object[] args) {
            var runner = new domainDomainRunner(delg, args, delg.GetHashCode());
            targetDomain.DoCallBack(runner.Invoke);

            return targetDomain.GetData("appDomainResult" + delg.GetHashCode());
        }

        public static object RunInAppDomain(Delegate delg, params object[] args) {
            AppDomain tempDomain = AppDomain.CreateDomain("domain_RunInAppDomain" + delg.GetHashCode());
            object returnValue;
            try {
                returnValue = RunInAppDomain(delg, tempDomain, args);
            }
            finally {
                AppDomain.Unload(tempDomain);
            }
            return returnValue;
        }
        #region Nested type: domainDomainRunner
        [Serializable]
        internal class domainDomainRunner {
            readonly object[] _arguments;
            readonly Delegate _delegate;
            public int _hash;

            public domainDomainRunner(Delegate delg, object[] args, int hash) {
                _delegate = delg;
                _arguments = args;
                _hash = hash;
            }

            public void Invoke() {
                Console.WriteLine("I'm running in domain named {0}", AppDomain.CurrentDomain.FriendlyName);
                if (_delegate != null)
                    AppDomain.CurrentDomain.SetData("appDomainResult" + _hash, _delegate.DynamicInvoke(_arguments));
            }
        }
        #endregion
    }
}

class MyApp {
    public delegate int MyDelegate();

    public static int Main(string[] args) {
        object[] a = {};
        object result = DomainRunner.RunInAppDomain(new MyDelegate(Foo), a);
        if (result == null)
            Console.WriteLine("is null");
        else
            Console.WriteLine("{0}", (int) result);
        return 0;
    }

    public static int Foo() {
        return 1;
    }
} 