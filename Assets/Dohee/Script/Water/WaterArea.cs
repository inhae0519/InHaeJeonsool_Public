using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dohee
{
    public class WaterArea : MonoBehaviour
    {
        [SerializeField] float EnterValue = 0.1f;
        [SerializeField] float OuterValue = 1.0f;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out FishHead head))
            {
                if(head.TryGetComponent(out FishMovement move))
                {
                    move.InWater = true;
                    move.GravityMultiply = EnterValue;
                }

                if(head.TryGetComponent(out AIBrain brain))
                {
                    brain.GetComponent<Rigidbody2D>().gravityScale = EnterValue;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out FishHead head))
            {
                if (head.TryGetComponent(out FishMovement move))
                {
                    move.InWater = false;
                    move.GravityMultiply = OuterValue;
                }


                if (head.TryGetComponent(out AIBrain brain))
                {
                    brain.GetComponent<Rigidbody2D>().gravityScale = OuterValue * 2;
                }
            }
        }
    }
}
