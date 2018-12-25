using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoorWorms.Logic.Camera;
using PoorWorms.Logic.Entities;
using PoorWorms.Logic.Entities.Weapons;
using PoorWorms.Logic.Map;
using PoorWorms.Logic.Team;
using PoorWorms.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoorWorms
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PerlinNoiseMap map;
        MapCollider mc;
        int _w;
        int _h;
        Camera camera;
        CameraMovement cm;
        TeamFactory teamFactory;
        Random r = new Random();

        int playersInTeam = 4;
        int teams = 2;

        float gravity = 0.05f;

        bool gameOver = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            _w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = _w;
            graphics.PreferredBackBufferHeight = _h;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;

            

            maptext = new Texture2D(graphics.GraphicsDevice, _w * 2, _h * 2);

            Assets.Camera = new Camera(new Rectangle(0, 0, _w * 2, _h * 2), new Rectangle(0,0,_w,_h));
            Assets.CrossTexture = Content.Load<Texture2D>("Cross");
            Assets.Entities = new List<CircularBody>();
            Assets.GraphicsDevice = GraphicsDevice;
            Assets.Map = new PerlinNoiseMap(_w * 2, _h * 2, maptext);
            Assets.PlayerTexture = Content.Load<Texture2D>("Player");
            Assets.RocketTexture = Content.Load<Texture2D>("RocketSpritesheet");
            Assets.GraveTexture = Content.Load<Texture2D>("Grave");
            Assets.EndGameFont = Content.Load<SpriteFont>("Font");

            StateMachine.GameState = GameStates.Restart;
        }
        Texture2D maptext;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }
        protected override void UnloadContent()
        {

        }
        int octaves = 9;
        bool zoomFlag = true;
        KeyboardState prevKeyb;
        Weapon releasedWeapon = null;
        BoomParticle[] particles = null;
        Worm prev;
        Worm curr;
        Worm target;
        int steps;
        Vector2 dir;
        float angle;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && prevKeyb.IsKeyDown(Keys.Tab))
            {
                
                zoomFlag = !zoomFlag;
            }
           
            Vector2 pos = Mouse.GetState().Position.ToVector2();
            Worm current = null;
            if (teamFactory!=null)
                current = teamFactory.ActiveWorm;
            
            switch (StateMachine.GameState)
            {
                case GameStates.Restart:
                    {

                        teamFactory = new TeamFactory(playersInTeam, teams);
                        foreach (Worm w in teamFactory)
                        {
                            w.Position = new Vector2(r.Next(100, 2 * _w - 30), 100);
                        }


                        map = (PerlinNoiseMap)Assets.Map;
                        map.GenerateMap(octaves);
                        mc = new MapCollider(map);
                        map.UpdateTexture();

                        camera = Assets.Camera;
                        cm = camera.CameraMovement;
                        cm.MovementOffset = 100;
                        cm.CameraLock = teamFactory.ActiveWorm;
                        Assets.Entities = new List<CircularBody>();
                        Assets.Entities.AddRange(teamFactory);
                        prev = null;
                        curr = null;
                        target = null;
                        particles = null;
                        releasedWeapon = null;
                        gameOver = false;
                        zoomFlag = true;
                        StateMachine.GameState = GameStates.Deploy;
                        break;
                    }
                case GameStates.Deploy:
                    {
                        if (teamFactory.All(x => x.IsStable))
                        {
                            StateMachine.GameState = GameStates.Walk;
                        }
                        break;
                    }
                case GameStates.Walk:
                    {
                        if (!current.Controllable)
                        {
                            target = teamFactory.SelectTarget();
                            steps = r.Next(1, 3);
                            dir = Vector2.Normalize(target.Position - current.Position);
                            StateMachine.GameState = GameStates.Jump;
                        }
                        break;
                    }
                case GameStates.Jump:
                    {
                        if (!current.Controllable)
                        {
                            if (steps > 0)
                            {
                                if (current.IsStable)
                                {
                                    steps--;
                                    if (dir.X < 0)
                                        current.JumpLeft();
                                    else
                                        current.JumpRight();
                                }
                            }
                            else
                            {
                                if (current.IsStable)
                                    StateMachine.GameState = GameStates.Aim;
                            }
                        }
                        break;
                    }
                case GameStates.Aim:
                    {


                        float speed = new Vector2(Worm.MaxShootPower * gameTime.ElapsedGameTime.Milliseconds).Length();
                        float g = new Vector2(0,gravity*gameTime.ElapsedGameTime.Milliseconds).Length() ;
                        Vector2 dif = (target.Position - current.Position);
                        float dist = Vector2.Distance(target.Position, current.Position);
                        float test23 = 90 - (float)(0.5 * Math.Asin(g * dist / (speed * speed)) * 180/Math.PI);
                        float dx = dif.X;
                        float dy = dif.Y;


                        float a = speed * speed * speed * speed - g * (g * dx * dx + 2 * dy * speed * speed);
                        if (a < 0)
                        {
                            steps = r.Next(1, 3);
                            dir = target.Position - current.Position;
                            StateMachine.GameState = GameStates.Jump;
                        }
                        else
                        {
                            float top = speed * speed + (float)Math.Sqrt(a);
                            float inBrackets = top / (g * dx);
                            angle = (float)(Math.Atan(inBrackets));
                            angle = MathHelper.ToDegrees(angle);
                            if(angle < 0)
                            {
                                current.TurnLeft();
                                angle = 180 - angle;
                            }
                            double dis =Math.Abs( speed * speed * Math.Sin(angle*Math.PI/90)/ (g));
                            if (current.IsStable)
                                StateMachine.GameState = GameStates.BeforeCharge;
                        }
                        break;
                    }
                case GameStates.BeforeCharge:
                    {
                        int ang = (int)angle;
                        if (current.AimAngle < ang)
                            current.CrossUp();
                        else if (curr.AimAngle > ang)
                            current.CrossDown();
                        else
                            StateMachine.GameState = GameStates.Charge;
                        break;
                    }
                case GameStates.Charge:
                    {
                        current.Charge();
                        if (current.CurrentWeapon.Charge == 100)
                            StateMachine.GameState = GameStates.Shoot;
                        break;
                    }
                case GameStates.Shoot:
                    {
                        releasedWeapon = teamFactory.ActiveWorm.Release();
                        Assets.Entities.Add(releasedWeapon);
                        StateMachine.GameState = GameStates.AfterShoot;
                        break;
                    }
                case GameStates.AfterShoot:
                    {
                        if (releasedWeapon.Dead)
                        {
                            double distance = Vector2.Distance(releasedWeapon.Position, current.Position);
                                  particles = ExplosionMaker.Explode(Assets.Map, Assets.Entities,releasedWeapon.Damage ,releasedWeapon.ExplosionRadius, releasedWeapon.ExplosionPower, releasedWeapon.Position, Assets.GraphicsDevice);
                            Assets.Entities.AddRange(particles);
                            StateMachine.GameState = GameStates.WaitingAfterShoot;
                        }
                        break;
                    }
                case GameStates.WaitingAfterShoot:
                    {
                        if (particles.All(x => x.Dead))
                        {
                            StateMachine.GameState = GameStates.ChangingPlayer;
                        }
                        break;
                    }
                case GameStates.ChangingPlayer:
                    {
                        prev = teamFactory.ActiveWorm;
                        curr = teamFactory.ChangePlayer();
                        if(prev == curr)
                        {
                            StateMachine.GameState = GameStates.GameOver;
                            break;
                        }
                        camera.CameraMovement.CameraLock = curr;
                        StateMachine.GameState = GameStates.Walk;
                        break;
                    }
                case GameStates.GameOver:
                    {
                        gameOver = true;
                        camera.CameraMovement.CameraLock = null;
                        curr.Active = false;
                        zoomFlag = false;
                        if(Keyboard.GetState().IsKeyUp(Keys.R) && prevKeyb.IsKeyDown(Keys.R))
                        {
                            StateMachine.GameState = GameStates.Restart;
                        }
                        break;
                    }
            }





            for (int i = 0; i < Assets.Entities.Count; i++)
            {
                var e = Assets.Entities[i];
                e.ApplyForce(new Vector2(0, gravity));//gravity
                e.Update(gameTime);
               
                if(mc.CollideWithCircularBody(e))
                {
                    if ( e.Bounces > 0)
                        e.Bounces--;
                }
                if (e.Position.Y >= camera.Screen.Bottom)
                {
                    e.Dead = true;
                }
            }
           
            Assets.Entities.RemoveAll(x => x.Dead);
            if (mc.WeaponAndPlayerCollide(releasedWeapon, teamFactory.ToList()))
            {
                releasedWeapon.Dead = true;
                StateMachine.GameState = GameStates.AfterShoot;
            }
            prevKeyb = Keyboard.GetState();
            cm.Update(gameTime);
            camera.Update(camera.Position);
            if (!zoomFlag)
            {
                camera.Zoom = new Vector2(0.75f, 0.75f);

            }
            else
            {
                camera.Zoom = Vector2.One;

            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(transformMatrix: camera.Transform);
            spriteBatch.Draw(maptext, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            for (int i = 0; i < Assets.Entities.Count; i++)
            {
                spriteBatch.Begin(transformMatrix: camera.Transform);
                Assets.Entities[i].Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameOver)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(Assets.EndGameFont, teamFactory.Teams[teamFactory.ActiveTeamIndex].Name + " Wins!!!", new Vector2(_w,_h)*camera.Zoom/2 - new Vector2(64,64), teamFactory.Teams[teamFactory.ActiveTeamIndex].TeamColor);
                spriteBatch.DrawString(Assets.EndGameFont, "Press R to Restart", new Vector2(_w, _h + 128) * camera.Zoom / 2, teamFactory.Teams[teamFactory.ActiveTeamIndex].TeamColor, scale: 0.5f,rotation:0,origin:Vector2.Zero,effects:default(SpriteEffects),layerDepth:0);
                spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }
    }
}
