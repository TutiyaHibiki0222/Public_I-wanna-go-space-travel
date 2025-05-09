using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("対応 キー")]          KeyCode shot = KeyCode.Z;
    [SerializeField, Tooltip("弾のPrefab")]         GameObject bulletPrefab;
    [SerializeField, Tooltip("弾の速度")]           float bulletSpeed = 4f;
    [SerializeField, Tooltip("弾の最大数 0は無限")] int maxBullets = 0;
    [SerializeField, Tooltip("間隔")]               float fireRate = .1f;
    [SerializeField, Tooltip("弾を出現させる座標")] Transform bulletOffset;
    [SerializeField, Tooltip("発射 SE")]            AudioClip shotSe;
    [SerializeField, Tooltip("AutoShot")]           bool autoShot = false;
    bool isShot = true;
    private List<GameObject> bulletObjects = new List<GameObject>();
    private PlayerCharacter2DBase playerScript;
    public bool AutoFire {  get { return autoShot; } set { autoShot = value; } }
    // Start is called before the first frame update
    void Start()
    {
        isShot = true;

        playerScript = GetComponent<PlayerCharacter2DBase>();

        if (bulletOffset == null) bulletOffset = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if ((autoShot) ? Input.GetKey(shot) : Input.GetKeyDown(shot) && !playerScript.DisablePlayerShot && isShot)
        {
            isShot = false;
            Invoke("ResetFire", fireRate);
            Shoot();
        }
    }
    /// <summary>
    /// 弾を発射する処理.
    /// </summary>
    void Shoot()
    {
        GameObject clone = GetBulletClone();
        if (clone != null)
        {
            if (playerScript.FacingRight)
                clone.GetComponent<Rigidbody2D>().velocity = new Vector2( bulletSpeed, 0);
            else
                clone.GetComponent<Rigidbody2D>().velocity = new Vector2(-bulletSpeed, 0);
            if (shotSe != null)
                MusicController.instance.OneShotAudio(shotSe);
        }
    }
    /// <summary>
    /// 使われていないObjか 足りなかったら増やす.
    /// </summary>
    /// <returns> <see cref="GameObject"/> </returns>
    GameObject GetBulletClone()
    {
        // 使用されていない Objを返す.
        foreach (GameObject child in bulletObjects)
        {
            if (!child.activeInHierarchy)
            {
                child.GetComponent<WhenBulletDies>().StopAllCoroutines();
                child.transform.position = bulletOffset.transform.position;
                child.SetActive(true);
                return child;
            }
        }
        // 足りない分足す処理.
        if (bulletObjects.Count < maxBullets || maxBullets == 0)
        {
            GameObject clone;
            clone = Instantiate (bulletPrefab, bulletOffset.position, bulletOffset.rotation) as GameObject;
            clone.tag = "Bullet";

            if (!clone.GetComponent<Rigidbody2D>())
            {
                clone.AddComponent<Rigidbody2D>();
            }
            if (!clone.GetComponent<WhenBulletDies>())
            {
                clone.AddComponent<WhenBulletDies>();
            }

            bulletObjects.Add(clone);

            return clone;
        }
        return null;
    }

    void ResetFire()
    {
        isShot = true;
    }
}
