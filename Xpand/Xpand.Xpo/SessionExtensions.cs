using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Generators;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace Xpand.Xpo {
    public class PropertyValueStore : List<KeyValuePair<XPMemberInfo, Object>> {
    }


    public static class SessionExtensions {
        public static int GetObjectCount<T>(this Session session, CriteriaOperator criteria = null) {
            return (int)session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);
        }

        public static PropertyValueStore CreatePropertyValueStore(XPClassInfo classInfo, MemberInitExpression memberInitExpression) {
            var propertyValueStore = new PropertyValueStore();

            foreach (var binding in memberInitExpression.Bindings) {
                var assignment = binding as MemberAssignment;
                if (binding == null) {
                    throw new NotImplementedException("All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");
                }

                // Get the memberInfo corresponding to the property name.
                string memberName = binding.Member.Name;
                XPMemberInfo memberInfo = classInfo.GetMember(memberName);
                if (memberInfo == null)
                    throw new ArgumentOutOfRangeException(memberName, String.Format("The member {0} of the {1} class could not be found.", memberName, classInfo.FullName));

                if (!memberInfo.IsPersistent)
                    throw new ArgumentException(memberName, String.Format("The member {0} of the {1} class is not persistent.", memberName, classInfo.FullName));

                // Compile and invoke the assignment expression to obtain the contant value to add as a parameter.
                if (assignment != null) {
                    var constant = Expression.Lambda(assignment.Expression, null).Compile().DynamicInvoke();

                    // Add the 
                    propertyValueStore.Add(new KeyValuePair<XPMemberInfo, Object>(memberInfo, constant));
                }
            }
            return propertyValueStore;
        }

        public static ModificationResult Delete<T>(this Session session, CriteriaOperator criteria) where T : IXPObject {
            if (ReferenceEquals(criteria, null))
                criteria = CriteriaOperator.Parse("True");
            XPClassInfo classInfo = session.GetClassInfo(typeof(T));
            var batchWideData = new BatchWideDataHolder4Modification(session);
            var recordsAffected = (int)session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);
            List<ModificationStatement> collection = DeleteQueryGenerator.GenerateDelete(classInfo, ObjectGeneratorCriteriaSet.GetCommonCriteriaSet(criteria), batchWideData);
            foreach (ModificationStatement item in collection) {
                item.RecordsAffected = recordsAffected;
            }
            ModificationStatement[] collectionToArray = collection.ToArray<ModificationStatement>();
            ModificationResult result = session.DataLayer.ModifyData(collectionToArray);
            return result;
        }

        public static ModificationResult Update<T>(this Session session, Expression<Func<T>> evaluator, CriteriaOperator criteria) where T : IXPObject {
            if (ReferenceEquals(criteria, null))
                criteria = CriteriaOperator.Parse("True");

            XPClassInfo classInfo = session.GetClassInfo(typeof(T));
            var batchWideData = new BatchWideDataHolder4Modification(session);
            var recordsAffected = (int)session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);

            PropertyValueStore propertyValueStore = null;
            int memberInitCount = 1;
            evaluator.Visit<MemberInitExpression>(expression => {
                if (memberInitCount > 1) {
                    throw new NotImplementedException("Only a single MemberInitExpression is allowed for the evaluator parameter.");
                }
                memberInitCount++;
                propertyValueStore = CreatePropertyValueStore(classInfo, expression);
                return expression;
            });

            var properties = new MemberInfoCollection(classInfo, propertyValueStore.Select(x => x.Key).ToArray());

            List<ModificationStatement> collection = UpdateQueryGenerator.GenerateUpdate(classInfo, properties, ObjectGeneratorCriteriaSet.GetCommonCriteriaSet(criteria), batchWideData);
            foreach (UpdateStatement updateStatement in collection.OfType<UpdateStatement>()) {
                for (int i = 0; i < updateStatement.Parameters.Count; i++) {
                    Object value = propertyValueStore[i].Value;
                    if (value is IXPObject)
                        updateStatement.Parameters[i].Value = ((IXPObject)(value)).ClassInfo.GetId(value);
                    else
                        updateStatement.Parameters[i].Value = value;
                }
                updateStatement.RecordsAffected = recordsAffected;
            }
            return session.DataLayer.ModifyData(collection.ToArray<ModificationStatement>());
        }

        public static void UnDelete(this XPBaseObject simpleObject) {
            simpleObject.Session.PurgeDeletedObjects();
        }
        public static bool IsNewObject(this IXPSimpleObject simpleObject) {
            return simpleObject.Session.IsNewObject(simpleObject);
        }
        public static int GetCount(this Session session, Type type, CriteriaOperator criteriaOperator) {
            return (int)session.Evaluate(type, new AggregateOperand("", Aggregate.Count), criteriaOperator);
        }

        public static int GetCount(this Session session, Type type) {
            return GetCount(session, type, null);
        }

        public static int GetCount<TClassType>(this Session session, CriteriaOperator criteriaOperator) {
            return (int)session.Evaluate<TClassType>(new AggregateOperand("", Aggregate.Count), criteriaOperator);
        }

        public static int GetCount<TClassType>(this Session session) {
            return GetCount<TClassType>(session, null);
        }

        public static object GetObject(this Session session, PersistentCriteriaEvaluationBehavior behavior, object o) {
            if (o == null)
                return null;
            XPMemberInfo property = ((PersistentBase)o).ClassInfo.KeyProperty;
            return session.FindObject(behavior, o.GetType(), new BinaryOperator(property.Name, property.GetValue(o)));
        }
        public static object GetObject(this Session session, object o) {
            if (o == null)
                return null;
            return session.GetObjectByKey(o.GetType(), ((PersistentBase)o).ClassInfo.KeyProperty.GetValue(o));
        }

        public static TClassType FindObject<TClassType>(this Session session, PersistentCriteriaEvaluationBehavior persistentCriteriaEvaluationBehavior, Expression<Func<TClassType, bool>> expression) {
            return (TClassType)
                session.FindObject(persistentCriteriaEvaluationBehavior, typeof(TClassType), new XPQuery<TClassType>(session).TransformExpression(expression));
        }

        public static object FindObject<TClassType>(this Session session, Type classType, Expression<Func<TClassType, bool>> expression, bool selectDeleted) {
            return session.FindObject(classType, new XPQuery<TClassType>(session).TransformExpression(expression),
                                      selectDeleted);
        }
        public static TClassType FindObject<TClassType>(this Session session, Expression<Func<TClassType, bool>> expression) {
            return (TClassType)session.FindObject(typeof(TClassType), new XPQuery<TClassType>(session).TransformExpression(expression), false);
        }
    }
}