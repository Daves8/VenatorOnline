using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    private bool _die = false;
    public AudioClip scream;
    private AudioSource _audioSource;
    private bool _attacked = false;
    private bool _attackedAdd = true;

    public Transform[] places;
    private Transform _place;

    private bool _startCoroutineW;
    private bool _startCoroutineE;
    private bool _walkCorout;
    private bool _nextPlace;
    private float _timeToWalk;
    private float _timeToEat;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _place = places[Random.Range(0, places.Length)];
        _startCoroutineW = false;
        _startCoroutineE = false;
        _nextPlace = false;
        _agent.speed = 2;
    }

    void Update()
    {
        if (_die)
        {
            return;
        }

        if (_agent.velocity.magnitude > 0.0f)
        {
            _animator.SetBool("Walk", true);
        }
        else
        {
            if (!_startCoroutineE)
            {
                _animator.SetBool("Walk", false);
            }
            if (!_startCoroutineE && Time.time - _timeToEat >= Random.Range(60.0f, 140.0f))
            {
            }
        }
        Walking();
    }

    private void Walking()
    {
        _walkCorout = true;
        if (Vector3.Distance(new Vector3(_place.position.x, 0f, _place.position.z), new Vector3(transform.position.x, 0f, transform.position.z)) <= 2.0f)
        {
            if (Time.time - _timeToWalk >= Random.Range(3.0f, 30.0f))
            {
                _nextPlace = true;
            }
        }
        else
        {
            _timeToWalk = Time.time;
            _agent.SetDestination(_place.position);
        }
        if (!_startCoroutineW)
        {
            StartCoroutine(WalkCorout());
        }
    }
    IEnumerator WalkCorout()
    {
        _startCoroutineW = true;
        while (_walkCorout)
        {
            _place = places[Random.Range(0, places.Length)].transform;
            yield return new WaitUntil(() => _nextPlace);
            _nextPlace = false;
        }
        _startCoroutineW = false;
    }

    private void Dying()
    {
        _die = true;
        _agent.enabled = false;
        _animator.SetTrigger("Die");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_die)
        {
            if (other.gameObject.tag == "Sword")
            {
                Dying();
                _animator.SetTrigger("Die");
            }
        }
    }
}