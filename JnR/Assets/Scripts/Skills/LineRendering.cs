using UnityEngine;
using System.Collections;

public class LineRendering : MonoBehaviour
{
    public float arcLength = 2.0f;
    public float arcVariation = 2.0f;
    public float inaccuracy = 1.0f;
    public LineRenderer line;

    public void DoEffect(Vector3 source, Vector3 target)
    {
        Vector3 lastPoint = source;
        // = gameObject.AddComponent<LineRenderer>();
        int i = 1;

        line.SetPosition(0, source);

        while (Vector3.Distance(target, lastPoint) > .5)
        {
            line.SetVertexCount(i + 1);

            Vector3 fwd = target - lastPoint;//gives the direction to our target from the end of the last arc

            fwd.Normalize();
            fwd = Randomize(fwd, inaccuracy);//we don't want a straight line to the target though
            fwd *= Random.Range(arcLength * arcVariation, arcLength);//nature is never too uniform
            fwd += lastPoint;//point + distance * direction = new point. this is where our new arc ends

            line.SetPosition(i, fwd);

            i++;
            lastPoint = fwd;
        }
    }

    private Vector3 Randomize(Vector3 v3, float inaccuracy2)
    {
        v3 += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * inaccuracy2;
        v3.Normalize();

        return v3;
    }
}
