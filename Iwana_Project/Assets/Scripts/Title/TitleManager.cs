using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // �_�ł�����Ώہi������Behaviour�ɕύX����Ă���j
    [SerializeField] private Behaviour _target;
    // �_�Ŏ���[s]
    [SerializeField] private float _cycle = 1;

    private double _time;

    private void Update()
    {
        // �����������o�߂�����
        _time += Time.deltaTime;

        // ����cycle�ŌJ��Ԃ��l�̎擾
        // 0�`cycle�͈̔͂̒l��������
        var repeatValue = Mathf.Repeat((float)_time, _cycle);

        // ��������time�ɂ����閾�ŏ�Ԃ𔽉f
        _target.enabled = repeatValue >= _cycle * 0.5f;

        //  Shift�L�[���͎��A���r�[��ʂɈڍs.
        if(Input.GetKey(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
