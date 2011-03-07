using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Razor;
using Microsoft.CSharp;

namespace Xpand.EmailTemplateEngine {
    public class EmailTemplateEngine : IEmailTemplateEngine {
        public const string DefaultSharedTemplateSuffix = "";
        public const string DefaultHtmlTemplateSuffix = "html";
        public const string DefaultTextTemplateSuffix = "text";

        private const string NamespaceName = "EmailModule";

        private static readonly Dictionary<string, IEnumerable<KeyValuePair<string, Type>>> typeMapping = new Dictionary<string, IEnumerable<KeyValuePair<string, Type>>>(StringComparer.OrdinalIgnoreCase);
        private static readonly ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

        private static readonly string[] referencedAssemblies = BuildReferenceList().ToArray();
        private static readonly RazorTemplateEngine razorEngine = CreateRazorEngine();

        public EmailTemplateEngine(IEmailTemplateContentReader contentReader)
            : this(contentReader, DefaultHtmlTemplateSuffix, DefaultTextTemplateSuffix, DefaultSharedTemplateSuffix) {
            ContentReader = contentReader;
        }

        public EmailTemplateEngine(IEmailTemplateContentReader contentReader, string htmlTemplateSuffix, string textTemplateSuffix, string sharedTemplateSuffix) {
            Invariant.IsNotNull(contentReader, "contentReader");

            ContentReader = contentReader;
            SharedTemplateSuffix = sharedTemplateSuffix;
            HtmlTemplateSuffix = htmlTemplateSuffix;
            TextTemplateSuffix = textTemplateSuffix;
        }

        protected IEmailTemplateContentReader ContentReader { get; private set; }

        protected string SharedTemplateSuffix { get; private set; }

        protected string HtmlTemplateSuffix { get; private set; }

        protected string TextTemplateSuffix { get; private set; }

        public virtual Email Execute(string templateName, object model = null) {
            Invariant.IsNotBlank(templateName, "templateName");

            var templates = CreateTemplateInstances(templateName);

            foreach (var pair in templates) {
                pair.Value.SetModel(WrapModel(model));
                pair.Value.Execute();
            }

            var mail = new Email();

            templates.SelectMany(x => x.Value.To)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Each(email => mail.To.Add(email));

            templates.SelectMany(x => x.Value.ReplyTo)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Each(email => mail.ReplyTo.Add(email));

            templates.SelectMany(x => x.Value.Bcc)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Each(email => mail.Bcc.Add(email));

            templates.SelectMany(x => x.Value.CC)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Each(email => mail.CC.Add(email));

            Action<string, Action<string>> set = (contentType, action) => {
                var item = templates.SingleOrDefault(x => x.Key.Equals(contentType));

                IEmailTemplate template = item.Value;

                if (item.Value != null) {
                    if (!string.IsNullOrWhiteSpace(template.From)) {
                        mail.From = template.From;
                    }

                    if (!string.IsNullOrWhiteSpace(template.Sender)) {
                        mail.Sender = template.Sender;
                    }

                    if (!string.IsNullOrWhiteSpace(template.Subject)) {
                        mail.Subject = template.Subject;
                    }

                    template.Headers.Each(pair => mail.Headers[pair.Key] = pair.Value);

                    if (action != null) {
                        action(template.Body);
                    }
                }
            };

            set(ContentTypes.Text, body => { mail.TextBody = body; });
            set(ContentTypes.Html, body => { mail.HtmlBody = body; });
            set(string.Empty, null);

            return mail;
        }

        protected virtual Assembly GenerateAssembly(params KeyValuePair<string, string>[] templates) {
            var assemblyName = NamespaceName + "." + Guid.NewGuid().ToString("N") + ".dll";

            var templateResults = templates.Select(pair => razorEngine.GenerateCode(new StringReader(pair.Value), pair.Key, NamespaceName, pair.Key + ".cs")).ToList();

            if (templateResults.Any(result => result.ParserErrors.Any())) {
                var parseExceptionMessage = string.Join(Environment.NewLine + Environment.NewLine, templateResults.SelectMany(r => r.ParserErrors).Select(e => e.Location + ":" + Environment.NewLine + e.Message).ToArray());

                throw new InvalidOperationException(parseExceptionMessage);
            }

            using (var codeProvider = new CSharpCodeProvider()) {
                var compilerParameter = new CompilerParameters(referencedAssemblies, assemblyName, false) {
                    GenerateInMemory = true,
                    CompilerOptions = "/optimize"
                };

                var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameter, templateResults.Select(r => r.GeneratedCode).ToArray());

                if (compilerResults.Errors.HasErrors) {
                    var compileExceptionMessage = string.Join(Environment.NewLine + Environment.NewLine, compilerResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(e => e.FileName + ":" + Environment.NewLine + e.ErrorText).ToArray());

                    throw new InvalidOperationException(compileExceptionMessage);
                }

                return compilerResults.CompiledAssembly;
            }
        }

        protected virtual dynamic WrapModel(object model) {
            if (model == null) {
                return null;
            }

            if (model is IDynamicMetaObjectProvider) {
                return model;
            }

            var propertyMap = model.GetType()
                                    .GetProperties()
                                    .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
                                    .ToDictionary(property => property.Name, property => property.GetValue(model, null));

            return new EmailTemplateModelWrapper(propertyMap);
        }

        private static RazorTemplateEngine CreateRazorEngine() {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage()) {
                DefaultBaseClass = typeof(EmailTemplate).FullName,
                DefaultNamespace = NamespaceName
            };

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Collections");
            host.NamespaceImports.Add("System.Collections.Generic");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Linq");

            return new RazorTemplateEngine(host);
        }

        private static IEnumerable<string> BuildReferenceList() {
            string currentAssemblyLocation = typeof(EmailTemplateEngine).Assembly.CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            return new List<string>
                       {
                           "mscorlib.dll",
                           "system.dll",
                           "system.core.dll",
                           "microsoft.csharp.dll",
                           currentAssemblyLocation
                       };
        }

        private IEnumerable<KeyValuePair<string, IEmailTemplate>> CreateTemplateInstances(string templateName) {
            return GetTemplateTypes(templateName).Select(pair => new KeyValuePair<string, IEmailTemplate>(pair.Key, (IEmailTemplate)Activator.CreateInstance(pair.Value)))
                                                 .ToList();
        }

        private IEnumerable<KeyValuePair<string, Type>> GetTemplateTypes(string templateName) {
            IEnumerable<KeyValuePair<string, Type>> templateTypes;

            syncLock.EnterUpgradeableReadLock();

            try {
                if (!typeMapping.TryGetValue(templateName, out templateTypes)) {
                    syncLock.EnterWriteLock();

                    try {
                        templateTypes = GenerateTemplateTypes(templateName);
                        typeMapping.Add(templateName, templateTypes);
                    } finally {
                        syncLock.ExitWriteLock();
                    }
                }
            } finally {
                syncLock.ExitUpgradeableReadLock();
            }

            return templateTypes;
        }

        private IEnumerable<KeyValuePair<string, Type>> GenerateTemplateTypes(string templateName) {
            var suffixesWithContentTypes = new Dictionary<string, string>
                                               {
                                                   { SharedTemplateSuffix, string.Empty },
                                                   { HtmlTemplateSuffix, ContentTypes.Html },
                                                   { TextTemplateSuffix, ContentTypes.Text }
                                               };

            var templates = suffixesWithContentTypes.Select(pair => new {
                Suffix = pair.Key,
                TemplateName = templateName + pair.Key,
                Content = ContentReader.Read(templateName, pair.Key),
                ContentType = pair.Value
            })
                                                    .Where(x => !string.IsNullOrWhiteSpace(x.Content))
                                                    .ToList();

            var compliableTemplates = templates.Select(x => new KeyValuePair<string, string>(x.TemplateName, x.Content)).ToArray();

            var assembly = GenerateAssembly(compliableTemplates);

            return templates.Select(x => new KeyValuePair<string, Type>(x.ContentType, assembly.GetType(NamespaceName + "." + x.TemplateName, true, false))).ToList();
        }
    }
}