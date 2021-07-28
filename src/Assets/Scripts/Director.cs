using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{

    const float A4_WIDTH = 210.0f;
    const float A4_HEIGHT = 297.0f;

    [Header("紙の大きさ")]
    [Range(0, 10)]
    public float scaling = 2.0f;

    [Header("瞳孔間距離")]
    [Range(0, 10)]
    public float Pupillary_Distance = 2.0f;

    [Header("左右のメガネの色")]
    [SerializeField] private Color _colorL = Color.red;
    [SerializeField] private Color _colorR = Color.cyan;
    [Header("モノラル処理時のパラメータ")]
    [Range(0, 10), Tooltip("白黒の閾値")]
    public float Threshold = 0.4f;
    [Range(0, 100), Tooltip("白黒の濃さ")]
    public float Tension = 30.0f;

    [Header("アウトレット接続")]
    [SerializeField]
    private GameObject gameObject_Paper;
    [SerializeField]
    private Camera gameObject_MainCamera;
    [SerializeField]
    private Camera gameObject_LeftCamera;
    [SerializeField]
    private Camera gameObject_RightCamera;
    [SerializeField]
    private Material anaglyph_material;

    // Start is called before the first frame update
    void Start()
    {
        OnValidate();
    }

    // Update is called once per frame
    void Update()
    {
        // カメラの向きが変わったら、紙の位置を変える
        Ray ray = gameObject_MainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            gameObject_Paper.transform.position = hit.point + Vector3.up * 0.001f;
        }
    }

    // インスペクタ―の更新が起きたら
    void OnValidate()
    {
        // 紙の大きさを変える
        const float coeff = 0.1f;// メッシュのサイズ分、小さく指定
        gameObject_Paper.transform.localScale = new Vector3(
            coeff * getWorldA4Width(),
            1.0f,
            coeff * getWorldA4Height());

        // カメラのスケールから左右のカメラの位置を変える
        float s = scaling * Pupillary_Distance * 0.5f;
        gameObject_LeftCamera.transform.localPosition = Vector3.left * s;
        gameObject_RightCamera.transform.localPosition = Vector3.right * s;

        if (anaglyph_material.HasProperty("_ColorL"))
        {
            anaglyph_material.SetColor("_ColorL", _colorL);
        }

        if (anaglyph_material.HasProperty("_ColorR"))
        {
            anaglyph_material.SetColor("_ColorR", _colorR);
        }
        if (anaglyph_material.HasProperty("_Threshold"))
        {
            anaglyph_material.SetFloat("_Threshold", Threshold);
        }
        if (anaglyph_material.HasProperty("_Tension"))
        {
            anaglyph_material.SetFloat("_Tension", Tension);
        }
    }

    // サービス関数
    public float getA4Width()
    {
        return A4_WIDTH;
    }

    public float getA4Height()
    {
        return A4_HEIGHT;
    }

    public float getPupillaryDistance()
    {
        return Pupillary_Distance;
    }

    public float getWorldA4Width()
    {
        return 0.1f * scaling * A4_WIDTH;
    }

    public float getWorldA4Height()
    {
        return 0.1f * scaling * A4_HEIGHT;
    }

    public Vector3 getWorldA4Center()
    {
        Vector3 c = gameObject_Paper.transform.position;
        c.y = 0.0f;
        return c;
    }
}
