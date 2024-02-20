using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDefinition
{
    Match3Item[,] boardDef;

    public int Width { get; set; }
    public int Height { get; set; }

    public BoardDefinition(int width, int height)
    {
        Width = width;
        Height = height;

        boardDef = new Match3Item[width, height];
    }

    public Match3Item[,] InitializeRandomBoard(bool matchesOk = false)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                boardDef[x, y] = MakeRandomItem();
            }
        }
        if (matchesOk) return boardDef;
        return FindAndReplaceMatches(boardDef);
    }

    public Match3Item MakeRandomItem()
    {
        return new Match3Item((Match3ItemType)Random.Range(0, System.Enum.GetValues(typeof(Match3ItemType)).Length), false);
    }

    public Match3Item[,] FindAndReplaceMatches(Match3Item[,] board)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (board[x, y] != null)
                {
                    HashSet<Vector2Int> matchedItems = IsConnected(new Vector2Int(x, y));
                    if (matchedItems.Count > 0)
                    {
                        //merge the matched items into the matches hashset
                        foreach (var matchedItem in matchedItems)
                        {
                            matches.Add(new Vector2Int(matchedItem.x, matchedItem.y));
                        }
                    }
                }
            }
        }

        if (matches.Count > 0)
        {
            foreach (Vector2Int item in matches)
            {
                board[item.x, item.y] = MakeRandomItem();
            }
            FindAndReplaceMatches(board); // Recursively continue fixing the board
        }
        return board;
    }

    HashSet<Vector2Int> IsConnected(Vector2Int item)
    {
        HashSet<Vector2Int> connectedItems = new() { item };

        //check horizontal
        CheckDirection(item, new Vector2Int(1, 0), connectedItems);
        CheckDirection(item, new Vector2Int(-1, 0), connectedItems);

        if (connectedItems.Count >= 3)
        {
            return connectedItems;
        }

        //reset the list
        connectedItems.Clear();
        connectedItems.Add(item);

        //check vertical
        CheckDirection(item, new Vector2Int(0, 1), connectedItems);
        CheckDirection(item, new Vector2Int(0, -1), connectedItems);
        if (connectedItems.Count >= 3)
        {
            return connectedItems;
        }
        return new HashSet<Vector2Int>();
    }

    void CheckDirection(Vector2Int item, Vector2Int direction, HashSet<Vector2Int> connectedItems)
    {
        Match3Item thisItem = boardDef[item.x, item.y];
        int x = item.x + direction.x;
        int y = item.y + direction.y;

        while (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            Match3Item neighborItem = boardDef[x, y];
            if (neighborItem.Equals(thisItem))
            {
                connectedItems.Add(new Vector2Int(x, y));
                x += direction.x;
                y += direction.y;
                continue;
            }
            break;
        }
    }
}
