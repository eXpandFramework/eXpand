using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.Utils;

namespace FeatureCenter.Base {
    public class DummyDataBuilder {
        readonly ObjectSpace _objectSpace;
        public DummyDataBuilder(ObjectSpace objectSpace) {
            _objectSpace = objectSpace;
        }

        public void CreateObjects() {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes.Where(info => typeof(ICustomer).IsAssignableFrom(info.Type))) {
                if (!typeInfo.IsPersistent || _objectSpace.FindObject(typeInfo.Type, null) != null) continue;
                var dummyDataAttribute = typeInfo.FindAttribute<DummyDataAttribute>();
                if (dummyDataAttribute != null && dummyDataAttribute.Exclude) continue;
                CreateObjects(typeInfo);
                _objectSpace.CommitChanges();
            }
            
        }

        void CreateObjects(ITypeInfo typeInfo) {
            var citynames = new List<string>(GetCitynames());
            XpandReflectionHelper.Shuffle(citynames);
            var rand = new Random();
            int i = 0;
            foreach (string name in GetCustomerNames()) {
                i++;
                var customer = (ICustomer)ReflectionHelper.CreateObject(typeInfo.Type, new object[] { _objectSpace.Session });
                customer.Name = name;
                customer.City = citynames.Count >= i ? citynames[i - 1] : citynames[rand.Next(citynames.Count - 1)];
                customer.Description = "Here is some description for " + customer.Name;
                ITypeInfo ordersTypeInfo = GetOrdersTypeInfo(typeInfo);
                if (ordersTypeInfo != null)
                    CreateOrders(customer, ordersTypeInfo);
             
            }
        }

        void CreateOrders(ICustomer customer, ITypeInfo ordersTypeInfo) {
            var rand = new Random();
            string[] articles = GetArticles();
            Dictionary<string, float> artprice = articles.ToDictionary<string, string, float>(ar => ar, ar => rand.Next(10, 200));
            for (int i = 0; i < rand.Next(5, 20); i++) {
                var order = GetOrder(rand, customer, i, ordersTypeInfo);
                ITypeInfo orderLineTypeInfo = GetOrderLineTypeInfo(ordersTypeInfo);
                if (orderLineTypeInfo != null)
                    CreateOrderLine(articles, rand, order, artprice, orderLineTypeInfo);
             
            }
        }

        void CreateOrderLine(string[] articles, Random rand, IOrder order, Dictionary<string, float> artprice, ITypeInfo orderLineTypeInfo) {
            for (int j = 0; j < rand.Next(2, 8); j++) {
                var line = (IOrderLine)ReflectionHelper.CreateObject(orderLineTypeInfo.Type, new object[] { _objectSpace.Session });
                line.Order = order;
                line.Article = articles[rand.Next(articles.Length)];
                line.UnitPrice = artprice[line.Article];
                line.Quantity = rand.Next(1, 4);
                line.OrderLineDate = DateTime.Now.AddDays((-rand.Next(365 * 3)));
                order.Total += line.TotalPrice;
            }
        }

        IOrder GetOrder(Random rand, ICustomer customer, int counter, ITypeInfo ordersTypeInfo) {
            var order = (IOrder)ReflectionHelper.CreateObject(ordersTypeInfo.Type, new object[] { _objectSpace.Session });
            order.Customer = customer;
            order.OrderDate = DateTime.Now.AddDays((-rand.Next(365 * 3)));
            order.Reference = String.Format("ORD{0}", counter + 1000);
            order.Total = 0F;
            return order;
        }

        string[] GetArticles() {
            return new[] {
                             "X-Ray glasses", "Laser gun", "Photon torpedoe", "Candle pack",
                             "Electric Toothbush",
                             "Batteries", "Mobile phone", "Set of saucepans", "WiFI access Point",
                             "Talking robot"
                         };
        }


        ITypeInfo GetOrdersTypeInfo(ITypeInfo typeInfo) {
            return GetTypeInfo(typeInfo, typeof(IOrder));
        }

        ITypeInfo GetTypeInfo(ITypeInfo typeInfo, Type type) {
            IMemberInfo firstOrDefault = typeInfo.OwnMembers.Where(info => info.IsList && type.IsAssignableFrom(info.ListElementType)).FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.ListElementTypeInfo : null;
        }

        ITypeInfo GetOrderLineTypeInfo(ITypeInfo ordersTypeInfo) {
            return GetTypeInfo(ordersTypeInfo, typeof(IOrderLine));
        }

        IEnumerable<string> GetCustomerNames() {
            return new[] {
                             "John SMITH", "Tom SAILOR", "Robert DUVAL", "Ken WONG ", "Jerry MERRY",
                             "Maria JONES", "James T KIRK", "Jean-Luc PICARD", "Benjamin CISCO",
                             "Jordy LAFORGE"
                         };
        }

        IEnumerable<string> GetCitynames() {
            return new[] {
                             "New York", "Paris", "London", "Hong Kong"
                         };
        }

    }
}