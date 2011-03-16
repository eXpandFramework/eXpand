using System;
using Machine.Specifications;
using Moq;
using Xpand.EmailTemplateEngine;
using Xpand.Tests.Xpand.EmailTemplateEngine;
using arg = Machine.Specifications.It;

namespace Xpand.Tests.Xpand.EmailTemplateEngines {
    [Subject(typeof(global::Xpand.EmailTemplateEngine.EmailTemplateEngine))]
    public class When_trying_to_execute_with_blank_template_name {
        static Exception exception;

        Because of = () => {
            var emailTemplateContentReader = new Mock<IEmailTemplateContentReader>().Object;
            var emailTemplateEngine = new global::Xpand.EmailTemplateEngine.EmailTemplateEngine(emailTemplateContentReader);
            exception = Catch.Exception(() => emailTemplateEngine.Execute(string.Empty));
        };

        arg should_throw_exception = () => exception.ShouldBeOfType<ArgumentException>();
    }

    public abstract class EmailTemplateEngineWhenExecutingSpec {
        protected static global::Xpand.EmailTemplateEngine.EmailTemplateEngine templateEngine;
        protected static Email email;

        Because of = () => {
            var model = new {
                EmailTemplateExecutingBehavior.From,
                EmailTemplateExecutingBehavior.To,
                EmailTemplateExecutingBehavior.CC,
                EmailTemplateExecutingBehavior.Bcc,
                EmailTemplateExecutingBehavior.Subject,
                EmailTemplateExecutingBehavior.Name,
                EmailTemplateExecutingBehavior.Password,
                EmailTemplateExecutingBehavior.LogOnUrl
            };

            email = templateEngine.Execute("Test" + Guid.NewGuid().ToString("N"), model);
        };
    }

    [Behaviors]
    public class EmailTemplateExecutingBehavior {
        public const string From = "me@myself.com";
        public const string To = "you@yourself.com";
        public const string CC = "he@himself.com";
        public const string Bcc = "she@herself.com";
        public const string Subject = "Welcome to myself.com";

        public const string Name = "Jon Smith";
        public const string Password = "ABcd12#$";
        public const string LogOnUrl = "http://myself.com/logon";

        protected static Email email;

        arg should_set_form_address = () => email.From.ShouldMatch(From);

        arg should_set_to_address = () => email.To.ShouldContain(To);

        arg should_set_cc_address = () => email.CC.ShouldContain(CC);

        arg should_set_bcc_address = () => email.Bcc.ShouldContain(Bcc);

        arg should_set_subject = () => email.Subject.ShouldMatch(Subject);
    }

    [Subject(typeof(global::Xpand.EmailTemplateEngine.EmailTemplateEngine))]
    public class When_executing_multi_view_template : EmailTemplateEngineWhenExecutingSpec {
        Establish context = () => {
            var contentReader = new Mock<IEmailTemplateContentReader>();

            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultSharedTemplateSuffix)).Returns(MailTemplates.SharedTemplate);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultHtmlTemplateSuffix)).Returns(MailTemplates.HtmlTemplate);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultTextTemplateSuffix)).Returns(MailTemplates.TextTemplate);

            templateEngine = new global::Xpand.EmailTemplateEngine.EmailTemplateEngine(contentReader.Object);
        };

        Behaves_like<EmailTemplateExecutingBehavior> template_execution;

        arg should_set_html_body_variables = () => {
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.Name);
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.Password);
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.LogOnUrl);
        };

        arg should_set_text_body_variables = () => {
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.Name);
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.Password);
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.LogOnUrl);
        };
    }

    [Subject(typeof(global::Xpand.EmailTemplateEngine.EmailTemplateEngine))]
    public class When_executing_single_html_view_template : EmailTemplateEngineWhenExecutingSpec {
        Establish context = () => {
            var contentReader = new Mock<IEmailTemplateContentReader>();

            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultSharedTemplateSuffix)).Returns((string)null);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultHtmlTemplateSuffix)).Returns(MailTemplates.SharedTemplate + MailTemplates.HtmlTemplate);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultTextTemplateSuffix)).Returns((string)null);

            templateEngine = new global::Xpand.EmailTemplateEngine.EmailTemplateEngine(contentReader.Object);
        };

        Behaves_like<EmailTemplateExecutingBehavior> template_execution;

        arg should_set_html_body_variables = () => {
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.Name);
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.Password);
            email.HtmlBody.ShouldContain(EmailTemplateExecutingBehavior.LogOnUrl);
        };

        arg should_not_set_text_body = () => email.TextBody.ShouldBeNull();
    }

    [Subject(typeof(global::Xpand.EmailTemplateEngine.EmailTemplateEngine))]
    public class When_executing_single_text_view_template : EmailTemplateEngineWhenExecutingSpec {
        Establish context = () => {
            var contentReader = new Mock<IEmailTemplateContentReader>();

            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultSharedTemplateSuffix)).Returns((string)null);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultHtmlTemplateSuffix)).Returns((string)null);
            contentReader.Setup(r => r.Read(Moq.It.IsAny<string>(), global::Xpand.EmailTemplateEngine.EmailTemplateEngine.DefaultTextTemplateSuffix)).Returns(MailTemplates.SharedTemplate + MailTemplates.TextTemplate);

            templateEngine = new global::Xpand.EmailTemplateEngine.EmailTemplateEngine(contentReader.Object);
        };

        Behaves_like<EmailTemplateExecutingBehavior> template_execution;

        arg should_set_text_body_variables = () => {
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.Name);
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.Password);
            email.TextBody.ShouldContain(EmailTemplateExecutingBehavior.LogOnUrl);
        };

        arg should_not_set_html_body = () => email.HtmlBody.ShouldBeNull();
    }
}