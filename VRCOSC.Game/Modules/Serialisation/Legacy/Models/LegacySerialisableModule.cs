﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace VRCOSC.Game.Modules.Serialisation.Legacy.Models;

public class LegacySerialisableModule
{
    [JsonProperty("enabled")]
    public bool Enabled;

    [JsonProperty("settings")]
    public Dictionary<string, object> Settings = new();

    [JsonProperty("parameters")]
    public Dictionary<string, object> Parameters = new();

    [JsonConstructor]
    public LegacySerialisableModule()
    {
    }

    public LegacySerialisableModule(Module module)
    {
        Enabled = module.Enabled.Value;

        module.Settings.ForEach(pair =>
        {
            if (pair.Value.IsDefault()) return;

            Settings.Add(pair.Key, pair.Value.GetSerialisableValue());
        });

        module.Parameters.ForEach(pair =>
        {
            if (pair.Value.IsDefault()) return;

            Parameters.Add(pair.Key.ToLookup(), pair.Value.GetSerialisableValue());
        });
    }
}
