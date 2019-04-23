using CommandLine;
using CommandLine.Text;

namespace BuildHelper {
    public class Options {
        [Option("afterbuild")]
        public bool AfterBuild { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
        

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
