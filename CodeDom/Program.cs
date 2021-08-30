using System;
using System.Reflection;
using CodeDom;

namespace CodeDomSort
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var array = ArrayGenerator.Generate(50);
            Console.WriteLine(array.ToFlatText());
            Console.WriteLine(array.ToItemCounts());
            
            var sortingAssembly = new CodeDomSortGenerating().FormAssembly();
            
            var codeDomSorter = sortingAssembly.Instantiate(
                    $"{CodeDomSortGenerating.Constants.NamespaceName}.{CodeDomSortGenerating.Constants.QuickSorterTypeName}");
            
            codeDomSorter.QuickSort<int>(array, 
                sortingAssembly.Instantiate($"{CodeDomSortGenerating.Constants.NamespaceName}.{CodeDomSortGenerating.Constants.PivotStrategy.FirstElementPivotStrategy}"));
            
            Console.WriteLine(array.ToFlatText());
            Console.WriteLine(array.ToItemCounts());
            
        }
    }
}
