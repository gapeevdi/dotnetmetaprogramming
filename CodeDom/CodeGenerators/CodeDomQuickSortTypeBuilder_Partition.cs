using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomQuickSortTypeBuilder
    {

        private CodeMemberMethod CreatePartitionMethod()
        {
            var partitionMethod = new CodeMemberMethod
            {
                Name = CodeDomSortGenerating.Constants.Partition.PartitionMethodName,
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                ReturnType = new CodeTypeReference(typeof(int))
            };

            partitionMethod.TypeParameters.Add(GetGenericComparableType());

            partitionMethod.Parameters.AddRange(DeclarePartitionMethodParameters());

            partitionMethod.Statements.Add(DeclarePivotVariable());
            
            partitionMethod.Statements.Add(DeclareIndexVariable(
                CodeDomSortGenerating.Constants.Partition.LeftToRightVariableName,
                CodeDomSortGenerating.Constants.LowIndexParameterName));
            
            
            partitionMethod.Statements.Add(DeclareIndexVariable(
                CodeDomSortGenerating.Constants.Partition.RightToLeftVariableName,
                CodeDomSortGenerating.Constants.TopIndexParameterName));
            
            partitionMethod.Statements.Add(DeclareLimitlessLoop());

            return partitionMethod;
        }

        private CodeParameterDeclarationExpression[] DeclarePartitionMethodParameters() => 
            new[]
        {
            new CodeParameterDeclarationExpression(CodeDomSortGenerating.Constants.GenericArrayTypeName,
                CodeDomSortGenerating.Constants.ArrayParameterName),

            new CodeParameterDeclarationExpression(typeof(int),
                CodeDomSortGenerating.Constants.LowIndexParameterName),

            new CodeParameterDeclarationExpression(typeof(int),
                CodeDomSortGenerating.Constants.TopIndexParameterName),

            new CodeParameterDeclarationExpression(CodeDomSortGenerating.Constants.PivotStrategy.PivotInterfaceName,
                CodeDomSortGenerating.Constants.PivotElementStrategy)
        };

        /// <summary>
        /// Returns a statement declaring the pivot variable
        /// </summary>
        /// <returns>T pivot = pivotElementStrategy.GetPivot(array, lowIndex, topIndex)</returns>
        private CodeVariableDeclarationStatement DeclarePivotVariable()
        {
            var localVariableName = "pivot";
            
            return new CodeVariableDeclarationStatement(CodeDomSortGenerating.Constants.GenericTypeName, localVariableName,
                new CodeMethodInvokeExpression(
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.PivotElementStrategyParameterName), 
                    
                    methodName: CodeDomSortGenerating.Constants.PivotStrategy.PivotMethodName,
                    
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.LowIndexParameterName),
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.TopIndexParameterName)));
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
            //that is why we convey an empty CodeSnippetStatement at 1st and 3rd position 
            var loopStatement = new CodeIterationStatement(new CodeSnippetStatement(), new CodePrimitiveExpression(true),
                new CodeSnippetStatement());
            
            loopStatement.Statements.Add(DeclareLeftToRightLoopStatement());
            loopStatement.Statements.Add(DeclareRightToLeftStatement());
            loopStatement.Statements.Add(DeclareConditionToSwapLeftAndRightValue());
            
            return loopStatement;
        }

        /// <summary>
        /// Returns a loop that moves leftToRight pointer forward
        /// </summary>
        /// <returns></returns>
        private CodeStatement DeclareLeftToRightLoopStatement()
        {
            // conditions is leftToRight <= top && array[leftToRight].CompareTo(pivot) < 0
            
            // for( ; leftToRight < topIndex && && array[leftToRight].CompareTo(pivot) < 0 ; )
            var leftToRightLoopStatement = new CodeIterationStatement(new CodeSnippetStatement(),
                new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(

                        // leftToRight < topIndex
                        new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.Partition
                            .LeftToRightVariableName),
                        CodeBinaryOperatorType.LessThan,
                        new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.TopIndexParameterName)),

                    // &&
                    CodeBinaryOperatorType.BooleanAnd,

                    // array[leftToRight].CompareTo(pivot) < 0
                    new CodeBinaryOperatorExpression(

                        // array[leftToRight].CompareTo(pivot)
                        new CodeMethodInvokeExpression(new CodeArrayIndexerExpression(
                                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                                    .LeftToRightVariableName)),

                            methodName: "CompareTo",
                            
                            parameters: new CodeVariableReferenceExpression("pivot")),

                        CodeBinaryOperatorType.LessThan, // <

                        new CodePrimitiveExpression(0) // 0
                    )
                ), new CodeSnippetStatement());


            // leftToRight = leftToRight + 1;
            leftToRightLoopStatement.Statements.Add(

                new CodeAssignStatement(
                    
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .LeftToRightVariableName),

                    //leftToRight + 1
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                            .LeftToRightVariableName),

                        CodeBinaryOperatorType.Add,

                        new CodePrimitiveExpression(1))));

            return leftToRightLoopStatement;
        }

        /// <summary>
        /// Returns a loop that moves rightToLeft pointer backward
        /// </summary>
        /// <returns></returns>
        private CodeStatement DeclareRightToLeftStatement()
        {
            // conditions is rightToLefty >= low && array[rightToLeft].CompareTo(pivot) > 0

            // for( ; rightToLeft >= low && array[rightToLeft].CompareTo(pivot) > 0 ; )
            var rightToLeftLoopStatement = new CodeIterationStatement(new CodeSnippetStatement(),

                // rightToLeft >= low
                new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression(
                            CodeDomSortGenerating.Constants.Partition
                                .RightToLeftVariableName),

                        //>=
                        CodeBinaryOperatorType.GreaterThan,

                        // low
                        new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.LowIndexParameterName)),

                    //&&
                    CodeBinaryOperatorType.BooleanAnd,

                    // array[rightToLeft].CompareTo(pivot) > 0
                    new CodeBinaryOperatorExpression(

                        new CodeMethodInvokeExpression(new CodeArrayIndexerExpression(
                                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                                    .RightToLeftVariableName)),

                            methodName: "CompareTo",

                            new CodeVariableReferenceExpression("pivot")),

                        CodeBinaryOperatorType.GreaterThan, // >

                        new CodePrimitiveExpression(0))), // 0 

                new CodeSnippetStatement());

            // rightToLeft = rightToLeft + 1
            rightToLeftLoopStatement.Statements.Add(

                new CodeAssignStatement(

                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .RightToLeftVariableName),

                    //rightToLeft - 1
                    new CodeBinaryOperatorExpression(

                        new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                            .RightToLeftVariableName),

                        CodeBinaryOperatorType.Subtract, // -

                        new CodePrimitiveExpression(1)
                    )));

            return rightToLeftLoopStatement;
        }


        private CodeStatement DeclareConditionToSwapLeftAndRightValue()
        {
            // if(leftToRight < rightToLeft)
            var ifStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition.LeftToRightVariableName),
                CodeBinaryOperatorType.LessThan,
                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition.RightToLeftVariableName)));

            // T temp = array[leftToRight]
            ifStatement.TrueStatements.Add(new CodeVariableDeclarationStatement(CodeDomSortGenerating.Constants.GenericTypeName, "temp",
                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition.LeftToRightVariableName))));

            // array[leftToRight] = array[rightToLeft]
            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                
                new CodeArrayIndexerExpression(
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .LeftToRightVariableName)),
                
                new CodeArrayIndexerExpression(
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .RightToLeftVariableName)))
            );

            // array[rightToLeft] = temp
            ifStatement.TrueStatements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .RightToLeftVariableName)),
                new CodeVariableReferenceExpression("temp")));

            // leftToRight = leftToRight + 1
            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition.LeftToRightVariableName),

                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .LeftToRightVariableName),

                    CodeBinaryOperatorType.Add,

                    new CodePrimitiveExpression(1))));
            
            
            //rightToLeft = rightToLeft - 1
            ifStatement.TrueStatements.Add(new CodeAssignStatement(
                new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition.RightToLeftVariableName),

                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .RightToLeftVariableName),

                    CodeBinaryOperatorType.Subtract,

                    new CodePrimitiveExpression(1))));

            // rightToLeft == topIndex ? topIndex-1 : rightToLeft;
            // ifStatement.FalseStatements.Add(
            //     new CodeMethodReturnStatement(
            //         new CodeVariableReferenceExpression(Constants.Partition.LeftToRightVariableName)));


            // else block for if(leftToRight < rightToLeft)
            ifStatement.FalseStatements.Add(
                
                // if(rightToLeft == topIndex)
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                        .RightToLeftVariableName),
                    CodeBinaryOperatorType.ValueEquality,
                    new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.TopIndexParameterName)),
                
                // return topIndex - 1
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(
                        new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.TopIndexParameterName),
                        CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)))
                },
                    // else return RightToLeftVariableName
                    new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeVariableReferenceExpression(CodeDomSortGenerating.Constants.Partition
                            .RightToLeftVariableName))
                }));

            return ifStatement;
        }
        
    }
}