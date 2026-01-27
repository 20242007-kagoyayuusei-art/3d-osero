using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("ターゲット設定")]
    [SerializeField] private Transform target; 
    
    [Header("回転設定")]
    [SerializeField] private float rotateSpeed = 0.1f;
    [SerializeField] private float verticalLimitMin = 5f;
    [SerializeField] private float verticalLimitMax = 80f;

    [Header("ズーム設定")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minDistance = 5.0f;
    [SerializeField] private float maxDistance = 40.0f;
    [SerializeField] private float defaultDistance = 10.0f;

    private float currentX = 0f;
    private float currentY = 0f;
    private float currentDistance;
    private Vector3 boardCenter; // ボード中心を保存

    void Start()
    {
        // ターゲットが未設定なら "Field" を探す
        if (target == null)
        {
            GameObject field = GameObject.Find("Field");
            if (field != null) target = field.transform;
        }

        // ボード中心を取得（ボードの transform.position を支点として使用）
        if (target != null)
        {
            boardCenter = target.position;
        }

        // 起動時の角度を取得
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        // 初期距離を設定（インスペクターの値を使う）
        //currentDistance = defaultDistance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        // --- 回転 (左ドラッグ) ---
        if (mouse.leftButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            currentX += mouseDelta.x * rotateSpeed;
            currentY -= mouseDelta.y * rotateSpeed;
            currentY = Mathf.Clamp(currentY, verticalLimitMin, verticalLimitMax);
        }

        // --- ズーム (ホイール) ---
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f) // 微小な入力をカット
        {
            // ズーム方向を直感的にし、感度を調整
            currentDistance -= scroll * zoomSpeed * 0.1f;
        }
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // --- 座標計算（ボード中心を支点に） ---
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        // ボード中心から後ろに currentDistance 分だけ離れた位置
        Vector3 position = rotation * new Vector3(0, 0, -currentDistance-60) + boardCenter;

        transform.position = position;
        transform.rotation = rotation;
    }
}