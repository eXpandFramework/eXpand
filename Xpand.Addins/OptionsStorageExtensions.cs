using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.CodeRush.Core;
using System.Linq;

namespace XpandAddIns {
    public static class OptionsStorageExtensions {
        public static List<string> GetGroupedKeys(this DecoupledStorage decoupledStorage, string section) {
            return decoupledStorage.GetKeys(section).Where(s => Regex.IsMatch(s, @"\A.*_Count\Z")).Select(s1 => Regex.Replace(s1, "(.*)_Count", "$1")).ToList();
        }
    }
}