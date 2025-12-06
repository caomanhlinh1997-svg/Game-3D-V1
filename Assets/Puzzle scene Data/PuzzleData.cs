using System;
using System.Collections.Generic;

// PuzzleData.cs
// Lưu dữ liệu của 1 câu puzzle, khớp với cấu trúc JSON

[Serializable]
public class PuzzleData
{
    public int id;                    // ID của puzzle
    public string sentence;           // Câu đầy đủ
    public List<string> pieces;       // Các mảnh chữ
}

// Wrapper để Unity parse được List bằng JsonUtility
[Serializable]
public class PuzzleDataList
{
    public List<PuzzleData> puzzles;
}
