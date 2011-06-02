using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ConditionalActionState.Logic;

namespace FeatureCenter.Module.Win.PropertyEditors
{
   [DisplayFeatureModel("DurationEditTestObject_DetailView", "DurationPropertyEdit")]
   [ActionStateRule("HideSaveAndClose_For_DurationPropertyEdit", "SaveAndClose", "1=1", null, ActionState.Hidden)]
   public class DurationEditTestObject : BaseObject, ISupportModificationStatements
   {

       private TimeSpan _Duration;
       [ImmediatePostData(true)]
       public TimeSpan Duration
       {
           get { return _Duration; }
           set { SetPropertyValue("Duration", ref _Duration, value); }
       }

       private TimeSpan _Duration2;

       public TimeSpan Duration2
       {
           get { return _Duration2; }
           set { SetPropertyValue("Duration2", ref _Duration2, value); }
       }


       public DurationEditTestObject(Session session)
            : base(session) {
        }

       [NonPersistent, Browsable(false)]
       public string ModificationStatements { get; set; }
   }
}
