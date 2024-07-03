using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Dohee
{
    public class FishMovement : MonoBehaviour
    {
        [Header("State")]
        public bool InWater = true;
        public bool Torrent = false;
        public bool Flying = false;

        [Header("Speed")]
        public float MaxSpeed = 10f;
        public float FishSpeed = 5f;

        [Header("Angle")]
        public float EulerSpeed = 10f;
        public float RandomAngle = 10f;
        public float maxChangeTime = 0.5f;
        [SerializeField] private float RandomValue = 0f;

        [Header("Gravity")]
        public float GravityScale = 9.8f;
        [SerializeField] private float gravityMultiply = 1f;

        public float GravityMultiply
        {
            get => gravityMultiply;
            set
            {
                gravityMultiply = Mathf.Clamp01(value);
            }
        }

        public bool Left { get; private set; }

        private Camera cam;
        private FishHead head;
        private Rigidbody2D rb;

        private float movingTime = 0f;
        private float currentTime = 0f;

        private void Awake()
        {
            cam = Camera.main;
            head = GetComponent<FishHead>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            bool tempWater = InWater;
            float tempMultiply = gravityMultiply;

            if (Flying)
            {
                InWater = true;
                gravityMultiply = 0.05f;
            }

            if (InWater)
            {
                if (movingTime == 0) SoundManager.Instance.Play("Effect/Splash-large");

                movingTime += Time.deltaTime;

                if (Input.GetMouseButton(0))
                {
                    if(movingTime > 0.5f)
                        Move(GetMouseDir());
                }
                else
                {
                    transform.rotation = head.bodys[1].rotation;
                }
            }

            if (!InWater)
            {
                movingTime = 0f;

                transform.rotation = head.bodys[1].rotation;
            }

            foreach (Rigidbody2D rb in head.rbs)
            {
                rb.AddForce(Vector2.down * GravityScale * GravityMultiply * Time.deltaTime);
                rb.drag = 5 * (1 - GravityMultiply);
                rb.angularDrag = 5 * (1 - GravityMultiply);
            }

            currentTime += Time.deltaTime;

            if (Left)
                RandomValue = Mathf.Lerp(-RandomAngle, RandomAngle, currentTime / maxChangeTime);
            else
                RandomValue = Mathf.Lerp(RandomAngle, -RandomAngle, currentTime / maxChangeTime);

            if (currentTime > maxChangeTime)
            {
                Left = !Left;
                currentTime = 0f;
            }

            InWater = tempWater;
            gravityMultiply = tempMultiply;
        }

        private Vector3 GetMouseDir()
        {
            Vector3 mPos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mPos, transform.position) > 0.5f)
                return (mPos - transform.position).normalized;
            else
                return Vector3.zero;
        }

        public void Move(Vector3 dir)
        {
            if(dir == Vector3.zero)
            {
                rb.AddForce(-rb.velocity);
                return;
            }

            rb.AddForce(transform.right * transform.localScale.magnitude * FishSpeed * ((MaxSpeed + transform.localScale.magnitude - rb.velocity.magnitude) / (MaxSpeed + transform.localScale.magnitude)) * Time.deltaTime);

            foreach (SpriteRenderer sr in head.sprites)
            {
                if(rb.velocity.x > 0)
                    sr.flipY = false;
                else
                    sr.flipY = true;
            }

            SetAngle(dir);
        }

        private void SetAngle(Vector3 dir)
        {
            if (dir == Vector3.zero)
            {
                return;
            }

            float current = transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z;
            float target = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + RandomValue;

            float temp = target - current;

            float way = Mathf.Abs(temp) < 180 ? temp : temp > 0 ? -360 + temp : 360 + temp;

            float z = way / Mathf.Abs(way) * Time.deltaTime * EulerSpeed;

            z *= Mathf.Clamp01(Mathf.Abs(way) / Mathf.Abs(z));

            transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + z);
        }
    }
}
