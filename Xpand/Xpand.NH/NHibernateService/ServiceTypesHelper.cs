using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Xpand.ExpressApp.NH.Core;
using Xpand.ExpressApp.NH.DataLayer;

namespace Xpand.ExpressApp.NH.Service
{
    static class ServiceTypesHelper
    {

        private static IList<Type> mappingTypes;
        private static IList<Type> knownTypes;

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            if (knownTypes == null)
            {
                List<Type> types = new List<Type>();

                foreach (var mappingType in MappingTypes)
                {
                    types.Add(mappingType.BaseType.GetGenericArguments()[0]);
                }
                types.Add(typeof(TypeMetadata));
                knownTypes = types;
            }

            return knownTypes;
        }
        public static IEnumerable<Type> MappingTypes
        {
            get
            {
                if (mappingTypes == null)
                {
                    mappingTypes = GetMappingTypes();

                }

                return mappingTypes;
            }
        }


        private static IList<Type> GetMappingTypes()
        {
            List<Type> result = new List<Type>();
            string location = typeof(ServiceTypesHelper).Assembly.Location;
            if (!string.IsNullOrEmpty(location) && File.Exists(location))
            {
                string directoryName = Path.GetDirectoryName(location);

                foreach (var file in Directory.GetFiles(directoryName, "*.dll"))
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = Assembly.LoadFrom(file);
                    }
                    catch (FileLoadException) { }

                    if (assembly != null)
                    {
                        try
                        {
                            foreach (var type in assembly.GetTypes().Where(IsMappingType))
                                result.Add(type);

                        }
                        catch (ReflectionTypeLoadException) { }
                        catch (TypeLoadException) { }
                    }
                }
            }
            return result;
        }

        private static bool IsMappingType(Type type)
        {
            return type.BaseType != null && type.BaseType.IsGenericType &&
                typeof(ClassMap<>).IsAssignableFrom(type.BaseType.GetGenericTypeDefinition()) && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
