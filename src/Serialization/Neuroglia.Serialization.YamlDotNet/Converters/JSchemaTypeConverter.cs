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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="JSchema"/>s
    /// </summary>
    public class JSchemaTypeConverter
        : JTokenSerializer
    {

        /// <inheritdoc/>
        public override bool Accepts(Type type)
        {
            return type == typeof(JSchema);
        }

        /// <inheritdoc/>
        public override void WriteYaml(IEmitter emitter, object value, Type type)
        {
            JSchema schema = value as JSchema;
            if (schema == null)
                return;
            string json = schema.ToString();
            JToken jtoken = JsonConvert.DeserializeObject<JToken>(json);
            this.WriteJToken(emitter, jtoken);
        }

    }

}
