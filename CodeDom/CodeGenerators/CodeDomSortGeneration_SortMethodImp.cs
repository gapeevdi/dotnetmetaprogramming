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

            sortMethodImplementation.TypeParameters.Add(_genericComparableType);
            sortMethodImplementation.Parameters.AddRange(CreateSortImplementationParameters());

            var guardStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(
                    _lowIndexArgumentReference,
                    CodeBinaryOperatorType.LessThan,
                    _topIndexArgumentReference),
                new CodeVariableDeclarationStatement(typeof(int), "borderIndex",
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(null, Constants.Partition.PartitionMethodName),
                        _arrayArgumentReference,
                        _lowIndexArgumentReference,
                        _topIndexArgumentReference,
                        _pivotElementStrategyArgumentReference)),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                    _arrayArgumentReference,
                    _lowIndexArgumentReference,
                    new CodeVariableReferenceExpression("borderIndex"),
                    _pivotElementStrategyArgumentReference)),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, Constants.QuickSortImplementationMethodName),
                    _arrayArgumentReference,
                    new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("borderIndex"),
                        CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)),
                    _topIndexArgumentReference,
                    _pivotElementStrategyArgumentReference))
            );

            sortMethodImplementation.Statements.Add(guardStatement);

            return sortMethodImplementation;
        }

        private CodeParameterDeclarationExpression[] CreateSortImplementationParameters()
        {
            var arrayParameter = new CodeParameterDeclarationExpression(new CodeTypeReference("T[]"),
                Constants.ArrayParameterName);

            var lowIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.LowIndexParameterName);
            var topIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.TopIndexParameterName);

            var pivotStrategyParameter =
                new CodeParameterDeclarationExpression(Constants.PivotStrategy.PivotInterfaceName, Constants.PivotElementStrategyParameterName);

            return new[] {arrayParameter, lowIndexParameter, topIndexParameter, pivotStrategyParameter};
        }
    }
}