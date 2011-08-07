using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;
using Xpand.Xpo;

namespace Xpand.ExpressApp.IO.Core {
    public class ImportEngine {
        readonly Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object> importedObjecs = new Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object>();
        UnitOfWork _unitOfWork;
        readonly bool _createErrorObjects;

        public ImportEngine(bool createErrorObjects) {
            _createErrorObjects = createErrorObjects;
        }

        public ImportEngine() {
        }

        public int ImportObjects(XDocument document, UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            if (document.Root != null) {
                foreach (XElement element in document.Root.Nodes().OfType<XElement>()) {
                    using (var nestedUnitOfWork = unitOfWork.BeginNestedUnitOfWork()) {
                        ITypeInfo typeInfo = GetTypeInfo(element);
                        if (typeInfo != null) {
                            IEnumerable<XElement> elements = element.Descendants("Property");
                            var xElements =
                                elements.Where(
                                    xElement =>
                                    xElement.GetAttributeValue("isKey").MakeFirstCharUpper() == true.ToString());
                            CriteriaOperator objectKeyCriteria = GetObjectKeyCriteria(typeInfo, xElements);
                            CreateObject(element, nestedUnitOfWork, typeInfo, objectKeyCriteria);
                            nestedUnitOfWork.CommitChanges();
                        }
                    }
                }
                unitOfWork.CommitChanges();
            }
            return 0;
        }
        public void ImportObjects(Stream stream, UnitOfWork unitOfWork) {
            Guard.ArgumentNotNull(stream, "Stream");
            stream.Position = 0;
            using (var streamReader = new StreamReader(stream)) {
                var xDocument = XDocument.Load(streamReader, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
                ImportObjects(xDocument, unitOfWork);
            }

        }

        XPBaseObject CreateObject(XElement element, UnitOfWork nestedUnitOfWork, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria) {
            XPBaseObject xpBaseObject = GetObject(nestedUnitOfWork, typeInfo, objectKeyCriteria);
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
                                    ((IList)xpBaseObject.GetMemberValue(element1.Parent.GetAttributeValue("name"))).Add(
                                        baseObject), NodeType.Collection);
        }

        private void HandleErrorComplex(XElement objectElement, ITypeInfo typeInfo, Action action) {
            var memberInfo = typeInfo.FindMember(objectElement.Parent.GetAttributeValue("name"));
            if (memberInfo != null) {
                action.Invoke();
            } else {
                HandleError(objectElement, FailReason.PropertyNotFound);
            }
        }
        void ImportComplexProperties(XElement element, UnitOfWork nestedUnitOfWork, Action<XPBaseObject, XElement> instance, NodeType nodeType) {
            IEnumerable<XElement> objectElements = GetObjectRefElements(element, nodeType);
            ITypeInfo typeInfo = GetTypeInfo(element);
            foreach (XElement objectElement in objectElements) {
                ITypeInfo memberTypeInfo = GetTypeInfo(objectElement);
                if (memberTypeInfo != null) {
                    var refObjectKeyCriteria = GetObjectKeyCriteria(memberTypeInfo, objectElement.Descendants("Key"));
                    XPBaseObject xpBaseObject;
                    XElement element1 = objectElement;
                    if (objectElement.GetAttributeValue("strategy") ==
                        SerializationStrategy.SerializeAsObject.ToString()) {
                        var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement();
                        if (findObjectFromRefenceElement != null) {
                            HandleErrorComplex(objectElement, typeInfo, () => {
                                xpBaseObject = CreateObject(findObjectFromRefenceElement, nestedUnitOfWork, memberTypeInfo, refObjectKeyCriteria);
                                instance.Invoke(xpBaseObject, element1);
                            });
                        }
                    } else {
                        HandleErrorComplex(objectElement, typeInfo, () => {
                            xpBaseObject = GetObject(nestedUnitOfWork, memberTypeInfo, refObjectKeyCriteria);
                            instance.Invoke(xpBaseObject, element1);
                        });
                    }
                }
            }
        }


        void HandleError(XElement element, FailReason failReason) {
            string innerXml = null;
            string elementXml;
            var firstOrDefault = element.Ancestors("SerializedObject").FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault != element) {
                innerXml = element.ToString();
                elementXml = firstOrDefault.ToString();
            } else {
                elementXml = element.ToString();
            }
            if (_createErrorObjects) {
                var errorInfoObject =
                    (IIOError)Activator.CreateInstance(XafTypesInfo.Instance.FindBussinessObjectType<IIOError>(), _unitOfWork);
                errorInfoObject.Reason = failReason;
                errorInfoObject.ElementXml = elementXml;
                errorInfoObject.InnerXml = innerXml;
            } else {
                throw new UserFriendlyException(new Exception("ImportFailed", new Exception("ELEMENTXML=" + elementXml + " INNERXML=" + innerXml)));
            }
        }


        IEnumerable<XElement> GetObjectRefElements(XElement element, NodeType nodeType) {
            return element.Properties(nodeType).SelectMany(
                element1 => element1.Descendants("SerializedObjectRef"));
        }

        void ImportSimpleProperties(XElement element, XPBaseObject xpBaseObject) {
            foreach (XElement simpleElement in element.Properties(NodeType.Simple)) {
                string propertyName = simpleElement.GetAttributeValue("name");
                XPMemberInfo xpMemberInfo = xpBaseObject.ClassInfo.FindMember(propertyName);
                if (xpMemberInfo != null) {
                    object value = GetValue(simpleElement, xpMemberInfo);
                    xpBaseObject.SetMemberValue(propertyName, value);
                } else {
                    HandleError(simpleElement, FailReason.PropertyNotFound);
                }
            }
        }

        object GetValue(XElement simpleElement, XPMemberInfo xpMemberInfo) {
            var valueConverter = xpMemberInfo.Converter;
            if (valueConverter != null && !(valueConverter is EnumsConverter)) {
                var value = GetValue(valueConverter.StorageType, simpleElement);
                return valueConverter.ConvertFromStorageType(value);
            }
            return GetValue(GetMemberType(xpMemberInfo), simpleElement);
        }

        Type GetMemberType(XPMemberInfo xpMemberInfo) {
            return xpMemberInfo is ServiceField
                       ? typeof(Nullable<>).MakeGenericType(new[] { xpMemberInfo.MemberType })
                       : xpMemberInfo.MemberType;
        }

        object GetValue(Type type, XElement simpleElement) {
            if (type == typeof(byte[])) {
                return string.IsNullOrEmpty(simpleElement.Value) ? null : Convert.FromBase64String(simpleElement.Value);
            }
            return XpandReflectionHelper.ChangeType(simpleElement.Value, type, CultureInfo.InvariantCulture);
        }

        ITypeInfo GetTypeInfo(XElement element) {
            var typeInfo = ReflectionHelper.FindTypeInfoByName(element.GetAttributeValue("type"));
            if (typeInfo == null)
                HandleError(element, FailReason.TypeNotFound);
            return typeInfo;
        }

        XPBaseObject GetObject(UnitOfWork unitOfWork, ITypeInfo typeInfo, CriteriaOperator criteriaOperator) {
            var xpBaseObject = unitOfWork.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, unitOfWork.GetClassInfo(typeInfo.Type),
                                                     criteriaOperator, true) as XPBaseObject ??
                               unitOfWork.FindObject(unitOfWork.GetClassInfo(typeInfo.Type), criteriaOperator, true) as XPBaseObject;
            return xpBaseObject ?? (XPBaseObject)ReflectionHelper.CreateObject(typeInfo.Type, unitOfWork);
        }

        CriteriaOperator GetObjectKeyCriteria(ITypeInfo typeInfo, IEnumerable<XElement> xElements) {
            string criteria = "";
            var parameters = new List<object>();
            foreach (var xElement in xElements) {
                var name = xElement.GetAttributeValue("name");
                parameters.Add(XpandReflectionHelper.ChangeType(xElement.Value, typeInfo.FindMember(name).MemberType));
                criteria += name + "=? AND ";
            }
            return CriteriaOperator.Parse(criteria.TrimEnd("AND ".ToCharArray()), parameters.ToArray());
        }

        public void ImportObjects(UnitOfWork unitOfWork, string fileName) {
            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                ImportObjects(fileStream, unitOfWork);
            }
        }

        public void ImportObjects(UnitOfWork unitOfWork, Type nameSpaceType, string resourceName) {
            Stream manifestResourceStream = nameSpaceType.Assembly.GetManifestResourceStream(nameSpaceType, resourceName);
            ImportObjects(manifestResourceStream, unitOfWork);
        }
    }
}