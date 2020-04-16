using System.Collections;

public class WaitForEditorSeconds : IEnumerator
{
    public object Current { get { return null; } }

    private float timeToEnd = 0;
    public WaitForEditorSeconds(float seconds)
    {
        timeToEnd = OGEditorCoroutines.Time + seconds;
    }

    public bool MoveNext()
    {
        return OGEditorCoroutines.Time < timeToEnd;
    }

    public void Reset()
    {
        
    }
}