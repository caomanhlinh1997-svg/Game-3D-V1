using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Button confirmButton;

    private PuzzleGeneratorRuntime generator;

    void Start()
    {
        generator = FindObjectOfType<PuzzleGeneratorRuntime>();

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirm);
    }

    void OnConfirm()
    {
        if (generator == null || generator.currentPuzzle == null)
        {
            Debug.LogError("PuzzleManager: Generator hoặc currentPuzzle null!");
            return;
        }

        bool isCorrect = CheckPuzzleCorrect();

        if (isCorrect)
            ShowWin();
        else
            ShowLose();
    }

    bool CheckPuzzleCorrect()
    {
        List<string> correctOrder = generator.currentPuzzle.pieces;
        List<DropSlot> slots = generator.GetSlotsOrdered();

        if (slots == null || slots.Count != correctOrder.Count)
            return false;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];

            if (slot == null || slot.currentPiece == null)
                return false;

            TMP_Text txt = slot.currentPiece.GetComponentInChildren<TMP_Text>();
            if (txt == null) return false;

            if (txt.text != correctOrder[i])
                return false;
        }

        return true;
    }

    void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
        if (losePanel) losePanel.SetActive(false);
        Debug.Log("WIN ✔");
    }

    void ShowLose()
    {
        if (losePanel) losePanel.SetActive(true);
        if (winPanel) winPanel.SetActive(false);
        Debug.Log("WRONG ✖");
    }
}
