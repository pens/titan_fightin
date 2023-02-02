using UnityEngine;
using System.Collections;

public class Flashable : MonoBehaviour {

    public Material flashMaterial;
    private Material originalMaterial;
    private Renderer mr;
    private const float flashDuration = 0.005f;
    private const float flickerFlashDuration = 0.02f;
    private const float flickerDuration = .5f;
    private float flashTime;
    private float flickerTime;
    private bool flashing;
    private bool flickering;

    void Start () {
        //must accept skinned meshes as well
        mr = GetComponent<Renderer>();
        originalMaterial = mr.sharedMaterial;
	}

    public void Flash()
    {
        mr.material = flashMaterial;
        flashTime = 0;
        flashing = true;
        flickering = false;
    }

    public void Flicker()
    {
        mr.material = flashMaterial;
        flickerTime = 0;
        flashTime = 0;
        flickering = true;
        flashing = false;
    }

    void FixedUpdate () {
        flashTime += Time.deltaTime;
        flickerTime += Time.deltaTime;
        if (flashing && flashTime > flashDuration)
        {
            flashing = false;
            mr.material = originalMaterial;
        }
        else if (flickering)
        {
            if (flickerTime > flickerDuration)
            {
                flickering = false;
                mr.material = originalMaterial;
            } else if (flashTime > flickerFlashDuration)
            {

                flashTime = 0;
                if (mr.sharedMaterial == originalMaterial)
                {
                    mr.material = flashMaterial;
                } else
                {
                    mr.material = originalMaterial;
                }
            }
        }
    }
}
