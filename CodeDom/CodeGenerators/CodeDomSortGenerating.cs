﻿using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        
        public string BuildSortAssembly() => GenerateAssemblyCode(CreateCodeNamespace());

        public Assembly FormAssembly() => FormAssembly(CreateCodeNamespace());

        private CodeNamespace CreateCodeNamespace()
        {
            var sortNamespace = CreateNamespace();
            sortNamespace.Types.Add(new CodeDomQuickSortTypeBuilder().BuildType());
            sortNamespace.Types.Add(CreatePivotElementStrategyInterface());
            
            
            sortNamespace.Types.Add(CreateFirstElementAsPivotStrategyType());
            sortNamespace.Types.Add(CreateLastElementAsPivotStrategyType());

            return sortNamespace;
        }
        

        private CodeNamespace CreateNamespace()
        {
            var sortNamespace = new CodeNamespace {Name = Constants.NamespaceName};
            sortNamespace.Imports.Add(new CodeNamespaceImport("System")); //using System;

            return sortNamespace;
        }

        /// <summary>
        /// Defining interface IPivotElementStrategy which implementation provides
        /// strategy to choose a pivot for the algorithm 
        /// </summary>
        /// <returns></returns>
        private CodeTypeDeclaration CreatePivotElementStrategyInterface()
        {
            var pivotElementInterface = new CodeTypeDeclaration
            {
                IsInterface = true,
                Name = Constants.PivotStrategy.PivotInterfaceName
            };

            pivotElementInterface.Members.Add(PivotMethodDeclaration());

            return pivotElementInterface;
        }

        private CodeMemberMethod PivotMethodDeclaration()
        {
            var pivotMethodDeclaration = new CodeMemberMethod
            {
                Name = Constants.PivotStrategy.PivotMethodName,
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference(Constants.GenericTypeName),
                TypeParameters = {CommonExpressions.GenericComparableType}
            };

            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(Constants.GenericArrayTypeName, Constants.ArrayParameterName));
            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(int), Constants.LowIndexParameterName));
            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(int), Constants.TopIndexParameterName));

            return pivotMethodDeclaration;

        }

        private CodeTypeDeclaration CreateFirstElementAsPivotStrategyType()
        {
            var firstElementPivotStrategyType = new CodeTypeDeclaration()
            {
                IsClass = true,
                Name = Constants.PivotStrategy.FirstElementPivotStrategy
            };

            firstElementPivotStrategyType.BaseTypes.Add(
                new CodeTypeReference(Constants.PivotStrategy.PivotInterfaceName));

            var pivotMethod = PivotMethodDeclaration();

            pivotMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArrayIndexerExpression(
                new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                new CodeArgumentReferenceExpression(Constants.LowIndexParameterName))));
            
            firstElementPivotStrategyType.Members.Add(pivotMethod);

            return firstElementPivotStrategyType;
        }

        private CodeTypeDeclaration CreateLastElementAsPivotStrategyType()
        {
            var lastElementPivotStrategyType = new CodeTypeDeclaration()
            {
                IsClass = true,
                Name = Constants.PivotStrategy.LastElementPivotStrategy
            };

            lastElementPivotStrategyType.BaseTypes.Add(new CodeTypeReference(Constants.PivotStrategy.PivotInterfaceName));

            var pivotMethod = PivotMethodDeclaration();
            
            pivotMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArrayIndexerExpression(
                new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                new CodeArgumentReferenceExpression(Constants.TopIndexParameterName))));
            
            lastElementPivotStrategyType.Members.Add(pivotMethod);

            return lastElementPivotStrategyType;
        }


        private string GenerateAssemblyCode(CodeNamespace codeNamespace)
        {
            var stringBuilder = new StringBuilder();
            using (var text = new StringWriter(stringBuilder))
            {
                CodeDomProvider.CreateProvider(Constants.ProgrammingLanguage).GenerateCodeFromNamespace(codeNamespace, text,
                    new CodeGeneratorOptions()
                    {
                        BracingStyle = "C",
                        IndentString = "    "
                    });
            }

            return stringBuilder.ToString();
        }

        private Assembly FormAssembly(CodeNamespace codeNamespace)
        {
            var compilationUnit = new CodeCompileUnit()
            {
                Namespaces = {codeNamespace},
                AssemblyCustomAttributes =
                {
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyVersionAttribute)),
                        new CodeAttributeArgument(new CodePrimitiveExpression("1.0.0.0")))
                },
                ReferencedAssemblies = {"System.dll"}
            };

            var compilerParameters = new CompilerParameters {
                OutputAssembly = Constants.OutputAssemblyName, 
                GenerateInMemory = true};

            var csharpProvider = CodeDomProvider.CreateProvider(Constants.ProgrammingLanguage);
            var compilationResult = csharpProvider.CompileAssemblyFromDom(compilerParameters, compilationUnit);

            return compilationResult.CompiledAssembly;
        }
    }
}