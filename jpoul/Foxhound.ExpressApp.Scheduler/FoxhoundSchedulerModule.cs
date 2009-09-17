using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo.Metadata;

namespace Foxhound.ExpressApp.Scheduler {
    public sealed partial class FoxhoundSchedulerModule : ModuleBase {
        public const string IsUsedAsSchedulerResourceAttribute = "IsUsedAsSchedulerResource";

        public FoxhoundSchedulerModule() {
            InitializeComponent();
        }

        public override Schema GetSchema() {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                @"<Element Name=""Application"">
					<Element Name=""Views"">
						<Element Name=""ListView"">
							<Attribute Name=""ShowDateNavigator""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""False,True""
							/>
                            <Attribute Name=""AppointmentEndTimeVisibility""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.AppointmentTimeVisibility}""
							/>
                            <Attribute Name=""AppointmentStartTimeVisibility""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.AppointmentTimeVisibility}""
							/>
                            <Attribute Name=""AppointmentTimeDisplayType""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.AppointmentTimeDisplayType}""
							/>
                            <Attribute Name=""AppointmentContinueArrowDisplayType""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.AppointmentContinueArrowDisplayType}""
							/>
                            <Attribute Name=""SchedulerGroupType""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.SchedulerGroupType}""
							/>
                            <Attribute Name=""ShowDateTimeScrollBar""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""False,True""
							/>
                            <Attribute Name=""AppointmentStatusDisplayType""
								IsLocalized=""False"" 
								IsInvisible=""{DevExpress.ExpressApp.Scheduler.ResourceClassNameVisibilityCalculator}""
								Choice=""{DevExpress.XtraScheduler.AppointmentStatusDisplayType}""
							/>

						</Element>
					</Element>
				</Element>"));
        }
        
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            //var typeInfo = typesInfo.FindTypeInfo(typeof(Salesman));
            //XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;

            //var resourceInterfaceInfo = xpDictionary.GetClassInfo(typeof(IResource));
            //if (!typeInfo.Implements<IResource>()) {
            //    //typeInfo.
            //    var classInfo = xpDictionary.GetClassInfo(typeof(Salesman));
            //    classInfo.AddMember();


            //}
        }

//        public override Schema GetSchema(){
//            string schema = string.Format(
//                @"<Element Name=""Application""><Element Name=""BOModel"" >
//                            <Element Name=""Class"" >
//                                <Attribute Name=""{0}"" Choice=""True,False""/>
//                          </Element>
//                       </Element>
//                  </Element>
//               </Element>", IsUsedAsSchedulerResourceAttribute);
//            return new Schema(new DictionaryXmlReader().ReadFromString(schema));
//       }
    }
}