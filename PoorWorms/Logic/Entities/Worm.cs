using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoorWorms.Logic.Entities.Weapons;
using PoorWorms.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Entities
{
    public class Worm : CircularBody
    {
        public static float MaxShootPower { get; } = 1.4f;
        public static float MinShootPower { get; } = 0.1f;


        public bool Active { get; set; } = false;
        public bool IsStable => !IsJumping && OnTheGround;
        public Texture2D CrosshairTexture { get; set; }
        public int CrosshairAngle { get; set; } = 0;
        public bool Controllable { get; set; }
        public readonly int MaxHealth = 100;
        public bool Grave => Health <= 0;
        public Vector2 CrosshairDirection
        {
            get
            {
                Vector2 dir = new Vector2((float)Math.Cos(AimAngle * Math.PI / 180),-(float)Math.Sin(AimAngle * Math.PI / 180));
                //if (!Direction)
                   // dir = new Vector2(dir.X * -1, dir.Y);
                return dir;
            }
        }
        public bool Direction
        {
            get
            {
                return CrosshairDirection.X > 0;
            }
        } //false - left, true - right


        public float AimAngle
        {
            get
            {
                return _aimAngle;
            }
            set
            {
                _aimAngle = value%360;
            }
        }
        public Vector2 CrosshairPos
        {
            get
            {
                return CrosshairDirection * CrosshairDistance + Position;
            }
        }
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                _health = value;
                Color[] colors = new Color[_healthW * _healthH];
                for (int x = 0; x < _healthW; x++)
                {
                    for (int y = 0; y < _healthH; y++)
                    {
                        int index = x + y * _healthW;
                        int indicator = (int)MathHelperExtension.Map(Health, 0, 100, 0, _healthW);
                        if (x >= indicator)
                        {
                            colors[index] = Color.Transparent;
                        }
                        else
                        {
                            colors[index] = TeamColor;
                        }
                    }
                }
                _healthTexture.SetData(colors);
            }
        }


        public int CrosshairDistance { get; set; } = 100;
        public Weapon CurrentWeapon { get; set; }

        public Color TeamColor { get; }

        private KeyboardState _prevState;

        public override bool Dead { get => base.Dead; set { base.Dead = value;Health = 0; } }

        private Texture2D _chargeTexture;//charge vars
        private bool _charging = false;
        private int _chargeW = 64;
        private int _chargeH = 5;

        private Texture2D _healthTexture;//health vars
        private int _healthW = 64;
        private int _healthH = 10;
        private int _health;

        private float _aimAngle = 0;


        public static Worm Create(Color team, bool controllable)
        {
            Worm worm = new Worm(32, Assets.PlayerTexture, Assets.CrossTexture,team,controllable);
            return worm;
        }

        public Worm(float radius, Texture2D wormTexture, Texture2D crosshairTexture, Color teamColor,bool controllable) : base(radius,wormTexture)
        {
            if (crosshairTexture == null)
                throw new ArgumentNullException("crosshairTexture was null");
            Friction = new Vector2(0.1f);
            Mass = 20f;
            TeamColor = teamColor;
            CrosshairTexture = crosshairTexture;
            _chargeTexture = new Texture2D(Assets.GraphicsDevice, _chargeW, _chargeH);
            _healthTexture = new Texture2D(Assets.GraphicsDevice, _healthW, _healthH);
            Controllable = controllable;
            Health = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardState state = Keyboard.GetState();
            if (IsStable && Active && !Grave && Controllable)
            {
                if (state.IsKeyUp(Keys.A) && _prevState.IsKeyDown(Keys.A))
                {
                    StateMachine.GameState = GameStates.Jump;
                    JumpLeft();
                }
                if (state.IsKeyUp(Keys.D) && _prevState.IsKeyDown(Keys.D))
                {
                    StateMachine.GameState = GameStates.Jump;
                    JumpRight();
                }
                if (state.IsKeyDown(Keys.Up))
                {
                    CrossUp();
                }
                if (state.IsKeyDown(Keys.Down))
                {
                    CrossDown();
                }
                if (state.IsKeyUp(Keys.Right) && _prevState.IsKeyDown(Keys.Right))
                {
                    TurnRight();
                }
                if (state.IsKeyUp(Keys.Left) && _prevState.IsKeyDown(Keys.Left))
                {
                    TurnLeft();
                }
                if (state.IsKeyDown(Keys.Space))
                {
                    StateMachine.GameState = GameStates.Charge;
                }
                if (state.IsKeyUp(Keys.Space) && _prevState.IsKeyDown(Keys.Space) || CurrentWeapon.Charge == 100)
                {
                    StateMachine.GameState = GameStates.Shoot;

                }
            }
            _prevState = state;
        }
        private float _x = 0.3f;
        private float _y = 1f;
        public void JumpLeft()
        {
            ApplyForce(new Vector2(-_x, -_y));
        }
        public void JumpRight()
        {
            ApplyForce(new Vector2(_x, -_y));
        }
        public void TurnLeft()
        {
            if (Direction)
            {
                AimAngle = 180 - AimAngle;
            }


        }
        public void TurnRight()
        {
            if (!Direction)
            {
                AimAngle = 180 - AimAngle;
            }

        }
        public void CrossDown()
        {
            if (Direction)
            {
                if (AimAngle > -90)
                    AimAngle--;

            }
            else
            {
                if (AimAngle < 268)
                    AimAngle++;
            }
        }
        public void CrossUp()
        {
            if (Direction)
            {
                if (AimAngle < 90)
                    AimAngle++;

            }
            else
            {
                if (AimAngle > 92)
                    AimAngle--;

            }
        }
        public void Charge()
        {
            if(CurrentWeapon!=null)
            {
                _charging = true;
                if (CurrentWeapon.NeedToCharge)
                {
                    CurrentWeapon.Charge++;
                    int indicator =(int) MathHelperExtension.Map(CurrentWeapon.Charge, 0, 100, 0, _chargeW);
                    Color[] Colors = new Color[_chargeW * _chargeH];
                    for (int x = 0; x < _chargeW; x++)
                    {
                        for (int y = 0; y < _chargeH; y++)
                        {
                            int index = x + y * _chargeW;
                            if(x < indicator)
                            {
                                if(y< _chargeH / 2)
                                {
                                    Colors[index] = Color.Red;
                                }
                                else
                                {
                                    Colors[index] = Color.Orange;
                                }
                            }
                            else
                            {
                                Colors[index] = Color.Transparent;
                            }
                        }
                    }
                    _chargeTexture.SetData(Colors);
                }
            }
        }
        public Weapon Release()
        {
            _charging = false;
            Weapon weapon = CurrentWeapon;
            float power = MathHelperExtension.Map((float)weapon.Release(), 0, 100, MinShootPower, MaxShootPower);
            weapon.Position = Position;
            weapon.Acceleration = Vector2.Zero;
            Vector2 Dir = Vector2.Normalize(CrosshairDirection);
            weapon.Velocity = Dir * power;
            Active = false;
            Assets.Camera.CameraMovement.CameraLock = weapon;
            CurrentWeapon = WeaponFactory.Create("Rocket", this);
            Assets.Entities.Add(CurrentWeapon);
            return weapon;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Point p = new Point(Texture.Width, Texture.Height);
            Point toDraw = Position.ToPoint() - new Point(p.X / 2, p.Y / 2);
            if (!Grave)
            {
                spriteBatch.Draw(Texture, destinationRectangle: new Rectangle(toDraw, p), color: Color.White);
                spriteBatch.Draw(_healthTexture, Position - new Vector2(_healthW / 2, _healthW * 1.5f), Color.White);
                if (Active)
                {
                    Point cross = new Point(CrosshairTexture.Width, CrosshairTexture.Height);

                    spriteBatch.Draw(CrosshairTexture, CrosshairPos - cross.ToVector2() / 2, color: Color.White);
                }
                if (_charging)
                {
                    spriteBatch.Draw(_chargeTexture, Position - new Vector2(_chargeW / 2, _chargeW), Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(Assets.GraveTexture, destinationRectangle: new Rectangle(toDraw, p), color: Color.White);
            }

        }
    }
}
