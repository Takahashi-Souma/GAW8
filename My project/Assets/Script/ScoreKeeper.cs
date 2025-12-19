
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    const string KEY_HIGH = "FLASH_HIGH_SCORE";

    public int GetHighScore() => PlayerPrefs.GetInt(KEY_HIGH, 0);

    public void TryUpdateHighScore(int current)
    {
        int best = GetHighScore();
        if (current > best)
        {
            PlayerPrefs.SetInt(KEY_HIGH, current);
            PlayerPrefs.Save();
        }
    }
}