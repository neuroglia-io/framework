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
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICsvReader"/> interface
    /// </summary>
    public class CsvReader
        : ICsvReader
    {

        /// <inheritdoc/>
        public virtual async Task<DataTable> ReadFromAsync(Stream stream, ICsvDocumentOptions options, CancellationToken cancellationToken = default)
        {
            DataTable table = new();
            using StreamReader reader = new(stream);
            string csv = await reader.ReadToEndAsync();
            string[] csvLines = csv.Split(Environment.NewLine);
            string csvRow = csvLines[0];
            string[] csvColumns = csvRow.Split(options.ValueSeparator);
            foreach (string column in csvColumns)
            {
                table.Columns.Add(column);
            }
            for (int rowIndex = 1; rowIndex < csvLines.Length; rowIndex++)
            {
                DataRow row = table.NewRow();
                csvRow = csvLines[rowIndex];
                csvColumns = csvRow.Split(options.ValueSeparator);
                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    row.SetField(columnIndex, csvColumns[columnIndex]);
                }
                table.Rows.Add(row);
            }
            return table;
        }

    }

}
