using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using EnvDTE80;
using eXpandAddIns;
using eXpandAddIns.Enums;
using eXpandAddIns.Extensioons;
using CodeElement=EnvDTE.CodeElement;
using Process=System.Diagnostics.Process;
using Project=EnvDTE.Project;
using Property=EnvDTE.Property;
using Attribute=DevExpress.CodeRush.StructuralParser.Attribute;
using Debugger=System.Diagnostics.Debugger;
using Expression=DevExpress.CodeRush.StructuralParser.Expression;

namespace eXpandAddIns
{
    public partial class PlugIn1 : StandardPlugIn
    {
        private bool syncing;
        #region InitializePlugIn
        #endregion
        #region FinalizePlugIn
        #endregion
//        private Class GenerateEmptyFieldsClass(ClassInfo classInfo)
//        {
//            Class class2 = new Class("FieldsClass");
//            class2.PrimaryAncestorType = classInfo.GetAncestorTypeForFieldsClass();
//            class2.Visibility = MemberVisibility.Public;
//            class2.IsNew = classInfo.CurrentClassHasAncestorWithFieldsClass;
//            ElementBuilder activeElementBuilder = CodeRush.Language.GetActiveElementBuilder();
//            Method parent = activeElementBuilder.BuildConstructor("FieldsClass");
//            parent.Visibility = MemberVisibility.Public;
//            activeElementBuilder.AddBaseConstructorInitializer(parent, new ExpressionCollection());
//            class2.AddNode(parent);
//            Method method2 = activeElementBuilder.BuildConstructor("FieldsClass");
//            method2.Visibility = MemberVisibility.Public;
//            method2.Parameters.Add(new Param(CodeRush.Language.GetSimpleTypeName("System.String"), "propertyName"));
//            ExpressionCollection arguments = new ExpressionCollection();
//            arguments.Add(new ElementReferenceExpression("propertyName"));
//            activeElementBuilder.AddBaseConstructorInitializer(method2, arguments);
//            class2.AddNode(method2);
//            return class2;
//        }


//        private Variable GenerateFieldsField()
//        {
//            Variable variable = CodeRush.Language.GetActiveElementBuilder().BuildVariable("FieldsClass", "_fields");
//            variable.Visibility = MemberVisibility.Private;
//            variable.IsStatic = true;
//            return variable;
//        }


//        private IEnumerable<LanguageElement> GenerateHelperElements(ClassInfo classInfo)
//        {
//            List<LanguageElement> list = new List<LanguageElement>();
//            Class fieldsClass = this.GenerateEmptyFieldsClass(classInfo);
//            this.FillFieldsClass(fieldsClass, classInfo);
//            list.Add(fieldsClass);
//            list.Add(this.GenerateFieldsField());
//            list.Add(this.GenerateFieldsProperty(classInfo));
//            return list;
//        }
//        private DevExpress.CodeRush.StructuralParser.Property GenerateFieldsProperty(ClassInfo classInfo)
//        {
//            ElementBuilder activeElementBuilder = CodeRush.Language.GetActiveElementBuilder();
//            DevExpress.CodeRush.StructuralParser.Property parent = activeElementBuilder.BuildProperty("FieldsClass", "Fields");
//            parent.IsStatic = true;
//            parent.Visibility = MemberVisibility.Public;
//            parent.IsNew = classInfo.CurrentClassHasAncestorWithFieldsClass;
//            Get get = activeElementBuilder.AddGetter(parent);
//            var source = new MethodReferenceExpression("ReferenceEquals", SourceRange.Empty);
//            MethodCallExpression expression = activeElementBuilder.BuildMethodCallExpression(source);
//            expression.Arguments.Add(new ElementReferenceExpression("_fields"));
//            expression.Arguments.Add(PrimitiveExpression.FromObject(null));
//            If @if = activeElementBuilder.AddIf(get, expression);
//            activeElementBuilder.AddAssignment(@if, "_fields", new ObjectCreationExpression(new TypeReferenceExpression("FieldsClass"), new ExpressionCollection()));
//            activeElementBuilder.AddReturn(get, "_fields");
//            return parent;
//        }

//        private DevExpress.CodeRush.StructuralParser.Property GenerateFieldsClassProperty(ClassInfo classInfo, Member persistentProperty)
//        {
//            return this.GenerateFieldsClassProperty(classInfo, persistentProperty, persistentProperty.Name, persistentProperty.Name);
//        }


//        private void FillFieldsClass(Class fieldsClass, ClassInfo classInfo)
//        {
//            foreach (Member member in classInfo.CurrentClassQueryablePropertyNodes)
//            {
//                DevExpress.CodeRush.StructuralParser.Property element = this.GenerateFieldsClassProperty(classInfo, member);
//                if (element != null)
//                {
//                    fieldsClass.AddNode(element);
//                }
//            }
//            foreach (Member member in classInfo.CurrentClassQueryableStructNodes)
//            {
//                IEnumerable<DevExpress.CodeRush.StructuralParser.Property> enumerable = this.GenerateFieldsClassStructProperties(classInfo, member);
//                foreach (DevExpress.CodeRush.StructuralParser.Property property in enumerable)
//                {
//                    fieldsClass.AddNode(property);
//                }
//            }
//        }

//        private void AppendCode(StringBuilder target, IEnumerable<LanguageElement> elements)
//        {
//            foreach (LanguageElement element in elements)
//            {
//                target.Append(CodeRush.Language.GenerateElement(element));
//            }
//        }
//        private static bool ChildTypesDontContain(TypeDeclaration @class, SourceRange range, bool logging)
//        {
//            if (logging)
//            {
////                LogSendMsg(ImageType.Info, "Checking child types for " + @class.Name);
//            }
//            foreach (TypeDeclaration declaration in @class.AllTypes)
//            {
//                if (logging)
//                {
////                    LogEnter(ImageType.Info, string.Concat(new object[] { "Checking type ", declaration.Name, " with range ", declaration.Range }));
//                }
//                if (declaration.Range.Contains(range))
//                {
//                    if (logging)
//                    {
////                        LogSendMsg(ImageType.Info, "type contains range, returning");
////                        LogExit();
//                    }
//                    return false;
//                }
//                if (logging)
//                {
////                    LogExit();
//                }
//            }
//            return true;
//        }

 

 

//        private static RegionDirective FindOurRegionInClassAndList(Class @class, string regionName, TextDocument doc, NodeList list, bool logging)
//        {
//            foreach (RegionDirective directive in list)
//            {
//                if (directive.Name == regionName)
//                {
//                    if (logging)
//                    {
////                        LogEnter(ImageType.Info, "Found region with right name: " + directive.Range);
//                    }
//                    if (@class.Range.Contains(directive.Range))
//                    {
//                        if (logging)
//                        {
////                            LogEnter(ImageType.Info, "Class " + @class.Name + " contains region");
//                        }
//                        if (ChildTypesDontContain(@class, directive.Range, logging))
//                        {
//                            if (logging)
//                            {
////                                LogSendMsg(ImageType.Info, "Child types of " + @class.Name + " don't contain region, returning");
////                                LogExit();
////                                LogExit();
//                            }
//                            return directive;
//                        }
//                        if (logging)
//                        {
////                            LogExit();
//                        }
//                    }
//                    if (logging)
//                    {
////                        LogExit();
//                    }
//                }
//                if (logging)
//                {
////                    LogEnter(ImageType.Info, "Looking at sub-regions");
//                }
//                RegionDirective directive2 = FindOurRegionInClassAndList(@class, regionName, doc, directive.Nodes, logging);
//                if (logging)
//                {
////                    LogExit();
//                }
//                if (directive2 != null)
//                {
//                    if (logging)
//                    {
////                        LogSendMsg(ImageType.Info, "Found sub-region, returning");
//                    }
//                    return directive2;
//                }
//            }
//            if (logging)
//            {
////                LogSendMsg(ImageType.Info, "Region not found, returning null");
//            }
//            return null;
//        }

 

 


//        private static RegionDirective FindOurRegionInClass(Class @class, string regionName, TextDocument doc, bool logging)
//        {
//            if (logging)
//            {
////                LogEnter(ImageType.Info, "Finding region in class " + @class.Name);
//            }
//            RegionDirective directive = FindOurRegionInClassAndList(@class, regionName, doc, doc.RegionRootNode.Nodes, logging);
//            if (logging)
//            {
////                LogExit();
//            }
//            return directive;
//        }

 

//        private bool SyncCurrentClass(bool calledExplicitly)
//        {
//            if (syncing)
//            {
//                return false;
//            }
//            syncing = true;
//            try
//            {
//                try
//                {
////                    if (this.options.Logging)
////                    {
////                        LogEnter(ImageType.Info, "XPO Field Sync");
////                    }
//                    Class activeClass = CodeRush.Source.ActiveClass;
//                    TextDocument activeTextDocument = CodeRush.Documents.ActiveTextDocument;
//                    var classInfo = new ClassInfo(this, activeClass);
//                    if (true)
//                    {
//                        var target = new StringBuilder();
//                        if (classInfo.IsCurrentClassQueryable)
//                        {
//                            target.AppendLine(CodeRush.Language.GetRegionHeader("XPO nested fields class - don't edit manually"));
//                            AppendCode(target, GenerateHelperElements(classInfo));
//                            target.Append(CodeRush.Language.GetRegionFooter());
//                        }
//                        RegionDirective directive = FindOurRegionInClass(activeClass, "XPO nested fields class - don't edit manually", activeTextDocument, this.options.Logging);
//                        if ((directive != null) || (target.Length > 0))
//                        {
//                            ParentUndoUnit parentUnit = this.StartUndoUnit();
//                            try
//                            {
//                                if (directive == null)
//                                {
//                                    target.AppendLine();
//                                    SourcePoint insertionPoint = new SourcePoint(activeClass.BlockEnd.Start.Line, 1);
//                                    activeTextDocument.QueueInsert(insertionPoint, target.ToString());
//                                }
//                                else
//                                {
//                                    activeTextDocument.QueueReplace(directive.Range, target.ToString());
//                                }
//                                activeTextDocument.ApplyQueuedEdits();
//                                this.EnsureNamespaces();
//                                activeTextDocument.ParseIfNeeded();
//                                directive = FindOurRegionInClass(CodeRush.Source.ActiveClass, "XPO nested fields class - don't edit manually", activeTextDocument, this.options.Logging);
//                                if (directive != null)
//                                {
//                                    activeTextDocument.Format(directive.Range, true);
//                                    activeTextDocument.ParseIfNeeded();
//                                    new RegionCollapseHandler(CodeRush.Source.ActiveClass.FullName, this.options.Logging).Collapse();
//                                }
//                            }
//                            finally
//                            {
//                                if (calledExplicitly)
//                                {
//                                    CodeRush.UndoStack.CommitParentUnit(parentUnit);
//                                }
//                                else
//                                {
//                                    CodeRush.UndoStack.DiscardParentUnit(parentUnit);
//                                }
//                            }
//                        }
//                    }
//                }
//                catch (Exception exception)
//                {
//                    LogSendException("XPO Field Sync exception", exception);
//                }
//            }
//            finally
//            {
//                this.syncing = false;
//                if (this.options.Logging)
//                {
//                    LogExit();
//                }
//            }
//            return true;
//        }

 

 



        private void convertProject_Execute(ExecuteEventArgs ea)
        {
            
            using (var storage = new DecoupledStorage(typeof (Options)))
            {
                string path = storage.ReadString(Options.GetPageName(), "projectConverterPath");
                string token = storage.ReadString(Options.GetPageName(), "token");
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token))
                {
                    var directoryName = Path.GetDirectoryName(CodeRush.Solution.Active.FileName);
                    
                    var userName = string.Format("/s /k:{0} \"{1}\"", token, directoryName);
                    Process.Start(path, userName);
                }
            }
            
            
            
        }

        private void collapseAllItemsInSolutionExplorer_Execute(ExecuteEventArgs ea)
        {
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        private void exploreXafErrors_Execute(ExecuteEventArgs ea)
        {
            DTE dte = CodeRush.ApplicationObject;
            Solution solution = dte.Solution;
            Property startUpProperty = solution.GetProperty(SolutionProperty.StartupProject);

            
            
            Project startUpProject = solution.FindProject((startUpProperty.Value + ""));
            Property outPut = startUpProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath);
            Property fullPath = startUpProject.FindProperty(ProjectProperty.FullPath);
            string path = Path.Combine(fullPath.Value.ToString(),outPut.Value.ToString());
            var reader = new InverseReader(Path.Combine(path,"expressAppFrameWork.log"));
            var stackTrace = new List<string>();
            while (!reader.SOF)
            {
                
                string readline = reader.Readline();
                stackTrace.Add(readline);
                if (readline.Trim().StartsWith("The error occured:"))
                {
                    stackTrace.Reverse();
                    string errorMessage = "";
                    foreach (var trace in stackTrace)
                    {
                        errorMessage += trace+Environment.NewLine;
                        if (trace.Trim().StartsWith("----------------------------------------------------"))
                            break;
                    }
                    Clipboard.SetText(errorMessage);
                    break;
                }
                    
                
            }
            reader.Close();

        }
        
        public static string Inject(string injectToString, int positionToInject, string stringToInject)
        {
            var builder = new StringBuilder();
            builder.Append(injectToString.Substring(0, positionToInject));
            builder.Append(stringToInject);
            builder.Append(injectToString.Substring(positionToInject));
            return builder.ToString();
        }

        private void PlugIn1_BuildBegin(vsBuildScope scope, vsBuildAction action)
        {

        }

        private void action1_Execute(ExecuteEventArgs ea)
        {
            
            var dte2 = (DTE)Marshal.GetActiveObject("VisualStudio.DTE.9.0");

            Class activeClass = CodeRush.Source.ActiveClass;
            NodeList attributes = activeClass.Attributes;
            var attribute = (Attribute) attributes[1];
            Expression expression = attribute.Arguments[0];
            object o = expression.Evaluate();
            ProjectItem projectItem = CodeRush.ProjectItems.Active;
            CodeClass codeClass = ExamineItem(projectItem);
            string kind = projectItem.Kind;
        }


        private CodeClass ExamineItem(ProjectItem item)
        {
            var model = (FileCodeModel2)item.FileCodeModel;
            foreach (CodeElement codeElement in model.CodeElements)
            {
                CodeClass element = ExamineCodeElement(codeElement);
                if (element!= null)
                    return element;
            }
            return null;
        }

        // recursively examine code elements
        private CodeClass ExamineCodeElement(CodeElement codeElement)
        {
            try
            {
                vsCMElement vsCmElement = codeElement.Kind;
                if (vsCmElement == vsCMElement.vsCMElementClass)
                {
                    var codeClass = ((CodeClass)codeElement);
                    return codeClass;
                }

                foreach (CodeElement childElement in codeElement.Children)
                    ExamineCodeElement(childElement);
            }
            catch
            {
            }
            return null;
        }

//        private void ImplementXpoManyPart_CheckAvailability(object sender, CheckContentAvailabilityEventArgs ea)
//        {
//            var calulator = new AvaliabiltityCalulator(ea.Element);
//            ea.Available = ((calulator.Avaliability & Avaliabiltity.XpoManyPart) == Avaliabiltity.XpoManyPart);
//        }

//        private void ImplementXpoOnePart_CheckAvailability(object sender, CheckContentAvailabilityEventArgs ea)
//        {
//            var calulator = new AvaliabiltityCalulator(ea.Element);
//            ea.Available = ((calulator.Avaliability & Avaliabiltity.XpoOnePart) == Avaliabiltity.XpoOnePart);
//        }

//        private void ImplementXpoOnePart_Apply(object sender, ApplyContentEventArgs ea)
//        {
//            var builder = new AssociationBuilder((DevExpress.CodeRush.StructuralParser.Property)ea.Element);
//            builder.BuildAssociatedProperty(Avaliabiltity.XpoOnePart);
//        }

//        private void ImplementXpoManyPart_Apply(object sender, ApplyContentEventArgs ea)
//        {
//            var builder = new AssociationBuilder((DevExpress.CodeRush.StructuralParser.Property) ea.Element);
//            builder.BuildAssociatedProperty(Avaliabiltity.XpoOnePart);
//        }


        private void events_DebuggerEnterRunMode(DebuggerEnterModeEventArgs ea)
        {
//            CodeRush.ApplicationObject.ExecuteCommand("View.FullScreen","");
        }

        private void events_DebuggerContextChanged(DebuggerContextChangedEventArgs ea)
        {
            
        }

        private void events_DebuggerEnterDesignMode(DebuggerEnterModeEventArgs ea)
        {
//            CodeRush.ApplicationObject.ExecuteCommand("View.FullScreen", "");
        }
    }
}