using System.Collections.Generic;
using UnityEngine;
public class MovesDB
{
    static Dictionary<string, MoveBase> moves;
    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        var moveArr = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArr)
        {
            if (moves.ContainsKey(move.moveName))
            {
                continue;
            }
            moves[move.moveName] =  move;
        }
    }
    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            return null;
        }
        return moves[name];
    }
}