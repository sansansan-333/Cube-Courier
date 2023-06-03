using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotControl : MonoBehaviour
{
    private readonly float rotateSensitivity = 3;
    private readonly float deceleration = 1000;
    private float acceleration;
    private readonly int accTimeFrame = 15;
    private ObjectBuffer<float> velocityBuffer;
    private ObjectBuffer<float> deltatimeBuffer;
    private bool rotateAreaClicked;

    void Start()
    {
        velocityBuffer = new ObjectBuffer<float>(accTimeFrame);
        deltatimeBuffer = new ObjectBuffer<float>(accTimeFrame);

        CenterCamera();
    }

    void Update()
    {
        RotateCamera();
    }

    public void CenterCamera()
    {
        var level = PuzzleManager.Instance.CurrentLevel;
        if(level != null)
        {
            var center = Vector3.zero;
            foreach(var cube in level.GetCubes())
            {
                center += cube.position;
            }
            center /= level.CountCube();

            transform.position = center;
        }
    }

    private void RotateCamera()
    {
        if(Input.GetMouseButtonDown(0))
        {
            acceleration = 0;
            velocityBuffer.Clear();
            deltatimeBuffer.Clear();
        }

        if (Input.GetMouseButton(0) && rotateAreaClicked)
        {
            // take the average velocity over the last [accTimeFrame] frames 
            // and calculate the current acceleration using it
            float distance = Input.GetAxis("Mouse X") * rotateSensitivity;
            velocityBuffer.Add(distance / Time.deltaTime);
            deltatimeBuffer.Add(Time.deltaTime);
            float velocitySum = 0, timeSum = 0;
            for(int i = 0; i < velocityBuffer.Count; i++)
            {
                velocitySum += velocityBuffer[i];
                timeSum += deltatimeBuffer[i];
            }
            acceleration = velocitySum / timeSum;
        }
        else
        {
            if(acceleration > 0)
            {
                acceleration -= deceleration;
                acceleration = Mathf.Max(0, acceleration);
            }
            else if(acceleration < 0)
            {
                acceleration += deceleration;
                acceleration = Mathf.Min(0, acceleration);
            }

            rotateAreaClicked = false;
        }

        var angles = transform.eulerAngles;
        angles.y += acceleration * Time.deltaTime * Time.deltaTime;
        transform.eulerAngles = angles;
    }

    public void OnRotateStartAreaClicked()
    {
        rotateAreaClicked = true;
    } 
}
