using System;
using System.Linq.Expressions;
using eXpand.Utils.Linq;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.Utils
{
    [Subject(typeof(ExpressionTransformer),"Transform")]
    public class When_expression_type_is_not_equal {
        static LambdaExpression _lambdaExpression;
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            _expression = info => info.Name != "test";
        };

        Because of = () => {
            _lambdaExpression = ((LambdaExpression) new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression));
            _transform =_lambdaExpression.Body as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_member_expression = () => _transform.Left.ShouldBeOfType(typeof(MemberExpression));
        It should_have_as_type_of_the_expression_of_the_memberexpression_the_transformarion_type = () => ((MemberExpression)_transform.Left).Expression.Type.ShouldEqual(typeof(TransformerExpressionClass));

        It should_have_as_expression_of_the_memberexpression_the_parameter_of_the_passed_in_lamda =
            () => _lambdaExpression.Parameters[0].ShouldEqual(((MemberExpression) _transform.Left).Expression);
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_type_is_equal
    {
        static LambdaExpression _lambdaExpression;
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () => {
            _expression = info => info.Name == "test" ;
        };

        Because of = () => {
            _lambdaExpression = ((LambdaExpression)new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression));
            _transform = _lambdaExpression.Body as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_member_expression = () => _transform.Left.ShouldBeOfType(typeof (MemberExpression));
        It should_have_as_type_of_the_expression_of_the_memberexpression_the_transformarion_type = () =>((MemberExpression) _transform.Left).Expression.Type.ShouldEqual(typeof(TransformerExpressionClass));

        It should_have_as_expression_of_the_memberexpression_the_parameter_of_the_passed_in_lamda =
            () => _lambdaExpression.Parameters[0].ShouldEqual(((MemberExpression)_transform.Left).Expression);
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_type_is_and_also {
        static LambdaExpression _lambdaExpression;
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            _expression = info => info.Name == "test" && info.Name == "test";
        };

        Because of = () => {
            _lambdaExpression = ((LambdaExpression)new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression));
            _transform = _lambdaExpression.Body as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
        It should_have_as_right_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));

        It should_have_as_expression_of_the_left_memberexpression_the_parameter_of_the_passed_in_lamda =
            () => _lambdaExpression.Parameters[0].ShouldEqual(((MemberExpression) ((BinaryExpression)_transform.Left).Left).Expression);
        It should_have_as_expression_of_the_right_memberexpression_the_parameter_of_the_passed_in_lamda =
            () => _lambdaExpression.Parameters[0].ShouldEqual(((MemberExpression) ((BinaryExpression)_transform.Right).Left).Expression);
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
            _transform = ((LambdaExpression)new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression)).Body as BinaryExpression;
        };

        It should_return_a_binary_expression = () => _transform.ShouldNotBeNull();

        It should_have_as_left_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
        It should_have_as_right_a_binary_expression = () => _transform.Left.ShouldBeOfType(typeof(BinaryExpression));
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    public class When_expression_is_null {
        static Expression _expression;

        Because of = () =>
        {
            _expression = new ExpressionTransformer().Transform(typeof(TransformerExpressionClass),null);
        };

        It should_return_null_as_expression = () => _expression.ShouldBeNull();
    }
    [Subject(typeof(ExpressionTransformer), "Transform")]
    internal class When_expression_right_member_is_not_constant {
        static BinaryExpression _transform;
        static Expression<Func<ITransformerExpressionClass, bool>> _expression;

        Establish context = () =>
        {
            var transformerExpressionClass = new TransformerExpressionClass();
            _expression = info => info.ExpressionClass == transformerExpressionClass;
        };

        Because of = () =>{
            _transform = ((LambdaExpression)new ExpressionTransformer().Transform(typeof(TransformerExpressionClass), _expression)).Body as BinaryExpression;
        };
        It should_not_be_transformed = () => ((MemberExpression)_transform.Right).Member.Name.ShouldEqual("transformerExpressionClass");
    }
}
