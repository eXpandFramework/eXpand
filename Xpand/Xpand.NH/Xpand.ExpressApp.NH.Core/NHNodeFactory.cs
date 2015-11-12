using Serialize.Linq;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public class NHNodeFactory : NodeFactory
    {

        public override ExpressionNode Create(Expression expression)
        {
            ConstantExpression constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
                return new ConstantSubstitutionNode(this, constantExpression);

            return base.Create(expression);
        }
    }
}