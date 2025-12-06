using System.Collections.Generic;
using UnityEngine;

// PuzzleDatabase.cs
// Tác dụng:
// 1. Load JSON chứa danh sách puzzle
// 2. Parse thành List<PuzzleData>
// 3. Cung cấp API lấy puzzle theo ID

public class PuzzleDatabase : MonoBehaviour
{
    public static PuzzleDatabase Instance;

    [Header("JSON file chứa puzzle data")]
    public TextAsset jsonFile;

    private List<PuzzleData> puzzles = new List<PuzzleData>();

    void Awake()
    {
        // Singleton đơn giản
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);

        LoadJson();
    }

    // -----------------------------
    // LOAD JSON
    // -----------------------------
    void LoadJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("[PuzzleDatabase] JSON file chưa được gán!");
            return;
        }

        // JsonUtility chỉ parse được object → cần wrapper
        string jsonText = "{ \"puzzles\": " + jsonFile.text + " }";

        PuzzleDataList wrapper = JsonUtility.FromJson<PuzzleDataList>(jsonText);

        puzzles = wrapper.puzzles;

        Debug.Log("[PuzzleDatabase] Loaded " + puzzles.Count + " puzzles.");
    }

    // -----------------------------
    // API LẤY PUZZLE THEO ID
    // -----------------------------
    public PuzzleData GetPuzzleById(int id)
    {
        return puzzles.Find(p => p.id == id);
    }

    // API lấy puzzle random (dùng test)
    public PuzzleData GetRandomPuzzle()
    {
        if (puzzles.Count == 0) return null;
        int r = Random.Range(0, puzzles.Count);
        return puzzles[r];
    }
}
