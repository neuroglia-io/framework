using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia;
using Neuroglia.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.JsonPatch
{

    public class PatchOperationAttribute
        : Attribute
    {

        public PatchOperationAttribute(string type, string path)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            this.Type = type;
            this.Path = path;
        }

        public PatchOperationAttribute(OperationType type, string path)
            : this(type.ToString().ToLower(), path)
        {

        }

        public virtual string Type { get; }

        public virtual string Path { get; }

    }

    public class AggregateObjectProvider
        : IObjectAdapter
    {

    }

    public interface IAggregatePatcherFactory
    {

        IAggregatePatcher CreatePatcher();

    }

    public interface IAggregatePatcher
    {

        Task ApplyPatchToAsync<TAggregate>(JsonPatchDocument patch, TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class;

    }

    public class AggregatePatcher
        : IAggregatePatcher
    {

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected virtual IServiceProvider ServiceProvider { get; }

        protected virtual ITypePatchDescriptorFactory TypePatchDescriptorFactory { get; }

        /// <inheritdoc/>
        public virtual async Task ApplyPatchToAsync<TAggregate>(JsonPatchDocument patch, TAggregate aggregate, CancellationToken cancellationToken = default) 
            where TAggregate : class
        {
            ITypePatchDescriptor typePatchDescriptor = this.TypePatchDescriptorFactory.Create(typeof(TAggregate));
            foreach(Operation operation in patch.Operations)
            {
                if (!typePatchDescriptor.TryGetOperation(operation.op, operation.path, out PatchOperationDescriptor operationDescriptor))
                    throw new NotSupportedException(); //todo: replace with meaningfull exception
                object parameter = operation.value;
                if (operationDescriptor.Parameter.IsCommandResult)
                {
                    IOperationResult operationResult = ;
                }
                if (operationDescriptor.Parameter.IsReference)
                {
                    IRepository repository = this.ServiceProvider.GetRequiredService(typeof(IRepository<>).MakeGenericType(operationDescriptor.Parameter.Type));
                    parameter = await repository.FindAsync(operation.value, cancellationToken);
                }  
            }
        }

    }

    public interface ITypePatchDescriptorFactory
    {

        ITypePatchDescriptor Create(Type type);

        ITypePatchDescriptor Create<T>();

    }

    public interface ITypePatchDescriptor
    {

        Type Type { get; }

        IReadOnlyCollection<PatchOperationDescriptor> Operations { get; }

        bool TryGetOperation(string type, string path, out PatchOperationDescriptor operation);

    }

    public class TypePatchDescriptor
        : ITypePatchDescriptor
    {

        public virtual Type Type { get; }

        public virtual IReadOnlyCollection<PatchOperationDescriptor> Operations { get; }

    }

    public class PatchOperationDescriptor
    {

        public PatchOperationDescriptor(string type, string path, MethodInfo method)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            this.Type = type;
            this.Path = path;
            this.Method = method;
        }

        public virtual string Type { get; }

        public virtual string Path { get; }

        public virtual MethodInfo Method { get; }

        public static PatchOperationDescriptor CreateFor(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (!method.TryGetCustomAttribute(out PatchOperationAttribute patchOperationAttribute))
                throw new NotSupportedException($"Only methods marked with the '{nameof(PatchOperationAttribute)}' are supported");
            return new PatchOperationDescriptor(patchOperationAttribute.Type, patchOperationAttribute.Path, method);
        }

    }

}
