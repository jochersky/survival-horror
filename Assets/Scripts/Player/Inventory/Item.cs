using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    private Container _container;

    public void ChangeContainer(Container newContainer)
    {
        if (_container) _container.RemoveItem(this);
        _container = newContainer;
        _container.AddItem(this);
    }

    public void DropIntoWorld()
    {
        _container.RemoveItem(this);
        // TODO: spawn the prefab into the game
    }
}
