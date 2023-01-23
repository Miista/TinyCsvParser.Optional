using System;
using System.Collections.Generic;
using System.Reflection;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Extensions.Optional
{
    public static class TypeConverterProviderExtensions
    {
        public static ITypeConverterProvider AddOptional(this TypeConverterProvider typeConverterProvider)
        {
            var registeredTypeConvertersField = typeof(TypeConverterProvider)
                                                    .GetField("typeConverters", BindingFlags.Instance | BindingFlags.NonPublic)
                                                ?? throw new InvalidOperationException(
                                                    $"Cannot get field 'typeConverters' from type {nameof(TypeConverterProvider)}"
                                                );

            var registeredTypeConvertersDictionary = registeredTypeConvertersField.GetValue(typeConverterProvider) as Dictionary<Type, ITypeConverter>
                                                     ?? throw new InvalidOperationException($"Unexpected field type. Expected {nameof(Dictionary<Type, ITypeConverter>)}");

            var alreadyRegisteredTypes = new Type[registeredTypeConvertersDictionary.Keys.Count];
            registeredTypeConvertersDictionary.Keys.CopyTo(alreadyRegisteredTypes, 0);
            
            foreach (var type in alreadyRegisteredTypes)
            {
                // 1. Create instance of OptionalConverter<T>
                var createMethod =
                    typeof(TypeConverterProviderExtensions)
                        .GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(type)
                    ?? throw new InvalidOperationException($"Cannot make static generic method from '{nameof(Create)}");
                // ReSharper disable once CoVariantArrayConversion
                var optionalConverterInstance = createMethod.Invoke(null, new[] { typeConverterProvider })
                                                ?? throw new Exception(
                                                    $"Cannot make instance of {typeof(OptionalConverter<>)} specialized to type '{type}'"
                                                );

                // 2. Add the instance to the TypeConverterProvider
                var addMethod =
                    typeof(TypeConverterProviderExtensions)
                        .GetMethod(nameof(Add), BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(type)
                    ?? throw new InvalidOperationException($"Cannot make static generic method from '{nameof(Add)}");
                // ReSharper disable once UnusedVariable
                var voidReturnValue = addMethod.Invoke(null, new[]{optionalConverterInstance, typeConverterProvider});
            }
            
            return typeConverterProvider;
        }

        private static OptionalConverter<T> Create<T>(ITypeConverterProvider typeConverterProvider)
        {
            return new OptionalConverter<T>(typeConverterProvider);
        }

        private static void Add<T>(OptionalConverter<T> converter, TypeConverterProvider typeConverterProvider)
        {
            typeConverterProvider.Add(converter);
        }
    }
}