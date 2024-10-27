using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;

namespace Stat_Code
{
    internal class Program
    {
        // Game State
        public enum Turn
        {
            BeforeTurn,
            Party,
            Enemy,
        }
        static void Main(string[] args)
        {
            // Create the list to hold characters in the party
            List<Character> party = new List<Character>();

            // The Assign Class (Manager) takes in the character's names
            AssignClass assignMgr = new AssignClass(new List<string>
            {
                {"Slapstick"},
                {"Gojo Satoru"}
            });

            // Each character's name will determine what Character Child Class that character gets
            // Party will take the Assign Manager's characters that have their assigned classes
            party = assignMgr.Party;

            // Temporary Default Enemy
            Enemy enemy = new Enemy("Enemy");

            // Game State is first set to Party, because BeforeTurn happens after the enemies turn
            // and before the party's turn. It also should not start happen when the game first starts
            Turn turn = Turn.Party;
            int choice = 0;
            int index = 0;
            int maxIndex = 0;
            int turnNum = 1;
            bool endTurn = false;
            do
            {
                switch (turn) 
                {
                    // Happens before party's turn, includes things like
                    // Automatic Regeneration and Bleeding.
                    // Also subtracts the turn counters so the states can end
                    case Turn.BeforeTurn:
                        turnNum++;
                        foreach (Character m in party)
                        {
                            m.TurnStats();
                        }
                        turn = Turn.Party;
                        Console.ReadLine();
                        break;
                    
                    // Party's Turn
                    case Turn.Party:
                        foreach (Character m in party)
                        {
                            endTurn = true;
                            if (!m.DidAction)
                            {
                                endTurn = false;
                                break;
                            }
                        }
                        if (endTurn)
                        {
                            turn = Turn.Enemy;
                            break;
                        }
                        Console.WriteLine("\nSelect Party Member");
                        // For each character, print their fake index, name, and the action (default is Attack)
                        // Fake index means that when asking for an input, the index will start and print at 1 instead of 0.
                        // But to get the correct index when that input is selected, it will start at 0
                        for (int i = 0; i < party.Count; i++)
                        {
                            Console.Write($"\n{i + 1}. {party[i].Name} - ({party[i].TurnHandler.Action})");
                            index = i;
                            maxIndex = index + 2;
                        }
                        Console.WriteLine($"\n{maxIndex}. End Turn");
                        Console.WriteLine($"{maxIndex + 1}. All Does Action");

                        index = int.Parse(Console.ReadLine()) - 1;

                        // If the chosen index is the max Index, end the party's turns.
                        // Ex. ReadLine index = 3 - 1 = 2, maxIndex = 1 + 2 = 3
                        // Index == maxIndex - 1 | 2 == (3 - 1)
                        if (index == maxIndex - 1)
                        {
                            turn = Turn.Enemy;
                            break;
                        }
                        // If the chosen index is the max Index, do all party's moves at once.
                        // Ex. ReadLine index = 4 - 1 = 3, maxIndex = 1 + 2 = 3
                        // Index == maxIndex | 3 = 3
                        else if (index == maxIndex)
                        {
                            foreach(Character m in party)
                            {
                                if (!m.DidAction && !m.Disabled && m.IsAlive)
                                {
                                    m.TurnHandler.DidAction = true;
                                    m.TurnHandler.Action = "Turn is Over";
                                    m.DoAction(m.TurnHandler.AbilityIndex, party, enemy);
                                }
                            }
                        }
                        else
                        {
                            party[index].TurnHandler.SelectAction(party, enemy);
                        }
                        break;
                        

                    case Turn.Enemy:
                        Console.WriteLine("Enemy Turn");
                        Console.ReadLine();
                        foreach(Character member in party)
                        {
                            enemy.DoAction(0, party, member);
                        }
                        Console.WriteLine(enemy.ToString());
                        turn = Turn.BeforeTurn;
                        Console.ReadLine();
                        break;
                }
                
                
            } while (true);
        }
    }
}