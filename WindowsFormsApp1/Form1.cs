using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dinogame__alpha_
{
    public partial class Form1 : Form
    {
        //Khai báo
        int lives = 2; 
        const int maxLives = 5;
        private int lifeBonusCount = 0; 
        int jumpVelocity = 0;
        int gravity = 1;
        int jumpPower = -15;
        bool isGrounded = false;
        int score = 0;
        bool gameOver = false;
        bool sliding = false;
        bool isJumping = false;
        private bool isInvincible = false;
        private Timer invincibleTimer = new Timer();
        private Timer invincibleFlashTimer = new Timer();
        private bool isIFlashing = false;
        private int flashCount = 0;
        private const int maxFlashCount = 10;
        Random rand = new Random();
        Image dinoJumpFrame;
        Image dinoFallFrame;
        Image dinoDeadFrame;
        Image[] dinoWalkFrames;
        int walkFrameIndex = 0;
        int walkFrameTimer = 0;
        int[] walkFrameDurations = new int[] { 120, 24, 120, 24 };
        int[] slideFrameDurations = new int[] { 100, 100 }; 
        int slideFrameIndex = 0;
        int slideFrameTimer = 0;
        int dinoOriginalHeight;
        int dinoOriginalWidth;
        List<PictureBox> obstacles = new List<PictureBox>();
        Image[] groundObstacleImages;
        Image[] flyingObstacleImages;
        int obstacleSpeed = 10;
        int gameProgress = 0; 
        int minFlyProgress = 100; 
        private int flySpawnCount = 0;  
        private int currentFlyWindow = 0;
        int nextColorChangeScore = 500; // cái này để thay nền
        Color[] retroColors = new Color[]
        {
            Color.FromArgb(0, 0, 255),      // blue
            Color.FromArgb(47, 79, 79),     // Dark Slate Gray
            Color.FromArgb(0, 255, 0),      // Lime
            Color.FromArgb(0, 255, 255),    // Cyan
            Color.FromArgb(255, 0, 255),    // Magenta
              Color.FromArgb(255, 0, 0),    //Red
            SystemColors.HighlightText

        };
        // Ảnh gốc - để recolor lại mỗi lần đổi nền
        private Image[] originalGroundObstacleImages;
        private Image[] originalFlyingObstacleImages;
        private Image[] originalDinoWalkFrames;
        private Image originalDinoJumpFrame;
        private Image originalDinoFallFrame;
        private Image originalDinoDeadFrame;
        private Image[] originalDinoSlideFrame;
        Timer flashTimer = new Timer();
        bool isFlashing = false;
        int flashDuration = 100; // milliseconds
        int flashElapsed = 0;
        Color targetBackgroundColor;
        Color originalBackgroundColor;
        Color inverseColor;
        private Point originalFormLocation;
        private Timer formMoveTimer = new Timer();

        private bool isMovingForm = false;
        private int nextMoveScoreThreshold = 1000;
        private Timer shrinkTimer = new Timer();
        private bool isShrinking = false;
        private Size originalFormSize;
        private int nextShrinkScoreThreshold = 1200;
        private bool pendingShrink = false;
        private DateTime formMoveStartTime;
        private TimeSpan formMoveDuration = TimeSpan.FromSeconds(10); // di chuyển trong 15 giây

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }
        //Game load
        private void Form1_Load(object sender, EventArgs e)
        {
            gameTimer.Interval = 20;
            LoadDinoFrames();
            dinoOriginalHeight = dino.Height;
            dinoOriginalWidth = dino.Width;

            originalGroundObstacleImages = new Image[]
            {
                Properties.Resources.Spike,
                Properties.Resources.Spike1,
                Properties.Resources.mediumObj,
                Properties.Resources.rectangleObj
            };
            groundObstacleImages = (Image[])originalGroundObstacleImages.Clone();

            originalFlyingObstacleImages = new Image[]
            {
                Properties.Resources.flyingObj,
                Properties.Resources.flyingObj1
            };
            flyingObstacleImages = (Image[])originalFlyingObstacleImages.Clone();

            LoadDinoFrames();


            // Initial spawn
            for (int i = 0; i < 3; i++) SpawnObstacle();

            flashTimer.Tick += FlashTimer_Tick; // <<< thêm dòng này để gắn flashTimer
            for (int i = 0; i < 3; i++) SpawnObstacle();

            invincibleTimer.Interval = 2000; // 2 giây bất tử
            invincibleTimer.Tick += (s, evt) =>
            {
                isInvincible = false;
                invincibleTimer.Stop();
            };

           
            invincibleFlashTimer.Interval = 200; // 0.2s/lần nhấp nháy
            invincibleFlashTimer.Tick += InvincibleFlashTimer_Tick;

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int centerX = (screen.Width - this.Width) / 2;
            int centerY = (screen.Height - this.Height) / 2;

            originalFormLocation = new Point(centerX, centerY);
            this.Location = originalFormLocation; // Đặt form vào giữa
            formMoveTimer.Tick += FormMoveTimer_Tick;
            shrinkTimer.Interval = 10000; // 10 giây
            shrinkTimer.Tick += ShrinkTimer_Tick;
            originalFormSize = this.Size;
        }
        //Score mechanic
        private void UpdateScore()
        {
            score++;
            labelScore.Text = "Score: " + score;
            if (score % 100 == 0 && obstacleSpeed < 25)
            {
                obstacleSpeed++;
            }
            if (gameProgress / 1000 > currentFlyWindow)
            {
                currentFlyWindow = gameProgress / 1000;
                flySpawnCount = 0; // Reset khi qua mỗi 1000 điểm
            }
            // Đổi màu nền khi đạt mốc điểm
            if (score >= nextColorChangeScore)
            {
                ChangeBackgroundColor();
                nextColorChangeScore += rand.Next(300, 800); 
            }
            if (score >= (lifeBonusCount + 1) * 3000 && lives < maxLives)
            {
                lives++;
                lifeBonusCount++;
                labelLives.Text = "♥ x " + lives;
            }
            // Dịch chuyển form nếu đạt mốc điểm
            if (score >= nextMoveScoreThreshold && !isMovingForm)
            {
                StartFormMovement(); // Gọi đúng logic đã chuẩn hóa
                nextMoveScoreThreshold += rand.Next(500, 800);
            }
            CheckShrinkTrigger();
        }
        //Game reset again
        private void ResetGame()
        {
            score = 0;
            labelScore.Text = "Score: 0";
            jumpVelocity = 0;
            sliding = false;
            gameOver = false;
            obstacleSpeed = 10;
            gameProgress = 0;
            flySpawnCount = 0;
            currentFlyWindow = 0;
            nextColorChangeScore = 500;
            lives = 2;
            labelLives.Text = "\u2665 x " + lives;
            this.BackColor = SystemColors.HighlightText;
            inverseColor = Color.Black;

            foreach (var obs in obstacles) this.Controls.Remove(obs);
            obstacles.Clear();

            dino.Height = dinoOriginalHeight;
            dino.Width = dinoOriginalWidth;
            dino.Top = ground.Top - dino.Height;
            // Reset animation
            walkFrameIndex = 0;
            walkFrameTimer = 0;
            dino.Image = dinoWalkFrames[0];

            groundObstacleImages = (Image[])originalGroundObstacleImages.Clone();
            flyingObstacleImages = (Image[])originalFlyingObstacleImages.Clone();

            ground.BackColor = Color.Black;
            for (int i = 0; i < 3; i++) SpawnObstacle();

            flashTimer.Stop();
            isFlashing = false;

            // Reset hiệu ứng thu nhỏ (shrink)
            shrinkTimer.Stop();
            isShrinking = false;
            this.Size = originalFormSize;
            nextShrinkScoreThreshold = 1200;

            // Reset hiệu ứng di chuyển form
            formMoveTimer.Stop();
            isMovingForm = false;

            nextMoveScoreThreshold = 1000;
            this.Location = originalFormLocation;
            SetupFormMoveTimer(); // Gán interval ban đầu, không khởi động
            // Khởi động lại game chính
            gameTimer.Start();
        }
        //Shrink mechanic
        private void ShrinkTimer_Tick(object sender, EventArgs e)
        {
            this.Size = originalFormSize;

            // Chỉ phục hồi vị trí gốc nếu không còn đang di chuyển
            if (!isMovingForm)
            {
                this.Location = originalFormLocation;
            }

            isShrinking = false;
            shrinkTimer.Stop();
        }
        private void CheckShrinkTrigger()
        {
            
            if (isShrinking || isMovingForm || score < nextShrinkScoreThreshold) return;
            // Chọn ngẫu nhiên giữa 3/4 và 2/3
            double[] shrinkRatios = { 0.75, 2.0 / 3.0 };
            double selectedRatio = shrinkRatios[rand.Next(shrinkRatios.Length)];

            int newWidth = (int)(originalFormSize.Width * selectedRatio);

            // Co từ phải sang trái
            this.Size = new Size(newWidth, originalFormSize.Height);
            this.Location = new Point(originalFormLocation.X + (originalFormSize.Width - newWidth), this.Location.Y);

            isShrinking = true;
            shrinkTimer.Start();

            // Đặt ngưỡng mới sau mỗi lần kích hoạt
            nextShrinkScoreThreshold += rand.Next(800, 1201); // +800 đến +1200 điểm
        }
       
        //Moving screen mechanic
          private void SetupFormMoveTimer()
        {
             formMoveTimer.Stop();
             formMoveTimer.Tick -= FormMoveTimer_Tick; // Gỡ handler cũ nếu có
             formMoveTimer.Tick += FormMoveTimer_Tick; // GẮN LẠI handler
             formMoveTimer.Interval = rand.Next(3000, 10000);
        }
        private void FormMoveTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver || !isMovingForm)
            {
                formMoveTimer.Stop();
                return;
            }

            if ((DateTime.Now - formMoveStartTime) >= formMoveDuration)
            {
                isMovingForm = false;
                formMoveTimer.Stop();

                // Nếu form không bị shrink, trả về vị trí gốc
                if (!isShrinking)
                {
                    this.Location = originalFormLocation;
                }

                return;
            }

            // Di chuyển trong khi shrink vẫn được phép
            Point newLocation = GetSafeRandomPosition(originalFormLocation);
            this.Location = newLocation;

            formMoveTimer.Interval = rand.Next(1200, 2000);
        }
        private void StartFormMovement()
        {
            if (isShrinking || isMovingForm) return; // không chạy nếu đang thu nhỏ hoặc đang chạy rồi

            isMovingForm = true;
            formMoveStartTime = DateTime.Now;
            formMoveTimer.Interval = rand.Next(1200, 2000);
            formMoveTimer.Start();
        }
        private Point GetSafeRandomPosition(Point origin)
        {
            int offsetX = rand.Next(-400, 401); // -200 đến +200
            int offsetY = rand.Next(-400, 401);

            int newX = origin.X + offsetX;
            int newY = origin.Y + offsetY;

            // Lấy kích thước màn hình
            Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;

            // Đảm bảo form không ra khỏi màn hình
            newX = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - this.Width, newX));
            newY = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - this.Height, newY));

            return new Point(newX, newY);
        }
//Spawn obstacles
        private int CountGroundObstacles()
        {
            return obstacles.Count(o =>
                o.Image != Properties.Resources.flyingObj &&
                o.Image != Properties.Resources.flyingObj1);
        }
        private int CountFlyingObstacles()
        {
            return obstacles.Count(o =>
                o.Image == Properties.Resources.flyingObj ||
                o.Image == Properties.Resources.flyingObj1);
        }
        private void SpawnObstacle()
        {
            if (obstacles.Count >= 4)
                return;

            int currentFlying = CountFlyingObstacles();
            int currentGround = CountGroundObstacles();

            bool allowFlying = gameProgress >= minFlyProgress;
            bool canSpawnFlying = currentFlying < 3;
            bool canSpawnGround = currentGround < 3;

            bool isFlying = false;

            if (allowFlying)
            {
                if (flySpawnCount < 10)
                {
                    if (currentFlying < 2 && currentGround < 2)
                    {
                        isFlying = rand.Next(5) == 0;
                    }
                    else if (currentFlying >= 2 && canSpawnGround)
                    {
                        isFlying = false;
                    }
                    else if (currentGround >= 2 && canSpawnFlying)
                    {
                        isFlying = rand.Next(3) == 0;
                    }
                    else
                    {
                        isFlying = rand.Next(6) == 0;
                    }
                }
            }

            int flyBatch = 1;
            if (isFlying)
            {
                if (flySpawnCount >= 10)
                {
                    isFlying = false;
                }
                else
                {
                    flyBatch = rand.Next(1, 4);
                    flySpawnCount++;
                }
            }

            int spawnAmount = isFlying ? flyBatch : rand.Next(1, 4);

            int lastX = obstacles.Count > 0
                ? obstacles.Max(o => o.Left + o.Width)
                : this.ClientSize.Width;

            int batchStartIndex = obstacles.Count;

            for (int i = 0; i < spawnAmount; i++)
            {
                Image img;
                if (isFlying)
                {
                    int index = rand.Next(flyingObstacleImages.Length);
                    img = flyingObstacleImages[index];
                }
                else
                {
                    int index = rand.Next(groundObstacleImages.Length);
                    img = groundObstacleImages[index];
                }

                PictureBox newObstacle = new PictureBox
                {
                    Size = img.Size,
                    Image = img,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                };

                if (isFlying)
                {
                    int flyHeight = rand.Next(40, 150);
                    newObstacle.Top = ground.Top - newObstacle.Height - flyHeight;
                    newObstacle.Tag = null; // Tạm thời để trống, lát sẽ set sau
                }
                else
                {
                    newObstacle.Top = ground.Top - newObstacle.Height;
                    newObstacle.Tag = null; // Ground obstacle không cần tag
                }


                int minSpacing = (int)(32 * obstacleSpeed);
                int maxSpacing = (int)(42 * obstacleSpeed);

                double progressFactor = Math.Min(1.0, gameProgress / 4000.0);
                int maxSpacingReduction = (int)(20 * progressFactor);
                maxSpacing = Math.Max(minSpacing + 20, maxSpacing - maxSpacingReduction);

                // Nếu vật trước là zigzag, nới rộng khoảng cách
                if (obstacles.Count > 0 && obstacles.Last().Tag is ObstacleTag previousTag && previousTag.Type == "zigzag")
                {
                    minSpacing += 50; // Nới rộng khoảng cách (thay đổi giá trị này tùy ý)
                    maxSpacing += 50;
                }

                // Kiểm tra nếu vật này là zigzag, đảm bảo khoảng cách với vật trước đó
                if (isFlying)
                {
                    minSpacing += 100;
                    maxSpacing += 200;
                }
                else
                {
                    if (spawnAmount > 1)
                    {
                        if (i == 0)
                        {
                            minSpacing = Math.Max(dino.Width + 40, (int)(25 * obstacleSpeed));
                            maxSpacing = (int)(40 * obstacleSpeed);
                        }
                        else
                        {
                            minSpacing = rand.Next(5, 12);
                            maxSpacing = rand.Next(10, 18);
                        }
                    }
                    else
                    {
                        minSpacing = Math.Max(dino.Width + 40, (int)(25 * obstacleSpeed));
                        maxSpacing = (int)(40 * obstacleSpeed);
                    }
                }

                if (maxSpacing <= minSpacing)
                {
                    maxSpacing = minSpacing + 20;
                }
                //recolor theo màu inverse hiện tại
                obstacle.Image = RecolorImage(obstacle.Image, inverseColor);

                newObstacle.Left = lastX + rand.Next(minSpacing, maxSpacing);
                this.Controls.Add(newObstacle);
                obstacles.Add(newObstacle);
                lastX = newObstacle.Left + newObstacle.Width;
            }
            // Sau khi spawn xong, chọn random 1 obstacle flying trong batch
            if (isFlying && spawnAmount > 1)
            {
                int randomIndex = rand.Next(batchStartIndex, batchStartIndex + spawnAmount);
                PictureBox candidate = obstacles[randomIndex];

                int flyDistance = ground.Top - (candidate.Top + candidate.Height);
                if (flyDistance >= 100)
                {
                    candidate.Tag = new ObstacleTag
                    {
                        Type = "zigzag",
                        OriginalTop = candidate.Top
                    };
                }
                else
                {
                    candidate.Tag = null;
                }
            }
        }
        public class ObstacleTag
        {
            public string Type { get; set; }    // Loại obstacle
            public int OriginalTop { get; set; } // Ghi nhớ vị trí spawn ban đầu
            public bool MovingDown { get; set; } = true; // Mặc định là đang rớt xuống
            public bool Initialized { get; set; } = false;
        }
        private void HandleObstacleMovement()
        {
            List<PictureBox> toRemove = new List<PictureBox>();

            foreach (var obs in obstacles)
            {
                // Di chuyển đối tượng luôn sang trái
                obs.Left -= obstacleSpeed;

                if (obs.Tag is ObstacleTag tag && tag.Type == "zigzag")
                {
                    int groundTop = ground.Top;
                    int originalTop = tag.OriginalTop;
                    int minY = originalTop;
                    int maxY = groundTop - obs.Height;

                    if (!tag.Initialized)
                    {
                        tag.MovingDown = true;
                        tag.Initialized = true;
                    }

                    // Điều chỉnh moveX và moveY sao cho di chuyển chéo
                    int moveX = 3;  //  Di chuyển nhanh ngang sang trái
                    int moveY = 1;  // Di chuyển chậm lên xuống tạo góc chéo

                    // Di chuyển ngang sang trái
                    obs.Left -= moveX;  // Di chuyển sang trái thay vì phải

                    if (tag.MovingDown)
                    {
                        obs.Top += moveY;  // Di chuyển xuống
                        if (obs.Top >= maxY) // Khi chạm đất, chuyển sang đi lên
                        {
                            tag.MovingDown = false;
                        }
                    }
                    else
                    {
                        obs.Top -= moveY;  // Di chuyển lên
                        if (obs.Top <= minY) // Khi đạt đỉnh, chuyển sang đi xuống
                        {
                            tag.MovingDown = true;
                        }
                    }
                }

                // Xóa object nếu nó ra ngoài màn hình
                if (obs.Left < -obs.Width)
                {
                    this.Controls.Remove(obs);
                    toRemove.Add(obs);
                }
            }

            foreach (var obs in toRemove)
            {
                obstacles.Remove(obs);
            }

            // Tạo thêm obstacle nếu chưa đủ
            if (obstacles.Count < 5)
            {
                SpawnObstacle();
            }
        }
        //Change color mechanic
        private void ChangeBackgroundColor()
        {
            originalBackgroundColor = this.BackColor;
            targetBackgroundColor = retroColors[rand.Next(retroColors.Length)];

            inverseColor = Color.FromArgb(255 - targetBackgroundColor.R, 255 - targetBackgroundColor.G, 255 - targetBackgroundColor.B);

            this.BackColor = targetBackgroundColor;
            ground.BackColor = inverseColor;

            // Flash effect
            isFlashing = true;
            flashElapsed = 0;
            flashTimer.Interval = 20;
            flashTimer.Start();

            // Recolor toàn bộ hình ảnh obstacles gốc
            for (int i = 0; i < groundObstacleImages.Length; i++)
            {
                groundObstacleImages[i] = RecolorImage(originalGroundObstacleImages[i], inverseColor);
            }
            for (int i = 0; i < flyingObstacleImages.Length; i++)
            {
                flyingObstacleImages[i] = RecolorImage(originalFlyingObstacleImages[i], inverseColor);
            }

            // Recolor các obstacles đang tồn tại
            foreach (var obs in obstacles)
            {
                obs.Image = RecolorImage(obs.Image, inverseColor);
            }
        }
        private Image RecolorImage(Image original, Color targetColor)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
            new float[] { 0, 0, 0, 0, 0 },
            new float[] { 0, 0, 0, 0, 0 },
            new float[] { 0, 0, 0, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { targetColor.R / 255f, targetColor.G / 255f, targetColor.B / 255f, 0, 1 }
                });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return newBitmap;
        }
//Flashing screen
        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            flashElapsed += flashTimer.Interval;

            if (flashElapsed < flashDuration)
            {
                this.BackColor = Color.White;
                return;
            }

            flashTimer.Stop();
            isFlashing = false;

            this.BackColor = targetBackgroundColor;
            ground.BackColor = inverseColor;

            // Recolor lại các obstacles đang tồn tại theo inverseColor
            foreach (var obs in obstacles)
            {
                obs.Image = RecolorImage(obs.Image, inverseColor);
            }
        }
    //Invincible mechanic flashing
        private void InvincibleFlashTimer_Tick(object sender, EventArgs e)
        {
            if (!isIFlashing) return;

            dino.Visible = !dino.Visible; // Lật trạng thái hiển thị

            flashCount++;
            if (flashCount >= maxFlashCount)
            {
                invincibleFlashTimer.Stop();
                isIFlashing = false;
                dino.Visible = true; // đảm bảo dino hiện lại
            }
        }
        //Game timer
        private void gameTimer_Tick_1(object sender, EventArgs e)
        {
            int elapsedTime = gameTimer.Interval;
            gameProgress++;
            HandleJumping();
            HandleObstacleMovement();
            CheckCollision();
            AnimateDino(elapsedTime);
            UpdateScore();
        }
        //Va cham
        private void CheckCollision()
        {
            if (isInvincible || gameOver) return;

            foreach (var obs in obstacles)
            {
                if (dino.Bounds.IntersectsWith(obs.Bounds))
                {
                    lives--;

                    labelLives.Text = "♥ x " + lives;

                    if (lives <= 0)
                    {
                        gameOver = true;
                        gameTimer.Stop();

                        dino.Height = dinoOriginalHeight;
                        dino.Width = dinoOriginalWidth;
                        dino.Top = ground.Top - dino.Height;
                        dino.Image = dinoDeadFrame;
                        return;
                    }

                    // Bật chế độ bất tử
                    isInvincible = true;
                    invincibleTimer.Start();
                    isIFlashing = true;
                    flashCount = 0;
                    invincibleFlashTimer.Start();

                    return;
                }
            }
        }
        //Key movement read
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (gameOver)
                {
                    ResetGame();
                }
                else if (isGrounded && !isJumping)
                {
                    isJumping = true;
                    isGrounded = false;
                    jumpVelocity = jumpPower;
                }
            }

            // Prevent sliding from affecting other states when in the air
            if (!isGrounded) sliding = false;

            if (e.KeyCode == Keys.Down && isGrounded)
            {
                sliding = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                sliding = false;
                dino.Height = dinoOriginalHeight;
                dino.Width = dinoOriginalWidth;

                if (isGrounded)
                {
                    dino.Top = ground.Top - dino.Height;
                }
            }
        }
//Load Dino
        private void LoadDinoFrames()
        {
            originalDinoWalkFrames = new Image[]
            {
        Properties.Resources.Walk1R,
        Properties.Resources.Blink1R,
        Properties.Resources.Walk2R,
        Properties.Resources.Blink2R
            };
            dinoWalkFrames = (Image[])originalDinoWalkFrames.Clone();

            originalDinoJumpFrame = Properties.Resources.JumpR;
            dinoJumpFrame = originalDinoJumpFrame;

            originalDinoFallFrame = Properties.Resources.FallR;
            dinoFallFrame = originalDinoFallFrame;

            originalDinoDeadFrame = Properties.Resources.cryR;
            dinoDeadFrame = originalDinoDeadFrame;

            originalDinoSlideFrame = new Image[]
 {
    Properties.Resources.slideR,
    Properties.Resources.slideL
 };
            
        }
        //Animation
        private void AnimateDino(int elapsedTime)
        {
            if (gameOver)
            {
                dino.Image = dinoDeadFrame;
                dino.Height = dinoOriginalHeight;
                dino.Width = dinoOriginalWidth;
                dino.Top = ground.Top - dino.Height;
                return;
            }
            // Handle sliding 
            if (sliding && isGrounded)
            {
                slideFrameTimer += elapsedTime;
                if (slideFrameTimer >= slideFrameDurations[slideFrameIndex])
                {
                    slideFrameTimer = 0;
                    slideFrameIndex = (slideFrameIndex + 1) % originalDinoSlideFrame.Length;
                }

                dino.Image = originalDinoSlideFrame[slideFrameIndex];
                dino.Height = 24;
                dino.Width = 60;
                dino.Top = ground.Top - dino.Height;

                return;
            }
            // Only update height and width when grounded and not jumping
            if (isGrounded && !isJumping && !sliding)
            {
                dino.Height = dinoOriginalHeight;
                dino.Width = dinoOriginalWidth;
                dino.Top = ground.Top - dino.Height; // Keep Dino on the ground while walking
            }

            // Jumping/Falling Frames
            if (isJumping || !isGrounded)
            {
                dino.Image = (jumpVelocity < 0) ? dinoJumpFrame : dinoFallFrame;
                return;
            }

            // Walking animation
            walkFrameTimer += elapsedTime;
            if (walkFrameTimer >= walkFrameDurations[walkFrameIndex])
            {
                walkFrameTimer = 0;
                walkFrameIndex = (walkFrameIndex + 1) % dinoWalkFrames.Length;
            }
            dino.Image = dinoWalkFrames[walkFrameIndex];
        }
        private void HandleJumping()
        {
            if (!isGrounded) // Only apply jump logic if Dino is not grounded
            {
                dino.Top += jumpVelocity;
                jumpVelocity += gravity;
            }

            // Use a buffer threshold for detecting the ground
            if (dino.Top + dino.Height >= ground.Top - 1)
            {
                dino.Top = ground.Top - dino.Height;
                jumpVelocity = 0;
                if (!isGrounded)
                {
                    isGrounded = true;
                    isJumping = false;
                    walkFrameIndex = 0;
                    walkFrameTimer = 0;
                }
            }
            else
            {
                isGrounded = false;
            }
        }
    }
}
