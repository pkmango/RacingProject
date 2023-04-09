using UnityEngine;

public class PlacesForRespawn : MonoBehaviour
{
    // Массив с вариантами смещения относительно центра при респауне
    public Vector3[] placesForRespawn;
    // Индекс массива
    private int index = 0;

    public Vector3 Place()
    {
        Vector3 place = placesForRespawn[index];
        
        if (index < placesForRespawn.Length - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        return place;
    }
}
