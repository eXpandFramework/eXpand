using DevExpress.Data.Filtering;

namespace eXpand.Xpo
{
    public static class CriteriaOperatorExtensions
    {
        public static CriteriaOperator Parse(string propertyPath, CriteriaOperator criteriaOperator)
        {
            while (propertyPath.IndexOf(".")>-1)
            {
                propertyPath = propertyPath.Substring(0, propertyPath.IndexOf(".")) + "[" +
                               propertyPath.Substring(propertyPath.IndexOf(".") + 1) + "]";
            }
//            string replace = criteriaOperator.ToString().Replace("[","").Replace("]","").Replace(" ","");
            for (int i = propertyPath.Length-1; i > -1; i--)
                if (propertyPath[i] != ']')
                {
                    propertyPath = propertyPath.Substring(0, i+1) + "[" + criteriaOperator.ToString() + "]" +
                                   new string(']', propertyPath.Length - i-1);
                    break;
                }
            
            return CriteriaOperator.Parse(propertyPath);
        }
    }
}
