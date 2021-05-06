using System.CodeDom;

namespace CodeDomSort
{
    public class CodeDomPivotElementStrategyBuilder
    {
        public CodeTypeDeclaration BuildFirstElementAsPivotStrategyType()
        {

            var returnStatement = new CodeMethodReturnStatement(new CodeArrayIndexerExpression(
                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.LowIndexParameterName)));
            
            var firstElementPivotStrategyType =
                DefinePivotInterfaceImplementation(CodeDomSortGenerating.Constants.PivotStrategy
                    .FirstElementPivotStrategy, returnStatement);

            return firstElementPivotStrategyType;
        }
        
        public CodeTypeDeclaration BuildLastElementAsPivotStrategyType()
        {
            
            var returnStatement = new CodeMethodReturnStatement(new CodeArrayIndexerExpression(
                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.ArrayParameterName),
                new CodeArgumentReferenceExpression(CodeDomSortGenerating.Constants.TopIndexParameterName)));

            var lastElementPivotStrategyType =
                DefinePivotInterfaceImplementation(
                    CodeDomSortGenerating.Constants.PivotStrategy.LastElementPivotStrategy, returnStatement);

            return lastElementPivotStrategyType;
        }

        /// <summary>
        /// Defining interface IPivotElementStrategy which implementation provides
        /// strategy to choose a pivot for the algorithm 
        /// </summary>
        /// <returns></returns>
        public CodeTypeDeclaration BuildPivotElementStrategyInterface()
        {
            var pivotElementInterface = new CodeTypeDeclaration
            {
                IsInterface = true,
                Name = CodeDomSortGenerating.Constants.PivotStrategy.PivotInterfaceName
            };

            pivotElementInterface.Members.Add(PivotMethodDeclaration());

            return pivotElementInterface;
        }

        private CodeTypeDeclaration DefinePivotInterfaceImplementation(string pivotStrategyClassName, CodeMethodReturnStatement returnStatement)
        {
            var pivotImplementation = new CodeTypeDeclaration()
            {
                IsClass = true,
                Name = pivotStrategyClassName
            };

            pivotImplementation.BaseTypes.Add(
                new CodeTypeReference(CodeDomSortGenerating.Constants.PivotStrategy.PivotInterfaceName));

            var pivotMethod = PivotMethodDeclaration();
            pivotMethod.Statements.Add(returnStatement);
            pivotImplementation.Members.Add(pivotMethod);
            
            return pivotImplementation;
        }


        private CodeMemberMethod PivotMethodDeclaration()
        {
            var pivotMethodDeclaration = new CodeMemberMethod
            {
                Name = CodeDomSortGenerating.Constants.PivotStrategy.PivotMethodName,
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference(CodeDomSortGenerating.Constants.GenericTypeName),
                TypeParameters = {CodeDomSortGenerating.CommonExpressions.GenericComparableType}
            };

            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(CodeDomSortGenerating.Constants.GenericArrayTypeName, CodeDomSortGenerating.Constants.ArrayParameterName));
            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(int), CodeDomSortGenerating.Constants.LowIndexParameterName));
            pivotMethodDeclaration.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(int), CodeDomSortGenerating.Constants.TopIndexParameterName));

            return pivotMethodDeclaration;

        }

    }
}