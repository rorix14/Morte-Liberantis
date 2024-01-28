using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSnapshotMessage
{

    public string playerId;
    public Enemy[] enemies;
    public bool initialMessage;

    [System.Serializable]
    public class Enemy
    {
        public string enemyId;
        public string type;

        public Vector3 position;
        public Vector3 rotation;
        public Vector2 velocity;

        public float life;

        public Bullet[] bullets;
    }

    [System.Serializable]
    public class Bullet
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}
