using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Xpand.ExpressApp.ImportWizard.Core {
    public abstract class DynamicClass {
        public override string ToString() {
            var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var sb = new StringBuilder();
            sb.Append(@"{");
            for (var i = 0; i < props.Length; i++) {
                if (i > 0) sb.Append(@", ");
                sb.Append(props[i].Name);
                sb.Append(@"=");
                sb.Append(props[i].GetValue(this, null));
            }
            sb.Append(@"}");
            return sb.ToString();
        }
    }

    public class DynamicProperty {
        private readonly string _name;
        private readonly Type _type;

        /// <exclude/>
        /// <excludeToc/>
        public DynamicProperty(string name, Type type) {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            _name = name;
            _type = type;
        }

        public string Name {
            get {
                return _name;
            }
        }

        public Type Type {
            get {
                return _type;
            }
        }
    }


    internal class Signature : IEquatable<Signature> {
        public int HashCode;
        public DynamicProperty[] Properties;

        [SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily")]
        public Signature(IEnumerable<DynamicProperty> properties) {
            HashCode = 0;
            if (properties == null) return;

            Properties = properties.ToArray();
            foreach (var p in properties) {
                HashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public override bool Equals(object obj) {
            return obj is Signature && Equals((Signature)obj);
        }

        public override int GetHashCode() {
            return HashCode;
        }

        public bool Equals(Signature other) {
            if (Properties.Length != other.Properties.Length) return false;
            return !Properties.Where((t, i) => t.Name != other.Properties[i].Name || t.Type != other.Properties[i].Type).Any();
        }
    }
}