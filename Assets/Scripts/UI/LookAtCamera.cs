namespace UI {
    using UnityEngine;

    public class LookAtCamera : MonoBehaviour {
        private void Update() {
            var transform1 = transform;
            if (!(Camera.main is null))
                transform1.LookAt(Camera.main.transform.position, Vector3.up);
            transform1.localEulerAngles = new Vector3(0, transform1.localEulerAngles.y, 0);
        }
    }
}
