using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class ClickableThingy : MonoBehaviour, IMixedRealityInputHandler
{
    public void OnInputUp(InputEventData eventData)
    {
        //Do something else
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("Air-tapped!!!");
    }
}