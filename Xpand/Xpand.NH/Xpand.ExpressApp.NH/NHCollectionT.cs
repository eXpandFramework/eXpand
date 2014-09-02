using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class NHCollection<T> : NHCollection, IList<T>
    {
        public NHCollection(NHObjectSpace objectSpace, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction)
            : base(objectSpace, typeof(T), criteria, sorting, inTransaction)
        {
        }
        public Int32 IndexOf(T obj)
        {
            return base.IndexOf(obj);
        }
        public void Insert(Int32 index, T obj)
        {
            base.Insert(index, obj);
        }
        public new T this[Int32 index]
        {
            get { return (T)base[index]; }
            set { base[index] = value; }
        }
        public void Add(T obj)
        {
            base.Add(obj);
        }
        public Boolean Contains(T obj)
        {
            return base.Contains(obj);
        }
        public void CopyTo(T[] array, Int32 index)
        {
            base.CopyTo(array, index);
        }
        public Boolean Remove(T obj)
        {
            base.Remove(obj);
            return true;
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            InitObjects();
            return objects.Cast<T>().GetEnumerator();
        }
    }
}
