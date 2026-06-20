using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PauseTimelineMarker : Marker, INotification
{
    public PropertyName id => new PropertyName();
}
