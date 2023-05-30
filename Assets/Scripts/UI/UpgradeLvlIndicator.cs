using UnityEngine;
using UnityEngine.UI;

public class UpgradeLvlIndicator : MonoBehaviour
{
    [SerializeField]
    private Image[] lvlPoints;
    [SerializeField]
    private Color onColor, offColor;

    public void SetLvlIndicator(int level)
    {
        for(int i = 0; i < lvlPoints.Length; i++)
        {
            if (i < level)
            {
                lvlPoints[i].color = onColor;
            }
            else
            {
                lvlPoints[i].color = offColor;
            }
        }
    }
}
