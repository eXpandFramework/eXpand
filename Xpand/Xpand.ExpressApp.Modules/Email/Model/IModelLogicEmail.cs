using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.Email.BusinessObjects;
using Xpand.ExpressApp.Email.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Email.Model {
    public interface IModelLogicEmail : IModelLogicContexts{
        IModelEmailReceipients EmailReceipients { get; }
        IModelEmailTemplateContexts EmailTemplateContexts { get; }
        IModelEmailLogicRules Rules { get; }
        IModelEmailSmtpClientContexts SmtpClientContexts { get; }
    }

    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelEmailLogicRules : IModelNode, IModelList<IModelEmailRule> {
    }

    [ModelInterfaceImplementor(typeof(IContextEmailRule), "Attribute")]
    public interface IModelEmailRule : IContextEmailRule, IModelConditionalLogicRule<IEmailRule>{
        [Browsable(false)]
        IEnumerable<string> SmtpClientContexts { get; }

        [Browsable(false)]
        IEnumerable<string> TemplateContexts { get; }
        
        [Browsable(false)]
        IEnumerable<string> EmailReceipientsContexts { get; }
    }

    public class ModelEmailRuleValidator : IModelNodeValidator<IModelEmailRule> {
        public bool IsValid(IModelEmailRule node, IModelApplication application) {
            return !string.IsNullOrEmpty(node.EmailReceipientsContext)||node.CurrentObjectEmailMember!=null;
        }
    }

    [DomainLogic(typeof(IModelEmailRule))]
    public class ModelEmailRuleDomainLogic {
        public static IEnumerable<string> Get_EmailReceipientsContexts(IModelEmailRule emailRule) {
            return (((IModelApplicationEmail) emailRule.Application).Email.EmailReceipients.Select(
                template => template.GetValue<string>("Id")));
        }
        public static IEnumerable<string> Get_SmtpClientContexts(IModelEmailRule emailRule) {
            return (((IModelApplicationEmail) emailRule.Application).Email.SmtpClientContexts.Select(
                template => template.GetValue<string>("Id")));
        }

        public static IEnumerable<string> Get_TemplateContexts(IModelEmailRule emailRule) {
            return (((IModelApplicationEmail) emailRule.Application).Email.EmailTemplateContexts.Select(
                template => template.GetValue<string>("Id")));
        }
    }

    public interface IModelEmailSmtpClientContexts : IModelList<IModelSmtpClientContext>,  IModelNode {
    }

    public interface IModelSmtpClientContext : IModelNode {
        [Category("SmtpClient")]
        bool EnableSsl { get; set; }
        [Required, Category("SmtpClient")]
        string Host { get; set; }
        [Required, Category("Credentials"), ModelBrowsable(typeof(ModelSmtpClientContextVisibilityCalculator))]
        string Password { get; set; }
        [DefaultValue(0x19), Category("SmtpClient")]
        int Port { get; set; }
        [RuleRegularExpression(null, DefaultContexts.Save, @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$"), Required]
        string SenderEmail { get; set; }
        [RuleRegularExpression(null, DefaultContexts.Save, @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$"), Required]
        [ModelValueCalculator("SenderEmail")]
        string ReplyToEmails { get; set; }
        [Category("Credentials")]
        bool UseDefaultCredentials { get; set; }
        [Category("Credentials"), ModelBrowsable(typeof(ModelSmtpClientContextVisibilityCalculator)), Required]
        string UserName { get; set; }
    }

    public class ModelSmtpClientContextVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !((IModelSmtpClientContext)node).UseDefaultCredentials;
        }
    }

    public interface IModelEmailReceipients : IModelList<IModelEmailReceipientGroup>, IModelNode {
    }

    public interface IModelEmailReceipientGroup : IModelNode, IModelList<IModelEmailReceipient>{
    }

    public interface IModelEmailReceipient : IModelNode {
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win"+AssemblyInfo.VSuffix
            , typeof(UITypeEditor)), CriteriaOptions("EmailReceipient.TypeInfo")]
        string Criteria { get; set; }
        [Required, DataSourceProperty("EmailMembers")]
        IModelMember EmailMember { get; set; }
        [Browsable(false)]
        IModelList<IModelMember> EmailMembers { get; }
        [DataSourceProperty("Application.BOModel"), Required]
        IModelClass EmailReceipient { get; set; }
        EmailType EmailType { get; set; }
    }

    public enum EmailType {
        Normal,
        CC,
        BCC
    }
    [DomainLogic(typeof(IModelEmailReceipient))]
    public class ModelEmailReceipientDomainLogic {
        public static IModelList<IModelMember> Get_EmailMembers(IModelEmailReceipient emailReceipient) {
            return ((emailReceipient.EmailReceipient != null) ? new CalculatedModelNodeList<IModelMember>(emailReceipient.EmailReceipient.AllMembers) : new CalculatedModelNodeList<IModelMember>());
        }
    }

    public interface IModelEmailTemplateContexts : IModelList<IModelEmailTemplate>, IModelNode {
    }

    public interface IModelEmailTemplate : IModelNode {
        [CriteriaOptions("EmailTemplate.TypeInfo"),
         Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win"+AssemblyInfo.VSuffix
             , typeof (UITypeEditor))]
        string Criteria { get; set; }
        [DataSourceProperty("EmailTemplates"), Required]
        IModelClass EmailTemplate { get; set; }
        [Browsable(false)]
        IModelList<IModelClass> EmailTemplates { get; }
    }

    [DomainLogic(typeof(IModelEmailTemplate))]
    public class ModelEmailTemplateDomainLogic {
        public static IModelClass Get_EmailTemplate(IModelEmailTemplate modelEmailTemplate) {
            return modelEmailTemplate.Application.BOModel.First(@class => (@class.TypeInfo.Type == typeof(EmailTemplate)));
        }

        public static IModelList<IModelClass> Get_EmailTemplates(IModelEmailTemplate modelEmailTemplate) {
            return new CalculatedModelNodeList<IModelClass>(modelEmailTemplate.Application.BOModel.Where(
                    @class => typeof (IEmailTemplate).IsAssignableFrom(@class.TypeInfo.Type)));
        }
    }

}