using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[System.Serializable]
public struct SongData
{
    public string letter;
    public float time;
}

[System.Serializable]
public class TextAliveJsonWrapper
{
    public List<SongData> data;
}

public class GameManager : MonoBehaviour
{
    // jslibの関数をインポート
    [DllImport("__Internal")]
    private static extern void PlaySongJS();
    [DllImport("__Internal")]
    private static extern void PauseSongJS();

    [Header("じゅんび")]
    public GameObject notePrefab;
    public Transform spawnPoint;
    public Transform judgeLine;

    [Header("ゲーム設定")]
    public float scrollSpeed = 5.0f;
    public float judgeWindow = 0.15f;

    public List<SongData> songDataList = new List<SongData>();
    private List<Notes> activeNotes = new List<Notes>();

    private float textAliveTime = 0.0f;
    private int spawnIndex = 0;
    private bool isSongReady = false;

    // ★ JavaScriptから歌詞データを受け取る
    public void SetSongDataFromJS(string jsonText)
    {
        TextAliveJsonWrapper wrapper = JsonUtility.FromJson<TextAliveJsonWrapper>(jsonText);
        songDataList = wrapper.data;
        spawnIndex = 0;
        isSongReady = true;
        Debug.Log($"TextAliveから {songDataList.Count} 文字の歌詞を受信！曲を再生します。");

        // データが届いたら曲を再生する
#if !UNITY_EDITOR && UNITY_WEBGL
        PlaySongJS();
#endif
    }

    // ★ JavaScriptから現在の再生時間（秒）を受け取る
    public void UpdateCurrentTimeFromJS(float timeFromJS)
    {
        textAliveTime = timeFromJS;
    }

    void Update()
    {
        if (!isSongReady) return;

        // 1. タイミングが来たら画面右端にノーツを生成（ターゲットの2秒前）
        if (spawnIndex < songDataList.Count && textAliveTime >= songDataList[spawnIndex].time - 2.0f)
        {
            SpawnNote(songDataList[spawnIndex]);
            spawnIndex++;
        }

        // 2. 全ノーツの位置を更新
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] != null)
            {
                activeNotes[i].UpdatePosition(textAliveTime);
            }
            else
            {
                activeNotes.RemoveAt(i);
            }
        }

        // 3. 入力判定
        CheckInput();
    }

    void SpawnNote(SongData data)
    {
        GameObject newObj = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        Notes note = newObj.GetComponent<Notes>();
        note.Setup(data.letter, data.time, judgeLine.position.x, scrollSpeed);
        activeNotes.Add(note);
    }

    void CheckInput()
    {
        if (Input.anyKeyDown)
        {
            if (activeNotes.Count > 0 && activeNotes[0] != null)
            {
                Notes targetNote = activeNotes[0];
                float timeDiff = Mathf.Abs(textAliveTime - targetNote.GetTargetTime());

                // タイミングが合っていればノーツを消去
                if (timeDiff <= judgeWindow)
                {
                    Debug.Log($"✨ PERFECT! [{targetNote.GetLetter()}] を叩いた！");
                    activeNotes.RemoveAt(0);
                    Destroy(targetNote.gameObject);
                }
            }
        }
    }
}