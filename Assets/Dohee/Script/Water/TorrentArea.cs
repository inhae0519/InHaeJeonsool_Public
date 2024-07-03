using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dohee
{
    public class TorrentArea : MonoBehaviour
    {
        [SerializeField] Transform EndPoint;
        [SerializeField] float power = 10f;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out FishBody body))
            {
                if (body.TryGetComponent(out Rigidbody2D rb))
                {
                    if(body.head.TryGetComponent(out FishMovement move))
                    {
                        if (move.Torrent)
                        {
                            rb.AddForce((EndPoint.position - rb.transform.position).normalized * power * power);
                        }
                        else
                        {
                            rb.AddForce((EndPoint.position - rb.transform.position).normalized * power * power * power);
                        }
                    }
                    if (collision.gameObject.CompareTag("Moon"))
                    {
                        rb.AddForce((EndPoint.position - rb.transform.position).normalized * power * power * power);
                    }
                }
            }
        }
    }
}
