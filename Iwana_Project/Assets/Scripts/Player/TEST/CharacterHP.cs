using Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHP : MonoBehaviour
{
    [SerializeField, Tooltip("最大HP")]       public int maxHp = 1;
    [SerializeField, Tooltip("ダメージ遅延")] public float damageDelay = 1f;

    [SerializeField, Tooltip("Damage Se")]   AudioClip hitSound;
    [SerializeField, Tooltip("Damage Se")]   AudioClip deathSe;
    [SerializeField, Tooltip("GameOver Se")] AudioClip deathMusic;

    private int hp;
    [Tooltip("Damageを食らうかどうか")] bool isDamage;
    private void Start()
    {
        hp = maxHp;
        StartCoroutine(DamageCT(0.2f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Div();
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && isDamage)
        {
            isDamage = false;
            StartCoroutine(DamageCT(damageDelay));

            hp--;
            if(hp <= 0) {
                Div();
            }
            else
            {
                if (hitSound)
                    MusicController.instance.OneShotAudio(hitSound);
            }
        }
    }
    /// <summary>
    /// 死亡処理.
    /// </summary>
    void Div()
    {

        if(deathSe)
            MusicController.instance.OneShotAudio(deathSe);

        if (deathMusic != null)
        {
            MusicController.instance.FadeMusic();
            MusicController.instance.OneShotAudio(deathMusic);
        }

        FindObjectOfType<GameManager>().SetGameState(Game.GameState.GAME_OVER);
        gameObject.SetActive(false);
        FindObjectOfType<BloodEffect>().SpawnBlood(transform.position);

        if(SaveManager.Instance) SaveManager.Instance.openSaveConfig.deleteCount++;
    }

    IEnumerator DamageCT(float d)
    {
        yield return new WaitForSeconds(d);
        isDamage = true;
    }
}
