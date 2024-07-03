using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dohee
{
    public class FishScale : MonoBehaviour
    {
        [SerializeField] private float defaultSize = 1f;
        [SerializeField] private float scaleMultiply = 1f;
        [SerializeField] private float scaleRandomize = 0;
        [SerializeField] private float edibleMultiply = 0.05f;


        [Header("Scale")]
        [SerializeField] private float scale = 0.1f;

        private List<Transform> bodys;
        private FishHead parts;
        private FishEdible edible;

        public bool CanModify = true;

        public float DefaultSize => defaultSize;
        public float ScaleMultiply => scaleMultiply;
        public float ScaleRandomize => scaleRandomize;
        public float EdibleMultiply => edibleMultiply;

        public float Scale
        {
            get => scale;
            set
            {
                if (CanModify)
                {
                    scale = Mathf.Clamp(value, 0, 6);

                    //Debug.Log(value);

                    foreach(Transform body in bodys)
                    {
                        body.localScale = Vector3.one * scale * ScaleMultiply;
                    }
                }
            }
        }

        private void Awake()
        {
            parts = GetComponent<FishHead>();
            edible = GetComponent<FishEdible>();

            bodys = parts.bodys;
        }

        private void Update()
        {
            if (edible)
            {
                edible.GivenSize = scale * EdibleMultiply;
            }
        }

        public void SetScale(float scale)
        {
            Scale = scale;
        }
    }
}
