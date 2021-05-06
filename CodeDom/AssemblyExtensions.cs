using System;
using System.Reflection;

namespace CodeDom
{
    public static class AssemblyExtensions
    {
        public static dynamic Instantiate(this Assembly @this, string fullTypeName, params object[] constructorArguments)
        {
            if (@this == null)
            {
                return null;
            }
            
            var quickSorterType = @this.GetType(fullTypeName);
            return Activator.CreateInstance(quickSorterType, constructorArguments);
        }
    }
}