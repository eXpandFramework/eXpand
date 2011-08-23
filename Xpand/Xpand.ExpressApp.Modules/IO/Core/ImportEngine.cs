using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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
        IObjectSpace _objectSpace;
        readonly ErrorHandling _errorHandling;

        public ImportEngine(ErrorHandling errorHandling) {
            _errorHandling = errorHandling;
        }

        public ImportEngine() {
        }

        public int ImportObjects(string xml, IObjectSpace objectSpace) {
            var xmlTextReader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml))) { WhitespaceHandling = WhitespaceHandling.Significant };
            var document = XDocument.Load(xmlTextReader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            return ImportObjects(document, objectSpace);
        }

        int ImportObjects(XDocument document, IObjectSpace objectSpace) {
            _objectSpace = objectSpace;
            if (document.Root != null) {
                foreach (XElement element in document.Root.Nodes().OfType<XElement>()) {
                    using (IObjectSpace nestedObjectSpace = objectSpace.CreateNestedObjectSpace()) {
                        ITypeInfo typeInfo = GetTypeInfo(element);
                        if (typeInfo != null) {
                            var keys = GetKeys(element);
                            CriteriaOperator objectKeyCriteria = GetObjectKeyCriteria(typeInfo, keys);
                            if (objectKeyCriteria != null) {
                                CreateObject(element, nestedObjectSpace, typeInfo, objectKeyCriteria);
                                nestedObjectSpace.CommitChanges();
                            }
                        }
                    }
                }
                objectSpace.CommitChanges();
            }
            return 0;
        }

        static IEnumerable<XElement> GetKeys(XElement element) {
            IEnumerable<XElement> elements = element.Descendants("Property");
            var xElements =
                elements.Where(xElement => xElement.GetAttributeValue("isKey").MakeFirstCharUpper() == true.ToString());
            return xElements;
        }

        public void ImportObjects(Stream stream, IObjectSpace objectSpace) {
            Guard.ArgumentNotNull(stream, "Stream");
            stream.Position = 0;
            using (var streamReader = new XmlTextReader(stream) { WhitespaceHandling = WhitespaceHandling.Significant }) {
                var xDocument = XDocument.Load(streamReader, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
                ImportObjects(xDocument, objectSpace);
            }

        }

        XPBaseObject CreateObject(XElement element, IObjectSpace nestedObjectSpace, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria) {
            XPBaseObject xpBaseObject = GetObject(typeInfo, objectKeyCriteria);
            var keyValuePair = new KeyValuePair<ITypeInfo, CriteriaOperator>(typeInfo, objectKeyCriteria);
            if (!importedObjecs.ContainsKey(keyValuePair)) {
                importedObjecs.Add(keyValuePair, null);
                ImportProperties(nestedObjectSpace, xpBaseObject, element);
            }
            return xpBaseObject;
        }

        void ImportProperties(IObjectSpace nestedObjectSpace, XPBaseObject xpBaseObject, XElement element) {
            ImportSimpleProperties(element, xpBaseObject);
            ImportComplexProperties(element, nestedObjectSpace,
                                    (o, xElement) =>
                                    xpBaseObject.SetMemberValue(xElement.Parent.GetAttributeValue("name"), o),
                                    NodeType.Object);
            ImportComplexProperties(element, nestedObjectSpace,
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
        void ImportComplexProperties(XElement element, IObjectSpace nestedObjectSpace, Action<XPBaseObject, XElement> instance, NodeType nodeType) {
            IEnumerable<XElement> objectElements = GetObjectRefElements(element, nodeType);
            ITypeInfo typeInfo = GetTypeInfo(element);
            foreach (XElement objectElement in objectElements) {
                ITypeInfo memberTypeInfo = GetTypeInfo(objectElement);
                if (memberTypeInfo != null) {
                    var refObjectKeyCriteria = GetObjectKeyCriteria(memberTypeInfo, objectElement.Descendants("Key"));
                    XPBaseObject xpBaseObject = null;
                    XElement element1 = objectElement;
                    if (objectElement.GetAttributeValue("strategy") ==
                        SerializationStrategy.SerializeAsObject.ToString()) {
                        var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement();
                        HandleErrorComplex(objectElement, typeInfo, () => {
                            if (findObjectFromRefenceElement != null)
                                xpBaseObject = CreateObject(findObjectFromRefenceElement, nestedObjectSpace, memberTypeInfo, refObjectKeyCriteria);
                            instance.Invoke(xpBaseObject, element1);
                        });

                    } else {
                        HandleErrorComplex(objectElement, typeInfo, () => {
                            xpBaseObject = GetObject(memberTypeInfo, refObjectKeyCriteria);
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
            if (_errorHandling == ErrorHandling.CreateErrorObjects) {
                var errorInfoObject =
                    (IIOError)_objectSpace.CreateObject(XafTypesInfo.Instance.FindBussinessObjectType<IIOError>());
                errorInfoObject.Reason = failReason;
                errorInfoObject.ElementXml = elementXml;
                errorInfoObject.InnerXml = innerXml;
            } else if (_errorHandling == ErrorHandling.ThrowException) {
                throw new UserFriendlyException(new Exception("ImportFailed", new Exception("Reason=" + failReason + "ELEMENTXML=" + elementXml + " INNERXML=" + innerXml)));
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

        XPBaseObject GetObject(ITypeInfo typeInfo, CriteriaOperator criteriaOperator) {
            if (criteriaOperator != null) {
                var unitOfWork = ((UnitOfWork)((ObjectSpace)_objectSpace).Session);
                var xpBaseObject = unitOfWork.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, unitOfWork.GetClassInfo(typeInfo.Type),
                                                         criteriaOperator, true) as XPBaseObject ??
                                   unitOfWork.FindObject(unitOfWork.GetClassInfo(typeInfo.Type), criteriaOperator, true) as XPBaseObject;
                return xpBaseObject ?? (XPBaseObject)ReflectionHelper.CreateObject(typeInfo.Type, unitOfWork);
            }
            return null;
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

        public void ImportObjects(IObjectSpace objectSpace, string fileName) {
            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                ImportObjects(fileStream, objectSpace);
            }
        }

        public void ImportObjects(IObjectSpace objectSpace, Type nameSpaceType, string resourceName) {
            Stream manifestResourceStream = nameSpaceType.Assembly.GetManifestResourceStream(nameSpaceType, resourceName);
            ImportObjects(manifestResourceStream, objectSpace);
        }
    }
}