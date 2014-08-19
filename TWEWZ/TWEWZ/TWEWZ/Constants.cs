#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO;
#endregion Using Statements

/// The World Ends With Zombies
/// Man & Coffee Games
/// Basic class of constants for the game

namespace TWEWZ
{
    public static class Constants
    {
        #region Screen Stuff
        public static int TOTAL_WIDTH = 1280;
        public static int TOTAL_HEIGHT = 720;

        public static float BASE_WIDTH = 756;
        public static float BASE_HEIGHT = 426;

        public static float SCALE_WIDTH = TOTAL_WIDTH / BASE_WIDTH;
        public static float SCALE_HEIGHT = TOTAL_HEIGHT / BASE_HEIGHT;
        public static Vector2 SCALE = new Vector2(SCALE_WIDTH, SCALE_HEIGHT);

        public static int PLAY_AREA_TOP_BASE = 0;
        public static int PLAY_AREA_LEFT_BASE = 0;
        public static int PLAY_AREA_BOTTOM_BASE = 426;
        public static int PLAY_AREA_RIGHT_BASE = 756;

        public static int PLAY_AREA_TOP = (int)(PLAY_AREA_TOP_BASE * SCALE_HEIGHT);
        public static int PLAY_AREA_LEFT = (int)(PLAY_AREA_LEFT_BASE * SCALE_WIDTH);
        public static int PLAY_AREA_BOTTOM = (int)(PLAY_AREA_BOTTOM_BASE * SCALE_HEIGHT);
        public static int PLAY_AREA_RIGHT = (int)(PLAY_AREA_RIGHT_BASE * SCALE_WIDTH);
        public static Rectangle PLAY_AREA = new Rectangle(PLAY_AREA_LEFT, PLAY_AREA_TOP, PLAY_AREA_RIGHT - PLAY_AREA_LEFT, PLAY_AREA_BOTTOM - PLAY_AREA_TOP);
        public static Rectangle PLAY_AREA_Minus1 = new Rectangle(PLAY_AREA_LEFT + 1, PLAY_AREA_TOP + 1, PLAY_AREA_RIGHT - PLAY_AREA_LEFT - 1, PLAY_AREA_BOTTOM - PLAY_AREA_TOP - 2);

        public const int MAX_OBJECTS_BEFORE_SUBDIVIDE = 4; // Used in QuadTree
        #endregion Screen Stuff

        #region Levels/Weapons
        //add in police station
        public static List<String> levels = new List<string> { "Tutorial", "Field", "JunkYard", "Hospital", "wHouse" };
        public static List<String> weaponNames = new List<string> { "Fists", "Pistol", "Lead Pipe", "Shotgun", "Chainsaw", "Machine Gun", "Grenade", "Railgun", "Rocket Launcher", "Flamethrower" };
        #endregion Levels/Weapons

        #region Player Stuff
        public static int PLAYER_BASE_SIZE = 21; // Used for sprite sheet dimensions
        public static int PLAYER_SIZE_X = (int)(PLAYER_BASE_SIZE * SCALE_WIDTH);
        public static int PLAYER_SIZE_Y = (int)(PLAYER_BASE_SIZE * SCALE_HEIGHT);
        public const int PLAYER_HEALTH = 100;
        public static int PLAYER_OUTER_BOUNDS = 85; // How close a zombie has to be to player to be "close"
        public const int FRAME_TIME = 75;
        #endregion Player Stuff

        #region Zombie
        public const int ZOMBIE_BASE_SIZE = 20; // Width & Height
        public static int ZOMBIE_SIZE_X = (int)(ZOMBIE_BASE_SIZE * SCALE_WIDTH);
        public static int ZOMBIE_SIZE_Y = (int)(ZOMBIE_BASE_SIZE * SCALE_HEIGHT);
        public const int ZOMBIE_HEALTH = 50; // Zombie's health
        public const int ZOMBIE_DAMAGE = 1; // Zombie attack damage
        public const int WANDER_DEGREE_CHANGE = 60; // Changes how much a zombie can change in degrees (both plus and minus)
        public const int MININUM_WANDER_DEGREE_CHANGE = 5; // They must change direction by at least this much

        //appropriate score values for zombies
        public const int NORMAL_ZOMBIE_SCORE = 10;
        public const int FLAMING_ZOMBIE_SCORE = 11;
        public const int ZOMBIE_DOG_SCORE = 5;
        public const int DOCTOR_ZOMBIE_SCORE = 12;
        public const int SWAT_ZOMBIE_SCORE = 15;

        public const int ZOMBIE_FRAME_TIME = 150;
        #endregion Zombie

        #region Melee / Stamina
        public const int MELEE_BASE_SIZE = 15; // Width & Height
        public static int MELEE_SIZE_X = (int)(MELEE_BASE_SIZE * SCALE_WIDTH);
        public static int MELEE_SIZE_Y = (int)(MELEE_BASE_SIZE * SCALE_HEIGHT);
        public static int MELEE_VAR = (int)(((((PLAYER_SIZE_X - MELEE_SIZE_X) / 2) + MELEE_SIZE_X) + (((PLAYER_SIZE_Y - MELEE_SIZE_Y) / 2) + MELEE_SIZE_Y)) / 2);
        public const int MAX_STAMINA = 100;
        public const int STAMINA_DECREMENT_AMOUNT = 2;
        public const int LOOPS_TILL_STAMINA_USAGE = 2;
        #endregion Melee / Stamina

        #region Ammo / Bullet
        public const int AMMO_BASE_SIZE = 6; // Width & Height
        public static int AMMO_SIZE_X = (int)(AMMO_BASE_SIZE * SCALE_WIDTH);
        public static int AMMO_SIZE_Y = (int)(AMMO_BASE_SIZE * SCALE_HEIGHT);
        public const int ROCKET_BASE_SIZE = 12; // Width & Height
        public static int ROCKET_SIZE_X = (int)(ROCKET_BASE_SIZE * SCALE_WIDTH);
        public static int ROCKET_SIZE_Y = (int)(ROCKET_BASE_SIZE * SCALE_HEIGHT);
        public const int GRENADE_BASE_SIZE = 8; // Width & Height
        public static int GRENADE_SIZE_X = (int)(GRENADE_BASE_SIZE * SCALE_WIDTH);
        public static int GRENADE_SIZE_Y = (int)(GRENADE_BASE_SIZE * SCALE_HEIGHT);

        public const int AMMO_GAIN_FROM_CASE = 25;
        public const int BULLET_SPEED = 24; //Speed of bullets (Must be divisible by BULLET_COLLISION_CHECK_SPEED to work perfectly)
        public const int BULLET_COLLISION_CHECK_SPEED = 4; //moves the bullet this amount until it reaches it's speed during collision testing

        public const int EXPLOSION_SIZE = 260;

        public static TimeSpan NEW_WEAPON_MESSAGE_TIME = new TimeSpan(0, 0, 0, 0, 5000);
        #endregion Ammo / Bullet

        #region Wall
        public static int WALL_BASE_SIZE = 21;
        public static int WALL_SIZE_X = (int)(WALL_BASE_SIZE * SCALE_WIDTH);
        public static int WALL_SIZE_Y = (int)(WALL_BASE_SIZE * SCALE_HEIGHT);
        public static int WALL_PIECE_BASE_SIZE = 3; // Width & Height per wall piece
        public static int WALL_PIECE_SIZE_X = (int)(WALL_PIECE_BASE_SIZE * SCALE_WIDTH);
        public static int WALL_PIECE_SIZE_Y = (int)(WALL_PIECE_BASE_SIZE * SCALE_HEIGHT);
        public static int WALL_HEALTH = 10; //amount of damage each wall piece can take
        #endregion Wall

        #region Item / Food-Barricade
        public const int ITEM_BASE_SIZE = 21; // Width & Height
        public static int ITEM_SIZE_X = (int)(ITEM_BASE_SIZE * SCALE_WIDTH);
        public static int ITEM_SIZE_Y = (int)(ITEM_BASE_SIZE * SCALE_HEIGHT);
        public const int FOOD_HEAL = 25;
        public static int BARRICADE_HEALTH = 300; //amount of damage each barricade can take
        #endregion Item / Food-Barricade

        #region Weapon Damage Array
        public static int[] weaponsDamage = new int[13] { 
            2, /*0 = fists*/
            9, /*1 = pistol*/
            4, /*2 = pipe*/
            7, /*3 = shotgun*/
            5, /*4 = chainsaw*/
            7, /*5 = bullpup (machine gun)*/
            ZOMBIE_HEALTH, /*6 = grenade*/
            5, /*7 = railgun*/
            ZOMBIE_HEALTH, /*8 = rocket*/
            26, /*9 = flamethrower*/
            0, 0, 0 }; /*10= food --- 11= fist animation --- 12= pipe animation*/
        #endregion Weapon Damage Array

        #region GamePad Controls
        public static Buttons SELECT_BUTTON, RETURN_BUTTON, ATTACK_BUTTON1, ATTACK_BUTTON2, EAT_BUTTON, BARRICADE_BUTTON, PAUSE_BUTTON,
            EXIT_BUTTON, SWITCH_FORWARD, SWITCH_BACK, PREVIOUS_WEAPON, SWITCH_RIGHT, SWITCH_LEFT, SWITCH_UP, SWITCH_DOWN;

        public static void SET_DEFAULT_BUTTONS()
        {
            SELECT_BUTTON = Buttons.A;
            RETURN_BUTTON = Buttons.B;

            ATTACK_BUTTON1 = Buttons.RightTrigger;
            ATTACK_BUTTON2 = Buttons.A;
            EAT_BUTTON = Buttons.Y;
            BARRICADE_BUTTON = Buttons.X;
            PAUSE_BUTTON = Buttons.Start;
            EXIT_BUTTON = Buttons.Back;

            SWITCH_FORWARD = Buttons.RightShoulder;
            SWITCH_BACK = Buttons.LeftShoulder;
            PREVIOUS_WEAPON = Buttons.LeftTrigger;

            SWITCH_RIGHT = Buttons.DPadRight;
            SWITCH_LEFT = Buttons.DPadLeft;
            SWITCH_UP = Buttons.DPadUp;
            SWITCH_DOWN = Buttons.DPadDown;
        }

        private static int passes = 0; // This keeps the recursion from looping forever (when switching conflicting buttons)
        public static void SET_BUTTON_BY_INDEX(int num, Buttons bttn)
        {
            int ii = 0;

            #region If one of the Buttons is the same as the one being set, this calls this methods again and switches them.
            List<Buttons> list = GET_BUTTONS_LIST();
            if (num <= 1) // This checks the Select / Return buttons, as it doesn't matter if they conflict with in-game buttons.
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i != num && list[i] == bttn && passes == 0)
                    {
                        passes++;
                        SET_BUTTON_BY_INDEX(i, list[num]);
                        break;
                    }
                }
            }
            else // This checks in-game buttons.
            {
                for (int i = 2; i < list.Count; i++)
                {
                    if (i != num && list[i] == bttn && passes == 0)
                    {
                        passes++;
                        SET_BUTTON_BY_INDEX(i, list[num]);
                        break;
                    }
                }
            }
            #endregion If one of the Buttons is the same as the one being set, this calls this methods again and switches them.

            if (num == ii++)
                SELECT_BUTTON = bttn;
            if (num == ii++)
                RETURN_BUTTON = bttn;

            if (num == ii++)
                ATTACK_BUTTON1 = bttn;
            if (num == ii++)
                ATTACK_BUTTON2 = bttn;
            if (num == ii++)
                EAT_BUTTON = bttn;
            if (num == ii++)
                BARRICADE_BUTTON = bttn;
            if (num == ii++)
                PAUSE_BUTTON = bttn;
            if (num == ii++)
                EXIT_BUTTON = bttn;
            if (num == ii++)
                SWITCH_FORWARD = bttn;
            if (num == ii++)
                SWITCH_BACK = bttn;
            if (num == ii++)
                PREVIOUS_WEAPON = bttn;
            passes = 0;
        }

        public static List<Buttons> GET_BUTTONS_LIST()
        {
            return new List<Buttons>()
            {
                SELECT_BUTTON,
                RETURN_BUTTON,
                ATTACK_BUTTON1,
                ATTACK_BUTTON2,
                EAT_BUTTON,
                BARRICADE_BUTTON,
                PAUSE_BUTTON,
                EXIT_BUTTON,
                SWITCH_FORWARD,
                SWITCH_BACK,
                PREVIOUS_WEAPON
            };
        }

        public static bool MENU_UP(GamePadState gamePad, GamePadState prevPad)
        {
            if ((gamePad.IsButtonUp(Buttons.DPadUp) && prevPad.IsButtonDown(Buttons.DPadUp)) ||
                (gamePad.IsButtonUp(Buttons.LeftThumbstickUp) && prevPad.IsButtonDown(Buttons.LeftThumbstickUp)) ||
                (gamePad.IsButtonUp(Buttons.RightThumbstickUp) && prevPad.IsButtonDown(Buttons.RightThumbstickUp)))
                return true;
            return false;
        }
        public static bool MENU_RIGHT(GamePadState gamePad, GamePadState prevPad)
        {
            if ((gamePad.IsButtonUp(Buttons.DPadRight) && prevPad.IsButtonDown(Buttons.DPadRight)) ||
                (gamePad.IsButtonUp(Buttons.LeftThumbstickRight) && prevPad.IsButtonDown(Buttons.LeftThumbstickRight)) ||
                (gamePad.IsButtonUp(Buttons.RightThumbstickRight) && prevPad.IsButtonDown(Buttons.RightThumbstickRight)))
                return true;
            return false;
        }
        public static bool MENU_DOWN(GamePadState gamePad, GamePadState prevPad)
        {
            if ((gamePad.IsButtonUp(Buttons.DPadDown) && prevPad.IsButtonDown(Buttons.DPadDown)) ||
                (gamePad.IsButtonUp(Buttons.LeftThumbstickDown) && prevPad.IsButtonDown(Buttons.LeftThumbstickDown)) ||
                (gamePad.IsButtonUp(Buttons.RightThumbstickDown) && prevPad.IsButtonDown(Buttons.RightThumbstickDown)))
                return true;
            return false;
        }
        public static bool MENU_LEFT(GamePadState gamePad, GamePadState prevPad)
        {
            if ((gamePad.IsButtonUp(Buttons.DPadLeft) && prevPad.IsButtonDown(Buttons.DPadLeft)) ||
                (gamePad.IsButtonUp(Buttons.LeftThumbstickLeft) && prevPad.IsButtonDown(Buttons.LeftThumbstickLeft)) ||
                (gamePad.IsButtonUp(Buttons.RightThumbstickLeft) && prevPad.IsButtonDown(Buttons.RightThumbstickLeft)))
                return true;
            return false;
        }
        #endregion GamePad Controls

        #region Keyboard Controls
        public static Keys KEY_UP, KEY_RIGHT, KEY_DOWN, KEY_LEFT, BARRICADE_KEY, EAT_KEY, PAUSE_KEY, EXIT_KEY,
            WEAPON1, WEAPON2, WEAPON3, WEAPON4, WEAPON5, WEAPON6, WEAPON7, WEAPON8, WEAPON9, WEAPON10;

        public static void SET_DEFAULT_KEYS()
        {
            KEY_UP = Keys.W;
            KEY_RIGHT = Keys.D;
            KEY_DOWN = Keys.S;
            KEY_LEFT = Keys.A;

            BARRICADE_KEY = Keys.Q;
            EAT_KEY = Keys.E;
            PAUSE_KEY = Keys.P;
            EXIT_KEY = Keys.Escape;

            WEAPON1 = Keys.D1;
            WEAPON2 = Keys.D2;
            WEAPON3 = Keys.D3;
            WEAPON4 = Keys.D4;
            WEAPON5 = Keys.D5;
            WEAPON6 = Keys.D6;
            WEAPON7 = Keys.D7;
            WEAPON8 = Keys.D8;
            WEAPON9 = Keys.D9;
            WEAPON10 = Keys.D0;
        }

        private static int key_passes = 0; // This keeps the recursion from looping forever (when switching conflicting buttons)
        public static void SET_KEY_BY_INDEX(int num, Keys key)
        {
            int ii = 0;

            #region If one of the Buttons is the same as the one being set, this calls this methods again and switches them.
            List<Keys> list = GET_KEY_LIST();
            for (int i = 0; i < list.Count; i++)
            {
                if (i != num && list[i] == key && key_passes == 0)
                {
                    key_passes++;
                    SET_KEY_BY_INDEX(i, list[num]);
                    break;
                }
            }
            #endregion If one of the Buttons is the same as the one being set, this calls this methods again and switches them.

            if (num == ii++)
                KEY_UP = key;
            if (num == ii++)
                KEY_RIGHT = key;
            if (num == ii++)
                KEY_DOWN = key;
            if (num == ii++)
                KEY_LEFT = key;

            if (num == ii++)
                BARRICADE_KEY = key;
            if (num == ii++)
                EAT_KEY = key;
            if (num == ii++)
                PAUSE_KEY = key;
            if (num == ii++)
                EXIT_KEY = key;

            key_passes = 0;
        }

        public static List<Keys> GET_KEY_LIST()
        {
            return new List<Keys>()
            {
                KEY_UP, KEY_RIGHT, KEY_DOWN, KEY_LEFT, BARRICADE_KEY, EAT_KEY, PAUSE_KEY, EXIT_KEY,
                WEAPON1, WEAPON2, WEAPON3, WEAPON4, WEAPON5, WEAPON6, WEAPON7, WEAPON8, WEAPON9, WEAPON10
            };
        }

        public static bool MOVE_UP(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(KEY_UP))
                return true;
            return false;
        }
        public static bool MOVE_RIGHT(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(KEY_RIGHT))
                return true;
            return false;
        }
        public static bool MOVE_DOWN(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(KEY_DOWN))
                return true;
            return false;
        }
        public static bool MOVE_LEFT(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(KEY_LEFT))
                return true;
            return false;
        }
        public static int WEAPON_CHOSEN(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(WEAPON1))
                return 0;
            if (keyboard.IsKeyDown(WEAPON2))
                return 1;
            if (keyboard.IsKeyDown(WEAPON3))
                return 2;
            if (keyboard.IsKeyDown(WEAPON4))
                return 3;
            if (keyboard.IsKeyDown(WEAPON5))
                return 4;
            if (keyboard.IsKeyDown(WEAPON6))
                return 5;
            if (keyboard.IsKeyDown(WEAPON7))
                return 6;
            if (keyboard.IsKeyDown(WEAPON8))
                return 7;
            if (keyboard.IsKeyDown(WEAPON9))
                return 8;
            if (keyboard.IsKeyDown(WEAPON10))
                return 9;

            return -1;
        }
        #endregion Keyboard Controls

        #region Colors
        public static Color INVENTORY_GRAY = Color.FromNonPremultiplied(50, 50, 50, 255);
        public static Color PAUSE_OUTER_GRAY = Color.FromNonPremultiplied(50, 50, 50, 200);
        public static Color WALL_GRAY = Color.FromNonPremultiplied(150, 150, 150, 255);
        public static Color STAT_EMPTY_BORDER_COLOR = Color.FromNonPremultiplied(20, 160, 200, 75);
        #endregion Colors

        #region Methods
        public static float DegreeToRadian(int degree)
        {
            return (float)(degree / (180 / Math.PI));
        }

        public static void Wait(int value)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < value)
            {
            }
            watch.Stop();
        }
        #endregion Methods
    }
}