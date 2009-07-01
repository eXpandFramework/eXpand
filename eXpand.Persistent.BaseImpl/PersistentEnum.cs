using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Interfaces;

namespace eXpand.Persistent.BaseImpl
{
    [DefaultProperty("Name")]
    public class PersistentEnum : BaseObject, ILocalizable, INamedObject {
        private Color color;
        private string cultureCode;
        private string group;
        private string name;
        private string enumValue;
        public PersistentEnum(Session session) : base(session) { }

        [Index(2)]
        public string EnumValue {
            get { return enumValue; }
            set { SetPropertyValue("EnumValue", ref enumValue, value); }
        }
        [Index(1)]
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        
        [Index(3)]
        public string CultureCode {
            get { return cultureCode; }
            set { SetPropertyValue("CultureCode", ref cultureCode, value); }
        }

        [Index(4)]
        [NonPersistent]
        public CultureInfo Culture {
            get {
                return (string.IsNullOrEmpty(CultureCode) ? null : CultureInfo.GetCultureInfo(CultureCode));
            }
        }
        [Index(0)]
        public string Group {
            get { return group; }
            set { SetPropertyValue("Group", ref group, value); }
        }
        [Index(5)]
        public Color Color {
            get { return color; }
            set { SetPropertyValue("Color", ref color, value); }
        }

        private string providerModuleName;
        [Browsable(false)]
        public string ProviderModuleName {
            get {
                return providerModuleName;
            }
            set {
                SetPropertyValue("ProviderModuleName", ref providerModuleName, value);
            }
        }

        public static PersistentEnum GetEnumByGroupAndValue(Session session, string group, string enumValue) {
            return session.FindObject<PersistentEnum>(
                new GroupOperator(GroupOperatorType.And,
                                  new CriteriaOperator[]{
                                                            new BinaryOperator("Group", group),
                                                            new BinaryOperator("EnumValue", enumValue)
                                                        }));
        }
        
        public static PersistentEnum RegisterPersistentEnum(Session session, string group, string name, string enumValue, string moduleRequesting) {
            PersistentEnum persistentEnum = GetEnumByGroupAndValue(session, group, enumValue);
            
            if (persistentEnum == null) {
                persistentEnum = new PersistentEnum(session)
                                     {
                                         ProviderModuleName = moduleRequesting,
                                         Group = group,
                                         Name = enumValue,
                                         EnumValue = enumValue,
                                         Color = Color.White
                                     };
                persistentEnum.Save();
            }
            return persistentEnum;
        }
    }
}