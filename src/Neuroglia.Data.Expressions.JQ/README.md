# Neuroglia.Data.Expressions.JQ

A .NET package to evaluate expressions using `jq`. This library integrates `jq` with .NET applications, making it simple to work with JSON data transformations and expressions.

## Getting Started

### Installation

1. **Add the package to your .NET project:**

```bash
dotnet add package Neuroglia.Data.Expressions.JQ
```

2. **Ensure `jq` is installed:**

```bash
apt-get update
apt-get install jq -y
```

For other systems, refer to the official [`jq` installation guide](https://stedolan.github.io/jq/download/).

---

### Configuration

Optionally, configure the JQ Expression Evaluator in your application's Dependency Injection (DI) container:

```csharp
services.AddJQExpressionEvaluator(options => 
{
    options.UseSerializer<MyJsonSerializerImplementation>(); // Optional: Defaults to the first registered `IJsonSerializer`
}, ServiceLifetime.Singleton);
```

---

### Usage

After setup, you can start using the evaluator to process expressions.

#### Example Code

```csharp
var defaultEvaluator = serviceProvider.GetRequiredService<IExpressionEvaluator>();
var explicitEvaluator = serviceProvider.GetRequiredService<IExpressionEvaluatorProvider>().GetEvaluator("jq");

var expression = "{ \"foo\": .foo, \"bar\": $param1.bar }";
var input = new
{
    foo = "bar"
};
var arguments = new Dictionary<string, object>()
{
    { 
        "param1", 
        new
        {
            bar = "baz"
        } 
    }
};

var result = await defaultEvaluator.EvaluateAsync(expression, input, arguments, typeof(MyResult));

public class MyResult
{
    public string Foo { get; set; }
    public string Bar { get; set; }
}
```

---

## Features

- Easily integrate the `jq` tool with .NET applications.
- Support for Dependency Injection with customizable options.
- Strongly-typed evaluation for JSON transformation.
- Flexible serialization options.

---

## Requirements

- **.NET Version**: Compatible with .NET 9+.
- **jq**: Ensure `jq` is installed on your system.

---

## Contributing

Feel free to contribute to this project! Please submit issues, feature requests, or pull requests on the [GitHub repository](https://github.com/your-repo-here).

---

## License

This project is licensed under the `Apache 2.0` license.
