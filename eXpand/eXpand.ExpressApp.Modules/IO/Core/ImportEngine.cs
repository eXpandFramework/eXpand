using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;
using eXpand.Xpo;

namespace eXpand.ExpressApp.IO.Core {
    public class ImportEngine {
        readonly Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object> importedObjecs = new Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object>();

        public int ImportObjects(XDocument document, UnitOfWork unitOfWork) {
            if (document.Root != null){
                foreach (XElement element in document.Root.Nodes().OfType<XElement>()){
                    using (var nestedUnitOfWork = unitOfWork.BeginNestedUnitOfWork()){
                        ITypeInfo typeInfo = GetTypeInfo(element);
                        IEnumerable<XElement> elements = element.Descendants("Property");
                        var xElements = elements.Where(xElement => xElement.GetAttributeValue("isKey").MakeFirstCharUpper() == true.ToString());
                        CriteriaOperator objectKeyCriteria = getObjectKeyCriteria(typeInfo, xElements);
                        createObject(element, nestedUnitOfWork, typeInfo, objectKeyCriteria);
                        nestedUnitOfWork.CommitChanges();
                    }
                }
                unitOfWork.CommitChanges();
            }
            return 0;
        }
        public void ImportObjects(Stream stream, UnitOfWork unitOfWork){
            unitOfWork.PurgeDeletedObjects();
            stream.Position = 0;
            using (var streamReader = new StreamReader(stream)) {
                var xDocument = XDocument.Load(streamReader);
                ImportObjects(xDocument, unitOfWork);
            }

        }

        XPBaseObject createObject(XElement element, UnitOfWork nestedUnitOfWork, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria){
            XPBaseObject xpBaseObject = getObject(nestedUnitOfWork, typeInfo,objectKeyCriteria) ;
            var keyValuePair = new KeyValuePair<ITypeInfo, CriteriaOperator>(typeInfo, objectKeyCriteria);
            if (!importedObjecs.ContainsKey(keyValuePair)) {
                importedObjecs.Add(keyValuePair, null);
                importProperties(nestedUnitOfWork, xpBaseObject, element);
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
                    var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement();
                    if (findObjectFromRefenceElement != null) {
                        xpBaseObject = createObject(findObjectFromRefenceElement, nestedUnitOfWork,
                                                    typeInfo, refObjectKeyCriteria);
                        instance.Invoke(xpBaseObject, objectElement);
                    }
                }
                else {
                    xpBaseObject = getObject(nestedUnitOfWork, typeInfo, refObjectKeyCriteria);
                    instance.Invoke(xpBaseObject, objectElement);
                }
                
            }
        }




        IEnumerable<XElement> GetObjectRefElements(XElement element, NodeType nodeType) {
            return element.Properties(nodeType).SelectMany(
                element1 => element1.Descendants("SerializedObjectRef"));
        }

        void importSimpleProperties(XElement element, XPBaseObject xpBaseObject) {            
            foreach (var simpleElement in element.Properties(NodeType.Simple)){
                string propertyName = simpleElement.GetAttributeValue("name");
                XPMemberInfo xpMemberInfo = xpBaseObject.ClassInfo.GetMember(propertyName);
                object value = GetValue(simpleElement, xpMemberInfo);
                xpBaseObject.SetMemberValue(propertyName, value);
            }
        }

        object GetValue(XElement simpleElement, XPMemberInfo xpMemberInfo) {
            var valueConverter = xpMemberInfo.Converter;
            
            if (valueConverter != null) {
                var value = GetValue(valueConverter.StorageType, simpleElement);
                return valueConverter.ConvertFromStorageType(value);
            }
            return GetValue(xpMemberInfo.MemberType, simpleElement);
        }

        object GetValue(Type type, XElement simpleElement) {
            if (type == typeof(byte[])){
                return string.IsNullOrEmpty(simpleElement.Value) ? null : Convert.FromBase64String(simpleElement.Value);
            }
            return ReflectorHelper.ChangeType(simpleElement.Value, type);
        }

        ITypeInfo GetTypeInfo(XElement element) {
            return ReflectionHelper.FindTypeInfoByName(element.GetAttributeValue("type"));
        }

        XPBaseObject getObject(UnitOfWork unitOfWork, ITypeInfo typeInfo, CriteriaOperator criteriaOperator) {
            var xpBaseObject = unitOfWork.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, typeInfo.Type,
                                                     criteriaOperator) as XPBaseObject;
            if (xpBaseObject == null) {
                xpBaseObject = unitOfWork.FindObject(typeInfo.Type, criteriaOperator) as XPBaseObject;
                if (xpBaseObject != null && xpBaseObject.IsDeleted) {
                    xpBaseObject.UnDelete();
                }
            }
            return xpBaseObject ?? (XPBaseObject)ReflectionHelper.CreateObject(typeInfo.Type, unitOfWork);
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

        public void ImportObjects(UnitOfWork unitOfWork,string fileName) {
            using (var fileStream = new FileStream(fileName,FileMode.Open)) {
                 ImportObjects(fileStream,unitOfWork);
            }
        }

        public void ImportObjects(UnitOfWork unitOfWork, Type nameSpaceType, string resourceName) {
            Stream manifestResourceStream = nameSpaceType.Assembly.GetManifestResourceStream(nameSpaceType,resourceName);
            ImportObjects(manifestResourceStream, unitOfWork);
        }
    }
}