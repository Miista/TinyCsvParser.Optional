[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet version](https://badge.fury.io/nu/TinyCsvParser.Optional.svg)](https://www.nuget.org/packages/TinyCsvParser.Optional)

# TinyCsvParser.Optional

Adding support for parsing options.

Supports **.NET Core** (.NET Standard 2+)

## Installation

```
PM> Install-Package TinyCsvParser.Optional
```

## Usage

The only thing you need to keep in mind when using this extension
is that your mapping class must have a constructor taking in an instance of `ITypeConverterProvider`
and passing it on to its base constructor. See example below.

```csharp
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
    // Need to take in ITypeConverterProvider
    public CsvPersonMapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
    {
        MapProperty(0, x => x.FirstName);
        MapProperty(1, x => x.LastName);
        MapProperty(2, x => x.BirthDate);
    }
}

// Parsing
var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
var typeConverterProvider = new TypeConverterProvider().AddOptional(); // <-- This line
var parser = new CsvParser<Person>(options, new CsvPersonMapping(typeConverterProvider));
var result = parser.ReadFromString(readerOptions, $"Philipp,Wagner,null").ToList();

Console.WriteLine(result[0].Result.FirstName); // Writes Philipp
Console.WriteLine(result[0].Result.LastName); // Writes Wagner
Console.WriteLine(result[0].Result.BirthDate); // Writes None
```

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.
