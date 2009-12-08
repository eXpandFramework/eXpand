namespace TypeMock.Extensions.Fixtures
{
    public class DummyClass:BaseDummyClass
    {
        
        public void DummyMethod()
        {
            


        }

        public override int PublicMethodWithBaseCall(){
            int call = base.PublicMethodWithBaseCall();
            return call+1;
        }

        private int publicProperty;
        public int PublicProperty
        {
            get { return PrivateMethod(); }
            set
            {
                publicProperty = value;
            }
        }
        public int PublicMethod()
        {
            return PrivateMethod();
        }
        private int PrivateMethod()
        {
            return 2;
        }
    }

    public class BaseDummyClass{
        public virtual int PublicMethodWithBaseCall()
        {
            return 1;
        }
    }
}
