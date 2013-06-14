using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.ConditionalActionState.Logic;

namespace FeatureCenter.Module.Win.PropertyEditors.NullAble {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderNullAble, "1=1", "1=1",
        Captions.ViewMessageNullAble, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderNullAble, "1=1", "1=1",
        Captions.HeaderNullAble, Position.Top, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule("NullAblePropertyEditors", "1=1", "1=1", null, Position.Bottom, MessageProperty = "ModificationStatements")]

    [DisplayFeatureModel("FeatureCenter.Module.Win.PropertyEditors.NullAble.PENACustomer_DetailView", "NullAblePropertyEditors")]
    [ActionStateRule("HideSaveAndClose_For_NullablePropertyEditors","SaveAndClose","1=1",null,ActionState.Hidden)]
    public class PENACustomer : BaseObject, ISupportModificationStatements {
        public PENACustomer(Session session)
            : base(session) {
        }
        private bool? _booleanPropery;
        public bool? BooleanPropery {
            get {
                return _booleanPropery;
            }
            set {
                SetPropertyValue("BooleanPropery", ref _booleanPropery, value);
            }
        }
        private bool _notNullAbleBoolean;
        public bool NotNullAbleBoolean {
            get {
                return _notNullAbleBoolean;
            }
            set {
                SetPropertyValue("NotNullAbleBoolean", ref _notNullAbleBoolean, value);
            }
        }
        private DateTime _notNullAbleDateTimeProperty;
        public DateTime NotNullAbleDateTimeProperty {
            get {
                return _notNullAbleDateTimeProperty;
            }
            set {
                SetPropertyValue("NotNullAbleDateTimeProperty", ref _notNullAbleDateTimeProperty, value);
            }
        }
        private DateTime? _dateTimeProperty;
        public DateTime? DateTimeProperty {
            get {
                return _dateTimeProperty;
            }
            set {
                SetPropertyValue("DateTimeProperty", ref _dateTimeProperty, value);
            }
        }
        private double _notNullAbleDoubleProperty;
        public double NotNullAbleDoubleProperty {
            get {
                return _notNullAbleDoubleProperty;
            }
            set {
                SetPropertyValue("NotNullAbleDoubleProperty", ref _notNullAbleDoubleProperty, value);
            }
        }
        private double? _doubleProperty;
        public double? DoubleProperty {
            get {
                return _doubleProperty;
            }
            set {
                SetPropertyValue("DoubleProperty", ref _doubleProperty, value);
            }
        }
        private int _notNullAbleIntegerProperty;
        public int NotNullAbleIntegerProperty {
            get {
                return _notNullAbleIntegerProperty;
            }
            set {
                SetPropertyValue("NotNullAbleIntegerProperty", ref _notNullAbleIntegerProperty, value);
            }
        }
        private int? _integerProperty;
        public int? IntegerProperty {
            get {
                return _integerProperty;
            }
            set {
                SetPropertyValue("IntegerProperty", ref _integerProperty, value);
            }
        }
        private long? _longProperty;
        public long? LongProperty {
            get {
                return _longProperty;
            }
            set {
                SetPropertyValue("LongProperty", ref _longProperty, value);
            }
        }
        [NonPersistent, Browsable(false)]
        public string ModificationStatements { get; set; }
    }
}
