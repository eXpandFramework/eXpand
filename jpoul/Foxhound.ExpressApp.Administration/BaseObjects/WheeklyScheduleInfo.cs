using System;
using System.Reflection;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using DevExpress.Xpo;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    public class SalesInfo:BaseObjectInfo{
        public SalesInfo(Session session) : base(session){
        }
        private Term term;
        [ProvidedAssociation(Associations.TermSalesInfos)]
        [Association(Associations.TermSalesInfos)]
        public Term Term
        {
            get
            {
                return term;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref term, value);
            }
        }
        private Term productCategory;
        [ProvidedAssociation(Associations.TermSalesInfosProductCategory)]
        [Association(Associations.TermSalesInfosProductCategory)]
        public Term ProductCategory
        {
            get
            {
                return productCategory;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref productCategory, value);
            }
        }
        
        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref date, value);
            }
        }
    }
    [DefaultClassOptions]
    public class WheeklyScheduleInfo : BaseObjectInfo
    {
        public WheeklyScheduleInfo(Session session) : base(session){
        }

        public WheeklyScheduleInfo(){


        }

        private DateTime day;
        public DateTime Day
        {
            get
            {
                return day;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref day, value);
            }
        }


        private string notes;
        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref notes, value);
            }
        }

        private string hourTo;
        public string HourTo
        {
            get
            {
                return hourTo;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref hourTo, value);
            }
        }

        private string hourFrom;
        public string HourFrom
        {
            get
            {
                return hourFrom;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref hourFrom, value);
            }
        }

        private Employee employee;
        [ProvidedAssociation(Associations.EmployeeWheeklyScheduleInfos)]
        [Association(Associations.EmployeeWheeklyScheduleInfos)]
        public Employee Employee
        {
            get
            {
                return employee;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref employee, value);
            }
        }
    }

    public class WeeklyScheduleEvaluationInfo : WheeklyScheduleInfo{
        public WeeklyScheduleEvaluationInfo(Session session) : base(session){
        }
    }
    public class WeeklyScheduleOvertimeInfo : WheeklyScheduleInfo
    {
        public WeeklyScheduleOvertimeInfo(Session session) : base(session){
        }
    }
    public class WeeklyScheduleVolunterayInfo : WheeklyScheduleInfo
    {
        public WeeklyScheduleVolunterayInfo(Session session) : base(session){
        }
    }

}