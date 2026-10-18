using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // ★追加

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtScore;

    [SerializeField]
    private TMP_Text txtTimer;

    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore()
    {
        // 画面に表示しているスコアの値を更新
        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="time"></param>
    public void UpdateDisplayGameTime(float time) // 引数のtimeはGameData.instance.gameTimeを渡す
    {
        txtTimer.text = time.ToString("F0"); // 小数点以下を表示しないようにするためにF0を使用
    }
}
