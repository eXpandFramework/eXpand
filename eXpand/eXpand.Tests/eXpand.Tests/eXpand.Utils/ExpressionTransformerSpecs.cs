using System;
using System.Linq.Expressions;
using eXpand.Utils.Linq;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.Utils
{
    [Subject(typeof(ExpressionTransformer),"Transform")]
    public class When_expression_type_is_not_equal {
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            _expression = info => info.Name != "test";
        };

        Because of = () =>
        {
            _transform = new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression) as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_member_expression = () => _transform.Left.ShouldBeOfType(typeof(MemberExpression));
        It should_have_as_type_of_the_expression_of_the_memberexpression_the_transformarion_type = () => ((MemberExpression)_transform.Left).Expression.Type.ShouldEqual(typeof(TransformerExpressionClass));
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_type_is_equal
    {
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () => {
            _expression = info => info.Name == "test" ;
        };

        Because of = () => {
            _transform = new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression) as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_member_expression = () => _transform.Left.ShouldBeOfType(typeof (MemberExpression));
        It should_have_as_type_of_the_expression_of_the_memberexpression_the_transformarion_type = () =>((MemberExpression) _transform.Left).Expression.Type.ShouldEqual(typeof(TransformerExpressionClass));
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_type_is_and_also {
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            _expression = info => info.Name == "test" && info.Name == "test";
        };

        Because of = () =>
        {
            _transform = new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression) as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
        It should_have_as_right_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_type_is_or_else {
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            _expression = info => info.Name == "test" || info.Name == "test";
        };

        Because of = () =>
        {
            _transform = new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression) as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
        It should_have_as_right_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
    }
}
