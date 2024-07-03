using System.Collections.Generic;
using UnityEngine;
using FSM;

public class AITransition : MonoBehaviour
{
    private List<AIDecision> _decisions;
    public AIState NextState;

    public void SetUp(Transform agent)
    {
        _decisions = new();
        GetComponents(_decisions);
        _decisions.ForEach(d => d.SetUp(agent));
    }

    public bool MakeATransition()
    {
        bool result = false;
        foreach (var decision in _decisions)
        {
            result = decision.MakeADecision();
            if (decision.IsReverse)
            {
                result = !result;
            }
            if (result == false) return false;
        }

        return result;
    }
}