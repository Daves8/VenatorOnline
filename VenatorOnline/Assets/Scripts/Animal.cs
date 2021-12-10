using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    public bool _die = false;

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
    }

    void Update()
    {
        if (_die)
        {
            _animator.SetTrigger("Die");
            return;
        }

        if (_agent.velocity.magnitude > 0.0f)
        {
            _animator.SetBool("Walk", true);
            _animator.SetBool("Eat", false);
        }
        else
        {
            if (!_startCoroutineE)
            {
                _animator.SetBool("Walk", false);
                _animator.SetBool("Eat", false);
            }
            if (!_startCoroutineE && Time.time - _timeToEat >= Random.Range(60.0f, 140.0f))
            {
                _timeToEat = Time.time;
                StartCoroutine(EatCorout());
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
    IEnumerator EatCorout()
    {
        _startCoroutineE = true;
        _animator.SetBool("Eat", true);
        yield return new WaitForSeconds(Random.Range(5f, 15f));
        _animator.SetBool("Eat", false);
        _startCoroutineE = false;
    }

    private void Dying()
    {
        _die = true;
        _agent.enabled = false;
        _animator.SetTrigger("Die");
        Invoke("Delete", 300.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Knife" || other.tag == "Sword" || other.tag == "SwordEn")
        {
            Dying();
        }
    }

    private void Delete()
    {
        Destroy(gameObject);
    }
}