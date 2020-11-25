using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor_PushButton : MonoBehaviour
{
    private bool doorOpened;
    private bool coroutineAllowed;
    // Start is called before the first frame update
    void Start()
    {
        doorOpened = false;
        coroutineAllowed = true;
        PushButton.ButtonPressed += RunCoroutine;
    }

    private void RunCoroutine()
    {
        StartCoroutine("OpenThatDoor");
    }

    private IEnumerator OpenThatDoor()
     {
        coroutineAllowed = false;
        if(!doorOpened)
        {
          
                transform.position = new Vector3(transform.position.x, transform.position.y+ 4.7f, transform.position.z);
                yield return new WaitForSeconds(0f);
      
            doorOpened = true;
        }
        else
        {

                transform.position = new Vector3(transform.position.x, transform.position.y - 4.7f, transform.position.z);
                yield return new WaitForSeconds(0f);
   
            doorOpened = false;
        }
        coroutineAllowed = true;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        PushButton.ButtonPressed -= RunCoroutine;
    }
}
