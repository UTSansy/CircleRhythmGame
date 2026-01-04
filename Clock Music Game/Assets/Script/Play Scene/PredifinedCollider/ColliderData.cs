using UnityEngine;

[CreateAssetMenu(fileName = "PredefinedCollider", menuName = "Game/PredefinedCollider")]
public class ColliderData : ScriptableObject {
    public Vector2[] maxColliderPath;
}