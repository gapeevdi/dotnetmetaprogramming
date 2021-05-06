using System;

namespace CodeDom
{
    public static class ArrayGenerator
    {
        public static int[] GenerateArray(int length)
        {
            var rnd = new Random();
            var array = new int[length];
            for (var i = 0; i < array.Length-1; i++)
            {
                array[i] = rnd.Next(1000);
            }

            array[array.Length - 1] = array[0];
            array[4] = 123;
            array[10] = 123;

            return array;
        }
    }
}