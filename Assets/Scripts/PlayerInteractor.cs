using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки поиска")]
    public LayerMask interactableLayer;
    public float searchRadius = 3.5f;

    [Header("UI Подсказка (снизу)")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    [Header("Пустое UI (по центру)")]
    public GameObject emptyUIPanel;

    private Interactable _currentTarget;
    private bool _isUIOpen = false;

    void Update()
    {
        if (_isUIOpen)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                CloseEmptyUI();
            }
            return;
        }

        CheckForInteractable();

        if (_currentTarget != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            _currentTarget.Interact();

            if (_currentTarget.promptText != "Использовать верстак" && _currentTarget.promptText != "Поговорить")
            {
                OpenEmptyUI();
            }
        }
    }

    void CheckForInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, interactableLayer);
        Interactable nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Interactable interactable))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = interactable;
                }
            }
        }

        _currentTarget = nearest;

        if (_currentTarget != null)
        {
            if (!promptPanel.activeSelf) promptPanel.SetActive(true);
            promptText.text = $"[E] {_currentTarget.promptText}";
        }
        else
        {
            if (promptPanel.activeSelf) promptPanel.SetActive(false);
        }
    }

    public void OpenUI()
    {
        _isUIOpen = true;
        if (promptPanel != null) promptPanel.SetActive(false);

        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseUI()
    {
        _isUIOpen = false;

        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowMessage(string message)
    {
        if (promptPanel != null && promptText != null)
        {
            promptPanel.SetActive(true);
            promptText.text = message;
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), 3f);
        }
    }

    private void HideMessage()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void OpenEmptyUI()
    {
        _isUIOpen = true;
        if (promptPanel != null) promptPanel.SetActive(false);
        if (emptyUIPanel != null) emptyUIPanel.SetActive(true);

        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (QuestManager.Instance != null)
            QuestManager.Instance.isChestOpened = true;
    }

    void CloseEmptyUI()
    {
        _isUIOpen = false;
        if (emptyUIPanel != null) emptyUIPanel.SetActive(false);

        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}