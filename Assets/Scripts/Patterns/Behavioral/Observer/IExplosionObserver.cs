using UnityEngine;
public interface IExplosionObserver
{
    void OnExplosion(Vector2 explosionPosition, int power);
}

//patlama olduysa yerini ve gücünü haber verir