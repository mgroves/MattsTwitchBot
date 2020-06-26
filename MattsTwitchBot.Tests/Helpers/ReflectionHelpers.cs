using System;
using System.Reflection;

namespace MattsTwitchBot.Tests.Helpers
{
    /// <summary>
    /// ABSOLUTELY do not use these outside of testing
    /// </summary>
    public static class ReflectionHelpers
    {
        public static T GetInternalPropertyValue<T,K>(this K obj, string propertyName)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            if(property == null)
                throw new Exception($"Type {obj.GetType()} has no property called {propertyName}.");
            var value = property.GetValue(obj);
            if(value.GetType() != typeof(T))
                throw new Exception($"Property {propertyName} is not of type {typeof(T).Name}");
            return (T) value;
        }
    }
}