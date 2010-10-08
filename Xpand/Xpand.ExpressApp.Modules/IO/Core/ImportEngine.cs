using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;
using Xpand.Xpo;

namespace Xpand.ExpressApp.IO.Core {
    public class ImportEngine {
        readonly Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object> importedObjecs = new Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object>();

        public int ImportObjects(XDocument document, UnitOfWork unitOfWork) {
            if (document.Root != null){
                foreach (XElement element in document.Root.Nodes().OfType<XElement>()){
                    using (var nestedUnitOfWork = unitOfWork.BeginNestedUnitOfWork()){
                        ITypeInfo typeInfo = GetTypeInfo(element);
                        IEnumerable<XElement> elements = element.Descendants("Property");
                        var xElements = elements.Where(xElement => xElement.GetAttributeValue("isKey").MakeFirstCharUpper() == true.ToString());
                        CriteriaOperator objectKeyCriteria = GetObjectKeyCriteria(typeInfo, xElements);
                        CreateObject(element, nestedUnitOfWork, typeInfo, objectKeyCriteria);
                        nestedUnitOfWork.CommitChanges();
                    }
                }
                unitOfWork.CommitChanges();
            }
            return 0;
        }
        public void ImportObjects(Stream stream, UnitOfWork unitOfWork){
            Guard.ArgumentNotNull(stream,"Stream");
            stream.Position = 0;
            using (var streamReader = new StreamReader(stream)) {
                var xDocument = XDocument.Load(streamReader,LoadOptions.SetLineInfo|LoadOptions.PreserveWhitespace);
                ImportObjects(xDocument, unitOfWork);
            }

        }

        XPBaseObject CreateObject(XElement element, UnitOfWork nestedUnitOfWork, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria){
            XPBaseObject xpBaseObject = GetObject(nestedUnitOfWork, typeInfo,objectKeyCriteria) ;
            var keyValuePair = new KeyValuePair<ITypeInfo, CriteriaOperator>(typeInfo, objectKeyCriteria);
            if (!importedObjecs.ContainsKey(keyValuePair)) {
                importedObjecs.Add(keyValuePair, null);
                ImportProperties(nestedUnitOfWork, xpBaseObject, element);
            }
            return xpBaseObject;
        }

        void ImportProperties(UnitOfWork nestedUnitOfWork, XPBaseObject xpBaseObject, XElement element) {
            ImportSimpleProperties(element, xpBaseObject);
            ImportComplexProperties(element, nestedUnitOfWork,
                                    (o, xElement) =>
                                    xpBaseObject.SetMemberValue(xElement.Parent.GetAttributeValue("name"), o),
                                    NodeType.Object);
            ImportComplexProperties(element, nestedUnitOfWork,
                                    (baseObject, element1) =>
                                    ((IList) xpBaseObject.GetMemberValue(element1.Parent.GetAttributeValue("name"))).Add(
                                        baseObject), NodeType.Collection);
        }

        void ImportComplexProperties(XElement element, UnitOfWork nestedUnitOfWork, Action<XPBaseObject,XElement> instance, NodeType nodeType) {
            IEnumerable<XElement> objectElements = GetObjectRefElements(element,nodeType);
            foreach (var objectElement in objectElements) {
                ITypeInfo typeInfo = GetTypeInfo(objectElement);
                var refObjectKeyCriteria = GetObjectKeyCriteria(typeInfo,objectElement.Descendants("Key"));
                XPBaseObject xpBaseObject;
                if (objectElement.GetAttributeValue("strategy") == SerializationStrategy.SerializeAsObject.ToString()) {
                    var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement();
                    if (findObjectFromRefenceElement != null) {
                        xpBaseObject = CreateObject(findObjectFromRefenceElement, nestedUnitOfWork,
                                                    typeInfo, refObjectKeyCriteria);
                        instance.Invoke(xpBaseObject, objectElement);
                    }
                }
                else {
                    xpBaseObject = GetObject(nestedUnitOfWork, typeInfo, refObjectKeyCriteria);
                    instance.Invoke(xpBaseObject, objectElement);
                }
                
            }
        }




        IEnumerable<XElement> GetObjectRefElements(XElement element, NodeType nodeType) {
            return element.Properties(nodeType).SelectMany(
                element1 => element1.Descendants("SerializedObjectRef"));
        }

        void ImportSimpleProperties(XElement element, XPBaseObject xpBaseObject) {            
            foreach (var simpleElement in element.Properties(NodeType.Simple)){
                string propertyName = simpleElement.GetAttributeValue("name");
                XPMemberInfo xpMemberInfo = xpBaseObject.ClassInfo.FindMember(propertyName);
                if (xpMemberInfo != null) {
                    object value = GetValue(simpleElement, xpMemberInfo);
                    xpBaseObject.SetMemberValue(propertyName, value);
                }
            }
        }

        object GetValue(XElement simpleElement, XPMemberInfo xpMemberInfo) {
            var valueConverter = xpMemberInfo.Converter;
            
            if (valueConverter != null) {
                var value = GetValue(valueConverter.StorageType, simpleElement);
                return valueConverter.ConvertFromStorageType(value);
            }
            return GetValue(GetMemberType(xpMemberInfo), simpleElement);
        }

        Type GetMemberType(XPMemberInfo xpMemberInfo) {
            return xpMemberInfo is ServiceField
                       ? typeof (Nullable<>).MakeGenericType(new[] {xpMemberInfo.MemberType})
                       : xpMemberInfo.MemberType;
        }

        object GetValue(Type type, XElement simpleElement) {
            if (type == typeof(byte[])){
                return string.IsNullOrEmpty(simpleElement.Value) ? null : Convert.FromBase64String(simpleElement.Value);
            }
            return XpandReflectionHelper.ChangeType(simpleElement.Value, type);
        }

        ITypeInfo GetTypeInfo(XElement element) {
            return ReflectionHelper.FindTypeInfoByName(element.GetAttributeValue("type"));
        }

        XPBaseObject GetObject(UnitOfWork unitOfWork, ITypeInfo typeInfo, CriteriaOperator criteriaOperator) {
            var xpBaseObject = unitOfWork.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, unitOfWork.GetClassInfo(typeInfo.Type),
                                                     criteriaOperator,true) as XPBaseObject ??
                               unitOfWork.FindObject(unitOfWork.GetClassInfo(typeInfo.Type), criteriaOperator,true) as XPBaseObject;
            return xpBaseObject ?? (XPBaseObject)ReflectionHelper.CreateObject(typeInfo.Type, unitOfWork);
        }

        CriteriaOperator GetObjectKeyCriteria(ITypeInfo typeInfo, IEnumerable<XElement> xElements)
        {
            string criteria = "";
            var parameters=new List<object>();
            foreach (var xElement in xElements) {
                var name = xElement.GetAttributeValue("name");
                parameters.Add(XpandReflectionHelper.ChangeType(xElement.Value, typeInfo.FindMember(name).MemberType));
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