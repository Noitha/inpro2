using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bobo_Defender.Scripts
{
    public static class GameState
    {
        public const string Url = /*"http://192.168.132.212"*/ "http://192.168.178.194:3000";
        
        public static UserData userData { get; private set; }

        static GameState()
        {
            userData = null;
        }

        public static void ClearUserData()
        {
            userData = null;
        }

        public static void SetNewActiveSkin(int id)
        {
            userData.activeSkinId = id;
        }

        public static void SetNewLevelData(JSONObject levelData)
        {
            var l = new List<Level>();
            
            foreach (var level in levelData.list)
            {
                level.GetField(out var levelId, "id", 0);
                level.GetField(out var stars, "stars", 0);
                        
                l.Add(new Level
                {
                    id = levelId,
                    stars = stars
                });
            }

            userData.levels = l;
        }
        
        public static void SetUserData(JSONObject userDataa)
        {
            var us = new List<int>();
            var l = new List<Level>();
                
            userDataa.GetField(out var username, "username", "");
            userDataa.GetField(out var activeSkinId, "activeSkinId", 1);
            userDataa.GetField("unlockedSkins", delegate(JSONObject unlockedSkins)
            {
                foreach (var unlockedSkin in unlockedSkins.list)
                {
                    us.Add((int)unlockedSkin.i);
                }
            });
                
            userDataa.GetField("levels", delegate(JSONObject levels)
            {
                foreach (var level in levels.list)
                {
                    level.GetField(out var levelId, "id", 0);
                    level.GetField(out var stars, "stars", 0);
                        
                    l.Add(new Level
                    {
                        id = levelId,
                        stars = stars
                    });
                }
            });

            userData = new UserData
            {
                username = username,
                activeSkinId = activeSkinId,
                unlockedSkins = us,
                levels = l
            };
        }
    }
    
    [Serializable]
    public class UserData
    {
        public string username;
        public int activeSkinId;
        public List<int> unlockedSkins = new List<int>();
        public List<Level> levels = new List<Level>();

        public override string ToString()
        {
            return "{ username: " + username + " activeSkinId: " + activeSkinId + " unlockedSkins: " + unlockedSkins + " levels: " + levels +  " }";
        }
    }

    [Serializable]
    public class Level
    {
        public int id;
        public int stars;

        public override string ToString()
        {
            return "{ id: " + id + " stars:" + stars + " }";
        }
    }
}