using System.Collections;
using UnityEngine;
using Game;
using Save;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField,Tooltip("Game状態")] GameState state;
    SaveConfig open;
    ScreenManager screenManager;
    private PlayerCharacter2DBase character;

    public void SetGameState(GameState state)
    {
        this.state = state;
    }

    private void Start()
    {
        state = GameState.GAME_LOAD;
        
        StartCoroutine(GameStep());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HandleGameLoad()
    {
        Debug.Log("ゲームデータのロードを開始します。");

        // セーブデータの取得
        open = GetSaveConfig();
        if (open == null)
        {
            Debug.LogError("セーブデータが存在しません。新規データを作成します。");
            open = new SaveConfig();
            GameData newGameData = new GameData
            {
                mapData = SceneManager.GetActiveScene().name
            };
            open.gameData = newGameData.ToJson();
        }
        else
        {
            print("セーブデータをゲーム中 に反映.");

            if(string.IsNullOrEmpty(open.gameData))
            {
                print("初めてのオープン.");
                // 初期化時に現在のシーン名を設定
                GameData newGame = new GameData();
                newGame.mapData  = SceneManager.GetActiveScene().name;
                newGame.position = CreatePlayer.instance.spawnPoints[0].position;
                open.gameData = newGame.ToJson();
                print($"{open.gameData}");
                SaveManager.Instance.ScreenShot_OR_Save();
            }
            else
            {
                GameData savedData = GameData.FromJson(open.gameData);
                string currentMap = SceneManager.GetActiveScene().name;
                if (currentMap == savedData.mapData && CreatePlayer.instance && !PlayerPrefs.HasKey("nextIndex"))
                {
                    CreatePlayer.instance.spawnPoints[0].position = savedData.position + new Vector3(0, 0.01f, 0);
                }
            }
            

        }

        screenManager = FindFirstObjectByType<ScreenManager>();
        if (CreatePlayer.instance) CreatePlayer.instance.SpawnPlayer();

        state = GameState.GAME_INIT;
        Debug.Log("ゲームデータのロードが完了しました。");
    }

    private SaveConfig GetSaveConfig()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManagerが見つかりません。");
            return null;
        }

        SaveConfig config = SaveManager.Instance.openSaveConfig;
        if (config == null)
        {
            Debug.LogError("SaveConfigがnullです。");
        }
        return config;
    }

    private void HandleGameInit()
    {
        Debug.Log("ゲームの初期化を開始します。");

        if (!screenManager)
        {
            Debug.LogError("ScreenManagerが見つかりません。");
            return;
        }

        // 画面要素の初期化
        screenManager.gameOver.SetActive(false);
        screenManager.GetPose.SetActive(false);

        // プレイヤーキャラクターの取得
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            character = player.GetComponent<PlayerCharacter2DBase>();
        }
        else
        {
            Debug.LogError("プレイヤーキャラクターが見つかりません。");
        }

        state = GameState.GAME_START;
        Debug.Log("ゲームの初期化が完了しました。");
    }

    private void HandleGameStart()
    {
        // プレイ中の処理（時間の更新など）
        UpdateTime();

        // ポーズ切り替え
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPose(true);
        }
    }

    void UpdateTime()
    {
        if (open != null) open.time += Time.deltaTime;
    }
    private void HandleGameStop()
    {
        // ポーズ解除
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPose(false);
        }
    }

    public void SetPose(bool set)
    {
        state = (set) ? GameState.GAME_STOP : GameState.GAME_START;
        screenManager.GetPose.SetActive(set);
        character.DisablePlayerMove = set;
        character.DisablePlayerJump = set;
        character.DisablePlayerShot = set;
    }

    private void HandleGameOver()
    {
        Debug.Log("ゲームオーバー処理を開始します。");

        // ゲームオーバー画面の表示
        if (screenManager.GetPose)
            screenManager.GetPose.SetActive(false);

        state = GameState.GAME_READY;
        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(0.3f);

        if (!screenManager.gameOver.isStatic)
        {
            screenManager.gameOver.SetActive(true);
        }
        
        Debug.Log("ゲームオーバー画面が表示されました。");
    }



    IEnumerator GameStep()
    {
        while (true)
        {
            switch (state)
            {
                case GameState.GAME_LOAD:
                    HandleGameLoad();
                    break;
                case GameState.GAME_INIT:
                    HandleGameInit();
                    break;
                case GameState.GAME_START:
                    HandleGameStart();
                    break;
                case GameState.GAME_STOP:
                    HandleGameStop();
                    break;
                case GameState.GAME_OVER:
                    HandleGameOver();
                    break;
                case GameState.GAME_READY:

                    break;
                case GameState.GAME_END:

                    break;
            }
            yield return null;
        }
    }
}
