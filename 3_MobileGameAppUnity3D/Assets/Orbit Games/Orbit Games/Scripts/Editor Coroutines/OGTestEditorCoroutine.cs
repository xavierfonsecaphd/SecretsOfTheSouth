using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGTestEditorCoroutine : MonoBehaviour
{
    [Buttons("testRoutine", "testRoutine", "stopRoutine", "stopRoutine")]
    public ButtonsContainer test;
    public int coroutineID;
    public void testRoutine()
    {
        coroutineID = OGEditorCoroutines.Run(CoroutineTest());
    }
    private IEnumerator CoroutineTest()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEditorSeconds(1);
            Debug.Log(i);
        }
    }
    public void stopRoutine()
    {
        OGEditorCoroutines.Stop(coroutineID);
    }
}