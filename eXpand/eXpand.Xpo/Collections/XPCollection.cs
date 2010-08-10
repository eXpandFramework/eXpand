using System;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    public class XPCollection<T> : DevExpress.Xpo.XPCollection<T>
    {
        public XPCollection(Session session, object theOwner, XPMemberInfo refProperty) : base(session, theOwner, refProperty)
        {
        }

        public XPCollection()
        {
        }

        public XPCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(theCriteria, sortProperties)
        {
        }
        public XPCollection(Expression<Func<T, bool>> theCriteria, params SortProperty[] sortProperties)
            : base(new XPQuery<T>(Session.DefaultSession).TransformExpression(theCriteria), sortProperties)
        {
        }

        public XPCollection(Session session) : base(session)
        {
        }

        public XPCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(session, theCriteria, sortProperties)
        {
        }

        public XPCollection(Session session, bool loadingEnabled) : base(session, loadingEnabled)
        {
        }

        public XPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XPCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XPCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XPCollection(Session session, IEnumerable originalCollection) : base(session, originalCollection)
        {
        }

        public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XPCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XPCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter) : base(originalCollection, filter)
        {
        }
        public XPCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter))
        {
        }

        public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive) : base(originalCollection, filter, caseSensitive)
        {
        }
        public XPCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter, bool caseSensitive)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter), caseSensitive)
        {
        }

        public XPCollection(XPBaseCollection originalCollection) : base(originalCollection)
        {
        }

        public XPCollection(Session session, XPBaseCollection originalCollection) : base(session, originalCollection)
        {
        }

        public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted) : base(criteriaEvaluationBehavior, session, condition, selectDeleted)
        {
        }
        public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T, bool>> condition, bool selectDeleted)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(condition), selectDeleted)
        {
        }

        public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition) : base(criteriaEvaluationBehavior, session, condition)
        {
        }
        public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T,bool>> expression)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(expression))
        {
        }
        public XPCollection(Session session, Expression<Func<T,bool>> expression,params SortProperty[] sortProperties)
            : base(session, new XPQuery<T>(session).TransformExpression(expression),sortProperties)
        {
        }
    }
}