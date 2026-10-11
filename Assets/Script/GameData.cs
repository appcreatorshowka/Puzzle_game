using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("ゲームに登場する干支の最大種類数")]
    public int etoTypeCount = 5;

    [Header("ゲーム開始時に生成する干支の数")]
    public int createEtoCount = 50;

    void Awake() // MonoBehaviourのAwakeメソッドは、スクリプトが有効化されたときに最初に呼び出される
    {
        if (instance == null) // まだインスタンスが存在しない場合
        {
            instance = this; // このインスタンスをシングルトンとして設定
            DontDestroyOnLoad(gameObject); // シーンが切り替わっても破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // すでにインスタンスが存在する場合は、このオブジェクトを破棄
        }
    }
}
