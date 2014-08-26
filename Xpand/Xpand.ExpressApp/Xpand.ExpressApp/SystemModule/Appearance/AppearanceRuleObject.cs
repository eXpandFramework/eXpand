using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Xpo;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.ExpressApp.SystemModule.Appearance{
    public class AppearanceRuleObject : XpandCustomObject, IAppearanceRuleProperties, ICheckedListBoxItemsProvider{
        private AppearanceItemType _appearanceItemType;
        private Color? _backColor;
        private AppearanceContext _context;
        private string _criteria;
        private bool? _enabled;
        private Color? _fontColor;
        private FontStyle? _fontStyle;
        private string _method;
        private String _name;
        private int _priority;
        private string _targetItems;
        private ViewItemVisibility? _viewItemVisibility;
        private Type _declaringType;

        public AppearanceRuleObject(Session session)
            : base(session){
        }

        public String Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public AppearanceItemType AppearanceItemType{
            get { return _appearanceItemType; }
            set { SetPropertyValue("AppearanceItemType", ref _appearanceItemType, value); }
        }

        [Browsable(false)]
        string IAppearanceRuleProperties.AppearanceItemType{
            get { return AppearanceItemType.ToString(); }
            set { AppearanceItemType = (AppearanceItemType) Enum.Parse(typeof (AppearanceItemType), value); }
        }

        
        public AppearanceContext Context{
            get { return _context; }
            set { SetPropertyValue("appearance_context", ref _context, value); }
        }

        [Browsable(false)]
        string IAppearanceRuleProperties.Context{
            get { return Context.ToString(); }
            set { Context = (AppearanceContext) Enum.Parse(typeof (AppearanceContext), value); }
        }

        [CriteriaOptions("DeclaringType")]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor), FieldSize(FieldSizeAttribute.Unlimited)]
        public string Criteria{
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
        public Type DeclaringType{
            get { return _declaringType; }
            set { SetPropertyValue("DeclaringType", ref _declaringType, value); }
        }

        public string Method{
            get { return _method; }
            set { SetPropertyValue("Method", ref _method, value); }
        }

        [EditorAlias(EditorAliases.CheckedListBoxEditor)]
        public string TargetItems{
            get { return _targetItems; }
            set { SetPropertyValue("TargetItems", ref _targetItems, value); }
        }

        [ValueConverter(typeof (ColorValueConverter))]
        public Color? BackColor{
            get { return _backColor; }
            set { SetPropertyValue("BackColor", ref _backColor, value); }
        }

        public bool? Enabled{
            get { return _enabled; }
            set { SetPropertyValue("Enabled", ref _enabled, value); }
        }

        [ValueConverter(typeof (ColorValueConverter))]
        public Color? FontColor{
            get { return _fontColor; }
            set { SetPropertyValue("FontColor", ref _fontColor, value); }
        }

        public FontStyle? FontStyle{
            get { return _fontStyle; }
            set { SetPropertyValue("FontStyle", ref _fontStyle, value); }
        }

        public int Priority{
            get { return _priority; }
            set { SetPropertyValue("Priority", ref _priority, value); }
        }

        public ViewItemVisibility? Visibility{
            get { return _viewItemVisibility; }
            set { SetPropertyValue("Visibility", ref _viewItemVisibility, value); }
        }

        public Dictionary<object, string> GetCheckedListBoxItems(string targetMemberName){
            var result = new Dictionary<object, string>();
            if (DeclaringType != null && targetMemberName == "TargetItems"){
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DeclaringType);
                foreach (IMemberInfo memberInfo in typeInfo.Members){
                    if (memberInfo.IsVisible){
                        result.Add(memberInfo.Name, CaptionHelper.GetMemberCaption(typeInfo, memberInfo.Name));
                    }
                }
            }
            return result;
        }

        event EventHandler ICheckedListBoxItemsProvider.ItemsChanged{
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}