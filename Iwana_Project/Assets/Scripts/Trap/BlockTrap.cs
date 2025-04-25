using UnityEngine;

public class BlockTrap : MonoBehaviour
{
    public enum ReactionArea
    {
        [Tooltip("�S��")] Full,
        [Tooltip("���̂�")] LeftOnly,
        [Tooltip("�E�̂�")] RightOnly,
        [Tooltip("��̂�")] TopOnly,
        [Tooltip("���̂�")] BottomOnly  
    }

    [Header("Trap Settings")]
    [Tooltip("�����鏰")] public bool disappearOnTouch = true;
    [Tooltip("����鏰")] public bool appearOnTouch = false;

    [Header("Reaction Settings")]
    [Tooltip("�͈�")] public ReactionArea reactionArea = ReactionArea.Full; // �����͈�

    [Header("Audio Settings")]
    public AudioClip activateSound;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int groundLayerIndex;
    private int transparentLayerIndex;

    private static AudioSource playingSound;
    private float delayBetweenAudio;

    private void Start()
    {
        // �R���|�[�l���g�L���b�V��
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // ���C���[�ݒ�
        groundLayerIndex = LayerMask.NameToLayer("SolidObjects");
        transparentLayerIndex = LayerMask.NameToLayer("Default");

        // �I�[�f�B�I�x���̌v�Z
        if (activateSound != null)
            delayBetweenAudio = activateSound.length / 10f;

        // ������Ԃ̐ݒ�
        InitializeState();
    }

    private void OnDestroy()
    {

    }

    private void InitializeState()
    {
        if (appearOnTouch)
        {
            gameObject.tag = "Untagged";
            spriteRenderer.enabled = false;
            gameObject.layer = transparentLayerIndex;
            boxCollider.isTrigger = true;
        }
        else
        {
            gameObject.tag = "Ground";
            spriteRenderer.enabled = true;
        }
    }

    private void Reset()
    {
        gameObject.SetActive(true);
        InitializeState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);

            // �v���C���[�̐ڐG�ʒu�ɉ���������
            if (ShouldReact(contactPoint))
            {
                HandleAudioPlayback();

                if (disappearOnTouch)
                {
                    gameObject.tag = "Untagged";
                    gameObject.SetActive(false);
                }

                if (appearOnTouch)
                {
                    gameObject.tag = "Ground";
                    gameObject.layer = groundLayerIndex;
                    spriteRenderer.enabled = true;
                    boxCollider.isTrigger = false;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        if (other.CompareTag("Player"))
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);

            // �v���C���[�̐ڐG�ʒu�ɉ���������
            if (ShouldReact(contactPoint))
            {
                HandleAudioPlayback();

                if (disappearOnTouch)
                {
                    gameObject.tag = "Untagged";
                    gameObject.SetActive(false);
                }

                if (appearOnTouch)
                {
                    gameObject.tag = "Ground";
                    gameObject.layer = groundLayerIndex;
                    spriteRenderer.enabled = true;
                    boxCollider.isTrigger = false;
                }
            }
        }
    }
    private bool ShouldReact(Vector2 contactPoint)
    {
        Vector2 localPoint = transform.InverseTransformPoint(contactPoint); // ���[�J�����W�n�ɕϊ�
        Vector2 size = GetComponent<BoxCollider2D>().size;
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        // ����͈͂̋��E��������
        float leftBoundary = -halfWidth;     // ���[
        float rightBoundary = halfWidth;    // �E�[
        float topBoundary = halfHeight;     // ��[
        float bottomBoundary = -halfHeight; // ���[

        float horizontalThreshold = size.x * 0.25f; // �������ł̃M���M�����蒲�� (25%�̗]��)
        float verticalThreshold = size.y * 0.25f;   // �c�����ł̃M���M�����蒲�� (25%�̗]��)

        switch (reactionArea)
        {
            case ReactionArea.Full:
                return true; // �S�̂Ŕ���

            case ReactionArea.LeftOnly:
                // ���̂�: �����̃G���A�Ŕ����A�������㉺�̗]�����l��
                return localPoint.x < leftBoundary + horizontalThreshold;

            case ReactionArea.RightOnly:
                // �E�̂�: �E���̃G���A�Ŕ����A�������㉺�̗]�����l��
                return localPoint.x > rightBoundary - horizontalThreshold;

            case ReactionArea.TopOnly:
                // ��̂�: �㑤�̃G���A�Ŕ����A���������E�̗]�����l��
                return localPoint.y > topBoundary - verticalThreshold;
            case ReactionArea.BottomOnly:
                // ���̂�: �����̃G���A�Ŕ����A���������E�̗]�����l��
                return localPoint.y < bottomBoundary + verticalThreshold;

            default:
                return false;
        }
    }



    private void HandleAudioPlayback()
    {
        if (activateSound == null) return;

        if (playingSound == null || playingSound.time > delayBetweenAudio)
        {
            playingSound = MusicController.instance.OneShotAudio(activateSound);
        }
    }
}
