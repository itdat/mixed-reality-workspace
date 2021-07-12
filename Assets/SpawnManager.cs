using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    [SerializeField]
    private GameObject[] listPrefabs;

    public bool isInRoom;

    public void SpawnStuff(int id) {
        if (id < 0 || id > listPrefabs.Length) return;
        GameObject go;
        var position = gameObject.transform.position;
        position.z += 1;
        if (isInRoom)
            go = PhotonNetwork.Instantiate("Objects\\" + listPrefabs[id].name, position,
                Quaternion.identity);
        else
            go = Instantiate(listPrefabs[id], position, Quaternion.identity);
        go.tag = "Stuff";
    }
}
