using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogflow.Event {
    [Serializable]
    public class AudioResponseEvent : UnityEvent<AudioClip> { }
}
