using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.Validation;
using Xpand.Utils.Helpers;
using System;

namespace Xpand.ExpressApp.Validation.Controllers {
    [ModelAbstractClass]
    public interface IModelMemberPasswordScore:IModelMember {    
        [Category("eXpand")]
        [Description("If bigger is considered valid")]
        PasswordScore? PasswordScore { get; set; }
    }

    public class PasswordScoreController:ObjectViewController,IModelExtender, IPasswordScoreController {
        IEnumerable<IModelMemberPasswordScore> _modelMemberPasswordScores;
        PersistenceValidationController _persistenceValidationController;

        protected override void OnActivated() {
            base.OnActivated();
            _modelMemberPasswordScores = View.Model.ModelClass.AllMembers.Cast<IModelMemberPasswordScore>().Where(member => member.PasswordScore != null);
            if (_modelMemberPasswordScores.Any()) {
                _persistenceValidationController = Frame.GetController<PersistenceValidationController>();
                _persistenceValidationController.ContextValidating+=OnContextValidating;
            }
        }

        void OnContextValidating(object sender, ContextValidatingEventArgs contextValidatingEventArgs) {
            if (contextValidatingEventArgs.Context==ContextIdentifier.Save.ToString()) {
                Validator.RuleSet.ValidationCompleted += RuleSetOnValidationCompleted;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_persistenceValidationController != null) {
                _persistenceValidationController.ContextValidating-=OnContextValidating;
            }
        }

        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs args) {
            Validator.RuleSet.ValidationCompleted -= RuleSetOnValidationCompleted;
            var ruleSetValidationResult = new RuleSetValidationResult();
            var validationException = args.Exception;
            if (validationException != null)
                ruleSetValidationResult = validationException.Result;
            foreach (var modelMemberPasswordScore in _modelMemberPasswordScores) {
                var password = View.ObjectTypeInfo.FindMember(modelMemberPasswordScore.Name).GetValue(View.CurrentObject);
                var passwordScore = PasswordAdvisor.CheckStrength(password +"");
                if (passwordScore<modelMemberPasswordScore.PasswordScore) {
                    var messageTemplate = String.Format(CaptionHelper.GetLocalizedText(XpandValidationModule.XpandValidation, "PasswordScoreFailed"), modelMemberPasswordScore.Name, passwordScore, modelMemberPasswordScore.PasswordScore);
                    var ruleValidationResult = new RuleValidationResult(null, View.CurrentObject, ValidationState.Invalid, messageTemplate);
                    ruleSetValidationResult.AddResult(new RuleSetValidationResultItem(View.CurrentObject, ContextIdentifier.Save, null,ruleValidationResult));
                    args.Handled = true;
                }
            }
            if (args.Handled)
                throw validationException??new ValidationException(ruleSetValidationResult);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember,IModelMemberPasswordScore>();
        }
    }
}
