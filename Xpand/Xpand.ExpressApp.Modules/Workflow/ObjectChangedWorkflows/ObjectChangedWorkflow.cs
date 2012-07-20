using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public enum ExecutionDomain {
        Server,
        Client
    }

    [DefaultClassOptions]
    [NavigationItem("Workflow")]
    public class ObjectChangedWorkflow : XpandCustomObject, IXpandWorkflowDefinition {
        public const string InitialXaml =
            @"<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.xWF1"" 
    xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" 
    xmlns:av=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
    xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v10.2"" 
    xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v10.2"" 
    xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v10.2"" 
    xmlns:local=""clr-namespace:DevExpress.Workflow"" 
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" 
    xmlns:mv=""clr-namespace:Microsoft.VisualBasic;assembly=System"" 
    xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities"" 
    xmlns:s=""clr-namespace:System;assembly=mscorlib"" 
    xmlns:s1=""clr-namespace:System;assembly=System"" 
    xmlns:s2=""clr-namespace:System;assembly=System.Xml"" 
    xmlns:s3=""clr-namespace:System;assembly=System.Core"" 
    xmlns:s4=""clr-namespace:System;assembly=System.ServiceModel"" 
    xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities"" 
    xmlns:sad=""clr-namespace:System.Activities.Debugger;assembly=System.Activities"" 
    xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" 
    xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=System"" 
    xmlns:scg1=""clr-namespace:System.Collections.Generic;assembly=System.ServiceModel"" 
    xmlns:scg2=""clr-namespace:System.Collections.Generic;assembly=System.Core"" 
    xmlns:scg3=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" 
    xmlns:sd=""clr-namespace:System.Data;assembly=System.Data"" 
    xmlns:sl=""clr-namespace:System.Linq;assembly=System.Core"" 
    xmlns:st=""clr-namespace:System.Text;assembly=mscorlib"" 
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	  <x:Members>
		<x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
        <x:Property Name=""propertyName"" Type=""InArgument(x:String)"" />
        <x:Property Name=""oldValue"" Type=""InArgument(x:Object)"" />
	  </x:Members>
</Activity>
";
        public override void AfterConstruction() {
            base.AfterConstruction();
            Xaml = InitialXaml;
        }

        ExecutionDomain _executionDomain;

        string _propertyName;

        Type _targetObjectType;

        public ObjectChangedWorkflow(Session session)
            : base(session) {
        }


        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type TargetObjectType {
            get { return _targetObjectType; }
            set { SetPropertyValue("TargetObjectType", ref _targetObjectType, value); }
        }


        [PropertyEditor(typeof(IStringLookupPropertyEditor))]
        [DataSourceProperty("PropertyNames")]
        public string PropertyName {
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }

        [Browsable(false)]
        public IList<string> PropertyNames {
            get { return TargetObjectType == null ? new List<string>() : XafTypesInfo.CastTypeToTypeInfo(TargetObjectType).OwnMembers.Select(info => info.Name).ToList(); }
        }

        public ExecutionDomain ExecutionDomain {
            get { return _executionDomain; }
            set { SetPropertyValue("ExecutionDomain", ref _executionDomain, value); }
        }
        #region IXpandWorkflowDefinition Members
        public string GetActivityTypeName() {
            return GetUniqueId();
        }

        public IList<IStartWorkflowCondition> GetConditions() {
            return new IStartWorkflowCondition[0];
        }

        public string GetUniqueId() {
            if (Session.IsNewObject(this)) {
                throw new InvalidOperationException();
            }
            return "ObjectChangedWorkflow" + Oid.ToString().ToUpper().Replace("-", "_");
        }

        [Browsable(false)]
        public bool CanCompile {
            get { return false; }
        }

        public bool IsActive {
            get { return GetPropertyValue<bool>("IsActive"); }
            set { SetPropertyValue("IsActive", value); }
        }

        [Browsable(false)]
        public bool CanOpenHost {
            get { return IsActive && !string.IsNullOrEmpty(Name); }
        }

        public string Name {
            get { return GetPropertyValue<string>("Name"); }
            set { SetPropertyValue("Name", value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [Custom("PropertyEditorType", "DevExpress.ExpressApp.Workflow.Win.WorkflowPropertyEditor")]
        public string Xaml {
            get { return GetPropertyValue<string>("Xaml"); }
            set { SetPropertyValue("Xaml", value); }
        }
        #endregion
    }
}