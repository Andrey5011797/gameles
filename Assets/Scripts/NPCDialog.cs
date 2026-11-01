using UnityEngine;
using TMPro;

public class NPCDialog : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogPanel;
    public TextMeshProUGUI npcText;
    public GameObject choiceButtons;
    public PlayerInteractor playerInteractor;

    [Header("Реплики")]
    [TextArea] public string startDialogue = "Кай. Я знал, что ты придёшь. Собиратель Тэрн ушёл в Заросшие Пустоши три дня назад. Он искал «особый» камень. Сегодня ночью в шахте видели свет.";
    [TextArea] public string lightAnswer = "Ты добрый юноша. Береги себя. И... если снова услышишь голоса, не отвечай им.";
    [TextArea] public string shadowAnswer = "Вот 10 монет. Ты стал слишком прагматичным, Кай.";

    private bool _isOpen = false;

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    public void StartDialog()
    {
        if (_isOpen) return;

        if (QuestManager.Instance != null && !QuestManager.Instance.IsAllQuestsDone())
        {
            if (playerInteractor != null)
                playerInteractor.ShowMessage("Сначала открой сундук и скрафти лампу!");
            return; 
        }

        _isOpen = true;
        dialogPanel.SetActive(true);
        npcText.text = startDialogue;

        if (choiceButtons != null) choiceButtons.SetActive(true);

        playerInteractor.OpenUI();
    }

    public void ChooseLight()
    {
        npcText.text = lightAnswer;
        if (choiceButtons != null) choiceButtons.SetActive(false);
        Invoke(nameof(CloseDialog), 4f);
    }

    public void ChooseShadow()
    {
        npcText.text = shadowAnswer;
        if (choiceButtons != null) choiceButtons.SetActive(false);
        Invoke(nameof(CloseDialog), 4f);
    }

    public void CloseDialog()
    {
        _isOpen = false;
        dialogPanel.SetActive(false);
        playerInteractor.CloseUI();
    }
}