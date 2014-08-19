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
using System.Threading;
#endregion using

/// The World Ends With Zombies
/// Man & Coffee Games
/// Main Game Loop

namespace TWEWZ
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        //Properties for the main game
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerImages, zombieImages, itemTextures, menuScreen, gameoverScreen, gameScreen, cursorImage, wallImage,
            optionsImage, creditsImage, instructionsImage, menuButtons, instructionsButtons, optionsButtons, creditsButtons, fileImage, hpBarImg, 
            stamBarImg, flameImage, explosionImage, grenadeSpot, pausedMenu, selectionOutline, loading, backButton, pausedBack,
            circle, lasagna, lgWeapons, smWeapons, lowStaminaWarning, lowHealthWarning, combo;
        SpriteFont gameText, smallText;
        Color color1_1, color2_1, color1_2, color2_2;
        Color[] greys;
        enum GameState { menu, game, paused, goToMenu, gameOver, loading }
        enum MenuState { credits, instructions, options, normal, newOrLoad, saveFile, exit }

        MenuState mState; GameState currentState;
        QuadTree tree, parent;
        Enemy newEnemy; Wall tempWall; Barricade tempBarricade;
        List<Enemy> enemies; List<Wall> walls; List<Barricade> barricades; List<Item> items; List<Melee> flameExtension; List<Ammo> multipleBullets; List<TextObject> buttonBindingText, keyBindingText;
        List<GameObject> objects; List<Ammo> fiveBlts; List<String> paths;
        KeyboardState kbState, previouskbState;
        MouseState mouseState, previousMouseState;
        GamePadState gamepadState, previousGamepadState;
        StreamWriter writer; StreamReader reader;

        int i, numEnemies, numEnemiesLeft, healthItems, currentLevel, currentWave, ammoLeft, regSpeed, maxItem, scrollCheck, prevScrollCheck, score,
            framesElapsed, frame, numFrames, fileNum, strengthNum, proficiencyNum, difficulty, waveSinceDrop, currentButtonUD, num, zombieType,
            currentButtonLR, bindingChoice, currentSong, newWeapon, prevWeapon, saveNum, width, height, messageWidth, messageHeight, 
            sizeDif, ySlide, randomPosX, randomPosY, row, col, pathNum, objectNum, xPos, yPos, lvl, str;
        bool singlePress, musicOn, soundEffectsOn, randomZombieColor, zombieHats, showOutlines, newGame, flameON, startExplosion, gamePadConnected, 
            grenadeActive, fired, allDead, shootFlag, switchFlag, empty;
        bool[] ownedWeapons, saveEmpty;
        double zombieMultiplier, timePart, timeDif, percent, percentByPart;
        float angle, buttonWidth, buttonHeight;
        byte msgAlpha;
        string savText, defaultText, firstLine, line, s, pathTotal, size;
        string[] position, objectInfo, parts, sizes;

        //Stats
        Vector2 playerLoc, mouseLoc, endLineVect;
        Random rng, posGeneratorX, posGeneratorY;
        Stopwatch stopWatch, flameWatch, tutorialWatch, loadWatch;
        TimeSpan newWeaponMessageTime;
        Player player1; Melee melee; Melee GHitSpot; Ammo laserSightAim;  Gun gun; Item drop;
        Rectangle rect, spot, rect1, rect2;
        Dictionary<string, Rectangle[]> buttons; Dictionary<string, int> stats; Dictionary<String, Song> songDictionary;

        //Sounds
        SoundEffect pistolShot, deathCry, zombieDeath, fistPunch, shotgunShot, gunEmpty, chainSaw, flameThrower, pipeHit, arFire, railGun, explosion;
        SoundEffectInstance chainsawPlay;
        Song menuMusic, theme1, theme2, theme3;
        #endregion Properties

        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion Constructor

        #region Initialize
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = Constants.TOTAL_HEIGHT;
            graphics.PreferredBackBufferWidth = Constants.TOTAL_WIDTH;

            //set to true to full screen
            //graphics.IsFullScreen = true;

            graphics.ApplyChanges();

            currentState = GameState.menu;
            mState = MenuState.normal;

            currentSong = 0; regSpeed = 5; scrollCheck = 0; prevScrollCheck = 0; fileNum = 0; ammoLeft = 50; healthItems = 0;
            waveSinceDrop = 0; maxItem = 11; zombieMultiplier = 1; numFrames = 5; difficulty = 0; currentButtonUD = 0;
            currentButtonLR = 0; bindingChoice = -1; newWeapon = -1; score = 0;

            singlePress = false; flameON = false; startExplosion = false;
            //get changed in loadValues()
            musicOn = true; soundEffectsOn = true; randomZombieColor = false; zombieHats = false; showOutlines = false;

            rng = new Random();
            stopWatch = new Stopwatch(); flameWatch = new Stopwatch(); tutorialWatch = new Stopwatch();
            stopWatch.Start(); flameWatch.Start();
            newWeaponMessageTime = TimeSpan.Zero;
            enemies = new List<Enemy>(); walls = new List<Wall>(); barricades = new List<Barricade>(); items = new List<Item>();
            ownedWeapons = new bool[maxItem]; saveEmpty = new bool[3];
            //play includes new, load options and save slots
            buttons = new Dictionary<string, Rectangle[]> {
            {"Play", new Rectangle[] 
            {
                new Rectangle((int)(100 * Constants.SCALE_WIDTH), (int)(300 * Constants.SCALE_HEIGHT), (int)(100 * Constants.SCALE_WIDTH), (int)(100 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(100 * Constants.SCALE_WIDTH), (int)(200 * Constants.SCALE_HEIGHT), (int)(97 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(200 * Constants.SCALE_WIDTH), (int)(200 * Constants.SCALE_HEIGHT), (int)(97 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(390 * Constants.SCALE_WIDTH), (int)(152 * Constants.SCALE_HEIGHT), (int)(65 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(455 * Constants.SCALE_WIDTH), (int)(152 * Constants.SCALE_HEIGHT), (int)(65 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(520 * Constants.SCALE_WIDTH), (int)(152 * Constants.SCALE_HEIGHT), (int)(65 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT))
            } },
            {"Menu", new Rectangle[] {
                new Rectangle((int)(250 * Constants.SCALE_WIDTH), (int)(300 * Constants.SCALE_HEIGHT), (int)(158 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(400 * Constants.SCALE_WIDTH), (int)(300 * Constants.SCALE_HEIGHT), (int)(151 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(550 * Constants.SCALE_WIDTH), (int)(300 * Constants.SCALE_HEIGHT), (int)(151 * Constants.SCALE_WIDTH), (int)(54 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(580 * Constants.SCALE_WIDTH), (int)(357 * Constants.SCALE_HEIGHT), (int)(151 * Constants.SCALE_WIDTH), (int)(52 * Constants.SCALE_HEIGHT))
            } },
            {"Instructions", new Rectangle[] {
                new Rectangle((int)(10 * Constants.SCALE_WIDTH), (int)(10 * Constants.SCALE_HEIGHT), (int)(75 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT))
            } },
            {"Options", new Rectangle[] {
                new Rectangle((int)(300 * Constants.SCALE_WIDTH), (int)(165 * Constants.SCALE_HEIGHT), (int)(95 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(335 * Constants.SCALE_WIDTH), (int)(165 * Constants.SCALE_HEIGHT), (int)(85 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(370 * Constants.SCALE_WIDTH), (int)(165 * Constants.SCALE_HEIGHT), (int)(115 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(300 * Constants.SCALE_WIDTH), (int)(200 * Constants.SCALE_HEIGHT), (int)(115 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(300 * Constants.SCALE_WIDTH), (int)(235 * Constants.SCALE_HEIGHT), (int)(115 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(10 * Constants.SCALE_WIDTH), (int)(10 * Constants.SCALE_HEIGHT), (int)(75 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT))
            } },
            {"Credits", new Rectangle[] {
                new Rectangle((int)(450 * Constants.SCALE_WIDTH), (int)(345 * Constants.SCALE_HEIGHT), (int)(95 * Constants.SCALE_WIDTH), (int)(17 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(10 * Constants.SCALE_WIDTH), (int)(10 * Constants.SCALE_HEIGHT), (int)(75 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT))
            } },
            {"Pause", new Rectangle[] {
                new Rectangle((int)(82 * Constants.SCALE_WIDTH), (int)(80 * Constants.SCALE_HEIGHT), (int)(123 * Constants.SCALE_WIDTH), (int)(27 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(82 * Constants.SCALE_WIDTH), (int)(116 * Constants.SCALE_HEIGHT), (int)(179 * Constants.SCALE_WIDTH), (int)(27 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(82 * Constants.SCALE_WIDTH), (int)(151 * Constants.SCALE_HEIGHT), (int)(178 * Constants.SCALE_WIDTH), (int)(27 * Constants.SCALE_HEIGHT)),
                new Rectangle((int)(82 * Constants.SCALE_WIDTH), (int)(187 * Constants.SCALE_HEIGHT), (int)(240 * Constants.SCALE_WIDTH), (int)(27 * Constants.SCALE_HEIGHT))
            } },
            {"GameOver", new Rectangle[] {
                new Rectangle((int)(10 * Constants.SCALE_WIDTH), (int)(10 * Constants.SCALE_HEIGHT), (int)(75 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT))
            } }
            };

            buttonWidth = 81 * Constants.SCALE_WIDTH; buttonHeight = 28 * Constants.SCALE_HEIGHT;

            //stats
            stats = new Dictionary<string, int>() { { "strength", 1 }, { "stamina", 100 }, { "weapons", 1 } };

            //sound
            songDictionary = new Dictionary<String, Song>();

            base.Initialize();
        }

        public void Initialize2()
        {
            player1 = new Player(Constants.PLAY_AREA_LEFT + Constants.PLAY_AREA_RIGHT / 2, Constants.PLAY_AREA_TOP + Constants.PLAY_AREA_BOTTOM / 2, Constants.PLAYER_SIZE_X, Constants.PLAYER_SIZE_Y, this.Content.Load<Texture2D>("Sprites/jim"), tree, difficulty, spriteBatch);
            playerLoc = new Vector2(player1.rectangleX, player1.rectangleY);

            gun = new Gun(player1.rectangleX + 40, player1.rectangleY + 40, 20, 1, 5, 3, spriteBatch);
            melee = new Melee(player1.rectangleX + 40, player1.rectangleY + 40, Constants.MELEE_SIZE_X, Constants.MELEE_SIZE_Y, 5, spriteBatch);
            GHitSpot = new Melee(0, 0, Constants.EXPLOSION_SIZE, Constants.EXPLOSION_SIZE, -1, spriteBatch);
            laserSightAim = new Ammo(0, 0, 4, 4, tree, 0, wallImage, spriteBatch);

            multipleBullets = new List<Ammo>();
            for (i = 0; i < 10; i++)
                multipleBullets.Add(new Ammo(player1.rectangleX + 35, player1.rectangleY + 25, Constants.AMMO_SIZE_X, Constants.AMMO_SIZE_Y, null, Ammo.Type.Bullet, itemTextures, spriteBatch));

            flameExtension = new List<Melee> { melee };
            for (i = 0; i < 11; i++)
                flameExtension.Add(new Melee(player1.rectangleX + 40, player1.rectangleY + 40, Constants.MELEE_SIZE_X, Constants.MELEE_SIZE_Y, 5, spriteBatch));

            Constants.SET_DEFAULT_BUTTONS();
            GamepadBindingLoad();
            SetButtonBindingText();
            
            Constants.SET_DEFAULT_KEYS();
            KeyBoardBindingLoad();
            SetKeyBindingText();

            tree = new QuadTree(Constants.PLAY_AREA_LEFT - 50, Constants.PLAY_AREA_TOP - 50, Constants.PLAY_AREA_RIGHT + 50, Constants.PLAY_AREA_BOTTOM + 50, null);
            tree.AddObject(player1);
            tree.AddObject(melee);
            foreach (Ammo amo in multipleBullets)
                tree.AddObject(amo);

            tree.AddObject(melee);

            foreach (Enemy en in enemies)
                tree.AddObject(en);
            foreach (Melee mle in flameExtension)
                tree.AddObject(mle);

            LoadValues();

            //load initial map
            GetMap("Tutorial");
        }
        #endregion Initialize

        #region LoadContent
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Texture2D
            //Sprites
            playerImages = this.Content.Load<Texture2D>("Sprites/jim");
            zombieImages = this.Content.Load<Texture2D>("Sprites/zombies");
            itemTextures = this.Content.Load<Texture2D>("Sprites/items");
            flameImage = this.Content.Load<Texture2D>("Sprites/Flame");
            explosionImage = this.Content.Load<Texture2D>("Sprites/Explosion");
            wallImage = this.Content.Load<Texture2D>("Sprites/Wall");
            grenadeSpot = this.Content.Load<Texture2D>("Sprites/GrenadeAim");

            //GUI/Menu Graphics
            menuScreen = this.Content.Load<Texture2D>("GUI/menu");
            instructionsImage = this.Content.Load<Texture2D>("GUI/instructions");
            optionsImage = this.Content.Load<Texture2D>("GUI/options");
            creditsImage = this.Content.Load<Texture2D>("GUI/credits");
            pausedMenu = this.Content.Load<Texture2D>("GUI/pause");
            pausedBack = this.Content.Load<Texture2D>("GUI/PausedBackground");

            gameoverScreen = this.Content.Load<Texture2D>("GUI/gameover");
            menuButtons = this.Content.Load<Texture2D>("GUI/menu_sheet");
            instructionsButtons = this.Content.Load<Texture2D>("GUI/instructions_sheet");
            optionsButtons = this.Content.Load<Texture2D>("GUI/options_sheet");
            creditsButtons = this.Content.Load<Texture2D>("GUI/credits_sheet");
            cursorImage = this.Content.Load<Texture2D>("GUI/Cursor");

            lowHealthWarning = this.Content.Load<Texture2D>("GUI/low_health");
            lowStaminaWarning = this.Content.Load<Texture2D>("GUI/low_stamina");
            combo = this.Content.Load<Texture2D>("GUI/combo");

            fileImage = this.Content.Load<Texture2D>("GUI/FileButton");
            loading = this.Content.Load<Texture2D>("GUI/loading");
            backButton = this.Content.Load<Texture2D>("GUI/btn_back");

            hpBarImg = this.Content.Load<Texture2D>("GUI/hp");
            stamBarImg = this.Content.Load<Texture2D>("GUI/stam");
            selectionOutline = this.Content.Load<Texture2D>("GUI/SelectOutline");
            circle = this.Content.Load<Texture2D>("GUI/circle");
            lasagna = this.Content.Load<Texture2D>("GUI/lasagna");
            lgWeapons = this.Content.Load<Texture2D>("GUI/largewepsheet");
            smWeapons = this.Content.Load<Texture2D>("GUI/smallwepsheet");
            #endregion Texture2D

            #region SpriteFont
            gameText = this.Content.Load<SpriteFont>("spritefonts/gameTexts");
            smallText = this.Content.Load<SpriteFont>("spritefonts/smallTexts");
            #endregion SpriteFont

            #region Sound
            //Sound Effects
            pistolShot = this.Content.Load<SoundEffect>("Sound/pistolShot");
            deathCry = this.Content.Load<SoundEffect>("Sound/deathScream");
            zombieDeath = this.Content.Load<SoundEffect>("Sound/zombieDeath");
            fistPunch = this.Content.Load<SoundEffect>("Sound/fistPunch");
            shotgunShot = this.Content.Load<SoundEffect>("Sound/Shotty");
            gunEmpty = this.Content.Load<SoundEffect>("Sound/PistolEmpty");
            chainSaw = this.Content.Load<SoundEffect>("Sound/Chainsaw");
            flameThrower = this.Content.Load<SoundEffect>("Sound/flamethrower");
            pipeHit = this.Content.Load<SoundEffect>("Sound/Pipe Hit");
            arFire = this.Content.Load<SoundEffect>("Sound/arFire");
            railGun = this.Content.Load<SoundEffect>("Sound/Rail Gun Firing");
            explosion = this.Content.Load<SoundEffect>("Sound/Grenade Explosion");
            chainsawPlay = chainSaw.CreateInstance();

            //Music
            menuMusic = this.Content.Load<Song>("Music/MenuMusic");
            theme1 = this.Content.Load<Song>("Music/purge");
            theme2 = this.Content.Load<Song>("Music/shamisen rock");
            theme3 = this.Content.Load<Song>("Music/the fall");
            songDictionary.Add("MainMenu", this.Content.Load<Song>("Music/MenuMusic"));
            songDictionary.Add("1", this.Content.Load<Song>("Music/purge"));
            songDictionary.Add("2", this.Content.Load<Song>("Music/shamisen rock"));
            songDictionary.Add("3", this.Content.Load<Song>("Music/the fall"));
            #endregion Sound
            
            Initialize2();
        }
        #endregion LoadContent

        #region Update
        protected override void Update(GameTime gameTime)
        {
            #region Update keys, mouse, frames...
            singlePress = false;

            //Gamepad
            previousGamepadState = gamepadState;
            gamepadState = GamePad.GetState(PlayerIndex.One);
            gamePadConnected = gamepadState.IsConnected;

            //Keyboard
            previouskbState = kbState;
            kbState = Keyboard.GetState();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();

            //Mouse
            if (!gamePadConnected)
                mouseLoc = new Vector2(mouseState.X, mouseState.Y);

            //Frames
            framesElapsed = (int)(gameTime.TotalGameTime.Milliseconds / 100);
            frame = framesElapsed % numFrames;
            #endregion Update keys, mouse, frames...

            switch (currentState)
            {
                #region GameState.Menu
                case GameState.menu:
                    if (musicOn && MediaPlayer.State != MediaState.Playing){
                        MediaPlayer.Stop();
                        MediaPlayer.Play(menuMusic);
                    }

                    #region Save file generation/loading from menu
                    if (mState == MenuState.normal || mState == MenuState.newOrLoad || mState == MenuState.saveFile){
                        if (mState == MenuState.normal){
                            if (CheckIfClicked(buttons["Play"][0]) || (currentButtonUD == 0 && SingleButtonPress(Constants.SELECT_BUTTON))){
                                mState = MenuState.newOrLoad;
                                saveEmpty[0] = CheckIfEmpty("Save1.sav");
                                saveEmpty[1] = CheckIfEmpty("Save2.sav");
                                saveEmpty[2] = CheckIfEmpty("Save3.sav");
                                currentButtonLR = 0;
                            }
                        }

                        else if (mState == MenuState.newOrLoad){
                            if (!gamePadConnected){
                                if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                                    mState = MenuState.normal;
                                if (CheckIfClicked(buttons["Play"][1])){
                                    newGame = true;
                                    mState = MenuState.saveFile;
                                    currentButtonLR = 0;
                                }
                                else if (CheckIfClicked(buttons["Play"][2]) && (!saveEmpty[0] || !saveEmpty[1] || !saveEmpty[2])){
                                    newGame = false;
                                    mState = MenuState.saveFile;
                                    currentButtonLR = 0;
                                }
                            }
                            else {
                                if (SingleButtonPress(Constants.RETURN_BUTTON)){
                                    mState = MenuState.normal;
                                    currentButtonLR = 0;
                                }
                                if (currentButtonUD == 0){
                                    if (currentButtonLR == 0 && SingleButtonPress(Constants.SELECT_BUTTON)){
                                        newGame = true;
                                        mState = MenuState.saveFile;
                                        currentButtonLR = 0;
                                    }
                                    else if (currentButtonLR == 1 && SingleButtonPress(Constants.SELECT_BUTTON) && (!saveEmpty[0] || !saveEmpty[1] || !saveEmpty[2])){
                                        newGame = false;
                                        mState = MenuState.saveFile;
                                        currentButtonLR = 0;
                                    }
                                }
                            }
                        }

                        else if (mState == MenuState.saveFile){
                            int saveNum = 0;
                            if (gamePadConnected){
                                if (SingleButtonPress(Constants.RETURN_BUTTON)){
                                    mState = MenuState.newOrLoad;
                                    currentButtonLR = 0;
                                }

                                if (currentButtonUD == 0){
                                    for (i = 0; i < 3; i++){
                                        saveNum = i + 1;
                                        if (currentButtonLR == i && SingleButtonPress(Constants.SELECT_BUTTON)){
                                            if (newGame){
                                                NewGame(saveNum);
                                                fileNum = saveNum;
                                                currentState = GameState.game;
                                                if (musicOn){
                                                    MediaPlayer.Stop();
                                                    MediaPlayer.Play(theme1);
                                                }
                                                ResetGame();
                                                currentButtonLR = 0;
                                            }
                                            else {
                                                if (!newGame && !saveEmpty[i]){
                                                    fileNum = saveNum;
                                                    currentState = GameState.game;
                                                    if (musicOn){
                                                        MediaPlayer.Stop();
                                                        MediaPlayer.Play(theme1);
                                                    }
                                                    ResetGame();
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            else {
                                if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                                    mState = MenuState.newOrLoad;
                                for (i = 0; i < 3; i++){
                                    saveNum = i + 1;
                                    if (CheckIfClicked(buttons["Play"][3 + i])){
                                        if (newGame){
                                            NewGame(saveNum);
                                            fileNum = saveNum;
                                            currentState = GameState.game;
                                            if (musicOn){
                                                MediaPlayer.Stop();
                                                MediaPlayer.Play(theme1);
                                            }
                                            ResetGame();
                                        }
                                        else {
                                            if (!newGame && !saveEmpty[i]){
                                                fileNum = saveNum;
                                                currentState = GameState.game;
                                                if (musicOn){
                                                    MediaPlayer.Stop();
                                                    MediaPlayer.Play(theme1);
                                                }
                                                ResetGame();
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion Save file generation/loading from menu

                        #region gamePad/Menu interactions
                        if (gamePadConnected){
                            if (currentButtonUD != 0 && currentButtonLR != 0)
                                currentButtonLR = 0;

                            if (Constants.MENU_DOWN(gamepadState, previousGamepadState))
                                currentButtonUD = LimitInts(currentButtonUD + 1, 0, 3);
                            else if (Constants.MENU_UP(gamepadState, previousGamepadState))
                                currentButtonUD = LimitInts(currentButtonUD - 1, 0, 3);

                            if (currentButtonUD == 0){
                                if (mState == MenuState.newOrLoad){
                                    if (Constants.MENU_LEFT(gamepadState, previousGamepadState))
                                        currentButtonLR = LimitInts(currentButtonLR - 1, 0, 1);
                                    if (Constants.MENU_RIGHT(gamepadState, previousGamepadState))
                                        currentButtonLR = LimitInts(currentButtonLR + 1, 0, 1);
                                }
                                else if (mState == MenuState.saveFile){
                                    if (Constants.MENU_LEFT(gamepadState, previousGamepadState))
                                        currentButtonLR = LimitInts(currentButtonLR - 1, 0, 2);
                                    if (Constants.MENU_RIGHT(gamepadState, previousGamepadState))
                                        currentButtonLR = LimitInts(currentButtonLR + 1, 0, 2);
                                }
                            }
                       
                            if (SingleButtonPress(Constants.SELECT_BUTTON)){
                                if (currentButtonUD == 1)
                                    mState = MenuState.instructions;
                                else if (currentButtonUD == 2){
                                    mState = MenuState.options;
                                    if (difficulty <= 3)
                                        currentButtonUD = difficulty;
                                    else
                                        currentButtonUD = 3;
                                }
                                else if (currentButtonUD == 3){
                                    mState = MenuState.credits;
                                    currentButtonUD = 0;
                                }
                            }
                            //change to an actual button object
                            if (SingleButtonPress(Constants.EXIT_BUTTON))
                                mState = MenuState.exit;
                        }
                        #endregion gamePad/Menu interactions

                        #region Mouse/Menu interactions
                        else{
                            if (CheckIfClicked(buttons["Menu"][0]))
                                mState = MenuState.instructions;
                            else if (CheckIfClicked(buttons["Menu"][1]))
                                mState = MenuState.options;
                            else if (CheckIfClicked(buttons["Menu"][2]))
                                mState = MenuState.credits;
                        }
                        #endregion Mouse/Menu interactions
                    }

                    #region Instructions
                    if (mState == MenuState.instructions){
                        if (gamePadConnected){
                            // This if doesn't let anything happen until the Select Button / Return Button are un-pressed so they don't activate
                            // right away after setting a new value to them.
                            if (bindingChoice <= -2){
                                if (gamepadState.IsButtonUp(Constants.SELECT_BUTTON) && previousGamepadState.IsButtonUp(Constants.SELECT_BUTTON)
                                    && gamepadState.IsButtonUp(Constants.RETURN_BUTTON) && previousGamepadState.IsButtonUp(Constants.RETURN_BUTTON))
                                    bindingChoice = -1;
                            }
                            if (bindingChoice >= 0){
                                Buttons bttn;
                                if (gamepadState.IsButtonDown(bttn = Buttons.A) || gamepadState.IsButtonDown(bttn = Buttons.B) || gamepadState.IsButtonDown(bttn = Buttons.Back) || gamepadState.IsButtonDown(bttn = Buttons.BigButton)
                                    || gamepadState.IsButtonDown(bttn = Buttons.LeftShoulder) || gamepadState.IsButtonDown(bttn = Buttons.LeftStick) || gamepadState.IsButtonDown(bttn = Buttons.LeftTrigger)
                                    || gamepadState.IsButtonDown(bttn = Buttons.RightShoulder) || gamepadState.IsButtonDown(bttn = Buttons.RightStick) || gamepadState.IsButtonDown(bttn = Buttons.RightTrigger)
                                    || gamepadState.IsButtonDown(bttn = Buttons.Start) || gamepadState.IsButtonDown(bttn = Buttons.X) || gamepadState.IsButtonDown(bttn = Buttons.Y))
                                {
                                    Constants.SET_BUTTON_BY_INDEX(bindingChoice, bttn);
                                    SetButtonBindingText();
                                    GamepadBindingSave();
                                    bindingChoice = -2;
                                }
                            }
                            else if (bindingChoice == -1){
                                if (Constants.MENU_DOWN(gamepadState, previousGamepadState))
                                    currentButtonUD = LimitInts(currentButtonUD + 1, 0, buttonBindingText.Count - 1);
                                else if (Constants.MENU_UP(gamepadState, previousGamepadState))
                                    currentButtonUD = LimitInts(currentButtonUD - 1, 0, buttonBindingText.Count - 1);

                                if (SingleButtonPress(Constants.SELECT_BUTTON)){
                                    if (currentButtonUD == buttonBindingText.Count - 1){
                                        Constants.SET_DEFAULT_BUTTONS();
                                        SetButtonBindingText();
                                        GamepadBindingSave();
                                    }
                                    else {
                                        bindingChoice = currentButtonUD;
                                        //buttonBindingText[bindingChoice].Color = Color.DarkGoldenrod;
                                        buttonBindingText[bindingChoice].Message = " ";
                                    }
                                }
                                else if (SingleButtonPress(Constants.RETURN_BUTTON)){
                                    mState = MenuState.normal;
                                    currentButtonUD = 0;
                                }
                            }
                        }

                        else {
                            // This if doesn't let anything happen until the Select Button / Return Button are un-pressed so they don't activate
                            // right away after setting a new value to them.
                            if (bindingChoice >= 0){
                                if (kbState.GetPressedKeys().Length > 0){
                                    Keys currentKey = kbState.GetPressedKeys()[0];
                                    // These keys are hardcoded to do something else.
                                    if (currentKey != Keys.Up && currentKey != Keys.Right && currentKey != Keys.Down && currentKey != Keys.Left &&
                                        currentKey != Keys.NumPad0 && currentKey != Keys.NumPad1 && currentKey != Keys.NumPad2 && currentKey != Keys.NumPad3 && currentKey != Keys.NumPad4 &&
                                        currentKey != Keys.NumPad5 && currentKey != Keys.NumPad6 && currentKey != Keys.NumPad7 && currentKey != Keys.NumPad8 && currentKey != Keys.NumPad9)
                                    {
                                        Constants.SET_KEY_BY_INDEX(bindingChoice, currentKey);
                                        SetKeyBindingText();
                                        KeyBoardBindingSave();
                                        bindingChoice = -1;
                                    }
                                }

                                if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed){
                                    bindingChoice = -1;
                                    SetKeyBindingText();
                                }
                            }
                            else if (bindingChoice == -1){
                                if (Constants.MENU_DOWN(gamepadState, previousGamepadState))
                                    currentButtonUD = LimitInts(currentButtonUD + 1, 0, buttonBindingText.Count - 1);
                                else if (Constants.MENU_UP(gamepadState, previousGamepadState))
                                    currentButtonUD = LimitInts(currentButtonUD - 1, 0, buttonBindingText.Count - 1);

                                if (SingleMouseClick()){
                                    for (i = 0; i < keyBindingText.Count; i++){
                                        if (keyBindingText[i].Rectangle.Intersects(new Rectangle((int)mouseLoc.X, (int)mouseLoc.Y, 1, 1))){
                                            if (i == keyBindingText.Count - 1){
                                                Constants.SET_DEFAULT_KEYS();
                                                SetKeyBindingText();
                                                KeyBoardBindingSave();
                                            }
                                            else{
                                                bindingChoice = i;
                                                keyBindingText[i].Color = Color.DarkGoldenrod;
                                                keyBindingText[i].Message = "Press a key to bind it.";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (CheckIfClicked(buttons["Instructions"][buttons["Instructions"].Length - 1])){
                            mState = MenuState.normal;
                        }
                    }
                    #endregion Instructions

                    #region Options
                    if (mState == MenuState.options)
                    {
                        //Difficulty is read as follows:
                        //0 easy, 1 normal, 2 hard, 3 nightmare

                        #region gamePad/options interactions
                        if (gamePadConnected){
                            if (Constants.MENU_DOWN(gamepadState, previousGamepadState))
                                currentButtonUD = LimitInts(currentButtonUD + 1, 0, buttons["Options"].Length - 1);
                            else if (Constants.MENU_UP(gamepadState, previousGamepadState))
                                currentButtonUD = LimitInts(currentButtonUD - 1, 0, buttons["Options"].Length - 1);

                            for (i = 0; i < buttons["Options"].Length; i++){
                                if (currentButtonUD == i && SingleButtonPress(Constants.SELECT_BUTTON)){
                                    difficulty = i;
                                    mState = MenuState.normal;
                                    player1.OuterBounds = new Rectangle(player1.rectangleX - Constants.PLAYER_OUTER_BOUNDS * ((2 + i) / 2), player1.rectangleY - Constants.PLAYER_OUTER_BOUNDS * ((2 + i) / 2), player1.Width + (Constants.PLAYER_OUTER_BOUNDS * 2 * ((2 + i) / 2)), player1.Height + (Constants.PLAYER_OUTER_BOUNDS * 2 * ((2 + i) / 2)));
                                    currentButtonUD = 0;
                                }
                            }
                        }
                        #endregion gamePad/options interactions

                        else {
                            if(mouseState.LeftButton == ButtonState.Pressed){
                                if (CheckIfClicked(buttons["Options"][0])){
                                    graphics.PreferredBackBufferWidth = 1280;
                                    graphics.PreferredBackBufferHeight = 720;
                                    graphics.ApplyChanges();
                                }
                                if (CheckIfClicked(buttons["Options"][1])){
                                    graphics.PreferredBackBufferWidth = 1920;
                                    graphics.PreferredBackBufferHeight = 1080;
                                    graphics.ApplyChanges();
                                }
                                if (CheckIfClicked(buttons["Options"][2])){
                                    if (graphics.IsFullScreen){
                                        graphics.IsFullScreen = false;
                                        graphics.ApplyChanges();
                                    }
                                    else {
                                        graphics.IsFullScreen = true;
                                        graphics.ApplyChanges();
                                    }
                                }
                                if (CheckIfClicked(buttons["Options"][3])){
                                    soundEffectsOn = !soundEffectsOn;
                                }
                                if (CheckIfClicked(buttons["Options"][4])){
                                    musicOn = !musicOn;
                                }
                                if (CheckIfClicked(buttons["Options"][buttons["Options"].Length - 1])){
                                    mState = MenuState.normal;
                                }
                            }
                        }

                        if (SingleButtonPress(Constants.RETURN_BUTTON))
                            mState = MenuState.normal;
                    }
                    #endregion Options

                    #region Credits
                    if (mState == MenuState.credits){
                        //change to actual button object?
                        if (SingleButtonPress(Constants.RETURN_BUTTON)){
                            mState = MenuState.normal;
                            currentButtonUD = 0;
                        }
                        if (mouseState.LeftButton == ButtonState.Pressed && CheckIfClicked(buttons["Credits"][1])){
                            mState = MenuState.normal;
                        }
                    }
                    #endregion Credits

                    break;
                #endregion Gamestate.Options
                    
                #region GameState.Game
                case GameState.game:
                    //save previous position
                    player1.PrevX = player1.rectangleX;
                    player1.PrevY = player1.rectangleY;

                    //Speed reduced if stamina is gone
                    if (stats["stamina"] < 15)
                        player1.Speed = player1.SlowSpeed;
                    else
                        player1.Speed = regSpeed;

                    #region Controls
                    if (!gamePadConnected){
                        if (Constants.MOVE_UP(kbState))
                            player1.rectangleY -= player1.Speed;
                        if (Constants.MOVE_LEFT(kbState))
                            player1.rectangleX -= player1.Speed;
                        if (Constants.MOVE_DOWN(kbState))
                            player1.rectangleY += player1.Speed;
                        if (Constants.MOVE_RIGHT(kbState))
                            player1.rectangleX += player1.Speed;

                        //Key support and pause
                        NumberKeys();
                    }
                    else {
                        // Goes Up
                        if (gamepadState.ThumbSticks.Left.Y == 1.0f)
                            player1.rectangleY -= player1.Speed;
                        // Goes Right
                        else if (gamepadState.ThumbSticks.Left.X == 1.0f)
                            player1.rectangleX += player1.Speed;
                        // Goes Down
                        else if (gamepadState.ThumbSticks.Left.Y == -1.0f)
                            player1.rectangleY += player1.Speed;
                        // Goes Left
                        else if (gamepadState.ThumbSticks.Left.X == -1.0f)
                            player1.rectangleX -= player1.Speed;
                        // Goes Up-Right
                        else if (Math.Sign(gamepadState.ThumbSticks.Left.Y) == 1 && Math.Sign(gamepadState.ThumbSticks.Left.X) == 1){
                            player1.rectangleY -= player1.Speed;
                            player1.rectangleX += player1.Speed;
                        }
                        // Goes Down-Right
                        else if (Math.Sign(gamepadState.ThumbSticks.Left.Y) == -1 && Math.Sign(gamepadState.ThumbSticks.Left.X) == 1){
                            player1.rectangleY += player1.Speed;
                            player1.rectangleX += player1.Speed;
                        }
                        // Goes Down-Left
                        else if (Math.Sign(gamepadState.ThumbSticks.Left.Y) == -1 && Math.Sign(gamepadState.ThumbSticks.Left.X) == -1){
                            player1.rectangleY += player1.Speed;
                            player1.rectangleX -= player1.Speed;
                        }
                        // Goes Up-Left
                        else if (Math.Sign(gamepadState.ThumbSticks.Left.Y) == 1 && Math.Sign(gamepadState.ThumbSticks.Left.X) == -1){
                            player1.rectangleY -= player1.Speed;
                            player1.rectangleX -= player1.Speed;
                        }

                        //Button support
                        ButtonStuff();
                    }

                    player1.OuterBounds = player1.Rectangle;

                    if (!gamePadConnected){
                        scrollCheck = mouseState.ScrollWheelValue;

                        //0 Fists, 1 Pistol, 2 Lead Pipe, 3 Shotgun, 4 Chainsaw, 5 Bullpup, 6 Grenades, 7 Rail gun, 8 Rocket Launcher, 9 Flamethrower
                        if (scrollCheck > prevScrollCheck && !player1.CanMoveBarricade){
                            prevWeapon = player1.CurrentItem;
                            player1.CurrentItem--;
                            chainsawPlay.Stop();
                            if (player1.CurrentItem > maxItem - 1)
                                player1.CurrentItem = 0;
                            while(true){
                                if (player1.CurrentItem < 0)
                                    player1.CurrentItem = maxItem - 2;
                                if (player1.CurrentItem <= 9){
                                    if (!ownedWeapons[player1.CurrentItem]){
                                        player1.CurrentItem--;
                                        if (player1.CurrentItem < 0)
                                            player1.CurrentItem = maxItem - 2;
                                    }
                                    else { break; }
                                }
                                else { break; }
                            }

                            // Cycles through the weapons, previous weapon is an un-owned weapon.
                            player1.PreviousItem = prevWeapon;
                            prevScrollCheck = scrollCheck;
                        }
                        //scroll wheel changes weapons
                        else if (scrollCheck < prevScrollCheck && !player1.CanMoveBarricade){
                            prevWeapon = player1.CurrentItem;
                            player1.CurrentItem++;
                            chainsawPlay.Stop();
                            if (player1.CurrentItem > maxItem - 2)
                                player1.CurrentItem = 0;
                            while (true){
                                if (player1.CurrentItem <= 9){
                                    if (!ownedWeapons[player1.CurrentItem]){
                                        player1.CurrentItem++;
                                        if (player1.CurrentItem > maxItem - 2)
                                            player1.CurrentItem = 0;
                                    }
                                    else { break; }
                                }
                                else { break; }
                            }

                            // Cycles through the weapons, previous weapon is an un-owned weapon.
                            player1.PreviousItem = prevWeapon;
                            prevScrollCheck = scrollCheck;
                        }

                        //get angle and direction for firing weapons
                        angle = (float)Math.Atan2((float)mouseState.Y - player1.Center.Y, (float)mouseState.X - player1.Center.X);
                    }

                    else {
                        if (SingleButtonPress(Constants.SWITCH_BACK)){
                            prevWeapon = player1.CurrentItem;
                            player1.CurrentItem--;
                            chainsawPlay.Stop();
                            if (player1.CurrentItem > maxItem - 1)
                                player1.CurrentItem = 0;
                            while(true) {
                                if (player1.CurrentItem < 0)
                                    player1.CurrentItem = maxItem - 2;
                                if (player1.CurrentItem <= 9) {
                                    if (!ownedWeapons[player1.CurrentItem]) {
                                        player1.CurrentItem--;
                                        if (player1.CurrentItem < 0)
                                            player1.CurrentItem = maxItem - 2;
                                    }
                                    else { break; }
                                }
                                else { break; }
                            }

                            // Cycles through the weapons, previous weapon is an un-owned weapon.
                            player1.PreviousItem = prevWeapon;
                        }
                        else if (SingleButtonPress(Constants.SWITCH_FORWARD)){
                            prevWeapon = player1.CurrentItem;
                            player1.CurrentItem++;
                            chainsawPlay.Stop();
                            if (player1.CurrentItem > maxItem - 2)
                                player1.CurrentItem = 0;
                            while (true){
                                if (player1.CurrentItem <= 9){
                                    if (!ownedWeapons[player1.CurrentItem]){
                                        player1.CurrentItem++;
                                        if (player1.CurrentItem > maxItem - 2)
                                            player1.CurrentItem = 0;
                                    }
                                    else { break; }
                                }
                                else { break; }
                            }

                            // Cycles through the weapons, previous weapon is an un-owned weapon.
                            player1.PreviousItem = prevWeapon;
                        }
                        else if (SingleButtonPress(Constants.SWITCH_LEFT)){
                            while (true){
                                if (player1.CurrentItem >= 0 && player1.CurrentItem <= 4)
                                    player1.CurrentItem = LimitInts(player1.CurrentItem - 1, 0, 4);
                                else if (player1.CurrentItem >= 5 && player1.CurrentItem <= 9)
                                    player1.CurrentItem = LimitInts(player1.CurrentItem - 1, 5, 9);
                                else if (player1.CurrentItem == 10)
                                    player1.CurrentItem = 4;
                                else
                                    player1.CurrentItem = 9;

                                if (ownedWeapons[player1.CurrentItem])
                                    break;
                            }
                        }
                        else if (SingleButtonPress(Constants.SWITCH_RIGHT)){
                            while (true){
                                if (player1.CurrentItem >= 0 && player1.CurrentItem <= 4)
                                    player1.CurrentItem = LimitInts(player1.CurrentItem + 1, 0, 4);
                                else if (player1.CurrentItem >= 5 && player1.CurrentItem <= 9)
                                    player1.CurrentItem = LimitInts(player1.CurrentItem + 1, 5, 9);
                                else if (player1.CurrentItem == 10)
                                    player1.CurrentItem = 0;
                                else
                                    player1.CurrentItem = 0;

                                if (ownedWeapons[player1.CurrentItem])
                                    break;
                            }
                        }
                        else if (SingleButtonPress(Constants.SWITCH_UP)){
                            if (ownedWeapons[LimitInts(player1.CurrentItem - 5, 0, 9)])
                                player1.CurrentItem = LimitInts(player1.CurrentItem - 5, 0, 9);
                        }
                        else if (SingleButtonPress(Constants.SWITCH_DOWN)){
                            if (ownedWeapons[LimitInts(player1.CurrentItem + 5, 0, 9)])
                                player1.CurrentItem = LimitInts(player1.CurrentItem + 5, 0, 9);
                        }
                        //get angle and direction for firing weapons
                        if (gamepadState.ThumbSticks.Right.X != 0 || gamepadState.ThumbSticks.Right.Y != 0)
                            angle = (float)Math.Atan2(gamepadState.ThumbSticks.Right.Y * -1.0, gamepadState.ThumbSticks.Right.X);
                    }
                    #endregion Controls

                    melee.IsActive = false;
                    player1.IsAttacking = false;
                    //updates locations
                    playerLoc = new Vector2(player1.rectangleX, player1.rectangleY);
                    player1.BarricadeStuff(stats, barricades, framesElapsed);

                    //0 Fists, 1 Pistol, 2 Lead Pipe, 3 Shotgun, 4 Chainsaw, 5 Bullpup, 6 Grenades, 7 Rail gun, 8 Rocket Launcher, 9 Flamethrower
                    //firing rates
                    if (player1.CurrentItem == 1)
                        gun.FireRate = 300;
                    else if (player1.CurrentItem == 3)
                        gun.FireRate = 450;
                    else if (player1.CurrentItem == 7 || player1.CurrentItem == 8)
                        gun.FireRate = 2000;
                    else if (player1.CurrentItem == 6)
                        gun.FireRate = 1000;
                    else
                        gun.FireRate = 200;

                    #region Melee/Ranged Attacks
                    //respective attacks for melee and ranged weapons
                    if ((mouseState.LeftButton == ButtonState.Pressed && mouseLoc.X > Constants.PLAY_AREA_LEFT && mouseLoc.X < Constants.PLAY_AREA_RIGHT && mouseLoc.Y > Constants.PLAY_AREA_TOP &&
                        mouseLoc.Y < Constants.PLAY_AREA_BOTTOM) || (gamePadConnected && (gamepadState.IsButtonDown(Constants.ATTACK_BUTTON1) || gamepadState.IsButtonDown(Constants.ATTACK_BUTTON2)))){
                        if ((player1.CurrentItem == 0 || player1.CurrentItem == 2 || player1.CurrentItem == 4) && player1.CanAttack){
                            if (player1.CurrentItem != 4){
                                if (stats["stamina"] > 0){
                                    player1.CanAttack = false;

                                    //fists
                                    if (player1.CurrentItem == 0){
                                        melee.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["strength"] * .10);
                                        melee.Attack(player1.Center, angle, 0);
                                        player1.IsAttacking = true;
                                    }
                                    //pipe
                                    else if (player1.CurrentItem == 2){
                                        melee.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["strength"] * .10);
                                        melee.Attack(player1.Center, angle, 0);
                                        player1.IsAttacking = true;
                                    }
                                    //Melee related stats 
                                    if (framesElapsed % Constants.LOOPS_TILL_STAMINA_USAGE == 0)
                                        stats["stamina"] -= Constants.STAMINA_DECREMENT_AMOUNT;
                                }
                            }
                            else {
                                if (ammoLeft > 0){
                                    if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                                        ammoLeft -= 5;
                                    player1.CanAttack = false;
                                    melee.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["strength"] * .10);
                                    melee.Attack(player1.Center, angle, 10);
                                    if (soundEffectsOn) { chainsawPlay.Play(); }
                                    if (framesElapsed % 10 == 0){
                                        strengthNum++;
                                        ammoLeft--;
                                    }
                                }
                            }
                        }

                        if (ammoLeft > 0){
                            if (player1.CurrentItem == 1 || player1.CurrentItem == 3 || player1.CurrentItem == 5 || player1.CurrentItem == 6 || player1.CurrentItem == 7 || player1.CurrentItem == 8 || player1.CurrentItem == 9){
                                if (stopWatch.ElapsedMilliseconds > gun.FireRate){
                                    fired = true;

                                    if (player1.CurrentItem == 3){
                                        if (soundEffectsOn) { shotgunShot.Play(); }
                                        gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                        fiveBlts = new List<Ammo>();
                                        foreach (Ammo blt in multipleBullets){
                                            if (!blt.IsActive && ammoLeft >= 5) { fiveBlts.Add(blt); }
                                            if (fiveBlts.Count >= 5) { break; }
                                        }
                                        if (fiveBlts.Count >= 5){
                                            for (int j = 0; j < 5; j++){
                                                gun.Fire(fiveBlts[j], player1.Center, angle, .2f, 34);
                                                fiveBlts[j].Direction += (j - 2) * .2f;
                                                tree.AddObject(fiveBlts[j]);
                                            }
                                            stopWatch = new Stopwatch();
                                            stopWatch.Start();
                                            proficiencyNum++;
                                            ammoLeft -= 3;
                                        }
                                    }
                                    else {
                                        for (i = 0; i < multipleBullets.Count; i++){
                                            if (!multipleBullets[i].IsActive){
                                                //Flamethrower
                                                if (player1.CurrentItem == 9){
                                                    // Check end of weapon code, flame thrower fires continueously when on, if flameON.
                                                    if (soundEffectsOn) { flameThrower.Play(); }
                                                    gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                                    if (ammoLeft >= 10) // makes it so that if there is enough ammo flamethrower can be fired
                                                        flameON = true;
                                                }
                                                //Grenade
                                                else if (player1.CurrentItem == 6){
                                                    if (ammoLeft >= 20){
                                                        Ammo gr = multipleBullets[i];
                                                        multipleBullets[i] = new Grenade((int)player1.Center.X, (int)player1.Center.Y, gr.Width, gr.Height, gr.Quad, itemTextures, spriteBatch);
                                                        //grenade box
                                                        ((Grenade)multipleBullets[i]).Shoot(angle, .1f, 10, GHitSpot.Center);
                                                        tree.AddObject(multipleBullets[i]);
                                                        ammoLeft -= 19;
                                                        proficiencyNum++;
                                                    }
                                                    else{
                                                        ammoLeft++;
                                                        fired = false;
                                                    }
                                                }
                                                else {
                                                    //Pistol
                                                    if (player1.CurrentItem == 1){
                                                        if (soundEffectsOn) { pistolShot.Play(); }
                                                        gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                                    }
                                                    //Bullpup Machine Gun
                                                    else if (player1.CurrentItem == 5){
                                                        if (soundEffectsOn) { arFire.Play(); }
                                                        gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                                    }
                                                    //Rail Gun
                                                    else if (player1.CurrentItem == 7){
                                                        if (ammoLeft >= 10){
                                                            if (soundEffectsOn) { railGun.Play(); }
                                                            gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                                            ammoLeft -= 9;
                                                        }
                                                        else {
                                                            ammoLeft++;
                                                            fired = false;
                                                        }
                                                    }
                                                    //Rocket Launcher
                                                    else if (player1.CurrentItem == 8){
                                                        if (ammoLeft >= 20){
                                                            gun.WeaponDamage = Constants.weaponsDamage[player1.CurrentItem] + (int)((double)stats["weapons"] * .01);
                                                            multipleBullets[i].AmmoType = Ammo.Type.Rocket; // Property takes care of changes.
                                                            ammoLeft -= 19;
                                                        }
                                                        else {
                                                            ammoLeft++;
                                                            fired = false;
                                                        }
                                                    }

                                                    if (fired){
                                                        gun.Fire(multipleBullets[i], player1.Center, angle, 0f, 32);
                                                        tree.AddObject(multipleBullets[i]);
                                                    }
                                                }
                                                // checks to see if the flame thrower is being used, if it is subtracts ammo from it
                                                if (!flameON && player1.CurrentItem != 9) { ammoLeft--; }
                                                else if (flameON) { ammoLeft -= 10; }

                                                stopWatch = new Stopwatch();
                                                stopWatch.Start();
                                                //Weapon Stats
                                                proficiencyNum++;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (MouseClick()){
                            if (soundEffectsOn) { gunEmpty.Play(); }
                            ammoLeft = 0;
                        }

                        #region Food
                        if (player1.CurrentItem == 10)
                        {
                            if (healthItems > 0 && player1.Health != Constants.PLAYER_HEALTH)
                            {
                                if (gamePadConnected)
                                {
                                    player1.Health += Constants.FOOD_HEAL;
                                    healthItems--;
                                }
                                else if (!gamePadConnected && MouseClick())
                                {
                                    player1.Health += Constants.FOOD_HEAL;
                                    healthItems--;
                                }
                            }
                            player1.CurrentItem = player1.PreviousItem;
                        }
                        #endregion Food

                        }

                    else if (flameON)
                        flameON = false;
                    if (flameON){
                        if (ammoLeft > 0){
                            for (i = 0; i < flameExtension.Count; i++)
                                flameExtension[i].Attack(player1.Center, angle, (i * 20));
                        }
                        else {
                            ammoLeft = 0;
                            flameON = false;
                        }
                    }
                    if (flameON && player1.CurrentItem != 9)
                        flameON = false;

                    grenadeActive = false;
                    for (i = 0; i < multipleBullets.Count; i++){
                        if (multipleBullets[i] is Grenade && multipleBullets[i].IsActive) { grenadeActive = true; break; }
                    }

                    // If grenade is exploding or in mid-air, GHitSpot does not move (this is so explosion and endpoint doesn't move)
                    // Since GHitspot is not really a Melee Object, weapon damage is used to tell animation time. If -1 animation is not playing.
                    if (player1.CurrentItem == 6 && !grenadeActive && GHitSpot.WeaponDamage == -1)
                        GHitSpot.Attack(player1.Center, angle, 175);

                    if (player1.CurrentItem == 5){
                        laserSightAim.Shoot(player1.Center, 0, angle, 0f, 41 + 300);
                    }
                    #endregion Melee/Ranged Attacks

                    #region Stats
                    //Stat related values
                    // Stamina
                    if (stats["stamina"] < Constants.MAX_STAMINA && !player1.IsAttacking){
                        if (!player1.HoldingBarricade){
                            if (framesElapsed % 2 == 0)
                                stats["stamina"]++;
                        }
                    }

                    // Strength from melee
                    if (strengthNum > 300 && stats["strength"] <= 99){
                        stats["strength"]++;
                        strengthNum = 0;
                    }

                    // Proficiency from guns
                    if (proficiencyNum > 100 && stats["weapons"] <= 99){
                        stats["weapons"]++;
                        proficiencyNum = 0;
                    }
                    #endregion Stats

                    #region Bullet Check
                    // Bullet check
                    for (i = 0; i < multipleBullets.Count; i++){
                        if (multipleBullets[i].IsActive)
                            multipleBullets[i].Move();
                        if (!(multipleBullets[i] is Grenade) && multipleBullets[i].IsActive && (multipleBullets[i].rectangleX <= Constants.PLAY_AREA_LEFT - Constants.BULLET_SPEED || multipleBullets[i].rectangleY <= Constants.PLAY_AREA_TOP - Constants.BULLET_SPEED ||
                            multipleBullets[i].rectangleX >= Constants.PLAY_AREA_RIGHT + Constants.BULLET_SPEED || multipleBullets[i].rectangleY >= Constants.PLAY_AREA_BOTTOM + Constants.BULLET_SPEED))
                        {
                            multipleBullets[i].IsActive = false;
                        }
                        if (multipleBullets[i].IsActive && multipleBullets[i] is Grenade){
                            if (((Grenade)multipleBullets[i]).CheckIfDone()){
                                startExplosion = true;
                                multipleBullets[i] = new Ammo(multipleBullets[i].rectangleX, multipleBullets[i].rectangleY, 1, 1, tree, Ammo.Type.Bullet, itemTextures, spriteBatch);
                                multipleBullets[i].AmmoType = Ammo.Type.Bullet;
                            }
                        }
                    }
                    #endregion Bullet Check

                    #region Enemy Collisions
                    // Checks to see if anything is colliding with an enemy, and acts accordingly
                    for (i = 0; i < enemies.Count; i++){
                        if (enemies[i].Alive){
                            //objects = tree.GetAllObjects();
                            objects = enemies[i].Quad.GetAllObjects();

                            foreach (GameObject obj in objects){
                                if (enemies[i].CheckCollision(obj)){
                                    //check player and enemy collision
                                    if (obj is Player){
                                        enemies[i].AttackPlayer(player1);
                                    }

                                    //check ammo collision with zombie
                                    else if (obj is Ammo){
                                        if (((Ammo)obj).IsActive && !(((Ammo)obj) is Grenade)){
                                            if (player1.CurrentItem == 8){
                                                if (enemies[i].Intersects(new Rectangle(((Ammo)obj).rectangleX, ((Ammo)obj).rectangleY, 100, 100))){
                                                    if (((Ammo)obj).AmmoType == Ammo.Type.Rocket){
                                                        startExplosion = true;
                                                        GHitSpot.Center = obj.Center;
                                                        ((Ammo)obj).IsActive = false;
                                                    }
                                                    else {
                                                        enemies[i].TakeDamage(((Ammo)obj).Damage, false);
                                                        //if timer up
                                                        ((Ammo)obj).IsActive = false;
                                                        tree.RemoveObject(((Ammo)obj));
                                                    }
                                                }
                                            }
                                            else {
                                                if (player1.CurrentItem == 7){
                                                    enemies[i].TakeDamage(((Ammo)obj).Damage, false);
                                                }
                                                else {
                                                    enemies[i].TakeDamage(((Ammo)obj).Damage, false);
                                                    //1 shot hits one zombie
                                                    ((Ammo)obj).IsActive = false;
                                                    tree.RemoveObject(((Ammo)obj));
                                                }
                                            }

                                            if (!enemies[i].Alive){
                                                //add to score
                                                if (enemies[i].Type == 0)
                                                    score += (Constants.NORMAL_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 1)
                                                    score += (Constants.FLAMING_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 2)
                                                    score += (Constants.ZOMBIE_DOG_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 3)
                                                    score += (Constants.DOCTOR_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 4)
                                                    score += (Constants.SWAT_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier)) ;
                                                
                                                //decrement amount of enemies remaining
                                                numEnemiesLeft--;
                                                //play death sound effect
                                                if (soundEffectsOn) { zombieDeath.Play(); }
                                                //drop item, if any
                                                drop = enemies[i].DropItem(itemTextures, ownedWeapons, ref waveSinceDrop, difficulty);
                                                if (drop != null){
                                                    items.Add(drop);
                                                    tree.AddObject(drop);
                                                }
                                            }
                                        }
                                    }

                                    else if (obj is Melee){
                                        if (((player1.CurrentItem == 0 || player1.CurrentItem == 2 || player1.CurrentItem == 4) && melee.IsActive) || flameON){
                                            if (!flameON && melee.IsActive){
                                                if (((Melee)obj) == melee){
                                                    enemies[i].TakeDamage(((Melee)obj).WeaponDamage, true);
                                                    strengthNum++;
                                                }
                                            }
                                            else if (flameON){
                                                enemies[i].Killed();
                                            }

                                            if (!enemies[i].Alive){
                                                //add to score
                                                if (enemies[i].Type == 0)
                                                    score += (Constants.NORMAL_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 1)
                                                    score += (Constants.FLAMING_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 2)
                                                    score += (Constants.ZOMBIE_DOG_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 3)
                                                    score += (Constants.DOCTOR_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));
                                                else if (enemies[i].Type == 4)
                                                    score += (Constants.SWAT_ZOMBIE_SCORE * (int)Math.Ceiling(zombieMultiplier));

                                                //decrement amount of enemies remaining
                                                numEnemiesLeft--;
                                                //play death sound effect
                                                if (soundEffectsOn) { zombieDeath.Play(); }
                                                //drop item, if any
                                                drop = enemies[i].DropItem(itemTextures, ownedWeapons, ref waveSinceDrop, difficulty);
                                                if (drop != null){
                                                    items.Add(drop);
                                                    tree.AddObject(drop);
                                                }
                                            }
                                        }
                                    }

                                    //keep enemies from overlapping
                                    else if (obj is Enemy){
                                        if (((Enemy)obj).Alive && ((Enemy)obj) != enemies[i]){
                                            enemies[i].GoBack(1);
                                            enemies[i].Direction -= (float)Math.PI/2;
                                        }
                                    }

                                    else if (obj is Wall){
                                        if (((Wall)obj).IsActive){
                                            ((Wall)obj).Attack();
                                            enemies[i].GoBack(2);
                                            enemies[i].Direction -= (float)Math.PI/2;
                                        }
                                    }

                                    else if (obj is Barricade){
                                        if (((Barricade)obj).IsActive){
                                            if (enemies[i].WithinRange || enemies[i].Enraged){
                                                ((Barricade)obj).Attack();
                                                enemies[i].GoBack(2);
                                                enemies[i].Direction -= (float)Math.PI/2;
                                            }
                                            else {
                                                ((Barricade)obj).Attack();
                                                enemies[i].GoBack(2);
                                            }
                                        }
                                    }
                                }
                            }
                            //if (currentLevel != 1){
                                enemies[i].Move(player1, gameTime.TotalGameTime.Milliseconds);
                            //}
                            if (enemies[i].Quad != tree.GetContainingQuad(enemies[i])){
                                tree.RemoveObject(enemies[i]);
                                tree.AddObject(enemies[i]);
                            }
                        }
                    }
                    #endregion Enemy Collisions

                    #region Player Collisions
                    //checks to see if a player collides with objects
                    objects = tree.GetAllObjects();

                    foreach (GameObject obj in objects){
                        if (player1.Intersects(obj)){
                            //keeps the player from going under enemies
                            if (obj is Enemy && ((Enemy)obj).Alive){
                                player1.rectangleX = player1.PrevX;
                                player1.rectangleY = player1.PrevY;
                            }

                            if (obj is Barricade && ((Barricade)obj).IsActive){
                                if (((Barricade)obj).IsActive && ((Barricade)obj).BeingHeld){
                                    ((Barricade)obj).Move(player1, angle);
                                    player1.CurrentItem = 11;
                                }
                                else if (player1.UnderBarricade && !player1.HoldingBarricade && ((Barricade)obj).Under){
                                    //lets you walk out from under barricade after dropping it
                                }
                                else {
                                    //else normal collision
                                    if (((Barricade)obj).CheckCollision(player1)){
                                        Rectangle tempPos = player1.Rectangle;
                                        player1.rectangleX = player1.PrevX;
                                        if (((Barricade)obj).CheckCollision(player1)){
                                            player1.Rectangle = tempPos;
                                            player1.rectangleY = player1.PrevY;
                                        }
                                    }
                                }
                                // If a barricade can be picked up, this method "allows" it.
                                ((Barricade)obj).CheckStuff(player1, kbState, gamepadState, this);
                            }

                            else if (obj is Wall){
                                if (((Wall)obj).CheckCollision(player1)){
                                    Rectangle tempPos = player1.Rectangle;
                                    player1.rectangleX = player1.PrevX;
                                    if (((Wall)obj).CheckCollision(player1)){
                                        player1.Rectangle = tempPos;
                                        player1.rectangleY = player1.PrevY;
                                    }
                                }
                            }

                            else if (obj is Item && !(obj is Weapon) && !(obj is Ammo)){
                                if (((Item)obj).CheckCollision(player1)){
                                    //remove item, add to inventory
                                    if (((Item)obj).ItemType == -1)
                                        healthItems += 1;
                                    else if (((Item)obj).ItemType == -2)
                                        ammoLeft += Constants.AMMO_GAIN_FROM_CASE;
                                    else if (((Item)obj).ItemType >= 0){
                                        NewWeapon(((Item)obj).ItemType, gameTime);
                                    }

                                    ((Item)obj).InInventory = true;
                                    ((Item)obj).OnScreen = false;
                                }
                            }
                        }
                    }
                    #endregion Player Collisions

                    #region Explosions
                    // This is a check for rocket collisions, which sets it up for actual explosions.
                    foreach (Ammo blt in multipleBullets){
                        if (blt.AmmoType == Ammo.Type.Rocket && blt.IsActive){
                            foreach (GameObject gmOb in tree.GetAllObjects()){
                                if (gmOb.IsActive){
                                    if (blt.Intersects((GameObject)gmOb) && !(((GameObject)gmOb) is Barricade) && !(((GameObject)gmOb) is Player) && !(((GameObject)gmOb) is Ammo) && !(((GameObject)gmOb) is Melee)){
                                        GHitSpot.Center = blt.Center;
                                        blt.IsActive = false;
                                        startExplosion = true;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }

                    // This starts explosions for grenades and rockets.
                    if(startExplosion){
                        foreach(GameObject gmOb in tree.GetAllObjects()){
                            if(GHitSpot.Intersects((GameObject)gmOb)){
                                if(Vector2.Distance(GHitSpot.Center, ((GameObject)gmOb).Center) <= (GHitSpot.Width / 2)){
                                    if (((GameObject)gmOb) is Enemy){
                                        if (((Enemy)gmOb).IsActive)
                                            ((Enemy)gmOb).Killed();
                                    }
                                    else{
                                        ((GameObject)gmOb).IsActive = false;
                                        if (((GameObject)gmOb) is Obstructions){
                                            ((Obstructions)gmOb).HitsTillBroken = 1;
                                            ((Obstructions)gmOb).Attack();
                                        }
                                    }
                                }
                            }
                        }

                        startExplosion = false;
                        if (soundEffectsOn)
                            explosion.Play();
                        // Since GHitspot is not really a Melee Object, weapon damage is used to tell animation time. If -1 animation is not playing.
                        GHitSpot.WeaponDamage = 0;
                    }
                    #endregion Explosions

                    #region Bullet/Melee Wall Collision
                    // Checks to see if a bullet/melee object has hit a wall
                    for (i = 0; i < multipleBullets.Count; i++){
                        if (multipleBullets[i].IsActive){
                            Ammo testB = multipleBullets[i];
                            testB.Point = new Vector2((float)testB.PrevX, (float)testB.PrevY);
                            tree.AddObject(testB);
                            for (int p = 0; p < Constants.BULLET_SPEED; p += Constants.BULLET_COLLISION_CHECK_SPEED){
                                testB.Point = new Vector2((float)(testB.PrevX + (Math.Cos(testB.Direction) * p)), (float)(testB.PrevY + (Math.Sin(testB.Direction) * p)));
                                tree.RemoveObject(testB);
                                tree.AddObject(testB);

                                objects = testB.Quad.GetAllObjects();
                                if (testB.Quad.Parent != null){
                                    foreach (GameObject obj in testB.Quad.Parent.GameObjects){
                                        // If something in the player's quad's parent intersects the player's quad before adding it to the list to check for collisions
                                        if (obj.Intersects(testB.Quad.Rectangle))
                                            objects.Add(obj);
                                    }
                                }

                                foreach (GameObject go in objects){
                                    if (go is Wall && ((Wall)go).IsActive && testB.Intersects(go)){
                                        if (testB.AmmoType == Ammo.Type.Rocket && testB.IsActive){
                                            GHitSpot.Center = testB.Center;
                                            multipleBullets[i].IsActive = false;
                                            startExplosion = true;
                                            break;
                                        }
                                        else{
                                            ((Wall)go).IsActive = false;{
                                                multipleBullets[i].IsActive = false;
                                                tree.RemoveObject(multipleBullets[i]);
                                            }
                                            tree.RemoveObject(((Wall)go));
                                            tree.RemoveObject(testB);
                                            break;
                                        }
                                    }
                                }
                                if (!multipleBullets[i].IsActive)
                                    break;
                            }
                        }
                    }
                    #endregion Bullet/Melee Wall Collision

                    if (!flameON && melee.IsActive){
                        objects = melee.Quad.GetAllObjects();
                        parent = melee.Quad.Parent;
                        while (parent != null){
                            foreach (GameObject GO in parent.GameObjects){
                                // Check to see if something in the melee's quad's parent intersects the melee's quad before adding it to the list to check for collisions
                                if (GO.Intersects(melee.Quad.Rectangle))
                                    objects.Add(GO);
                            }
                            parent = parent.Parent;
                        }
                        foreach (GameObject GO in objects){
                            if (melee.Intersects(GO)){
                                if (GO is Wall)
                                    ((Wall)GO).Attack();
                            }
                        }
                    }
                    else if (flameON){
                        foreach (Melee mle in flameExtension){
                            objects = mle.Quad.GetAllObjects();
                            parent = mle.Quad.Parent;
                            while (parent != null){
                                foreach (GameObject GO in parent.GameObjects){
                                    // Check to see if something in the melee's quad's parent intersects the melee's quad before adding it to the list to check for collisions
                                    if (GO.Intersects(mle.Quad.Rectangle))
                                        objects.Add(GO);
                                }
                                parent = parent.Parent;
                            }
                            foreach (GameObject GO in objects){
                                if (mle.Intersects(GO)){
                                    if (GO is Wall){
                                        ((Wall)GO).HitsTillBroken = 0;
                                        ((Wall)GO).Attack();
                                    }
                                }
                            }
                        }
                    }

                    #region Reload Quads
                    // Reload quad in QuadTree (if necessary)
                    if (player1.Quad != tree.GetContainingQuad(player1)){
                        tree.RemoveObject(player1);
                        tree.AddObject(player1);
                    }
                    if (melee.Quad != tree.GetContainingQuad(melee)){
                        tree.RemoveObject(melee);
                        tree.AddObject(melee);
                    }
                    foreach (Melee mle in flameExtension){
                        if (mle.Quad != tree.GetContainingQuad(mle)){
                            tree.RemoveObject(mle);
                            tree.AddObject(mle);
                        }
                    }
                    foreach (Enemy enmy in enemies){
                        if (enmy.Alive && enmy.Quad != tree.GetContainingQuad(enmy)){
                            tree.RemoveObject(enmy);
                            tree.AddObject(enmy);
                        }
                    }
                    foreach (Ammo amo in multipleBullets){
                        if (amo.IsActive){
                            if (amo.Quad != tree.GetContainingQuad(amo)){
                                tree.RemoveObject(amo);
                                tree.AddObject(amo);
                            }
                        }
                    }
                    foreach (Item itm in items){
                        if (itm.OnScreen){
                            if (itm.Quad != tree.GetContainingQuad(itm)){
                                tree.RemoveObject(itm);
                                tree.AddObject(itm);
                            }
                        }
                    }
                    foreach (Barricade barr in barricades){
                        if (barr.IsActive){
                            if (barr.Quad != tree.GetContainingQuad(barr)){
                                tree.RemoveObject(barr);
                                tree.AddObject(barr);
                            }
                        }
                    }
                    #endregion Reload Quads

                    if (!player1.Alive){
                        currentState = GameState.gameOver;
                        if (musicOn) { MediaPlayer.Stop(); }
                        chainsawPlay.Stop();
                        if (soundEffectsOn) { deathCry.Play(); }
                    }

                    allDead = true;
                    foreach (Enemy enmy in enemies){
                        if (enmy.Alive)
                            allDead = false;
                    }
                    if (allDead)
                        NextWave();
                    break;
                #endregion GameState.Game

                #region Gamestate.paused
                case GameState.paused:
                    // This if is for when user turns music from off to on
                    if (!musicOn && MediaPlayer.State == MediaState.Playing)
                        MediaPlayer.Stop();

                    if (Constants.MENU_DOWN(gamepadState, previousGamepadState))
                        currentButtonUD = LimitInts(currentButtonUD + 1, 0, 4);
                    else if (Constants.MENU_UP(gamepadState, previousGamepadState))
                        currentButtonUD = LimitInts(currentButtonUD - 1, 0, 4);

                    if (CheckIfClicked(buttons["Pause"][0]) || (currentButtonUD == 0 && SingleButtonPress(Constants.SELECT_BUTTON))){
                        musicOn = !musicOn;
                        MediaPlayer.Play(theme1);
                        StartSave();
                    }

                    if (CheckIfClicked(buttons["Pause"][1]) || (currentButtonUD == 1 && SingleButtonPress(Constants.SELECT_BUTTON))){
                        soundEffectsOn = !soundEffectsOn;
                        StartSave();
                    }

                    if (CheckIfClicked(buttons["Pause"][2]) || (currentButtonUD == 2 && SingleButtonPress(Constants.SELECT_BUTTON))){
                        randomZombieColor = !randomZombieColor;
                        StartSave();
                        if (randomZombieColor){
                            foreach (Enemy enm in enemies){
                                if (enm.Alive)
                                    enm.PickRandColor(0);
                            }
                        }
                        else {
                            foreach (Enemy enm in enemies){
                                if (enm.Alive)
                                    enm.Color = Color.White;
                            }
                        }
                    }

                    if (CheckIfClicked(buttons["Pause"][3]) || (currentButtonUD == 3 && SingleButtonPress(Constants.SELECT_BUTTON))){
                        zombieHats = !zombieHats;
                        StartSave();
                    }

                    if (gamePadConnected){
                        if (SingleButtonPress(Buttons.LeftStick))
                            showOutlines = !showOutlines;
                        if (SingleButtonPress(Buttons.RightStick)){
                            foreach (Enemy nmy in enemies){
                                nmy.Killed();
                                numEnemiesLeft--;
                                drop = nmy.DropItem(itemTextures, ownedWeapons, ref waveSinceDrop, difficulty);
                                if (drop != null){
                                    items.Add(drop);
                                    tree.AddObject(drop);
                                }
                            }
                        }
                    }
                    else {
                        if (CheckIfClicked(new Rectangle((int)(506 * Constants.SCALE_WIDTH), (int)(102 * Constants.SCALE_HEIGHT), (int)(101 * Constants.SCALE_WIDTH), (int)(107 * Constants.SCALE_HEIGHT))))
                            showOutlines = !showOutlines;
                        if (CheckIfClicked(new Rectangle(0, 0, 5, 5))){
                            foreach (Enemy nmy in enemies){
                                nmy.Killed();
                                numEnemiesLeft--;
                                drop = nmy.DropItem(itemTextures, ownedWeapons, ref waveSinceDrop, difficulty);
                                if (drop != null){
                                    items.Add(drop);
                                    tree.AddObject(drop);
                                }
                            }
                        }
                    }

                    if (gamePadConnected){
                        if (SingleButtonPress(Constants.RETURN_BUTTON)){
                            currentState = GameState.game;
                            currentButtonUD = 0;
                        }
                    }
                    else {
                        if (SingleKeyPress(Constants.PAUSE_KEY) || SingleKeyPress(Constants.EXIT_KEY))
                            currentState = GameState.game;
                    }
                    break;
                #endregion GameState.paused

                #region GameState.gameOver
                case GameState.gameOver:
                    //Returns to the menu screen
                    //if (!gamePadConnected && SingleKeyPress(Keys.Space)){
                    //    currentState = GameState.menu;
                    //    mState = MenuState.normal;
                    //}
                    //else if (gamePadConnected && gamepadState.IsButtonDown(Buttons.Start)){
                    //    currentState = GameState.menu;
                     //   mState = MenuState.normal;
                    //}

                    if (CheckIfClicked(buttons["GameOver"][buttons["GameOver"].Length - 1])){
                        currentState = GameState.menu;
                        mState = MenuState.normal;
                    }

                    currentSong = 1;
                    enemies.Clear();
                    break;
                #endregion GameState.gameOver
            }
            base.Update(gameTime);
        }
        #endregion Update

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            switch (currentState)
            {
                #region GameState.menu
                case GameState.menu:
                    switch (mState)
                    {
                        #region MenuState.credits
                        case MenuState.credits:
                            spriteBatch.Draw(creditsImage, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            if (gamePadConnected){
                                if (currentButtonUD == 0)
                                    spriteBatch.Draw(selectionOutline, buttons["Credits"][currentButtonUD], Color.White);
                            }
                            spriteBatch.Draw(creditsButtons, new Vector2(buttons["Credits"][0].X, buttons["Credits"][0].Y), new Rectangle(0, 0, 446, 204), Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                            spriteBatch.Draw(backButton, new Vector2(buttons["Credits"][buttons["Credits"].Length - 1].X, buttons["Credits"][buttons["Credits"].Length - 1].Y), new Rectangle(0, 0, 345, 257), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            break;
                        #endregion MenuState.credits

                        #region MenuState.options
                        case MenuState.options:
                            spriteBatch.Draw(optionsImage, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            if (gamePadConnected){
                                for (i = 0; i < buttons["Options"].Length; i++){
                                    if (currentButtonUD == i)
                                        spriteBatch.Draw(selectionOutline, buttons["Options"][i], Color.White);
                                }
                            }
                            else {
                                spriteBatch.Draw(cursorImage, new Rectangle((int)mouseLoc.X - 16, (int)mouseLoc.Y - 16, 32, 32), Color.White);
                            }

                            //screen resolution settings
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][0].X, buttons["Options"][0].Y), new Rectangle(246, 616, 70, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][1].X, buttons["Options"][1].Y), new Rectangle(432, 620, 70, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][2].X, buttons["Options"][2].Y), new Rectangle(610, 610, 172, 50), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

                            //music, sound
                            if (musicOn){
                                spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][3].X, buttons["Options"][3].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            }
                            else{
                                spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][3].X, buttons["Options"][3].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            }

                            if (soundEffectsOn){
                                spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][4].X, buttons["Options"][4].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            }
                            else{
                                spriteBatch.Draw(optionsButtons, new Vector2(buttons["Options"][4].X, buttons["Options"][4].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                            }

                            spriteBatch.Draw(backButton, new Vector2(buttons["Options"][buttons["Options"].Length - 1].X, buttons["Options"][buttons["Options"].Length - 1].Y), new Rectangle(0, 0, 345, 257), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            break;
                        #endregion MenuState.difficulty

                        #region MenuState.instructions
                        case MenuState.instructions:
                            spriteBatch.Draw(instructionsImage, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            if (gamePadConnected){
                                if (bindingChoice < 0)
                                    spriteBatch.Draw(selectionOutline, buttonBindingText[currentButtonUD].Rectangle, Color.White);
                                for (i = 0; i < buttonBindingText.Count; i++)
                                    buttonBindingText[i].Draw();
                            }
                            else {
                                for (i = 0; i < keyBindingText.Count; i++){
                                    keyBindingText[i].Draw();
                                    if (bindingChoice < 0 && keyBindingText[i].Rectangle.Intersects(new Rectangle((int)mouseLoc.X, (int)mouseLoc.Y, 1, 1)))
                                        spriteBatch.Draw(selectionOutline, keyBindingText[i].Rectangle, Color.White);
                                }
                                spriteBatch.Draw(cursorImage, new Rectangle((int)mouseLoc.X - 16, (int)mouseLoc.Y - 16, 32, 32), Color.White);
                            }
                            spriteBatch.Draw(backButton, new Vector2(buttons["Instructions"][buttons["Instructions"].Length - 1].X, buttons["Instructions"][buttons["Instructions"].Length - 1].Y), new Rectangle(0, 0, 345, 257), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            break;
                        #endregion MenuState.instructions

                        #region MenuState.normal
                        case MenuState.normal:
                            spriteBatch.Draw(menuScreen, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            if (gamePadConnected){
                                if (currentButtonUD == 0)
                                    spriteBatch.Draw(selectionOutline, buttons["Play"][currentButtonUD], Color.White);
                            }
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][0].X, buttons["Play"][0].Y), new Rectangle(100, 334, 400, 270), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][0].X, buttons["Menu"][0].Y), new Rectangle(500, 334, 488, 300), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][1].X, buttons["Menu"][1].Y), new Rectangle(142, 668, 360, 248), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][2].X, buttons["Menu"][2].Y), new Rectangle(522, 660, 290, 260), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            break;
                        #endregion MenuState.normal

                        #region MenuState.newOrLoad
                        case MenuState.newOrLoad:
                            spriteBatch.Draw(menuScreen, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][0].X, buttons["Play"][0].Y), new Rectangle(100, 334, 400, 270), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][0].X, buttons["Menu"][0].Y), new Rectangle(500, 334, 488, 300), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][1].X, buttons["Menu"][1].Y), new Rectangle(142, 668, 360, 248), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][2].X, buttons["Menu"][2].Y), new Rectangle(522, 660, 290, 260), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            //new and load
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][1].X, buttons["Play"][1].Y), new Rectangle(270, 56, 242, 208), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][2].X, buttons["Play"][2].Y), new Rectangle(561, 80, 254, 198), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            if(gamePadConnected && currentButtonUD == 0){
                                if(currentButtonLR == 0)
                                    spriteBatch.Draw(selectionOutline, buttons["Play"][1], Color.White);
                                else if(currentButtonLR == 1)
                                    spriteBatch.Draw(selectionOutline, buttons["Play"][2], Color.White);
                            }

                            break;
                        #endregion MenuState.newOrLoad

                        #region MenuState.saveFile
                        case MenuState.saveFile:
                            spriteBatch.Draw(menuScreen, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][0].X, buttons["Play"][0].Y), new Rectangle(100, 334, 400, 270), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][0].X, buttons["Menu"][0].Y), new Rectangle(500, 334, 488, 300), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][1].X, buttons["Menu"][1].Y), new Rectangle(142, 668, 360, 248), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Menu"][2].X, buttons["Menu"][2].Y), new Rectangle(522, 660, 290, 260), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            //new and load
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][1].X, buttons["Play"][1].Y), new Rectangle(270, 56, 242, 208), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            spriteBatch.Draw(menuButtons, new Vector2(buttons["Play"][2].X, buttons["Play"][2].Y), new Rectangle(561, 80, 254, 198), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                            for (i = 0; i < saveEmpty.Length; i++){
                                saveNum = i + 1;
                                if (saveEmpty[i]){
                                    spriteBatch.Draw(fileImage, new Vector2(buttons["Play"][3 + i].X, buttons["Play"][3 + i].Y), new Rectangle(0, 0, 219, 274), Color.White, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                                }
                                else{
                                    savText = GetSaveLevel(saveNum) + "\n" + GetSaveStr(saveNum) + "\n" + GetSaveProf(saveNum);
                                    spriteBatch.Draw(fileImage, new Vector2(buttons["Play"][3 + i].X, buttons["Play"][3 + i].Y), new Rectangle(438, 0, 219, 274), Color.White, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                                    spriteBatch.DrawString(smallText, savText, new Vector2(buttons["Play"][3 + i].X + ((buttons["Play"][3 + i].Width - smallText.MeasureString(savText).X) / 2), buttons["Play"][3 + i].Y + ((buttons["Play"][3 + i].Height - smallText.MeasureString(savText).Y) / 2)), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                                }
                                
                                if (gamePadConnected && currentButtonUD == 0 && currentButtonLR == i)
                                    spriteBatch.Draw(selectionOutline, buttons["Play"][3 + i], Color.White);
                            }
                            break;
                        #endregion MenuState.saveFile

                        #region MenuState.exit
                        case MenuState.exit:
                            spriteBatch.Draw(menuScreen, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                            //This gives the grayed out look behind exit box
                            spriteBatch.Draw(pausedBack, new Rectangle(0, 0, graphics.PreferredBackBufferWidth - 1, graphics.PreferredBackBufferHeight - 1), Constants.PAUSE_OUTER_GRAY);
                            width = 262; height = 112;
                            rect = new Rectangle(graphics.PreferredBackBufferWidth / 2 - width / 2, graphics.PreferredBackBufferHeight / 2 - height / 2, width, height);
                            //draw new yes/no here
                            //spriteBatch.Draw(yesNoTexture, rect, Color.White);

                            if(gamePadConnected){
                                if (SingleButtonPress(Constants.SELECT_BUTTON))
                                    Environment.Exit(0);
                                else if (SingleButtonPress(Constants.RETURN_BUTTON))
                                    mState = MenuState.normal;
                            }
                            else{
                                if (CheckIfClicked(new Rectangle(rect.X + 6, rect.Y + 5, rect.Width / 2 - 10, rect.Height - 10)))
                                    Environment.Exit(0);
                                else if (CheckIfClicked(new Rectangle(rect.X + rect.Width / 2 + 0, rect.Y + 5, rect.Width / 2 - 9, rect.Height - 10)))
                                    mState = MenuState.normal;
                                spriteBatch.Draw(cursorImage, new Rectangle((int)mouseLoc.X - 16, (int)mouseLoc.Y - 16, 32, 32), Color.White);
                            }
                            break;
                        #endregion MenuState.exit
                    }

                    if (mState == MenuState.normal || mState == MenuState.newOrLoad || mState == MenuState.saveFile)
                    {
                        if(gamePadConnected){
                            if (currentButtonUD > 0){
                                spriteBatch.Draw(selectionOutline, buttons["Menu"][currentButtonUD - 1], Color.White);
                            }
                        }
                        else {
                            spriteBatch.Draw(cursorImage, new Rectangle((int)mouseLoc.X - 16, (int)mouseLoc.Y - 16, 32, 32), Color.White);
                        }
                    }
                    break;
                #endregion GameState.menu

                #region GameState.loading
                case GameState.loading:
                    // The reason that the zombie spawn/quad-tree loading methods are here in an if is so that it draws the loading menu before it goes into a really long loop.
                    spriteBatch.DrawString(gameText, "Level " + Constants.levels[currentLevel - 1] + ", Wave " + currentWave, new Vector2(Constants.TOTAL_WIDTH / 2, Constants.TOTAL_HEIGHT / 3), Color.Red);
                    spriteBatch.Draw(loading, new Rectangle(0, 0, graphics.PreferredBackBufferWidth - 1, graphics.PreferredBackBufferHeight - 1), Color.White);

                    if (loadWatch.ElapsedMilliseconds > 500){
                        SpawnZombies();
                        SetupQuadTree();

                        //spawn check for overlapping walls/barricades on the player
                        foreach (Wall wall in walls){
                            if (wall.Intersects(player1))
                                wall.IsActive = false;
                        }
                        foreach (Barricade barricade in barricades){
                            if (barricade.Intersects(player1))
                                barricade.IsActive = false;
                        }
                        currentState = GameState.game;
                        loadWatch.Stop();
                    }
                    break;
                #endregion GameState.loading

                #region The rest
                //if GameState is paused, stuff is still drawn in background
                case GameState.game: case GameState.paused: case GameState.goToMenu:
                    spriteBatch.Draw(wallImage, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.FromNonPremultiplied(45, 0, 0, 255));
                    spriteBatch.Draw(gameScreen, Constants.PLAY_AREA, Color.White);

                    //draws enemies/blood
                    for (i = 0; i < enemies.Count; i++){
                        enemies[i].Draw(player1, gameTime/*, zombieHats*/);
                    }

                    foreach (Item itm in items)
                        itm.Draw();

                    endLineVect = laserSightAim.Center;

                    if (player1.CurrentItem == 5)
                        DrawLine(player1.Center, endLineVect, 1, Color.Red);

                    numFrames = 1; //anything else
                    if (player1.CurrentItem == 0 || player1.CurrentItem == 2) // Fists or Pipe
                        numFrames = 4;

                    player1.Draw(spriteBatch, angle, playerImages, healthItems, gameTime, numFrames, fistPunch, pipeHit, soundEffectsOn);

                    grenadeActive = false;
                    for (i = 0; i < multipleBullets.Count; i++){
                        if (multipleBullets[i] is Grenade && multipleBullets[i].IsActive) { grenadeActive = true; }
                    }
                    if (player1.CurrentItem == 6 && !grenadeActive)
                        spriteBatch.Draw(grenadeSpot, GHitSpot.Center, null, Color.DarkGreen, 0, new Vector2(grenadeSpot.Width / 2, grenadeSpot.Height / 2), 1, SpriteEffects.None, 0);
                    else if (grenadeActive)
                        spriteBatch.Draw(grenadeSpot, GHitSpot.Center, null, Color.DarkRed, 0, new Vector2(grenadeSpot.Width / 2, grenadeSpot.Height / 2), 1, SpriteEffects.None, 0);

                    //Since GHitspot is not really a Melee Object, weapon damage is used to tell animation time. If -1 animation is not playing.
                    if (GHitSpot.WeaponDamage != -1){
                        if (GHitSpot.WeaponDamage <= 10){
                            spriteBatch.Draw(explosionImage, new Rectangle((int)GHitSpot.Center.X, (int)GHitSpot.Center.Y, GHitSpot.Width, GHitSpot.Height), new Rectangle(0, (GHitSpot.WeaponDamage / 4) * 250, 250, 250), Color.White, 0, new Vector2(250 / 2, 250 / 2), SpriteEffects.None, 0);
                            GHitSpot.WeaponDamage++;
                        }
                        else if (GHitSpot.WeaponDamage <= 15){
                            spriteBatch.Draw(explosionImage, new Rectangle((int)GHitSpot.Center.X, (int)GHitSpot.Center.Y, GHitSpot.Width, GHitSpot.Height), new Rectangle(0, (2 - GHitSpot.WeaponDamage / 4) * 250, 250, 250), Color.White, 0, new Vector2(250 / 2, 250 / 2), SpriteEffects.None, 0);
                            GHitSpot.WeaponDamage++;
                        }
                        else if (GHitSpot.WeaponDamage <= 25){
                            spriteBatch.Draw(explosionImage, new Rectangle((int)GHitSpot.Center.X, (int)GHitSpot.Center.Y, GHitSpot.Width, GHitSpot.Height), new Rectangle(0, (3 - GHitSpot.WeaponDamage / 4) * 250, 250, 250), Color.White, 0, new Vector2(250 / 2, 250 / 2), SpriteEffects.None, 0);
                            GHitSpot.WeaponDamage++;
                        }
                        else if (GHitSpot.WeaponDamage <= 30){
                            spriteBatch.Draw(explosionImage, new Rectangle((int)GHitSpot.Center.X, (int)GHitSpot.Center.Y, GHitSpot.Width, GHitSpot.Height), new Rectangle(0, (4 - GHitSpot.WeaponDamage / 4) * 250, 250, 250), Color.White, 0, new Vector2(250 / 2, 250 / 2), SpriteEffects.None, 0);
                            GHitSpot.WeaponDamage++;
                        }
                        else if (GHitSpot.WeaponDamage > 30)
                            GHitSpot.WeaponDamage = -1;
                    }

                    for (i = 0; i < walls.Count; i++)
                        walls[i].Draw();
                    for (i = 0; i < barricades.Count; i++){
                        if (barricades[i].HitsTillBroken < Constants.BARRICADE_HEALTH / 3)
                            barricades[i].CutOut = new Rectangle(126, 84, 42, 42);
                        barricades[i].Draw();
                    }

                    for (i = 0; i < multipleBullets.Count; i++){
                        if (multipleBullets[i].IsActive)
                            multipleBullets[i].Draw();
                    }

                    if (player1.CurrentItem == 9 && flameON)
                        spriteBatch.Draw(flameImage, new Rectangle((int)(player1.Center.X + (Math.Cos(angle + .21f) * Constants.MELEE_VAR)), (int)(player1.Center.Y + (Math.Sin(angle + .21f) * Constants.MELEE_VAR)), (int)(147 * Constants.SCALE_WIDTH), (int)(53 * Constants.SCALE_HEIGHT)), new Rectangle(0, (framesElapsed % 6) * 90, 248, 90), Color.White, angle + .1f, new Vector2(0, (540 / 6) / 2), SpriteEffects.None, 0);

                    if (showOutlines){
                        foreach (GameObject obj in tree.GetAllObjects()){
                            if (obj is Player)
                                DrawRectangleOutline(obj.Rectangle, Color.Purple);
                            else if (obj is Ammo)
                                DrawRectangleOutline(obj.Rectangle, Color.Red);
                            else if (obj is Melee)
                                DrawRectangleOutline(obj.Rectangle, Color.Orange);
                            else
                                DrawRectangleOutline(obj.Rectangle, Color.DarkSlateBlue);
                        }

                        DrawRectangleOutline(player1.OuterBounds, Color.DarkGoldenrod);
                        if (GHitSpot != null)
                            DrawRectangleOutline(GHitSpot.Rectangle, Color.DarkGreen);
                        if (laserSightAim != null)
                            DrawRectangleOutline(laserSightAim.Rectangle, Color.Gold);

                        foreach (Rectangle rec in tree.GetAllRectangles())
                            DrawRectangleOutline(rec, Color.Gold);
                    }

                    //health and stamina warnings
                    if (player1.Health < 15)
                        spriteBatch.Draw(lowHealthWarning, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 0, 105, 23), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (stats["stamina"] < 15)
                        spriteBatch.Draw(lowStaminaWarning, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 0, 105, 23), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                    #region Combo display
                    //combos
                    if (zombieMultiplier == 1)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 0, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 2)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 36, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 3)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 72, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 4)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 108, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 5)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 144, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 6)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 180, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 7)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 216, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 8)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 252, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 9)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 288, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 10)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 324, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 11)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 360, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 12)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 396, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 13)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 432, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 14)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 468, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 15)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 474, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 16)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 510, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 17)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 546, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 18)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 582, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 19)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 618, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    if (zombieMultiplier == 20)
                        spriteBatch.Draw(combo, new Vector2(player1.rectangleX, player1.rectangleY - 100), new Rectangle(0, 654, 50, 36), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    #endregion Combo display

                    #region New item drop down notification
                    if (newWeapon >= 0 && gameTime.TotalGameTime.TotalMilliseconds < (newWeaponMessageTime + Constants.NEW_WEAPON_MESSAGE_TIME).TotalMilliseconds)
                    {
                        // This keeps track of a value of 1/5 of total message display time, or 20%
                        timePart = Constants.NEW_WEAPON_MESSAGE_TIME.TotalMilliseconds / 5;
                        // base alpha for everything here. if changed everything else will update accordingly.
                        msgAlpha = 125;
                        messageWidth = (int)(160 * Constants.SCALE_WIDTH);
                        messageHeight = (int)(50 * Constants.SCALE_HEIGHT);
                        // the space between rect1 and rect2.
                        sizeDif = (int)((4 * Constants.SCALE_WIDTH) + (4 * Constants.SCALE_HEIGHT) / 2);
                        timeDif = gameTime.TotalGameTime.TotalMilliseconds - newWeaponMessageTime.TotalMilliseconds;
                        // value that lets eveything slide down.
                        ySlide = 0;
                        percent = 1;
                        percentByPart = timePart * 3; // 60% - when fadeout part of message begins
                        //move variables to attributes section

                        // if 20% down done
                        if (timeDif < timePart)
                        {
                            // gets a percent (0%(min pos) / 20% = 0% so it works), and since the percent goes up, 1 - percent makes it go down before applying to ySlide.
                            ySlide = -(int)(messageHeight * (1 - (timeDif / timePart)));
                        }
                        // if 60%+ done
                        else if (timeDif > percentByPart)
                        {
                            //since timeDif has to be over 60% to be here, 60%/100% != 0%, so you have to subtract 60% from each side to get a percent from 100%.
                            // Since we want percent to go go 1-percentUp = percent down.
                            percent = (1 - ((timeDif - percentByPart) / (Constants.NEW_WEAPON_MESSAGE_TIME.TotalMilliseconds - percentByPart)));
                        }

                        // All positions based of this Rectangle, if not directly (text 2 is related of rect2 which is related of rect1).
                        rect1 = new Rectangle(Constants.PLAY_AREA_LEFT + ((Constants.PLAY_AREA.Width - messageWidth) / 2), ySlide, messageWidth, messageHeight);
                        rect2 = new Rectangle(rect1.X + sizeDif, rect1.Y + sizeDif + (sizeDif * 2), rect1.Width - (sizeDif * 2), rect1.Height - (sizeDif * 4));

                        // This fades everything based on percent (apparently, unlike using Color.FromPreMultiplied, you have to lighten the color to for it to truely "fade").
                        color1_1 = Color.Maroon; color1_1.A = (byte)(msgAlpha * percent); color1_1.B = (byte)(color1_1.B * percent); color1_1.G = (byte)(color1_1.G * percent); color1_1.R = (byte)(color1_1.R * percent);
                        color2_1 = Color.NavajoWhite; color2_1.A = (byte)(msgAlpha * percent); color2_1.B = (byte)(color2_1.B * percent); color2_1.G = (byte)(color2_1.G * percent); color2_1.R = (byte)(color2_1.R * percent);
                        color1_2 = Color.IndianRed; color1_2.A = (byte)(msgAlpha * percent); color1_2.B = (byte)(color1_2.B * percent); color1_2.G = (byte)(color1_2.G * percent); color1_2.R = (byte)(color1_2.R * percent);
                        color2_2 = Color.Black; color2_2.A = (byte)(msgAlpha * percent); color2_2.B = (byte)(color2_2.B * percent); color2_2.G = (byte)(color2_2.G * percent); color2_2.R = (byte)(color2_2.R * percent);

                        //spriteBatch.Draw(bar1Image, rect1, color1_1);
                        spriteBatch.Draw(wallImage, rect2, color2_1);

                        defaultText = "New Weapon:";
                        spriteBatch.DrawString(smallText, defaultText, new Vector2(rect1.X + ((rect1.Width - (smallText.MeasureString(defaultText).X * Constants.SCALE.X)) / 2), rect1.Y),
                            color1_2, 0, Vector2.Zero, Constants.SCALE, SpriteEffects.None, 0);

                        defaultText = Constants.weaponNames[newWeapon] + "!";
                        spriteBatch.DrawString(smallText, defaultText, new Vector2(rect2.X + ((rect2.Width - (smallText.MeasureString(defaultText).X * Constants.SCALE.X)) / 2), rect2.Y + ((rect2.Height - (smallText.MeasureString(defaultText).Y * Constants.SCALE.Y)) / 2)),
                            color2_2, 0, Vector2.Zero, Constants.SCALE, SpriteEffects.None, 0);
                    }
                    else if (newWeapon >= 0){
                        newWeapon = -1;
                        newWeaponMessageTime = TimeSpan.Zero;
                    }
                    #endregion New item drop down notification

                    #region HUD Food display
                    // Food
                    spriteBatch.Draw(lasagna, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(100 * Constants.SCALE_HEIGHT)), new Rectangle(0, 0, 77, 80), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(gameText, "" + healthItems, new Vector2((int)(20 * Constants.SCALE_WIDTH), (int)(100 * Constants.SCALE_HEIGHT)), Color.Red);
                    #endregion HUD Food display

                    #region HUD Weapon/Ammo Display
                    //Weapons
                    //add all weapons, optimize code by making variables for size, rather than massive blocks of code
                    if (player1.CurrentItem == 0){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(0, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 1){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 2){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 2, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 3){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 3, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 4){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 4, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 5){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(0, 50, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 6){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50, 50, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 7){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 2, 50, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 8){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 3, 50, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else if (player1.CurrentItem == 9){
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(50 * 4, 50, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }
                    else{
                        spriteBatch.Draw(lgWeapons, new Vector2((int)(10 * Constants.SCALE_WIDTH), (int)(60 * Constants.SCALE_HEIGHT)), new Rectangle(0, 0, 50, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    }

                    //Ammo
                    spriteBatch.DrawString(gameText, "Ammo", new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(65 * Constants.SCALE_HEIGHT)), Color.Red);
                    spriteBatch.DrawString(gameText, "" + ammoLeft, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(75 * Constants.SCALE_HEIGHT)), Color.Red);
                    #endregion HUD Weapon/Ammo Display

                    #region HUD Health/Stamina/Strength/Proficiency Display
                    //health bar
                    if (player1.Health <= 100 && player1.Health > 75){
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 5, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (player1.Health <= 75 && player1.Health > 50){
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 4, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (player1.Health <= 50 && player1.Health > 25){
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 3, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (player1.Health <= 25 && player1.Health > 10){
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 2, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (player1.Health <= 10 && player1.Health > 0){
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else{
                        spriteBatch.Draw(hpBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(15 * Constants.SCALE_HEIGHT)), new Rectangle(0, 0, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }

                    //stamina bar
                    if (stats["stamina"] <= 100 && stats["stamina"] > 75){
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 5, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (stats["stamina"] <= 75 && stats["stamina"] > 50){
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 4, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (stats["stamina"] <= 50 && stats["stamina"] > 25){
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 3, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (stats["stamina"] <= 25 && stats["stamina"] > 10){
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25 * 2, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else if (stats["stamina"] <= 10 && stats["stamina"] > 0){
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 25, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }
                    else{
                        spriteBatch.Draw(stamBarImg, new Vector2((int)(50 * Constants.SCALE_WIDTH), (int)(25 * Constants.SCALE_HEIGHT)), new Rectangle(0, 0, 275, 25), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    }

                    //origin circle
                    spriteBatch.Draw(circle, new Vector2((int)(5 * Constants.SCALE_WIDTH), (int)(5 * Constants.SCALE_WIDTH)), new Rectangle(0, 0, 79, 79), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    #endregion HUD Health/Stamina/Strength/Proficiency Display

                    //call tutorial if enabled
                    if(currentLevel == 0/*&& tutorialEnabled*/)
                        Tutorial();

                    #region GameState.paused
                    if (currentState == GameState.paused){
                        //this gives the grayed out look behind paused menu
                        spriteBatch.Draw(pausedBack, new Rectangle(0, 0, graphics.PreferredBackBufferWidth - 1, graphics.PreferredBackBufferHeight - 1), Constants.PAUSE_OUTER_GRAY);
                        spriteBatch.Draw(pausedMenu, new Rectangle((int)(59 * Constants.SCALE_WIDTH), (int)(59 * Constants.SCALE_HEIGHT), (int)(639 * Constants.SCALE_WIDTH), (int)(307 * Constants.SCALE_HEIGHT)), Color.White);

                        //scale pause menu options
                        spot = buttons["Pause"][0];

                        //music option
                        if(musicOn)
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][0].X, buttons["Pause"][0].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][0].X, buttons["Pause"][0].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(gameText, "Play Music", new Vector2(211, 144), Color.DarkOrange);

                        //sound option
                        if (soundEffectsOn)
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][1].X, buttons["Pause"][1].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][1].X, buttons["Pause"][1].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(gameText, "Play Sound Effects", new Vector2(211, 204), Color.DarkOrange);

                        //zombie color option
                        if (randomZombieColor)
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][2].X, buttons["Pause"][2].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][2].X, buttons["Pause"][2].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(gameText, "Randomly Colored Zombies", new Vector2(211, 264), Color.DarkOrange);

                        //zombie hats option
                        if (zombieHats)
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][3].X, buttons["Pause"][3].Y), new Rectangle(332, 820, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(optionsButtons, new Vector2(buttons["Pause"][3].X, buttons["Pause"][3].Y), new Rectangle(332, 755, 58, 46), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(gameText, "Zombie Hats", new Vector2(211, 324), Color.DarkOrange);

                        spot = buttons["Pause"][currentButtonUD]; spot.X += spot.Height; spot.Width -= spot.Height;
                        if (gamePadConnected)
                            spriteBatch.Draw(selectionOutline, spot, Color.White);

                        if (showOutlines)
                            DrawRectangleOutline(new Rectangle((int)(506 * Constants.SCALE_WIDTH), (int)(102 * Constants.SCALE_HEIGHT), (int)(101 * Constants.SCALE_WIDTH), (int)(107 * Constants.SCALE_HEIGHT)), Color.White);
                    }
                    #endregion GameState.paused

                    #region GameState.goToMenu
                    if (currentState == GameState.goToMenu)
                    {
                        // This gives the grayed out look behind exit box
                        spriteBatch.Draw(pausedBack, new Rectangle(0, 0, graphics.PreferredBackBufferWidth - 1, graphics.PreferredBackBufferHeight - 1), Constants.PAUSE_OUTER_GRAY);
                        width = 262; height = 112;
                        rect = new Rectangle(graphics.PreferredBackBufferWidth / 2 - width / 2, graphics.PreferredBackBufferHeight / 2 - height / 2, width, height);
                        //spriteBatch.Draw(yesNoTexture, rect, Color.White);

                        if (gamePadConnected){
                            if (SingleButtonPress(Constants.SELECT_BUTTON)){
                                currentState = GameState.menu;
                                mState = MenuState.normal;
                                stats["stamina"] = 100;
                            }
                            else if (SingleButtonPress(Constants.RETURN_BUTTON))
                                currentState = GameState.paused;
                        }
                        else{
                            if (CheckIfClicked(new Rectangle(rect.X + 6, rect.Y + 5, rect.Width / 2 - 10, rect.Height - 10))){
                                currentState = GameState.menu;
                                mState = MenuState.normal;
                                stats["stamina"] = 100;
                            }
                            else if (CheckIfClicked(new Rectangle(rect.X + rect.Width / 2 + 0, rect.Y + 5, rect.Width / 2 - 9, rect.Height - 10)))
                                currentState = GameState.paused;
                        }
                    }
                    #endregion GameState.goToMenu

                    break;
                #endregion The rest

                #region GameState.gameOver
                case GameState.gameOver:
                    spriteBatch.Draw(gameoverScreen, new Rectangle(0, 0, Constants.TOTAL_WIDTH, Constants.TOTAL_HEIGHT), Color.White);
                    spriteBatch.DrawString(gameText, "Current Level: " + currentLevel, new Vector2(graphics.PreferredBackBufferWidth / 2 - (gameText.MeasureString("Current Level: " + currentLevel).X / 2), graphics.PreferredBackBufferHeight - 60), Color.Maroon);
                    spriteBatch.DrawString(gameText, "Final Score: " + score, new Vector2(graphics.PreferredBackBufferWidth / 2 - (gameText.MeasureString("Final Score: " + score).X / 2), graphics.PreferredBackBufferHeight - 40), Color.Maroon);

                    spriteBatch.Draw(backButton, new Vector2(buttons["GameOver"][buttons["GameOver"].Length - 1].X, buttons["GameOver"][buttons["GameOver"].Length - 1].Y), new Rectangle(0, 0, 345, 257), Color.White, 0, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0);
                    //if (gamePadConnected)
                    //    spriteBatch.DrawString(gameText, "Press Start to return to Main Menu...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (gameText.MeasureString("Press Start to return to Main Menu...").X / 2), graphics.PreferredBackBufferHeight - gameText.MeasureString("Press Start to return to Main Menu...").Y - 10), Color.SlateGray);
                    //else
                    //    spriteBatch.DrawString(gameText, "Press Space to return to Main Menu...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (gameText.MeasureString("Press Space to return to Main Menu...").X / 2), graphics.PreferredBackBufferHeight - gameText.MeasureString("Press Space to return to Main Menu...").Y - 10), Color.SlateGray);
                    break;
                #endregion GameState.gameOver
            }

            //draw cursor if gamePad is not connected
            if (!gamePadConnected)
                spriteBatch.Draw(cursorImage, new Rectangle((int)mouseLoc.X - 16, (int)mouseLoc.Y - 16, 32, 32), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion Draw

        #region DrawRectangle
        private void DrawRectangleOutline(Rectangle rect, Color color)
        {
            // Draw the 4 lines as 4 thin boxes: top, right, bottom, left
            DrawBox(rect.X, rect.Y, rect.Width, 1, color);
            DrawBox(rect.X + rect.Width, rect.Y, 1, rect.Height, color);
            DrawBox(rect.X, rect.Y + rect.Height, rect.Width, 1, color);
            DrawBox(rect.X, rect.Y, 1, rect.Height, color);
        }

        private void DrawBox(int x, int y, int width, int height, Color color)
        {
            spriteBatch.Draw(wallImage, new Rectangle(x, y, width, height), color);
        }

        private void DrawLine(Vector2 start, Vector2 end, int lineWidth, Color color)
        {
            // Pixel should be global, but this is not currently used.
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            Vector2 scale = new Vector2(Vector2.Distance(end, start), lineWidth);
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            spriteBatch.Draw(pixel, start, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        public int LimitInts(int value, int min, int max)
        {
            if (value < min){
                if (((value - min) % (max - min + 1)) == 0)
                    return 0;
                return (max + 1) + ((value - min) % (max - min + 1));
            }
            else if (value > max){
                return min + (value % (max - min + 1));
            }
            return value;
        }
        #endregion DrawRectangle

        #region Number Key/Button Support
        public void NumberKeys()
        { 
            if (SingleKeyPress(Constants.WEAPON1))
                if (ownedWeapons[0]) { player1.CurrentItem = 0; }
            if (SingleKeyPress(Constants.WEAPON2))
                if (ownedWeapons[1]) { player1.CurrentItem = 1; }
            if (SingleKeyPress(Constants.WEAPON3))
                if (ownedWeapons[2]) { player1.CurrentItem = 2; }
            if (SingleKeyPress(Constants.WEAPON4))
                if (ownedWeapons[3]) { player1.CurrentItem = 3; }
            if (SingleKeyPress(Constants.WEAPON5))
                if (ownedWeapons[4]) { player1.CurrentItem = 4; }
            if (SingleKeyPress(Constants.WEAPON6))
                if (ownedWeapons[5]) { player1.CurrentItem = 5; }
            if (SingleKeyPress(Constants.WEAPON7))
                if (ownedWeapons[6]) { player1.CurrentItem = 6; }
            if (SingleKeyPress(Constants.WEAPON8))
                if (ownedWeapons[7]) { player1.CurrentItem = 7; }
            if (SingleKeyPress(Constants.WEAPON9))
                if (ownedWeapons[8]) { player1.CurrentItem = 8; }
            if (SingleKeyPress(Constants.WEAPON10))
                if (ownedWeapons[9]) { player1.CurrentItem = 9; }

            if (SingleKeyPress(Constants.EAT_KEY)){
                if (healthItems > 0 && player1.Health != Constants.PLAYER_HEALTH){
                    player1.Health += Constants.FOOD_HEAL;
                    healthItems--;
                }
            }

            if (SingleKeyPress(Constants.PAUSE_KEY)){
                currentState = GameState.paused;
                currentButtonUD = 0;
            }

            if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                player1.CurrentItem = player1.PreviousItem;

            if (kbState.IsKeyDown(Constants.EXIT_KEY))
                currentState = GameState.goToMenu;

            if (player1.CurrentItem != 4)
                chainsawPlay.Stop();
        }

        /// <summary> Button Support (For GameState.game)</summary>
        public void ButtonStuff()
        {
            if (gamePadConnected && SingleButtonPress(Constants.EAT_BUTTON))
            {
                player1.CurrentItem = 10;
                if (healthItems > 0 && player1.Health != Constants.PLAYER_HEALTH)
                {
                    player1.Health += Constants.FOOD_HEAL;
                    healthItems--;
                }
            }

            if (gamePadConnected && SingleButtonPress(Constants.PREVIOUS_WEAPON))
                player1.CurrentItem = player1.PreviousItem;

            if (SingleButtonPress(Constants.PAUSE_BUTTON))
            {
                currentState = GameState.paused;
                currentButtonUD = 0;
            }

            if (SingleButtonPress(Constants.EXIT_BUTTON))
                currentState = GameState.goToMenu;
        }
        #endregion Number Key/Button Support

        #region Tutorial
        //Runs tutorial if option is enabled
        public void Tutorial()
        {
            //Start tutorial timer
            tutorialWatch.Start();

            if (tutorialWatch.ElapsedMilliseconds < 5000){
                spriteBatch.DrawString(gameText, "Welcome to the End of the World", new Vector2(500, 0), Color.Crimson);
            }
            else if (tutorialWatch.ElapsedMilliseconds >= 5000 && tutorialWatch.ElapsedMilliseconds < 10000){
                spriteBatch.DrawString(gameText, "It's dangerous to go alone, you'll need to learn some skills.", new Vector2(500, 0), Color.Crimson);
            }
            else if (tutorialWatch.ElapsedMilliseconds > 15000 && tutorialWatch.ElapsedMilliseconds < 20000){
                spriteBatch.DrawString(gameText, "First, use WASD to move.", new Vector2(500, 0), Color.Crimson);
            }
            else if (tutorialWatch.ElapsedMilliseconds > 20000 && tutorialWatch.ElapsedMilliseconds < 25000)
            {
                spriteBatch.DrawString(gameText, "Move over to a crate and press Q to pick it up and move it.", new Vector2(500, 0), Color.Crimson);
            }
            else if ((tutorialWatch.ElapsedMilliseconds > 25000 && tutorialWatch.ElapsedMilliseconds < 30000) && !ownedWeapons[1])
            {
                spriteBatch.DrawString(gameText, "You'll need some protection. Move over the pistol to pick it up.", new Vector2(500, 0), Color.Crimson);
            }

            //switch and fire weapon
            if (ownedWeapons[1]){
                switchFlag = true;

                if (SingleMouseClick()){
                    shootFlag = true;
                }
                //spawn dummy enemies then ammo

                //decrement health, then spawn lasagna

                //completion message
                if (tutorialWatch.ElapsedMilliseconds > 40000 && ownedWeapons[1] && shootFlag){
                    spriteBatch.DrawString(gameText, "Good. You've gained a few skills. Now let's see you use them.", new Vector2(500, 0), Color.Crimson);
                    //NextLevel();
                }
            }
        }
        #endregion Tutorial

        #region Level Generation (NextLevel, NextWave, SetupQuadTree)
        /// <summary> Sets up the next level once a level is completed </summary>
        public void NextLevel()
        {
            GHitSpot.WeaponDamage = -1; // Ends explosions animation so it doesn't carry over to next level.

            currentLevel++;
            currentWave = 1;

            player1.Health = Constants.PLAYER_HEALTH;

            //set player spawn for tutorial/current level
            if (currentLevel == 1){
                player1.rectangleX = (int)(40 * Constants.SCALE_WIDTH);
                player1.rectangleY = (int)(186 * Constants.SCALE_HEIGHT);
            }
            else {
                player1.rectangleX = Constants.PLAY_AREA_LEFT + Constants.PLAY_AREA_RIGHT / 2 - 150;
                player1.rectangleY = Constants.PLAY_AREA_TOP + Constants.PLAY_AREA_BOTTOM / 2 - 20;
            }

            enemies = new List<Enemy>();
            walls = new List<Wall>();
            barricades = new List<Barricade>();
            items = new List<Item>();

            if (currentLevel <= Constants.levels.Count)
                GetMap(Constants.levels[currentLevel - 1]);
            else {
                currentLevel = 1;
                zombieMultiplier += .5;
                GetMap(Constants.levels[currentLevel - 1]);
            }
            foreach (GameObject gmOb in tree.GetAllObjects()) {
                if (player1.Intersects((GameObject)gmOb)) {
                    ((GameObject)gmOb).IsActive = false;
                    if (((GameObject)gmOb) is Obstructions) {
                        ((Obstructions)gmOb).HitsTillBroken = 1;
                        ((Obstructions)gmOb).Attack();
                    }
                }
            }
            SaveData();
            ammoLeft += 150;

            currentState = GameState.loading;
            loadWatch = new Stopwatch();
            loadWatch.Start();
        }

        public void NextWave()
        {
            System.Timers.Timer t = new System.Timers.Timer();
            if (currentWave >= 3)
                NextLevel();
            else {
                currentWave++;
                waveSinceDrop++;

                currentState = GameState.loading;
                loadWatch = new Stopwatch();
                loadWatch.Start();
            }
        }

        public void SetupQuadTree()
        {
            tree = new QuadTree(Constants.PLAY_AREA_LEFT - 50, Constants.PLAY_AREA_TOP - 50, Constants.PLAY_AREA_RIGHT + 50, Constants.PLAY_AREA_BOTTOM + 50, null);
            tree.Divide();
            foreach (QuadTree quadT in tree._divisions) {
                quadT.Divide();
                foreach (QuadTree quadT2 in quadT._divisions)
                    quadT2.Divide();
            }
            tree.AddObject(player1);
            tree.AddObject(melee);
            foreach (Wall wl in walls)
                if (wl.IsActive) { tree.AddObject(wl); }
            foreach (Ammo amo in multipleBullets)
                if (amo.IsActive) { amo.IsActive = false; tree.RemoveObject(amo); }
            foreach (Barricade bar in barricades)
                tree.AddObject(bar);
            foreach (Enemy en in enemies)
                tree.AddObject(en);
        }
        #endregion Level Generation (NextLevel, NextWave, SetupQuadTree)

        #region Spawning (Zombies, Weapons)
        public void SpawnZombies()
        {
            //calculate amount of enemies to generate
            numEnemies = (int)(1 + currentLevel + currentWave + (currentLevel + 2) * zombieMultiplier);
            numEnemies *= (difficulty + 1);
            ammoLeft += numEnemies;
            healthItems++;
            numEnemiesLeft = numEnemies;

            posGeneratorX = new Random();
            Constants.Wait(2);
            posGeneratorY = new Random();

            //generate new enemies at random places 100 pixels in any direction away from the player
            for (i = 0; i < numEnemies; i++){
                Constants.Wait(5);
                num = rng.Next(20);

                //random number determining zombie type based on level
                //sets currentZombieImage to the corresponding one
                zombieType = 0;

                if (currentLevel == 2)
                    zombieType = rng.Next(1);
                else if (currentLevel == 3)
                    zombieType = rng.Next(2);
                else if (currentLevel == 4)
                    zombieType = rng.Next(3);
                else if (currentLevel == 5)
                    zombieType = rng.Next(4);

                randomPosX = Constants.PLAY_AREA_LEFT + posGeneratorX.Next(Constants.PLAY_AREA_RIGHT - Constants.PLAY_AREA_LEFT);
                randomPosY = Constants.PLAY_AREA_TOP + posGeneratorY.Next(Constants.PLAY_AREA_BOTTOM - Constants.PLAY_AREA_TOP);
                if (num == 0)
                    newEnemy = new Enemy(Constants.PLAY_AREA_LEFT, Constants.PLAY_AREA_TOP, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num == 1)
                    newEnemy = new Enemy(Constants.PLAY_AREA_RIGHT, Constants.PLAY_AREA_TOP, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num == 2)
                    newEnemy = new Enemy(Constants.PLAY_AREA_LEFT, Constants.PLAY_AREA_BOTTOM, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num == 3)
                    newEnemy = new Enemy(Constants.PLAY_AREA_RIGHT, Constants.PLAY_AREA_BOTTOM, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num >= 4 && num <= 7)
                    newEnemy = new Enemy(randomPosX, Constants.PLAY_AREA_TOP, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num >= 8 && num <= 11)
                    newEnemy = new Enemy(randomPosX, Constants.PLAY_AREA_BOTTOM, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num >= 11 && num <= 14)
                    newEnemy = new Enemy(Constants.PLAY_AREA_LEFT, randomPosY, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else if (num >= 15)
                    newEnemy = new Enemy(Constants.PLAY_AREA_RIGHT, randomPosY, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                else
                    newEnemy = new Enemy(0, 0, Constants.ZOMBIE_SIZE_X, Constants.ZOMBIE_SIZE_Y, zombieType, zombieImages, tree, difficulty, new Random(), randomZombieColor, spriteBatch);
                enemies.Add(newEnemy); 
            }
        }

        public void NewWeapon(int wepNum, GameTime gt)
        {
            if (!ownedWeapons[wepNum]){
                ownedWeapons[wepNum] = true;
                newWeapon = wepNum;
                newWeaponMessageTime = new TimeSpan(0, 0, 0, 0, (int)gt.TotalGameTime.TotalMilliseconds);
            }
        }
        #endregion Spawning (Zombies, Weapons)

        #region Reset
        public void ResetGame()
        {
            player1.CurrentItem = 0; currentLevel = 0; currentWave = 1; ammoLeft = 200; waveSinceDrop = 0;
            player1.Health = Constants.PLAYER_HEALTH;
            player1.Alive = true;
            score = 0;
            for (i = 0; i < ownedWeapons.Length; i++) { ownedWeapons[i] = false; }
            if (newGame) { ownedWeapons[0] = true; ownedWeapons[1] = true; }
            LoadData();
            ownedWeapons[10] = true; // You always have food "weapon", it just might be empty
            NextLevel();
        }
        #endregion Reset

        #region Singular input and specific clicks
        public bool SingleKeyPress(Keys key)
        {
            if (kbState.IsKeyUp(key) && previouskbState.IsKeyDown(key))
                return true;
            return false;
        }

        public bool SingleButtonPress(Buttons bttn)
        {
            if (!singlePress && gamepadState.IsButtonUp(bttn) && previousGamepadState.IsButtonDown(bttn))
            {
                singlePress = true;
                return true;
            }
            return false;
        }

        public bool SingleMouseClick()
        {
            if (!singlePress && mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                singlePress = true;
                return true;
            }
            return false;
        }

        public bool MouseClick()
        {
            if (mouseState.LeftButton == previousMouseState.LeftButton)
                return false;
            return true;
        }

        /// <summary> Checks if your mouse has clicked a specific Rectangle </summary>
        public bool CheckIfClicked(Rectangle area)
        {
            if (area.Intersects(new Rectangle(mouseState.X, mouseState.Y, 1, 1)))
            {
                if (mouseState.LeftButton == ButtonState.Pressed && MouseClick())
                    return true;
            }
            return false;
        }

        public bool DoesMouseCollide(Rectangle area)
        {
            if (area.Intersects(new Rectangle(mouseState.X, mouseState.Y, 1, 1)))
            {
                return true;
            }
            return false;
        }
        #endregion Singular input and specific clicks

        #region Key/Button Binding
        #region Binding Menu
        /// <summary> Sets the Text for Button Binding Menu based on current Constant's value for the Button. </summary>
        public void SetButtonBindingText()
        {
            int ii = 0;
            buttonBindingText = new List<TextObject>()
            {
                new TextObject("Select : " + Constants.SELECT_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Return : " + Constants.RETURN_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Attack (choice 1) : " + Constants.ATTACK_BUTTON1, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Attack (choice 2) : " + Constants.ATTACK_BUTTON2, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Eat : " + Constants.EAT_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Pick Up Barricade : " + Constants.BARRICADE_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Pause : " + Constants.PAUSE_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Exit : " + Constants.EXIT_BUTTON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Switch Weapon Forward : " + Constants.SWITCH_FORWARD, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Switch Weapon Back : " + Constants.SWITCH_BACK, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("Use Previous Weapon : " + Constants.PREVIOUS_WEAPON, (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch),
                new TextObject("SET DEFAULT BUTTONS", (int)(100 * Constants.SCALE_WIDTH), (int)((50 * Constants.SCALE_HEIGHT) + ((25 * ii++) * Constants.SCALE_HEIGHT)), gameText, Color.Red, spriteBatch)
            };
        }

        public void SetKeyBindingText()
        {
            keyBindingText = new List<TextObject>()
            {
                new TextObject("" + Constants.KEY_UP, (int)(250 * Constants.SCALE_WIDTH), (int)(158* Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                new TextObject("" + Constants.KEY_LEFT, (int)(255 * Constants.SCALE_WIDTH), (int)(216 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                new TextObject("" + Constants.KEY_DOWN, (int)(270 * Constants.SCALE_WIDTH), (int)(274 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                new TextObject("" + Constants.KEY_RIGHT, (int)(265 * Constants.SCALE_WIDTH), (int)(332 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                
                /*new TextObject("Attack : " + Constants.BARRICADE_KEY, (int)(200 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                new TextObject("Switch Weapons : " + Constants.BARRICADE_KEY, (int)(200 * Constants.SCALE_WIDTH), (int)(50 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),*/

                new TextObject("" + Constants.EAT_KEY, (int)(550 * Constants.SCALE_WIDTH), (int)(274 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),
                new TextObject("" + Constants.BARRICADE_KEY, (int)(550 * Constants.SCALE_WIDTH), (int)(332 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch),

                new TextObject("SET DEFAULT BUTTONS", (int)(400 * Constants.SCALE_WIDTH), (int)(400 * Constants.SCALE_HEIGHT), gameText, Color.Red, spriteBatch)
            };
        }
        #endregion Binding Menu

        #region Button Binding
        public void GamepadBindingSave()
        {
            StreamWriter writer = new StreamWriter("ButtonBinding.sav");
            writer.WriteLine(Constants.SELECT_BUTTON.ToString() + " // Select Button");
            writer.WriteLine(Constants.RETURN_BUTTON.ToString() + " // Return Button");
            writer.WriteLine(Constants.ATTACK_BUTTON1.ToString() + " // Attack Button (Choice 1)");
            writer.WriteLine(Constants.ATTACK_BUTTON2.ToString() + " // Attack Button (Choice 2)");
            writer.WriteLine(Constants.EAT_BUTTON.ToString() + " // Eat");
            writer.WriteLine(Constants.BARRICADE_BUTTON.ToString() + " // Barricade");
            writer.WriteLine(Constants.PAUSE_BUTTON.ToString() + " // Pause");
            writer.WriteLine(Constants.EXIT_BUTTON.ToString() + " // Exit");
            writer.WriteLine(Constants.SWITCH_FORWARD.ToString() + " // Switch Forward");
            writer.WriteLine(Constants.SWITCH_BACK.ToString() + " // Switch Back");
            writer.WriteLine(Constants.PREVIOUS_WEAPON.ToString() + " // Previous Weapon");
            writer.Close();
            writer.Dispose();
        }

        public void GamepadBindingLoad()
        {
            if (File.Exists("ButtonBinding.sav"))
            {
                StreamReader reader = new StreamReader("ButtonBinding.sav");
                string firstLine = null;
                if ((firstLine = reader.ReadLine()) != null)
                {
                    Constants.SELECT_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.RETURN_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.ATTACK_BUTTON1 = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.ATTACK_BUTTON2 = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.EAT_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.BARRICADE_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.PAUSE_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.EXIT_BUTTON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.SWITCH_FORWARD = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.SWITCH_BACK = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.PREVIOUS_WEAPON = (Buttons)Enum.Parse(typeof(Buttons),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                }
                else
                    Constants.SET_DEFAULT_BUTTONS();
                reader.Close();
                reader.Dispose();
            }
            else
                Constants.SET_DEFAULT_BUTTONS();
        }
        #endregion Button Binding

        #region Key Binding
        public void KeyBoardBindingSave()
        {
            StreamWriter writer = new StreamWriter("KeyBinding.sav");
            writer.WriteLine(Constants.KEY_UP.ToString() + " // Up Key");
            writer.WriteLine(Constants.KEY_RIGHT.ToString() + " // Right Key");
            writer.WriteLine(Constants.KEY_DOWN.ToString() + " // Down Key");
            writer.WriteLine(Constants.KEY_LEFT.ToString() + " // Left Key");
            writer.WriteLine(Constants.BARRICADE_KEY.ToString() + " // Barricade Key");
            writer.WriteLine(Constants.EAT_KEY.ToString() + " // Eat Key");
            writer.WriteLine(Constants.PAUSE_KEY.ToString() + " // Pause Key");
            writer.WriteLine(Constants.EXIT_KEY.ToString() + " // Exit Key");
            writer.WriteLine(Constants.WEAPON1.ToString() + " // Weapon 1");
            writer.WriteLine(Constants.WEAPON2.ToString() + " // Weapon 2");
            writer.WriteLine(Constants.WEAPON3.ToString() + " // Weapon 3");
            writer.WriteLine(Constants.WEAPON4.ToString() + " // Weapon 4");
            writer.WriteLine(Constants.WEAPON5.ToString() + " // Weapon 5");
            writer.WriteLine(Constants.WEAPON6.ToString() + " // Weapon 6");
            writer.WriteLine(Constants.WEAPON7.ToString() + " // Weapon 7");
            writer.WriteLine(Constants.WEAPON8.ToString() + " // Weapon 8");
            writer.WriteLine(Constants.WEAPON9.ToString() + " // Weapon 9");
            writer.WriteLine(Constants.WEAPON10.ToString() + " // Weapon 10");
            writer.Close();
            writer.Dispose();
        }

        public void KeyBoardBindingLoad()
        {
            if (File.Exists("KeyBinding.sav"))
            {
                StreamReader reader = new StreamReader("KeyBinding.sav");
                string firstLine = null;
                if ((firstLine = reader.ReadLine()) != null)
                {
                    Constants.KEY_UP = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.KEY_RIGHT = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.KEY_DOWN = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.KEY_LEFT = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.BARRICADE_KEY = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.EAT_KEY = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.PAUSE_KEY = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.EXIT_KEY = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();

                    Constants.WEAPON1 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON2 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON3 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON4 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON5 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON6 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON7 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON8 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON9 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                    firstLine = reader.ReadLine();
                    Constants.WEAPON10 = (Keys)Enum.Parse(typeof(Keys),
                        firstLine.Remove(firstLine.IndexOf("//")), true);
                }
                else
                    Constants.SET_DEFAULT_KEYS();
                reader.Close();
                reader.Dispose();
            }
            else
                Constants.SET_DEFAULT_BUTTONS();
        }
        #endregion Key Binding
        #endregion Key/Button Binding

        #region New Game
        public void NewGame(int fileN)
        {
            StreamWriter writer = new StreamWriter("Save" + fileN + ".sav");
            writer.WriteLine(1 + " // Zombie Multiplier"); // Initial multiplier
            writer.WriteLine(0 + " // Level (starts at 0, ends at " + (Constants.levels.Count - 1) + ")"); // Current level - 1
            //Stats
            writer.WriteLine(1 + " // Strength"); // Strength
            writer.WriteLine(1 + " // Proficency"); // Proficency
            writer.WriteLine(0 + " // Score"); // Score
            //Store Weapons
            writer.Write(0); writer.WriteLine(" // Weapons: 0-9, last number must have a ',' (ex: 0,1,2,)");
            //Food
            writer.WriteLine(1 + " // Food");
            //Ammo
            writer.WriteLine(50 + " // Ammo");
            writer.Close();
            writer.Dispose();
        }
        #endregion New Game

        #region Save
        public int CheckScore()
        {
            reader = new StreamReader("Save" + fileNum + ".sav");
            reader.ReadLine(); reader.ReadLine(); reader.ReadLine(); reader.ReadLine();
            String scoreLine = reader.ReadLine();
            int oldScore;
            bool result = Int32.TryParse(scoreLine, out oldScore);
            if (result)
            {
                reader.Close();
                return oldScore;
            }
            else
            {
                reader.Close();
                return 0;
            }
        }
        public void SaveData()
        {
            int oldScore = CheckScore();
            /*if (oldScore > score)
            {
                score = oldScore;
            }*/
            writer = new StreamWriter("Save" + fileNum + ".sav");
            writer.WriteLine(zombieMultiplier + " // Zombie Multiplier");
            writer.WriteLine(currentLevel - 1 + " // Level (starts at 0, ends at " + (Constants.levels.Count - 1) + ")");
            //Stats
            writer.WriteLine(stats["strength"] + " // Strength");
            writer.WriteLine(stats["weapons"] + " // Proficency");
            writer.WriteLine(score + " // Score");
            //Store Weapons
            for (i = 0; i < ownedWeapons.Length; i++){
                //ownedWeapons[10] is food and is always true
                if (ownedWeapons[i] == true && i != 10)
                    writer.Write(i + ",");
            }
            writer.WriteLine(" // Weapons: 0-9, last number must have a ',' (ex: 0,1,2,)");
            //Food
            writer.WriteLine(healthItems + " // Food");
            //Ammo
            writer.WriteLine(ammoLeft + " // Ammo");
            writer.Close();
            writer.Dispose();
        }

        // Checks if a file is empty (include extension)
        public bool CheckIfEmpty(string fileN)
        {
            empty = false;
            if (File.Exists(fileN)) {
                reader = new StreamReader(fileN);
                if (reader.ReadLine() == null)
                    empty = true;
                reader.Close();
                reader.Dispose();
            }
            else
                empty = true;
            return empty;
        }
        
        public int GetSaveLevel(int fileN)
        {
            lvl = 0;
            if (File.Exists("Save" + fileN + ".sav")){
                reader = new StreamReader("Save" + fileN + ".sav");
                reader.ReadLine();
                line = reader.ReadLine();
                lvl = Int32.Parse(line.Remove(line.IndexOf("//")));
                reader.Close();
                reader.Dispose();
            }
            return lvl + 1;
        }

        public int GetSaveStr(int fileN)
        {
            str = 0;
            if (File.Exists("Save" + fileN + ".sav")){
                reader = new StreamReader("Save" + fileN + ".sav");
                reader.ReadLine(); reader.ReadLine();
                line = reader.ReadLine();
                str = Int32.Parse(line.Remove(line.IndexOf("//")));
                reader.Close();
                reader.Dispose();
            }
            return str;
        }

        public int GetSaveProf(int fileN)
        {
            int prof = 0;
            if (File.Exists("Save" + fileN + ".sav")){
                reader = new StreamReader("Save" + fileN + ".sav");
                reader.ReadLine(); reader.ReadLine(); reader.ReadLine();
                string line = reader.ReadLine();
                prof = Int32.Parse(line.Remove(line.IndexOf("//")));
                reader.Close();
                reader.Dispose();
            }
            return prof;
        }

        /// <summary> Saves values (Such as MusicOn) that are remembered on next program start-up. </summary>
        public void StartSave()
        {
            writer = new StreamWriter("StartUpSave.sav");
            writer.WriteLine(musicOn + " // Music");
            writer.WriteLine(soundEffectsOn + " // Sound");
            writer.WriteLine(randomZombieColor + " // Randomly Color Zombie");
            writer.Close();
            writer.Dispose();
        }
        #endregion Save

        #region Load
        /// <summary> Loads values at start-up for different properties that were saved from last time program was run. </summary>
        public void LoadValues()
        {
            if (File.Exists("StartUpSave.sav")){
                reader = new StreamReader("StartUpSave.sav");
                firstLine = null;
                if ((firstLine = reader.ReadLine()) != null){
                    musicOn = Boolean.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    firstLine = reader.ReadLine();
                    soundEffectsOn = Boolean.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    firstLine = reader.ReadLine();
                    randomZombieColor = Boolean.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                }
                else {
                    musicOn = true;
                    soundEffectsOn = true;
                    randomZombieColor = false;
                }
                reader.Close();
                reader.Dispose();
            }
            else {
                musicOn = true;
                soundEffectsOn = true;
                randomZombieColor = false;
            }
        }

        public void LoadData()
        {
            if (File.Exists("Save" + fileNum + ".sav")){
                reader = new StreamReader("Save" + fileNum + ".sav");
                firstLine = null;
                if ((firstLine = reader.ReadLine()) != null){
                    zombieMultiplier = Double.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    firstLine = reader.ReadLine();
                    currentLevel = Int32.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    //Stats
                    firstLine = reader.ReadLine();
                    stats["strength"] = Int32.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    firstLine = reader.ReadLine();
                    stats["weapons"] = Int32.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    //skip score
                    reader.ReadLine();
                    //Get Weapons
                    firstLine = reader.ReadLine();
                    line = firstLine.Remove(firstLine.IndexOf("//"));
                    parts = line.Split(',');
                    for (i = 0; i < parts.Length - 1; i++)
                        ownedWeapons[Int32.Parse(parts[i])] = true;
                    ownedWeapons[0] = true;
                    //Food
                    firstLine = reader.ReadLine();
                    healthItems = Int32.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                    //Ammo
                    firstLine = reader.ReadLine();
                    ammoLeft = Int32.Parse(firstLine.Remove(firstLine.IndexOf("//")));
                }
                else {
                    zombieMultiplier = 1;
                    currentLevel = 1;
                    strengthNum = 1;
                    proficiencyNum = 1;
                    score = 0;
                    ownedWeapons[0] = true;
                    healthItems = 1;
                    ammoLeft = 500;
                }
                reader.Close();
                reader.Dispose();
            }
            else {
                zombieMultiplier = 1; currentLevel = 1; strengthNum = 1; proficiencyNum = 1; healthItems = 1; ammoLeft = 500;
                ownedWeapons[0] = true;
            }
        }

        public void GetMap(string map)
        {
            greys = new Color[5] { Color.Gray, Color.DarkGray, Color.SlateGray, Color.DimGray, Constants.WALL_GRAY };
            rng = new Random();

            if (File.Exists(map + ".ktr")){
                reader = new StreamReader(map + ".ktr");
                reader.ReadLine();                      // reads the line to skip over the name information
                size = reader.ReadLine();
                sizes = size.Split(',');       // parses the size of the map
                row = int.Parse(sizes[0]);          // gets number of rows
                col = int.Parse(sizes[1]);          // gets number of columns
                s = reader.ReadLine();           // gets null background info

                //change to use all maps, set to "s"
                gameScreen = this.Content.Load<Texture2D>("Maps/" + s);  // sets background based on the mapname

                if (musicOn){
                    currentSong++;
                    if (currentSong > 3)
                        currentSong = 1;
                    theme1 = songDictionary["" + currentSong];
                }

                //skip past door info
                reader.ReadLine();

                // object reading
                pathTotal = reader.ReadLine();
                pathNum = int.Parse(pathTotal);
                paths = new List<string>();

                for (i = 0; i < pathNum; i++) { paths.Add(reader.ReadLine()); }
                objectNum = int.Parse(reader.ReadLine());
                for (i = 0; i < objectNum; i++){
                    objectInfo = reader.ReadLine().Split(':');
                    position = objectInfo[0].Split(',');
                    xPos = (int)((int.Parse(position[0]) * Constants.WALL_SIZE_X) + Constants.PLAY_AREA_LEFT);
                    yPos = (int)((int.Parse(position[1]) * Constants.WALL_SIZE_Y) + Constants.PLAY_AREA_TOP);

                    if (objectInfo[4] == "N"){
                        for(int k = 0; k < 7; k++){
                            for (int l = 0; l < 7; l++){
                                tempWall = new Wall(xPos + (k * Constants.WALL_PIECE_SIZE_X), yPos + (l * Constants.WALL_PIECE_SIZE_Y), Constants.WALL_PIECE_SIZE_X, Constants.WALL_PIECE_SIZE_Y, Constants.WALL_HEALTH, wallImage, tree, greys[rng.Next(5)], spriteBatch);
                                walls.Add(tempWall);
                            }
                        }
                    }
                    else if (objectInfo[4] == "B"){
                        tempBarricade = new Barricade(xPos, yPos, Constants.WALL_SIZE_X, Constants.WALL_SIZE_Y, Constants.BARRICADE_HEALTH, itemTextures, tree, spriteBatch);
                        //position of barricade on sprite sheet: 3, 3
                        tempBarricade.CutOut = new Rectangle(84, 84, 42, 42);
                        barricades.Add(tempBarricade);
                    }
                }
            }
            else {
                gameScreen = this.Content.Load<Texture2D>("Maps/Field");
                if (musicOn) { theme1 = songDictionary["" + currentSong]; } // sets background based on the mapname
            }
        }
        #endregion Load
    }
}