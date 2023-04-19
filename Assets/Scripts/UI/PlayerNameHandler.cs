using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(InputField))]
public class PlayerNameHandler : MonoBehaviour
{
    public string defaultName = "Player";

    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private float paddingWdth = 256f;

    private RectTransform rt;
    private InputField nameField;
    private float minWidth;

    void Awake()
    {
        DefaultNameCheck();

        nameField = GetComponent<InputField>();
        if (nameField == null)
            Debug.Log("InputField not found");

        rt = GetComponent<RectTransform>();
        if (rt == null)
            Debug.Log("RectTransform not found");

        minWidth = rt.sizeDelta.x;

        FirstTimeSetName();
    }

    private void DefaultNameCheck()
    {
        if (string.IsNullOrEmpty(defaultName))
        {
            defaultName = "Player";
        }
    }

    private void FirstTimeSetName()
    {
        if (string.IsNullOrEmpty(playerData.Name))
        {
            playerData.Name = defaultName;
        }

        nameField.text = playerData.Name;
        Resize();
    }

    public void SetName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            newName = defaultName;
            nameField.text = newName;
        }

        playerData.Name = newName;
        Resize();
    }

    private void Resize()
    {
        float newWidth = nameTxt.preferredWidth + paddingWdth;

        if (newWidth <= minWidth)
            newWidth = minWidth;

        rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
    }
}
