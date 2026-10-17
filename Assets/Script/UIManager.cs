using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // ★追加

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtScore;

    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore()
    {
        // 画面に表示しているスコアの値を更新
        txtScore.text = GameData.instance.score.ToString();
    }
}
