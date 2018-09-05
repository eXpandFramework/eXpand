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
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;
using Xpand.Xpo;

namespace Xpand.ExpressApp.IO.Core {

    public class ImportEngine {
        readonly Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object> _importedObjects = new Dictionary<KeyValuePair<ITypeInfo, CriteriaOperator>, object>();
        readonly ErrorHandling _errorHandling;
        private IObjectSpace _objectSpace;

        public ImportEngine(ErrorHandling errorHandling) {
            _errorHandling = errorHandling;
        }

        public ImportEngine() {
        }

        public int ImportObjects(string xml, Func<ITypeInfo, IObjectSpace> objectSpaceQuery) {
            
            var xmlTextReader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml))) { WhitespaceHandling = WhitespaceHandling.Significant };
            var document = XDocument.Load(xmlTextReader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            return ImportObjects(document, objectSpaceQuery);
        }

        int ImportObjects(XDocument document, Func<ITypeInfo, IObjectSpace> objectSpaceQuery) {
            if (document.Root != null) {
                var elementInfo = ElementSchema.Get(document);
                foreach (var element in document.Root.Nodes().OfType<XElement>()) {
                    var typeInfo = GetTypeInfo(element,elementInfo);
                    _objectSpace = _objectSpace ?? objectSpaceQuery(typeInfo);
                    var keys = GetKeys(element,elementInfo);
                    CriteriaOperator objectKeyCriteria = GetObjectKeyCriteria(typeInfo, keys,elementInfo);
                    if (!ReferenceEquals(objectKeyCriteria, null)){
                        CreateObject(element, typeInfo, objectKeyCriteria,elementInfo);
                    }
                }
                _objectSpace.CommitChanges();
            }
            return 0;
        }

        static IEnumerable<XElement> GetKeys(XElement element,ElementSchema elementSchema) {
            if (element.GetAttributeValue(elementSchema.Strategy) == SerializationStrategy.SerializeAsValue.ToString()){
                return element.Descendants(elementSchema.Key);
            }
            IEnumerable<XElement> elements = element.Descendants(elementSchema.Property);
            var xElements =
                elements.Where(xElement => xElement.GetAttributeValue(elementSchema.IsKey).MakeFirstCharUpper() == true.ToString(CultureInfo.InvariantCulture));
            return xElements;
        }

        public void ImportObjects(Stream stream, Func<ITypeInfo, IObjectSpace> objectSpaceQuery) {
            Guard.ArgumentNotNull(stream, "Stream");
            stream.Position = 0;
            using (var streamReader = new XmlTextReader(stream) { WhitespaceHandling = WhitespaceHandling.Significant }) {
                var xDocument = XDocument.Load(streamReader, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
                ImportObjects(xDocument, objectSpaceQuery);
            }

        }

        object CreateObject(XElement element, ITypeInfo typeInfo, CriteriaOperator objectKeyCriteria,
            ElementSchema elementSchema) {
            var xpBaseObject = GetObject(element,typeInfo, objectKeyCriteria, elementSchema);
            var keyValuePair = new KeyValuePair<ITypeInfo, CriteriaOperator>(typeInfo, objectKeyCriteria);
            if (!_importedObjects.ContainsKey(keyValuePair)) {
                _importedObjects.Add(keyValuePair, null);
                ImportProperties(typeInfo, xpBaseObject, element,elementSchema);
            }
            return xpBaseObject;
        }

        void ImportProperties(ITypeInfo typeInfo, object theObject, XElement element, ElementSchema elementSchema) {
            ImportSimpleProperties(typeInfo,element, theObject,elementSchema);
            ImportComplexProperties(element, (value, xElement) =>
                    typeInfo.FindMember(xElement.Parent.GetAttributeValue(elementSchema.Name)).SetValue(theObject,value),
                                    NodeType.Object, elementSchema);
            ImportComplexProperties(element, (baseObject, element1) =>
                ((IList)typeInfo.FindMember(element1.Parent.GetAttributeValue(elementSchema.Name)).GetValue(theObject)).Add(
                    baseObject), NodeType.Collection, elementSchema);
        }

        private void HandleErrorComplex(XElement objectElement, ITypeInfo typeInfo, Action action, ElementSchema elementSchema) {
            var memberInfo = typeInfo.FindMember(objectElement.Parent.GetAttributeValue(elementSchema.Name));
            if (memberInfo != null) {
                action.Invoke();
            } else {
                HandleError(objectElement, FailReason.PropertyNotFound,elementSchema);
            }
        }

        void ImportComplexProperties(XElement element, Action<object, XElement> instance, NodeType nodeType,
            ElementSchema elementSchema) {
            var objectElements = GetObjectRefElements(element, nodeType,elementSchema);
            var typeInfo = GetTypeInfo(element,elementSchema);
            foreach (var objectElement in objectElements) {
                var memberTypeInfo = GetTypeInfo(objectElement,elementSchema);
                if (memberTypeInfo != null) {
                    var refObjectKeyCriteria = GetObjectKeyCriteria(memberTypeInfo, objectElement.Descendants(elementSchema.Key),elementSchema);
                    object xpBaseObject = null;
                    var element1 = objectElement;
                    if (objectElement.GetAttributeValue(elementSchema.Strategy) ==
                        SerializationStrategy.SerializeAsObject.ToString()) {
                        var findObjectFromRefenceElement = objectElement.FindObjectFromRefenceElement(elementSchema.IsMinified);
                        HandleErrorComplex(objectElement, typeInfo, () => {
                            if (findObjectFromRefenceElement != null)
                                xpBaseObject = CreateObject(findObjectFromRefenceElement,  memberTypeInfo, refObjectKeyCriteria,elementSchema);
                            instance.Invoke(xpBaseObject, element1);
                        }, elementSchema);

                    } else {
                        HandleErrorComplex(objectElement, typeInfo, () => {
                            xpBaseObject = GetObject(objectElement, memberTypeInfo, refObjectKeyCriteria, elementSchema);
                            instance.Invoke(xpBaseObject, element1);
                        }, elementSchema);
                    }

                }
            }
        }


        void HandleError(XElement element, FailReason failReason,ElementSchema elementSchema) {
            string innerXml = null;
            string elementXml;
            var firstOrDefault = element.Ancestors(elementSchema.SerializedObject).FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault != element) {
                innerXml = element.ToString();
                elementXml = firstOrDefault.ToString();
            } else {
                elementXml = element.ToString();
            }
            if (_errorHandling == ErrorHandling.CreateErrorObjects) {
                var errorInfoObject = _objectSpace.Create<IIOError>();
                errorInfoObject.Reason = failReason;
                errorInfoObject.ElementXml = elementXml;
                errorInfoObject.InnerXml = innerXml;
            } else if (_errorHandling == ErrorHandling.ThrowException) {
                throw new UserFriendlyException(new Exception("ImportFailed", new Exception("Reason=" + failReason + "ELEMENTXML=" + elementXml + " INNERXML=" + innerXml)));
            }
        }


        IEnumerable<XElement> GetObjectRefElements(XElement element, NodeType nodeType,ElementSchema elementSchema) {
            return element.Properties(nodeType,elementSchema.IsMinified).SelectMany(
                element1 => element1.Descendants(elementSchema.SerializedObjectRef));
        }

        void ImportSimpleProperties(ITypeInfo typeInfo, XElement element,  object theObject,ElementSchema elementSchema) {
            foreach (XElement simpleElement in element.Properties(NodeType.Simple,elementSchema.IsMinified)) {
                string propertyName = simpleElement.GetAttributeValue(elementSchema.Name);
                var memberInfo = typeInfo.FindMember(propertyName);
                if (memberInfo != null) {
                    object value = GetValue(simpleElement, memberInfo);
                    typeInfo.FindMember(propertyName).SetValue(theObject,value);
                } else {
                    HandleError(simpleElement, FailReason.PropertyNotFound,elementSchema);
                }
            }
        }

        object GetValue(XElement simpleElement, IMemberInfo memberInfo) {
            var valueConverterAttribute = memberInfo.FindAttribute<ValueConverterAttribute>();
            var valueConverter = valueConverterAttribute?.Converter;
            if (valueConverter != null && !(valueConverter is EnumsConverter)) {
                var value = GetValue(valueConverter.StorageType, simpleElement);
                return valueConverter.ConvertFromStorageType(value);
            }
            return GetValue(GetMemberType(memberInfo), simpleElement);
        }

        Type GetMemberType(IMemberInfo xpMemberInfo) {
            return xpMemberInfo is ServiceField
                       ? typeof(Nullable<>).MakeGenericType(xpMemberInfo.MemberType)
                       : xpMemberInfo.MemberType;
        }

        object GetValue(Type type, XElement simpleElement) {
            if (type == typeof(byte[])) {
                return string.IsNullOrEmpty(simpleElement.Value) ? null : Convert.FromBase64String(simpleElement.Value);
            }
            return !string.IsNullOrEmpty(simpleElement.Value) ? XpandReflectionHelper.ChangeType(simpleElement.Value, type, CultureInfo.InvariantCulture) : null;
        }

        ITypeInfo GetTypeInfo(XElement element,ElementSchema elementSchema) {
            var typeInfo = ReflectionHelper.FindTypeInfoByName(element.GetAttributeValue(elementSchema.Type));
            if (typeInfo == null)
                HandleError(element, FailReason.TypeNotFound,elementSchema);
            return typeInfo;
        }

        object GetObject(XElement element, ITypeInfo typeInfo, CriteriaOperator criteriaOperator,
            ElementSchema elementSchema) {
            if (!ReferenceEquals(criteriaOperator, null)){
                var theObject = _objectSpace.ModifiedObjects.Cast<object>().FirstOrDefault(o => IsAlreadyCreated(typeInfo, criteriaOperator, o));
                var objectKeyCriteria = GetObjectKeyCriteria(typeInfo, GetKeys(element,elementSchema),elementSchema);
                return theObject ??
                       ((UnitOfWork) _objectSpace.Session()).FindObject(typeInfo.QueryXPClassInfo(),objectKeyCriteria,true)??_objectSpace.CreateObject(typeInfo.Type);
            }
            return null;
        }

        private bool IsAlreadyCreated(ITypeInfo typeInfo, CriteriaOperator criteriaOperator, object o){
            if (o.GetType() != typeInfo.Type) return false;
            var isObjectFitForCriteria = _objectSpace.IsObjectFitForCriteria(o,criteriaOperator);
            return isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value;
        }

        CriteriaOperator GetObjectKeyCriteria(ITypeInfo typeInfo, IEnumerable<XElement> xElements,ElementSchema  elementSchema, string prefix = "") {
            CriteriaOperator op = CriteriaOperator.Parse("");
            foreach (var xElement in xElements) {
                var propertyName = xElement.GetAttributeValue(elementSchema.Name);
                var iMemberInfo = typeInfo.FindMember(propertyName);

                if (iMemberInfo != null) {
                    var memberType = iMemberInfo.MemberTypeInfo;

                    if (typeof(XPBaseObject).IsAssignableFrom(memberType.Type))
                        op &= GetObjectKeyCriteria(memberType, xElement.Elements().First().Elements(),elementSchema, prefix + propertyName + ".");
                    else if (iMemberInfo.MemberType == typeof(Type)) {
                        var typeName = (string)GetValue(typeof(string), xElement);
                        var type = XafTypesInfo.Instance.FindTypeInfo(typeName).Type;
                        op &= CriteriaOperator.Parse(prefix + propertyName + "=?", type);
                    }
                    else {
                        op &= CriteriaOperator.Parse(prefix + propertyName + "=?", XpandReflectionHelper.ChangeType(GetValue(iMemberInfo.MemberType, xElement), memberType.Type));
                    }
                }
                else
                    HandleError(xElement, FailReason.PropertyNotFound,elementSchema);
            }
            return op;
        }

        public void ImportObjects(Func<ITypeInfo, IObjectSpace> objectSpaceQuery, string fileName) {
            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                ImportObjects(fileStream, objectSpaceQuery);
            }
        }

        public void ImportObjects(Func<ITypeInfo, IObjectSpace> objectSpaceQuery, Type nameSpaceType,
            string resourceName) {
            Stream manifestResourceStream = nameSpaceType.Assembly.GetManifestResourceStream(nameSpaceType, resourceName);
            ImportObjects(manifestResourceStream, objectSpaceQuery);
        }
    }
}