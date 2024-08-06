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

namespace Neuroglia.Blazor.Dagre;

public static class Constants
{
    public const double NodeWidth = 80;
    public const double NodeHeight = 40;
    public const double NodeRadius = 5;

    public const double ClusterWidth = 120;
    public const double ClusterHeight = 80;
    public const double ClusterRadius = 10;
    /**
     * Observed cluster padding, don't know where is comes from. 
     * The "ranksep" and "nodesep" default values at 50...?
     */
    public const double ClusterPaddingX = 50;
    public const double ClusterPaddingY = 70;

    public const double LabelHeight = 25;

    public const double EdgeLabelWidth = 100;
    public const double EdgeLabelHeight = 50;
    public const string EdgeEndArrowId = "end-arrow";

    public const decimal MinScale = 0.2M;
    public const decimal MaxScale = 5M;
}
