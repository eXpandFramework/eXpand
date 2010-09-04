using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Xpand.Utils.DependentAssembly
{
    [Serializable]
    public class DependentAttributeInspector : MarshalByRefObject, IDependentAssemblyInspector
    {
        string _path;
        #region IAssemblyInspector Members

        public List<string> GetAssemblies(string path)
        {
            _path = path;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            Assembly assembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(path));
            var locations = assembly.GetCustomAttributes(typeof(DependentAssemblyAttribute), true).OfType<DependentAssemblyAttribute>().Select(attribute => attribute.Assembly.Location).ToList();
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            return locations;
        }

        Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_path != null)
            {
                string path = Path.Combine(Path.GetDirectoryName(_path), args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll");
                return Assembly.LoadFile(path);
            }
            throw new NotImplementedException();    
        }
        #endregion

    }

}
