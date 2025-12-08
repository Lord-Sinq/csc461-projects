using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public enum HitType { Perfect, Good, Miss }

    [Header("Scoring values")]
    public int perfectScore = 100;
    public int goodScore = 50;
    public int missScore = 0;

    [Header("Runtime state")]
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }

    [Header("UI (optional)")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

    [Header("Persistence")]
    public string highScoreKey = "HighScore";

    public event Action<int> ScoreChanged;
    public event Action<int> ComboChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // If no UI assigned, create a simple world-space HUD attached to the main camera so you can see the score by default.
        if (scoreText == null)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                GameObject hud = new GameObject("ScoreHUD");
                hud.transform.parent = cam.transform;
                hud.transform.localRotation = Quaternion.identity;
                hud.transform.localPosition = new Vector3(0.5f, -0.45f, 2f); // adjust so it's visible in front of camera
                var tmp = hud.AddComponent<TextMeshPro>();
                tmp.fontSize = 36;
                tmp.alignment = TextAlignmentOptions.TopLeft;
                scoreText = tmp;
            }
        }

        RefreshUI();
    }

    public void ResetAll()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        RefreshUI();
    }

    public void RegisterHit(HitType hit)
    {
        int delta = hit switch
        {
            HitType.Perfect => perfectScore,
            HitType.Good => goodScore,
            HitType.Miss => missScore,
            _ => 0
        };

        if (hit == HitType.Miss)
        {
            Combo = 0;
        }
        else
        {
            Combo++;
            if (Combo > MaxCombo) MaxCombo = Combo;
        }

        Score += delta;
        ScoreChanged?.Invoke(Score);
        ComboChanged?.Invoke(Combo);

        RefreshUI();
    }

    void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";

        if (comboText != null)
            comboText.text = Combo > 0 ? $"Combo: {Combo}" : "";
    }

    public void SaveHighScore()
    {
        int prev = PlayerPrefs.GetInt(highScoreKey, 0);
        if (Score > prev)
        {
            PlayerPrefs.SetInt(highScoreKey, Score);
            PlayerPrefs.Save();
        }
    }

    public int LoadHighScore()
    {
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }
}
