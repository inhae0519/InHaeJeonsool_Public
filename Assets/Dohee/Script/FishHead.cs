using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dohee
{
    public class FishHead : MonoBehaviour
    {
        public List<Transform> bodys = new();
        public List<Rigidbody2D> rbs = new();
        public List<HingeJoint2D> joints = new();
        public List<SpriteRenderer> sprites = new();

        private void Awake()
        {
            bodys.Add(transform);

            rbs.Add(bodys[bodys.Count - 1].GetComponent<Rigidbody2D>());
            sprites.Add(bodys[bodys.Count - 1].GetComponent<SpriteRenderer>());

            HingeJoint2D joint = null;

            while (true)
            {
                joint = bodys[bodys.Count - 1].GetComponent<HingeJoint2D>();
                if (joint == null) break;

                bodys.Add(joint.connectedBody.transform);
                joints.Add(joint);
                rbs.Add(bodys[bodys.Count - 1].GetComponent<Rigidbody2D>());
                sprites.Add(bodys[bodys.Count - 1].GetComponent<SpriteRenderer>());
            }

            foreach (Transform body in bodys)
            {
                body.AddComponent<FishBody>().head = this;
            }

            FishScale scale = GetComponent<FishScale>();

            if(scale)
                scale.SetScale(scale.DefaultSize);
        }
    }
}
