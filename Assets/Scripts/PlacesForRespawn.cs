using UnityEngine;

public class PlacesForRespawn : MonoBehaviour
{
    public Vector3[] placesForRespawn;

    private int index = 0;

    public Vector3 Place()
    {
        Vector3 place = placesForRespawn[index];
        Debug.Log(place);
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
