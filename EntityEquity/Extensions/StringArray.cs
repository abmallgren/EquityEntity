namespace EntityEquity.Extensions
{
    public static class StringArray
    {
        public static int[] ToIntArray(this string[] stringArray)
        {
            int[] returnValue = new int[stringArray.Count()];
            for (int i = 0; i < stringArray.Count(); i++)
            {
                returnValue[i] = int.Parse(stringArray[i]);
            }
            return returnValue;
        }
    }
}
