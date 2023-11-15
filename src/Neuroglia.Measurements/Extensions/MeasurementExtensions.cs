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

namespace Neuroglia.Measurements;

/// <summary>
/// Defines extensions for <see cref="Measurement"/>s
/// </summary>
public static class MeasurementExtensions
{

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Capacity"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Capacity"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Capacity"/></returns>
    public static Capacity? AsCapacity(this Measurement measurement) => measurement is Capacity capacity ? capacity : measurement.Unit.Type == UnitOfMeasurementType.Capacity ? new Capacity(measurement.Value, measurement.Unit) : null;

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Energy"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Energy"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Energy"/></returns>
    public static Energy? AsEnergy(this Measurement measurement) => measurement is Energy energy ? energy : measurement.Unit.Type == UnitOfMeasurementType.Energy ? new Energy(measurement.Value, measurement.Unit) : null;

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Length"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Length"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Length"/></returns>
    public static Length? AsLength(this Measurement measurement) => measurement is Length length ? length : measurement.Unit.Type == UnitOfMeasurementType.Length ? new Length(measurement.Value, measurement.Unit) : null;

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Mass"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Mass"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Mass"/></returns>
    public static Mass? AsMass(this Measurement measurement) => measurement is Mass mass ? mass : measurement.Unit.Type == UnitOfMeasurementType.Mass ? new Mass(measurement.Value, measurement.Unit) : null;

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Surface"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Surface"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Surface"/></returns>
    public static Surface? AsSurface(this Measurement measurement) => measurement is Surface surface ? surface : measurement.Unit.Type == UnitOfMeasurementType.Surface ? new Surface(measurement.Value, measurement.Unit) : null;

    /// <summary>
    /// Converts the <see cref="Measurement"/> to a <see cref="Volume"/>
    /// </summary>
    /// <param name="measurement">The <see cref="Measurement"/> to convert</param>
    /// <returns>A new <see cref="Volume"/>, if the <see cref="Measurement"/> is expressed with a unit of type <see cref="UnitOfMeasurementType.Volume"/></returns>
    public static Volume? AsVolume(this Measurement measurement) => measurement is Volume volume ? volume : measurement.Unit.Type == UnitOfMeasurementType.Volume ? new Volume(measurement.Value, measurement.Unit) : null;

}
