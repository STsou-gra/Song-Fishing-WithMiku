using UnityEngine;
using TMPro;

public class Notes : MonoBehaviour
{
    public TextMeshPro textMesh;

    private string targetLetter; //このノーツの文字
    private float targetTime; //叩くべき正確な時間
    private float judgeLineX; //判定ラインのX座標
    private float scrollSpeed; //流れるスピード


    public void Setup(string letter, float time, float lineX, float speed)
    {
        targetLetter = letter;
        targetTime = time;
        judgeLineX = lineX;
        scrollSpeed = speed;

        if (textMesh != null)
        {
            textMesh.text = targetLetter;
        }
    }

    public void UpdatePosition(float currentTime)
    {
        //アプローチA（時間同期型）の計算式、ターゲット時間と現在時間の差にスピードをかける
        float newX = judgeLineX + ((targetTime - currentTime) * scrollSpeed);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        //もし判定ラインを通り過ぎて画面左外
        if (newX < judgeLineX - 3.0f) //ノーツが判定ラインを大きく過ぎたら削除
        {
            Destroy(gameObject);
        }
    }

    //外部から「このノーツの文字と時間」をチェックする為の関数
    public string GetLetter() => targetLetter;
    public float GetTargetTime() => targetTime;
}
