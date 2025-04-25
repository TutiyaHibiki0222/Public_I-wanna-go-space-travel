using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("ëŒâû ÉLÅ[")]          KeyCode shot = KeyCode.Z;
    [SerializeField, Tooltip("íeÇÃPrefab")]         GameObject bulletPrefab;
    [SerializeField, Tooltip("íeÇÃë¨ìx")]           float bulletSpeed = 4f;
    [SerializeField, Tooltip("íeÇÃç≈ëÂêî 0ÇÕñ≥å¿")] int maxBullets = 0;
    [SerializeField, Tooltip("ä‘äu")]               float fireRate = .1f;
    [SerializeField, Tooltip("íeÇèoåªÇ≥ÇπÇÈç¿ïW")] Transform bulletOffset;
    [SerializeField, Tooltip("î≠éÀ SE")]            AudioClip shotSe;
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
    /// íeÇî≠éÀÇ∑ÇÈèàóù.
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
    /// égÇÌÇÍÇƒÇ¢Ç»Ç¢ObjÇ© ë´ÇËÇ»Ç©Ç¡ÇΩÇÁëùÇ‚Ç∑.
    /// </summary>
    /// <returns> <see cref="GameObject"/> </returns>
    GameObject GetBulletClone()
    {
        // égópÇ≥ÇÍÇƒÇ¢Ç»Ç¢ ObjÇï‘Ç∑.
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
        // ë´ÇËÇ»Ç¢ï™ë´Ç∑èàóù.
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
