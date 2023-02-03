using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Optional
{
    public static class TypeConverterProviderExtensions
    {
        public static ITypeConverterProvider AddOptional(this ITypeConverterProvider typeConverterProvider)
        {
            if (typeConverterProvider == null) throw new ArgumentNullException(nameof(typeConverterProvider));

            return new OptionalTypeConverterProviderDecorator(typeConverterProvider);
            
        }
    }
}