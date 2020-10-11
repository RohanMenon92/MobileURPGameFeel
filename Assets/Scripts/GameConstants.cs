using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    public enum ProjectileType
    {
        Pusher,
        Puller,
        Imploder,
        Exploder,
        MultiHit
    }

    public enum ProjectileColour
    {
        Blue,
        Green,
        Grey,
        Orange,
        Pink,
        Red,
        Yellow
    }

    public static int RagDollCount = 6;

    public static float pullForce = 1.0f;
    public static float pushForce = 15.0f;
    public static float explosionForce = 2f;
    public static float implosionForce = 10.0f;
    public const float multiHitForce = 3.0f;
    public const float multiHitTime = 0.5f;
}
