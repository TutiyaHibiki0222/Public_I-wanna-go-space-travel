using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class MusicData
{
    [Tooltip("�~���[�W�b�N")]        public float MusicVolume = 1;
    [Tooltip("�T�E���h�G�t�F�N�g")]  public float SeVolume    = 1;
}

/// <summary>
/// �T�E���h���R���g���[������ cs;
/// </summary>
public class MusicController : MonoBehaviour
{
    [Tooltip("�ǂ��ł��g�p�\�ɂ���.")] public static MusicController instance;

    [SerializeField, Tooltip("�X�e�[�WBGM.")] AudioClip musicClip;
    [Tooltip("�I�[�f�B�I�\�[�X")] AudioSource audioSource;

    private float fadeSpeed = 5;
    private float unfadeSpeed = 5;
    private float maxVolume;

    [SerializeField, Range(0f, 1f), Tooltip("�X�e�[�WBGM Volume.")] public float musicVolume = 1f;
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
    /// �V�����~���[�W�b�N���Đ�.
    /// </summary>
    /// <param name="newAudioClip"> �V�����~���[�W�b�N </param>
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
    /// �Đ�����|�C���g��ύX.
    /// </summary>
    /// <param name="value"> time </param>
    public void SkipToPoint(float value)
    {
        audioSource.time = value;
    }
    /// <summary>
    /// ���[�v�Đ���؂�ւ���.
    /// </summary>
    /// <param name="isLoop"> isLoop </param>
    public void SetLoopSong(bool isLoop)
    {
        audioSource.loop = isLoop;
    }
    /****************************************************************************************************************************/
    /// <summary>
    /// SE ��炷.
    /// </summary>
    /// <param name="newAudioClip"> new SE Clip </param>
    /// <returns> �쐬���ꂽ<see cref="AudioSource"/></returns>
    public AudioSource OneShotAudio(AudioClip newAudioClip)
    {
        if(!newAudioClip) return null;
        GameObject tmpObj = new GameObject("ShotAudio");
        tmpObj.transform.position = gameObject.transform.position;

        AudioSource tmpAudio = tmpObj.AddComponent<AudioSource>();
        tmpAudio.volume = seVolume;
        tmpAudio.clip = newAudioClip;
        tmpAudio.Play();
        // Destroy �ݒ�� �N���b�v�̍ő厞�Ԃ��Z�b�g.
        Destroy(tmpObj, newAudioClip.length);

        return tmpAudio;
    }
/****************************************************************************************************************************/
    // ��������� �l�X�Ȍ��ʋ@�\.

    /// <summary>
    /// <para>��Ŏ��s����Ă���R���[�`�������ׂĂ��Đ����Ă��� </para>�~���[�W�b�N�̉������񂾂񉺂���(0�܂�).
    /// </summary>
    public void FadeMusic()
    {
        StopAllCoroutines();
        StartCoroutine(IE_FadeMusic());
    }
    /// <summary>
    /// music�����񂾂񏬂�������R���[�`��
    /// </summary>
    /// <returns> ���� </returns>
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
    /// <para>��Ŏ��s����Ă���R���[�`�������ׂĂ��Đ����Ă��� </para>�~���[�W�b�N�̉������񂾂�グ��(�ő� volume).
    /// </summary>
    public void UnFadeMusic()
    {
        StopAllCoroutines();
        StartCoroutine(IE_UnFadeMusic());
    }
    /// <summary>
    /// �~���[�W�b�N�̉������񂾂�グ��R���[�`��
    /// </summary>
    /// <returns> ���� </returns>
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
    /// <para> �N���b�v���~���[�W�b�N�R���g���[���[�ɒǉ����� </para>
    /// �t�F�[�h�A�E�g�A�Đ��A�t�F�[�h�C�����������s����R���[�`�����J�n.
    /// </summary>
    /// <param name="newAudioClip"> �N���b�v </param>
    public void FadePlayUnfade(AudioClip newAudioClip)
    {
        StopAllCoroutines();
        musicClip = newAudioClip;
        StartCoroutine(IE_FadePlayUnfade());
    }

    /// <summary>
    /// �t�F�[�h�A�E�g / �Đ� / �t�F�[�h�C�����������s����R���[�`��
    /// </summary>
    /// <returns> ���� </returns>
    IEnumerator IE_FadePlayUnfade()
    {
        // �{�����[�������ȉ��ɂȂ�܂Ńt�F�[�h�A�E�g.
        while (audioSource.volume > 0.1F)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0F, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        // ���S�Ƀ{�����[����0�ɐݒ�.
        audioSource.volume = 0;

        // �Đ����̃I�[�f�B�I���~���A�V�����N���b�v��ݒ肵�čĐ��J�n.
        audioSource.Stop();
        audioSource.clip = musicClip;
        audioSource.Play();

        // �{�����[�����ő�l�ɋ߂Â��܂Ńt�F�[�h�C��.
        while (audioSource.volume < maxVolume - 0.1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume, Time.deltaTime * unfadeSpeed);
            yield return null;
        }
        // �ŏI�I�Ƀ{�����[�����ő�l�ɐݒ�
        audioSource.volume = maxVolume;
    }
/****************************************************************************************************************************/
    // �]�T���������� volume�ۑ��֐����쐬.


}
