
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public static CreatePlayer instance;

    [SerializeField, Tooltip("�v���C���[")] public GameObject playerPrefab;
    [SerializeField, Tooltip("�X�|�[����")] public Transform[] spawnPoints;

    [SerializeField] public int index;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // �X�|�[���|�C���g�̃f�t�H���g�ݒ�
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
    /// �v���C���[���X�|�[�����郁�\�b�h
    /// </summary>
    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab���ݒ肳��ĂȂ�");
            return;
        }

        Transform spawnPoint = GetSpawnPoint(GetAndClearNextIndex());

        Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }

    /// <summary>
    /// �X�|�[���|�C���g���擾
    /// </summary>
    /// <param name="index">�X�|�[���|�C���g�̃C���f�b�N�X</param>
    /// <returns>�I�����ꂽ�X�|�[���|�C���g</returns>
    private Transform GetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            return spawnPoints[index];
        }
        else
        {
            Debug.LogWarning("�����ȃC���f�b�N�X�̂��߁A�ŏ��̃X�|�[���|�C���g���g�p���܂��B");
            return spawnPoints[0];
        }
    }
    /// <summary>
    /// nextIndex �̔ԍ����擾.
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
