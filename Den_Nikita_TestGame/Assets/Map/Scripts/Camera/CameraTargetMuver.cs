using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetMover : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _heightSpeed = 1f;

    [Header("Cinemachine Components")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    private CinemachineInputAxisController _inputAxisController;
    private CinemachineRotationComposer _rotationComposer;

    private void Awake()
    {
        if (_cinemachineCamera != null)
        {
            _inputAxisController = _cinemachineCamera.GetComponent<CinemachineInputAxisController>();
            _rotationComposer = _cinemachineCamera.GetComponent<CinemachineRotationComposer>();
        }
    }

    private void Update()
    {
        // Сначала проверяем правую кнопку мыши
        if (Input.GetMouseButton(1)) // 1 — это правая кнопка
        {
            // Если правая кнопка зажата, замораживаем вообще всё
            SetCameraRotationEnabled(false);
            // Не выполняем никакие перемещения и выходим из метода
            return;
        }

        // Дальше обычная логика — проверяем среднюю кнопку (колёсико)
        bool isPanning = Input.GetMouseButton(2);

        if (isPanning)
        {
            SetCameraRotationEnabled(false);
            Move();
        }
        else
        {
            SetCameraRotationEnabled(true);
        }

        // Прокрутка колёсика для изменения высоты
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            AdjustHeight(scroll);
        }
    }

    private void Move()
    {
        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        Vector3 move = (right * dx + forward * dy) * _moveSpeed;
        _target.position += move;
    }

    private void AdjustHeight(float scrollAmount)
    {
        _target.position += Vector3.up * scrollAmount * _heightSpeed;
    }

    private void SetCameraRotationEnabled(bool enabled)
    {
        if (_inputAxisController != null)
        {
            _inputAxisController.enabled = enabled;
        }

        if (_rotationComposer != null)
        {
            _rotationComposer.enabled = enabled;
        }
    }
}
