using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        private readonly CodeTypeParameter _genericComparableType = new CodeTypeParameter()
        {
            Name = Constants.GenericTypeName,
            Constraints =
            {
                new CodeTypeReference("IComparable", new CodeTypeReference(Constants.GenericTypeName))
            }
        };
            
        private CodeMemberMethod CreateSortMethod()
        {
            var sortMethod = new CodeMemberMethod
            {
                Name = Constants.QuickSortMethodName, 
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            
            var arrayParameter = new CodeParameterDeclarationExpression(new CodeTypeReference("T[]"),
                Constants.ArrayParameterName);
            var pivotStrategyParameter = new CodeParameterDeclarationExpression(Constants.PivotStrategy.PivotInterfaceName, 
                Constants.PivotElementStrategyParameterName);

            sortMethod.Parameters.AddRange(new[]
            {
                arrayParameter,
                pivotStrategyParameter
            });
           
            sortMethod.TypeParameters.Add(_genericComparableType);
            sortMethod.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                new CodeArgumentReferenceExpression(Constants.ArrayParameterName), new CodePrimitiveExpression(0),
                new CodeBinaryOperatorExpression(
                    new CodePropertyReferenceExpression(
                        new CodeArgumentReferenceExpression(Constants.ArrayParameterName), "Length"),
                    CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)),
                _pivotElementStrategyArgumentReference)
            );
            
            return sortMethod;
        }
    }
}