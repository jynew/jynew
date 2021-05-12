using UnityEngine;
using System.Collections;
using AClockworkBerry;

public class BouncingBall : MonoBehaviour {
    public float Acceleration = -1;
    public float HorizVelocity = 1;
    public float Wall = 5;
    public float Radius = 1;
    public float RotationSpeed = 1;

    private Vector3 Velocity;
    private float MaxVelocity;

	void Start () {
        Velocity = new Vector3(HorizVelocity, 0, 0);

        MaxVelocity = Mathf.Sqrt(2 * -Acceleration * (transform.position.y - Radius));
        transform.position = new Vector3(0, transform.position.y, 0);

        Debug.LogWarning("Start...");
	}
	
	void Update () {
        if (Input.GetButtonDown("Fire1"))            
            ScreenLogger.Instance.ShowLog = !ScreenLogger.Instance.ShowLog;

        Velocity.y += Acceleration * Time.deltaTime;
        transform.position += Velocity * Time.deltaTime;

        if (transform.position.y - Radius < 0)
        {
            transform.position = new Vector3(transform.position.x, Radius, 0);
            Velocity.y = MaxVelocity;

            Debug.Log("Boing at " + transform.position.x + "...");
        }

        if (Mathf.Abs(transform.position.x) + Radius > Wall)
        {
            transform.position = new Vector3(
                transform.position.x > 0 ? Wall - Radius : -Wall + Radius,
                transform.position.y,
                0);

            Velocity.x *= -1;
        }

        transform.Rotate(new Vector3(0, RotationSpeed * Time.deltaTime, 0));
	}
}

/*
The MIT License

Copyright © 2016 Screen Logger - Giuseppe Portelli <giuseppe@aclockworkberry.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/