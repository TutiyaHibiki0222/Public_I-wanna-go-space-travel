using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("�Ή� �L�[")]          KeyCode shot = KeyCode.Z;
    [SerializeField, Tooltip("�e��Prefab")]         GameObject bulletPrefab;
    [SerializeField, Tooltip("�e�̑��x")]           float bulletSpeed = 4f;
    [SerializeField, Tooltip("�e�̍ő吔 0�͖���")] int maxBullets = 0;
    [SerializeField, Tooltip("�Ԋu")]               float fireRate = .1f;
    [SerializeField, Tooltip("�e���o����������W")] Transform bulletOffset;
    [SerializeField, Tooltip("���� SE")]            AudioClip shotSe;
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
    /// �e�𔭎˂��鏈��.
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
    /// �g���Ă��Ȃ�Obj�� ����Ȃ������瑝�₷.
    /// </summary>
    /// <returns> <see cref="GameObject"/> </returns>
    GameObject GetBulletClone()
    {
        // �g�p����Ă��Ȃ� Obj��Ԃ�.
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
        // ����Ȃ�����������.
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
