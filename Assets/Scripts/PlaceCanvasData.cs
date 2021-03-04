using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceCanvasData : MonoBehaviour
{
    public Text racerName;
    public Text racerTime;

    public void SetPlaceCanvasData(string name, string time)
    {
        racerName.text = name;
        racerTime.text = time;
    }

}
