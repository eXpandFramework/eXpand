using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;
using eXpand.Xpo;

namespace eXpand.ExpressApp.IO.Core {
    public class ImportEngine {
        readonly List<object> importingObjecs=new List<object>();

        public int ImportObjects(Stream stream, ObjectSpace objectSpace)
        {
            var unitOfWork = ((UnitOfWork) objectSpace.Session);
            stream.Position = 0;
            using (var streamReader = new StreamReader(stream)) {
                var xDocument = XDocument.Load(streamReader);
                if (xDocument.Root != null) {
                    foreach (XElement element in xDocument.Root.Nodes()) {
                        using (var nestedUnitOfWork = unitOfWork.BeginNestedUnitOfWork()) {
                            ITypeInfo typeInfo = GetTypeInfo(element);
                            IEnumerable<XElement> elements = element.Descendants("Property");
                            var xElements = elements.Where(xElement => xElement.GetAttributeValue("isKey").MakeFirstCharUpper() == true.ToString());
                            CriteriaOperator objectKeyCriteria = getObjectKeyCriteria(typeInfo, xElements);
                            createObject(element, nestedUnitOfWork,typeInfo, objectKeyCriteria);
                            nestedUnitOfWork.CommitChanges();
                        }
                    }
                    unitOfWork.CommitChanges();
                }
            }
            return 0;
        }

        XPBaseObject createObject(XElement element, UnitOfWork nestedUnitOfWork, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria)
        {
            XPBaseObject xpBaseObject = findObject(nestedUnitOfWork, typeInfo,objectKeyCriteria) ??
                                        (XPBaseObject) Activator.CreateInstance(typeInfo.Type, nestedUnitOfWork);
            if (!importingObjecs.Contains(xpBaseObject)) {
                importingObjecs.Add(xpBaseObject);
                importProperties(nestedUnitOfWork, xpBaseObject, element);
                importingObjecs.Remove(xpBaseObject);
            }
            return xpBaseObject;
        }

        void importProperties(UnitOfWork nestedUnitOfWork, XPBaseObject xpBaseObject, XElement element) {
            importSimpleProperties(element, xpBaseObject);
            importComplexProperties(element, nestedUnitOfWork,
                                    (o, xElement) =>
                                    xpBaseObject.SetMemberValue(xElement.Parent.GetAttributeValue("name"), o),
                                    NodeType.Object);
            importComplexProperties(element, nestedUnitOfWork,
                                    (baseObject, element1) =>
                                    ((IList) xpBaseObject.GetMemberValue(element1.Parent.GetAttributeValue("name"))).Add(
                                        baseObject), NodeType.Collection);
        }

        void importComplexProperties(XElement element, UnitOfWork nestedUnitOfWork, Action<XPBaseObject,XElement> instance, NodeType nodeType) {
            IEnumerable<XElement> objectElements = GetObjectRefElements(element,nodeType);
            foreach (var objectElement in objectElements) {
                ITypeInfo typeInfo = GetTypeInfo(objectElement);
                var refObjectKeyCriteria = getObjectKeyCriteria(typeInfo,objectElement.Descendants("Key"));
                XPBaseObject xpBaseObject;
                if (objectElement.GetAttributeValue("strategy") == SerializationStrategy.SerializeAsObject.ToString()) {
                    var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement(false);
                    if (findObjectFromRefenceElement != null) {
                        xpBaseObject = createObject(findObjectFromRefenceElement, nestedUnitOfWork,
                                                    typeInfo, refObjectKeyCriteria);
                        instance.Invoke(xpBaseObject, objectElement);
                    }
                }
                else {
                    xpBaseObject = findObject(nestedUnitOfWork, typeInfo, refObjectKeyCriteria);
                    instance.Invoke(xpBaseObject, objectElement);
                }
                
            }
        }




        IEnumerable<XElement> GetObjectRefElements(XElement element, NodeType nodeType) {
            return element.Descendants("Property").Where(
                xElement => xElement.GetAttributeValue("type") == nodeType.ToString().MakeFirstCharLower()).SelectMany(
                element1 => element1.Descendants("SerializedObjectRef"));
        }

        void importSimpleProperties(XElement element, XPBaseObject xpBaseObject) {
            IEnumerable<XElement> simpleElements =
                element.Descendants("Property").Where(
                    xElement => xElement.GetAttributeValue("type") == NodeType.Simple.ToString().MakeFirstCharLower());
            foreach (var simpleElement in simpleElements) {
                string propertyName = simpleElement.GetAttributeValue("name");
                XPMemberInfo xpMemberInfo = xpBaseObject.ClassInfo.GetMember(propertyName);
                object value = GetValue(simpleElement, xpMemberInfo);
                if (simpleElement.GetAttributeValue("isNaturalKey")=="true"&&!xpBaseObject.IsNewObject())
                    continue;
                xpBaseObject.SetMemberValue(propertyName, value);
            }
        }

        object GetValue(XElement simpleElement, XPMemberInfo xpMemberInfo) {
            var valueConverter = xpMemberInfo.Converter;
            return valueConverter != null
                       ? valueConverter.ConvertFromStorageType(ReflectionHelper.Convert(simpleElement.Value, valueConverter.StorageType))
                       : ReflectionHelper.Convert(simpleElement.Value, xpMemberInfo.MemberType);
        }

        ITypeInfo GetTypeInfo(XElement element) {
            return ReflectionHelper.FindTypeInfoByName(element.GetAttributeValue("type"));
        }

        XPBaseObject findObject(UnitOfWork unitOfWork, ITypeInfo typeInfo, CriteriaOperator criteriaOperator) {
            var xpBaseObject = unitOfWork.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, typeInfo.Type,
                                                     criteriaOperator) as XPBaseObject;
            if (xpBaseObject == null) {
                xpBaseObject = unitOfWork.FindObject(typeInfo.Type, criteriaOperator, true) as XPBaseObject;
                if (xpBaseObject != null && xpBaseObject.IsDeleted) {
                    xpBaseObject.UnDelete();
                }
            }
            return xpBaseObject;
        }

        CriteriaOperator getObjectKeyCriteria(ITypeInfo typeInfo, IEnumerable<XElement> xElements)
        {
            string criteria = "";
            var parameters=new List<object>();
            foreach (var xElement in xElements) {
                var name = xElement.GetAttributeValue("name");
                parameters.Add(ReflectorHelper.ChangeType(xElement.Value, typeInfo.FindMember(name).MemberType));
                criteria += name + "=? AND ";
            }
            return CriteriaOperator.Parse(criteria.TrimEnd("AND ".ToCharArray()),parameters.ToArray());
        }
    }
}