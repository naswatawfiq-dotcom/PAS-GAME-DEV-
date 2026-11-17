using UnityEngine;
using UnityEngine.Serialization;

// Data item (ScriptableObject). Buat aset lewat:
// klik kanan di Project > Create > Inventory > Item Details
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Details")]
public class ItemDetails : ScriptableObject
{
    // FormerlySerializedAs: supaya aset yang dibuat dengan versi lama tidak kehilangan isinya
    [SerializeField, FormerlySerializedAs("ItemName")] private string itemName;
    [SerializeField, TextArea(2, 5)] private string description;
    [SerializeField, FormerlySerializedAs("Quantity")] private int quantity = 1;
    [SerializeField, FormerlySerializedAs("ItemIcon")] private Sprite itemIcon;

    public string ItemName => itemName;
    public string Description => description;
    public int Quantity => quantity;
    public Sprite ItemIcon => itemIcon;
}