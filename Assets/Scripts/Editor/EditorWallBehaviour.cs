using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallBehaviour))]
public class EditorWallBehaviour : Editor
{
  [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
  public static void RenderCustomGizmo(WallBehaviour wall, GizmoType gizmo)
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(wall.transform.position, wall.Collider.size);
  }
}