using UnityEngine;
using System.Collections;

public class Effect3DBuilder : MonoBehaviour
{
    public static void DoEffect(Transform source, Transform destination, Skill _skill) 
    {
        Vector3 newSource = source.position;
        newSource += _skill._spellOffSetSource;

        Vector3 newTarget = destination.position;

        newTarget += _skill._spellOffSetTarget;

        switch (_skill._3dEffectType)
        {
            case Effect3DType.CharacterEffect:
                GameObject characterEffect = GameObject.Instantiate(_skill._3dEffect) as GameObject;
                Debug.Log("char effect");
                characterEffect.transform.position = newTarget;

                if (_skill._id == 2) 
                {
                    characterEffect.transform.forward = destination.forward;
                    characterEffect.transform.position = newTarget + destination.forward;
                }

                characterEffect.transform.parent = destination;

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
