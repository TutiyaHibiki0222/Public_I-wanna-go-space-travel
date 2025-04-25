using UnityEngine;

public class BlockTrap : MonoBehaviour
{
    public enum ReactionArea
    {
        [Tooltip("全体")] Full,
        [Tooltip("左のみ")] LeftOnly,
        [Tooltip("右のみ")] RightOnly,
        [Tooltip("上のみ")] TopOnly,
        [Tooltip("下のみ")] BottomOnly  
    }

    [Header("Trap Settings")]
    [Tooltip("消える床")] public bool disappearOnTouch = true;
    [Tooltip("現れる床")] public bool appearOnTouch = false;

    [Header("Reaction Settings")]
    [Tooltip("範囲")] public ReactionArea reactionArea = ReactionArea.Full; // 反応範囲

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
        // コンポーネントキャッシュ
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // レイヤー設定
        groundLayerIndex = LayerMask.NameToLayer("SolidObjects");
        transparentLayerIndex = LayerMask.NameToLayer("Default");

        // オーディオ遅延の計算
        if (activateSound != null)
            delayBetweenAudio = activateSound.length / 10f;

        // 初期状態の設定
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

            // プレイヤーの接触位置に応じた処理
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

            // プレイヤーの接触位置に応じた処理
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
        Vector2 localPoint = transform.InverseTransformPoint(contactPoint); // ローカル座標系に変換
        Vector2 size = GetComponent<BoxCollider2D>().size;
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        // 判定範囲の境界を厳密化
        float leftBoundary = -halfWidth;     // 左端
        float rightBoundary = halfWidth;    // 右端
        float topBoundary = halfHeight;     // 上端
        float bottomBoundary = -halfHeight; // 下端

        float horizontalThreshold = size.x * 0.25f; // 横方向でのギリギリ判定調整 (25%の余白)
        float verticalThreshold = size.y * 0.25f;   // 縦方向でのギリギリ判定調整 (25%の余白)

        switch (reactionArea)
        {
            case ReactionArea.Full:
                return true; // 全体で反応

            case ReactionArea.LeftOnly:
                // 左のみ: 左側のエリアで反応、ただし上下の余白を考慮
                return localPoint.x < leftBoundary + horizontalThreshold;

            case ReactionArea.RightOnly:
                // 右のみ: 右側のエリアで反応、ただし上下の余白を考慮
                return localPoint.x > rightBoundary - horizontalThreshold;

            case ReactionArea.TopOnly:
                // 上のみ: 上側のエリアで反応、ただし左右の余白を考慮
                return localPoint.y > topBoundary - verticalThreshold;
            case ReactionArea.BottomOnly:
                // 下のみ: 下側のエリアで反応、ただし左右の余白を考慮
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
