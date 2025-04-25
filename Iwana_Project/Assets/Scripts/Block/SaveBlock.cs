using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Save;
using UnityEngine.SceneManagement;
public class SaveBlock : MonoBehaviour
{
    [SerializeField, Tooltip("Save クールタイム")]    float saveBuffer = 1f;
    [SerializeField, Tooltip("Targetになるタグ名")]   List<string> tagName;
    [SerializeField, Tooltip("セーブ時のサウンド")]   AudioClip saveSe;
    [SerializeField, Tooltip("セーブ専用エフェクト")] GameObject effect;
    SaveManager saveManager;
    Animator anim;
    private bool canSave;
    private GameObject effectClone;
    // Start is called before the first frame update
    void Start()
    {
        saveManager = SaveManager.Instance;
        anim = GetComponent<Animator>();
        canSave = true;
    }

    void Saving()
    {
        anim.SetBool("IsActive", true);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && saveManager != null)
        {
            GameData gameSave = new GameData();
            gameSave.position = player.transform.position;
            gameSave.mapData = SceneManager.GetActiveScene().name;
            saveManager.openSaveConfig.gameData = gameSave.ToJson();
            saveManager.ScreenShot_OR_Save();
        }

        if(effect != null)
            effectClone = Instantiate(effect,transform.position,Quaternion.identity);

        if (saveSe != null)
        {
            MusicController.instance.OneShotAudio(saveSe);
        }

        canSave = false;
        StartCoroutine(ResetSave());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && canSave)
        {
            foreach (string tagName in tagName)
            {
                if (other.tag == tagName)
                {
                    Saving();
                    break;
                }
            }
        }
    }

    IEnumerator ResetSave()
    {
        yield return new WaitForSeconds(saveBuffer);
        canSave = true;
        anim.SetBool("IsActive", false);
    }

}
