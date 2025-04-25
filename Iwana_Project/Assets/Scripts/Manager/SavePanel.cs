using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Save;
using UnityEngine.UI;
using System.IO;

public class SavePanel : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] GameObject Film;
    [SerializeField] SaveConfig saveConfig;
    [SerializeField] TMP_Text tmText;
    [SerializeField] TMP_Text gameClearText;
    public void SetFilm(bool flg)
    {
        Film.SetActive(flg);
    }
    public void SetText(string text)
    {
        tmText.text = text;
    }
    public void SetSaveConfig(SaveConfig set)
    {
        saveConfig = set;
    }


    // Start is called before the first frame update
    public void Init()
    {
        Debug.Log($"SaveConfig: {saveConfig}");
        Debug.Log($"GameComplete: {saveConfig.gameComplete}");
        gameClearText.gameObject.SetActive(saveConfig.gameComplete);


        string file = Path.Combine(Application.persistentDataPath, "save_screenshot" + saveConfig.listNo + ".png");
        if(File.Exists(file))
        {
            byte[] data = File.ReadAllBytes(file);

            Texture2D texture = new Texture2D(800, 608);
            texture.LoadImage(data);
            img.sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height),new Vector2(0.5f,0.5f));
        }
        else
        {
            img.sprite = Resources.Load<Sprite>("img/NewGame");
        }
        string txt = 
           "GameMode: " + saveConfig.gameMode + "\n" +
           "PlayTime: " + saveConfig.CreateTextTimer() + "\n" +
           "DeathCount: " + saveConfig.deleteCount.ToString("");
        SetText(txt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteSave()
    {
        string file = Path.Combine(Application.persistentDataPath, "save_screenshot" + saveConfig.listNo + ".png");
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }
}
