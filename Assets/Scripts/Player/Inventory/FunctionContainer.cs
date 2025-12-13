using UnityEngine;

public class FunctionContainer : Container
{
    [Header("References")]
    [SerializeField] private Toggleable[] toggleables;
    
    [SerializeField] private string requiredItemName;
    public ContainerManager containerManager;
    
    private delegate void ToggleFunctionality();
    private event ToggleFunctionality OnToggleFunctionality;

    private void Start()
    {
        containerManager.OnNewItemAdded += CheckItemAdded;
        
        foreach (Toggleable t in toggleables)
        {
            OnToggleFunctionality += t.Toggle;
        }
    }
    
    private void CheckItemAdded(string itemName)
    {
        if (itemName == requiredItemName) OnToggleFunctionality?.Invoke();
    }
}
