using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;

namespace karu.bsonly.Generator
{
  // NEXT STEP
  //  - write seralize function by hand
  //  - test serialization
  //  - make generator generate the proper functions
  // -- see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to

  [Generator(LanguageNames.CSharp)]
  public class ApiGenerator : IIncrementalGenerator
  {
    const string API_SERIALIZATION_ATTR_NAME = "karu.bsonly.Generator.ApiGeneratorAttribute";
    const string FAILED_NAME = "FAILED";

    // private ImmutableArray<Model> _models;
    // private record Model(string Namespace, string ClassName);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      // see https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.cookbook.md

      var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
          fullyQualifiedMetadataName: API_SERIALIZATION_ATTR_NAME,
          predicate: static (syntaxNode, cancellationToken) =>
          {
            return syntaxNode is ClassDeclarationSyntax class_node && class_node.Modifiers.Any(SyntaxKind.PartialKeyword);
          },
          transform: static (context, cancellationToken) =>
          {
            var syntax_node = context.TargetSymbol;
            // https://sharplab.io/
            var class_syntax = context.TargetNode as ClassDeclarationSyntax;

            var properties = GetPublicProperties(class_syntax!, context.SemanticModel);
            var props = new EquatableArray<Property>(properties);
            // var foo = containingClass.fullyQualifiedMetadataName
            //var props = new EquatableArray<(string type, string name)>(new (string, string)[] { ("foo", "bar"), ("prop2", "prop2__") });
            return new Model
            {
              // Note: this is a simplified example. You will also need to handle the case where the type is in a global namespace, nested, etc.
              Namespace = GetNamespace(class_syntax!),
              ClassName = syntax_node.Name,
              Properties = props,
              WriteEncode = true,
              WriteDecode = true
            };
          }
      );

      context.RegisterSourceOutput(pipeline, static (context, model) =>
      {
        //set the location we want to save the log file
        StringBuilder builder = new();
        using System.IO.StringWriter writer = new(builder, CultureInfo.InvariantCulture);
        using (var sourceWriter = new System.CodeDom.Compiler.IndentedTextWriter(writer, "  "))
        {
          SerializationFunctionWriter.WriteIntro(model, sourceWriter);

          SerializationFunctionWriter.WriteSerializeToBson(model, sourceWriter);
          SerializationFunctionWriter.WriteSerializeFromBson(model, sourceWriter);
          SerializationFunctionWriter.WriteClassEnd(model, sourceWriter);

          SerializationFunctionWriter.WriteOuttro(model, sourceWriter);

          context.AddSource($"{model.ClassName}.g.cs", builder.ToString());
        }
      });
    }

    static Property[] GetPublicProperties(ClassDeclarationSyntax classNode, SemanticModel semantic_model)
    {
      var properties = new System.Collections.Generic.List<Property>();
      for (var idx = 0; idx < classNode.Members.Count; ++idx)
      {
        var member = classNode.Members[idx];
        if (member.Kind() == SyntaxKind.FieldDeclaration && member.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
          var field = member as FieldDeclarationSyntax;

          var member_attributes = new AttrProperty(string.Empty, string.Empty, -1);
          if (field!.AttributeLists.Count > 0)
          {
            for (var attr_list_idx = 0; attr_list_idx < field!.AttributeLists.Count; ++attr_list_idx)
            {
              AttributeListSyntax? attr_list = field!.AttributeLists[attr_list_idx];
              for (var attr_idx = 0; attr_idx < attr_list.Attributes.Count; ++attr_idx)
              {
                var attr = attr_list.Attributes[attr_idx];
                var name = attr!.Name.ToString();
                var attribute_data = GeneratorAttributeData.SupportedAttributes(name);
                var prop = GetMemberAttributeProperties(attribute_data, name, attr!);
                if (prop.BsonType != string.Empty)
                  member_attributes.BsonType = prop.BsonType;
                if (prop.BsonName != string.Empty)
                  member_attributes.BsonName = prop.BsonName;
                if (prop.Order != -1 && member_attributes.Order != Property.IGNORE_VALUE)
                  member_attributes.Order = prop.Order;

                // member_attributes.Debug += ">>" + prop.Debug + "<<";
              }
            }
          }

          var type = field!.Declaration.Type.ToString();
          for (var var_idx = 0; var_idx < field.Declaration.Variables.Count; ++var_idx)
          {
            var variable = field.Declaration.Variables[var_idx];
            var name = variable.Identifier.ToString();

            if (member_attributes.BsonName == string.Empty)
              member_attributes.BsonName = "\"" + name + "\"";
            if (member_attributes.BsonType == string.Empty)
              member_attributes.BsonType = "\"" + type + "\"";
            properties.Add(new Property(type, name, member_attributes.BsonName, member_attributes.BsonType, member_attributes.Order/*, member_attributes.Debug*/));
          }
        }
      }

      return SortByOrder(properties);
    }

    private static Property[] SortByOrder(System.Collections.Generic.List<Property> properties)
    {
      // order 2, 4 are set
      //     0, 1->4, 2, 3 -> 2, 4, 5
      //     0, 2, 3, 4, 1, 5
      // idx 0, 1, 2, 3, 4, 5

      var num_of_props = properties.Count;
      var prop_array = new Property[num_of_props];
      for (var idx = 0; idx < num_of_props; ++idx)
      {
        var found_idx = properties.FindIndex(p => p.Order == idx);
        if (found_idx == -1)
        {
          found_idx = properties.FindIndex(p => p.Order == -1);
          if (found_idx != -1)
          {
            prop_array[idx] = properties[found_idx];
            properties.RemoveAt(found_idx);
          }
          else
          {
            prop_array[idx] = properties[0];
            properties.RemoveAt(0);
          }
        }
        else
        {
          prop_array[idx] = properties[found_idx];
          properties.RemoveAt(found_idx);
        }
      }
      return prop_array;
    }

    static AttrProperty GetMemberAttributeProperties(Attributes attribute, string debug_value, AttributeSyntax attribute_data)
    {
      if (attribute == Attributes.ApiIgnore)
        return new(debug_value, debug_value, Property.IGNORE_VALUE);

      if (attribute == Attributes.ApiElement)
        return HandleApiElement(attribute_data);

      if (attribute == Attributes.ApiOrder)
        return HandleApiOrder(attribute_data);

      if (attribute == Attributes.ApiType)
        return HandleApiType(attribute_data);

      if (attribute == Attributes.ApiName)
        return HandleApiName(attribute_data);

      if (attribute == Attributes.ApiUtf8)
        return new(string.Empty, "Utf8", Property.DEAULT_ORDER_VALUE);


      return new AttrProperty(string.Empty, $"unknown attribute {debug_value}", -1);
    }

    static (string name, string value) HandleAttributeArgument(AttributeArgumentSyntax argument)
    {
      var name_colon = argument.NameColon;
      if (name_colon != null)
      {
        var syntax = argument.Expression as LiteralExpressionSyntax;
        if (syntax != null)
          return (name_colon.Name.ToString(), syntax.Token.ToString());

        return ("FAILED", argument.ToString());
      }

      var name_equals = argument.NameEquals;
      if (name_equals != null)
      {
        var syntax = argument.Expression as LiteralExpressionSyntax;
        if (syntax != null)
          return (name_equals.Name.ToString(), syntax.Token.ToString());

        return ("FAILED", argument.ToString());
      }

      var expression_syntax = argument.Expression as LiteralExpressionSyntax;
      if (expression_syntax != null)
        return (string.Empty, expression_syntax.Token.ToString());

      return ("FAILED", argument.ToString());
    }

    static AttrProperty HandleApiName(AttributeSyntax attribute_data)
    {
      if (attribute_data.ArgumentList != null)
      {
        var args = attribute_data.ArgumentList.Arguments;
        var (name, value) = HandleAttributeArgument(args[0]);
        if (name == "name" || name == string.Empty)
          return new AttrProperty(value, string.Empty, Property.DEAULT_ORDER_VALUE);
      }
      var prop = new AttrProperty(FAILED_NAME, string.Empty, Property.DEAULT_ORDER_VALUE);
      // prop.Debug = $"FAILED ApiName >{value}<";
      return prop;
    }

    static AttrProperty HandleApiType(AttributeSyntax attribute_data)
    {
      if (attribute_data.ArgumentList != null)
      {
        var args = attribute_data.ArgumentList.Arguments;
        var (name, value) = HandleAttributeArgument(args[0]);
        if (name == "name" || name == string.Empty)
          return new AttrProperty(value, string.Empty, Property.DEAULT_ORDER_VALUE);
      }
      var prop = new AttrProperty(FAILED_NAME, string.Empty, Property.DEAULT_ORDER_VALUE);
      // prop.Debug = $"FAILED ApiType >{value}<";
      return prop;
    }

    static AttrProperty HandleApiOrder(AttributeSyntax attribute_data)
    {
      if (attribute_data.ArgumentList != null)
      {
        var args = attribute_data.ArgumentList.Arguments;
        var (name, value) = HandleAttributeArgument(args[0]);
        if (name == "order" || name == string.Empty)
          if (int.TryParse(value, out var order))
            return new AttrProperty(string.Empty, string.Empty, order);
      }
      var prop = new AttrProperty(FAILED_NAME, string.Empty, Property.DEAULT_ORDER_VALUE);
      // prop.Debug = $"FAILED ApiOrder >{value}<";
      return prop;
    }


    static AttrProperty HandleApiElement(AttributeSyntax attribute_data)
    {
      var properties = new AttrProperty(string.Empty, string.Empty, -1);

      if (attribute_data.ArgumentList != null)
      {
        var arguments = attribute_data.ArgumentList.Arguments;
        for (var arg_idx = 0; arg_idx < arguments.Count; ++arg_idx)
        {
          var (name, value) = HandleAttributeArgument(arguments[arg_idx]);
          if (name != FAILED_NAME)
          {
            if (name == "name")
            {
              properties.BsonName = value;
              continue;
            }
            else if (name == "type")
            {
              properties.BsonType = value;
              continue;
            }
            else if (name == "order")
            {
              if (int.TryParse(value, out var order))
              {
                properties.Order = order;
                continue;
              }
            }

            if (arg_idx == 0)
            {
              properties.BsonName = value;
              continue;
            }
            else if (arg_idx == 1 && arguments.Count == 3)
            {
              properties.BsonType = value;
              continue;
            }
            else if (arg_idx == 1 && arguments.Count == 2
                    || arg_idx == 2 && arguments.Count == 3)
            {
              if (int.TryParse(value, out var order))
              {
                properties.Order = order;
                continue;
              }
            }
          }

          // properties.Debug += "FAILED: {value}";
        }
      }
      return properties;
    }

    // determine the namespace the class/enum/struct is declared in, if any
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
      // If we don't have a namespace at all we'll return an empty string
      // This accounts for the "default namespace" case
      string nameSpace = string.Empty;

      // Get the containing syntax node for the type declaration
      // (could be a nested type, for example)
      SyntaxNode? potentialNamespaceParent = syntax.Parent;

      // Keep moving "out" of nested classes etc until we get to a namespace
      // or until we run out of parents
      while (potentialNamespaceParent != null &&
              potentialNamespaceParent is not NamespaceDeclarationSyntax
              && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
      {
        potentialNamespaceParent = potentialNamespaceParent.Parent;
      }

      // Build up the final namespace by looping until we no longer have a namespace declaration
      if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
      {
        // We have a namespace. Use that as the type
        nameSpace = namespaceParent.Name.ToString();

        // Keep moving "out" of the namespace declarations until we 
        // run out of nested namespace declarations
        while (true)
        {
          if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
          {
            break;
          }

          // Add the outer namespace as a prefix to the final namespace
          nameSpace = $"{namespaceParent.Name}.{nameSpace}";
          namespaceParent = parent;
        }
      }

      // return the final namespace
      return nameSpace;
    }

    private static int ComparePropertyByOrder(Property x, Property y)
    {
      if (x == null)
      {
        if (y == null)
        {
          // If x is null and y is null, they're
          // equal.
          return 0;
        }
        else
        {
          // If x is null and y is not null, y
          // is greater.
          return -1;
        }
      }
      else
      {
        // If x is not null...
        //
        if (y == null)
        // ...and y is null, x is greater.
        {
          return 1;
        }
        else
        {
          return x.Order.CompareTo(y.Order);
        }
      }
    }
  }
}

#region Copyright notice and license

// Copyright 2025 The bsonly Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion