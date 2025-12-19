
using UnityEngine;

[CreateAssetMenu(fileName = "FlashAnzanConfig", menuName = "FlashAnzan/Config")]
public class FlashAnzanConfig : ScriptableObject
{
    [Header("表示タイミング")]
    [Tooltip("各数字の表示秒数")]
    public float flashTime = 0.6f;

    [Tooltip("数字間のインターバル秒数")]
    public float intervalTime = 0.25f;

    [Tooltip("結果画面の表示秒数")]
    public float resultDisplayTime = 3f;

    [Header("入力")]
    [Tooltip("回答に与える制限時間（秒）。0以下で無制限")]
    public float answerTimeLimit = 10f;

    [Header("出題")]
    [Tooltip("1問の数字数")]
    public int numbersCount = 7;

    [Tooltip("最小値")]
    public int minValue = 1;

    [Tooltip("最大値")]
    public int maxValue = 9;

    [Tooltip("負の数を許可するか")]
    public bool allowNegative = false;

    [Tooltip("0を含めない")]
    public bool noZero = true;

    [Header("ゲーム全体")]
    [Tooltip("ラウンド数")]
    public int rounds = 10;

    [Tooltip("初期ライフ")]
    public int initialLives = 3;

    [Header("スコア")]
    [Tooltip("正解時の基礎点")]
    public int baseScore = 100;

    [Tooltip("難易度係数（numbersCountやflashTimeに掛ける倍率の重み）")]
    public float difficultyWeight = 10f;
}

