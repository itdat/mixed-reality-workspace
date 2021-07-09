using AcquireChan.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon {
    public class NetworkManager : MonoBehaviourPunCallbacks {
        public string gameVersion = "1";
        public bool isAdmin;
        public GameObject buttonTakePhoto;

        private void Awake() {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start() {
            ConnectToServer();
        }

        private void ConnectToServer() {
            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.JoinRoom(PhotonNetwork.CountOfRooms.ToString());
            }
            else {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public override void OnConnectedToMaster() {
            Debug.Log("OnConnectedToMaster");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby() {
            Debug.Log("OnJoinedLobby");
            PhotonNetwork.JoinRoom(PhotonNetwork.CountOfRooms.ToString());
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            Debug.Log("OnJoinRoomFailed " + message);
            var options = new RoomOptions { MaxPlayers = 2 };
            PhotonNetwork.CreateRoom((PhotonNetwork.CountOfRooms + 1).ToString(), options);
        }

        public override void OnCreatedRoom() {
            Debug.Log("OnCreatedRoom: " + PhotonNetwork.CurrentRoom);
            PhotonNetwork.JoinRoom(PhotonNetwork.CountOfRooms.ToString());
        }

        private GameObject model;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnJoinedRoom() {
            Debug.Log("OnJoinedRoom: " + PhotonNetwork.CurrentRoom);
            if (!isAdmin) {
                model = PhotonNetwork.Instantiate("AcquireChan", gameObject.transform.position,
                    Quaternion.identity);
                model.GetComponent<AgentController>().enabled = true;
                model.GetComponent<MoveFollowCamera>().enabled = true;
            }
            else {
                var go = Resources.Load<GameObject>("Audio\\Speaker");
                Instantiate(go);
                buttonTakePhoto.SetActive(true);
            }
        }

        public override void OnDisconnected(DisconnectCause cause) {
            PhotonNetwork.Destroy(model);
            Debug.Log("OnDisconnected " + cause);
        }
    }
}
