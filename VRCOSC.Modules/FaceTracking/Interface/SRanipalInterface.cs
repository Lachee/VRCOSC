﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using SRanipalLib;
using VRCOSC.Game.Modules;
using VRCOSC.Modules.FaceTracking.Interface.Eyes;
using VRCOSC.Modules.FaceTracking.Interface.Lips;

namespace VRCOSC.Modules.FaceTracking.Interface;

public class SRanipalInterface
{
    public readonly SRanipalAPIInterface APIInterface = new();
    public readonly EyeTrackingData EyeData = new();
    public readonly LipTrackingData LipData = new();

    public bool EyeAvailable => APIInterface.EyeStatus.Value == Error.WORK;
    public bool LipAvailable => APIInterface.LipStatus.Value == Error.WORK;

    public void Initialise(bool eye, bool lip)
    {
        APIInterface.Initialise(eye, lip);
        EyeData.Initialise();
        LipData.Initialise();
    }

    public void Update()
    {
        APIInterface.Update();
        if (EyeAvailable) EyeData.Update(APIInterface.EyeData);
        if (LipAvailable) LipData.Update(APIInterface.LipData);
    }

    public void Release()
    {
        APIInterface.Release();
    }
}