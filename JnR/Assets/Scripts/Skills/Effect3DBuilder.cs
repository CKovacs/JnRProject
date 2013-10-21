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
        newSource += _skill._spellOffSetSource;

        Vector3 newTarget = destination.position;

        newTarget += _skill._spellOffSetTarget;

        switch (_skill._3dEffectType)
        {
            case Effect3DType.CharacterEffect:
                GameObject characterEffect = GameObject.Instantiate(_skill._3dEffect) as GameObject;

                characterEffect.transform.position = newTarget;

                Object.Destroy(characterEffect, _skill._destroyTime);
                break;
            case Effect3DType.Projectile:
                GameObject projectile = GameObject.Instantiate(_skill._3dEffect) as GameObject;

                projectile.transform.position = newSource;

                Vector3 position = Vector3.Lerp(projectile.transform.position, newTarget, Time.deltaTime * 10.1f);
                projectile.transform.position = position;
                Object.Destroy(projectile, _skill._destroyTime);
                projectile.rigidbody.AddRelativeForce((newTarget - newSource) * 200);
                  
                break;
            case Effect3DType.Streaming:
                GameObject streaming = GameObject.Instantiate(_skill._3dEffect) as GameObject;
                Stream stream = streaming.GetComponent<Stream>();
                stream.DoEffect(newSource, newTarget);

                Object.Destroy(streaming, _skill._destroyTime);
                break;
            case Effect3DType.Strike:
                GameObject strike = GameObject.Instantiate(_skill._3dEffect) as GameObject;

                strike.transform.position = newSource;

                Strike striking = strike.GetComponent<Strike>();
                striking.DoEffect(newSource, newTarget);

                Object.Destroy(strike, _skill._destroyTime);
                break;
        }
    }
}
