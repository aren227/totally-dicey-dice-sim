using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public string partName;

    public float mass;

    public Vector3Int pos => Vector3Int.RoundToInt(transform.localPosition);

    public bool upAttachable = true;
    public bool downAttachable = true;
    public bool leftAttachable = true;
    public bool rightAttachable = true;
    public bool frontAttachable = true;
    public bool backAttachable = true;

    public bool upOpaque = true;
    public bool downOpaque = true;
    public bool leftOpaque = true;
    public bool rightOpaque = true;
    public bool frontOpaque = true;
    public bool backOpaque = true;

    public Side ToLocal(Side side) {
        Vector3Int v = side.GetVector();
        Vector3 rot = Quaternion.Inverse(transform.localRotation) * v;
        return SideExtensions.GetSide(Vector3Int.RoundToInt(rot));
    }

    public Side ToGlobal(Side side) {
        Vector3Int v = side.GetVector();
        Vector3 rot = transform.localRotation * v;
        return SideExtensions.GetSide(Vector3Int.RoundToInt(rot));
    }

    public bool CanAttach(Side side) {
        side = ToLocal(side);
        if (side == Side.UP) return upAttachable;
        if (side == Side.DOWN) return downAttachable;
        if (side == Side.LEFT) return leftAttachable;
        if (side == Side.RIGHT) return rightAttachable;
        if (side == Side.FRONT) return frontAttachable;
        return backAttachable;
    }

    public bool IsOpaque(Side side) {
        side = ToLocal(side);
        if (side == Side.UP) return upOpaque;
        if (side == Side.DOWN) return downOpaque;
        if (side == Side.LEFT) return leftOpaque;
        if (side == Side.RIGHT) return rightOpaque;
        if (side == Side.FRONT) return frontOpaque;
        return backOpaque;
    }
}

public enum Side {
    UP,
    DOWN,
    LEFT,
    RIGHT,
    FRONT,
    BACK,
}

public static class SideExtensions {
    public static Side GetOpposite(this Side dir) {
        if (dir == Side.UP) return Side.DOWN;
        if (dir == Side.DOWN) return Side.UP;
        if (dir == Side.LEFT) return Side.RIGHT;
        if (dir == Side.RIGHT) return Side.LEFT;
        if (dir == Side.FRONT) return Side.BACK;
        else return Side.FRONT;
    }

    public static Vector3Int GetVector(this Side dir) {
        if (dir == Side.UP) return Vector3Int.up;
        if (dir == Side.DOWN) return Vector3Int.down;
        if (dir == Side.LEFT) return Vector3Int.left;
        if (dir == Side.RIGHT) return Vector3Int.right;
        if (dir == Side.FRONT) return Vector3Int.forward;
        return Vector3Int.back;
    }

    public static Side GetSide(Vector3Int dir) {
        if (dir == Vector3Int.up) return Side.UP;
        if (dir == Vector3Int.down) return Side.DOWN;
        if (dir == Vector3Int.left) return Side.LEFT;
        if (dir == Vector3Int.right) return Side.RIGHT;
        if (dir == Vector3Int.forward) return Side.FRONT;
        return Side.BACK;
    }
}