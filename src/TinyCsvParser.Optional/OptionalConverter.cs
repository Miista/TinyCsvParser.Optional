using System;
using Optional;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Optional
{
    public class OptionalConverter<T> : ITypeConverter<Option<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public OptionalConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string value, out Option<T> result)
        {
            var innerTypeConverter = _typeConverterProvider.Resolve<T>();

            result = innerTypeConverter.TryConvert(value, out var innerResult)
                ? innerResult.Some()
                : Option.None<T>();

            return true;
        }

        public Type TargetType { get; } = typeof(Option<T>);
    }
}