using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionSystem : Singleton<MotionSystem>
{
    private List<MotionInfo> registeredMotions;

    void Start()
    {
        registeredMotions = new List<MotionInfo>();
    }

    void Update()
    {
        registeredMotions.RemoveAll((motion) => {
            if(motion.Move())
            {
                motion.onMotionEnd?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        });
    }

    public void StartMoving(Transform transform, Vector3 destination, float speed, Action onEnd)
    {
        registeredMotions.Add(new MotionInfo(transform, destination, speed, onEnd));
    }

    private class MotionInfo
    {
        public Transform transform;
        public Vector3 destination;
        public float speed; // [m/s]
        public Action onMotionEnd;

        private Vector3 direction;

        public MotionInfo(Transform transform, Vector3 destination, float speed, Action onMotionEnd)
        {
            this.transform = transform;
            this.destination = destination;
            this.speed = speed;
            this.onMotionEnd = onMotionEnd;

            direction = destination - transform.position;
            direction.Normalize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when move ends.</returns>
        public bool Move()
        {
            transform.Translate(direction * speed * Time.deltaTime); // [m/s * s/f = m/f]
            // if the object already passed the destination
            var v = destination - transform.position;
            if(Vector3.Dot(v, direction) <= 0)
            {
                transform.position = destination;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
