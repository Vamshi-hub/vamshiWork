using System;
using System.Reflection;

namespace astorWorkShared.Utilities
{
    public static class CommonUtility
    {
        public static void MemberwiseCopy(object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            if (sourceType.IsEquivalentTo(targetType))
            {
                try
                {
                    foreach (PropertyInfo propertyInfo in sourceType.GetProperties())
                    {
                        if (propertyInfo.CanRead && propertyInfo.CanWrite)
                        {
                            object value = propertyInfo.GetValue(source, null);
                            propertyInfo.SetValue(target, value);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Cannot copy property from {sourceType.Name} to {targetType.Name} because {exc.Message}");
                }
            }
        }
    }
}
