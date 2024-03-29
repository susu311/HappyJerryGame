using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace HappyJerryGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private enum MainScreen
        {
            Welcome1,
            Welcome2,
            Play,
            Touch,
            Gameover
        }
        MainScreen mMainScreen = MainScreen.Welcome1;
        Texture2D Welcome1;
        Texture2D Welcome2;

        
        int mChooseItem = 0;
        Texture2D Item1;
        
        BoundingSphere playsphere;
        BoundingSphere objectsphere;
        bool touchItem;
        bool chooseAns;
        bool ansCorrect;
        int listoption;
        int correctAns;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D sprite; //this will hold image
        int framecounter = 0; //this frame will hold x coordinates while animating        
        Vector2 pos = Vector2.Zero;// set zero for the position of sprite
        Vector2 test = Vector2.One;
        Vector2 speed = Vector2.Zero;//for moving the sprite
        SpriteEffects se = SpriteEffects.FlipHorizontally; //to turn the sprite horizontally
        double framedelay = 0.07;
        Level level;
        SpriteFont spriteFont;
        Texture2D background;

        //blood
        int score = 0;        //total 22
        Texture2D blood;
        Texture2D bloodBase;

        //monkey
        Texture2D Blank;
        Texture2D Congratulation;
        Texture2D monkey;
        int monkeyX = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(
                        graphics_Settings_NoDepthBuffer);

            Content.RootDirectory = "Content";
            //this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler(Window_ClientSizeChanged);

            this.Exiting += new EventHandler(Game1_Exiting);

        }
        void graphics_Settings_NoDepthBuffer(object sender, PreparingDeviceSettingsEventArgs e)//no depth buffer
        {
            e.GraphicsDeviceInformation.PresentationParameters.EnableAutoDepthStencil = false;
        }


        protected override void Initialize()
        {
            //create a serializer and a streamreader to read the file
            XmlSerializer xs= new XmlSerializer(typeof(Level));
            TextReader tr= new StreamReader("Content/example.xml");

           //write values from the XML file into the object
            level = (Level)xs.Deserialize(tr);
            tr.Close();
            pos = new Vector2(50f, 50f);

            score = 0;
            touchItem = false;
            base.Initialize();    
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            background = Content.Load<Texture2D>("FinalHouse");
            sprite = Content.Load<Texture2D>("4Sprites+in1");
            spriteFont = Content.Load<SpriteFont>("Arial");

            blood = Content.Load<Texture2D>("blood");
            bloodBase = Content.Load<Texture2D>("SimpleBody");

            Blank = Content.Load<Texture2D>("blank");
            Congratulation = Content.Load<Texture2D>("Congratulation");
            monkey = Content.Load<Texture2D>("monkey");

            Welcome1 = Content.Load<Texture2D>("welcomePage");
            Welcome2 = Content.Load<Texture2D>("welcomePage2");
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = new KeyboardState();
            KeyboardState oldks = new KeyboardState();
            ks = Keyboard.GetState();

            if (mMainScreen == MainScreen.Welcome1)
            {
                if (ks.IsKeyDown(Keys.X))
                    mMainScreen = MainScreen.Play;
                if (ks.IsKeyDown(Keys.H))
                    mMainScreen = MainScreen.Welcome2;
            }
            else if (mMainScreen == MainScreen.Welcome2)
            {
                if (ks.IsKeyDown(Keys.X))
                    mMainScreen = MainScreen.Play;
                if (ks.IsKeyDown(Keys.B))
                    mMainScreen = MainScreen.Welcome1;
            }
            else if (mMainScreen == MainScreen.Play)
            {
                speed = Vector2.Zero;
                bool moving = false;
                

                if (ks.IsKeyDown(Keys.Right))
                {
                    speed.X = 2;
                    moving = true;
                    se = SpriteEffects.FlipHorizontally;
                }

                if (ks.IsKeyDown(Keys.Left))
                {
                    speed.X = -2;
                    moving = true;
                    se = SpriteEffects.None;
                }
                if (ks.IsKeyDown(Keys.Up))
                {
                    speed.Y = -2;
                    moving = true;
                    se = SpriteEffects.None;
                }
                if (ks.IsKeyDown(Keys.Down))
                {
                    speed.Y = 2;
                    moving = true;
                    se = SpriteEffects.None;
                }
                
                
                Collision();    //check if the sprite collision the wall
                


                //add speed value to our position to move the sprite
                pos += speed;


                if (moving)
                {
                    touchItem = false;
                    framedelay -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (framedelay < 0)
                    {
                        framedelay = 0.07;

                        //sprite
                        framecounter += (sprite.Height - 4);
                        if (framecounter >= sprite.Width)
                            framecounter = 0;                        
                    }

                }
                
                Rectangle people = new Rectangle((int)pos.X, (int)pos.Y, 30, 30);
                
                //list1
                Rectangle item1 = new Rectangle(250, 0, 30, 20);        //piano
                Rectangle item2 = new Rectangle(0, 100, 10, 20);        //TV
                Rectangle item3 = new Rectangle(130, 150, 30, 10);      //sofa

                //list2
                Rectangle item4 = new Rectangle(80, 0, 100, 10);        //bookshelf
                Rectangle item5 = new Rectangle(340, 110, 10, 5);      //telephone
                Rectangle item6 = new Rectangle(0, 0, 20, 20);          //computer

                //list3
                Rectangle item7 = new Rectangle(300, 70, 10, 10);        //drawing
                
                //list4
                Rectangle item8 = new Rectangle(150, 430, 10, 20);      //rice
                Rectangle item9 = new Rectangle(130, 430, 10, 20);      //pasta
                Rectangle item10 = new Rectangle(110, 430, 10, 20);     //potato

                //list5
                Rectangle item11 = new Rectangle(90, 430, 20, 20);      //vegetable
                Rectangle item12 = new Rectangle(200, 430, 20, 20);     //meat
                Rectangle item13 = new Rectangle(120, 430, 20, 20);     //fruit

                //list6
                Rectangle item14 = new Rectangle(170, 430, 30, 20);     //cooker
                Rectangle item15 = new Rectangle(10, 350, 20, 20);      //fridge

                //list7
                Rectangle item16 = new Rectangle(490, 220, 50, 100);    //bed
                Rectangle item17 = new Rectangle(330, 250, 50, 30);     //clothes shelf
                Rectangle item18 = new Rectangle(570, 10, 20, 20);      //bath
              
                //list8
                Rectangle item19 = new Rectangle(470, 10, 20, 20);      //shower
                Rectangle item20 = new Rectangle(220, 260, 20, 20);     //handwash
                Rectangle item21 = new Rectangle(660, 120, 10, 10);     //toilet

                if ((people.Intersects(item1)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 1;
                    correctAns = 1;
                }
                else if ((people.Intersects(item2)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 2;
                    correctAns = 0;
                }
                else if ((people.Intersects(item3)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 3;
                    correctAns = 2;
                }
                else if ((people.Intersects(item4)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 4;
                    correctAns = 0;
                }
                else if ((people.Intersects(item5)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 5;
                    correctAns = 1;
                }
                else if ((people.Intersects(item6)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 6;
                    correctAns = 2;
                }
                else if ((people.Intersects(item7)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 7;
                    correctAns = 0;
                }
                else if ((people.Intersects(item8)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 8;
                    correctAns = 0;
                }
                else if ((people.Intersects(item9)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 9;
                    correctAns = 1;
                }
                else if ((people.Intersects(item10)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 10;
                    correctAns = 2;
                }
                else if ((people.Intersects(item11)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 11;
                    correctAns = 0;
                }
                else if ((people.Intersects(item12)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 12;
                    correctAns = 1;
                }
                else if ((people.Intersects(item13)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 13;
                    correctAns = 2;
                }
                else if ((people.Intersects(item14)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 14;
                    correctAns = 0;
                }
                else if ((people.Intersects(item15)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 15;
                    correctAns = 2;
                }
                else if ((people.Intersects(item16)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 16;
                    correctAns = 0;
                }
                else if ((people.Intersects(item17)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 17;
                    correctAns = 1;
                }
                else if ((people.Intersects(item18)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 18;
                    correctAns = 2;
                }
                else if ((people.Intersects(item19)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 19;
                    correctAns = 0;
                }
                else if ((people.Intersects(item20)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 20;
                    correctAns = 1;
                }
                else if ((people.Intersects(item21)) && (!touchItem))
                {
                    mMainScreen = MainScreen.Touch;
                    listoption = 21;
                    correctAns = 2;
                }
            }
            if (mMainScreen == MainScreen.Touch) 
            {
                chooseAns = false;
                ansCorrect = false;
                touchItem = true;   //touch the item
                if (ks.IsKeyDown(Keys.A))
                    mChooseItem = 0;
                else if (ks.IsKeyDown(Keys.B))
                    mChooseItem = 1;
                else if (ks.IsKeyDown(Keys.C))
                    mChooseItem = 2;

                if (ks.IsKeyDown(Keys.Enter))
                {
                    if (checkAns(listoption, mChooseItem, correctAns))
                        score++;
                    mMainScreen = MainScreen.Play;
                    mChooseItem = 0;
                    pos -= speed;
                }
                
            }

            if (mMainScreen == MainScreen.Gameover)
            {
                //monkey
                if (gameTime.TotalGameTime.Milliseconds % 150 == 0)
                {
                    monkeyX = monkeyX + monkey.Width / 3;
                    if (monkeyX >= monkey.Width + monkey.Width / 3)
                        monkeyX = monkey.Width / 3;
                }
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);
            spriteBatch.Begin();

            if (mMainScreen == MainScreen.Welcome1)
            {
                spriteBatch.Draw(Welcome1, new Rectangle(0, 0, 800, 600), Color.White);
            }
            else if (mMainScreen == MainScreen.Welcome2)
            {
                spriteBatch.Draw(Welcome2, new Rectangle(0, 0, 800, 600), Color.White);
            }
            else if (mMainScreen == MainScreen.Play)
            {
                int x = (int)pos.X;
                int y = (int)pos.Y;

                //count the screen size
                if (x < 60)
                    x = 60;
                if (x > 200)
                    x = 200;
                if (y < 30)
                    y = 30;
                if (y > 250)
                    y = 250;

                spriteBatch.Draw(background, Vector2.Zero, new Rectangle(x, y, 800 + x, 1280 + y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

                spriteBatch.Draw(sprite, pos, new Rectangle(framecounter, 0, sprite.Height - 4, sprite.Height), Color.White, 0, Vector2.Zero, 1, se, 0);

                //score = 21;
                fillinBlood(score);
                
                //GAME OVER => monkey jump out
                if (score == 22)
                    mMainScreen = MainScreen.Gameover;
                
            }
            else if (mMainScreen == MainScreen.Touch)
            {
                if ((listoption == 1) || (listoption == 2) || (listoption == 3))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Television", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Piano", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Sofa", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Television", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Piano", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Sofa", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Television", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Piano", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Sofa", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 4) || (listoption == 5) || (listoption == 6))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Anaquel de libro", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Anaquel de libro", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Anaquel de libro", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if (listoption == 7)
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Empate y pintura", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Empate y pintura", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Empate y pintura", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Telefono", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Computadora", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 8) || (listoption == 9) || (listoption == 10))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Arroz", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Pasta", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Patata", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Arroz", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Pasta", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Patata", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Arroz", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Pasta", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Patata", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 11) || (listoption == 12) || (listoption == 13))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Verdura", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Fruta", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Verdura", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Fruta", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Verdura", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Fruta", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 14) || (listoption == 15))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Cocina", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Nevera", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Cocina", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Nevera", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Cocina", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Carne", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Nevera", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 16) || (listoption == 17) || (listoption == 18))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Cama", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Anaquel de ropa", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Banera", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Cama", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Anaquel de ropa", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Banera", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Cama", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Anaquel de ropa", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Banera", new Vector2(10, 50), Color.Yellow);
                    }
                }
                else if ((listoption == 19) || (listoption == 20) || (listoption == 21))
                {
                    if (mChooseItem == 0)
                    {
                        spriteBatch.DrawString(spriteFont, "Ducha", new Vector2(10, 10), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Mano se lava", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Retrete", new Vector2(10, 50), Color.White);
                    }
                    else if (mChooseItem == 1)
                    {
                        spriteBatch.DrawString(spriteFont, "Ducha", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Mano se lava", new Vector2(10, 30), Color.Yellow);
                        spriteBatch.DrawString(spriteFont, "Retrete", new Vector2(10, 50), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(spriteFont, "Ducha", new Vector2(10, 10), Color.White);
                        spriteBatch.DrawString(spriteFont, "Mano se lava", new Vector2(10, 30), Color.White);
                        spriteBatch.DrawString(spriteFont, "Retrete", new Vector2(10, 50), Color.Yellow);
                    }
                }

            }
            else if (mMainScreen == MainScreen.Gameover)  //GAME OVER 
            {
                spriteBatch.Draw(Blank, new Rectangle(0, 0, 800, 600), Color.White);
                spriteBatch.Draw(Congratulation, new Rectangle(0, 0, 800, 600), Color.White);
                spriteBatch.Draw(monkey, new Vector2(100f, 230f), new Rectangle(monkeyX, 0, monkey.Width / 3, monkey.Height), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }


        void Game1_Exiting(object sender, EventArgs e)
        {
            // Add any code that must execute before the game ends.
        }


        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Make changes to handle the new window size.            
        }


        private void Collision()
        {
            if (pos.X < 0)
                pos.X = 0;
            if (pos.Y < 0)
                pos.Y = 0;
            if (pos.X > 650)
                pos.X = 650;
            if (pos.Y > 420)
                pos.Y = 420;

            //wall between living room and hallway
            if ((pos.Y > 125) && (pos.Y < 155) && (pos.X > 40) && (pos.X < 300))
            {
                if (pos.Y < 140)
                    pos.Y = 125;
                else
                    pos.Y = 155;
            }

            //wall between kitchen and hallway
            if ((pos.Y > 170) && (pos.Y < 200) && (pos.X > 40) && (pos.X < 190))
            {
                if (pos.Y < 185)
                    pos.Y = 170;
                else
                    pos.Y = 200;
            }

            //wall between bedroom and toilet
            if ((pos.Y > 170) && (pos.Y < 210) && (pos.X > 330))
            {
                if (pos.Y < 190)
                    pos.Y = 170;
                else
                    pos.Y = 210;
            }

            //wall between living room and toilet
            if ((pos.Y < 130) && (pos.X > 320) && (pos.X < 440))        //why 440?!?!
            {
                if (pos.X < 380)
                    pos.X = 320;
                else
                    pos.X = 440;
            }

            //wall between kitchen and bedroom
            if ((pos.Y > 180) && (pos.X > 190) && (pos.X < 320))
            {
                if (pos.X < 260)
                    pos.X = 190;
                else
                    pos.X = 320;
            }

        }
        private void fillinBlood(int score)
        {
            spriteBatch.Draw(bloodBase, new Vector2(730f, 0f), Color.White);

            int tmp1, tmp2;      //for blood length count    

            //score 1~5  draw leg
            if (score > 0)
            {
                //spriteBatch.Draw(blood, new Vector2(753f, 80f), new Rectangle(0, 0, 8, 30), Color.White);  //right leg
                //spriteBatch.Draw(blood, new Vector2(769f, 80f), new Rectangle(0, 0, 8, 30), Color.White);  //left leg
                if (score > 5)
                {
                    tmp1 = 30;
                    tmp2 = 80;
                }
                else
                {
                    tmp1 = score * 6;
                    tmp2 = 80 + (5 - score) * 6;
                }

                spriteBatch.Draw(blood, new Vector2(753f, (float)tmp2), new Rectangle(0, 0, 8, tmp1), Color.White);  //right leg
                spriteBatch.Draw(blood, new Vector2(769f, (float)tmp2), new Rectangle(0, 0, 8, tmp1), Color.White);  //left leg

                if (score >= 5)
                    spriteBatch.Draw(blood, new Vector2(753f, 73f), new Rectangle(0, 0, 24, 7), Color.White);
            }

            //score 6-9 draw body
            if (score > 5)
            {
                //spriteBatch.Draw(blood, new Vector2(760f, 46f), new Rectangle(0, 0, 8, 28), Color.White);
                if (score > 9)
                {
                    tmp1 = 28;
                    tmp2 = 46;
                }
                else
                {
                    tmp1 = (score - 5) * 7;
                    tmp2 = 46 + (4 - (score - 5)) * 7;
                }
                spriteBatch.Draw(blood, new Vector2(760f, (float)tmp2), new Rectangle(0, 0, 8, tmp1), Color.White);
            }


            //10~14 draw hand
            if (score > 9)
            {
                //spriteBatch.Draw(blood, new Vector2(768f, 46f), new Rectangle(0, 0, 20, 8), Color.White);      //left hand
                //spriteBatch.Draw(blood, new Vector2(740f, 46f), new Rectangle(0, 0, 20, 8), Color.White);      //right hand

                if (score > 14)
                {
                    tmp1 = 20;
                    tmp2 = 740;
                }
                else
                {
                    tmp1 = (score - 9) * 4;
                    tmp2 = 740 + (20 - tmp1);
                }
                spriteBatch.Draw(blood, new Vector2(768f, 46f), new Rectangle(0, 0, tmp1, 8), Color.White);          //left hand
                spriteBatch.Draw(blood, new Vector2((float)tmp2, 46f), new Rectangle(0, 0, tmp1, 8), Color.White);  //right hand
            }

            //15~16 draw neck
            if (score > 14)
            {
                //spriteBatch.Draw(blood, new Vector2(760f, 37f), new Rectangle(0, 0, 8, 9), Color.White);
                if (score == 15)
                    spriteBatch.Draw(blood, new Vector2(760f, 41f), new Rectangle(0, 0, 8, 5), Color.White);
                else
                    spriteBatch.Draw(blood, new Vector2(760f, 37f), new Rectangle(0, 0, 8, 9), Color.White);
            }

            //17~22 draw head
            if ((score > 16) && (score <22))
            {
                //spriteBatch.Draw(blood, new Vector2(749f, 14f), new Rectangle(0, 0, 31, 23), Color.White);     //score:22 dont need to draw
                spriteBatch.Draw(blood, new Vector2(749f, 17 + (5 - (score - 16)) * 4), new Rectangle(0, 0, 31, (score - 16) * 4), Color.White);
            }

        }

        private bool checkAns(int chk_list, int chk_choice, int chk_correct) //user choice, list, correctans
        {
            int[] chkclick;
            chkclick = new int[21];
            if ((chk_correct == chk_choice) && (chkclick[chk_choice]<3))
            {
                chkclick[chk_choice]++;
                return true;
            }
            else
                return false;
        }

    }
}
