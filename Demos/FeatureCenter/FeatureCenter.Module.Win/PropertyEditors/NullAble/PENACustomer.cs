using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;

namespace FeatureCenter.Module.Win.PropertyEditors.NullAble {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderNullAble, "1=1", "1=1",
        Captions.ViewMessageNullAble, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderNullAble, "1=1", "1=1",
        Captions.HeaderNullAble, Position.Top, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule("NullAblePropertyEditors", "1=1", "1=1", null, Position.Bottom,
        MessageProperty = "ModificationStatements")]
    [DisplayFeatureModel("PENACustomer_DetailView", "NullAblePropertyEditors")]
    [ActionStateRule("HideSaveAndClose_For_NullablePropertyEditors", "SaveAndClose", "1=1", null, ActionState.Hidden)]
    public class PENACustomer : BaseObject, ISupportModificationStatements {
        bool? _booleanPropery;
        DateTime? _dateTimeProperty;
        double? _doubleProperty;
        int? _integerProperty;
        long? _longProperty;

        bool _notNullAbleBoolean;

        DateTime _notNullAbleDateTimeProperty;
        double _notNullAbleDoubleProperty;
        int _notNullAbleIntegerProperty;

        public PENACustomer(Session session)
            : base(session) {
        }

        public bool? BooleanPropery {
            get { return _booleanPropery; }
            set { SetPropertyValue("BooleanPropery", ref _booleanPropery, value); }
        }

        public bool NotNullAbleBoolean {
            get { return _notNullAbleBoolean; }
            set { SetPropertyValue("NotNullAbleBoolean", ref _notNullAbleBoolean, value); }
        }

        public DateTime NotNullAbleDateTimeProperty {
            get { return _notNullAbleDateTimeProperty; }
            set { SetPropertyValue("NotNullAbleDateTimeProperty", ref _notNullAbleDateTimeProperty, value); }
        }

        public DateTime? DateTimeProperty {
            get { return _dateTimeProperty; }
            set { SetPropertyValue("DateTimeProperty", ref _dateTimeProperty, value); }
        }

        public double NotNullAbleDoubleProperty {
            get { return _notNullAbleDoubleProperty; }
            set { SetPropertyValue("NotNullAbleDoubleProperty", ref _notNullAbleDoubleProperty, value); }
        }

        public double? DoubleProperty {
            get { return _doubleProperty; }
            set { SetPropertyValue("DoubleProperty", ref _doubleProperty, value); }
        }

        public int NotNullAbleIntegerProperty {
            get { return _notNullAbleIntegerProperty; }
            set { SetPropertyValue("NotNullAbleIntegerProperty", ref _notNullAbleIntegerProperty, value); }
        }

        public int? IntegerProperty {
            get { return _integerProperty; }
            set { SetPropertyValue("IntegerProperty", ref _integerProperty, value); }
        }

        public long? LongProperty {
            get { return _longProperty; }
            set { SetPropertyValue("LongProperty", ref _longProperty, value); }
        }

        [NonPersistent, Browsable(false)]
        public string ModificationStatements { get; set; }
    }
}