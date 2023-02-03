using System;
using System.Linq;
using System.Reflection;
using Optional;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Optional
{
    public class OptionalTypeConverterProviderDecorator : ITypeConverterProvider
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public OptionalTypeConverterProviderDecorator(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public ITypeConverter<TTargetType> Resolve<TTargetType>()
        {
            if (typeof(TTargetType).IsGenericType && typeof(TTargetType).GetGenericTypeDefinition() == typeof(Option<>))
            {
                var typeArgument = typeof(TTargetType).GenericTypeArguments.First();

                return CreateConverter<TTargetType>(typeArgument, _typeConverterProvider);
            }

            return _typeConverterProvider.Resolve<TTargetType>();
        }

        private static ITypeConverter<T> CreateConverter<T>(Type targetType, ITypeConverterProvider typeConverterProvider)
        {
            var createMethod =
                typeof(OptionalTypeConverterProviderDecorator)
                    .GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(targetType)
                ?? throw new InvalidOperationException($"Cannot make static generic method from '{nameof(Create)}");

            var optionalConverterInstance = createMethod.Invoke(null, new object[] { typeConverterProvider })
                                            ?? throw new InvalidOperationException(
                                                $"Cannot make instance of OptionalConverter specialized to type '{targetType}'"
                                            );

            return optionalConverterInstance as ITypeConverter<T>;
        }

        private static OptionalConverter<T> Create<T>(ITypeConverterProvider typeConverterProvider) =>
            new OptionalConverter<T>(typeConverterProvider);

        public IArrayTypeConverter<TTargetType> ResolveCollection<TTargetType>() =>
            _typeConverterProvider.ResolveCollection<TTargetType>();
    }
}