﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Valve.VR;
using VRCOSC.Game.OpenVR.Device;
using VRCOSC.Game.OpenVR.Metadata;

namespace VRCOSC.Game.OpenVR;

public class OVRClient
{
    private static readonly uint vrevent_t_size = (uint)Unsafe.SizeOf<VREvent_t>();

    public Action? OnShutdown;

    public bool HasInitialised { get; private set; }

    public readonly OVRMetadata Metadata;
    public readonly OVRSystem System;
    public readonly OVRInput Input;

    public HMD HMD => System.HMD;
    public Controller LeftController => System.LeftController;
    public Controller RightController => System.RightController;
    public IEnumerable<Tracker> Trackers => System.Trackers;

    public OVRClient(OVRMetadata metadata)
    {
        Metadata = metadata;
        System = new OVRSystem();
        Input = new OVRInput(this);
    }

    public void Init()
    {
        if (HasInitialised) return;

        if (!OVRHelper.InitialiseOpenVR(Metadata.ApplicationType)) return;

        Valve.VR.OpenVR.Applications.AddApplicationManifest(Metadata.ApplicationManifest, false);
        System.Init();
        Input.Init();

        HasInitialised = true;
    }

    public void Update()
    {
        if (!HasInitialised) return;

        pollEvents();

        if (!HasInitialised) return;

        System.Update();
        Input.Update();
    }

    private void pollEvents()
    {
        var evenT = new VREvent_t();

        while (Valve.VR.OpenVR.System.PollNextEvent(ref evenT, vrevent_t_size))
        {
            var eventType = (EVREventType)evenT.eventType;

            switch (eventType)
            {
                case EVREventType.VREvent_Quit:
                    Valve.VR.OpenVR.System.AcknowledgeQuit_Exiting();
                    shutdown();
                    return;
            }
        }
    }

    public void TriggerLeftControllerHaptic(float durationSeconds, float frequency, float amplitude) => OVRHelper.TriggerHaptic(Input.LeftControllerHapticActionHandle, System.LeftController.Id, durationSeconds, frequency, amplitude);
    public void TriggerRightControllerHaptic(float durationSeconds, float frequency, float amplitude) => OVRHelper.TriggerHaptic(Input.RightControllerHapticActionHandle, System.RightController.Id, durationSeconds, frequency, amplitude);

    private void shutdown()
    {
        Valve.VR.OpenVR.Shutdown();
        HasInitialised = false;
        OnShutdown?.Invoke();
    }

    public void SetAutoLaunch(bool value)
    {
        if (!HasInitialised) return;

        Valve.VR.OpenVR.Applications.SetApplicationAutoLaunch("volcanicarts.vrcosc", value);
    }
}
