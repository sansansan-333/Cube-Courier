using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotControl : MonoBehaviour
{
    private readonly float rotateMouseSensitivity = 3;
    private readonly float mouseDeceleration = 1000;
    private float mouseAcceleration;
    private readonly int accTimeFrame = 15;
    private ObjectBuffer<float> velocityBuffer;
    private ObjectBuffer<float> deltatimeBuffer;
    private readonly float rotateKeySensitivity = 0.04f;
    private readonly float keyDeceleration = 1000;
    private readonly float keyMaxAcceleration = 50000;
    private float keyAcceleration;
    private bool rotateAreaClicked;

    void Start()
    {
        velocityBuffer = new ObjectBuffer<float>(accTimeFrame);
        deltatimeBuffer = new ObjectBuffer<float>(accTimeFrame);

        CenterCamera();
    }

    void Update()
    {
        RotateCameraByArrows();
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

    private void RotateCameraByMouse()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mouseAcceleration = 0;
            velocityBuffer.Clear();
            deltatimeBuffer.Clear();
        }

        if (Input.GetMouseButton(0) && rotateAreaClicked)
        {
            // take the average velocity over the last [accTimeFrame] frames 
            // and calculate the current acceleration using it
            float distance = Input.GetAxis("Mouse X") * rotateMouseSensitivity;
            velocityBuffer.Add(distance / Time.deltaTime);
            deltatimeBuffer.Add(Time.deltaTime);
            float velocitySum = 0, timeSum = 0;
            for(int i = 0; i < velocityBuffer.Count; i++)
            {
                velocitySum += velocityBuffer[i];
                timeSum += deltatimeBuffer[i];
            }
            mouseAcceleration = velocitySum / timeSum;
        }
        else
        {
            // decelerate
            if(mouseAcceleration > 0)
            {
                mouseAcceleration -= mouseDeceleration;
                mouseAcceleration = Mathf.Max(0, mouseAcceleration);
            }
            else if(mouseAcceleration < 0)
            {
                mouseAcceleration += mouseDeceleration;
                mouseAcceleration = Mathf.Min(0, mouseAcceleration);
            }

            rotateAreaClicked = false;
        }

        var angles = transform.eulerAngles;
        angles.y += mouseAcceleration * Time.deltaTime * Time.deltaTime;
        transform.eulerAngles = angles;
    }

    private void RotateCameraByArrows()
    {
        bool arrowKeyPressed = false;

        if(Input.GetKey("left"))
        {
            keyAcceleration -= rotateKeySensitivity / (Time.deltaTime * Time.deltaTime);
            arrowKeyPressed = true;
        }
        if(Input.GetKey("right"))
        {
            keyAcceleration += rotateKeySensitivity / (Time.deltaTime * Time.deltaTime);
            arrowKeyPressed = true;
        }

        if(!arrowKeyPressed)
        {
            // decelerate
            if (keyAcceleration > 0)
            {
                keyAcceleration -= keyDeceleration;
                keyAcceleration = Mathf.Max(0, keyAcceleration);
            }
            else if (keyAcceleration < 0)
            {
                keyAcceleration += keyDeceleration;
                keyAcceleration = Mathf.Min(0, keyAcceleration);
            }
        }

        // limit speed
        keyAcceleration = Mathf.Sign(keyAcceleration) * Mathf.Min(Mathf.Abs(keyAcceleration), keyMaxAcceleration);

        var angles = transform.eulerAngles;
        angles.y += keyAcceleration * Time.deltaTime * Time.deltaTime;
        transform.eulerAngles = angles;
    }

    public void OnRotateStartAreaClicked()
    {
        rotateAreaClicked = true;
    } 
}
