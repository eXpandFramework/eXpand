using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Utils.Helpers;
using Xpand.Xpo.Converters.ValueConverters;

namespace XpandSystemTester.Module.FunctionalTests.PropertyEditors.ChooseFromListCollectionEditor{
    [DefaultClassOptions]
    [NavigationItem("PropertyEditors")]
    public class ChooseFromListCollectionEditorObject : BaseObject{
        private const string DdMm = "dd/MM";

        [Persistent("DatesExcluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DateTime> _datesExcluded;

        public ChooseFromListCollectionEditorObject(Session session) : base(session){
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            _datesExcluded = new List<DateTime>();
        }

        [VisibleInDetailViewAttribute(false)]
        public string ListViewDates {
            get { return string.Join(", ", GetDateTimes(DatesExcluded)); }
        }

        [DataSourceProperty("AllDates")]
        [DisplayFormat("{0:" +DdMm+ "}")]
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        public List<DateTime> DatesExcluded {
            get { return _datesExcluded; }
        }

        [Browsable(false)]
        public List<DateTime> AllDates {
            get { return DateTimeUtils.GetDates().ToList(); }
        }

        private IEnumerable<string> GetDateTimes(IEnumerable<DateTime> dateTimes) {
            return dateTimes.Select(time => time.ToString(DdMm));
        }
    }
}