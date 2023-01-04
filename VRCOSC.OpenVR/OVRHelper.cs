﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Valve.VR;

namespace VRCOSC.OpenVR;

[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public static class OVRHelper
{
    public static Action<string>? OnError;

    private static void error(string methodName, ETrackedDeviceProperty property, ETrackedPropertyError error, uint index)
    {
        var name = GetStringTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_RenderModelName_String);
        OnError?.Invoke($"{methodName} encountered error {error} on device {name} when getting property {property}");
    }

    private static readonly uint compositor_frametiming_size = (uint)Unsafe.SizeOf<Compositor_FrameTiming>();
    private static readonly uint inputanalogactiondata_t_size = (uint)Unsafe.SizeOf<InputAnalogActionData_t>();
    private static readonly uint inputdigitalactiondata_t_size = (uint)Unsafe.SizeOf<InputDigitalActionData_t>();

    internal static bool InitialiseOpenVR(EVRApplicationType applicationType)
    {
        var err = new EVRInitError();
        var state = Valve.VR.OpenVR.InitInternal(ref err, applicationType);
        return err == EVRInitError.None && state != 0;
    }

    internal static float GetFrameTimeMilli()
    {
        var frameTiming = new Compositor_FrameTiming
        {
            m_nSize = compositor_frametiming_size
        };
        Valve.VR.OpenVR.Compositor.GetFrameTiming(ref frameTiming, 0);
        return frameTiming.m_flTotalRenderGpuMs;
    }

    internal static InputAnalogActionData_t GetAnalogueInput(ulong identifier)
    {
        var data = new InputAnalogActionData_t();
        Valve.VR.OpenVR.Input.GetAnalogActionData(identifier, ref data, inputanalogactiondata_t_size, Valve.VR.OpenVR.k_ulInvalidInputValueHandle);
        return data;
    }

    internal static InputDigitalActionData_t GetDigitalInput(ulong identifier)
    {
        var data = new InputDigitalActionData_t();
        Valve.VR.OpenVR.Input.GetDigitalActionData(identifier, ref data, inputdigitalactiondata_t_size, Valve.VR.OpenVR.k_ulInvalidInputValueHandle);
        return data;
    }

    // GetTrackedDeviceIndexForControllerRole doesn't work when a tracker thinks it's a controller and assumes that role
    // We can forcibly find the correct indexes by using the model name
    internal static uint GetControllerIdFromHint(string controllerHint)
    {
        return GetIndexesForTrackedDeviceClass(ETrackedDeviceClass.Controller)
               .Where(index => GetStringTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_RenderModelName_String).Contains(controllerHint, StringComparison.InvariantCultureIgnoreCase))
               .SingleOrDefault(Valve.VR.OpenVR.k_unTrackedDeviceIndexInvalid);
    }

    internal static uint GetIndexForTrackedDeviceClass(ETrackedDeviceClass klass)
    {
        var indexes = GetIndexesForTrackedDeviceClass(klass).ToArray();
        return indexes.Any() ? indexes[0] : Valve.VR.OpenVR.k_unTrackedDeviceIndexInvalid;
    }

    internal static IEnumerable<uint> GetIndexesForTrackedDeviceClass(ETrackedDeviceClass klass)
    {
        for (uint i = 0; i < Valve.VR.OpenVR.k_unMaxTrackedDeviceCount; i++)
        {
            if (Valve.VR.OpenVR.System.GetTrackedDeviceClass(i) == klass) yield return i;
        }
    }

    internal static bool GetBoolTrackedDeviceProperty(uint index, ETrackedDeviceProperty property)
    {
        var error = new ETrackedPropertyError();
        var value = Valve.VR.OpenVR.System.GetBoolTrackedDeviceProperty(index, property, ref error);

        if (error == ETrackedPropertyError.TrackedProp_Success) return value;

        OVRHelper.error(nameof(GetBoolTrackedDeviceProperty), property, error, index);
        return false;
    }

    internal static int GetInt32TrackedDeviceProperty(uint index, ETrackedDeviceProperty property)
    {
        var error = new ETrackedPropertyError();
        var value = Valve.VR.OpenVR.System.GetInt32TrackedDeviceProperty(index, property, ref error);

        if (error == ETrackedPropertyError.TrackedProp_Success) return value;

        OVRHelper.error(nameof(GetInt32TrackedDeviceProperty), property, error, index);
        return 0;
    }

    internal static float GetFloatTrackedDeviceProperty(uint index, ETrackedDeviceProperty property)
    {
        var error = new ETrackedPropertyError();
        var value = Valve.VR.OpenVR.System.GetFloatTrackedDeviceProperty(index, property, ref error);

        if (error == ETrackedPropertyError.TrackedProp_Success) return value;

        OVRHelper.error(nameof(GetFloatTrackedDeviceProperty), property, error, index);
        return 0f;
    }

    private static readonly StringBuilder sb = new((int)Valve.VR.OpenVR.k_unMaxPropertyStringSize);
    private static readonly object string_lock = new();

    internal static string GetStringTrackedDeviceProperty(uint index, ETrackedDeviceProperty property)
    {
        string str;

        lock (string_lock)
        {
            var error = new ETrackedPropertyError();
            sb.Clear();
            Valve.VR.OpenVR.System.GetStringTrackedDeviceProperty(index, property, sb, Valve.VR.OpenVR.k_unMaxPropertyStringSize, ref error);

            if (error != ETrackedPropertyError.TrackedProp_Success)
            {
                OVRHelper.error(nameof(GetStringTrackedDeviceProperty), property, error, index);
                return string.Empty;
            }

            str = sb.ToString();
        }

        return str;
    }
}
