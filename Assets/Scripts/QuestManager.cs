using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Прогресс")]
    public bool isChestOpened = false;
    public bool isLampCrafted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsAllQuestsDone()
    {
        return isChestOpened && isLampCrafted;
    }
}