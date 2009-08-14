using System;
using System.Collections;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Converters.ValueConverters
{
    /// <summary>
    /// Summary description for IEnumerableValueConverter.
    /// </summary>
    public class IEnumerableValueConverter : ValueConverter
    {
        private readonly string delimeter = "ÿ";
        private readonly string hashTabledelimeter = "¶";

        public override Type StorageType
        {
            get { return typeof (string); }
        }

        public override object ConvertToStorageType(object value)
        {
            if (value == null) return null;
            string s = null;
            foreach (object o in ((IEnumerable) value))
            {
                if (o != null)
                    s += o + delimeter;
            }
            
            if (s != null) return s.TrimEnd(delimeter.ToCharArray());
            return null;
        }

        public override object ConvertFromStorageType(object value)
        {
            if (value == null) return null;
            string[] split = value.ToString().Split(delimeter.ToCharArray());
            if (value.ToString().IndexOf(hashTabledelimeter) > -1)
            {
                var hashtable = new Hashtable();
                foreach (string s in split)
                {
                    string[] strings = s.Split(hashTabledelimeter.ToCharArray());
                    hashtable.Add(strings[0], strings.Length == 0 ? null : strings[1]);
                }
                return hashtable;
            }
            return new ArrayList(split);
        }
    }
}