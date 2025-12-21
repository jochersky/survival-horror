using System;
using System.Collections.Generic;
using UnityEditor.Scripting;
using UnityEngine;

public class Container : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject containerUI;
    private GameObject _inventoryUIContainer;

    public GameObject ContainerUI => containerUI;

    public virtual void OnContainerAdded()
    {
        AudioManager.Instance.PlaySFX(SfxType.MetalContainerOpening);
    }

    public virtual void OnContainerRemoved()
    {
        AudioManager.Instance.PlaySFX(SfxType.MetalContainerClosing);
    }

    public void Interact()
    {
        InventoryManager.instance.AddContainer(this);
    }
}
