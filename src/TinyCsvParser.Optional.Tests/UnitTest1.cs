using System.Linq;
using FluentAssertions;
using Optional;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;
using Xunit;

namespace TinyCsvParser.Optional.Tests
{
    public class UnitTest1
    {
        private class Data
        {
            public class Mapping : CsvMapping<Data>
            {
                public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                {
                    MapProperty(0, data => data.OptionalInt);
                    MapProperty(1, data => data.OptionalString);
                }
            }

            public Option<int> OptionalInt { get; set; }
            public Option<string> OptionalString { get; set; }
        }
        
        [Fact]
        public void Can_parse_optional_int()
        {
            // Arrange
            const int value = 1;
            
            var (parser, readerOptions) = CreateCsvParser();
            
            // Act
            var results = parser.ReadFromString(readerOptions, $"{value},null").ToList();

            // Assert
            results.Should().NotBeNullOrEmpty();
            var firstResult = results.FirstOrDefault();

            firstResult.Should().NotBeNull();
            firstResult.Result.OptionalInt.Should().Be(value.Some());
        }
        
        [Fact]
        public void Can_parse_optional_string()
        {
            // Arrange
            const string value = "1";
            
            var (parser, readerOptions) = CreateCsvParser();
            
            // Act
            var results = parser.ReadFromString(readerOptions, $"0,{value}").ToList();

            // Assert
            results.Should().NotBeNullOrEmpty();
            var firstResult = results.FirstOrDefault();

            firstResult.Should().NotBeNull();
            firstResult.Result.OptionalString.Should().Be(value.Some());
        }
        
        [Fact]
        public void Can_parse_optional_string_from_null()
        {
            // Arrange
            const string value = null;
            
            var (parser, readerOptions) = CreateCsvParser();
            
            // Act
            var results = parser.ReadFromString(readerOptions, $"0,{value}").ToList();

            // Assert
            results.Should().NotBeNullOrEmpty();
            var firstResult = results.FirstOrDefault();

            firstResult.Should().NotBeNull();
            firstResult.Result.OptionalString.Should().Be(string.Empty.Some());
        }

        private static (CsvParser<Data> Parser, CsvReaderOptions ReaderOptions) CreateCsvParser()
        {
            var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
            var typeConverterProvider = new TypeConverterProvider().AddOptional();
            var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
            
            var readerOptions = new CsvReaderOptions(new[] { ";" });
            
            return (Parser: parser, ReaderOptions: readerOptions);
        }
    }
}