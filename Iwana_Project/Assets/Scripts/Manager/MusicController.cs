using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class MusicData
{
    [Tooltip("ミュージック")]        public float MusicVolume = 1;
    [Tooltip("サウンドエフェクト")]  public float SeVolume    = 1;
}

/// <summary>
/// サウンドをコントロールする cs;
/// </summary>
public class MusicController : MonoBehaviour
{
    [Tooltip("どこでも使用可能にする.")] public static MusicController instance;

    [SerializeField, Tooltip("ステージBGM.")] AudioClip musicClip;
    [Tooltip("オーディオソース")] AudioSource audioSource;

    private float fadeSpeed = 5;
    private float unfadeSpeed = 5;
    private float maxVolume;

    [SerializeField, Range(0f, 1f), Tooltip("ステージBGM Volume.")] public float musicVolume = 1f;
    [SerializeField, Range(0f, 1f), Tooltip("SE Volume.")] public float seVolume = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        if (instance == null)
        {

            maxVolume = musicVolume;
            audioSource.volume = musicVolume;

            PlayNewClip(musicClip);

            DontDestroyOnLoad(gameObject);
            instance = this;
            gameObject.transform.parent = null;
        }
        else if (instance != this)
        {
            if (musicClip != null)
            {
                if (MusicController.instance.musicClip == null)
                    MusicController.instance.PlayNewClip(musicClip);
                else
                if (musicClip.name != MusicController.instance.audioSource.clip.name)
                    MusicController.instance.FadePlayUnfade(musicClip);
                else
                    MusicController.instance.UnFadeMusic();
            }

            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 新しいミュージックを再生.
    /// </summary>
    /// <param name="newAudioClip"> 新しいミュージック </param>
    public void PlayNewClip(AudioClip newAudioClip)
    {
        if (audioSource.clip != newAudioClip)
        {
            audioSource.Stop();
            musicClip = newAudioClip;
            audioSource.clip = newAudioClip;
            audioSource.Play();
        }
    }
    /// <summary>
    /// 再生するポイントを変更.
    /// </summary>
    /// <param name="value"> time </param>
    public void SkipToPoint(float value)
    {
        audioSource.time = value;
    }
    /// <summary>
    /// ループ再生を切り替える.
    /// </summary>
    /// <param name="isLoop"> isLoop </param>
    public void SetLoopSong(bool isLoop)
    {
        audioSource.loop = isLoop;
    }
    /****************************************************************************************************************************/
    /// <summary>
    /// SE を鳴らす.
    /// </summary>
    /// <param name="newAudioClip"> new SE Clip </param>
    /// <returns> 作成された<see cref="AudioSource"/></returns>
    public AudioSource OneShotAudio(AudioClip newAudioClip)
    {
        if(!newAudioClip) return null;
        GameObject tmpObj = new GameObject("ShotAudio");
        tmpObj.transform.position = gameObject.transform.position;

        AudioSource tmpAudio = tmpObj.AddComponent<AudioSource>();
        tmpAudio.volume = seVolume;
        tmpAudio.clip = newAudioClip;
        tmpAudio.Play();
        // Destroy 設定で クリップの最大時間をセット.
        Destroy(tmpObj, newAudioClip.length);

        return tmpAudio;
    }
/****************************************************************************************************************************/
    // ここからは 様々な効果機能.

    /// <summary>
    /// <para>上で実行されているコルーチンをすべてし再生している </para>ミュージックの音をだんだん下げる(0まで).
    /// </summary>
    public void FadeMusic()
    {
        StopAllCoroutines();
        StartCoroutine(IE_FadeMusic());
    }
    /// <summary>
    /// musicをだんだん小さくするコルーチン
    /// </summary>
    /// <returns> 無し </returns>
    IEnumerator IE_FadeMusic()
    {
        while (audioSource.volume > .1F)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0F, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        audioSource.volume = 0;
        yield break;
    }
    /// <summary>
    /// <para>上で実行されているコルーチンをすべてし再生している </para>ミュージックの音をだんだん上げる(最大 volume).
    /// </summary>
    public void UnFadeMusic()
    {
        StopAllCoroutines();
        StartCoroutine(IE_UnFadeMusic());
    }
    /// <summary>
    /// ミュージックの音をだんだん上げるコルーチン
    /// </summary>
    /// <returns> 無し </returns>
    IEnumerator IE_UnFadeMusic()
    {
        while (audioSource.volume < maxVolume - .1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume, Time.deltaTime * unfadeSpeed);
            yield return null;
        }
        audioSource.volume = maxVolume;
        yield break;
    }
    /// <summary>
    /// <para> クリップをミュージックコントローラーに追加して </para>
    /// フェードアウト、再生、フェードイン処理を実行するコルーチンを開始.
    /// </summary>
    /// <param name="newAudioClip"> クリップ </param>
    public void FadePlayUnfade(AudioClip newAudioClip)
    {
        StopAllCoroutines();
        musicClip = newAudioClip;
        StartCoroutine(IE_FadePlayUnfade());
    }

    /// <summary>
    /// フェードアウト / 再生 / フェードイン処理を実行するコルーチン
    /// </summary>
    /// <returns> 無し </returns>
    IEnumerator IE_FadePlayUnfade()
    {
        // ボリュームが一定以下になるまでフェードアウト.
        while (audioSource.volume > 0.1F)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0F, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        // 完全にボリュームを0に設定.
        audioSource.volume = 0;

        // 再生中のオーディオを停止し、新しいクリップを設定して再生開始.
        audioSource.Stop();
        audioSource.clip = musicClip;
        audioSource.Play();

        // ボリュームが最大値に近づくまでフェードイン.
        while (audioSource.volume < maxVolume - 0.1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume, Time.deltaTime * unfadeSpeed);
            yield return null;
        }
        // 最終的にボリュームを最大値に設定
        audioSource.volume = maxVolume;
    }
/****************************************************************************************************************************/
    // 余裕があったら volume保存関数を作成.


}
