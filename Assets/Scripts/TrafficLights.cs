using System.Collections;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    [SerializeField, Tooltip("Ассет для события из папки Scripts/Events")]
    private SoundEvent onPlaySoundRequest;
    [SerializeField]
    private float delay = 1.0f;
    [SerializeField]
    private SoundType TrafficLightsSFXRed = SoundType.TrafficLigts1;
    [SerializeField]
    private SoundType TrafficLightsSFXGreen = SoundType.TrafficLigts2;
    [SerializeField]
    private int signalsNumber = 3;

    private void OnEnable()
    {
        StartCoroutine(PlaySFX());
    }

    private IEnumerator PlaySFX()
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < signalsNumber - 1; i++)
        {
            if (onPlaySoundRequest != null)
                onPlaySoundRequest.Raise(TrafficLightsSFXRed, transform.position, 0f);

            yield return new WaitForSeconds(delay);
        }

        onPlaySoundRequest.Raise(TrafficLightsSFXGreen, transform.position, 0f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
