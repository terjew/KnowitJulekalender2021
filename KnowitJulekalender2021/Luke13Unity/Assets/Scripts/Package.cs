using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Move
{
    public Vector3Int Offset { get; }
    protected Move(Vector3Int offset)
    {
        Offset = offset;
    }

    public static Move North = new Move(Vector3Int.forward);
    public static Move South = new Move(Vector3Int.back);
    public static Move West = new Move(Vector3Int.left);
    public static Move East = new Move(Vector3Int.right);
    public static Move Down = new Move(Vector3Int.down);
    public static Move None = new Move(Vector3Int.zero);
}

public class Package : MonoBehaviour
{
    public string Moves;
    public int MoveNo = 0;
    public Vector3Int IntPos;

    public void Start()
    {
        GetComponentInChildren<Renderer>().material.color = Random.ColorHSV();
    }

    public Move GetNextMove()
    {
        if (MoveNo >= Moves.Length) return Move.None;
        var c = Moves[MoveNo++];
        switch (c)
        {
            case 'N': return Move.North;
            case 'S': return Move.South;
            case 'E': return Move.East;
            case 'W': return Move.West;
        }
        return Move.None;
    }

    public void Update()
    {
        transform.localPosition = IntPos;
    }
}
