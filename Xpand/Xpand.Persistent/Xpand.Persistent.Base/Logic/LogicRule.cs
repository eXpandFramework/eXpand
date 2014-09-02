﻿using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.Logic {
    public interface ILogicRuleObject:ILogicRule {
        ExecutionContext ExecutionContext { get; set; }
        FrameTemplateContext FrameTemplateContext { get; set; }
        HashSet<string> Views { get; }
        HashSet<string> ObjectChangedPropertyNames { get; }
    }

    public abstract class LogicRule : ILogicRuleObject {
        readonly HashSet<string> _views=new HashSet<string>();
        private readonly HashSet<string> _objectChangedPropertyNames=new HashSet<string>();

        protected LogicRule(IContextLogicRule logicRule) {
            Description = logicRule.Description;
            ID = logicRule.Id;
            Index = logicRule.Index;
            IsRootView = logicRule.IsRootView;
            Nesting = logicRule.Nesting;
            TypeInfo = logicRule.TypeInfo;
            View = logicRule.View;
            ViewType = logicRule.ViewType;
            ViewEditMode = logicRule.ViewEditMode;
            NormalCriteria=logicRule.NormalCriteria;
            EmptyCriteria=logicRule.EmptyCriteria;
            IsNew = logicRule.IsNew;
        }

        public string ID { get; set; }
        #region ILogicRule Members
        
        public ViewEditMode? ViewEditMode { get; set; }

        public string NormalCriteria { get; set; }

        public string EmptyCriteria { get; set; }
        public bool? IsNew { get; set; }

        public bool? IsRootView { get; set; }

        public ViewType ViewType { get; set; }

        public IModelView View { get; set; }

        public Nesting Nesting { get; set; }


        string IRule.Id {
            get { return ID; }
            set { ID = value; }
        }

        public string Description { get; set; }

        public ITypeInfo TypeInfo { get; set; }

        public int? Index { get; set; }

        public ExecutionContext ExecutionContext { get; set; }

        public FrameTemplateContext FrameTemplateContext { get; set; }

        public HashSet<string> Views {
            get { return _views; }
        }
        public HashSet<string> ObjectChangedPropertyNames {
            get { return _objectChangedPropertyNames; }
        }
        #endregion
        public override string ToString() {
            return !string.IsNullOrEmpty(ID) ? ID : base.ToString();
        }
    }
}