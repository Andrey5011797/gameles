using UnityEngine;
using TMPro;
using System.Collections;

public class DoorBlocker : MonoBehaviour
{
    [Header("UI (Предупреждение)")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public PlayerInteractor playerInteractor;

    private bool _showingWarning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (QuestManager.Instance != null && !QuestManager.Instance.IsAllQuestsDone() && !_showingWarning)
        {
            StartCoroutine(ShowWarning());
        }
    }

    IEnumerator ShowWarning()
    {
        _showingWarning = true;

        if (warningPanel != null && warningText != null)
        {
            warningPanel.SetActive(true);

            if (!QuestManager.Instance.isChestOpened && !QuestManager.Instance.isLampCrafted)
                warningText.text = "Вы забыли открыть сундук и скрафтить лампу!";
            else if (!QuestManager.Instance.isChestOpened)
                warningText.text = "Вы забыли открыть сундук!";
            else if (!QuestManager.Instance.isLampCrafted)
                warningText.text = "Вы забыли скрафтить лампу!";

            yield return new WaitForSeconds(3f);
            warningPanel.SetActive(false);
        }

        _showingWarning = false;
    }
}