using System.ComponentModel;

namespace Frame;

partial class InitializeForm
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
        progressBar1 = new System.Windows.Forms.ProgressBar();
        btnSkip = new System.Windows.Forms.Button();
        lblFileName = new System.Windows.Forms.Label();
        lblTime = new System.Windows.Forms.Label();
        SuspendLayout();
        // 
        // progressBar1
        // 
        progressBar1.Location = new System.Drawing.Point(23, 44);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new System.Drawing.Size(475, 40);
        progressBar1.TabIndex = 0;
        // 
        // btnSkip
        // 
        btnSkip.Location = new System.Drawing.Point(224, 109);
        btnSkip.Name = "btnSkip";
        btnSkip.Size = new System.Drawing.Size(82, 31);
        btnSkip.TabIndex = 1;
        btnSkip.Text = "跳过";
        btnSkip.UseVisualStyleBackColor = true;
        btnSkip.Click += button1_Click;
        // 
        // lblFileName
        // 
        lblFileName.Location = new System.Drawing.Point(98, 18);
        lblFileName.Name = "lblFileName";
        lblFileName.Size = new System.Drawing.Size(400, 23);
        lblFileName.TabIndex = 2;
        // 
        // lblTime
        // 
        lblTime.Location = new System.Drawing.Point(23, 18);
        lblTime.Name = "lblTime";
        lblTime.Size = new System.Drawing.Size(91, 23);
        lblTime.TabIndex = 3;
        lblTime.Text = "用时0秒";
        // 
        // InitializeForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(512, 177);
        Controls.Add(lblTime);
        Controls.Add(lblFileName);
        Controls.Add(btnSkip);
        Controls.Add(progressBar1);
        Text = "初始化";
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label lblFileName;
    private System.Windows.Forms.Label lblTime;

    private System.Windows.Forms.Button btnSkip;

    private System.Windows.Forms.ProgressBar progressBar1;

    #endregion
}