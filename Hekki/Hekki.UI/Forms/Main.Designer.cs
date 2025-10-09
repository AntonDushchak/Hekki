namespace Hekki
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            Cherkasy = new Button();
            EveryOnEvery = new Button();
            numbersOfKarts = new RichTextBox();
            versionNumber = new Label();
            SprintOld = new Button();
            TestNew = new Button();
            SprintNew = new Button();
            SuspendLayout();
            // 
            // Cherkasy
            // 
            Cherkasy.Location = new Point(61, 61);
            Cherkasy.Name = "Cherkasy";
            Cherkasy.Size = new Size(204, 86);
            Cherkasy.TabIndex = 0;
            Cherkasy.Text = "Cherkasy";
            Cherkasy.UseVisualStyleBackColor = true;
            Cherkasy.Click += Cherkasy_Click;
            // 
            // EveryOnEvery
            // 
            EveryOnEvery.Location = new Point(637, 236);
            EveryOnEvery.Name = "EveryOnEvery";
            EveryOnEvery.Size = new Size(204, 86);
            EveryOnEvery.TabIndex = 4;
            EveryOnEvery.Text = "Every On Every";
            EveryOnEvery.UseVisualStyleBackColor = true;
            EveryOnEvery.Click += EveryOnEvery_Click;
            // 
            // numbersOfKarts
            // 
            numbersOfKarts.Location = new Point(61, 350);
            numbersOfKarts.Name = "numbersOfKarts";
            numbersOfKarts.Size = new Size(125, 162);
            numbersOfKarts.TabIndex = 15;
            numbersOfKarts.Text = "11\n12\n13\n14\n15\n16\n17\n18\n19\n20";
            numbersOfKarts.TextChanged += numbersOfKarts_TextChanged;
            numbersOfKarts.KeyPress += numbersOfKarts_KeyPress;
            // 
            // versionNumber
            // 
            versionNumber.AutoSize = true;
            versionNumber.Location = new Point(845, 20);
            versionNumber.Margin = new Padding(2, 0, 2, 0);
            versionNumber.Name = "versionNumber";
            versionNumber.Size = new Size(59, 20);
            versionNumber.TabIndex = 16;
            versionNumber.Text = "Версия";
            // 
            // SprintOld
            // 
            SprintOld.Location = new Point(344, 61);
            SprintOld.Name = "SprintOld";
            SprintOld.Size = new Size(204, 86);
            SprintOld.TabIndex = 17;
            SprintOld.Text = "SprintOld";
            SprintOld.UseVisualStyleBackColor = true;
            SprintOld.Click += SprintOld_Click;
            // 
            // TestNew
            // 
            TestNew.Location = new Point(344, 236);
            TestNew.Name = "TestNew";
            TestNew.Size = new Size(204, 86);
            TestNew.TabIndex = 18;
            TestNew.Text = "TestNew";
            TestNew.UseVisualStyleBackColor = true;
            TestNew.Click += TestNew_Click;
            // 
            // SprintNew
            // 
            SprintNew.Location = new Point(637, 61);
            SprintNew.Name = "SprintNew";
            SprintNew.Size = new Size(204, 86);
            SprintNew.TabIndex = 19;
            SprintNew.Text = "SprintNew";
            SprintNew.UseVisualStyleBackColor = true;
            SprintNew.Click += SprintNew_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 524);
            Controls.Add(SprintNew);
            Controls.Add(TestNew);
            Controls.Add(SprintOld);
            Controls.Add(versionNumber);
            Controls.Add(numbersOfKarts);
            Controls.Add(EveryOnEvery);
            Controls.Add(Cherkasy);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Hekki";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Cherkasy;
        private Button EveryOnEvery;
        private RichTextBox numbersOfKarts;
        private Label versionNumber;
        private Button SprintOld;
        private Button TestNew;
        private Button SprintNew;
    }
}