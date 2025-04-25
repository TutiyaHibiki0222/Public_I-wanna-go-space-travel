using Game;
using Save;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BloodEffect : MonoBehaviour
{
    [SerializeField, Tooltip("���̃v���n�u")] GameObject bloodPlafab;
    [SerializeField, Tooltip("���̔�юU���")] int num;
    [SerializeField, Tooltip("���̍L�����")] float velocity;
    private void Update()
    {

#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
#endif
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
    void ResetGame()
    {
        GameData gameSave = GameData.FromJson(SaveManager.Instance.openSaveConfig.gameData);
        SceneManager.LoadScene(gameSave.mapData);
    }

    public void SpawnBlood(Vector3 pos)
    {
        StartCoroutine(IESpawnBlood(pos));
       /* 
        for (int i = 0; i < num; i++)
        {
            // ����Plafab�쐬.
            GameObject bllood = Instantiate(bloodPlafab, pos, Random.rotation);
            Rigidbody2D rb = bllood.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 ve = new Vector2(Random.Range(-velocity, velocity), Random.Range(0, velocity));
                // �͂�������.
                rb.AddForce(ve);
            }
        }
       */
    }

    IEnumerator IESpawnBlood(Vector3 pos)
    {
        int count = 0;

        while (count < num)
        {
            int random = Random.Range((num - count) / 9, num - count);
            for (int i = 0; i < random; i++)
            {
                count++;
                Quaternion r = Random.rotation;
                // ����Plafab�쐬.
                GameObject bllood = Instantiate(bloodPlafab, pos,new Quaternion(r.x,r.y, 0,0));
                bllood.transform.SetParent(transform, false);
                bllood.transform.position = pos;
                Rigidbody2D rb = bllood.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 ve = new Vector2(Random.Range(-velocity, velocity), Random.Range(-1, velocity));
                    // �͂�������.
                    rb.AddForce(ve);
                }
            }

            if(count < num)
            {
                yield return null;
            }
        }

        yield break;
    }

}
