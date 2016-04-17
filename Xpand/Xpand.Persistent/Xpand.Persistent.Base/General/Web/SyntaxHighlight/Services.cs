using System;
using System.ComponentModel;
using System.Globalization;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight {
    public abstract class SyntaxHighLightLanguagesProviderBase : TypeConverter {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return value;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(Values);
        }

        public abstract string[] Values { get; }
    }
    public class SyntaxHighLightModeProvider : SyntaxHighLightLanguagesProviderBase {

        public override string[] Values
        {
            get
            {
                return new[]{
                    "coffee",
                    "css",
                    "folding",
                    "html",
                    "javascript",
                    "json",
                    "lua",
                    "php",
                    "xml",
                    "xquery",
                    "diff",
                    "django",
                    "dockerfile",
                    "dot",
                    "drools",
                    "eiffel",
                    "ejs",
                    "elixir",
                    "elm",
                    "erlang",
                    "forth",
                    "fortran",
                    "ftl",
                    "gcode",
                    "gherkin",
                    "gitignore",
                    "glsl",
                    "gobstones",
                    "golang",
                    "groovy",
                    "haml",
                    "handlebars",
                    "haskell",
                    "haxe",
                    "html",
                    "html_completions",
                    "html_elixir",
                    "html_ruby",
                    "html_worker",
                    "ini",
                    "io",
                    "jack",
                    "jade",
                    "java",
                    "javascript",
                    "javascript_worker",
                    "json",
                    "jsoniq",
                    "json_worker",
                    "jsp",
                    "jsx",
                    "julia",
                    "latex",
                    "lean",
                    "less",
                    "liquid",
                    "lisp",
                    "livescript",
                    "logiql",
                    "lsl",
                    "lua",
                    "luapage",
                    "lua_worker",
                    "lucene",
                    "makefile",
                    "markdown",
                    "mask",
                    "matching_brace_outdent",
                    "matching_parens_outdent",
                    "matlab",
                    "maze",
                    "mel",
                    "mushcode",
                    "mysql",
                    "nix",
                    "nsis",
                    "objectivec",
                    "ocaml",
                    "pascal",
                    "perl",
                    "pgsql",
                    "php",
                    "php_completions",
                    "php_worker",
                    "plain_text",
                    "powershell",
                    "praat",
                    "prolog",
                    "properties",
                    "protobuf",
                    "python",
                    "r",
                    "razor",
                    "rdoc",
                    "rhtml",
                    "rst",
                    "ruby",
                    "rust",
                    "sass",
                    "scad",
                    "scala",
                    "scheme",
                    "scss",
                    "sh",
                    "sjs",
                    "smarty",
                    "snippets",
                    "space",
                    "sql",
                    "sqlserver",
                    "stylus",
                    "svg",
                    "swift",
                    "tcl",
                    "tex",
                    "text",
                    "textile",
                    "toml",
                    "tsx",
                    "twig",
                    "typescript",
                    "vala",
                    "vbscript",
                    "velocity",
                    "verilog"
                };
            }
        }
    }
    public class SyntaxHighLightThemesProvider : SyntaxHighLightLanguagesProviderBase {
        public override string[] Values
        {
            get
            {
                return new[]{
                    "ambiance",
                    "chaos",
                    "chrome",
                    "clouds",
                    "clouds_midnight",
                    "cobalt",
                    "crimson_editor",
                    "dawn",
                    "dreamweaver",
                    "eclipse",
                    "github",
                    "gruvbox",
                    "idle_fingers",
                    "iplastic",
                    "katzenmilch",
                    "kr_theme",
                    "kuroir",
                    "merbivore",
                    "merbivore_soft",
                    "monokai",
                    "mono_industrial",
                    "pastel_on_dark",
                    "solarized_dark",
                    "solarized_light",
                    "sqlserver",
                    "terminal",
                    "textmate",
                    "tomorrow",
                    "tomorrow_night",
                    "tomorrow_night_blue",
                    "tomorrow_night_bright",
                    "tomorrow_night_eighties",
                    "twilight",
                    "vibrant_ink",
                    "xcode"
                };
            }
        }
    }


}
