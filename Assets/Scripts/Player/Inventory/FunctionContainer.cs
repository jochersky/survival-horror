using Unity.VisualScripting;
using UnityEngine;

public class FunctionContainer : Container
{
    [Header("References")]
    [SerializeField] private Toggleable[] toggleables;
    public ContainerManager containerManager;
    
    [SerializeField] private string requiredItemName;
    
    protected delegate void ToggleFunctionality();
    protected event ToggleFunctionality OnToggleFunctionality;

    public virtual void Start()
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
