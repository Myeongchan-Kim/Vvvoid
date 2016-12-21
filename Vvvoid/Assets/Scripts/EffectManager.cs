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
        //float xOffset = (food.transform.position.x - player.transform.position.x);
        //float yOffset = (food.transform.position.y - player.transform.position.y);

        //xOffset *= deltaScale;
        //yOffset *= deltaScale;
        
        food.transform.DOMove(new Vector3(newPos.x, newPos.y, food.transform.position.z), duration);
    }

    //public static void MetorUpScale(GameObject meteor, GameObject player)
    //{
    //    //MetorChangingScaleMove(meteor, 2.0f, player);
    //}
    //public static void MeteorDownScale(GameObject meteor, GameObject player)
    //{
    //    //MetorChangingScaleMove(meteor, duration, player);
    //}

    public static void ScaleChange(GameObject obj, float targetScale)
    {
        obj.transform.DOScale(targetScale, duration);
    }

    public static void SuckingEffectPlay(ParticleSystem ps, Food food)
    {
        float lifeTime = ps.startLifetime;

        ps.transform.position = food.transform.position;
        ps.transform.DOMove(new Vector3(0f, 0f, 0f), lifeTime);

        ps.Play();

        food.transform.position = new Vector3(-100f, -100f);
    }
}
