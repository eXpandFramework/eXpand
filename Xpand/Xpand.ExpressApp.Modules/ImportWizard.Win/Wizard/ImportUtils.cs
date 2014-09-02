using System;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard{
    public static class ImportUtils{
        public static string GetQString(string oid){
            try{
                int s1 = oid.IndexOf(@"'", StringComparison.Ordinal);
                int s2 = oid.IndexOf(@"'", s1 + 1, StringComparison.Ordinal);
                string s3 = oid.Substring(s1 + 1, s2 - 1);
                return s3;
            }
            catch (Exception){
                return string.Empty;
            }
        }

        internal static Object GetCollectionOwner(Object masterObject, IMemberInfo memberInfo){
            return memberInfo.GetOwnerInstance(masterObject);
        }
    }
}