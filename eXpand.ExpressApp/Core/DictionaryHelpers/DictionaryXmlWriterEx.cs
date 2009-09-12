using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using DevExpress.ExpressApp;
using System.Linq;

namespace eXpand.ExpressApp.Core.DictionaryHelpers{
    public class DictionaryXmlWriterEx{
        private readonly bool orderNodes;

        public DictionaryXmlWriterEx(){
        }

        public DictionaryXmlWriterEx(bool orderNodes)
            : this(){
            this.orderNodes = orderNodes;
        }

        private bool IsDefaultAspect(string aspect, DictionaryNode node){
            return ((node.Dictionary == null) || (aspect == DictionaryAttribute.DefaultLanguage));
        }

        private void BuildAttribute(string aspect, DictionaryAttribute attribute, StringBuilder attributesTextBuilder){
            if (!attribute.IsKey && attribute.IsValueAssigned(aspect) &&
                (IsDefaultAspect(aspect, attribute.Parent) || attribute.IsLocalized)){
                attributesTextBuilder.Append(" ");
                attributesTextBuilder.Append(attribute.Name);
                attributesTextBuilder.Append("=\"");
                attributesTextBuilder.Append(HttpUtility.HtmlEncode(attribute.GetValue(aspect)));
                attributesTextBuilder.Append("\"");
            }
        }

        private string GetAspectXml(string aspect, DictionaryNode node, int indent, bool includeChildNodes){
            string result = string.Empty;
            var indentString = new string('\t', indent);
            var childrenTextBuilder = new StringBuilder();
            var attributesTextBuilder = new StringBuilder();
            if (includeChildNodes){
                IEnumerable<DictionaryNode> nodes = orderNodes
                                                        ? node.ChildNodes.GetOrderedByName()
                                                        : (IEnumerable<DictionaryNode>) node.ChildNodes;
                foreach (DictionaryNode childNode in nodes){
                    childrenTextBuilder.Append(GetAspectXml(aspect, childNode, indent + 1, true));
                }
            }
            foreach (DictionaryAttribute attribute in node.Attributes){
                BuildAttribute(aspect, attribute, attributesTextBuilder);
            }
            if ((IsDefaultAspect(aspect, node) && node.IsNew) || attributesTextBuilder.Length > 0 ||
                childrenTextBuilder.Length > 0){
                DictionaryAttribute key = node.KeyAttribute;
                if (key != null){
                    attributesTextBuilder.Insert(0, "\"");
                    attributesTextBuilder.Insert(0, HttpUtility.HtmlEncode(key.Value));
                    attributesTextBuilder.Insert(0, "=\"");
                    attributesTextBuilder.Insert(0, key.Name);
                    attributesTextBuilder.Insert(0, " ");
                }
            }
            if (attributesTextBuilder.Length > 0 || childrenTextBuilder.Length > 0){
                if (childrenTextBuilder.Length > 0){
                    result = string.Format(indentString + "<{0}{1}>\r\n{2}" + indentString + "</{0}>\r\n", node.Name,
                                           attributesTextBuilder, childrenTextBuilder);
                }
                else{
                    result = string.Format(indentString + "<{0}{1} />\r\n", node.Name, attributesTextBuilder);
                }
            }
            else if (IsDefaultAspect(aspect, node) && node.IsNew){
                result = string.Format(indentString + "<{0} />\r\n", node.Name);
            }
            return result;
        }

        private string GetFullXml(string indent, DictionaryNode node){
            string attributesText = String.Empty;
            foreach (DictionaryAttribute attribute in node.Attributes){
                string attributeValueText = String.Empty;
                const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                var values =
                    (IReadOnlyAttributeValuesDictionary)
                    attribute.GetType().GetProperty("Values", flag).GetValue(attribute, null);
                foreach (string key in values.GetKeys()){
                    attributeValueText += key + ":" + values[key] + "¥";
                }
                attributeValueText = attributeValueText.TrimEnd('¥');
                attributeValueText = shortByAspect(attributeValueText);
                attributesText += string.Format("{0}=\"{1}\" ", attribute.Name,
                                                HttpUtility.HtmlEncode(attributeValueText));
            }
            attributesText = attributesText.TrimEnd(' ');
            string childrenText = String.Empty;
            IEnumerable<DictionaryNode> nodes = orderNodes
                                                    ? node.ChildNodes.GetOrderedByName()
                                                    : (IEnumerable<DictionaryNode>) node.ChildNodes;
            foreach (DictionaryNode childNode in nodes){
                childrenText += GetFullXml(indent + '\t', childNode);
            }
            string result = String.Empty;
            if (childrenText != String.Empty){
                result += indent +
                          string.Format("<{0} {1}>\r\n{2}" + indent + "</{0}>\r\n", node.Name, attributesText,
                                        childrenText);
            }
            else{
                result += indent + string.Format("<{0} {1}/>\r\n", node.Name, attributesText);
            }
            result = result.Replace(DictionaryAttribute.DefaultLanguage, null);
            return result;
        }

        private string shortByAspect(string value){
            value = value.Replace(DictionaryAttribute.DefaultLanguage, "0" + DictionaryAttribute.DefaultLanguage);
            string ret = "";
            string[] strings = value.Split('¥');
            foreach (var aspect in strings.OrderBy(s => s))
            {
                ret += aspect + "¥";
            }
            ret = ret.Replace("0" + DictionaryAttribute.DefaultLanguage, DictionaryAttribute.DefaultLanguage).TrimEnd('¥');
            return ret;
        }

        public string GetFullXml(DictionaryNode node){
            return GetFullXml("", node);
        }

        public string GetCurrentAspectXml(DictionaryNode node){
            if (node.Dictionary == null){
                return GetAspectXml(DictionaryAttribute.DefaultLanguage, node);
            }
            return GetAspectXml(node.Dictionary.CurrentAspect, node);
        }

        public string GetAspectXml(string aspect, DictionaryNode node){
            return GetAspectXml(aspect, node, true);
        }

        public string GetAspectXml(string aspect, DictionaryNode node, bool includeChildNodes){
            string result = GetAspectXml(aspect, node, 0, includeChildNodes);
            if (string.IsNullOrEmpty(result) && IsDefaultAspect(aspect, node)){
                result = string.Format("<{0} />\r\n", node.Name);
            }
            return result;
        }
    }
}