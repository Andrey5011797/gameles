using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;
    public float targetHeightOffset = 1.5f;

    [Header("Дистанция")]
    public float distance = 5f;

    [Header("Вращение мышью")]
    public float mouseSensitivity = 0.15f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Автовыравнивание за спиной")]
    [Tooltip("Включить автоматический возврат камеры за спину игрока")]
    public bool autoAlign = true;
    [Tooltip("Через сколько секунд без движения мыши начать выравнивание")]
    public float autoAlignDelay = 1.0f;
    [Tooltip("Скорость выравнивания (чем меньше — тем плавнее)")]
    public float autoAlignSmoothTime = 0.4f;
    [Tooltip("Насколько точно камера должна встать за спину, прежде чем остановится")]
    public float autoAlignAngleThreshold = 1f;

    [Header("Сглаживание")]
    public float followSmoothTime = 0.05f;

    [Header("Столкновения")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;

    private float _yaw;
    private float _pitch = 20f;

    private Vector3 _currentVelocity;
    private Vector3 _smoothTargetPos;

    private float _yawVelocity;
    private float _lastMouseMoveTime;
    private bool _isAutoAligning;

    private PlayerMovement _playerMovement;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: не назначен target!");
            return;
        }

        _playerMovement = target.GetComponent<PlayerMovement>();

        _yaw = target.eulerAngles.y;
        _pitch = 20f;
        _smoothTargetPos = target.position + Vector3.up * targetHeightOffset;
        _lastMouseMoveTime = Time.time;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleMouseInput();
        HandleAutoAlign();
        HandlePosition();

        Vector3 desiredTargetPos = target.position + Vector3.up * targetHeightOffset;
        _smoothTargetPos = Vector3.SmoothDamp(
            _smoothTargetPos,
            desiredTargetPos,
            ref _currentVelocity,
            followSmoothTime);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
            Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
        }
    }

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 delta = mouse.delta.ReadValue();

        if (delta.sqrMagnitude > 0.01f)
        {
            _lastMouseMoveTime = Time.time;
            _isAutoAligning = false;

            _yaw += delta.x * mouseSensitivity;
            _pitch -= delta.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }
    }

    private void HandleAutoAlign()
    {
        if (!autoAlign) return;
        if (target == null) return;

        bool playerIsMoving = _playerMovement != null && _playerMovement.IsMoving;

        bool enoughIdleTime = Time.time - _lastMouseMoveTime >= autoAlignDelay;

        if (!playerIsMoving || !enoughIdleTime)
        {
            _isAutoAligning = false;
            return;
        }

        float desiredYaw = target.eulerAngles.y;
        float currentYaw = _yaw;

        float diff = Mathf.DeltaAngle(currentYaw, desiredYaw);

        if (Mathf.Abs(diff) < autoAlignAngleThreshold)
        {
            _yaw = desiredYaw;
            _isAutoAligning = false;
            return;
        }

        _isAutoAligning = true;

        _yaw = Mathf.SmoothDampAngle(
            currentYaw,
            desiredYaw,
            ref _yawVelocity,
            autoAlignSmoothTime);

        _pitch = Mathf.Lerp(_pitch, 20f, Time.deltaTime / autoAlignSmoothTime);
    }

    private void HandlePosition()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 direction = rotation * Vector3.back;
        Vector3 desiredCameraPos = _smoothTargetPos + direction * distance;

        if (Physics.SphereCast(
                _smoothTargetPos,
                collisionRadius,
                direction,
                out RaycastHit hit,
                distance,
                collisionMask,
                QueryTriggerInteraction.Ignore))
        {
            desiredCameraPos = _smoothTargetPos + direction * (hit.distance - 0.1f);
        }

        transform.position = desiredCameraPos;
        transform.LookAt(_smoothTargetPos);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Vector3 lookPoint = target.position + Vector3.up * targetHeightOffset;
        Gizmos.DrawWireSphere(lookPoint, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(lookPoint, transform.position);
    }
}
