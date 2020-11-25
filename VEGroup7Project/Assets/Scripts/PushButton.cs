using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PushButton : MonoBehaviour
{
    public static event Action ButtonPressed = delegate { };
    private bool coroutineAllowed = true;

    private void OnMouseDown()
    {
        if (coroutineAllowed)
        {
            StartCoroutine("PushTheButton");
        }
    }

    private IEnumerator PushTheButton()
    {
        ButtonPressed();
        coroutineAllowed = false;
        for (int i = 0; i <= 20; i++)
        {
            transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y -0.1f, transform.localScale.z);
            yield return null;
        }
        for (int i = 0; i <= 20; i++)
        {
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 0.1f, transform.localScale.z);
            yield return null;
        }
        coroutineAllowed = true;
    }

}




