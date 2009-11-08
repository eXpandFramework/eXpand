using System.Collections;
using System.Collections.Generic;
using DevExpress.Persistent.Base.General;

namespace eXpand.ExpressApp.TreeListEditors.Win.Controllers
{
    public static class TreeNodeExtensions
    {
        public static List<T> GetAllTreeNodes<T>(this T category) where T : ITreeNode
        {
            var memberCategories = new List<T> { category };
            GetAllTreeNodes(category, memberCategories);
            return memberCategories;
        }
        public static List<T> GetAllTreeNodes<T>(this List<T> categories) where T : ITreeNode
        {
            var memberCategories = new List<T>();
            foreach (var category in categories)
            {
                memberCategories.Add(category);
                GetAllTreeNodes(category, memberCategories);
            }

            return memberCategories;
        }
        private static void GetAllTreeNodes<T>(T memberCategory, IList memberCategories) where T : ITreeNode
        {
            foreach (T child in memberCategory.Children)
            {
                GetAllTreeNodes(child, memberCategories);
                memberCategories.Add(child);
            }
        }

    }
}
