// See https://aka.ms/new-console-template for more information

using System.Text;
using Optional;
using TinyCsvParser;
using TinyCsvParser.Optional;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace Sandbox
{
    public class Data
    {
        public class Mapping : CsvMapping<Data>
        {
            public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
            {
                MapProperty(0, data => data.OptionalInt);
                MapProperty(1, data => data.OptionalDouble);
                MapProperty(2, data => data.OptionalGuid);
                MapProperty(3, data => data.OptionalTimeSpan);
                MapProperty(4, data => data.OptionalBool);
            }
        }

        public Option<int> OptionalInt { get; set; }
        public Option<double> OptionalDouble { get; set; }
        public Option<bool?> OptionalBool { get; set; }
        public Option<Guid> OptionalGuid { get; set; }
        public Option<TimeSpan> OptionalTimeSpan { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Instance:");
            
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                builder.AppendLine($"  {propertyInfo.Name}: {propertyInfo.GetValue(this)}");
            }

            return builder.ToString();
        }
    }

    public class Program
    {
        // Entity
        private class Person
        {
            public string FirstName { get; set; }
    
            public string LastName { get; set; }
    
            public Option<DateTime> BirthDate { get; set; }
        }

        // Mapping
        private class CsvPersonMapping : CsvMapping<Person>
        {
            public CsvPersonMapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
            {
                MapProperty(0, x => x.FirstName);
                MapProperty(1, x => x.LastName);
                MapProperty(2, x => x.BirthDate);
            }
        }
        
        public static void Main(string[] args)
        {
            var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
            var typeConverterProvider = new TypeConverterProvider().AddOptional();
            // var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
            var parser = new CsvParser<Person>(options, new CsvPersonMapping(typeConverterProvider));
            var readerOptions = new CsvReaderOptions(new[] { ";" });
            // var results = parser.ReadFromString(readerOptions, $"0,null,null,null,null;null,2,null,null,null;null,null,{Guid.Empty},null,false").ToList();
            var results = parser.ReadFromString(readerOptions, $"Philipp,Wagner,null").ToList();

            Console.WriteLine($"Results: {results.Count}");
            foreach (var result in results)
            {
                if (result.IsValid) Console.WriteLine(result.Result.ToString());
                else Console.WriteLine($"Result is invalid: {result.Error}");
            }
        }
    }
}