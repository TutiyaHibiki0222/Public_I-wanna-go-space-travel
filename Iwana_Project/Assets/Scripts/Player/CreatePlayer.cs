
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public static CreatePlayer instance;

    [SerializeField, Tooltip("プレイヤー")] public GameObject playerPrefab;
    [SerializeField, Tooltip("スポーン先")] public Transform[] spawnPoints;

    [SerializeField] public int index;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // スポーンポイントのデフォルト設定
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[] { transform };
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }
    /// <summary>
    /// プレイヤーをスポーンするメソッド
    /// </summary>
    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefabが設定されてない");
            return;
        }

        Transform spawnPoint = GetSpawnPoint(GetAndClearNextIndex());

        Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }

    /// <summary>
    /// スポーンポイントを取得
    /// </summary>
    /// <param name="index">スポーンポイントのインデックス</param>
    /// <returns>選択されたスポーンポイント</returns>
    private Transform GetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            return spawnPoints[index];
        }
        else
        {
            Debug.LogWarning("無効なインデックスのため、最初のスポーンポイントを使用します。");
            return spawnPoints[0];
        }
    }
    /// <summary>
    /// nextIndex の番号を取得.
    /// </summary>
    /// <returns></returns>
    private int GetAndClearNextIndex()
    {
        if (PlayerPrefs.HasKey("nextIndex"))
        {
            int index = PlayerPrefs.GetInt("nextIndex");
            PlayerPrefs.DeleteKey("nextIndex");
            return index;
        }
        return 0;
    }
}
