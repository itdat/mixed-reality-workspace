using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Pen {
    public class InputData : MonoBehaviour {
        [Serializable]
        public class GrabEvent : UnityEvent<bool> { }

        public GrabEvent grabEvent = new GrabEvent();

        private void Update() {
            foreach (var controller in CoreServices.InputSystem.DetectedControllers) {
                var inputSource = controller.InputSource;
                if (controller.ControllerHandedness != Handedness.Right) continue;
                if (inputSource.SourceType != InputSourceType.Hand) continue;

                foreach (var inputMapping in controller.Interactions) {
                    if (inputMapping.Description != "Grab") continue;
                    grabEvent.Invoke(inputMapping.BoolData);
                    return;
                }
            }
        }
    }
}
