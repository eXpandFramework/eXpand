using CommandLine;
using CommandLine.Text;

namespace GacInstaller {
    public enum Mode{
        Install,
        UnInstall
    }
    public class Options {
        [Option('r',  HelpText = "There should be a mtach with assembly names")]
        public string Regex { get; set; }

        [Option('m', HelpText = "Installer Mode = (Install or Uninstall)",DefaultValue = Mode.Install)]
        public Mode Mode { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
