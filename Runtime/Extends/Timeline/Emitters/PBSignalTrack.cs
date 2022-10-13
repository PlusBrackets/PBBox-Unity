using System;
using UnityEngine.Timeline;
using UnityEngine;

namespace PBBox.Timeline
{
    [Serializable]
    [TrackBindingType(typeof(PBSignalReceiver))]
    [TrackColor(0.25f, 0.25f, 0.5f)]
    [ExcludeFromPreset]
    public class PBSignalTrack : MarkerTrack { }
}
