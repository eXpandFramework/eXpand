namespace Xpand.Tests.Xpand.Utils
{
    internal interface ITransformerExpressionClass {
        string Name { get; set; }
        ITransformerExpressionClass ExpressionClass { get; set; }
    }

    internal class TransformerExpressionClass : ITransformerExpressionClass {
        public string Name { get; set; }

        public ITransformerExpressionClass ExpressionClass { get; set; }
    }
}
