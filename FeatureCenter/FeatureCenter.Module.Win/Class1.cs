using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using System.Linq;
[assembly: Xpand.Persistent.Base.General.DataStoreAttribute(@"XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=quartz", @"XQuartz.QRTZ_BLOB_TRIGGERS")]
[assembly: System.Reflection.AssemblyVersionAttribute(@"1.0")]
namespace XQuartz { public class DynamicXQuartzModule : DevExpress.ExpressApp.ModuleBase { } }
namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_LOCKS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_LOCKS", @"QRTZ_LOCKS_ListView")]

    public class QRTZ_LOCKS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_LOCKS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();



        }





        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.KeyAttribute(false)]

        public XQuartz.QRTZ_LOCKSKeyStruct QRTZ_LOCKSKeyStruct;

    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_CALENDARS", @"QRTZ_CALENDARS_ListView")]

    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_CALENDARS")]

    public class QRTZ_CALENDARS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_CALENDARS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();



        }



        byte[] _CALENDAR;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for CALENDAR at QRTZ_CALENDARS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"CALENDAR")]

        public byte[] CALENDAR {

            get {

                return _CALENDAR;

            }

            set {

                SetPropertyValue("CALENDAR", ref _CALENDAR, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.KeyAttribute(false)]

        [DevExpress.Xpo.SizeAttribute(100)]

        public XQuartz.QRTZ_CALENDARSKeyStruct QRTZ_CALENDARSKeyStruct;

    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_FIRED_TRIGGERSKeyStruct {



        string _ENTRY_ID;

        [DevExpress.Xpo.PersistentAttribute(@"ENTRY_ID")]

        [DevExpress.Xpo.SizeAttribute(95)]

        public string ENTRY_ID {

            get {

                return _ENTRY_ID;

            }

            set {

                _ENTRY_ID = value;



            }

        }



        string _SCHED_NAME;

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_FIRED_TRIGGERS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_FIRED_TRIGGERS", @"QRTZ_FIRED_TRIGGERS_ListView")]

    public class QRTZ_FIRED_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_FIRED_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();



        }



        long _FIRED_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"FIRED_TIME")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for FIRED_TIME at QRTZ_FIRED_TRIGGERS", @"Save")]

        public long FIRED_TIME {

            get {

                return _FIRED_TIME;

            }

            set {

                SetPropertyValue("FIRED_TIME", ref _FIRED_TIME, value);

            }

        }



        string _TRIGGER_NAME;

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_NAME at QRTZ_FIRED_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_NAME")]

        public string TRIGGER_NAME {

            get {

                return _TRIGGER_NAME;

            }

            set {

                SetPropertyValue("TRIGGER_NAME", ref _TRIGGER_NAME, value);

            }

        }



        string _STATE;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for STATE at QRTZ_FIRED_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"STATE")]

        [DevExpress.Xpo.SizeAttribute(16)]

        public string STATE {

            get {

                return _STATE;

            }

            set {

                SetPropertyValue("STATE", ref _STATE, value);

            }

        }



        string _INSTANCE_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"INSTANCE_NAME")]

        [DevExpress.Xpo.SizeAttribute(200)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for INSTANCE_NAME at QRTZ_FIRED_TRIGGERS", @"Save")]

        public string INSTANCE_NAME {

            get {

                return _INSTANCE_NAME;

            }

            set {

                SetPropertyValue("INSTANCE_NAME", ref _INSTANCE_NAME, value);

            }

        }





        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.KeyAttribute(false)]

        public XQuartz.QRTZ_FIRED_TRIGGERSKeyStruct QRTZ_FIRED_TRIGGERSKeyStruct;

        string _TRIGGER_GROUP;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_GROUP at QRTZ_FIRED_TRIGGERS", @"Save")]

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_GROUP")]

        public string TRIGGER_GROUP {

            get {

                return _TRIGGER_GROUP;

            }

            set {

                SetPropertyValue("TRIGGER_GROUP", ref _TRIGGER_GROUP, value);

            }

        }



        int _PRIORITY;

        [DevExpress.Xpo.PersistentAttribute(@"PRIORITY")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for PRIORITY at QRTZ_FIRED_TRIGGERS", @"Save")]

        public int PRIORITY {

            get {

                return _PRIORITY;

            }

            set {

                SetPropertyValue("PRIORITY", ref _PRIORITY, value);

            }

        }



        string _IS_NONCONCURRENT;

        [DevExpress.Xpo.SizeAttribute(1)]

        [DevExpress.Xpo.PersistentAttribute(@"IS_NONCONCURRENT")]

        public string IS_NONCONCURRENT {

            get {

                return _IS_NONCONCURRENT;

            }

            set {

                SetPropertyValue("IS_NONCONCURRENT", ref _IS_NONCONCURRENT, value);

            }

        }



        string _REQUESTS_RECOVERY;

        [DevExpress.Xpo.PersistentAttribute(@"REQUESTS_RECOVERY")]

        [DevExpress.Xpo.SizeAttribute(1)]

        public string REQUESTS_RECOVERY {

            get {

                return _REQUESTS_RECOVERY;

            }

            set {

                SetPropertyValue("REQUESTS_RECOVERY", ref _REQUESTS_RECOVERY, value);

            }

        }



        string _JOB_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"JOB_NAME")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string JOB_NAME {

            get {

                return _JOB_NAME;

            }

            set {

                SetPropertyValue("JOB_NAME", ref _JOB_NAME, value);

            }

        }



        string _JOB_GROUP;

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Xpo.PersistentAttribute(@"JOB_GROUP")]

        public string JOB_GROUP {

            get {

                return _JOB_GROUP;

            }

            set {

                SetPropertyValue("JOB_GROUP", ref _JOB_GROUP, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_SIMPLE_TRIGGERS", @"QRTZ_SIMPLE_TRIGGERS_ListView")]

    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_SIMPLE_TRIGGERS")]

    public class QRTZ_SIMPLE_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_SIMPLE_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();

            if (QRTZ_SIMPLE_TRIGGERSKeyStruct.QRTZ_TRIGGERS.Session != Session) { QRTZ_SIMPLE_TRIGGERSKeyStruct.QRTZ_TRIGGERS = Session.GetObjectByKey<QRTZ_TRIGGERS>(QRTZ_SIMPLE_TRIGGERSKeyStruct.QRTZ_TRIGGERS.QRTZ_TRIGGERSKeyStruct); }

        }



        long _REPEAT_INTERVAL;

        [DevExpress.Xpo.PersistentAttribute(@"REPEAT_INTERVAL")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for REPEAT_INTERVAL at QRTZ_SIMPLE_TRIGGERS", @"Save")]

        public long REPEAT_INTERVAL {

            get {

                return _REPEAT_INTERVAL;

            }

            set {

                SetPropertyValue("REPEAT_INTERVAL", ref _REPEAT_INTERVAL, value);

            }

        }



        int _REPEAT_COUNT;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for REPEAT_COUNT at QRTZ_SIMPLE_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"REPEAT_COUNT")]

        public int REPEAT_COUNT {

            get {

                return _REPEAT_COUNT;

            }

            set {

                SetPropertyValue("REPEAT_COUNT", ref _REPEAT_COUNT, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.KeyAttribute(false)]

        public XQuartz.QRTZ_SIMPLE_TRIGGERSKeyStruct QRTZ_SIMPLE_TRIGGERSKeyStruct;

        int _TIMES_TRIGGERED;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TIMES_TRIGGERED at QRTZ_SIMPLE_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"TIMES_TRIGGERED")]

        public int TIMES_TRIGGERED {

            get {

                return _TIMES_TRIGGERED;

            }

            set {

                SetPropertyValue("TIMES_TRIGGERED", ref _TIMES_TRIGGERED, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_CRON_TRIGGERSKeyStruct {



        XQuartz.QRTZ_TRIGGERS _QRTZ_TRIGGERS;

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS")]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public XQuartz.QRTZ_TRIGGERS QRTZ_TRIGGERS {

            get {

                return _QRTZ_TRIGGERS;

            }

            set {

                _QRTZ_TRIGGERS = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_LOCKSKeyStruct {



        string _SCHED_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



        string _LOCK_NAME;

        [DevExpress.Xpo.SizeAttribute(40)]

        [DevExpress.Xpo.PersistentAttribute(@"LOCK_NAME")]

        public string LOCK_NAME {

            get {

                return _LOCK_NAME;

            }

            set {

                _LOCK_NAME = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_TRIGGERSKeyStruct {



        XQuartz.QRTZ_JOB_DETAILS _QRTZ_JOB_DETAILS;

        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS")]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        public XQuartz.QRTZ_JOB_DETAILS QRTZ_JOB_DETAILS {

            get {

                return _QRTZ_JOB_DETAILS;

            }

            set {

                _QRTZ_JOB_DETAILS = value;



            }

        }



        string _TRIGGER_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_NAME")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string TRIGGER_NAME {

            get {

                return _TRIGGER_NAME;

            }

            set {

                _TRIGGER_NAME = value;



            }

        }



        string _TRIGGER_GROUP;

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_GROUP")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string TRIGGER_GROUP {

            get {

                return _TRIGGER_GROUP;

            }

            set {

                _TRIGGER_GROUP = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_SCHEDULER_STATE", @"QRTZ_SCHEDULER_STATE_ListView")]

    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_SCHEDULER_STATE")]

    public class QRTZ_SCHEDULER_STATE : DevExpress.Xpo.XPLiteObject {

        public QRTZ_SCHEDULER_STATE(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();



        }



        long _LAST_CHECKIN_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"LAST_CHECKIN_TIME")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for LAST_CHECKIN_TIME at QRTZ_SCHEDULER_STATE", @"Save")]

        public long LAST_CHECKIN_TIME {

            get {

                return _LAST_CHECKIN_TIME;

            }

            set {

                SetPropertyValue("LAST_CHECKIN_TIME", ref _LAST_CHECKIN_TIME, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.KeyAttribute(false)]

        public XQuartz.QRTZ_SCHEDULER_STATEKeyStruct QRTZ_SCHEDULER_STATEKeyStruct;

        long _CHECKIN_INTERVAL;

        [DevExpress.Xpo.PersistentAttribute(@"CHECKIN_INTERVAL")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for CHECKIN_INTERVAL at QRTZ_SCHEDULER_STATE", @"Save")]

        public long CHECKIN_INTERVAL {

            get {

                return _CHECKIN_INTERVAL;

            }

            set {

                SetPropertyValue("CHECKIN_INTERVAL", ref _CHECKIN_INTERVAL, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_BLOB_TRIGGERS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_BLOB_TRIGGERS", @"QRTZ_BLOB_TRIGGERS_ListView")]

    public class QRTZ_BLOB_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_BLOB_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }



        string _SCHED_NAME;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for SCHED_NAME at QRTZ_BLOB_TRIGGERS", @"Save")]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                SetPropertyValue("SCHED_NAME", ref _SCHED_NAME, value);

            }

        }



        string _TRIGGER_NAME;

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_NAME at QRTZ_BLOB_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_NAME")]

        public string TRIGGER_NAME {

            get {

                return _TRIGGER_NAME;

            }

            set {

                SetPropertyValue("TRIGGER_NAME", ref _TRIGGER_NAME, value);

            }

        }



        byte[] _BLOB_DATA;

        [DevExpress.Xpo.PersistentAttribute(@"BLOB_DATA")]

        public byte[] BLOB_DATA {

            get {

                return _BLOB_DATA;

            }

            set {

                SetPropertyValue("BLOB_DATA", ref _BLOB_DATA, value);

            }

        }



        string _TRIGGER_GROUP;

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_GROUP")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_GROUP at QRTZ_BLOB_TRIGGERS", @"Save")]

        public string TRIGGER_GROUP {

            get {

                return _TRIGGER_GROUP;

            }

            set {

                SetPropertyValue("TRIGGER_GROUP", ref _TRIGGER_GROUP, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_PAUSED_TRIGGER_GRPS", @"QRTZ_PAUSED_TRIGGER_GRPS_ListView")]

    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_PAUSED_TRIGGER_GRPS")]

    public class QRTZ_PAUSED_TRIGGER_GRPS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_PAUSED_TRIGGER_GRPS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();



        }





        [DevExpress.Xpo.KeyAttribute(false)]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public XQuartz.QRTZ_PAUSED_TRIGGER_GRPSKeyStruct QRTZ_PAUSED_TRIGGER_GRPSKeyStruct;

    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_JOB_DETAILS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_JOB_DETAILS", @"QRTZ_JOB_DETAILS_ListView")]

    public class QRTZ_JOB_DETAILS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_JOB_DETAILS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }



        protected override void OnLoaded() {

            base.OnLoaded();



        }



        string _JOB_CLASS_NAME;

        [DevExpress.Xpo.SizeAttribute(250)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for JOB_CLASS_NAME at QRTZ_JOB_DETAILS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"JOB_CLASS_NAME")]

        public string JOB_CLASS_NAME {

            get {

                return _JOB_CLASS_NAME;

            }

            set {

                SetPropertyValue("JOB_CLASS_NAME", ref _JOB_CLASS_NAME, value);

            }

        }



        string _IS_NONCONCURRENT;

        [DevExpress.Xpo.PersistentAttribute(@"IS_NONCONCURRENT")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for IS_NONCONCURRENT at QRTZ_JOB_DETAILS", @"Save")]

        [DevExpress.Xpo.SizeAttribute(1)]

        public string IS_NONCONCURRENT {

            get {

                return _IS_NONCONCURRENT;

            }

            set {

                SetPropertyValue("IS_NONCONCURRENT", ref _IS_NONCONCURRENT, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.KeyAttribute(false)]

        [DevExpress.Xpo.SizeAttribute(100)]

        public XQuartz.QRTZ_JOB_DETAILSKeyStruct QRTZ_JOB_DETAILSKeyStruct;

        byte[] _JOB_DATA;

        [DevExpress.Xpo.PersistentAttribute(@"JOB_DATA")]

        public byte[] JOB_DATA {

            get {

                return _JOB_DATA;

            }

            set {

                SetPropertyValue("JOB_DATA", ref _JOB_DATA, value);

            }

        }



        string _REQUESTS_RECOVERY;

        [DevExpress.Xpo.PersistentAttribute(@"REQUESTS_RECOVERY")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for REQUESTS_RECOVERY at QRTZ_JOB_DETAILS", @"Save")]

        [DevExpress.Xpo.SizeAttribute(1)]

        public string REQUESTS_RECOVERY {

            get {

                return _REQUESTS_RECOVERY;

            }

            set {

                SetPropertyValue("REQUESTS_RECOVERY", ref _REQUESTS_RECOVERY, value);

            }

        }



        string _IS_UPDATE_DATA;

        [DevExpress.Xpo.SizeAttribute(1)]

        [DevExpress.Xpo.PersistentAttribute(@"IS_UPDATE_DATA")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for IS_UPDATE_DATA at QRTZ_JOB_DETAILS", @"Save")]

        public string IS_UPDATE_DATA {

            get {

                return _IS_UPDATE_DATA;

            }

            set {

                SetPropertyValue("IS_UPDATE_DATA", ref _IS_UPDATE_DATA, value);

            }

        }



        string _DESCRIPTION;

        [DevExpress.Xpo.PersistentAttribute(@"DESCRIPTION")]

        [DevExpress.Xpo.SizeAttribute(250)]

        public string DESCRIPTION {

            get {

                return _DESCRIPTION;

            }

            set {

                SetPropertyValue("DESCRIPTION", ref _DESCRIPTION, value);

            }

        }



        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS")]

        public XPCollection<XQuartz.QRTZ_TRIGGERS> QRTZ_TRIGGERSKeyStructQRTZ_JOB_DETAILSs {

            get {



                return GetCollection<XQuartz.QRTZ_TRIGGERS>("QRTZ_TRIGGERSKeyStructQRTZ_JOB_DETAILSs");

            }

        }



        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS")]

        public XPCollection<XQuartz.QRTZ_TRIGGERS> QRTZ_TRIGGERSQRTZ_JOB_DETAILSs {

            get {



                return GetCollection<XQuartz.QRTZ_TRIGGERS>("QRTZ_TRIGGERSQRTZ_JOB_DETAILSs");

            }

        }



        string _IS_DURABLE;

        [DevExpress.Xpo.PersistentAttribute(@"IS_DURABLE")]

        [DevExpress.Xpo.SizeAttribute(1)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for IS_DURABLE at QRTZ_JOB_DETAILS", @"Save")]

        public string IS_DURABLE {

            get {

                return _IS_DURABLE;

            }

            set {

                SetPropertyValue("IS_DURABLE", ref _IS_DURABLE, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_CRON_TRIGGERS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_CRON_TRIGGERS", @"QRTZ_CRON_TRIGGERS_ListView")]

    public class QRTZ_CRON_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_CRON_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();

            if (QRTZ_CRON_TRIGGERSKeyStruct.QRTZ_TRIGGERS.Session != Session) { QRTZ_CRON_TRIGGERSKeyStruct.QRTZ_TRIGGERS = Session.GetObjectByKey<QRTZ_TRIGGERS>(QRTZ_CRON_TRIGGERSKeyStruct.QRTZ_TRIGGERS.QRTZ_TRIGGERSKeyStruct); }

        }



        string _TIME_ZONE_ID;

        [DevExpress.Xpo.PersistentAttribute(@"TIME_ZONE_ID")]

        [DevExpress.Xpo.SizeAttribute(80)]

        public string TIME_ZONE_ID {

            get {

                return _TIME_ZONE_ID;

            }

            set {

                SetPropertyValue("TIME_ZONE_ID", ref _TIME_ZONE_ID, value);

            }

        }



        string _CRON_EXPRESSION;

        [DevExpress.Xpo.SizeAttribute(120)]

        [DevExpress.Xpo.PersistentAttribute(@"CRON_EXPRESSION")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for CRON_EXPRESSION at QRTZ_CRON_TRIGGERS", @"Save")]

        public string CRON_EXPRESSION {

            get {

                return _CRON_EXPRESSION;

            }

            set {

                SetPropertyValue("CRON_EXPRESSION", ref _CRON_EXPRESSION, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.KeyAttribute(false)]

        [DevExpress.Xpo.SizeAttribute(100)]

        public XQuartz.QRTZ_CRON_TRIGGERSKeyStruct QRTZ_CRON_TRIGGERSKeyStruct;

    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_PAUSED_TRIGGER_GRPSKeyStruct {



        string _SCHED_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



        string _TRIGGER_GROUP;

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_GROUP")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string TRIGGER_GROUP {

            get {

                return _TRIGGER_GROUP;

            }

            set {

                _TRIGGER_GROUP = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_TRIGGERS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_TRIGGERS", @"QRTZ_TRIGGERS_ListView")]

    public class QRTZ_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();

            if (QRTZ_TRIGGERSKeyStruct.QRTZ_JOB_DETAILS.Session != Session) { QRTZ_TRIGGERSKeyStruct.QRTZ_JOB_DETAILS = Session.GetObjectByKey<QRTZ_JOB_DETAILS>(QRTZ_TRIGGERSKeyStruct.QRTZ_JOB_DETAILS.QRTZ_JOB_DETAILSKeyStruct); }

        }



        long _PREV_FIRE_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"PREV_FIRE_TIME")]

        public long PREV_FIRE_TIME {

            get {

                return _PREV_FIRE_TIME;

            }

            set {

                SetPropertyValue("PREV_FIRE_TIME", ref _PREV_FIRE_TIME, value);

            }

        }



        string _CALENDAR_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"CALENDAR_NAME")]

        [DevExpress.Xpo.SizeAttribute(200)]

        public string CALENDAR_NAME {

            get {

                return _CALENDAR_NAME;

            }

            set {

                SetPropertyValue("CALENDAR_NAME", ref _CALENDAR_NAME, value);

            }

        }



        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS")]

        public XPCollection<XQuartz.QRTZ_SIMPLE_TRIGGERS> QRTZ_SIMPLE_TRIGGERSKeyStructQRTZ_TRIGGERSs {

            get {



                return GetCollection<XQuartz.QRTZ_SIMPLE_TRIGGERS>("QRTZ_SIMPLE_TRIGGERSKeyStructQRTZ_TRIGGERSs");

            }

        }



        int _PRIORITY;

        [DevExpress.Xpo.PersistentAttribute(@"PRIORITY")]

        public int PRIORITY {

            get {

                return _PRIORITY;

            }

            set {

                SetPropertyValue("PRIORITY", ref _PRIORITY, value);

            }

        }





        [DevExpress.Xpo.KeyAttribute(false)]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public XQuartz.QRTZ_TRIGGERSKeyStruct QRTZ_TRIGGERSKeyStruct;

        long _END_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"END_TIME")]

        public long END_TIME {

            get {

                return _END_TIME;

            }

            set {

                SetPropertyValue("END_TIME", ref _END_TIME, value);

            }

        }



        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS")]

        public XPCollection<XQuartz.QRTZ_CRON_TRIGGERS> QRTZ_CRON_TRIGGERSKeyStructQRTZ_TRIGGERSs {

            get {



                return GetCollection<XQuartz.QRTZ_CRON_TRIGGERS>("QRTZ_CRON_TRIGGERSKeyStructQRTZ_TRIGGERSs");

            }

        }



        byte[] _JOB_DATA;

        [DevExpress.Xpo.PersistentAttribute(@"JOB_DATA")]

        public byte[] JOB_DATA {

            get {

                return _JOB_DATA;

            }

            set {

                SetPropertyValue("JOB_DATA", ref _JOB_DATA, value);

            }

        }



        XQuartz.QRTZ_JOB_DETAILS _QRTZ_JOB_DETAILS;

        [DevExpress.Xpo.SizeAttribute(150)]

        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for JOB_NAME at QRTZ_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"")]

        public XQuartz.QRTZ_JOB_DETAILS QRTZ_JOB_DETAILS {

            get {

                return _QRTZ_JOB_DETAILS;

            }

            set {

                SetPropertyValue("QRTZ_JOB_DETAILS", ref _QRTZ_JOB_DETAILS, value);

            }

        }



        int _MISFIRE_INSTR;

        [DevExpress.Xpo.PersistentAttribute(@"MISFIRE_INSTR")]

        public int MISFIRE_INSTR {

            get {

                return _MISFIRE_INSTR;

            }

            set {

                SetPropertyValue("MISFIRE_INSTR", ref _MISFIRE_INSTR, value);

            }

        }



        string _TRIGGER_TYPE;

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_TYPE at QRTZ_TRIGGERS", @"Save")]

        [DevExpress.Xpo.SizeAttribute(8)]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_TYPE")]

        public string TRIGGER_TYPE {

            get {

                return _TRIGGER_TYPE;

            }

            set {

                SetPropertyValue("TRIGGER_TYPE", ref _TRIGGER_TYPE, value);

            }

        }



        string _DESCRIPTION;

        [DevExpress.Xpo.PersistentAttribute(@"DESCRIPTION")]

        [DevExpress.Xpo.SizeAttribute(250)]

        public string DESCRIPTION {

            get {

                return _DESCRIPTION;

            }

            set {

                SetPropertyValue("DESCRIPTION", ref _DESCRIPTION, value);

            }

        }



        long _START_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"START_TIME")]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for START_TIME at QRTZ_TRIGGERS", @"Save")]

        public long START_TIME {

            get {

                return _START_TIME;

            }

            set {

                SetPropertyValue("START_TIME", ref _START_TIME, value);

            }

        }



        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS")]

        public XPCollection<XQuartz.QRTZ_SIMPROP_TRIGGERS> QRTZ_SIMPROP_TRIGGERSKeyStructQRTZ_TRIGGERSs {

            get {



                return GetCollection<XQuartz.QRTZ_SIMPROP_TRIGGERS>("QRTZ_SIMPROP_TRIGGERSKeyStructQRTZ_TRIGGERSs");

            }

        }



        long _NEXT_FIRE_TIME;

        [DevExpress.Xpo.PersistentAttribute(@"NEXT_FIRE_TIME")]

        public long NEXT_FIRE_TIME {

            get {

                return _NEXT_FIRE_TIME;

            }

            set {

                SetPropertyValue("NEXT_FIRE_TIME", ref _NEXT_FIRE_TIME, value);

            }

        }



        string _TRIGGER_STATE;

        [DevExpress.Xpo.SizeAttribute(16)]

        [DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@"RuleRequired for TRIGGER_STATE at QRTZ_TRIGGERS", @"Save")]

        [DevExpress.Xpo.PersistentAttribute(@"TRIGGER_STATE")]

        public string TRIGGER_STATE {

            get {

                return _TRIGGER_STATE;

            }

            set {

                SetPropertyValue("TRIGGER_STATE", ref _TRIGGER_STATE, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_SIMPROP_TRIGGERSKeyStruct {



        XQuartz.QRTZ_TRIGGERS _QRTZ_TRIGGERS;

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS")]

        public XQuartz.QRTZ_TRIGGERS QRTZ_TRIGGERS {

            get {

                return _QRTZ_TRIGGERS;

            }

            set {

                _QRTZ_TRIGGERS = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_SIMPLE_TRIGGERSKeyStruct {



        XQuartz.QRTZ_TRIGGERS _QRTZ_TRIGGERS;

        [DevExpress.Xpo.AssociationAttribute(@"FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS")]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public XQuartz.QRTZ_TRIGGERS QRTZ_TRIGGERS {

            get {

                return _QRTZ_TRIGGERS;

            }

            set {

                _QRTZ_TRIGGERS = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_CALENDARSKeyStruct {



        string _CALENDAR_NAME;

        [DevExpress.Xpo.SizeAttribute(200)]

        [DevExpress.Xpo.PersistentAttribute(@"CALENDAR_NAME")]

        public string CALENDAR_NAME {

            get {

                return _CALENDAR_NAME;

            }

            set {

                _CALENDAR_NAME = value;



            }

        }



        string _SCHED_NAME;

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_JOB_DETAILSKeyStruct {



        string _JOB_GROUP;

        [DevExpress.Xpo.PersistentAttribute(@"JOB_GROUP")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string JOB_GROUP {

            get {

                return _JOB_GROUP;

            }

            set {

                _JOB_GROUP = value;



            }

        }



        string _JOB_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"JOB_NAME")]

        [DevExpress.Xpo.SizeAttribute(150)]

        public string JOB_NAME {

            get {

                return _JOB_NAME;

            }

            set {

                _JOB_NAME = value;



            }

        }



        string _SCHED_NAME;

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;



    [DevExpress.Xpo.PersistentAttribute(@"QRTZ_SIMPROP_TRIGGERS")]

    [Xpand.ExpressApp.Attributes.XpandNavigationItemAttribute(@"WorldCreator/Quartz/BOModel/QRTZ_SIMPROP_TRIGGERS", @"QRTZ_SIMPROP_TRIGGERS_ListView")]

    public class QRTZ_SIMPROP_TRIGGERS : DevExpress.Xpo.XPLiteObject {

        public QRTZ_SIMPROP_TRIGGERS(Session session) : base(session) { }

        public override void AfterConstruction() {

            base.AfterConstruction();

        }



        protected override void OnSaving() {

            base.OnSaving();

        }



        protected override void OnChanged(string propertyName, object oldValue, object newValue) {

            base.OnChanged(propertyName, oldValue, newValue);

        }

        protected override void OnLoaded() {

            base.OnLoaded();

            if (QRTZ_SIMPROP_TRIGGERSKeyStruct.QRTZ_TRIGGERS.Session != Session) { QRTZ_SIMPROP_TRIGGERSKeyStruct.QRTZ_TRIGGERS = Session.GetObjectByKey<QRTZ_TRIGGERS>(QRTZ_SIMPROP_TRIGGERSKeyStruct.QRTZ_TRIGGERS.QRTZ_TRIGGERSKeyStruct); }

        }



        string _BOOL_PROP_1;

        [DevExpress.Xpo.SizeAttribute(1)]

        [DevExpress.Xpo.PersistentAttribute(@"BOOL_PROP_1")]

        public string BOOL_PROP_1 {

            get {

                return _BOOL_PROP_1;

            }

            set {

                SetPropertyValue("BOOL_PROP_1", ref _BOOL_PROP_1, value);

            }

        }



        string _STR_PROP_2;

        [DevExpress.Xpo.PersistentAttribute(@"STR_PROP_2")]

        [DevExpress.Xpo.SizeAttribute(512)]

        public string STR_PROP_2 {

            get {

                return _STR_PROP_2;

            }

            set {

                SetPropertyValue("STR_PROP_2", ref _STR_PROP_2, value);

            }

        }



        int _INT_PROP_2;

        [DevExpress.Xpo.PersistentAttribute(@"INT_PROP_2")]

        public int INT_PROP_2 {

            get {

                return _INT_PROP_2;

            }

            set {

                SetPropertyValue("INT_PROP_2", ref _INT_PROP_2, value);

            }

        }



        int _INT_PROP_1;

        [DevExpress.Xpo.PersistentAttribute(@"INT_PROP_1")]

        public int INT_PROP_1 {

            get {

                return _INT_PROP_1;

            }

            set {

                SetPropertyValue("INT_PROP_1", ref _INT_PROP_1, value);

            }

        }





        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        [DevExpress.Xpo.KeyAttribute(false)]

        public XQuartz.QRTZ_SIMPROP_TRIGGERSKeyStruct QRTZ_SIMPROP_TRIGGERSKeyStruct;

        long _LONG_PROP_1;

        [DevExpress.Xpo.PersistentAttribute(@"LONG_PROP_1")]

        public long LONG_PROP_1 {

            get {

                return _LONG_PROP_1;

            }

            set {

                SetPropertyValue("LONG_PROP_1", ref _LONG_PROP_1, value);

            }

        }



        decimal _DEC_PROP_1;

        [DevExpress.Xpo.PersistentAttribute(@"DEC_PROP_1")]

        public decimal DEC_PROP_1 {

            get {

                return _DEC_PROP_1;

            }

            set {

                SetPropertyValue("DEC_PROP_1", ref _DEC_PROP_1, value);

            }

        }



        string _STR_PROP_1;

        [DevExpress.Xpo.SizeAttribute(512)]

        [DevExpress.Xpo.PersistentAttribute(@"STR_PROP_1")]

        public string STR_PROP_1 {

            get {

                return _STR_PROP_1;

            }

            set {

                SetPropertyValue("STR_PROP_1", ref _STR_PROP_1, value);

            }

        }



        string _BOOL_PROP_2;

        [DevExpress.Xpo.SizeAttribute(1)]

        [DevExpress.Xpo.PersistentAttribute(@"BOOL_PROP_2")]

        public string BOOL_PROP_2 {

            get {

                return _BOOL_PROP_2;

            }

            set {

                SetPropertyValue("BOOL_PROP_2", ref _BOOL_PROP_2, value);

            }

        }



        long _LONG_PROP_2;

        [DevExpress.Xpo.PersistentAttribute(@"LONG_PROP_2")]

        public long LONG_PROP_2 {

            get {

                return _LONG_PROP_2;

            }

            set {

                SetPropertyValue("LONG_PROP_2", ref _LONG_PROP_2, value);

            }

        }



        decimal _DEC_PROP_2;

        [DevExpress.Xpo.PersistentAttribute(@"DEC_PROP_2")]

        public decimal DEC_PROP_2 {

            get {

                return _DEC_PROP_2;

            }

            set {

                SetPropertyValue("DEC_PROP_2", ref _DEC_PROP_2, value);

            }

        }



        string _STR_PROP_3;

        [DevExpress.Xpo.PersistentAttribute(@"STR_PROP_3")]

        [DevExpress.Xpo.SizeAttribute(512)]

        public string STR_PROP_3 {

            get {

                return _STR_PROP_3;

            }

            set {

                SetPropertyValue("STR_PROP_3", ref _STR_PROP_3, value);

            }

        }



    }

}

namespace XQuartz {

    using System;

    using System.Collections.Generic;

    using System.ComponentModel;

    using DevExpress.Persistent.Base;

    using DevExpress.Persistent.Validation;

    using DevExpress.Xpo;

    using Xpand.Persistent.Base.PersistentMetaData;

    using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;



    using System.Linq;





    public struct QRTZ_SCHEDULER_STATEKeyStruct {



        string _SCHED_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"SCHED_NAME")]

        [DevExpress.Xpo.SizeAttribute(100)]

        public string SCHED_NAME {

            get {

                return _SCHED_NAME;

            }

            set {

                _SCHED_NAME = value;



            }

        }



        string _INSTANCE_NAME;

        [DevExpress.Xpo.PersistentAttribute(@"INSTANCE_NAME")]

        [DevExpress.Xpo.SizeAttribute(200)]

        public string INSTANCE_NAME {

            get {

                return _INSTANCE_NAME;

            }

            set {

                _INSTANCE_NAME = value;



            }

        }



    }

}

