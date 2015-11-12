using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class CriteriaObjectReplacer : ClientCriteriaVisitorBase
    {
        private readonly ITypesInfo typesInfo;
        public CriteriaObjectReplacer(ITypesInfo typesInfo)
        {
            if (typesInfo == null)
                throw new ArgumentNullException("typesInfo");

            this.typesInfo = typesInfo;
        }

        private ITypeInfo GetPersistentTypeInfo(CriteriaOperator op)
        {
            OperandValue theOperand = op as OperandValue;
            if (!ReferenceEquals(null, theOperand) && !ReferenceEquals(null, theOperand.Value))
            {
                var ti = typesInfo.FindTypeInfo(theOperand.Value.GetType());
                if (ti != null && ti.IsPersistent && ti.KeyMember != null)
                    return ti;
            }

            return null;
        }
        protected override CriteriaOperator Visit(OperandValue theOperand)
        {

            if (theOperand.Value != null)
            {
                var ti = GetPersistentTypeInfo(theOperand);
                if (ti != null)
                    return new OperandValue(ti.KeyMember.GetValue(theOperand.Value));
            }
            return base.Visit(theOperand);
        }

        protected override CriteriaOperator Visit(OperandProperty theOperand)
        {
            return base.Visit(theOperand);
        }

        protected override CriteriaOperator Visit(BinaryOperator theOperator)
        {
            var pti = GetPersistentTypeInfo(theOperator.LeftOperand);
            OperandProperty op = theOperator.RightOperand as OperandProperty;
            if (pti != null && !ReferenceEquals(null, op))
            {
                return new BinaryOperator((CriteriaOperator)theOperator.LeftOperand.Accept(this),
                    AddKeyProperty(pti, op), theOperator.OperatorType);
            }

            pti = GetPersistentTypeInfo(theOperator.RightOperand);
            op = theOperator.LeftOperand as OperandProperty;
            if (pti != null && !ReferenceEquals(null, op))
            {
                return new BinaryOperator(AddKeyProperty(pti, op),
                    (CriteriaOperator)theOperator.RightOperand.Accept(this), theOperator.OperatorType);

            }
            return base.Visit(theOperator);
        }

        private static OperandProperty AddKeyProperty(ITypeInfo pti, OperandProperty op)
        {
            return new OperandProperty(op.PropertyName + "." + pti.KeyMember.Name);
        }


    }
}
