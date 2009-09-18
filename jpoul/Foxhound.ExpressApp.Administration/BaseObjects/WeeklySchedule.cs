using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class WeeklySchedule : TaxonomyBaseObject{
        public WeeklySchedule(Session session) : base(session){

        }


        private int weekNumber;
        public int WeekNumber
        {
            get
            {
                return weekNumber;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref weekNumber, value);
            }
        }

        private int year;
        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref year, value);
            }
        }

        private CompanyUnit companyUnit;
        [ProvidedAssociation(Associations.CompanyUnitWeeklySchedules)]
        [Association(Associations.CompanyUnitWeeklySchedules)]
        public CompanyUnit CompanyUnit
        {
            get
            {
                return companyUnit;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref companyUnit, value);
            }
        }


    }
}