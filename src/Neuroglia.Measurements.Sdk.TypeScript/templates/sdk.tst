${
    using Typewriter.Extensions.Types;
    using System.Text.RegularExpressions;

    Template(Settings settings)
    {
      settings.PartialRenderingMode = PartialRenderingMode.Combined;
      settings.IncludeProject("Neuroglia.Measurements");
      settings.SkipAddingGeneratedFilesToProject = true;
      settings.OutputFilenameFactory = (File file) =>
      {
        string fullName = "";
        string fileName = "";
        if (file.Classes.Any())
        {
          fullName = file.Classes.First().FullName;
          fileName = ToKebabCase(file.Classes.First().Name);
        }
        if (file.Records.Any())
        {
          fullName = file.Records.First().FullName;
          fileName = ToKebabCase(file.Records.First().Name);
        }
        if (file.Enums.Any())
        {
          fullName = file.Enums.First().FullName;
          fileName = ToKebabCase(file.Enums.First().Name);
        }
        if (!string.IsNullOrWhiteSpace(fullName))
        {
          if (file.Enums.Any()) return $"{PACKAGE_ROOT}\\enums\\{fileName}.ts";
          if (IsModel(fullName))
          {
            var name = fullName.Split('.').Last();
            if (ExtendedModels.Contains(name) || name == "Measurement") return $"{PACKAGE_ROOT}\\models\\{fileName}-base.ts";
            return $"{PACKAGE_ROOT}\\models\\{fileName}.ts";
          }
          if (IsEvent(fullName)) return $"{PACKAGE_ROOT}\\events\\{fileName}.ts";
          if (IsCommand(fullName)) return $"{PACKAGE_ROOT}\\commands\\{fileName}.ts";
          if (IsController(fullName)) return $"{PACKAGE_ROOT}\\{GetServiceFileName(fileName)}.ts";
          if (IsHub(fullName)) return $"{PACKAGE_ROOT}\\{GetSignlaRFileName(fileName)}.ts";
        }
        return "UNSUPPORTED_" + file.Name;
      };
    }

    const string PACKAGE_ROOT = "..\\packages\\neuroglia\\measurements\\src\\lib";
    
    const string PROJECT_DIRECTORY = "Neuroglia.Measurements.Sdk.TypeScript";

    const string CONTAINER_DIRECTORY = "src";    

    string ToKebabCase(string name) => Regex
      .Replace(name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled)
      .Trim()
      .ToLower();

    string GetServiceClassName(string name) => name.Replace("Controller", "Service");

    string GetServiceFileName(string name) => name.Replace("-controller", ".service");

    string GetSignlaRFileName(string name) => name.Replace("-hub", ".signalr.service");

    bool IsModel(string fullName) => !fullName.Contains("Extensions");

    bool IsEvent(string fullName) => fullName.Contains("Integration.Events");

    bool IsCommand(string fullName) =>
      fullName.Contains("Integration.Commands") /*&&
      !fullName.Contains("CommandDataTransferObject")*/;

    bool IsController(string fullName) => fullName.Contains(".Controllers");

    bool IsHub(string fullName) => fullName.EndsWith("Hub");

    System.IO.DirectoryInfo GetClosestDirectory(File file, string target)
    {
      var directory = new System.IO.FileInfo(file.FullName).Directory;
      while (directory.Name != target && directory.Parent != null)
      {
        directory = directory.Parent;
      }
      return directory;
    }

    static Dictionary<string, bool> ExistingExportsIndex = new Dictionary<string, bool>()
    {
        { "models", false },
        { "enums", false },
        { "events", false },
        { "commands", false },
        { "ROOT", false }
    };

    static string[] ExtendedModels = new string[]
    {
      "Capacity",
      "Energy",
      "Length",
      "Mass",
      //"Measurement",
      "Surface",
      "Temperature",
      "Unit",
      "Volume",
    };

    void BuildExportsIndex(File sourceFile, string fullName, string name, bool isEnum = false)
    {
      bool hasIndex = false;
      string typeDestination = "";
      string exportedFileName = ToKebabCase(name);
      if (IsController(fullName))
      {
        hasIndex = ExistingExportsIndex["ROOT"];
        exportedFileName = GetServiceFileName(exportedFileName);
      }
      else if (IsHub(fullName))
      {
        hasIndex = ExistingExportsIndex["ROOT"];
        exportedFileName = GetSignlaRFileName(exportedFileName);
      }
      else if (isEnum) {
        typeDestination = "enums";
        hasIndex = ExistingExportsIndex[typeDestination];
      }
      else if (IsModel(fullName)) {
        if (name == "Measurement" || ExtendedModels.Contains(name))
        {
          return; // will be exported by the extension
        }
        typeDestination = "models";
        hasIndex = ExistingExportsIndex[typeDestination];
      }
      else if (IsEvent(fullName)) {
        typeDestination = "events";
        hasIndex = ExistingExportsIndex[typeDestination];
      }
      else if (IsCommand(fullName)) {
        typeDestination = "commands";
        hasIndex = ExistingExportsIndex[typeDestination];
      }
      string indexFileDestination = System.IO.Path.Combine(new string[]
      {
        GetClosestDirectory(sourceFile, CONTAINER_DIRECTORY).FullName,
        PROJECT_DIRECTORY,
        PACKAGE_ROOT.Replace("..\\", ""),
        typeDestination,
        "index.ts"
      });
      string export = $"export * from './{exportedFileName}';";
      using (var stream = System.IO.File.Open(indexFileDestination, !hasIndex ? System.IO.FileMode.Create : System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read))
      {
        using (var writer = new System.IO.StreamWriter(stream))
        {
          writer.WriteLine(export);
        }
      }
      if (IsController(fullName) || IsHub(fullName)) {
        ExistingExportsIndex["ROOT"] = true;
      }
      else {
        ExistingExportsIndex[typeDestination] = true;
      }
    }

    void BuildExportsIndex(Record record)
    {
      BuildExportsIndex((File)record.Parent, record.FullName, record.Name);
    }

    void BuildExportsIndex(Class clazz)
    {
      BuildExportsIndex((File)clazz.Parent, clazz.FullName, clazz.Name);
    }

    void BuildExportsIndex(Enum enumeration)
    {
      BuildExportsIndex((File)enumeration.Parent, enumeration.FullName, enumeration.Name, true);
    }

    static class Options
    {
      public static char IndentationChar = ' ';
      public static int IndentationCount = 2;
    }

    const string NEW_LINE = "\r\n"; 

    string Indent(int depth, string message, Boolean newLine = true)
    {
      return new String(Options.IndentationChar, Options.IndentationCount * depth) + message + (newLine ? NEW_LINE : "");
    }

    string BuildServiceImports()
    {
      string output = "";
      output += Indent(0, "import { Injectable, inject } from '@angular/core';");
      output += Indent(0, "import { HttpClient, HttpParams } from '@angular/common/http';");
      output += Indent(0, "import { Observable } from 'rxjs';");
      output += Indent(0, "import { catchError, map, tap } from 'rxjs/operators';");
      output += Indent(0, "import { format } from 'date-fns';");
      output += Indent(0, "import { URIComponentQueryEncoder } from '@neuroglia/angular-common';");
      output += Indent(0, "import { ILogger } from '@neuroglia/logging';");
      output += Indent(0, "import { NamedLoggingServiceFactory } from '@neuroglia/angular-logging';");
      output += Indent(0, "import {");
      output += Indent(1, "defaultHttpOptions,");
      output += Indent(1, "HttpErrorObserverService,");
      output += Indent(1, "HttpRequestInfo,");
      output += Indent(1, "logHttpRequest,");
      output += Indent(1, "ODataQueryOptions,");
      output += Indent(1, "ODataQueryResultDto,");
      output += Indent(1, "UrlHelperService,");
      output += Indent(0, "} from '@neuroglia/angular-rest-core';");
      output += Indent(0, "import { REST_API_URL_TOKEN } from './rest-api-url-token';");
      output += Indent(0, "import * as Models from './models';");
      //output += Indent(0, "import * as Enums from './enums';");
      output += Indent(0, "import * as Commands from './commands';");
      output += Indent(0, "import * as Events from './events';");
      return output;
    }

    static string[] KnownBases = new string[]
    {
      "DataTransferObject",
      //"AggregateStateDataTransferObject",
      //"EntityDataTransferObject",
      //"CommandDataTransferObject",
      "ValueObject"
    };

    string RemoveArraySuffix(string input) => input.Replace("[]", "");

    static string[] AnylikeTypes = new string[]
    {
      "any",
      "Any",
      "Object",
      "JObject",
      "JToken",
      "JSchema",
      "JsonSchema",
      "ExpandoObject"
    };

    static string[] StringlikeTypes = new string[]
    {
      "Version",
      "Uri",
      "TimeOnly"
    };

    bool ShouldBeImported(Type importedType, string originName, string originBaseName, ITypeCollection originTypeArguments) =>
      (!importedType.IsPrimitive || importedType.IsEnum) &&
      RemoveArraySuffix(importedType.Name) != originName &&
      RemoveArraySuffix(importedType.Name) != originBaseName &&
      RemoveArraySuffix(importedType.Name) != "any" &&
      importedType.OriginalName != "JsonPatchDocument" &&
      !StringlikeTypes.Contains(importedType.OriginalName) && 
      !AnylikeTypes.Contains(importedType.OriginalName) && 
      !originTypeArguments.Select(t => t.Name).Contains(importedType.Name) &&
      !importedType.Attributes.Any(a => a.Name == "JsonIgnore");

    string GetRelativePath(Type importedType, string originFullname)
    {
      if (
        !importedType.IsEnum && (
          (IsModel(importedType.FullName) && IsModel(originFullname)) ||
          (IsEvent(importedType.FullName) && IsEvent(originFullname)) ||
          (IsCommand(importedType.FullName) && IsCommand(originFullname)) ||
          ((IsController(importedType.FullName) || IsHub(importedType.FullName)) && (IsController(originFullname) || IsHub(originFullname)))
        )
      )
      {
        return "./";
      }
      string prefix = IsController(originFullname) || IsHub(originFullname) ? "./" : "../";
      if (importedType.IsEnum) return $"{prefix}enums/";
      if (IsModel(importedType.FullName)) return $"{prefix}models/";
      if (IsEvent(importedType.FullName)) return $"{prefix}events/";
      if (IsCommand(importedType.FullName)) return $"{prefix}commands/";
      return "..UNKNONW..";
    }

    static Dictionary<string, string> KnownExternalTypes = new Dictionary<string, string>()
    {
    };

    string BuildImport(Type importedType, string originFullname)
    {
      string name = Regex.Replace(RemoveArraySuffix(importedType.Name).Split('<').ElementAt(0), @"{ \[key: string\]: (\w*); }", "$1", RegexOptions.Compiled);
      if (KnownExternalTypes.Keys.Contains(name))
      {
        return $"import {{ {name} }} from '{KnownExternalTypes[name]}';";
      }
      if (ExtendedModels.Contains(name))
      {
        name += "Base";
      }
      return $"import {{ {RemoveArraySuffix(name)} }} from '{GetRelativePath(importedType, originFullname)}{ToKebabCase(RemoveArraySuffix(name))}';";
    }

    string BuildModelImports(string fullName, string name, string baseName, IPropertyCollection properties, ITypeCollection typeArguments)
    {
      string output = "";
      if (ExtendedModels.Contains(baseName))
      {        
        baseName += "Base";
      }
      if (baseName == "Measurement")
      {
        output += Indent(0, "import { UnitOfMeasurementType } from '../enums';");
        output += Indent(0, "import { UnitOfMeasurement } from './unit-of-measurement';");
        output += Indent(0, "import { Measurement } from '../measurement';");
      }
      else if (!string.IsNullOrWhiteSpace(baseName) && !KnownBases.Contains(baseName))
      {
        output += Indent(0, $"import {{ {baseName} }} from './{ToKebabCase(baseName)}';");
      }
      else if (!string.IsNullOrWhiteSpace(baseName) && baseName == "EntityDataTransferObject")
      {
        output += Indent(0, $"import {{ EntityDto }} from '@neuroglia/angular-rest-core';");
      }
      else
      {
        output += Indent(0, $"import {{ ModelConstructor }} from '@neuroglia/common';");
      }
      output += properties
        .Where(p => !p.Attributes.Any(a => a.Name == "JsonIgnore"))
        .SelectMany(p => !p.Type.IsGeneric ?
          new[] { p.Type } as IEnumerable<Type> :
            p.Type.FullName.Contains("System") ?
            p.Type.TypeArguments :
            (new[] { p.Type } as IEnumerable<Type>).Concat(p.Type.TypeArguments)
        )
        .Where(t => ShouldBeImported(t, name, baseName, typeArguments))
        .Select(t => BuildImport(t, fullName))
        .Distinct()
        .Aggregate("", (all, import) => Indent(0, $"{all}{import}"))
        .TrimStart();
      return output;
    }

    string BuildImports(Record record)
    {
      if (IsController(record.FullName)) {
        return BuildServiceImports();
      }
      return BuildModelImports(record.FullName, record.Name, record.BaseRecord?.Name ?? "", record.Properties, record.TypeArguments);
    }

    string BuildImports(Class clazz)
    {
      if (IsController(clazz.FullName)) {
        return BuildServiceImports();
      }
      return BuildModelImports(clazz.FullName, clazz.Name, clazz.BaseClass?.Name ?? "", clazz.Properties, clazz.TypeArguments);
    }

    static List<KeyValuePair<string, string>> CommentFormattingPatterns = new List<KeyValuePair<string, string>>() {
        new KeyValuePair<string, string>(@"<see cref=""T:.*.Controller"" />" , "service"),
        new KeyValuePair<string, string>("controller" , "service"),
        new KeyValuePair<string, string>(@"<see cref=""T:.*.\.Models\.(.*)"" \/>" , "@see {@link Models.$}")
    };

    string FormatComment(string comment) => CommentFormattingPatterns.Aggregate(
      comment ?? "",
      (output, pattern) => Regex.Replace(output, pattern.Key, pattern.Value)
    );

    string BuildComments(DocComment docComment, bool asBlock, int depth)
    {
      if (docComment?.Summary == null)
      {
        return "";
      }
      string formattedComment = FormatComment(docComment.Summary);
      string output = "";
      if (asBlock) {
        output += Indent(depth, $"/**");
        output += Indent(depth, $" * {formattedComment}");
        output += Indent(depth, $" */");
      }
      else {
        output += Indent(depth, $"/** {formattedComment} */");
      }
      return output;
    }

    string BuildServiceDeclaration(string serviceName)
    {
      string output = "";
      output += Indent(0, $"@Injectable({{");
      output += Indent(1, $"providedIn: 'root'");
      output += Indent(0, $"}})");
      output += Indent(0, $"export class {serviceName}", false);
      return output;
    }

    string GetClassName(string name, ITypeCollection typeArguments)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return name;
      }
      if (ExtendedModels.Contains(name))
      {
        name += "Base";
      }
      if (typeArguments != null && typeArguments.Any()) {
        name += $"<{String.Join(",",typeArguments.Select(t => t.Name))}>";
      }
      return name;
    }

    string BuildModelDeclaration(string name, string baseName, ITypeCollection typeArguments, ITypeCollection baseTypeArguments)
    {
      name = GetClassName(name, typeArguments);
      baseName = GetClassName(baseName, baseTypeArguments);
      if (!string.IsNullOrWhiteSpace(baseName) && !KnownBases.Contains(baseName))
      {
        return Indent(0, $"export class {name} extends {baseName}", false);
      }
      return Indent(0, $"export class {name} extends ModelConstructor", false);
    }

    string BuildDeclaration(Record record)
    {
      string output = BuildComments(record.DocComment, true, 0);
      if (IsController(record.FullName))
      {
        return output + BuildServiceDeclaration(GetServiceClassName(record.Name));
      }
      return output + BuildModelDeclaration(record.Name, record.BaseRecord?.Name ?? "", record.TypeArguments, record.BaseRecord?.TypeArguments);
    }

    string BuildDeclaration(Class clazz)
    {
      string output = BuildComments(clazz.DocComment, true, 0);
      if (IsController(clazz.FullName))
      {
        return output + BuildServiceDeclaration(GetServiceClassName(clazz.Name));
      }
      return output + BuildModelDeclaration(clazz.Name, clazz.BaseClass?.Name ?? "", clazz.TypeArguments, clazz.BaseClass?.TypeArguments);
    }

    string BuildServiceProperties()
    {
      string output = NEW_LINE;
      output += Indent(1, "protected apiUrl: string = inject(REST_API_URL_TOKEN);");
      output += Indent(1, "protected errorObserver = inject(HttpErrorObserverService);");
      output += Indent(1, "protected namedLoggingServiceFactory = inject(NamedLoggingServiceFactory);");
      output += Indent(1, "protected http = inject(HttpClient);");
      output += Indent(1, "protected urlHelperService = inject(UrlHelperService);");
      output += Indent(1, "protected logger: ILogger;");
      return output;
    }

    string GetPropertyName(Property property)
    {
      Attribute jsonPropertyAttribute = property.Attributes.FirstOrDefault(a => a.Name == "JsonProperty");
      if(jsonPropertyAttribute != null && jsonPropertyAttribute.Arguments.Any())
      {
        return jsonPropertyAttribute.Arguments.First().Value.ToString();
      }
      return property.name;
    }

    string GetType(Type type)
    {
      string typeName = type.Name;
      if (AnylikeTypes.Contains(type.OriginalName))
      {
        typeName = "any";
      }
      else if (StringlikeTypes.Contains(type.OriginalName))
      {
        typeName = "string";
      }
      else if (type.OriginalName == "JsonPatchDocument")
      {
        typeName = "any[]";
      }
      else if (type.OriginalName == "NameValueCollection")
      {
        typeName = $"Record<string, {type.TypeArguments[0].Name}>";
      }

      if (type.IsEnum || typeName == "Date")
      {
        typeName += " | string";
      }

      return type;
    }

    string BuildPropertyDeclaration(Property property)
    {
      string comment = BuildComments(property.DocComment, false, 1);
      string name = GetPropertyName(property);
      if (property.Type.IsNullable)
      {
        name += "?";
      }
      else {
        name += "!";
      }
      string type = GetType(property.Type);
      return comment + Indent(1, $"{name}: {type};"); 
    }

    string BuildModelProperties(IPropertyCollection properties) => !properties.Any() ? "" : properties
        .Where(p => !p.Attributes.Any(a => a.Name == "JsonIgnore"))
        .Select(p => BuildPropertyDeclaration(p))
        .Distinct()
        .Aggregate(NEW_LINE, (all, prop) => $"{all}{prop}");

    string BuildProperties(Record record)
    {
      if (IsController(record.FullName))
      {
        return BuildServiceProperties();
      }
      return BuildModelProperties(record.Properties);
    }

    string BuildProperties(Class clazz)
    {
      if (IsController(clazz.FullName))
      {
        return BuildServiceProperties();
      }
      return BuildModelProperties(clazz.Properties);
    }

    string BuildServiceConstructor(string serviceName)
    {
      string output = NEW_LINE;
      output += Indent(1, "constructor() {");
      output += Indent(2, $"this.logger = this.namedLoggingServiceFactory.create('{serviceName}');");
      output += Indent(1, "}");
      return output;
    }

    string BuildModelConstructor(string name, IPropertyCollection properties, ITypeCollection typeArguments)
    {
      IEnumerable<Property> complexProperties = properties.Where(p =>
        p.Type.IsEnumerable ||
        (
          !p.Type.IsPrimitive &&
          !p.Type.IsEnum &&
          !typeArguments.Select(t => t.Name).Contains(p.Type.Name) &&
          !StringlikeTypes.Contains(p.Type.OriginalName) && 
          !AnylikeTypes.Contains(p.Type.OriginalName) &&
          !p.Attributes.Any(a => a.Name == "JsonIgnore")
        )
      );
      string output = "";
      if (
        name == "Capacity" ||
        name == "Energy" ||
        name == "Length" ||
        name == "Mass" ||
        name == "Surface" ||
        name == "Temperature" ||
        name == "Unit" ||
        name == "Volume"
      )
      {
        output += Indent(1, "constructor(value?: number, unit?: UnitOfMeasurement);");
        output += Indent(1, $"constructor(model?: Partial<{GetClassName(name, typeArguments)}>);");
        output += Indent(1, $"constructor(...args: Array<number | UnitOfMeasurement | Partial<{GetClassName(name, typeArguments)}> | undefined>) {{");
        output += Indent(2, $"let model: Partial<{GetClassName(name, typeArguments)}> = {{}};");
        output += Indent(2, "if (args?.length === 1) {");
        output += Indent(3, $"model = args[0] as Partial<{GetClassName(name, typeArguments)}>;");
        output += Indent(2, "}");
        output += Indent(2, "else if (args?.length == 2) {");
        output += Indent(3, "const [value, unit] = args as [number, UnitOfMeasurement];");
        output += Indent(3, "model = { value, unit };");
        output += Indent(2, "}");
        output += Indent(2, "super(model);");
        output += Indent(2, $"if (!this.unit.type) this.unit.type = UnitOfMeasurementType.{name};");
        output += Indent(2, $"if (this.unit.type !== UnitOfMeasurementType.{name}) throw new Error(`Invalid unit of measurement type '${{this.unit.type}}', expected '${{UnitOfMeasurementType.{name}}}'.`);");
      }
      else {
        output += Indent(1, $"constructor(model?: Partial<{GetClassName(name, typeArguments)}>) {{");
        output += Indent(2, "super(model);");
      }
      if (complexProperties.Any())
      {
        complexProperties.ToList().ForEach(property =>
        {
          string propertyName = GetPropertyName(property);
          if (property.Type.IsDictionary || property.Type.FullName.Contains("NameValueCollection<"))
          {
            int valueTypeArgumentIndex = property.Type.IsDictionary ? 1 : 0;
            Type valueType = property.Type.TypeArguments[valueTypeArgumentIndex];
            if (valueType.IsPrimitive)
            {
              output += Indent(2, $"this.{propertyName} = {{ ...(model?.{propertyName}||{{}}) }};");
            }
            if (GetType(valueType) == "any")
            {
              output += Indent(2, $"this.{propertyName} = Object.entries(model?.{propertyName}||{{}}).reduce((acc, [key, value]: [string, any]) => {{ acc[key] = {{ ...value }}; return acc; }}, {{}} as {GetType(property.Type)});");
            }
            else
            {
              output += Indent(2, $"this.{propertyName} = Object.entries(model?.{propertyName}||{{}}).reduce((acc, [key, value]: [string, {valueType.Name}]) => {{ acc[key] = new {RemoveArraySuffix(valueType.Name)}(value); return acc; }}, {{}} as {GetType(property.Type)});");
            }
          }
          else if (property.Type.IsEnumerable || property.Type.OriginalName == "JsonPatchDocument")
          {
            if (property.Type.IsPrimitive || (property.Type.IsGeneric && property.Type.TypeArguments[0].IsPrimitive))
            {
              output += Indent(2, $"this.{propertyName} = [ ...(model?.{propertyName}||[]) ];");
            }
            else if (GetType(property.Type) == "any" || property.Type.OriginalName == "JsonPatchDocument")
            {
              output += Indent(2, $"this.{propertyName} = (model?.{propertyName}||[]).map((value: any) => ({{ ...value }}));");
            }
            else
            {
              output += Indent(2, $"this.{propertyName} = (model?.{propertyName}||[]).map((value: {GetType(property.Type)}) => new {RemoveArraySuffix(property.Type.Name)}(value));");
            }
          }
          else {
            if (GetType(property.Type) == "any") {
              output += Indent(2, $"this.{propertyName} = {{ ...(model?.{propertyName}||{{}}) }};");
            }
            else {
              output += Indent(2, $"this.{propertyName} = new {RemoveArraySuffix(property.Type.Name)}(model?.{propertyName}||{{}});");
            }
          }
        });
      }
      output += Indent(1, "}");
      return output;
    }

    string BuildConstructor(Record record)
    {
      if (IsController(record.FullName))
      {
        return BuildServiceConstructor(GetServiceClassName(record.Name));
      }
      return BuildModelConstructor(record.Name, record.Properties, record.TypeArguments);
    }

    string BuildConstructor(Class clazz)
    {
      if (IsController(clazz.FullName))
      {
        return BuildServiceConstructor(GetServiceClassName(clazz.Name));
      }
      return BuildModelConstructor(clazz.Name, clazz.Properties, clazz.TypeArguments);
    }

    string TypeScriptEnumValue(EnumValue enumeration)
    {
        dynamic attribute = enumeration.Attributes.Where(a => a.Name == "EnumMember").FirstOrDefault();
        if(attribute == null)
          return enumeration.name;
        else
          return attribute.Value.Split('=')[1].Replace("\"", "").Trim();
    }

}$Records(record => IsModel(record.FullName))[$BuildExportsIndex$BuildImports
$BuildDeclaration {
$BuildProperties
$BuildConstructor
}]$Classes(clazz => IsModel(clazz.FullName))[$BuildExportsIndex$BuildImports
$BuildDeclaration {
$BuildProperties
$BuildConstructor
}]$Enums(*)[$BuildExportsIndex export enum $Name {
  $Values[$Name = '$TypeScriptEnumValue'][,
  ]
}]
