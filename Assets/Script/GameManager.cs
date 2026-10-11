using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("干支のプレファブ")]
    public Eto etoPrefab;            // <=　☆　宣言する型を GameObject型 から Eto型に変更。EtoPrefabは前と同じようにProject内からアサインできる

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
        for(int i = 0; i < etoSprites.Length; i++) // 12種類の干支の画像を順番に読み込む
        { 
        //    etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i); // 画像の名前が「eto_0」「eto_1」・・・「eto_11」となっている場合
        }

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

            // ランダムな干支を12種類の中から１つ選択
            int randomValue = Random.Range(0, (int)EtoType.Count);

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
        // 干支をつなげる処理
        // if (Input.GetMouseButtonDown(0) && firstSelectEto == null) // <= ☆　変更。　最初にドラッグした干支がnullの場合のみ、干支をつなげる処理を行う

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && firstSelectEto == null)
        {
            // 干支を最初にドラッグした際の処理
            OnStartDrag();
        }
    }

    private void OnStartDrag()
    {
        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(mousePosition),
            Vector2.zero
        );

        linkCount = 0;

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            {
                firstSelectEto = dragEto;
                lastSelectEto = dragEto;
                currentEtoType = dragEto.etoType;
                dragEto.isSelected = true;
                dragEto.num = linkCount;

                eraseEtoList = new List<Eto>();
                AddEraseEtoList(dragEto);
            }
        }
    }

    /// <summary>
    /// 干支を最初にドラッグした際の処理
    /// </summary>
    // private void OnStartDrag()
    // {
        // 画面をタップした際の位置情報を、CameraクラスのScreenToWorldPointメソッドを利用してCanvas上の座標に変換
       //  RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 干支がつながっている数を初期化
        // linkCount = 0;

        // 変換した座標のコライダーを持つゲームオブジェクトがあるか確認
        // if (hit.collider != null)
        // {
            // ゲームオブジェクトがあった場合、そのゲームオブジェクトがEtoクラスを持っているかどうか確認		
            // if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            // {
                // Etoクラスを持っていた場合には、以下の処理を行う

                // 最初にドラッグした干支の情報を変数に代入
                // firstSelectEto = dragEto;

                // 最後にドラッグした干支の情報を変数に代入(最初のドラッグなので、最後のドラッグも同じ干支)
                // lastSelectEto = dragEto;

                // 最初にドラッグしている干支の種類を代入 = 後ほど、この情報を使ってつながる干支かどうかを判別する
                // currentEtoType = dragEto.etoType;

                // 干支の状態が「選択中」であると更新
                // dragEto.isSelected = true;

                // 干支に何番目に選択されているのか、通し番号を登録
                // dragEto.num = linkCount;

                // 削除する対象の干支を登録するリストを初期化
                // eraseEtoList = new List<Eto>();

                // ドラッグ中の干支を削除の対象としてリストに登録
                // AddEraseEtoList(dragEto);
   //          }
   //      }
   //  }

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