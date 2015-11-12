using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Xpand.ExpressApp.NH
{
    public class NHDataViewRecord : XafDataViewRecord
    {
        private readonly object obj;

        public NHDataViewRecord(XafDataView dataView, object obj)
            : base(dataView)
        {
            this.obj = obj;
        }


        private new NHDataView DataView
        {
            get { return (NHDataView)base.DataView; }
        }
        public override object this[string name]
        {
            get
            {
                int index = DataView.GetPropertyIndex(name);
                if (index >= 0)
                {
                    return this[index];
                }
                return null;
            }
        }

        public override object this[int index]
        {
            get
            {
                object[] array = obj as object[];
                if (array != null)
                    return array[index];
                else
                    return null;
            }
        }
    }

    
}
