using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MultiplayerMessage
{
    public string type;
    public string playerId;
    public float horizontalAxis;
    public float varticalAxis;
    public float health;
    public string[] actions;
    public Position position;

    public MultiplayerMessage()
    {

    }

    public MultiplayerMessage(string type, string playerId, float horizontalAxis, float verticalAxis, string[] actions, Vector3 position, Vector3 rotation, int sceneIndex, float health = 0)
    {
        this.type = type;
        this.playerId = playerId;
        this.horizontalAxis = horizontalAxis;
        this.varticalAxis = verticalAxis;
        this.health = health;
        this.actions = actions;
        this.position = new Position(position, rotation, sceneIndex);
    }

    [System.Serializable]
    public class Position
    {
        public Vector3 position;
        public Vector3 rotation;
        public int sceneIndex;

        public Position(Vector3 position, Vector3 rotation, int sceneIndex)
        {
            this.position = position;
            this.rotation = rotation;
            this.sceneIndex = sceneIndex;
        }
    }
}
