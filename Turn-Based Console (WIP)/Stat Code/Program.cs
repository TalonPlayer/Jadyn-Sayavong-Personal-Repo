using System.Security.Cryptography.X509Certificates;

namespace Stat_Code
{
    internal class Program
    {
        public enum Turn
        {
            BeforeTurn,
            Party,
            Enemy,
        }
        static void Main(string[] args)
        {
            List<Character> party = new List<Character>();

            List<string> member = new List<string>
            {
                {"Slapstick"},
                {"Gojo Satoru"}
            };

            AssignClass assignMgr = new AssignClass(member);

            party = assignMgr.Party;

            Enemy enemy = new Enemy("Enemy");

            Turn turn = Turn.Party;
            int choice = 0;
            int index = 0;
            int maxIndex = 0;

            int turnNum = 1;
            do
            {
                switch (turn) 
                {
                    case Turn.BeforeTurn:
                        turnNum++;
                        foreach (Character m in party)
                        {
                            m.TurnStats();
                        }
                        turn = Turn.Party;
                        break;
                    case Turn.Party:
                        Console.WriteLine("\nSelect Party Member");
                        for (int i = 0; i < party.Count; i++)
                        {
                            Console.Write($"\n{i + 1}. {party[i].Name} - ({party[i].Action})");
                            index = i;
                            maxIndex = index + 2;
                        }
                        Console.WriteLine($"\n{maxIndex}. End Turn");
                        index = int.Parse(Console.ReadLine()) - 1;

                        if (index == maxIndex - 1)
                        {
                            turn = Turn.Enemy;
                            break;
                        }

                        if (!party[index].DidAction && !party[index].Disabled)
                        {
                            party[index].SelectAction(party, enemy);

                        }
                        else
                        {
                            Console.WriteLine(party[index].Name + " can't be selected.");
                        }
                        break;
                        

                    case Turn.Enemy:
                        Console.WriteLine("Enemy Turn");
                        foreach(Character m in party)
                        {
                            enemy.DoAction(0, party, m);
                        }
                        turn = Turn.BeforeTurn;
                        break;
                }
                
                
            } while (true);
        }
    }
}