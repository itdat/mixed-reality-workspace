using System;
using UnityEngine.Events;

namespace Dialogflow.Event {
    [Serializable]
    public class TextResponseEvent : UnityEvent<string> { }
}
