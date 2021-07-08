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
using System;

namespace Neuroglia
{
    /// <summary>
    /// Defines helpers for UNIX time stamps
    /// </summary>
    public static class UnixTimeStamp
    {

        private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a UNIX time stamp to a <see cref="DateTime"/>
        /// </summary>
        /// <param name="unixTimestamp">The UNIX time stamp to convert</param>
        /// <returns>The resulting <see cref="DateTime"/></returns>
        public static DateTime ToDateTime(long unixTimestamp)
        {
            return Epoch.AddSeconds(unixTimestamp).ToLocalTime();
        }

        /// <summary>
        /// Converts a UNIX time stamp to a <see cref="DateTimeOffset"/>
        /// </summary>
        /// <param name="unixTimestamp">The UNIX time stamp to convert</param>
        /// <returns>The resulting <see cref="DateTime"/></returns>
        public static DateTimeOffset ToDateTimeOffset(long unixTimestamp)
        {
            return Epoch.AddSeconds(unixTimestamp).ToLocalTime();
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a UNIX time stamp
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to convert</param>
        /// <returns>The resulting UNIX time stamp</returns>
        public static long Parse(DateTime dateTime)
        {
            return (long)Math.Round(dateTime.ToUniversalTime().Subtract(Epoch).TotalSeconds, 0);
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> to a UNIX time stamp
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTimeOffset"/> to convert</param>
        /// <returns>The resulting UNIX time stamp</returns>
        public static long Parse(DateTimeOffset dateTime)
        {
            return (long)Math.Round(dateTime.ToUniversalTime().Subtract(Epoch).TotalSeconds, 0);
        }

    }

}