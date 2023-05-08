﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

namespace VRCOSC.Game.Serialisation;

public interface ISerialiser<TReturn> where TReturn : class
{
    public bool Deserialise(out TReturn? data);
    public bool Serialise();
}
