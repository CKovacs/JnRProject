using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingTextHandler : MonoBehaviour 
{
    public GameObject _floatingText;
    private List<GameObject> _gameObjectList = new List<GameObject>();

    private const float TOLERANCEDISTANCE = 0.8f; 
    private const float NEWY = 1.1f;

    public void CreateFloatingAmountText(Vector3 position, int amount) 
    {
        GameObject newFloatingText = (GameObject)Instantiate(_floatingText);

        newFloatingText.transform.position = TextReposition(position);

        TextMesh textMesh = newFloatingText.GetComponent<TextMesh>();

        textMesh.text = amount.ToString();
        
        if(amount >= 0)
        {
            textMesh.color = Color.green;
        }
        else
        {
            textMesh.color = Color.red;
        }

        _gameObjectList.Add(newFloatingText); 
    }

    public void CreateFloatingSpecialText(Vector3 position, EffectType type, int percentage)
    {
        GameObject newFloatingText = (GameObject)Instantiate(_floatingText);

        newFloatingText.transform.position = TextReposition(position);

        TextMesh textMesh = newFloatingText.GetComponent<TextMesh>();

        string text = string.Empty;

        switch (type) 
        {
            case EffectType.run:
                if (percentage >= 0)
                {
                    text = "speed";
                }
                else 
                {
                    text = "slowed";
                }

                break;
            case EffectType.stun:
                text = "stunned";

                break;
            case EffectType.def:
                if (percentage >= 0)
                {
                    text = "defence";
                }
                else
                {
                    text = "volnurable";
                }

                break;
        }

        textMesh.text = text;
        textMesh.color = Color.cyan;

        _gameObjectList.Add(newFloatingText); 
    }

    private Vector3 TextReposition(Vector3 position) 
    {
        GameObject lastElement = null;

        if (_gameObjectList.Count > 0)
        {
            lastElement = _gameObjectList[_gameObjectList.Count - 1];
        }

        if (lastElement != null)
        {
            if (Vector3.Distance(lastElement.transform.position, position) < TOLERANCEDISTANCE)
            {
                position.y = position.y - NEWY;
            }
        }

        return position;
    }
}