﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Generic;
using VRCOSC.Game.Modules.Sources;

namespace VRCOSC.Game.Modules;

public interface IModuleManager : IEnumerable<Module>
{
    public void AddSource(IModuleSource source);
    public bool RemoveSource(IModuleSource source);
    public void Load();
    public void Start();
    public void Stop();
}
