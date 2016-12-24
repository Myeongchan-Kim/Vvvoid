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

    public static void ChangeFoodPositon(GameObject food, GameObject player, Vector3 newPos)
    {
        food.transform.DOMove(new Vector3(newPos.x, newPos.y, food.transform.position.z), duration);
    }

    
    public static void ScaleChange(GameObject obj, float targetScale)
    {
        obj.transform.DOScale(targetScale, duration);
    }

    public static void SuckingEffectPlay(ParticleSystem ps, Food food)
    {
        float lifeTime = ps.startLifetime;

        ps.transform.position = food.transform.position;
        ps.transform.DOMove(new Vector3(0f, 0f, 0f), lifeTime);
        ps.transform.localScale = food.transform.localScale / 5.0f;
        ps.Play();

        food.transform.position = new Vector3(-100f, -100f);
    }
}
