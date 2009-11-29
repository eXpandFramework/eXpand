using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using ITypeInfo=DevExpress.ExpressApp.DC.ITypeInfo;

namespace eXpand.Persistent.BaseImpl
{
    public class FilterWithObjectsUserFrendlyStringProcessor : CriteriaProcessorBase
    {
        private readonly Type objectType;

        public FilterWithObjectsUserFrendlyStringProcessor(Type objectType)
        {
            this.objectType = objectType;
        }

        protected override void Process(OperandProperty theOperand)
        {
            base.Process(theOperand);
            theOperand.PropertyName =
                CaptionHelper.GetFullMemberCaption(XafTypesInfo.Instance.FindTypeInfo(objectType),
                                                   theOperand.PropertyName);
        }

        protected override void Process(OperandValue theOperand)
        {
            base.Process(theOperand);

            if (theOperand.Value != null)
            {
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(theOperand.Value.GetType());
                if (typeInfo != null)
                {
                    if (typeInfo.DefaultMember != null)
                    {
                        theOperand.Value = typeInfo.DefaultMember.GetValue(theOperand.Value).ToString();
                        return;
                    }
                }
                theOperand.Value = theOperand.Value.ToString();
            }
        }
    }
}