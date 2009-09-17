
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.Persistent.TaxonomyImpl{

    [DefaultProperty("FullAddress")]
    [CalculatedPersistentAlias("FullAddress", "FullAddressPersistentAlias")]
    public class Address : TaxonomyBaseObject, IAddress{
        private const string defaultFullAddressFormat = "{Country.Name}; {StateProvince}; {City}; {Street}; {ZipPostal}";
        private const string defaultfullAddressPersistentAlias = "Country.Name + StateProvince + City + Street + ZipPostal";

        private static string fullAddressPersistentAlias = defaultfullAddressPersistentAlias;
        private readonly AddressImpl address = new AddressImpl();

        static Address(){
            AddressImpl.FullAddressFormat = defaultFullAddressFormat;
        }

        public Address(Session session) : base(session) {}

        [Obsolete("Use SetFullNameFormat instead")]
        public static string FullAddressFormat{
            get { return AddressImpl.FullAddressFormat; }
            set { AddressImpl.FullAddressFormat = value; }
        }

        public static string FullAddressPersistentAlias{
            get { return fullAddressPersistentAlias; }
        }

        public Country Country{
            get { return address.Country as Country; }
            set{
                address.Country = value;
                OnChanged("Country");
            }
        }
        #region IAddress Members
        public string Street{
            get { return address.Street; }
            set{
                address.Street = value;
                OnChanged("Street");
            }
        }

        public string City{
            get { return address.City; }
            set{
                address.City = value;
                OnChanged("City");
            }
        }

        public string StateProvince{
            get { return address.StateProvince; }
            set{
                address.StateProvince = value;
                OnChanged("StateProvince");
            }
        }

        public string ZipPostal{
            get { return address.ZipPostal; }
            set{
                address.ZipPostal = value;
                OnChanged("ZipPostal");
            }
        }

        ICountry IAddress.Country{
            get { return address.Country; }
            set{
                address.Country = value;
                OnChanged("Country");
            }
        }

        [Persistent]
        public string FullAddress{
            get { return address.FullAddress; }
        }
        #endregion
        public static void SetFullAddressFormat(string format, string persistentAlias){
            AddressImpl.FullAddressFormat = format;
            fullAddressPersistentAlias = persistentAlias;
        }
    }
}