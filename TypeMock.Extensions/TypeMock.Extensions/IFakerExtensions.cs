using TypeMock.ArrangeActAssert;

namespace TypeMock.Extensions
{
    public static class IFakerExtensions
    {
        public static T BaseCallsIsolated<T>(this IFaker faker){
            var instance = faker.Instance<T>(Members.CallOriginal,ConstructorWillBe.Called);
            MockManager.GetMockOf(instance).IsolateBaseCalls();
            return instance;
        }
        public static T InstanceWithNonPublicNonIsolated<T>(this IFaker faker,Members behaviour,ConstructorWillBe contructorBehaviour,params object[] constructorArguments){
            var instance = faker.Instance<T>(behaviour,contructorBehaviour,constructorArguments);
            MockManager.GetMockOf(instance).ClearNonPublicIsolations();
            return instance;
        }
        public static T InstanceAndSwapAll<T>(this IFaker faker, Members behaviour, ConstructorWillBe contructorBehaviour, params object[] constructorArguments)
        {
            var instance = Isolate.Fake.Instance<T>(behaviour,contructorBehaviour,constructorArguments);
            Isolate.Swap.AllInstances<T>().With(instance);
            return instance;
        }
        public static T InstanceAndSwapAll<T>(this IFaker faker, Members behaviour)
        {
            return InstanceAndSwapAll<T>(faker,behaviour,ConstructorWillBe.Ignored);
        }

        public static T InstanceAndSwapAll<T>(this IFaker faker){
            return InstanceAndSwapAll<T>(faker,Members.ReturnRecursiveFakes);
        }

        public static T InstanceWithNonPublicNonIsolated<T>(this IFaker faker)
        {
            return InstanceWithNonPublicNonIsolated<T>(faker,Members.ReturnRecursiveFakes,ConstructorWillBe.Ignored);
        }

    }
}
