using System;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Collections
{
    public class XpandXPCollection<T> : XPCollection<T>
    {
        public XpandXPCollection(Session session, object theOwner, XPMemberInfo refProperty) : base(session, theOwner, refProperty)
        {
        }

        public XpandXPCollection()
        {
        }

        public XpandXPCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(theCriteria, sortProperties)
        {
        }
        public XpandXPCollection(Expression<Func<T, bool>> theCriteria, params SortProperty[] sortProperties)
            : base(new XPQuery<T>(Session.DefaultSession).TransformExpression(theCriteria), sortProperties)
        {
        }

        public XpandXPCollection(Session session) : base(session)
        {
        }

        public XpandXPCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(session, theCriteria, sortProperties)
        {
        }

        public XpandXPCollection(Session session, bool loadingEnabled) : base(session, loadingEnabled)
        {
        }

        public XpandXPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XpandXPCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XpandXPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XpandXPCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XpandXPCollection(Session session, IEnumerable originalCollection) : base(session, originalCollection)
        {
        }

        public XpandXPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XpandXPCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XpandXPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XpandXPCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XpandXPCollection(XPBaseCollection originalCollection, CriteriaOperator filter) : base(originalCollection, filter)
        {
        }
        public XpandXPCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter))
        {
        }

        public XpandXPCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive) : base(originalCollection, filter, caseSensitive)
        {
        }
        public XpandXPCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter, bool caseSensitive)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter), caseSensitive)
        {
        }

        public XpandXPCollection(XPBaseCollection originalCollection) : base(originalCollection)
        {
        }

        public XpandXPCollection(Session session, XPBaseCollection originalCollection) : base(session, originalCollection)
        {
        }

        public XpandXPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted) : base(criteriaEvaluationBehavior, session, condition, selectDeleted)
        {
        }
        public XpandXPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T, bool>> condition, bool selectDeleted)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(condition), selectDeleted)
        {
        }

        public XpandXPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition) : base(criteriaEvaluationBehavior, session, condition)
        {
        }
        public XpandXPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T,bool>> expression)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(expression))
        {
        }
        public XpandXPCollection(Session session, Expression<Func<T,bool>> expression,params SortProperty[] sortProperties)
            : base(session, new XPQuery<T>(session).TransformExpression(expression),sortProperties)
        {
        }
    }
}