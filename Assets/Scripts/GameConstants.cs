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
    public enum EffectTypes
    {
        Ignition,
        Explosion,
        Implosion
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
    public static int EffectPoolSize = 20;

    public static float pullForce = 1.0f;
    public static float pushForce = 15.0f;
    public static float explosionForce = 2f;
    public static float implosionForce = 10.0f;
    public const float multiHitForce = 3.0f;
    public const float multiHitTime = 0.5f;

    public const float effectsTime = 1.0f;

    // UI Values
    public static float scrollFade = 1f;
    public static float windUpTransition = 1.5f;
    public static float colorTransition = 1.5f;

    public static Vector3 selectPos = new Vector3(-0.3f, 3f , 7f);
    public static Vector3 selectRot = new Vector3(45f, 180f, 0f);

    public static Vector3 windUpPos = new Vector3(-5f, 3f, 0f);
    public static Vector3 windUpRot = new Vector3(30f, 40f, 0f);

    public static Vector3 performAPos = new Vector3(-5f, 0f, 0f);
    public static Vector3 PerformARot = new Vector3(45f, 330f, 0f);

    public static Vector3 completeAPos = new Vector3(0f, 5f, 1f);
    public static Vector3 competeARot = new Vector3(30f, 180f, 0f);
}
