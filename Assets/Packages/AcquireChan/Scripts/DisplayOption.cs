using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace AcquireChan.Scripts {
    public class DisplayOption : MonoBehaviour {
        public void HideMesh() {
            Debug.Log("HideMesh");
            var observer =
                CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
        }

        public void ShowMesh() {
            Debug.Log("ShowMesh");
            var observer =
                CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
        }

    }
}
