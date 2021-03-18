﻿
namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        private static class Constants
        {  
            public const string ProgrammingLanguage                 = "c#";
            public const string NamespaceName                       = "AutoGenerated.Sorting.QuickSort";
            public const string OutputAssemblyName                  = "SortionAssembly";
            public const string LowIndexParameterName               = "lowIndex";
            public const string TopIndexParameterName               = "topIndex";
            public const string PivotElementStrategyParameterName   = "pivotElementStrategy";
            public const string ArrayParameterName                  = "array";
            public const string PivotElementStrategy                = "pivotElementStrategy";
            public const string QuickSortMethodName                 = "QuickSort";
            public const string QuickSortImplementationMethodName   = "QuickSortImplementation";
            public const string GenericTypeName                     = "T";
            public const string GenericArrayTypeName                = "T[]";

            public static class PivotStrategy
            {
                public const string PivotInterfaceName              = "IPivotElementStrategy";
                public const string PivotMethodName                 = "GetPivot";
                public const string FirstElementPivotStrategy       = "FirstElementPivotStrategy";
                public const string LastElementPivotStrategy        = "LastElementPivotStrategy";
            }

            public static class Partition
            {
                public const string PartitionMethodName             = "Partition";
                public const string LeftToRightVariableName         = "leftToRight";
                public const string RightToLeftVariableName         = "rightToLeft";
            }
        } 
    }
}