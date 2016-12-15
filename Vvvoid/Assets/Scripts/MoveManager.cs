using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveManager : MonoBehaviour {
    public void Start()
    {
        DOTween.Init();
    }

    public static void MetorChangingScaleMove(GameObject resource, float wheelIntensity , GameObject player)
    {
        float xOffset = (resource.transform.position.x - player.transform.position.x);
        float yOffset = (resource.transform.position.y - player.transform.position.y);
        Vector3 newScale = resource.transform.localScale;

        if (wheelIntensity > 0)
        {
            newScale *= 2;
            xOffset *= 2;
            yOffset *= 2;
        }
        else if(wheelIntensity < 0)
        {
            newScale /= 2;
            xOffset /= 2;
            yOffset /= 2;
        }

        resource.transform.DOMove(new Vector3(xOffset, yOffset, resource.transform.position.z), 0.5f);
        resource.transform.DOScale(newScale, 0.5f);
    }

    public void ScaleChange(GameObject obj, float targetScale)
    {

    }
}
