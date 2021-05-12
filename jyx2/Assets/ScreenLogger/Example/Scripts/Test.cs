using UnityEngine;
using System.Collections;
using AClockworkBerry;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    float timeout = 5, lasttime = -1;

    void Start()
    {
        ScreenLogger.Instance.ShowLog = true;

        TestMessageTypes();
    }

    void Update()
    {
        if (timeout > 0)
            timeout -= Time.deltaTime;

        if ((int)lasttime != (int)timeout && timeout > 0)
            Debug.Log("-" + ((int)timeout + 1));

        lasttime = timeout;

        if (timeout <= 0 && timeout != -1)
        {
            timeout = -1;
            Debug.Log("Loading new scene...");
            SceneManager.LoadScene("ExampleScene");
        }
    }

    void TestMessageTypes()
    {
        Debug.Log("Log message...");
        Debug.LogWarning("Warning message...");
        Debug.LogError("Error message...");
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