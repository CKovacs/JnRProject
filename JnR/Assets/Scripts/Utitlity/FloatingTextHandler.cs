using UnityEngine;
using System.Collections;

public class FloatingTextHandler : MonoBehaviour 
{
    public GameObject _floatingText;

    public void CreateFloatingAmountText(Vector3 position, int amount) 
    {
        GameObject newFloatingText = (GameObject)Instantiate(_floatingText);

        newFloatingText.transform.position = position;

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
    }

    public void CreateFloatingSpecialText(Vector3 position, string type)
    {
        GameObject newFloatingText = (GameObject)Instantiate(_floatingText);

        newFloatingText.transform.position = position;

        TextMesh textMesh = newFloatingText.GetComponent<TextMesh>();

        textMesh.text = type;
        textMesh.color = Color.cyan;
    }
}