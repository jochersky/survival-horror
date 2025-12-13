using UnityEngine;

public class Toggleable : MonoBehaviour
{
    public bool toggled = false; 

    public void Toggle()
    {
        toggled = !toggled;
    }
}
