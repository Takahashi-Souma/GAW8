
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashAnzanManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private FlashAnzanConfig config;

    [Header("UI")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text timerText; // 残り時間表示したい場合

    // セッション状態
    private int currentRound = 0;
    private int score = 0;
    private int correctCount = 0;
    private int lives = 0;

    private List<int> sequence;
    private int sequenceSum;

    private bool _answerReceived = false;
    private int _lastAnswer = 0;

    void Start()
    {
        ResetUI();
        inputField.onSubmit.AddListener(OnSubmitAnswer);
    }

    public void OnClickStart()
    {
        startButton.SetActive(false);
        StartCoroutine(GameLoop());
    }

    void ResetUI()
    {
        numberText.text = "";
        feedbackText.text = "はじめるで開始";
        scoreText.text = "Score: 0";
        lives = Mathf.Max(config.initialLives, 1);
        livesText.text = $"Lives: {lives}";
        inputField.text = "";
        inputField.interactable = false;
        if (timerText != null) timerText.text = "";
    }

    IEnumerator GameLoop()
    {
        currentRound = 0;
        score = 0;
        correctCount = 0;
        UpdateHeader();

        while (currentRound < config.rounds && lives > 0)
        {
            GenerateSequence();

            // フラッシュ表示（設定から秒数取得）
            yield return StartCoroutine(FlashSequence(config.flashTime, config.intervalTime));

            // 入力待ち
            inputField.text = "";
            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();
            feedbackText.text = "合計を入力してください";

            // 制限時間付きにする（0以下なら無制限）
            if (config.answerTimeLimit > 0f)
            {
                yield return StartCoroutine(WaitForAnswerWithTimeout(config.answerTimeLimit));
            }
            else
            {
                yield return new WaitUntil(() => _answerReceived);
            }

            // 判定
            if (_answerReceived)
            {
                CheckAnswer(_lastAnswer);
            }

            currentRound++;
            UpdateHeader();

            // ラウンド間インターバル（必要なら設定化してもOK）
            yield return new WaitForSeconds(5f);
        }

        // 結果表示（表示時間は設定から取得）
        float acc = (config.rounds > 0)
            ? (100f * correctCount / Mathf.Min(config.rounds, currentRound))
            : 0f;

        feedbackText.text = "終了！\nScore: " + score + "\n正答率: " + acc.ToString("0") + "%";
        inputField.interactable = false;

        // ★ マジックナンバー禁止：configから取得
        yield return new WaitForSeconds(config.resultDisplayTime);

        startButton.SetActive(true);
    }

    void GenerateSequence()
    {
        sequence = new List<int>(config.numbersCount);
        sequenceSum = 0;

        for (int i = 0; i < config.numbersCount; i++)
        {
            int attempts = 0;
            int val;
            do
            {
                val = Random.Range(config.minValue, config.maxValue + 1);
                attempts++;
            } while ((config.noZero && val == 0) ||
                     (!config.allowNegative && val < 0) && attempts < 50);

            sequence.Add(val);
            sequenceSum += val;
        }
    }

    IEnumerator FlashSequence(float displaySec, float intervalSec)
    {
        inputField.interactable = false;
        feedbackText.text = "";

        foreach (var n in sequence)
        {
            numberText.text = n.ToString();
            yield return new WaitForSeconds(displaySec);
            numberText.text = "";
            yield return new WaitForSeconds(intervalSec);
        }
    }

    IEnumerator WaitForAnswerWithTimeout(float timeLimitSec)
    {
        _answerReceived = false;
        float t = 0f;

        while (!_answerReceived && t < timeLimitSec)
        {
            t += Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"残り時間: {(timeLimitSec - t):0.0}s";
            }
            yield return null;
        }

        if (!_answerReceived)
        {
            feedbackText.text = "時間切れ！";
            lives--;
            inputField.interactable = false;
        }
    }

    void OnSubmitAnswer(string s)
    {
        if (!inputField.interactable) return;

        s = s.Trim().Replace("＋", "+").Replace("－", "-").Replace("−", "-");
        if (!int.TryParse(s, out var val))
        {
            feedbackText.text = "数字を入力してください";
            return;
        }

        _lastAnswer = val;
        _answerReceived = true;
        inputField.interactable = false;
    }

    void CheckAnswer(int answer)
    {
        _answerReceived = false;
        inputField.interactable = false;

        if (answer == sequenceSum)
        {
            correctCount++;
            // ★ マジックナンバー禁止：スコア算出は設定値から
            int gain = config.baseScore
                     + Mathf.RoundToInt(config.numbersCount * config.difficultyWeight / Mathf.Max(0.1f, config.flashTime));

            score += gain;
            feedbackText.text = $"正解！ +{gain}（合計:{sequenceSum}）";
        }
        else
        {
            lives--;
            feedbackText.text = $"不正解… 正:{sequenceSum} \n 入力:{answer}";
        }
    }

    void UpdateHeader()
    {
        scoreText.text = $"Score: {score}";
        livesText.text = $"Lives: {lives}";
    }
}
