namespace DataSyncProject
{
    partial class Main
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstBackupLoc = new System.Windows.Forms.ListBox();
            this.btnAddBackup = new System.Windows.Forms.Button();
            this.btnChgBackup = new System.Windows.Forms.Button();
            this.btnDelBackup = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.TreeView();
            this.lstWorkSpaces = new System.Windows.Forms.ListBox();
            this.lstWorkSpaceSave = new System.Windows.Forms.TreeView();
            this.btnDelWorkSpace = new System.Windows.Forms.Button();
            this.btnChgWorkSpace = new System.Windows.Forms.Button();
            this.btnAddWorkSpace = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.pgbSync = new System.Windows.Forms.ProgressBar();
            this.btnForcedHome = new System.Windows.Forms.Button();
            this.btnForcedBackUp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menu1 = new System.Windows.Forms.MenuStrip();
            this.fichierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstBackupLoc
            // 
            this.lstBackupLoc.FormattingEnabled = true;
            this.lstBackupLoc.Location = new System.Drawing.Point(12, 442);
            this.lstBackupLoc.Name = "lstBackupLoc";
            this.lstBackupLoc.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstBackupLoc.Size = new System.Drawing.Size(393, 108);
            this.lstBackupLoc.Sorted = true;
            this.lstBackupLoc.TabIndex = 0;
            this.lstBackupLoc.SelectedIndexChanged += new System.EventHandler(this.lstBackupLoc_SelectedIndexChanged);
            // 
            // btnAddBackup
            // 
            this.btnAddBackup.Location = new System.Drawing.Point(6, 29);
            this.btnAddBackup.Name = "btnAddBackup";
            this.btnAddBackup.Size = new System.Drawing.Size(119, 23);
            this.btnAddBackup.TabIndex = 3;
            this.btnAddBackup.Text = "Ajouter un dossier";
            this.btnAddBackup.UseVisualStyleBackColor = true;
            this.btnAddBackup.Click += new System.EventHandler(this.btnAddBackup_Click);
            // 
            // btnChgBackup
            // 
            this.btnChgBackup.Enabled = false;
            this.btnChgBackup.Location = new System.Drawing.Point(6, 58);
            this.btnChgBackup.Name = "btnChgBackup";
            this.btnChgBackup.Size = new System.Drawing.Size(119, 23);
            this.btnChgBackup.TabIndex = 4;
            this.btnChgBackup.Text = "Changer un dossier";
            this.btnChgBackup.UseVisualStyleBackColor = true;
            this.btnChgBackup.Click += new System.EventHandler(this.btnChgBackup_Click);
            // 
            // btnDelBackup
            // 
            this.btnDelBackup.Enabled = false;
            this.btnDelBackup.Location = new System.Drawing.Point(6, 87);
            this.btnDelBackup.Name = "btnDelBackup";
            this.btnDelBackup.Size = new System.Drawing.Size(119, 23);
            this.btnDelBackup.TabIndex = 5;
            this.btnDelBackup.Text = "Retirer un dossier";
            this.btnDelBackup.UseVisualStyleBackColor = true;
            this.btnDelBackup.Click += new System.EventHandler(this.btnDelBackup_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.Location = new System.Drawing.Point(12, 38);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(393, 385);
            this.lstFiles.TabIndex = 6;
            // 
            // lstWorkSpaces
            // 
            this.lstWorkSpaces.FormattingEnabled = true;
            this.lstWorkSpaces.Location = new System.Drawing.Point(411, 38);
            this.lstWorkSpaces.MultiColumn = true;
            this.lstWorkSpaces.Name = "lstWorkSpaces";
            this.lstWorkSpaces.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstWorkSpaces.Size = new System.Drawing.Size(361, 95);
            this.lstWorkSpaces.Sorted = true;
            this.lstWorkSpaces.TabIndex = 7;
            this.lstWorkSpaces.SelectedIndexChanged += new System.EventHandler(this.lstWorkSpaces_SelectedIndexChanged);
            // 
            // lstWorkSpaceSave
            // 
            this.lstWorkSpaceSave.CheckBoxes = true;
            this.lstWorkSpaceSave.Location = new System.Drawing.Point(411, 168);
            this.lstWorkSpaceSave.Name = "lstWorkSpaceSave";
            this.lstWorkSpaceSave.Size = new System.Drawing.Size(195, 255);
            this.lstWorkSpaceSave.TabIndex = 8;
            this.lstWorkSpaceSave.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.lstWorkSpaceSave_AfterCheck);
            this.lstWorkSpaceSave.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.lstWorkSpaceSave_NodeMouseClick);
            // 
            // btnDelWorkSpace
            // 
            this.btnDelWorkSpace.Enabled = false;
            this.btnDelWorkSpace.Location = new System.Drawing.Point(12, 78);
            this.btnDelWorkSpace.Name = "btnDelWorkSpace";
            this.btnDelWorkSpace.Size = new System.Drawing.Size(133, 23);
            this.btnDelWorkSpace.TabIndex = 11;
            this.btnDelWorkSpace.Text = "Retirer un dossier";
            this.btnDelWorkSpace.UseVisualStyleBackColor = true;
            this.btnDelWorkSpace.Click += new System.EventHandler(this.btnDelWorkSpace_Click);
            // 
            // btnChgWorkSpace
            // 
            this.btnChgWorkSpace.Enabled = false;
            this.btnChgWorkSpace.Location = new System.Drawing.Point(12, 49);
            this.btnChgWorkSpace.Name = "btnChgWorkSpace";
            this.btnChgWorkSpace.Size = new System.Drawing.Size(133, 23);
            this.btnChgWorkSpace.TabIndex = 10;
            this.btnChgWorkSpace.Text = "Changer un dossier";
            this.btnChgWorkSpace.UseVisualStyleBackColor = true;
            this.btnChgWorkSpace.Click += new System.EventHandler(this.btnChgWorkSpace_Click);
            // 
            // btnAddWorkSpace
            // 
            this.btnAddWorkSpace.Location = new System.Drawing.Point(12, 20);
            this.btnAddWorkSpace.Name = "btnAddWorkSpace";
            this.btnAddWorkSpace.Size = new System.Drawing.Size(133, 23);
            this.btnAddWorkSpace.TabIndex = 9;
            this.btnAddWorkSpace.Text = "Ajouter un dossier";
            this.btnAddWorkSpace.UseVisualStyleBackColor = true;
            this.btnAddWorkSpace.Click += new System.EventHandler(this.btnAddWorkSpace_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Fichiers de la station";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(408, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Dossiers de la Station";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(408, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Fichiers à garder de la Station";
            // 
            // btnSync
            // 
            this.btnSync.Enabled = false;
            this.btnSync.Location = new System.Drawing.Point(617, 457);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(89, 23);
            this.btnSync.TabIndex = 13;
            this.btnSync.Text = "Synchroniser";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // pgbSync
            // 
            this.pgbSync.Location = new System.Drawing.Point(551, 427);
            this.pgbSync.Name = "pgbSync";
            this.pgbSync.Size = new System.Drawing.Size(227, 23);
            this.pgbSync.TabIndex = 14;
            // 
            // btnForcedHome
            // 
            this.btnForcedHome.Enabled = false;
            this.btnForcedHome.Location = new System.Drawing.Point(594, 505);
            this.btnForcedHome.Name = "btnForcedHome";
            this.btnForcedHome.Size = new System.Drawing.Size(135, 23);
            this.btnForcedHome.TabIndex = 13;
            this.btnForcedHome.Text = "Recupérer la sauvegarde";
            this.btnForcedHome.UseVisualStyleBackColor = true;
            // 
            // btnForcedBackUp
            // 
            this.btnForcedBackUp.Enabled = false;
            this.btnForcedBackUp.Location = new System.Drawing.Point(594, 481);
            this.btnForcedBackUp.Name = "btnForcedBackUp";
            this.btnForcedBackUp.Size = new System.Drawing.Size(135, 23);
            this.btnForcedBackUp.TabIndex = 13;
            this.btnForcedBackUp.Text = "Effectuer la sauvegarde";
            this.btnForcedBackUp.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDelWorkSpace);
            this.groupBox1.Controls.Add(this.btnChgWorkSpace);
            this.groupBox1.Controls.Add(this.btnAddWorkSpace);
            this.groupBox1.Location = new System.Drawing.Point(612, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(159, 113);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Actions de Station";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDelBackup);
            this.groupBox2.Controls.Add(this.btnChgBackup);
            this.groupBox2.Controls.Add(this.btnAddBackup);
            this.groupBox2.Location = new System.Drawing.Point(410, 429);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(135, 120);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Actions des Sauvegardes";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 426);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(196, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Liste des emplacements de Sauvegarde";
            // 
            // menu1
            // 
            this.menu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichierToolStripMenuItem});
            this.menu1.Location = new System.Drawing.Point(0, 0);
            this.menu1.Name = "menu1";
            this.menu1.Size = new System.Drawing.Size(784, 24);
            this.menu1.TabIndex = 18;
            this.menu1.Text = "menu1";
            // 
            // fichierToolStripMenuItem
            // 
            this.fichierToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newConfig,
            this.saveConfig,
            this.toolStripMenuItem1});
            this.fichierToolStripMenuItem.Name = "fichierToolStripMenuItem";
            this.fichierToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.fichierToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.fichierToolStripMenuItem.ShowShortcutKeys = false;
            this.fichierToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.fichierToolStripMenuItem.Text = "S&tations";
            // 
            // newConfig
            // 
            this.newConfig.Name = "newConfig";
            this.newConfig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newConfig.Size = new System.Drawing.Size(257, 22);
            this.newConfig.Text = "&Nouvelle configuration";
            this.newConfig.Click += new System.EventHandler(this.newConfig_Click);
            // 
            // saveConfig
            // 
            this.saveConfig.Enabled = false;
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveConfig.Size = new System.Drawing.Size(257, 22);
            this.saveConfig.Text = "Enregi&strer la configuration";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(254, 6);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pgbSync);
            this.Controls.Add(this.btnForcedBackUp);
            this.Controls.Add(this.btnForcedHome);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstWorkSpaceSave);
            this.Controls.Add(this.lstWorkSpaces);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.lstBackupLoc);
            this.Controls.Add(this.menu1);
            this.MainMenuStrip = this.menu1;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Main";
            this.Text = "Système de Sauvegarde";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.menu1.ResumeLayout(false);
            this.menu1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstBackupLoc;
        private System.Windows.Forms.Button btnAddBackup;
        private System.Windows.Forms.Button btnChgBackup;
        private System.Windows.Forms.Button btnDelBackup;
        private System.Windows.Forms.TreeView lstFiles;
        private System.Windows.Forms.ListBox lstWorkSpaces;
        private System.Windows.Forms.TreeView lstWorkSpaceSave;
        private System.Windows.Forms.Button btnDelWorkSpace;
        private System.Windows.Forms.Button btnChgWorkSpace;
        private System.Windows.Forms.Button btnAddWorkSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ProgressBar pgbSync;
        private System.Windows.Forms.Button btnForcedHome;
        private System.Windows.Forms.Button btnForcedBackUp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menu1;
        private System.Windows.Forms.ToolStripMenuItem fichierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newConfig;
        private System.Windows.Forms.ToolStripMenuItem saveConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}

