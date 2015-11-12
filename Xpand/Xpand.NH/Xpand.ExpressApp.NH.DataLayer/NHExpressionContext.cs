using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Mapping;
using System.Collections;
using Xpand.ExpressApp.NH.Core;
using DevExpress.Utils;
using System.Globalization;
using DevExpress.Data.Filtering;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace Xpand.ExpressApp.NH.DataLayer
{
    public class NHExpressionContext : Serialize.Linq.ExpressionContext, ISubstitutionExpressionContext
    {
        private readonly ISession session;

        public NHExpressionContext(ISession session)
        {
            this.session = session;
        }

        public override System.Linq.Expressions.ParameterExpression GetParameterExpression(Serialize.Linq.Nodes.ParameterExpressionNode node)
        {
            return base.GetParameterExpression(node);
        }

        public override Type ResolveType(Serialize.Linq.Nodes.TypeNode node)
        {
            if (node.Name == typeof(RemoteObjectQuery<>).FullName)
            {
                return typeof(NhQueryable<>);
            }
            return base.ResolveType(node);
        }


        public Expression Substitute(ConstantSubstitutionNode node)
        {
            if (node.Type.Name == typeof(RemoteObjectQuery<>).FullName && node.Type.GenericArguments.Length == 1)
            {
                Type genericArgumentType = node.Type.GenericArguments[0].ToType(this);
                Type queryableType = typeof(NhQueryable<>).MakeGenericType(genericArgumentType);

                return Expression.Constant(Activator.CreateInstance(queryableType, session.GetSessionImplementation()),
                    queryableType);
            }
            else
                return null;
        }
    }
}
