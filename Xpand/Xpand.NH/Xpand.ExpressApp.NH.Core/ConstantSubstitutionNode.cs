using Serialize.Linq;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    [Serializable]
    [DataContract]
    public class ConstantSubstitutionNode : ConstantExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantSubstitutionNode"/> class.
        /// </summary>
        public ConstantSubstitutionNode()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantSubstitutionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="value">The value.</param>
        public ConstantSubstitutionNode(INodeFactory factory, object value)
            : base(factory, value)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantSubstitutionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public ConstantSubstitutionNode(INodeFactory factory, object value, Type type)
            : base(factory, value, type)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantSubstitutionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="expression">The expression.</param>
        public ConstantSubstitutionNode(INodeFactory factory, ConstantExpression expression)
            : base(factory, expression)
        {

        }

        public override Expression ToExpression(ExpressionContext context)
        {
            ISubstitutionExpressionContext substitutionContext = context as ISubstitutionExpressionContext;
            if (context != null)
            {
                var result = substitutionContext.Substitute(this);
                
                if (result != null)
                    return result;
            }

            return base.ToExpression(context);
        }
    }
}
