using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour 
{
    public Color _fadeColor = Color.clear;
    public float _fadeDuration = 1.0F;
    public float _textSpeed = 1.0f;
    public float _destroyDuration = 3.0f;

    void Start() 
    {
        Destroy(gameObject, _destroyDuration);
    }

    void Update()
    {
        float lerp = Mathf.PingPong(Time.time, _fadeDuration) / _fadeDuration;
        renderer.material.color = Color.Lerp(renderer.material.color, _fadeColor, lerp);
        transform.Translate(Vector3.up * _textSpeed * Time.deltaTime, Space.World);
    }
}
