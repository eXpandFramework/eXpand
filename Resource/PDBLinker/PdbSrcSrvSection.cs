using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDBLinker{
    public class PdbSrcSrvSection{
        private const string SrcSrvIni = @"SRCSRV: ini ------------------------------------------------";
        private const string SrcSrvVariables = @"SRCSRV: variables ------------------------------------------";
        private const string SrcSrvSources = @"SRCSRV: source files ---------------------------------------";
        private const string SrcSrvEnd = @"SRCSRV: end ------------------------------------------------";

        private const string WindowsNewLine = "\r\n";

        public PdbSrcSrvSection(){
            Ini = new Dictionary<string, string>();
            Variables = new Dictionary<string, string>();
            Sources = new List<IList<string>>();
        }

        public IDictionary<string, string> Ini { get; private set; }
        public IDictionary<string, string> Variables { get; private set; }
        public IList<IList<string>> Sources { get; private set; }


        public static PdbSrcSrvSection Parse(string text){
            if (string.IsNullOrEmpty(text))
                return null;

            var result = new PdbSrcSrvSection();

            string ini = text.Remove(0, text.IndexOf(SrcSrvIni, System.StringComparison.Ordinal) + SrcSrvIni.Length);
            ini = ini.Remove(ini.IndexOf(SrcSrvVariables, System.StringComparison.Ordinal)).Trim();
            string variables = text.Remove(0, text.IndexOf(SrcSrvVariables, System.StringComparison.Ordinal) + SrcSrvVariables.Length);
            variables = variables.Remove(variables.IndexOf(SrcSrvSources, System.StringComparison.Ordinal)).Trim();
            string sources = text.Remove(0, text.IndexOf(SrcSrvSources, System.StringComparison.Ordinal) + SrcSrvSources.Length);
            sources = sources.Remove(sources.IndexOf(SrcSrvEnd, System.StringComparison.Ordinal)).Trim();

            foreach (string line in ini.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                result.Ini.Add(line.Remove(line.IndexOf("=", System.StringComparison.Ordinal)), line.Substring(line.IndexOf("=", System.StringComparison.Ordinal) + 1));

            foreach (string line in variables.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                result.Variables.Add(line.Remove(line.IndexOf("=", System.StringComparison.Ordinal)), line.Substring(line.IndexOf("=", System.StringComparison.Ordinal) + 1));

            foreach (string line in sources.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                result.Sources.Add(new List<string>(line.Split('*')));

            return result;
        }

        public override string ToString(){
            var sb = new StringBuilder();

            sb.Append(SrcSrvIni).Append(WindowsNewLine);
            foreach (var ini in Ini)
                sb.AppendFormat("{0}={1}", ini.Key, ini.Value).Append(WindowsNewLine);

            sb.Append(SrcSrvVariables).Append(WindowsNewLine);
            foreach (var variable in Variables)
                sb.AppendFormat("{0}={1}", variable.Key, variable.Value).Append(WindowsNewLine);

            sb.Append(SrcSrvSources).Append(WindowsNewLine);
            foreach (var source in Sources)
                sb.Append(string.Join("*", source.ToArray())).Append(WindowsNewLine);

            sb.Append(SrcSrvEnd).Append(WindowsNewLine);

            return sb.ToString();
        }
    }
}