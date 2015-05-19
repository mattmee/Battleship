using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*BATTLESHIP: Play battleship against yourself in a 10 x 10 grid.  One playing field is set along with random ship 
 * placement. Input is of "x,y" */
namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid Matt = new Grid();
            Matt.PlayGame();

        }
    }
    /// <summary>
    /// Basic class for a point, includes X and Y properties along with an enum PointStatus
    /// </summary>
    class Point
    {
        public enum PointStatus
        { Empty, Ship, Hit, Miss }

        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }

        public Point(int x, int y, PointStatus p)
        {
            this.X = x;
            this.Y = y;
            this.Status = p;
        }

    }
    /// <summary>
    /// Class for a ship. A ship has OccupiedPoints, a Length, a ShipType, and a bool value of IsDestroyed
    /// </summary>
    class Ship
    {
        public enum ShipType
        { Carrier, Battlesship, Cruiser, Submarine, Minesweeper }

        public ShipType Type { get; set; }
        public List<Point> OccupiedPoints { get; set; }
        public int Length { get; set; }
        public bool IsDestroyed
        {
            get
            { return OccupiedPoints.All(x => x.Status == Point.PointStatus.Hit); }  //returns true if all OccupiedPoints have PointStatus Hit
        }

        public Ship(ShipType typeOfShip)             //Constructor that takes in ShipType, which determines the Length
        {
            this.OccupiedPoints = new List<Point>();  //initialize OccupiedPoints
            this.Type = typeOfShip;                   //initialize Type
            switch (typeOfShip)                      //sets length based on typeOfShip
            {
                case ShipType.Carrier:
                    this.Length = 5;
                    break;
                case ShipType.Battlesship:
                    this.Length = 4;
                    break;
                case ShipType.Cruiser:
                case ShipType.Submarine:
                    this.Length = 3;
                    break;
                case ShipType.Minesweeper:
                    this.Length = 2;
                    break;
            }
        }
    }
    /// <summary>
    /// Grid class. Sets the grid, or map.  
    /// </summary>
    class Grid
    {
        public enum PlaceShipDirecton
        { Horizontal, Vertical }
        public Point[,] Ocean { get; set; }
        public List<Point> OceanOccupiedPoints { get; set; }
        public List<Ship> ListOfShips { get; set; }
        public bool AllShipsDestroyed
        {
            get
            {
                return ListOfShips.All(x => x.IsDestroyed == true);
            }
        }
        public int CombatRound { get; set; }
        //Grid constructor. Sets a grid in a multidimensional array of points, 10 by 10 with PointStatus of Empty
        public Grid()                 
        {
            this.OceanOccupiedPoints = new List<Point>();
            this.Ocean = new Point[10, 10];
            for (int i = 0; i < Ocean.GetLength(0); i++)
            {
                for (int j = 0; j < Ocean.GetLength(1); j++)
                {
                    Ocean[i, j] = new Point(i, j, Point.PointStatus.Empty);
                }
            }
            this.ListOfShips = new List<Ship>();
            this.ListOfShips.Add(new Ship(Ship.ShipType.Carrier));
            this.ListOfShips.Add(new Ship(Ship.ShipType.Battlesship));
            this.ListOfShips.Add(new Ship(Ship.ShipType.Cruiser));
            this.ListOfShips.Add(new Ship(Ship.ShipType.Minesweeper));
            this.ListOfShips.Add(new Ship(Ship.ShipType.Submarine));
            PlaceShipsRandomly();
        }
        //Places a ship within the Grid
        public void PlaceShip(Ship shipToPlace, PlaceShipDirecton direction, int startX, int startY)   //places the ship within the ocean
        {
            for (int i = 0; i < shipToPlace.Length; i++)
            {
                switch (direction)
                {
                    case PlaceShipDirecton.Horizontal:
                        Ocean[startX + i, startY].Status = Point.PointStatus.Ship;
                        shipToPlace.OccupiedPoints.Add(this.Ocean[startX + i, startY]);
                        this.OceanOccupiedPoints.Add(this.Ocean[startX + i, startY]);
                        break;
                    case PlaceShipDirecton.Vertical:
                        Ocean[startX, startY + i].Status = Point.PointStatus.Ship;
                        shipToPlace.OccupiedPoints.Add(this.Ocean[startX, startY + i]);
                        this.OceanOccupiedPoints.Add(this.Ocean[startX, startY + i]);
                        break;
                }
            }
        }
        //Displasys the ocean to the console
        public void DisplayOcean()   //displays the ocean to the console
        {
            Console.WriteLine("    0  1  2  3  4  5  6  7  8  9  -----  X");
            for (int i = 0; i < Ocean.GetLength(0); i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("{0}--", i);
                for (int j = 0; j < Ocean.GetLength(1); j++)
                {
                    switch (this.Ocean[j, i].Status)
                    {
                        case Point.PointStatus.Ship:
                        //Console.Write("[S]");                 //for debugging the random placement
                        //break;
                        case Point.PointStatus.Empty:
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write("[ ]");
                            break;
                        case Point.PointStatus.Hit:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("[X]");
                            break;
                        case Point.PointStatus.Miss:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("[o]");
                            break;
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Handles the logic for determining hits or misses, return true if a ship has been destroyed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Target(int x, int y)
        {
            int numberOfShipsDestroyed = ListOfShips.Where(X => X.IsDestroyed == true).Count();
            Point P = this.Ocean[x, y];
            if (P.Status == Point.PointStatus.Ship)
                P.Status = Point.PointStatus.Hit;
            else if (P.Status == Point.PointStatus.Empty)
                P.Status = Point.PointStatus.Miss;
            if (numberOfShipsDestroyed < ListOfShips.Where(X => X.IsDestroyed == true).Count())
                return true;
            else
                return false;
        }

        public void PlaceShipsRandomly()   //randomly places all ships
        {
            Random randNum = new Random();
            int randNumSelectedGained = 0;
            int randNumSelected = 0;
            int randNumForDirection = 0;
            for (int i = 0; i < 5; i++) //places 5 ships, 1 at a time
            {
                while (true)
                {
                    randNumSelectedGained = randNum.Next(0, 10 - this.ListOfShips[i].Length);
                    randNumSelected = randNum.Next(0, 10);
                    randNumForDirection = randNum.Next(0, 2);
                    int startOver = 0;

                    for (int j = 0; j < this.ListOfShips[i].Length; j++)
                    {
                        switch (randNumForDirection)
                        {
                            case 0:
                                if (Ocean[randNumSelectedGained + j, randNumSelected].Status != Point.PointStatus.Empty)
                                    startOver++;
                                break;
                            case 1:
                                if (Ocean[randNumSelected, randNumSelectedGained + j].Status != Point.PointStatus.Empty)
                                    startOver++;
                                break;
                        }
                    }
                    if (startOver == 0)
                    {
                        switch (randNumForDirection)
                        {
                            case 0:
                                this.PlaceShip(ListOfShips[i], PlaceShipDirecton.Horizontal, randNumSelectedGained, randNumSelected);
                                break;
                            case 1:
                                this.PlaceShip(ListOfShips[i], PlaceShipDirecton.Vertical, randNumSelected, randNumSelectedGained);
                                break;
                        }
                        break;
                    }
                }
            }
        }

        public void PlayGame()    //plays the game 
        {
            string userInput = string.Empty;
            string[] splitString = new string[2];
            string x = string.Empty;
            string y = string.Empty;
            

            while (!this.AllShipsDestroyed)
            {
                this.DisplayOcean();
                Console.WriteLine("Combat Round: " + this.CombatRound);
                System.Threading.Thread.Sleep(500);
                this.CombatRound++;
                Console.WriteLine("Pick an x,y coordinate");
                while (true)            //infinite loop until userInput is 'valid'
                {
                    userInput = Console.ReadLine();
                    if (userInput.Length == 3 && "0123456789".Contains(userInput[0]) && userInput[1] == ',' && "0123456789".Contains(userInput[2]))//if user input is '1' '2' or '3' the infinite loop is broken
                    {
                        break;
                    }
                    Console.WriteLine("Pick an x,y coordinate.... example: 2,4");
                }
                splitString = userInput.Split(',');
                x = splitString[0];
                y = splitString[1];
                Target(int.Parse(x), int.Parse(y));
                Console.Clear();
            }
        }
    }

}
