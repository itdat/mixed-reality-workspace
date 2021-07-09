using UnityEngine;

public class ResponseHandler : MonoBehaviour {
    public MeshRenderer MeshRenderer;

    public void ToggleClose() {
        Debug.Log("ToggleClose Image");
        Destroy(gameObject);
    }
}
