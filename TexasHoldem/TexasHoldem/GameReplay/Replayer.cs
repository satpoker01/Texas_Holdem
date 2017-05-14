﻿using System;
using System.Collections.Generic;
using System.IO;
using TexasHoldem.Game;
using TexasHoldem.Loggers;
using TexasHoldem.Properties;

namespace TexasHoldem.GameReplay
{
    public class Replayer
    {
        private static readonly string CounterPath = Directory.GetCurrentDirectory() + "\\gameReplayCounter.txt";

        private Replayer() { }

        public static string CreateReplay()
        {
            var counter = 1;
            if (File.Exists(CounterPath))
            {
                counter = int.Parse(File.ReadAllText(CounterPath));
            }

            var filename = "gameReplay#" + counter + ".csv";
            File.AppendAllText(filename, Resources.Replayer_CreateReplay_turn_no___seat0__seat1__seat2__seat3__seat4__seat5__seat6__seat7__seat8__pot__community___ + Environment.NewLine);
            File.WriteAllText(CounterPath, "" + ++counter);

            return filename;
        }

        public static void Save(string filename, int round, List<Player> players, int pot, Card[] community, string comment)
        {
            var path = Directory.GetCurrentDirectory() + "\\" + filename;
            if (!File.Exists(path))
            {
                var e = new Exception("gameReplay file does not exists");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            if (round < 1)
            {
                var e = new Exception("round no. is invalid");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            if (players == null)
            {
                var e = new Exception("player list is invalid");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            if (players.Count == 0)
            {
                var e = new Exception("player list is empty");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            if (pot < 0)
            {
                var e = new Exception("pot value is invalid");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }

            var entry = round + ",";
            foreach (var p in players)
            {
                if (!p.Folded)
                {
                    entry += p.CurrentBet + ",";
                }
                else
                {
                    entry += "fold,";
                }
            }
            for (var i = 0; i <= 8-players.Count; i++)
            {
                entry += "undef,";
            }
            entry += pot + ",";
            if (community != null)
            {
                for (var i = 0; i < community.Length; i++)
                {
                    if (community[i] != null)
                    {
                        entry += community[i].Value + community[i].Type.ToString().Substring(0, 1) + ";";
                    }
                }
            }
            else
            {
                entry += "-";
            }
            entry += ",,," + comment;

            File.AppendAllText(path, entry + Environment.NewLine);
        }

        public static string SaveTurn(string replay, int turnNum)
        {
            var path = Directory.GetCurrentDirectory() + "\\" + replay;
            var turn = "";
            if (File.Exists(path))
            {
                var file = new StreamReader(path);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (Char.GetNumericValue(line[0]) == turnNum)
                    { 
                        turn += line + "\n";
                    }
                }

                file.Close();
            }
            else
            {
                var e = new Exception("game replay not found");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            if (turn == "")
            {
                var e = new Exception("turn not found in game replay");
                Logger.Log(Severity.Exception, e.Message);
                throw e;
            }
            return turn;
        }
    }
}
