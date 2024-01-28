using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBeawvior : MonoBehaviour
{
    public abstract Vector2 calculateMove(FloackAgent agent, List<Transform> context, Flock flock);

}
