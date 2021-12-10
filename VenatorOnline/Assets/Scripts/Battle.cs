using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    private Animator _animator;
    private Transform _cameraTransform;
    private int _countCombo = 0;
    private float _previousTime = 0f;
    private float _delay = 0f;

    private float _timeToBattle = 1.5f;

    private bool _firstAttack = true;
    private bool _thirdCombo_1 = false;
    private bool _thirdCombo_2 = false;

    private float _turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;

    public GameObject _swordOn;
    public GameObject _swordOff;

    [SerializeField] private CapsuleCollider _swordCollider;

    private string _changeWeaponAnim;
    private bool _previousWeapon = false;

    private Weapon _selectedWeapon;

    public bool AllowBattle = true;

    private Movement _movement;

    void Start()
    {
        _swordOn.SetActive(false);
        _swordOff.SetActive(true);
        _swordCollider.enabled = false;
        _animator = GetComponent<Animator>();
        _cameraTransform= GameObject.FindGameObjectWithTag("MainCamera").transform;
        _movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (!AllowBattle)
        {
            _selectedWeapon = Weapon.None;
            return;
        }

        ChangeWeapon();

        _delay = Time.time - _previousTime;
        if (_delay > 100) { _delay = 100.0f; }
        if (Input.GetButtonDown("Fire1"))
        {
            if (_delay >= _timeToBattle)
            {
                _firstAttack = true;
                _thirdCombo_1 = false;
                _thirdCombo_2 = false;
                _countCombo = 0;
            }

            switch (_selectedWeapon)
            {
                case Weapon.Sword:
                    _swordCollider.enabled = true;
                    Invoke("WeaponColliderOff", 1.0f);
                    SwordBattle();
                    break;
                default:
                    break;
            }
        }
    }

    private void WeaponColliderOff()
    {
        _swordCollider.enabled = false;
    }
    private void ChangeWeapon_Animation()
    {
        _animator.SetTrigger(_changeWeaponAnim);
    }
    private void ChangeWeapon()
    {
        if (Input.GetButtonDown("Equip First Item"))
        {
            _changeWeaponAnim = "Withdrawing";

            if (_selectedWeapon == Weapon.Sword)
            {
                Invoke("SwordOff", 0.5f);
                _animator.SetTrigger("Sheathing");
                _selectedWeapon = Weapon.None;
                return;
            }
            if (_previousWeapon)
            {
                Invoke("SwordOn", 0.8f);
                Invoke("ChangeWeapon_Animation", 0.4f);
            }
            else
            {
                Invoke("SwordOn", 0.6f);
                _animator.SetTrigger("Withdrawing");
            }

            _previousWeapon = false;
            _selectedWeapon = Weapon.Sword;
        }
    }

    #region Смена оружия
    private void SwordOff()
    {
        _swordOn.SetActive(false);
        _swordOff.SetActive(true);
    }
    private void SwordOn()
    {
        _swordOff.SetActive(false);
        _swordOn.SetActive(true);
    }
    #endregion 
    public IEnumerator RotateToCamera()
    {
        while (true)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }
    }
    private void DestroyCoroutine()
    {
        StopCoroutine("RotateToCamera");
    }
    private void SwordBattle()
    {
        if (_movement.moveDirection.magnitude <= 0.1f) { StartCoroutine("RotateToCamera"); }
        Invoke("DestroyCoroutine", 1.0f);

        ++_countCombo;
        _countCombo = _countCombo % 3;
        if (_countCombo == 0)
        {
            _countCombo = 3;
        }
        if (_delay >= 0.5f)
        {
            _previousTime = Time.time;
            if (_countCombo == 3 && _thirdCombo_1 && _thirdCombo_2)
            {
                _animator.SetTrigger("SwordStrike3");
                _thirdCombo_1 = false;
                _thirdCombo_2 = false;
            }
            else
            {
                if (_firstAttack)
                {
                    _animator.SetTrigger("SwordStrike1");
                    _firstAttack = false;
                    if (_countCombo == 1)
                    {
                        _thirdCombo_1 = true;
                    }
                }
                else
                {
                    _animator.SetTrigger("SwordStrike2");
                    _firstAttack = true;
                    if (_countCombo == 2 && _thirdCombo_1)
                    {
                        _thirdCombo_2 = true;
                    }
                    else
                    {
                        _thirdCombo_1 = false;
                    }
                }
            }
        }
    }

    public enum Weapon
    {
        None = 0,
        Sword = 1
    }
}