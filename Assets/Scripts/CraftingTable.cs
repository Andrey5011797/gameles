using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject craftingPanel;
    public PlayerInteractor playerInteractor;

    [Header("Что создаем")]
    public GameObject lampPrefab;
    public Transform spawnPoint;

    private bool _isOpen = false;

    void Start()
    {
        if (craftingPanel != null) craftingPanel.SetActive(false);
    }

    public void OpenCraftingMenu()
    {
        if (_isOpen) return;
        _isOpen = true;
        craftingPanel.SetActive(true);
        playerInteractor.OpenUI();
    }

    public void CraftLamp()
    {
        Debug.Log("Лампа создана!");

        if (lampPrefab != null && spawnPoint != null)
        {
            Instantiate(lampPrefab, spawnPoint.position, Quaternion.identity);
        }

        if (QuestManager.Instance != null)
            QuestManager.Instance.isLampCrafted = true;

        CloseCraftingMenu();
    }

    public void CloseCraftingMenu()
    {
        _isOpen = false;
        craftingPanel.SetActive(false);
        playerInteractor.CloseUI();
    }
}