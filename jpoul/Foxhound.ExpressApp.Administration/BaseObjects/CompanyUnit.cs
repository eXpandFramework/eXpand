using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.General;
using eXpand.Xpo.Converters.ValueConverters;
using Organization=eXpand.Persistent.TaxonomyImpl.Organization;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class CompanyUnit : Organization, ITreeNode {
        private Company company;
        
        public CompanyUnit(Session session) : base(session){
            resourceImpl = new ResourceImpl();
        }

        [Association("Company-Units")]
        public Company Company{
            get { return company; }
            set { SetPropertyValue("Company", ref company, value); }
        }
        

        #region Resources and activities
        [ValueConverter(typeof (SerializableObjectConverter))] [Persistent("ResourceImpl"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))] private readonly ResourceImpl resourceImpl;

        [NonPersistent, Browsable(false)]
        public object Id{
            get{
                return resourceImpl.Id;
            }
        }

        [NonPersistent, Browsable(false)]
        public int OleColor{
            get { return ColorTranslator.ToOle(resourceImpl.Color); }
        }

        [NonPersistent]
        public Color Color{
            get { return resourceImpl.Color; }
            set{
                resourceImpl.Color = value;
                OnChanged("resourceImpl");
            }
        }

        public string Caption{
            get{
                if (string.IsNullOrEmpty(resourceImpl.Caption)){
                    CreateCaption();
                }
                return resourceImpl.Caption;
            }
            set{
                resourceImpl.Caption = value;
                OnChanged("resourceImpl");
            }
        }

        private void CreateCaption(){
            //resourceImpl.Caption = string.Format("{0}<br/>({1})", Name, vendorKey);
        }

        protected override void OnSaved(){
            if (resourceImpl.Id == null || ((Guid) resourceImpl.Id) == Guid.Empty){
                resourceImpl.Id = Oid;
                Save();
                Session.CommitTransaction();
                Reload();
            }
        }
        #endregion
        #region Tree structure
        [Browsable(false)]
        ITreeNode ITreeNode.Parent{
            get { return Company; }
        }

        [Browsable(false)]
        IBindingList ITreeNode.Children{
            get { return new BindingList<object>(); }
        }
        #endregion
    }
}