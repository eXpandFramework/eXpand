using System;
using System.Reflection;

namespace eXpand.Utils {
    public class ObjectMerger
    {
        public enum MergeConcurrency
        {
            Source,
            Target
        }

        public static void Merge<S, T>(ref S source, ref T target)
        {
            Merge(source, ref target, MergeConcurrency.Source);
        }

        public static void Merge<S, T>(S source, ref T target, MergeConcurrency winner)
        {
            PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo targetProp in properties) {
                PropertyInfo sourceProperty = source.GetType().GetProperty(targetProp.Name);
                if (sourceProperty != null) {
                    if (sourceProperty.GetValue(source, null) != targetProp.GetValue(target, null)) {
                        if (winner == MergeConcurrency.Source)
                            targetProp.SetValue(target,
                                                Convert.ChangeType(sourceProperty.GetValue(source, null),
                                                                   targetProp.PropertyType), null);
                        else
                            sourceProperty.SetValue(source,
                                                    Convert.ChangeType(targetProp.GetValue(source, null),
                                                                       sourceProperty.PropertyType), null);
                    }
                }
            }
        }
    }
}