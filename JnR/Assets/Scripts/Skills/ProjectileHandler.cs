using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileHandler : MonoBehaviour 
{
    public PlayerObject _origin;
    public PlayerObject _target;
    public Skill _skill;
    public GameManager _gameManager;

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log(this);

        if (Network.isServer)
        {
            if (_skill._penetrationProjectile && (_target._playerPrefab.gameObject != collider.gameObject))
            {
                Debug.Log("Something one the way");
                _gameManager.S_ApplyProjectileEffect(_origin, _target, _skill._effects);    
            }
            else if (_target._playerPrefab.gameObject == collider.gameObject)
            {
                Debug.Log("The target");
                _gameManager.S_ApplyProjectileEffect(_origin, _target, _skill._effects);
                Destroy(gameObject);
            }
        }
        else 
        {
            if (_target._playerPrefab.gameObject == collider.gameObject)
            {
                Debug.Log("Destroy effect on client");
                Destroy(gameObject);
            }
        }
    }	
}
