using UnityEngine;

public class FunctionContainer : Container
{
    [SerializeField] private string requiredItemName;

    public void CheckItemAdded(string itemName)
    {
        Debug.Log(itemName);
        if (itemName == requiredItemName) Debug.Log(requiredItemName + " added");
    }
}
