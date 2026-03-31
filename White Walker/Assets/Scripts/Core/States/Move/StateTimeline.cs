using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/StateTimeline")]
public class StateTimeline : ScriptableObject
{
    public int totalFrames;
    public List<StateEvent> events = new List<StateEvent>();
}