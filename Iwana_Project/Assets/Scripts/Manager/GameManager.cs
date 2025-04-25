using System.Collections;
using UnityEngine;
using Game;
using Save;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField,Tooltip("Game���")] GameState state;
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
        Debug.Log("�Q�[���f�[�^�̃��[�h���J�n���܂��B");

        // �Z�[�u�f�[�^�̎擾
        open = GetSaveConfig();
        if (open == null)
        {
            Debug.LogError("�Z�[�u�f�[�^�����݂��܂���B�V�K�f�[�^���쐬���܂��B");
            open = new SaveConfig();
            GameData newGameData = new GameData
            {
                mapData = SceneManager.GetActiveScene().name
            };
            open.gameData = newGameData.ToJson();
        }
        else
        {
            print("�Z�[�u�f�[�^���Q�[���� �ɔ��f.");

            if(string.IsNullOrEmpty(open.gameData))
            {
                print("���߂ẴI�[�v��.");
                // ���������Ɍ��݂̃V�[������ݒ�
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
        Debug.Log("�Q�[���f�[�^�̃��[�h���������܂����B");
    }

    private SaveConfig GetSaveConfig()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager��������܂���B");
            return null;
        }

        SaveConfig config = SaveManager.Instance.openSaveConfig;
        if (config == null)
        {
            Debug.LogError("SaveConfig��null�ł��B");
        }
        return config;
    }

    private void HandleGameInit()
    {
        Debug.Log("�Q�[���̏��������J�n���܂��B");

        if (!screenManager)
        {
            Debug.LogError("ScreenManager��������܂���B");
            return;
        }

        // ��ʗv�f�̏�����
        screenManager.gameOver.SetActive(false);
        screenManager.GetPose.SetActive(false);

        // �v���C���[�L�����N�^�[�̎擾
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            character = player.GetComponent<PlayerCharacter2DBase>();
        }
        else
        {
            Debug.LogError("�v���C���[�L�����N�^�[��������܂���B");
        }

        state = GameState.GAME_START;
        Debug.Log("�Q�[���̏��������������܂����B");
    }

    private void HandleGameStart()
    {
        // �v���C���̏����i���Ԃ̍X�V�Ȃǁj
        UpdateTime();

        // �|�[�Y�؂�ւ�
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
        // �|�[�Y����
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
        Debug.Log("�Q�[���I�[�o�[�������J�n���܂��B");

        // �Q�[���I�[�o�[��ʂ̕\��
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
        
        Debug.Log("�Q�[���I�[�o�[��ʂ��\������܂����B");
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
