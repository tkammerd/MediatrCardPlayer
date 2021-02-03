using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace CardPlayer.Data.Models
{
    public class Player
    {
        public int Id { get; } = Math.Abs(DateTime.Now.GetHashCode()) + 
            (new Random()).Next(1000);
        public string Name { get; set; } = "";
        public Hand PlayerHand { get; set; } = new Hand();

        [JsonConstructor]
        public Player(int id, string name, Hand playerHand) : this()
        {
            Id = id;
            Name = name;
            PlayerHand = playerHand;
            /* TAK_Debug */ StackTrace DebugCallStack = new StackTrace(true);
            /* TAK_Debug */ Console.WriteLine($"JsonConstructor Constructing  Player" +
            /* TAK_Debug */     $"[Id: {Id}, Name: {Name}, Hand Size: {PlayerHand.Cards.Count}");
            /* TAK_Debug */ Console.WriteLine(DebugCallStack);
        }

        public Player() 
        {
            /* TAK_Debug */ StackTrace DebugCallStack = new StackTrace(true);
            /* TAK_Debug */ Console.WriteLine($"Default Constructing  Player" +
            /* TAK_Debug */     $"Id: {Id}, Name: {Name}, Handsize: {PlayerHand.Cards.Count}");
            /* TAK_Debug */ Console.WriteLine(DebugCallStack);
            /* TAK_Debug */ Console.WriteLine();
        }
    }
}

