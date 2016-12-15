using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EffectManager : MonoBehaviour
{
    static float duration = 0.5f;
    public void Start()
    {
        DOTween.Init();
    }

    public static void MetorChangingScaleMove(GameObject resource, float deltaScale, GameObject player)
    {
        float xOffset = (resource.transform.position.x - player.transform.position.x);
        float yOffset = (resource.transform.position.y - player.transform.position.y);
        Vector3 newScale = resource.transform.localScale;

        newScale *= deltaScale;
        xOffset *= deltaScale;
        yOffset *= deltaScale;

        resource.transform.DOMove(new Vector3(xOffset, yOffset, resource.transform.position.z), duration);
        resource.transform.DOScale(newScale, duration);
    }

    public static void MetorUpScale(GameObject resource, GameObject player)
    {
        MetorChangingScaleMove(resource, 2.0f, player);
    }
    public static void MeteorDownScale(GameObject resource, GameObject player)
    {
        MetorChangingScaleMove(resource, duration, player);
    }

    public static void ScaleChange(GameObject obj, float targetScale)
    {
        obj.transform.DOScale(targetScale, duration);
    }
}
