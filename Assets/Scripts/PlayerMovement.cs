using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 9f;
    [SerializeField] private float _crouchSpeed = 2.5f; // Скорость при приседании
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _gravity = -9.81f;

    private CharacterController _characterController;
    private Vector2 _movementInput;
    private Vector3 _velocity;
    private bool _isGrounded;

    [Header("Crouch Settings")]
    [SerializeField] private float _crouchHeight = 1f;  // Высота при приседании
    [SerializeField] private float _standHeight = 2f;  // Высота в стоячем положении
    [SerializeField] private float _crouchTransitionSpeed = 5f; // Скорость перехода между состояниями
    private bool _isCrouching = false;

    [Header("Camera Settings")]
    [SerializeField] private float _mouseSensitivity = 75f;
    private Camera _playerCamera;
    private Vector2 _cameraRotation;

    void Start()
    {
        // Инициализация компонентов
        _characterController = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();

        // Блокировка курсора
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        // Проверка, на земле ли персонаж
        _isGrounded = _characterController.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Лёгкое притяжение к земле
        }

        // Получение данных о движении
        _movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Выбор скорости: ходьба, бег или приседание
        float speed = _isCrouching ? _crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed);

        // Вычисление направления движения
        Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.y;
        _characterController.Move(move * speed * Time.deltaTime);

        // Прыжок
        if (Input.GetButtonDown("Jump") && _isGrounded && !_isCrouching)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        // Применение гравитации
        _velocity.y += _gravity * Time.deltaTime;

        // Перемещение персонажа
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        // Обработка приседания: реагирует на нажатие кнопки в любом состоянии
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _isCrouching = false;
        }

        // Плавный переход между состояниями
        float targetHeight = _isCrouching ? _crouchHeight : _standHeight;
        _characterController.height = Mathf.Lerp(_characterController.height, targetHeight, Time.deltaTime * _crouchTransitionSpeed);
    }

    private void HandleCamera()
    {
        // Получение данных мыши
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Обновление вращения камеры
        _cameraRotation.y += mouseInput.x * _mouseSensitivity * Time.deltaTime;
        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x - mouseInput.y * _mouseSensitivity * Time.deltaTime, -90f, 90f);

        // Применение вращения к камере и объекту
        _playerCamera.transform.localEulerAngles = new Vector3(_cameraRotation.x, 0f, 0f);
        transform.eulerAngles = new Vector3(0f, _cameraRotation.y, 0f);
    }
}