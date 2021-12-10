using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _controller;
    private Transform _camera;

    public Vector3 moveDirection;

    public bool IsReadyToMove;
    public bool IsReadyToRun;
    public bool IsReadyToRotate;

    private PhotonView _photonView;

    private float _turnSmoothTime;
    private float _gravity;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private float _speed;
    private float _speedWalk;
    private float _speedRun;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        _photonView = GetComponent<PhotonView>();

        _speed = 2.0f;
        _turnSmoothTime = 0.1f;
        _gravity = -19.62f;

        IsReadyToMove = true;
        IsReadyToRun = true;
        IsReadyToRotate = true;

        _speedRun = _speed * 3.5f;
        _speedWalk = _speed;
    }

    private void Update()
    {
        if (!_photonView.IsMine) { return; }
        // движение
        CharacterMove();
        // падение
        CharacterFalling();
    }

    private void CharacterMove()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        if (moveDirection.magnitude >= 0.1f && _controller.isGrounded && IsReadyToMove)
        {
            AnimationsStandings();
            CharacterMove_Func(moveDirection);
        }
        else
        {
            _animator.SetBool("IdleToWalking", false);
            _animator.SetBool("WalkingToRun", false);
        }
    }
    private void CharacterMove_Func(Vector3 moveDirection)
    {
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

        if (IsReadyToRotate)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        _controller.Move(moveDir.normalized * _speed * Time.deltaTime);
    }

    private void CharacterFalling()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

    }

    private void AnimationsStandings()
    {
        if (Input.GetButton("Change Speeds") && IsReadyToRun)
        {
            _speed = _speedRun;
            _animator.SetBool("WalkingToRun", true);
        }
        else
        {
            _speed = _speedWalk;
            _animator.SetBool("WalkingToRun", false);
        }

        _animator.SetBool("IdleToWalking", true);
    }
}