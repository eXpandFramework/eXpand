using System.Collections.Generic;

namespace TypeMock.Extensions.Fixtures
{
    public class ClassWithProperties
    {
        public int Number { get; set; }

        public List<Product> Products { get; set; }
    }

    public class Product
    {
    }
}
