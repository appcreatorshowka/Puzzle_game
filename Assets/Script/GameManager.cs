using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("干支のプレファブ")]
    public Eto etoPrefab;          

    [Header("干支の生成位置")]
    public Transform etoSetTran;

    [Header("干支生成時の最大回転角度")]
    public float maxRotateAngle = 35.0f;

    [SerializeField, Header("干支生成時の左右のランダム幅")]
    private float maxRange = 400.0f;

    [SerializeField, Header("干支生成時の落下位置")]
    private float fallPos = 1400.0f;

    [SerializeField, Header("生成された干支のリスト")]
    private List<Eto> etoList = new List<Eto>();

    [SerializeField, Header("干支の画像データ")]
    private Sprite[] etoSprites;

    // 最初にドラッグした干支の情報
    private Eto firstSelectEto;

    // 最後にドラッグした干支の情報
    private Eto lastSelectEto;

    // 最初にドラッグした干支の種類
    private EtoType? currentEtoType;

    [SerializeField, Header("削除対象となる干支を登録するリスト")]
    private List<Eto> eraseEtoList = new List<Eto>();

    [SerializeField, Header("つながっている干支の数")]
    private int linkCount = 0;


    IEnumerator Start()
    {   // <=  ☆　戻り値を void から IEnumerator型に変更して、コルーチンメソッドにする

        // 干支の画像を読みこむ。この処理が終了するまで、次の処理へはいかないようにする
        yield return StartCoroutine(LoadEtoSprites());

        // 引数で指定した数の干支を生成する
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    /// <summary>
    /// 干支の画像を読み込んで配列から使用できるようにする
    /// </summary>
    private IEnumerator LoadEtoSprites()
    {
        // 配列の初期化(12個の画像が入るようにSprite型の配列を12個用意する)
        // etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAllを行い、分割されている干支の画像を順番にすべて読み込んで配列に代入
        // etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ※　１つのファイルを１２分割していない場合には、以下の処理を使います。12分割している場合には使用しません。
        // for(int i = 0; i < etoSprites.Length; i++){
        //      etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        // }

        yield break;
    }

    /// <summary>
    /// 干支を生成
    /// </summary>
    /// <param name="count">生成する数</param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 干支プレファブのクローンを、干支の生成位置に生成
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);　　　　// <= ☆　変更。　生成された干支を代入する型をGameObject型からEto型に変更する

            // 生成された干支の回転情報を設定(色々な角度になるように)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            // 生成位置をランダムにして落下位置を変化させる
            eto.transform.localPosition = new Vector2(Random.Range(-maxRange, maxRange), fallPos);

            // ランダムな干支を2種類の中から１つ選択
            int randomValue = Random.Range(0, etoSprites.Length);       // ← 第2引数については配列の仕組みが分かっている場合には、etoSprites.Length のように書いて制限しても問題ありません。

            // 生成された干支の初期設定(干支の種類と干支の画像を引数を使ってEtoへ渡す)
            eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);

            // etoListに追加
            etoList.Add(eto);

            // 0.03秒待って次の干支を生成
            yield return new WaitForSeconds(0.03f);
        }
    }

    void Update()
    {
        // Mouse.current → Pointer.current へ変更
        if (Pointer.current == null)
        {
            return;
        }
        // Mouse.current → Pointer.current へ変更
        if (Pointer.current.press.wasPressedThisFrame && firstSelectEto == null)
        {
            // 干支を最初にドラッグした際の処理
            OnStartDrag();
        }
        else if (Pointer.current.press.isPressed && firstSelectEto != null)
        {
            // 干支のドラッグ（スワイプ）中の処理      
            OnDragging();
        }
    }
    /// <summary>
    /// 干支を最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag()
    {
        // 変数名を mousePosition から pointerPosition に変更。Mouse.current → Pointer.current へ変更
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        // 変数名を mousePosition から pointerPosition に変更
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPosition), Vector2.zero);

        // 以下は変更なし
    }
    /// <summary>
    /// 干支のドラッグ（スワイプ）中処理
    /// </summary>
    private void OnDragging()
    {
        // 変数名を mousePosition から pointerPosition に変更。Mouse.current → Pointer.current へ変更
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        // 変数名を mousePosition から pointerPosition に変更
        // OnStartDragメソッドと同じ処理で、指の位置をワールド座標に変換しRayを発射し、その位置にあるコライダーを持つオブジェクトを取得してhit変数へ代入
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPosition), Vector2.zero);

        // 以下は変更なし
    }

    /// <summary>
    /// 選択された干支を削除リストに追加
    /// </summary>
    /// <param name="dragEto"></param>
    private void AddEraseEtoList(Eto dragEto)
    {
        // 削除リストにドラッグ中の干支を追加
        eraseEtoList.Add(dragEto);

        // ドラッグ中の干支のアルファ値を0.5fにする(半透明にすることで、選択中であることをユーザーに伝える)
        ChangeEtoAlpha(dragEto, 0.5f);
    }

    /// <summary>
    /// 干支のアルファ値を変更
    /// </summary>
    /// <param name="dragEto"></param>
    /// <param name="alphaValue"></param>
    private void ChangeEtoAlpha(Eto dragEto, float alphaValue)
    {
        // 現在ドラッグしている干支のアルファ値を変更
        dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
    }
}