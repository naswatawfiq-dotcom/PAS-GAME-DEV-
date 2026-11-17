using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, GrabableItem
{
    InputAction inventoryAction;

    [SerializeField] private GameObject InventoryCanvas;            // panel inventory (yang dibuka/tutup)
    [SerializeField] private int slotCount = 16;
    [SerializeField] private ItemSlot slotPrefab;                   // opsional
    [SerializeField] private Transform slotParent;                  // opsional
    [SerializeField] private ItemDetailsPanel detailsPanel;         // opsional

    private bool InventoryActive;
    public ItemSlot[] itemSlot;

    void Start()
    {
        if (InventoryCanvas == null)
        {
            Debug.LogError("Inventory Canvas belum di-drag di Inspector.", this);
            itemSlot = new ItemSlot[0];
            return;
        }

        inventoryAction = InputSystem.actions.FindAction("Inventory");
        if (inventoryAction == null)
        {
            Debug.LogError("Action 'Inventory' tidak ditemukan di Input Actions.", this);
        }

        // Pastikan panel aktif dulu supaya Awake semua slot sudah jalan sebelum dipakai
        InventoryCanvas.SetActive(true);
        CreateSlots();

        InventoryCanvas.SetActive(false);
        InventoryActive = false;
        DesselectAllSlot();
    }

    void Update()
    {
        if (inventoryAction == null || InventoryCanvas == null) return;

        if (inventoryAction.triggered)
        {
            InventoryActive = !InventoryActive;
            Time.timeScale = InventoryActive ? 0f : 1f;
            InventoryCanvas.SetActive(InventoryActive);
            Debug.Log(InventoryActive ? "Inventory is active" : "Inventory is deactivated");

            DesselectAllSlot();
        }
    }

    void OnDestroy()
    {
        // Jangan biarkan game membeku kalau scene di-reload saat inventory terbuka
        Time.timeScale = 1f;
    }

    // Siapkan slot: pakai slot yang sudah ada di scene, kalau tidak ada buat dari prefab
    private void CreateSlots()
    {
        Transform parent = slotParent;
        if (parent == null)
        {
            GridLayoutGroup grid = InventoryCanvas.GetComponentInChildren<GridLayoutGroup>(true);
            parent = grid != null ? grid.transform : InventoryCanvas.transform;
        }

        // 1) Slot yang sudah dipasang di scene (komponen ItemSlot)
        ItemSlot[] existing = parent.GetComponentsInChildren<ItemSlot>(true);
        if (existing.Length > 0)
        {
            itemSlot = existing;
            foreach (ItemSlot slot in itemSlot)
            {
                slot.inventoryManager = this;
                slot.Clear();
            }
            return;
        }

        // 2) Buat dari prefab (drag ke Slot Prefab, atau taruh di Resources dengan nama "Slot"/"ItemSlot")
        ItemSlot prefab = slotPrefab;
        if (prefab == null) prefab = Resources.Load<ItemSlot>("Slot");
        if (prefab == null) prefab = Resources.Load<ItemSlot>("ItemSlot");

        if (prefab == null)
        {
            Debug.LogError("Tidak ada slot: pasang komponen ItemSlot di slot scene, atau isi Slot Prefab.", this);
            itemSlot = new ItemSlot[0];
            return;
        }

        itemSlot = new ItemSlot[slotCount];
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i] = Instantiate(prefab, parent);
            itemSlot[i].inventoryManager = this;
            itemSlot[i].Clear();
        }
    }

    public bool AddItem(ItemDetails item)
    {
        if (item == null) return false;

        Debug.Log("Item added to inventory: " + item.ItemName + " Quantity: " + item.Quantity);

        if (itemSlot == null || itemSlot.Length == 0)
        {
            Debug.LogWarning("Inventory has no slots, '" + item.ItemName + "' was not added.");
            return false;
        }

        // Cek dulu apakah semuanya muat
        int freeSpace = 0;
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.IsEmpty || slot.CanStackWith(item.ItemName))
                freeSpace += slot.FreeSpace;
        }

        if (freeSpace < item.Quantity)
        {
            Debug.LogWarning("Inventory is full, '" + item.ItemName + "' was not added.");
            return false;
        }

        int remaining = item.Quantity;

        // Tumpuk dulu ke stack yang sama
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            if (itemSlot[i].CanStackWith(item.ItemName))
            {
                remaining = itemSlot[i].AddQuantity(remaining);
            }
        }

        // Sisanya ke slot kosong
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            if (itemSlot[i].IsEmpty)
            {
                remaining = itemSlot[i].AddItem(item, remaining);
            }
        }

        RefreshSelectedDetails();
        return true;
    }

    public void ShowDetails(ItemSlot slot)
    {
        if (detailsPanel != null) detailsPanel.Show(slot);
    }

    public void OnSlotChanged(ItemSlot slot)
    {
        if (!slot.isItemSelected) return;

        if (slot.IsEmpty)
        {
            slot.Deselect();
            if (detailsPanel != null) detailsPanel.Hide();
        }
        else if (detailsPanel != null)
        {
            detailsPanel.Show(slot);
        }
    }

    private void RefreshSelectedDetails()
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isItemSelected) OnSlotChanged(slot);
        }
    }

    public void DesselectAllSlot()
    {
        if (itemSlot != null)
        {
            foreach (ItemSlot slot in itemSlot)
            {
                slot.Deselect();
            }
        }

        if (detailsPanel != null) detailsPanel.Hide();
    }
}