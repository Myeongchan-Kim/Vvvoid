using UnityEngine;
using System.Collections;

public class MoveManager : MonoBehaviour {

	public static void MetorChangingScaleMove(GameObject resource, float d , GameObject player)
    {
        if( d > 0)
        {
            resource.transform.localScale *= 2;

            float x = (resource.transform.position.x - player.transform.position.x) * 2;
            float y = (resource.transform.position.y - player.transform.position.y) * 2;
            resource.transform.position = new Vector3(x, y, resource.transform.position.z);

        }
        else if(d < 0)
        {
            resource.transform.localScale /= 2;
            float x = (resource.transform.position.x - player.transform.position.x) / 2;
            float y = (resource.transform.position.y - player.transform.position.y) / 2;
            resource.transform.position = new Vector3(x, y, resource.transform.position.z);
        }
    }
}
