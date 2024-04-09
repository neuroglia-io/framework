// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Lambda2Js;
using Neuroglia;
using Neuroglia.Collections;
using System.Collections;
using System.Linq.Expressions;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines custom <see cref="JavascriptConversionExtension"/>s used to translate projections to EventStore
/// </summary>
public static class EsdbJavaScriptConversion
{

    /// <summary>
    /// Gets an array of the <see cref="JavascriptConversionExtension"/>s used to translate projections to EventStore 
    /// </summary>
    public static readonly JavascriptConversionExtension[] Extensions = [new NullCheckConversionExtension(), new BinaryExpressionConversionExtension(), new ObjectConversionExtension(), new ArrayInitializerConversionExtension(), new ListConversionExtension(), new CollectionConversionExtension(), new DictionaryConversionExtension(), new EventRecordConversionExtension(), new ProjectionConversionExtension()];

    class NullCheckConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not BinaryExpression binary || (binary.NodeType != ExpressionType.Equal && binary.NodeType != ExpressionType.NotEqual)) return;
            var leftNull = binary.Left is ConstantExpression left && left.Value == null;
            var rightNull = binary.Right is ConstantExpression right && right.Value == null;
            if ((!leftNull && !rightNull) || (leftNull && rightNull)) return;
            context.PreventDefault();
            var compareTo = leftNull ? binary.Right : binary.Left;
            var op = binary.NodeType switch
            {
                ExpressionType.Equal => "===",
                ExpressionType.NotEqual => "!==",
                _ => throw new NotSupportedException()
            };
            context.Write(compareTo);
            context.Write($" {op} undefined && ");
            context.Write(compareTo);
            context.Write($" {op} null");
        }

    }

    class BinaryExpressionConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not BinaryExpression binary) return;
            switch (binary.NodeType)
            {
                case ExpressionType.Add:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" + ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.Subtract:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" - ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.Multiply:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" * ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.Divide:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" / ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.And:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" & ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.AndAlso:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" && ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.Or:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" | ");
                    context.Write(binary.Right);
                    break;
                case ExpressionType.OrElse:
                    context.PreventDefault();
                    context.Write(binary.Left);
                    context.Write(" || ");
                    context.Write(binary.Right);
                    break;
            }
        }

    }

    class ObjectConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MethodCallExpression call || call.Method.DeclaringType != typeof(ObjectExtensions)) return;
            context.PreventDefault();
            switch (call.Method.Name)
            {
                case nameof(ObjectExtensions.GetProperty):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty))
                                context.WriteNode(call.Arguments.First());
                            context.Write($".{call.Arguments.Last().ToString()[1..^1]}");
                        }
                        break;
                    }
            }
        }

    }

    class ArrayInitializerConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not NewExpression constructor || !constructor.Type.IsArray && !typeof(IEnumerable).IsAssignableFrom(constructor.Type)) return;
            context.PreventDefault();
            context.Write("[]");
        }

    }
   
    class ListConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MethodCallExpression call || !typeof(ICollection).IsAssignableFrom(call.Method.DeclaringType)) return;
            context.PreventDefault();
            switch (call.Method.Name)
            {
                case nameof(ICollection<object>.Contains):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty))
                                context.Write($"{call.Object}.includes");
                            context.WriteManyIsolated('(', ')', ',', call.Arguments);
                        }
                        break;
                    }
                case nameof(ICollection<object>.Add):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty))
                                context.Write($"{call.Object}.push");
                            context.WriteManyIsolated('(', ')', ',', call.Arguments);
                        }
                        break;
                    }

            }
        }

    }

    class CollectionConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MethodCallExpression call || call.Method.DeclaringType != typeof(ICollectionExtensions)) return;
            context.PreventDefault();
            switch (call.Method.Name)
            {
                case nameof(ICollectionExtensions.Contains):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty))context.WriteNode(call.Arguments.First());
                            context.Write(".includes(");
                            context.WriteNode(call.Arguments.Last());
                            context.Write(")");
                        }
                        break;
                    }
                case nameof(ICollectionExtensions.Add):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty)) context.WriteNode(call.Arguments.First());
                            context.Write(".push(");
                            context.WriteNode(call.Arguments.Last());
                            context.Write(")");
                        }
                        break;
                    }

            }
        }

    }

    class DictionaryConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MethodCallExpression call || call.Method.DeclaringType != typeof(IDictionaryExtensions)) return;
            context.PreventDefault();
            switch (call.Method.Name)
            {
                case nameof(IDictionaryExtensions.Get):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.IndexerProperty))
                                context.WriteNode(call.Arguments.First());
                            context.Write($".{call.Arguments.Last().ToString()[1..^1]}");
                        }
                        return;
                    }
            }
        }

    }

    class EventRecordConversionExtension
       : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MemberExpression member || member.Member.DeclaringType != typeof(IEventRecord)) return;
            context.PreventDefault();
            using (context.Operation(JavascriptOperationTypes.IndexerProperty))
            {
                context.WriteNode(member.Expression);
                context.Write($".{member.Member.Name.ToCamelCase()}");
            }
        }

    }

    class ProjectionConversionExtension
        : JavascriptConversionExtension
    {

        /// <inheritdoc/>
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is not MethodCallExpression call || call.Method.DeclaringType != typeof(Projection)) return;
            context.PreventDefault();
            switch (call.Method.Name)
            {
                case nameof(Projection.LinkEventTo):
                    {
                        using (context.Operation(JavascriptOperationTypes.Call))
                        {
                            using (context.Operation(JavascriptOperationTypes.Call))
                            {
                                using (context.Operation(JavascriptOperationTypes.IndexerProperty))context.Write("linkTo");
                                context.WriteManyIsolated('(', ')', ',', call.Arguments);
                            }
                        }
                        break;
                    }
            }
        }

    }

}
