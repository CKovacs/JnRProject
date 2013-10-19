using UnityEngine;
using System.Collections;

public class Effect3DBuilder : MonoBehaviour
{
    public Transform source;
    public Transform destination;
    public Skill _skill;

    void Start() //Player source, Player destination, Skill _skill
    {
        InvokeRepeating("DoEffect", 2.0f, 4.0f);
    }

    public void DoEffect() 
    {
        Vector3 newSource = source.position;
        newSource += new Vector3(0, 1, 0);

        Vector3 newTarget = destination.position;

        newTarget += new Vector3(0, 1, 0);

        switch (_skill._3dEffectType)
        {
            case Effect3DType.CharacterEffect:
                break;
            case Effect3DType.Projectile:
                
                GameObject projectile = GameObject.Instantiate(_skill._3dEffect) as GameObject;

                projectile.transform.position = newSource;


                Vector3 position = Vector3.Lerp(projectile.transform.position, newTarget, Time.deltaTime * 10.1f);
                projectile.transform.position = position;
                Object.Destroy(projectile, 0.5f);
                projectile.rigidbody.AddRelativeForce((newTarget - newSource) * 200);
                  
                break;
            case Effect3DType.Streaming:
                GameObject streaming = GameObject.Instantiate(_skill._3dEffect) as GameObject;
                LineRendering stream = streaming.GetComponent<LineRendering>();
                stream.DoEffect(source.position, destination.position);

                Object.Destroy(streaming, 2.5f);
                break;
            case Effect3DType.Strike:
                break;
        }
    }
}
