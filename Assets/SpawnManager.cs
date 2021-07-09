using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    [SerializeField]
    private GameObject[] listPrefabs;

    public void SpawnStuff(int id) {
        if (id < 0 || id > listPrefabs.Length) return;
        var go = PhotonNetwork.Instantiate("Objects\\" + listPrefabs[id].name, gameObject.transform.position,
            Quaternion.identity);
        go.tag = "Stuff";
    }
}
