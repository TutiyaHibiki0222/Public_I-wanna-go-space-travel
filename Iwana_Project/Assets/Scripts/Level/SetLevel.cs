using GameMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetLevel : MonoBehaviour
{
    [SerializeField,Tooltip("“ïˆÕ“x")]  GameMode gameMode = GameMode.None;
    [SerializeField,Tooltip("scene–¼")] string   sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if(obj.tag == "Player")
        {
            SaveManager save = SaveManager.Instance;
            if (save)
            {
                save.openSaveConfig.gameMode = gameMode;

                save.Save();

            }

            SceneManager.LoadScene(sceneName);
        }

    }
}
