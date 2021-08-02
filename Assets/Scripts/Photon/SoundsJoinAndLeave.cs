using System;
using Photon.Pun;
using UnityEngine;
using Player = Photon.Realtime.Player;

namespace Photon {
    public class SoundsJoinAndLeave : MonoBehaviourPunCallbacks {
        public AudioClip joinClip;
        public AudioClip leaveClip;
        private AudioSource source;
        public static int numPlayer;

        private void Start() {
            numPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            if (joinClip == null) return;
            if (source == null) source = FindObjectOfType<AudioSource>();
            source.PlayOneShot(joinClip);
            numPlayer++;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            if (leaveClip == null) return;
            if (source == null) source = FindObjectOfType<AudioSource>();
            source.PlayOneShot(leaveClip);
            numPlayer--;
        }
    }
}
