using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eto : MonoBehaviour
{
    [Header("干支の種類")]
    public EtoType etoType;

    [Header("干支のイメージ変更用")]
    public Image imgEto;

    [Header("スワイプされた干支である判定。trueの場合、この干支は削除対象となる")]
    public bool isSelected;

    [Header("スワイプされた通し番号。スワイプされた順番が代入される")]
    public int num;

    /// <summary>
    /// 干支の初期設定
    /// </summary>
    public void SetUpEto(EtoType etoType, Sprite sprite) // 干支の種類と干支のイメージを引数で受け取る
    {
        // 干支の種類を設定
        this.etoType = etoType;

        // 干支の名前を、設定した干支の種類の名前に変更
        name = this.etoType.ToString(); ;

        // 引数で届いた干支のイメージに合わせてメージを変更
        ChangeEtoImage(sprite);
    }

    /// <summary>
    /// 干支のイメージを変更
    /// </summary>
    /// <param name="changeSprite">干支のイメージ</param>
    public void ChangeEtoImage(Sprite changeSprite) // 干支のイメージを引数で受け取る
    {
        imgEto.sprite = changeSprite; // 干支のイメージを変更
        Debug.Log(changeSprite);
    }
}
