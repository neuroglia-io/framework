/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Flux.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IStoreFactory"/> interface
    /// </summary>
    public class StoreFactory
        : IStoreFactory
    {

        private static MethodInfo AddFeatureMethod = typeof(IStoreExtensions).GetMethods()
            .First(m => m.Name == nameof(IStoreExtensions.AddFeature) && m.GetParameters().Length == 2);

        /// <summary>
        /// Initializes a new <see cref="StoreFactory"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="fluxOptions">The current <see cref="Configuration.FluxOptions"/></param>
        public StoreFactory(IServiceProvider serviceProvider, IOptions<FluxOptions> fluxOptions)
        {
            this.ServiceProvider = serviceProvider;
            this.FluxOptions = fluxOptions.Value;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the current <see cref="Configuration.FluxOptions"/>
        /// </summary>
        protected FluxOptions FluxOptions { get; }

        /// <inheritdoc/>
        public virtual IStore CreateStore()
        {
            var store = (IStore)ActivatorUtilities.CreateInstance(this.ServiceProvider, this.FluxOptions.StoreType);
            var reducersPerState = new Dictionary<Type, List<IReducer>>();
            foreach (var reducerDeclaringType in TypeCacheUtil.FindFilteredTypes("nflux-reducers", 
                t => t.GetMethods().Any(m => m.TryGetCustomAttribute<ReducerAttribute>(out _)), this.FluxOptions.AssembliesToScan?.ToArray()))
            {
                foreach(var reducerMethod in reducerDeclaringType.GetMethods()
                    .Where(m => m.TryGetCustomAttribute<ReducerAttribute>(out _)))
                {
                    if(!reducerMethod.IsStatic)
                        throw new Exception($"The method '{reducerMethod.Name}' in type '{reducerMethod.DeclaringType!.FullName}' must be static to be used as a Flux reducer");
                    if (reducerMethod.GetParameters().Length != 2)
                        throw new Exception($"The method '{reducerMethod.Name}' in type '{reducerMethod.DeclaringType!.FullName}' must declare exactly 2 parameters to be used as a Flux reducer");
                    var stateType = reducerMethod.GetParameters()[0].ParameterType;
                    var actionType = reducerMethod.GetParameters()[1].ParameterType;
                    var reducerType = typeof(Reducer<,>).MakeGenericType(stateType, actionType);
                    var reducerLambda = this.BuildReducerLambda(stateType, actionType, reducerMethod);
                    var reducerFunction = reducerLambda.Compile();
                    var reducer = (IReducer)ActivatorUtilities.CreateInstance(this.ServiceProvider, reducerType, reducerFunction);
                    if (reducersPerState.TryGetValue(stateType, out var reducers))
                        reducers.Add(reducer);
                    else
                        reducersPerState.Add(stateType, new() { reducer });
                }
            }
            foreach (var featureType in TypeCacheUtil.FindFilteredTypes("nflux-features",
               t => t.IsClass && !t.IsAbstract && !t.IsInterface && !t.IsGenericType && t.TryGetCustomAttribute<FeatureAttribute>(out _)))
            {
                var reducersPerFeature = new List<IReducer>();
                foreach (var stateType in featureType.GetProperties()
                    .Where(p => p.GetGetMethod(true) != null && p.GetSetMethod(true) != null)
                    .Select(p => p.PropertyType))
                {
                    if(reducersPerState.TryGetValue(stateType, out var reducers))
                        reducersPerFeature.AddRange(reducers);
                }
                AddFeatureMethod.MakeGenericMethod(featureType).Invoke(null, new object[] { store, reducersPerFeature.ToArray() });
            }
            foreach (var effectDeclaringType in TypeCacheUtil.FindFilteredTypes("nflux-effects",
                t => t.GetMethods().Any(m => m.TryGetCustomAttribute<EffectAttribute>(out _)), this.FluxOptions.AssembliesToScan?.ToArray()))
            {
                foreach (var effectMethod in effectDeclaringType.GetMethods()
                   .Where(m => m.TryGetCustomAttribute<EffectAttribute>(out _)))
                {
                    if (!effectMethod.IsStatic)
                        throw new Exception($"The method '{effectMethod.Name}' in type '{effectMethod.DeclaringType!.FullName}' must be static to be used as a Flux effect");
                    if (effectMethod.GetParameters().Length != 1)
                        throw new Exception($"The method '{effectMethod.Name}' in type '{effectMethod.DeclaringType!.FullName}' must declare exactly 1 parameters to be used as a Flux effect");
                    var actionType = effectMethod.GetParameters()[0].ParameterType;
                    var effectType = typeof(Effect<>).MakeGenericType(actionType);
                    var effectLambda = this.BuildEffectLambda(actionType, effectMethod);
                    var effectFunction = effectLambda.Compile();
                    var effect = (IEffect)ActivatorUtilities.CreateInstance(this.ServiceProvider, effectType, effectFunction);
                    store.AddEffect(effect);
                }
            }
            foreach (var middlewareType in TypeCacheUtil.FindFilteredTypes("nflux-middlewares",
                  t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsGenericType && typeof(IMiddleware).IsAssignableFrom(t), this.FluxOptions.AssembliesToScan?.ToArray()))
            {
                var middleware = (IMiddleware)ActivatorUtilities.CreateInstance(this.ServiceProvider, middlewareType);
                store.AddMiddleware(middleware);
            }
            return store;
        }

        /// <summary>
        /// Builds a new reducer <see cref="LambdaExpression"/> for the specified state type, action type and reducer method
        /// </summary>
        /// <param name="stateType">The type of state to create the reducer <see cref="LambdaExpression"/> for</param>
        /// <param name="actionType">The type of action to create the reducer <see cref="LambdaExpression"/> for</param>
        /// <param name="reducerMethod">The reducer method</param>
        /// <returns>A new reducer <see cref="LambdaExpression"/></returns>
        protected virtual LambdaExpression BuildReducerLambda(Type stateType, Type actionType, MethodInfo reducerMethod)
        {
            if(stateType == null)
                throw new ArgumentNullException(nameof(stateType));
            if (actionType == null)
                throw new ArgumentNullException(nameof(actionType));
            if (reducerMethod == null)
                throw new ArgumentNullException(nameof(reducerMethod));
            var stateParam = Expression.Parameter(stateType, "state");
            var actionParam = Expression.Parameter(actionType, "action");
            var methodCall = Expression.Call(null, reducerMethod, stateParam, actionParam);
            var lambda = Expression.Lambda(methodCall, stateParam, actionParam);
            return lambda;
        }

        /// <summary>
        /// Builds a new effect <see cref="LambdaExpression"/> for the specified action type and reducer method
        /// </summary>
        /// <param name="actionType">The type of action to create the effect <see cref="LambdaExpression"/> for</param>
        /// <param name="effectMethod">The effect method</param>
        /// <returns>A new effect <see cref="LambdaExpression"/></returns>
        protected virtual LambdaExpression BuildEffectLambda(Type actionType, MethodInfo effectMethod)
        {
            if (actionType == null)
                throw new ArgumentNullException(nameof(actionType));
            if (effectMethod == null)
                throw new ArgumentNullException(nameof(effectMethod));
            var actionParam = Expression.Parameter(actionType, "action");
            var methodCall = Expression.Call(null, effectMethod, actionParam);
            var lambda = Expression.Lambda(methodCall, actionParam);
            return lambda;
        }

    }

}
