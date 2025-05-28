using System.ComponentModel;

namespace Frame;

partial class AudioTestForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        txtNormal = new System.Windows.Forms.TextBox();
        lblNormal = new System.Windows.Forms.Label();
        btnNormalTask = new System.Windows.Forms.Button();
        btnInsert = new System.Windows.Forms.Button();
        txtInsert = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        lblPlayStatus = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        btnPlay = new System.Windows.Forms.Button();
        btnPause = new System.Windows.Forms.Button();
        btnStop = new System.Windows.Forms.Button();
        btnClearToPlay = new System.Windows.Forms.Button();
        trackBar1 = new System.Windows.Forms.TrackBar();
        combSoundCard = new System.Windows.Forms.ComboBox();
        ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
        SuspendLayout();
        // 
        // txtNormal
        // 
        txtNormal.Location = new System.Drawing.Point(27, 67);
        txtNormal.Multiline = true;
        txtNormal.Name = "txtNormal";
        txtNormal.Size = new System.Drawing.Size(535, 134);
        txtNormal.TabIndex = 0;
        txtNormal.Text = "C:\\output\\2-15773ms.mp3\r\nC:\\output\\10-78798ms.mp3\r\nC:\\output\\7-11568ms.mp3";
        // 
        // lblNormal
        // 
        lblNormal.Location = new System.Drawing.Point(27, 41);
        lblNormal.Name = "lblNormal";
        lblNormal.Size = new System.Drawing.Size(100, 23);
        lblNormal.TabIndex = 2;
        lblNormal.Text = "常规任务";
        // 
        // btnNormalTask
        // 
        btnNormalTask.Location = new System.Drawing.Point(369, 36);
        btnNormalTask.Name = "btnNormalTask";
        btnNormalTask.Size = new System.Drawing.Size(75, 25);
        btnNormalTask.TabIndex = 3;
        btnNormalTask.Text = "加入任务";
        btnNormalTask.UseVisualStyleBackColor = true;
        btnNormalTask.Click += btnNormalTask_Click;
        // 
        // btnInsert
        // 
        btnInsert.Location = new System.Drawing.Point(487, 207);
        btnInsert.Name = "btnInsert";
        btnInsert.Size = new System.Drawing.Size(75, 25);
        btnInsert.TabIndex = 6;
        btnInsert.Text = "加入任务";
        btnInsert.UseVisualStyleBackColor = true;
        btnInsert.Click += btnInsert_Click;
        // 
        // txtInsert
        // 
        txtInsert.Location = new System.Drawing.Point(27, 238);
        txtInsert.Multiline = true;
        txtInsert.Name = "txtInsert";
        txtInsert.Size = new System.Drawing.Size(535, 110);
        txtInsert.TabIndex = 4;
        txtInsert.Text = ("C:\\output\\3-25028ms.mp3\r\nC:\\output\\5-23695ms.mp3\r\nC:\\output\\1-23929ms.mp3\r\nC:\\out" + "put\\2-10286ms.mp3");
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(27, 351);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(46, 21);
        label1.TabIndex = 7;
        label1.Text = "状态";
        // 
        // lblPlayStatus
        // 
        lblPlayStatus.Location = new System.Drawing.Point(81, 351);
        lblPlayStatus.Name = "lblPlayStatus";
        lblPlayStatus.Size = new System.Drawing.Size(481, 90);
        lblPlayStatus.TabIndex = 8;
        lblPlayStatus.Text = "正在播放";
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(27, 212);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(100, 23);
        label2.TabIndex = 9;
        label2.Text = "插队任务";
        // 
        // btnPlay
        // 
        btnPlay.Location = new System.Drawing.Point(27, 1);
        btnPlay.Name = "btnPlay";
        btnPlay.Size = new System.Drawing.Size(75, 25);
        btnPlay.TabIndex = 10;
        btnPlay.Text = "播放";
        btnPlay.UseVisualStyleBackColor = true;
        btnPlay.Click += btnPlay_Click;
        // 
        // btnPause
        // 
        btnPause.Location = new System.Drawing.Point(108, 1);
        btnPause.Name = "btnPause";
        btnPause.Size = new System.Drawing.Size(75, 25);
        btnPause.TabIndex = 11;
        btnPause.Text = "暂停";
        btnPause.UseVisualStyleBackColor = true;
        btnPause.Click += btnPause_Click;
        // 
        // btnStop
        // 
        btnStop.Location = new System.Drawing.Point(189, 1);
        btnStop.Name = "btnStop";
        btnStop.Size = new System.Drawing.Size(75, 25);
        btnStop.TabIndex = 12;
        btnStop.Text = "停止";
        btnStop.UseVisualStyleBackColor = true;
        btnStop.Click += btnStop_Click;
        // 
        // btnClearToPlay
        // 
        btnClearToPlay.Location = new System.Drawing.Point(450, 36);
        btnClearToPlay.Name = "btnClearToPlay";
        btnClearToPlay.Size = new System.Drawing.Size(114, 25);
        btnClearToPlay.TabIndex = 13;
        btnClearToPlay.Text = "停止并播放新任务";
        btnClearToPlay.UseVisualStyleBackColor = true;
        btnClearToPlay.Click += btnClearToPlay_Click;
        // 
        // trackBar1
        // 
        trackBar1.Location = new System.Drawing.Point(369, 385);
        trackBar1.Maximum = 100;
        trackBar1.Name = "trackBar1";
        trackBar1.Size = new System.Drawing.Size(193, 45);
        trackBar1.TabIndex = 14;
        trackBar1.Scroll += trackBar1_Scroll;
        // 
        // combSoundCard
        // 
        combSoundCard.FormattingEnabled = true;
        combSoundCard.Location = new System.Drawing.Point(369, 354);
        combSoundCard.Name = "combSoundCard";
        combSoundCard.Size = new System.Drawing.Size(193, 25);
        combSoundCard.TabIndex = 15;
        // 
        // AudioTestForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(591, 450);
        Controls.Add(combSoundCard);
        Controls.Add(trackBar1);
        Controls.Add(btnClearToPlay);
        Controls.Add(btnStop);
        Controls.Add(btnPause);
        Controls.Add(btnPlay);
        Controls.Add(label2);
        Controls.Add(lblPlayStatus);
        Controls.Add(label1);
        Controls.Add(btnInsert);
        Controls.Add(txtInsert);
        Controls.Add(btnNormalTask);
        Controls.Add(lblNormal);
        Controls.Add(txtNormal);
        Text = "语音播放测试";
        ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.ComboBox combSoundCard;

    private System.Windows.Forms.TrackBar trackBar1;

    private System.Windows.Forms.Button btnClearToPlay;

    private System.Windows.Forms.Button btnPause;
    private System.Windows.Forms.Button btnStop;

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.Button btnInsert;

    private System.Windows.Forms.Label lblNormal;
    private System.Windows.Forms.Button btnNormalTask;
    private System.Windows.Forms.Label lblPlayStatus;

    private System.Windows.Forms.TextBox txtNormal;
    private System.Windows.Forms.TextBox txtInsert;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnPlay;

    #endregion
}