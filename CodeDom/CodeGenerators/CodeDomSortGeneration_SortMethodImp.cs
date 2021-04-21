using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        
        private CodeMemberMethod CreateSortImplementation()
        {
            var sortMethodImplementation = new CodeMemberMethod
            {
                Name = Constants.QuickSortImplementationMethodName,
                Attributes = MemberAttributes.Private | MemberAttributes.Final
            };

            sortMethodImplementation.TypeParameters.Add(GetGenericComparableType());
            sortMethodImplementation.Parameters.AddRange(CreateSortImplementationParameters());

            var guardStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(
                    CommonExpressions.LowIndexArgumentReference,
                    CodeBinaryOperatorType.LessThan,
                    CommonExpressions.TopIndexArgumentReference),
                new CodeVariableDeclarationStatement(typeof(int), "borderIndex",
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(null, Constants.Partition.PartitionMethodName),
                        CommonExpressions.ArrayArgumentReference,
                        CommonExpressions.LowIndexArgumentReference,
                        CommonExpressions.TopIndexArgumentReference,
                        CommonExpressions.PivotElementStrategyArgumentReference)),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                    CommonExpressions.ArrayArgumentReference,
                    CommonExpressions.LowIndexArgumentReference,
                    new CodeVariableReferenceExpression("borderIndex"),
                    CommonExpressions.PivotElementStrategyArgumentReference)),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                    CommonExpressions.ArrayArgumentReference,
                    new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("borderIndex"),
                        CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)),
                    CommonExpressions.TopIndexArgumentReference,
                    CommonExpressions.PivotElementStrategyArgumentReference))
            );

            sortMethodImplementation.Statements.Add(guardStatement);

            return sortMethodImplementation;
        }

        private CodeParameterDeclarationExpression[] CreateSortImplementationParameters()
        {
            var arrayParameter = new CodeParameterDeclarationExpression(new CodeTypeReference(Constants.GenericArrayTypeName),
                Constants.ArrayParameterName);

            var lowIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.LowIndexParameterName);
            var topIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.TopIndexParameterName);

            var pivotStrategyParameter =
                new CodeParameterDeclarationExpression(Constants.PivotStrategy.PivotInterfaceName, Constants.PivotElementStrategyParameterName);

            return new[] {arrayParameter, lowIndexParameter, topIndexParameter, pivotStrategyParameter};
        }
    }
}