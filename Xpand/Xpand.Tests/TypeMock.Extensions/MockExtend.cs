using System;
using System.Reflection;

namespace TypeMock.Extensions
{
    public static class MockExtend
    {
        public static void IsolateBaseCalls<T>(this Mock<T> mock)
        {
            throw new NotImplementedException();
//            object instance = MockManager.Mock(typeof(T).BaseType).MockedInstance;
//            foreach (var methodInfo in typeof(T).BaseType.GetMethods(BindingFlags.NonPublic|BindingFlags.Public | BindingFlags.Instance)){
//                if (methodInfo.ReturnType == typeof(void))
//                    mock.CallBase.ExpectCall(methodInfo.Name);
//                else{
//                    mock.CallBase.ExpectAndReturn(methodInfo.Name, methodInfo.Invoke(instance,new object[]{methodInfo.GetParameters()[0].}));
//                }
//            }
//            foreach (PropertyInfo property in typeof (T).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)){
//                mock.Clear(property.Name);
//            }
        }
        public static void ClearNonPublicIsolations<T>(this Mock<T> mock) 
        {
            foreach (var property in typeof (T).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance))
                mock.Clear(property.Name);
            foreach (var property in typeof(T).GetProperties(BindingFlags.NonPublic|BindingFlags.Instance)){
                mock.Clear(property.Name);
            }
        }
        public static TrackInstances<T> TrackInstances<T>(this Mock<T> mock) where T : class
        {
            var tracker = new TrackInstances<T> { Count = 0, LastInstance = null };

            mock.ExpectConstructor();
            // we need to catch the constructor,
            //  -> as there is a missing API for dynamic return values for constructors
            //     we will use the MockMethodCalled Event to catch it.
            mock.MockMethodCalled += ((sender, e) =>
                                          {
                                              if (e.CalledMethodName == ".ctor")
                                              {
                                                  tracker.LastInstance = (T)sender;
                                                  tracker.Count++;
                                                  if (tracker.InitializeAction != null)
                                                      tracker.InitializeAction(tracker.LastInstance);
                                              }
                                          });
            return tracker;
        }

        public static void DoAlways<T>(this Mock<T> mock, string method, Action<T> action)
        {
            mock.AlwaysReturn(method, new DynamicReturnValue((p, o) =>
            {
                action((T)o);
                return null;
            }));
        }
        public static void ActAsFields<T>(T fake)
        {
            var mock = MockManager.GetMockOf(fake);
            if (mock == null) throw new Exception("Must be fake");

            foreach (var property in fake.GetType().GetProperties())
            {
                if (property.CanRead && property.CanWrite)
                {
                    object fakePropertyValue = null;
                    mock.AlwaysReturn(property.GetSetMethod().Name,
                        new DynamicReturnValue(
                            (p, o) =>
                            {
                                fakePropertyValue = p[0];
                                return null;
                            }
                            ));
                    mock.AlwaysReturn(property.GetGetMethod().Name,
                        new DynamicReturnValue((p, o) => fakePropertyValue));
                }
            }
        }
    }
}
