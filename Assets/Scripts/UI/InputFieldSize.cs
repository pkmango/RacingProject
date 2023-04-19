using UnityEngine;
using UnityEngine.UI;

public class InputFieldSize : MonoBehaviour
{
    public string defaultName = "Player";

    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private float paddingWdth = 256f;

    private RectTransform rt;
    private InputField nameField;
    private float minWidth;

    void Awake()
    {
        nameField = GetComponent<InputField>();
        rt = GetComponent<RectTransform>();
        minWidth = rt.sizeDelta.x;
    }

    public void Resize(string newName)
    {
        if (newName != "")
        {
            float newWidth = nameTxt.preferredWidth + paddingWdth;

            if (newWidth <= minWidth)
                newWidth = minWidth;

            rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
        }
        else
        {
            if (defaultName != "")
            {
                nameField.text = defaultName;
                Resize(defaultName);
            }
            else
            {
                Debug.Log("defaultName is empty!");
            }
        }
    }

    //public void InputFieldNewValue(string txt)
    //{
    //    Debug.Log(txt);
    //}
}
