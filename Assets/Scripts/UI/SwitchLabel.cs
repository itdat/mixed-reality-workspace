using TMPro;
using UnityEngine;

namespace UI {
    public class SwitchLabel : MonoBehaviour {
        public string selectLabel = "On";

        public string deselectLabel = "Off";

        private TextMeshPro textMeshPro;

        private TextMesh textMesh;

        // Start is called before the first frame update
        private void Start() {
            textMeshPro = GetComponent<TextMeshPro>();
            textMesh = GetComponent<TextMesh>();
        }

        public void UpdateLabel(bool value) {
            if (textMeshPro != null) {
                textMeshPro.text = value ? selectLabel : deselectLabel;
            }

            if (textMesh != null) {
                textMesh.text = value ? selectLabel : deselectLabel;
            }
        }
    }
}
