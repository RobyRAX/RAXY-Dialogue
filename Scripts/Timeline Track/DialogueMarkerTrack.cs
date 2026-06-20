using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RAXY.Dialogue
{
    [TrackColor(0.3f, 0.8f, 1f)]
    [TrackBindingType(typeof(DialogueController_MainTimeline))]
    public class DialogueMarkerTrack : MarkerTrack
    {
    }
}

