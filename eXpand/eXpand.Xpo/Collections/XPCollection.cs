using System;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    public class XpandXpCollection<T> : XPCollection<T>
    {
        public XpandXpCollection(Session session, object theOwner, XPMemberInfo refProperty) : base(session, theOwner, refProperty)
        {
        }

        public XpandXpCollection()
        {
        }

        public XpandXpCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(theCriteria, sortProperties)
        {
        }
        public XpandXpCollection(Expression<Func<T, bool>> theCriteria, params SortProperty[] sortProperties)
            : base(new XPQuery<T>(Session.DefaultSession).TransformExpression(theCriteria), sortProperties)
        {
        }

        public XpandXpCollection(Session session) : base(session)
        {
        }

        public XpandXpCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(session, theCriteria, sortProperties)
        {
        }

        public XpandXpCollection(Session session, bool loadingEnabled) : base(session, loadingEnabled)
        {
        }

        public XpandXpCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XpandXpCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XpandXpCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XpandXpCollection(Session session, IEnumerable originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XpandXpCollection(Session session, IEnumerable originalCollection) : base(session, originalCollection)
        {
        }

        public XpandXpCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }
        public XpandXpCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter, bool caseSensitive)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter), caseSensitive)
        {
        }

        public XpandXpCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }
        public XpandXpCollection(Session session, XPBaseCollection originalCollection, Expression<Func<T, bool>> copyFilter)
            : base(session, originalCollection, new XPQuery<T>(session).TransformExpression(copyFilter))
        {
        }

        public XpandXpCollection(XPBaseCollection originalCollection, CriteriaOperator filter) : base(originalCollection, filter)
        {
        }
        public XpandXpCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter))
        {
        }

        public XpandXpCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive) : base(originalCollection, filter, caseSensitive)
        {
        }
        public XpandXpCollection(XPBaseCollection originalCollection, Expression<Func<T, bool>> filter, bool caseSensitive)
            : base(originalCollection, new XPQuery<T>(originalCollection.Session).TransformExpression(filter), caseSensitive)
        {
        }

        public XpandXpCollection(XPBaseCollection originalCollection) : base(originalCollection)
        {
        }

        public XpandXpCollection(Session session, XPBaseCollection originalCollection) : base(session, originalCollection)
        {
        }

        public XpandXpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted) : base(criteriaEvaluationBehavior, session, condition, selectDeleted)
        {
        }
        public XpandXpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T, bool>> condition, bool selectDeleted)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(condition), selectDeleted)
        {
        }

        public XpandXpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition) : base(criteriaEvaluationBehavior, session, condition)
        {
        }
        public XpandXpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T,bool>> expression)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(expression))
        {
        }
        public XpandXpCollection(Session session, Expression<Func<T,bool>> expression,params SortProperty[] sortProperties)
            : base(session, new XPQuery<T>(session).TransformExpression(expression),sortProperties)
        {
        }
    }
}