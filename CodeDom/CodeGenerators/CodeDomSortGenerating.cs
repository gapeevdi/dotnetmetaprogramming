using System.CodeDom;
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
            AddCodeDomPivotStrategyType(sortNamespace);

            return sortNamespace;
        }

        private void AddCodeDomPivotStrategyType(CodeNamespace codeNamespace)
        {
            var pivotTypeBuilder = new CodeDomPivotElementStrategyBuilder();
            codeNamespace.Types.Add(pivotTypeBuilder.BuildFirstElementAsPivotStrategyType());
            codeNamespace.Types.Add(pivotTypeBuilder.BuildLastElementAsPivotStrategyType());
            codeNamespace.Types.Add(pivotTypeBuilder.BuildPivotElementStrategyInterface());
        }

        private CodeNamespace CreateNamespace()
        {
            var sortNamespace = new CodeNamespace {Name = Constants.NamespaceName};
            sortNamespace.Imports.Add(new CodeNamespaceImport("System")); //using System;

            return sortNamespace;
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