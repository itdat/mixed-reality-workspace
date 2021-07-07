using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Pen {
    public class HandGizmoHandler : MonoBehaviour {
        public InputSourceType sourceType;
        public Handedness handedness;
        public Renderer[] renderers;
        private bool isDataAvailable = true;

        private void SetIsDataAvailable(bool value) {
            if (value != isDataAvailable) {
                foreach (var item in renderers) {
                    item.enabled = value;
                }
            }

            isDataAvailable = value;
        }

        public void Update() {
            if (InputRayUtils.TryGetRay(sourceType, handedness, out var myRay)) {
                transform.position = myRay.origin;
                transform.rotation = Quaternion.LookRotation(myRay.direction, Vector3.up);
                SetIsDataAvailable(true);
            }
            else {
                SetIsDataAvailable(false);
            }
        }
    }
}
