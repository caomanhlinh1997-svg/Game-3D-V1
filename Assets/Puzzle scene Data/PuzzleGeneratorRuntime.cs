using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class PuzzleGeneratorRuntime : MonoBehaviour
{
    [Header("UI Parents")]
    public RectTransform slotsParent;    // assign SlotsParent (RectTransform)
    public RectTransform piecesParent;   // assign PiecesParent (RectTransform)

    [Header("Prefabs")]
    public GameObject slotTemplatePrefab;
    public GameObject pieceButtonPrefab;

    [Header("Debug / Test")]
    public int testPuzzleId = 1;

    // public so other scripts can read it (PuzzleManager)
    public PuzzleData currentPuzzle;

    void Start()
    {
        LoadPuzzle();
        GenerateUI();
    }

    // load puzzle from PuzzleDatabase (assumes that compiles)
    public void LoadPuzzle()
    {
        var db = FindObjectOfType<PuzzleDatabase>();
        if (db == null)
        {
            Debug.LogError("[PuzzleGeneratorRuntime] PuzzleDatabase not found in scene.");
            return;
        }

        currentPuzzle = db.GetPuzzleById(testPuzzleId);
        if (currentPuzzle == null)
            Debug.LogError("[PuzzleGeneratorRuntime] Puzzle id not found: " + testPuzzleId);
        else
            Debug.Log("[PuzzleGeneratorRuntime] Loaded puzzle: " + currentPuzzle.sentence);
    }

    public void GenerateUI()
    {
        if (currentPuzzle == null)
            return;

        // clear existing children
        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(slotsParent.GetChild(i).gameObject);

        for (int i = piecesParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(piecesParent.GetChild(i).gameObject);

        // create slots
        for (int i = 0; i < currentPuzzle.pieces.Count; i++)
        {
            GameObject slot = Instantiate(slotTemplatePrefab, slotsParent);
            slot.name = "Slot_" + i;
            // ensure slot has DropSlot component
            if (slot.GetComponent<DropSlot>() == null)
                slot.AddComponent<DropSlot>();
            // clear label if using TMP in slot
            var label = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = "";
        }

        // create shuffled pieces
        var shuffled = new List<string>(currentPuzzle.pieces);
        ShuffleList(shuffled);

        for (int i = 0; i < shuffled.Count; i++)
        {
            GameObject piece = Instantiate(pieceButtonPrefab, piecesParent);
            piece.name = "Piece_" + i;
            var txt = piece.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = shuffled[i];

            // ensure draggable component exists
            if (piece.GetComponent<DraggablePiece>() == null)
                piece.AddComponent<DraggablePiece>();
        }
    }

    void ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(0, list.Count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    // public so PuzzleManager can read ordered slots
    public List<DropSlot> GetSlotsOrdered()
    {
        List<DropSlot> slots = new List<DropSlot>();
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            var s = slotsParent.GetChild(i).GetComponent<DropSlot>();
            slots.Add(s);
        }
        return slots;
    }
}
