using UnityEngine;

public class ExplosionVisual : MonoBehaviour
{
    public float lifeTime = 0.5f;

    void OnEnable()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("❌ Explosion'da SpriteRenderer yok!");
            return;
        }

        // 🔥 HER TÜRLÜ GÖRÜNME ENGELİNİ KALDIR
        sr.enabled = true;
        sr.color = Color.white;                 // alpha = 1
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 999;                  // her şeyin üstü
        
        // 🔥 SCALE VE Z POZİSYONU GARANTİ
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            0f                               // kamera önünde
        );

        // 🔥 KAMERAYA ZORLA BAKTIR
        transform.rotation = Quaternion.identity;

        Destroy(gameObject, lifeTime);
    }
}
