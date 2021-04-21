using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomQuickSortTypeBuilder
    {
        
        private CodeTypeDeclaration BuildType()
        {
            var quickSortType = new CodeTypeDeclaration
            {
                Name = CodeDomSortGenerating.Constants.QuickSorterTypeName,
                Attributes = MemberAttributes.Public,
                Comments = {new CodeCommentStatement("The type supplies the quick sort algorithm to sort a generic array")}
            };

            quickSortType.Members.Add(CreateSortMethod());
            quickSortType.Members.Add(CreateSortImplementation());
            quickSortType.Members.Add(CreatePartitionMethod());
            return quickSortType;
        }

        private CodeMemberMethod CreateSortMethod()
        {
            var sortMethod = new CodeMemberMethod
            {
                Name = CodeDomSortGenerating.Constants.QuickSorterTypeName, 
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };

            sortMethod.Parameters.AddRange(BuildSortMethodParameters());
            sortMethod.TypeParameters.Add(GetGenericComparableType());
            sortMethod.Statements.Add(BuildSortMethodInvokeExpression());
            
            return sortMethod;
        }

        private CodeParameterDeclarationExpression[] BuildSortMethodParameters()
        {
            return new[]
            {
                new CodeParameterDeclarationExpression(
                    new CodeTypeReference(CodeDomSortGenerating.Constants.GenericArrayTypeName),
                    CodeDomSortGenerating.Constants.ArrayParameterName),

                new CodeParameterDeclarationExpression(
                    CodeDomSortGenerating.Constants.PivotStrategy.PivotInterfaceName,
                    CodeDomSortGenerating.Constants.PivotElementStrategyParameterName)
            };
        }

        /// <summary>
        /// Returns QuickSortImplementation(array, 0, array.Length-1, pivotElementStrategy) call statement
        /// </summary>
        /// <returns></returns>
        private CodeMethodInvokeExpression BuildSortMethodInvokeExpression()
        {
            return new CodeMethodInvokeExpression(
                
                
                new CodeMethodReferenceExpression(null,
                    CodeDomSortGenerating.Constants.QuickSortImplementationMethodName),
                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                
                new CodePrimitiveExpression(0),
                
                new CodeBinaryOperatorExpression(
                    new CodePropertyReferenceExpression(
                        new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                        "Length"),
                    CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)
                    
                    ),
                
                CodeDomSortGenerating.CommonExpressions.PivotElementStrategyArgumentReference);
        }
        
    }
}