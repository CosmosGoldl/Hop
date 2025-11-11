namespace Dinogame__alpha_
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dino = new System.Windows.Forms.PictureBox();
            this.obstacle = new System.Windows.Forms.PictureBox();
            this.ground = new System.Windows.Forms.PictureBox();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.labelScore = new System.Windows.Forms.Label();
            this.labelLives = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dino)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.obstacle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ground)).BeginInit();
            this.SuspendLayout();
            // 
            // dino
            // 
            this.dino.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.dino.Image = global::Dinogame__alpha_.Properties.Resources.Walk2R;
            this.dino.Location = new System.Drawing.Point(119, 129);
            this.dino.Name = "dino";
            this.dino.Size = new System.Drawing.Size(60, 50);
            this.dino.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dino.TabIndex = 0;
            this.dino.TabStop = false;
            // 
            // obstacle
            // 
            this.obstacle.Image = global::Dinogame__alpha_.Properties.Resources.Spike1;
            this.obstacle.Location = new System.Drawing.Point(1063, 129);
            this.obstacle.Name = "obstacle";
            this.obstacle.Size = new System.Drawing.Size(21, 64);
            this.obstacle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.obstacle.TabIndex = 1;
            this.obstacle.TabStop = false;
            this.obstacle.Visible = false;
            // 
            // ground
            // 
            this.ground.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ground.Location = new System.Drawing.Point(-5, 186);
            this.ground.Name = "ground";
            this.ground.Size = new System.Drawing.Size(1099, 59);
            this.ground.TabIndex = 2;
            this.ground.TabStop = false;
            // 
            // gameTimer
            // 
            this.gameTimer.Enabled = true;
            this.gameTimer.Interval = 20;
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick_1);
            // 
            // labelScore
            // 
            this.labelScore.AutoSize = true;
            this.labelScore.Font = new System.Drawing.Font("Press Start 2P", 12F);
            this.labelScore.Location = new System.Drawing.Point(307, 9);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(132, 27);
            this.labelScore.TabIndex = 3;
            this.labelScore.Text = "Score:";
            // 
            // labelLives
            // 
            this.labelLives.AutoSize = true;
            this.labelLives.Font = new System.Drawing.Font("Press Start 2P", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLives.ForeColor = System.Drawing.Color.Black;
            this.labelLives.Location = new System.Drawing.Point(641, 9);
            this.labelLives.Name = "labelLives";
            this.labelLives.Size = new System.Drawing.Size(112, 27);
            this.labelLives.TabIndex = 4;
            this.labelLives.Text = "♥ x 2";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1084, 241);
            this.Controls.Add(this.labelLives);
            this.Controls.Add(this.labelScore);
            this.Controls.Add(this.ground);
            this.Controls.Add(this.obstacle);
            this.Controls.Add(this.dino);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.dino)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.obstacle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



        private System.Windows.Forms.PictureBox dino;
        private System.Windows.Forms.PictureBox obstacle;
        private System.Windows.Forms.PictureBox ground;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.Label labelLives;
    }
}

