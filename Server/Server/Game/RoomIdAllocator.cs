using System;
using System.Collections.Generic;
using WX.Utils;

namespace WX.Game;

public static class RoomIdAllocator
{
    const string LETTERTABLE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static HashSet<string> roomIdSet = new();
    private const int MAXATTEMPTCOUNT = 10;

    public static string? AllocateId()
    {
        for (int i = 0; i < MAXATTEMPTCOUNT; i++)
        {
            var r = Generate();
            if (roomIdSet.Contains(r))
            {
                continue;
            }
            roomIdSet.Add(r);
            return r;
        }

        return null;
    }

    public static void OnRemoveRoom(string roomId)
    {
        if (roomIdSet.Contains(roomId))
        {
            roomIdSet.Remove(roomId);
        }
    }

    private static string Generate()
    {
        var id = new char[6];
        Random r = new Random();
        for (int i = 0; i < id.Length; i++)
        {
            var a = (char) r.Next(65, 91);
            id[i] = a;
        }

        var res = new string(id);
        return res;
    }
}