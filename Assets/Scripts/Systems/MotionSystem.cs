using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MotionSystem : Singleton<MotionSystem>
{
    protected Dictionary<string, Queue<MotionQueueElement>> motionQueueDict;
    private Dictionary<string, Action> finalActions;
    private List<string> emptyQueueKeys = new List<string>();

    void Start()
    {
        
    }

    void Update()
    {
        if(motionQueueDict == null) return;

        emptyQueueKeys.Clear();

        // run motion queues
        foreach(var motionKey in motionQueueDict.Keys)
        {
            if(motionQueueDict[motionKey].Count > 0)
            {
                var headMotion = motionQueueDict[motionKey].Peek();
                if (headMotion != null)
                {
                    // call start if it's not yet called
                    if(!headMotion.isStartCalled)
                    {
                        headMotion.Start();
                        headMotion.isStartCalled = true;
                    }

                    // run motion
                    if(headMotion.Run())
                    {
                        motionQueueDict[motionKey].Dequeue().End();
                        if (motionQueueDict[motionKey].Count == 0) emptyQueueKeys.Add(motionKey);
                    }
                }
            }
        }

        // finish motion queues
        foreach(string key in emptyQueueKeys)
        {
            motionQueueDict.Remove(key);
            finalActions[key]?.Invoke();
            finalActions.Remove(key);
        }
    }

    public class MotionQueue
    {
        public delegate bool Condition();
        private MotionSystem MSystem => MotionSystem.Instance;
        private Queue<MotionQueueElement> queue;

        public MotionQueue()
        {
            queue = new Queue<MotionQueueElement>();   
        }

        public MotionQueue PushMotion(MotionInfo motionInfo, Action onEnd = null)
        {
            queue.Enqueue(new Motion(motionInfo, onEnd));
            return this;
        }

        public MotionQueue PushDelay(float second)
        {
            queue.Enqueue(new Delay(second));
            return this;
        }

        public MotionQueue PushDelayUntil(Condition condition)
        {
            queue.Enqueue(new DelayUntil(condition));
            return this;
        }

        public void Run(Action onEnd = null)
        {
            if (MSystem.motionQueueDict == null)
            {
                MSystem.motionQueueDict = new Dictionary<string, Queue<MotionQueueElement>>();
            }
            if(MSystem.finalActions == null)
            {
                MSystem.finalActions = new Dictionary<string, Action>();
            }

            string key = Guid.NewGuid().ToString();
            MSystem.motionQueueDict[key] = queue;
            MSystem.finalActions[key] = onEnd;
        }
    }

    protected abstract class MotionQueueElement 
    {
        public bool isStartCalled = false;

        public abstract void Start();

        /// <summary>
        /// Returns true if done.
        /// </summary>
        /// <returns></returns>
        public abstract bool Run();

        public abstract void End();
    }

    protected class Motion : MotionQueueElement
    {
        private MotionInfo motionInfo;
        private Vector3 direction;
        private float elapsedTime;
        private float expectedTime;
        private Action onEnd;
        
        public Motion(MotionInfo motionInfo, Action onEnd)
        {
            this.motionInfo = motionInfo;

            direction = (motionInfo.to - motionInfo.from).normalized;

            var distance = (motionInfo.to - motionInfo.from).magnitude;
            expectedTime = distance / IntegrateAnimationCurve(motionInfo.speedCurve) / motionInfo.maxSpeed;

            this.onEnd = onEnd;
        }

        public override void Start() 
        {
            motionInfo.transform.position = motionInfo.from;
        }

        public override bool Run()
        {
            elapsedTime += Time.deltaTime;
            motionInfo.transform.Translate(direction * motionInfo.maxSpeed * motionInfo.speedCurve.Evaluate(elapsedTime / expectedTime) * Time.deltaTime);

            if(elapsedTime >= expectedTime)
            {
                motionInfo.transform.position = motionInfo.to;
                return true;
            }
            else return false;
        }

        public override void End()
        {
            onEnd?.Invoke();
        }

        // https://blog.devgenius.io/calculating-the-area-under-an-animationcurve-in-unity-c43132a3abf8
        public static float IntegrateAnimationCurve(AnimationCurve curve, int stepsize = 1000)
        {
            float sum = 0;
            for (int i = 0; i < stepsize; i++)
            {
                sum += IntegralOnStep(
                            (float)i / stepsize,
                            curve.Evaluate((float)i / stepsize),
                            (float)(i + 1) / stepsize,
                            curve.Evaluate((float)(i + 1) / stepsize)
                        );
            }

            return sum;   
        }

        private static float IntegralOnStep(float x0, float y0, float x1, float y1)
        {
            float a = (y1 - y0) / (x1 - x0);
            float b = y0 - a * x0;
            return (a / 2 * x1 * x1 + b * x1) - (a / 2 * x0 * x0 + b * x0);
        }
    }

    protected class Delay : MotionQueueElement
    {
        private float second;
        private bool done;

        public Delay(float second)
        {
            this.second = second;
            done = false;
        }

        public override void Start()
        {
            Timing.Instance.DelayedExecute(second, () => { done = true; });
        }

        public override bool Run()
        {
            return done;
        }

        public override void End()
        {
            
        }
    }

    protected class DelayUntil : MotionQueueElement
    {
        private MotionQueue.Condition condition;

        public DelayUntil(MotionQueue.Condition condition)
        {
            this.condition = condition;
        }

        public override void Start()
        {

        }

        public override bool Run()
        {
            return condition();
        }

        public override void End()
        {

        }
    }
}

public class MotionInfo
{
    public Transform transform;
    public Vector3 from;
    public Vector3 to;
    public AnimationCurve speedCurve;
    public float maxSpeed;

    /// <summary>
    /// *speedCurve's value must be from 0 to 1 and its key also must be from 0 to 1.
    /// </summary>
    public MotionInfo(Transform transform, Vector3 from, Vector3 to, AnimationCurve speedCurve, float maxSpeed)
    {
        this.transform = transform;
        this.from = from;
        this.to = to;
        this.speedCurve = speedCurve;
        this.maxSpeed = maxSpeed;
    }
}