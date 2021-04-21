using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        
        private CodeMemberMethod CreateSortMethod()
        {
            var sortMethod = new CodeMemberMethod
            {
                Name = Constants.QuickSortMethodName, 
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            
            var arrayParameter = new CodeParameterDeclarationExpression(new CodeTypeReference(Constants.GenericArrayTypeName),
                Constants.ArrayParameterName);
            var pivotStrategyParameter = new CodeParameterDeclarationExpression(
                Constants.PivotStrategy.PivotInterfaceName,
                Constants.PivotElementStrategyParameterName);

            sortMethod.Parameters.AddRange(new[]
            {
                arrayParameter,
                pivotStrategyParameter
            });
           
            sortMethod.TypeParameters.Add(GetGenericComparableType());
            sortMethod.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                new CodeArgumentReferenceExpression(Constants.ArrayParameterName), new CodePrimitiveExpression(0),
                new CodeBinaryOperatorExpression(
                    new CodePropertyReferenceExpression(
                        new CodeArgumentReferenceExpression(Constants.ArrayParameterName), "Length"),
                    CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)),
                CommonExpressions.PivotElementStrategyArgumentReference)
            );
            
            return sortMethod;
        }
    }
}