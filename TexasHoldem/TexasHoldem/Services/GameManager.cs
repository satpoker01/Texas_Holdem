﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldem.Services
{
    class GameManager
    {
        private readonly GameCenter gameCenter;

        public GameManager()
        {
            gameCenter = GameCenter.GetGameCenter();
        }

        public Room CreateGame(string roomName, string creatorUserName, string creatorName, Gametype gameType, int buyInPolicy, int chipPolicy, int minBet, int minPlayers, int maxPlayers,
            bool spectating) // UC 5
        {
            return gameCenter.CreateRoom(roomName, creatorUserName, creatorName, gameType, buyInPolicy, chipPolicy, minBet, minPlayers, maxPlayers,
            spectating);
        }

        public bool IsRoomExist(string roomName)
        {
            return gameCenter.IsRoomExist(roomName);
        }

        public void JoinGame(string username, string roomName, string playerName) // UC 6
        {
            gameCenter.AddUserToRoom(username, roomName, false, playerName);
        }

        public void SpectateGame(string username, string roomName, string playerName) // UC 7
        {
            gameCenter.AddUserToRoom(username, roomName, true);
        }

        public void LeaveGame(string username, string roomName, string playerName) // UC 8
        {
            gameCenter.RemoveUserFromRoom(username, roomName, playerName);
        }

        public List<string> FindGames(string username, string playerName, bool playerFlag, int potSize, bool potFlag,
            Gametype gameType, int buyInPolicy, int chipPolicy, int minBet, int minPlayers, int maxPlayers,
            bool spectating, bool prefFlag, bool leagueFlag) // UC 11
        {
            return gameCenter.FindGames(username, playerName, playerFlag, potSize, potFlag,
                gameType, buyInPolicy, chipPolicy, minBet, minPlayers, maxPlayers,
                spectating, prefFlag, leagueFlag);
        }

        public List<string> FindGames(string username) // UC 11 (Finds any available game)
        {
            return gameCenter.FindGames(username, "", false, 0, false, Gametype.NoLimit, 0, 10, 4, 3, 7, false, false,
                false);
        }

        public void PlayGame() // UC 12
        {
            
        }

        public void PlaceBets() // UC 13
        {
            
        }

        public void SetDefaultRank(string username, int rank) // UC 14
        {
            gameCenter.SetDefaultRank(username, rank);
        }

        public void SetExpCriteria(string username, int exp) // UC 14
        {
            gameCenter.SetExpCriteria(username, exp);
        }

        public void SetUserLeague(string username, string usernameToSet, int rank) // UC 14
        {
            gameCenter.SetUserRank(username, usernameToSet, rank);
        }

        public bool restartGameCenter()
        {
            try
            {
                gameCenter.DeleteAllRooms();
                gameCenter.DeleteAllUsers();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}