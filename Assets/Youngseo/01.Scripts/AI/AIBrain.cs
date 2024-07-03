using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class AIBrain : Poolable
{
    private List<AIState> _states;
    [SerializeField] private AIState _currentState;
    public Transform playerTrm;

    private Rigidbody2D _rigid;

    private bool _isStunned;
    private bool _isOverWater;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if(playerTrm == null)
            playerTrm = FishSingleton.Singleton.transform;

        _states = new();
        GetComponentsInChildren(_states);
        
        _states.ForEach(state => state.SetUp(this.transform));
        _currentState.OnEnterState();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTrm.position) > 30 * transform.localScale.x)
        {
            _rigid.velocity = new Vector2(0, 0.024f);
            return;
        }
        if (playerTrm is null) _currentState = GetComponentInChildren<PatrolState>();
        if (_isStunned || _isOverWater) return;
        _currentState.UpdateState();
    }

    public void ChangeState(AIState state)
    {
        _currentState.OnExitState();
        _currentState = state;
        _currentState.OnEnterState();
    }

    public void Stun(float duration)
    {
        StopCoroutine(nameof(StunDelay));
        StartCoroutine(nameof(StunDelay), duration);
    }

    private IEnumerator StunDelay(float duration)
    {
        _isStunned = true;
        yield return new WaitForSeconds(duration);
        _isStunned = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OverWater"))
        {
            _isOverWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("OverWater"))
        {
            _isOverWater = false;
        }
    }

    public override void Initialize()
    {
    }
}