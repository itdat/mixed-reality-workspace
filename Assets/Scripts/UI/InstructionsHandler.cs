using Photon.Pun;
using UnityEngine;

namespace UI {
    public class InstructionsHandler : MonoBehaviour {
        private PhotonView photonView;


        private void Update() {
            if (Input.GetKeyDown(KeyCode.H)) {
                var children = GetComponentInChildren<Transform>();
                foreach (Transform child in children) {
                    var active = child.gameObject.activeSelf;
                    child.gameObject.SetActive(!active);
                }
            }

            if (photonView == null) {
                var model = GameObject.Find("AcquireChan(Clone)");
                photonView = model.GetComponent<PhotonView>();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                photonView.RPC("PlayWelcome", RpcTarget.All);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                photonView.RPC("PlayTalking", RpcTarget.All, 2f);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                photonView.RPC("PlayThank", RpcTarget.All);
        }
    }
}
