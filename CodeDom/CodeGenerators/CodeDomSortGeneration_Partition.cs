using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        
        private CodeMemberMethod CreatePartitionMethod()
        {
            var partitionMethod = new CodeMemberMethod
            {
                Name = Constants.Partition.PartitionMethodName, 
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                ReturnType = new CodeTypeReference(typeof(int))
            };

            partitionMethod.TypeParameters.Add(_genericComparableType);
            
            var arrayParameter = new CodeParameterDeclarationExpression(Constants.GenericArrayTypeName, Constants.ArrayParameterName);
            
            var lowIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.LowIndexParameterName);
            var topIndexParameter = new CodeParameterDeclarationExpression(typeof(int), Constants.TopIndexParameterName);

            var pivotStrategyParameter =
                new CodeParameterDeclarationExpression(Constants.PivotStrategy.PivotInterfaceName,
                    Constants.PivotElementStrategy);
            
            partitionMethod.Parameters.AddRange(new[]
            {
                arrayParameter,
                lowIndexParameter,
                topIndexParameter,
                pivotStrategyParameter
            });

            partitionMethod.Statements.Add(DeclarePivotVariable());
            partitionMethod.Statements.Add(DeclareIndexVariable(Constants.Partition.LeftToRightVariableName, Constants.LowIndexParameterName));
            partitionMethod.Statements.Add(DeclareIndexVariable(Constants.Partition.RightToLeftVariableName, Constants.TopIndexParameterName));

            partitionMethod.Statements.Add(DeclareLimitlessLoop());
            
            return partitionMethod;
        }

        private CodeVariableDeclarationStatement DeclarePivotVariable()
        {
            return new CodeVariableDeclarationStatement(Constants.GenericTypeName, "pivot",
                new CodeMethodInvokeExpression(
                    new CodeArgumentReferenceExpression(Constants.PivotElementStrategyParameterName), "GetPivot",
                    new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                    new CodeArgumentReferenceExpression(Constants.LowIndexParameterName),
                    new CodeArgumentReferenceExpression(Constants.TopIndexParameterName)));
        }

        private CodeVariableDeclarationStatement DeclareIndexVariable(string variableName, string argumentName)
        {
            return new CodeVariableDeclarationStatement(typeof(int), variableName,
                new CodeArgumentReferenceExpression(argumentName));
        }
        
        private CodeStatement DeclareLimitlessLoop()
        {
            //since CodeDom doesn't provide while and do-while loops
            //to arrange limitless iterating we generate for(;true;) here
            //that is why we pass an empty CodeSnippetStatement at 1st and 3rd position 
            var loopStatement = new CodeIterationStatement(new CodeSnippetStatement(), new CodePrimitiveExpression(true),
                new CodeSnippetStatement());
            
            loopStatement.Statements.Add(DeclareLeftToRightStatement());
            loopStatement.Statements.Add(DeclareRightToLeftStatement());
            loopStatement.Statements.Add(DeclareLeftAndRightIndexes());
            return loopStatement;
        }

        private CodeStatement DeclareLeftToRightStatement()
        {
            // conditions is leftToRight <= top && array[leftToRight].CompareTo(pivot) < 0
            var leftToRightLoopStatement = new CodeIterationStatement(new CodeSnippetStatement(), 
                new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("leftToRight"), 
                        
                        CodeBinaryOperatorType.LessThan,
                        new CodeArgumentReferenceExpression(Constants.TopIndexParameterName)),
                    
                    CodeBinaryOperatorType.BooleanAnd,
                    
                    new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(Constants.ArrayParameterName), 
                            new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName)), "CompareTo", new CodeVariableReferenceExpression("pivot")) , CodeBinaryOperatorType.LessThan,
                        new CodePrimitiveExpression(0))), new CodeSnippetStatement());
            
            leftToRightLoopStatement.Statements.Add( new CodeAssignStatement(new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName), new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName), CodeBinaryOperatorType.Add,
                new CodePrimitiveExpression(1))));
            
            return leftToRightLoopStatement;
        }

        private CodeStatement DeclareRightToLeftStatement()
        {
            // conditions is rightToLefty >= low && array[leftToRight].CompareTo(pivot) > 0
            var rightToLeftLoopStatement = new CodeIterationStatement(new CodeSnippetStatement(), 
                new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("rightToLeft"), 
                        
                        CodeBinaryOperatorType.GreaterThan,
                        new CodeArgumentReferenceExpression(Constants.LowIndexParameterName)),
                    
                    CodeBinaryOperatorType.BooleanAnd,
                    
                    new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(Constants.ArrayParameterName), 
                            new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName)), "CompareTo", new CodeVariableReferenceExpression("pivot")) , CodeBinaryOperatorType.GreaterThan,
                        new CodePrimitiveExpression(0))), new CodeSnippetStatement());

            rightToLeftLoopStatement.Statements.Add(new CodeAssignStatement(
                new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName),
                new CodeBinaryOperatorExpression(

                    new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName),
                    CodeBinaryOperatorType.Subtract,
                    new CodePrimitiveExpression(1)
                )));

            return rightToLeftLoopStatement;
        }

        private CodeStatement DeclareLeftAndRightIndexes()
        {
            var ifStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName),
                CodeBinaryOperatorType.LessThan,
                new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName)));

            ifStatement.TrueStatements.Add(new CodeVariableDeclarationStatement(Constants.GenericTypeName, "temp",
                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName))));

            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName)),
                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName)))
            );

            ifStatement.TrueStatements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(
                    new CodeArgumentReferenceExpression(Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName)),
                new CodeVariableReferenceExpression("temp")));

            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName),
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName),
                    CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
            
            
            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName),
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName),
                    CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));

            // rightToLeft == topIndex ? topIndex-1 : rightToLeft;
            // ifStatement.FalseStatements.Add(
            //     new CodeMethodReturnStatement(
            //         new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName)));

            ifStatement.FalseStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName),
                    CodeBinaryOperatorType.ValueEquality,
                    new CodeArgumentReferenceExpression(Constants.TopIndexParameterName)), new CodeStatement[]
                {
                    new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(
                        new CodeArgumentReferenceExpression(Constants.TopIndexParameterName),
                        CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)))
                }, new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeVariableReferenceExpression(Constants.Partition.RightToLeftVariableName))
                }));

            return ifStatement;
        }
    }
}