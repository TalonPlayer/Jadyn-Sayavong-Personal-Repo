using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.Security.Principal;
using System.Reflection.Metadata.Ecma335;

namespace Turn_Based_Game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Random rng = new Random();

        private Texture2D square;
        private Texture2D transparentSquare;

        private Texture2D background;
        private Texture2D mindaReidy;

        private SpriteFont font;
        private SpriteFont bigFont;

        private Vector2 window;

        // Keyboard States
        private KeyboardState currentKey;
        private KeyboardState previousKey;

        private List<Window> windows = new List<Window>();

        // Temporary things for the party
        private Dictionary<Button, string> listOfActions = new Dictionary<Button, string>();
        private Button selected;

        private Dictionary<Button, Rectangle> attackList = new Dictionary<Button, Rectangle>();

        private List<Character> partyList = new List<Character>();

        private List<string> names;

        private Character selectedCharacter;

        private Character previousCharacter;

        private List<Color> buttonColors;

        private List<Color> playerColors;

        private List<Button> partyButtons = new List<Button>();

        private int index;

        // Temporary things for the enemy

        private Enemy enemy;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {            
            // TODO: Add your initialization logic here

            window = new Vector2(1500, 1000);
            _graphics.PreferredBackBufferWidth = (int) window.X;
            _graphics.PreferredBackBufferHeight = (int) window.Y;

            _graphics.ApplyChanges();

            // Initialize List Colors
            buttonColors = new List<Color>() { Color.White, Color.Red, Color.DarkRed };

            playerColors = new List<Color>() { Color.White, Color.White, Color.White };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Content
            square = Content.Load<Texture2D>("pixel");
            transparentSquare = Content.Load<Texture2D>("transparent gray");
            background = Content.Load<Texture2D>("Platform");
            mindaReidy = Content.Load<Texture2D>("Mrs. Reidy");

            font = Content.Load<SpriteFont>("Arial26");
            bigFont = Content.Load<SpriteFont>("Arial50");

            // List of names, these names are the file names
            names = 
                new List<string> ()
                {
                    //{ "Yuta Okkotsu"},
                    { "Gojo Satoru"},
                    //{ "Yu Narukami"},
                    { "Slapstick"},
                    //{ "Grimir Oversplitter"},
                    //{ "Osamu Dazai" },
                    //{ "Denji" },
                    //{ "Tohru Adachi" }
                };

            // Temporary enemy
            enemy = new Enemy(square, 
                new Rectangle(1200, 250, 275, 500),
                "Boss", 1, 100);

            enemy.LoadAssets(font, rng);

            // Load the characters,
            // This puts all the characters in partyList
            // It creates the buttons on the characters and assigns them to their unique classes
            LoadCharacters(names);

            // Load the window and buttons
            LoadWindows();
            LoadButtons();

            // The selected action is neither of them
            // 0 Attack
            // 1 Abilities
            // 2 Block
            // 3 Items
            // 4 Neither
            selected = listOfActions.ElementAt(4).Key;

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKey = Keyboard.GetState();

            // The options window will pause the game
            if (!windows[4].Active)
            {

                // Update everything
                enemy.Update(gameTime);

                foreach (Window window in windows)
                {
                    window.Update(gameTime);
                }

                windows[5].Open(partyList[index]);


                foreach (Button button in partyButtons)
                {
                    button.Update(gameTime);
                }

                foreach (Character character in partyList)
                {
                    character.Update(gameTime);

                    // If the character is clicked, make that character the
                    // selected one
                    if (character.Selected)
                    {
                        selectedCharacter = character;
                                                
                        // This is the index that gets the character's abilities
                        index = partyList.IndexOf(character);
                    }

                    if (previousCharacter != null)
                    {
                        // If the new selected character is different from the last selected
                        // character
                        if (previousCharacter != selectedCharacter)
                        {
                            selected = listOfActions.ElementAt(4).Key; // Action is neither
                            windows[0].Close();                        // Close the windows
                            previousCharacter.Close();                 // Close the window
                        }
                    }
                    
                    // The previous character is the selected one
                    previousCharacter = selectedCharacter;
                }
                
                // If there is a selected character, open their window
                if (selectedCharacter != null)
                {
                    OpenActionWindow(gameTime, index);
                }
            }

            // Only update the options window
            windows[4].Update(gameTime);
            previousCharacter = selectedCharacter;
            previousKey = currentKey;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            _spriteBatch.Begin();

            // Background
            _spriteBatch.Draw(background,
                background.Bounds,
                Color.White);

            // Top Bar
            _spriteBatch.Draw(square, 
                new Rectangle(0,0, (int) window.X, 125), 
                Color.Gray);

            // Draw Enemy
            enemy.Draw(_spriteBatch, Color.Red);

            // Draw every Character
            foreach (Character character in partyList)
            {
                character.Draw(_spriteBatch, Color.White);
            }


            // every window is drawn
            foreach (Window window in windows)
            {
                window.Draw(_spriteBatch);
            }
            // Draw the party stats
            if (windows[1].Active)
            {
                for(int i = 0; i < partyButtons.Count; i++)
                {
                    _spriteBatch.Draw(square, new Rectangle(
                        partyButtons[i].Position.X, 
                        partyButtons[i].Position.Y + (8 * i),
                        partyButtons[i].Position.Width, 
                        partyButtons[i].Position.Height),
                        Color.White);
                    _spriteBatch.DrawString(font,
                        partyList[i].Name + ": " + partyList[i].Power + 
                        "    " + partyList[i].Health + " / " + partyList[i].MaxHealth, 
                        new Vector2(partyButtons[i].Position.X, partyButtons[i].Position.Y + (8 * i)), Color.Black);
                }
            }
            
            // Draw each action for the selected character
            if (windows[0].Active)
            {
                foreach (var button in listOfActions)
                {
                    // If the button is selected, draw that button red
                    if (button.Key == selected)
                    {
                        button.Key.Selected(); // This method draws the button red in the class
                        _spriteBatch.DrawString(
                            bigFont, selectedCharacter.Name + button.Value,
                            new Vector2(40, 160), Color.White);
                    }
                    button.Key.Draw(_spriteBatch);
                }

                // Draw the abilities buttons
                if (selected != listOfActions.ElementAt(4).Key)
                {
                    foreach (var attackButtons in attackList)
                    {
                        attackButtons.Key.Draw(_spriteBatch);
                        _spriteBatch.Draw(
                                transparentSquare, attackButtons.Value,
                                Color.White);
                    }
                }
            }

            windows[3].Draw(_spriteBatch);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public bool SinglePressKey(Keys key, KeyboardState currentState)
        {
            return currentState.IsKeyDown(key) && previousKey.IsKeyUp(key);
        }

        public void LoadWindows()
        {
            // The rectangle of the windows
            Rectangle actions = new Rectangle(25, 150, 575, 800);

            Rectangle info = new Rectangle(625, 575, 675, 250);

            Rectangle all = new Rectangle(1325, 575, 150, 250);

            Rectangle options = new Rectangle(375, 50, 750, 950);

            // Rectangle for the buttons to open and close
            
            Rectangle partyButton = new Rectangle(625, 850, 325, 100);  // Open
            Rectangle partyClose = new Rectangle(575, 925, 25, 25);     // Close

            Rectangle infoButton = new Rectangle(975, 850, 325, 100);
            Rectangle infoClose = new Rectangle(1275, 800, 25, 25);

            Rectangle allButton = new Rectangle(1325, 850, 150, 100);
            Rectangle allClose = new Rectangle(1450, 800, 25, 25);

            Rectangle optionButton = new Rectangle(20, 25, 255, 75);
            Rectangle optionClose = new Rectangle(375, 50, 25, 25);

            // Create the windows
            Window actionsWindow = new Window(transparentSquare, actions, Color.White,
                square, buttonColors,
                "BUTTON",
                new Rectangle(-100,-100, 0, 0),
                // CLOSE
                new Rectangle(575, 150, 25, 25), font);

            Window infoWindow = new Window(transparentSquare, info, Color.White,
                square, buttonColors,
                "ENEMY ANALYSIS",
                infoButton,
                // CLOSE
                infoClose, font);

            Window allWindow = new Window(transparentSquare, all, Color.White,
                square, buttonColors,
                "ALL",
                allButton,
                // CLOSE
                allClose, font);

            Window optionsWindow = new Window(transparentSquare, options, Color.White,
                square, buttonColors,
                "OPTIONS",
                optionButton,
                // CLOSE
                optionClose, font);

            Window partyWindow = new Window(transparentSquare, actions, Color.White,
                square, buttonColors,
                "PARTY STATS",
                partyButton,
                // CLOSE
                partyClose, font);

            Window selectPartyWindow = new Window(transparentSquare, actions, Color.White,
                square, buttonColors,
                "BUTTON",
                new Rectangle(-100, -100, 0, 0),
                // CLOSE
                partyClose, font);


            // Add windows to the list
            windows.Add(actionsWindow);

            windows.Add(partyWindow);

            windows.Add(infoWindow);

            windows.Add(allWindow);

            windows.Add(optionsWindow);

            windows.Add(selectPartyWindow);

        }

        public void LoadButtons()
        {
            // Create buttons for the list of actions
            Button physical = new Button(square,
                new Rectangle(50, 700, 250, 100), buttonColors,
                font, "ATTACK", Color.Black);

            Button abilities = new Button(square,
                new Rectangle(325, 700, 250, 100), buttonColors,
                font, "ABILTIIES", Color.Black);

            Button block = new Button(square,
                new Rectangle(50, 825, 250, 100), buttonColors,
                font, "BLOCK", Color.Black);

            Button items = new Button(square,
                new Rectangle(325, 825, 250, 100), buttonColors,
                font, "ITEMS", Color.Black);

            Button neither = new Button(square,
                new Rectangle(0, 0, 0, 0), buttonColors,
                font, " ", Color.Black);

            // Add Buttons and String to list of actions,
            // The value adds an extra text to the top: Slapstick - Abilities
            listOfActions.Add(physical, " - Attacks");
            listOfActions.Add(abilities, " - Abilities");
            listOfActions.Add(block, " - Block");
            listOfActions.Add(items, " - Items");
            listOfActions.Add(neither, " ");
        }

        public void LoadCharacters(List<string> list)
        {
            // For every name in the list, create a character, only up to 8
            for (int i = 1; i <= list.Count; i++)
            {
                
                Texture2D asset;
                Vector2 offset = Vector2.Zero;
                string name = list[i - 1];

                // Load the character's asset
                asset = Content.Load<Texture2D>("Units/" + name);


                // This is so that all of the characters are not ontop of eachother
                if (i % 2 == 0)
                {
                    offset.Y = 35 + (i * 5);
                }
                else
                {

                    offset.Y = 0;
                }

                offset.X -= 20 * i;


                // Create characters and assign them to their appropriate class
                AssignClass(name, asset, offset, i);

                partyList[i - 1].LoadAssets(asset, bigFont, square, playerColors, buttonColors,
                    new Rectangle(50, 175 + (85 * (i - 1)), 525, 85));

                partyButtons.Add(partyList[i - 1].Stats);
            }
        }

        public void AssignClass(string name, Texture2D asset, Vector2 offset, int index)
        {
            Character character;
            switch (name)
            {
                case "Slapstick":
                    character = new Slapstick(asset,
                        new Rectangle(
                                600 + (int)offset.X + (150 * ((index - 1) % 2)),
                                225 + (int)offset.Y + (100 * ((index - 1) / 2)),
                                asset.Width, asset.Height),
                            name, 1);
                    partyList.Add(character);
                    break;
                case "Gojo Satoru":
                    character = new Gojo_Satoru(asset,
                        new Rectangle(
                                600 + (int)offset.X + (150 * ((index - 1) % 2)),
                                225 + (int)offset.Y + (100 * ((index - 1) / 2)),
                                asset.Width, asset.Height),
                            name, 1);
                    partyList.Add(character);
                    break;
                default:
                    break;
            }
        }

        public void OpenActionWindow(GameTime gameTime, int index)
        {
            windows[0].Open(partyList[index]);

            if (windows[0].Active)
            {
                foreach (var button in listOfActions)
                {
                    button.Key.Update(gameTime);
                    if (button.Key.Click)
                    {
                        selected = button.Key;

                        if (selected == listOfActions.ElementAt(1).Key)
                        {
                            attackList = partyList[index].AbilityList;
                        }
                        else if (selected == listOfActions.ElementAt(0).Key)
                        {
                            partyList[index].Action(selected, partyList[index], enemy, index);
                            selected = listOfActions.ElementAt(4).Key;
                            windows[0].Close();
                            partyList[index].Close();
                        }
                        else
                        {
                            selected = listOfActions.ElementAt(4).Key;
                            windows[0].Close();
                            partyList[index].Close();
                        }
                        button.Key.Reset();
                    }

                    foreach (var attackButtons in attackList)
                    {
                        attackButtons.Key.Update(gameTime);

                        if (attackButtons.Key.Click)
                        {
                            partyList[index].Action(attackButtons.Key, partyList[index], enemy, index);

                            if (partyList[index].OpenWindow)
                            {
                                partyList[index].Close();
                                windows[0].Close();
                            }
                            else
                            {
                                attackButtons.Key.Reset();
                                partyList[index].Close();
                                windows[0].Close();
                            }
                        }
                    }
                }
                selected.Update(gameTime);

            }
        }
    }
}