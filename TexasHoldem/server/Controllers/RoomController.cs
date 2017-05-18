﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using Server.Models;
using Room = TexasHoldem.Game.Room;

namespace Server.Controllers
{
    public class RoomController : ApiController
    {
        public static Dictionary<string,Dictionary<string, List<RoomState>>> Replays =new Dictionary<string, Dictionary<string, List<RoomState>>>();

        // Put: /api/Room?game_name=moshe&player_name=kaki
        public RoomState Put(string gameName, string playerName) //get current status
        {
            Room r = null;
            var ans = new RoomState();
            try
            {
                r = Server.GameFacade.RoomStatus(gameName);
            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
            if (r != null) CreateRoomState(playerName, r, ans);
            return ans;
        }


        // GET: /api/Room?game_name=moshe&player_name=kaki
        public RoomState GET(string gameName, string playerName) //start game
        {
            Room r = null;
            var ans = new RoomState();
            try
            {
                r = Server.GameFacade.StartGame(gameName);
            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
            if (r != null) CreateRoomState(playerName, r, ans);
            return ans;
        }

        // GET: /api/Room?user_name=sean&game_name=moshe&player_name=kaki&option=join
        public RoomState GET(string userName ,string gameName, string playerName, string option)// join/spectate game// leave game
        {
            Room r = null;
            var ans = new RoomState();
            try
            {
                switch (option)
                {
                    case "join":
                        r = Server.GameFacade.JoinGame(userName, gameName, playerName);
                        if (r != null) Replays[r.Name].Add(playerName, new List<RoomState>());
                        break;
                    case "spectate":
                        r = Server.GameFacade.SpectateGame(userName, gameName, playerName);
                        break;
                    case "leave":
                        r = Server.GameFacade.LeaveGame(userName, gameName, playerName);
                        break;
                }
            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
            if (r != null) CreateRoomState(playerName, r, ans);
            return ans;
        }


        // GET: /api/Room?game_name=moshe&player_name=kaki&bet=100
        public RoomState GET(string gameName, string playerName, int bet) //palce bet
        {
            Room r = null;
            var ans = new RoomState();
            try
            {
                r = Server.GameFacade.PlaceBet(gameName, playerName, bet);
            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
            if(r!=null) CreateRoomState(playerName, r, ans);
            return ans;
        }


        // GET: /api/Room?game_name=moshe&player_name=kaki&option=call 
        public RoomState GET(string gameName, string playerName, string option) //call / fold 
        {
            Room r = null;
            var ans = new RoomState();
            try
            {
                switch (option)
                {
                    case "fold":
                        r = Server.GameFacade.Fold(gameName, playerName);
                        break;
                    case "call":
                        r = Server.GameFacade.Call(gameName, playerName);
                        break;
                }
                if (r != null) CreateRoomState(playerName, r, ans);
                return ans;
            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
            return ans;
        }

        // POST: api/Room   create room
        public RoomState Post([FromBody]Models.Room value)
        {
            if(Server.ChangeLeagues.AddDays(7)<= DateTime.Now)
            {
                Server.GameFacade.SetLeagues();
                Server.ChangeLeagues = DateTime.Now;
            }
            var ans = new RoomState();
            try
            {
                Room r = Server.GameFacade.CreateGameWithPreferences(value.RoomName, value.CreatorUserName, value.CreatorPlayerName, value.GameType, value.BuyInPolicy, value.ChipPolicy, value.MinBet, value.MinPlayers, value.MaxPlayers, value.SepctatingAllowed);
                if (r != null)
                {
                    var roomDic = new Dictionary<string, List<RoomState>>();
                    var userList = new List<RoomState>();
                    if (Replays.ContainsKey(r.Name))
                    {
                        Replays.Remove(r.Name);
                    }
                    roomDic.Add(value.CreatorPlayerName, userList);
                    Replays.Add(r.Name,roomDic);
                    CreateRoomState(value.CreatorPlayerName, r, ans);
                }
                return ans;
            }

            catch (Exception e)
            {
                ans.Messege = e.Message;
                return ans;
            }

        }

        public static void CreateRoomState(string player, Room r, RoomState ans)
        {
            try
            {
                var spectator = false;
                foreach (var u in r.SpectateUsers)
                {
                    if (u.Username == player) spectator = true;
                }

                ans.RoomName = r.Name;
                ans.IsOn = r.IsOn;
                ans.Pot = r.Pot;
                ans.GameStatus = r.GameStatus.ToString();
                ans.CommunityCards = new string[5];
                ans.AllPlayers = new Player[r.Players.Count];
                ans.CurrentPlayer = r.Players[r.CurrentTurn].Name;
                for (var i = 0; i < 5; i++)
                {
                    if (r.CommunityCards[i] == null) break;
                    ans.CommunityCards[i] = r.CommunityCards[i].ToString();
                }
                var j = 0;
                foreach (var p in r.Players)
                {
                    var p1 = new Player
                    {
                        PlayerName = p.Name,
                        CurrentBet = p.CurrentBet,
                        ChipsAmount = p.ChipsAmount,
                        Avatar = p.User.AvatarPath,
                        PlayerHand = new string[2]
                    };
                    if (player == p.Name&&r.IsOn)
                    {
                        if (p.Hand[0] != null) p1.PlayerHand[0] = p.Hand[0].ToString();
                        if (p.Hand[1] != null) p1.PlayerHand[1] = p.Hand[1].ToString();
                        foreach (var pa in p.User.Notifications)
                        {
                            if (pa.Item1 == r.Name)
                            {
                                p1.Messages.Add(pa.Item2);
                            }
                        }
                    }
                    else if(!r.IsOn)
                    {

                        if (p.Hand[0] != null&&!p.Folded) p1.PlayerHand[0] = p.Hand[0].ToString();
                        if (p.Hand[1] != null&&!p.Folded) p1.PlayerHand[1] = p.Hand[1].ToString();
                        if(player == p.Name)
                        {
                            foreach (var pa in p.User.Notifications)
                            {
                                if (pa.Item1 == r.Name)
                                {
                                    p1.Messages.Add(pa.Item2);
                                }
                            }
                        }
                    }
                    ans.AllPlayers[j] = p1;
                    j++;
                }   
                
                ans.Spectators = new UserData[r.SpectateUsers.Count];
                var u1 = new UserData();
                foreach(var u in r.SpectateUsers)
                {
                    u1.Username = u.Username;
                    if (spectator&& player==u.Username)
                    {     
                        foreach (var pa in u.Notifications)
                        {
                            if (pa.Item1 == r.Name)
                            {
                                u1.Messages.Add(pa.Item2);
                            }
                        }
                    }
                }

                if (Replays[r.Name][player].Count!=0&&!Replays[r.Name][player][Replays[r.Name][player].Count-1].Equals(ans))
                {
                    Replays[r.Name][player].Add(ans);
                }

                else if(Replays[r.Name][player].Count ==0) Replays[r.Name][player].Add(ans);

            }
            catch (Exception e)
            {
                ans.Messege = e.Message;
            }
        }


    }
}
