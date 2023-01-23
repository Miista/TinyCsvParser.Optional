// See https://aka.ms/new-console-template for more information

using System.Text;
using Optional;
using TinyCsvParser;
using TinyCsvParser.Extensions.Optional;
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
        public static void Main(string[] args)
        {
            var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
            var typeConverterProvider = new TypeConverterProvider().AddOptional();
            var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
            var readerOptions = new CsvReaderOptions(new[] { ";" });
            var results = parser.ReadFromString(readerOptions, $"0,null,null,null,null;null,2,null,null,null;null,null,{Guid.Empty},null,false").ToList();

            Console.WriteLine($"Results: {results.Count}");
            foreach (var result in results)
            {
                if (result.IsValid) Console.WriteLine(result.Result.ToString());
                else Console.WriteLine($"Result is invalid: {result.Error}");
            }
        }
    }
}