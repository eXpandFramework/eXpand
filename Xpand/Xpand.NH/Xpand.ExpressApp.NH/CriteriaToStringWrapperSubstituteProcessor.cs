using DevExpress.Data.Filtering.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class CriteriaToStringWrapperSubstituteProcessor : CriteriaToBasicStyleParameterlessProcessor
    {
        internal string ConvertToString(DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            return Process(criteria).Result;
        }

        public override CriteriaToStringVisitResult Visit(DevExpress.Data.Filtering.OperandValue operand)
        {
            if (operand.GetType().FullName == "DevExpress.Persistent.Base.CriteriaWrapperOperandParameter") //Non Public Type
            {
                return new CriteriaToStringVisitResult(ValueToString(operand.Value));
            }
            else
            {
                return base.Visit(operand);
            }
        }
    }
}
