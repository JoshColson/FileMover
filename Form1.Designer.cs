using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows.Forms;
using RadioButton = System.Windows.Forms.RadioButton;
using Label = System.Windows.Forms.Label;


namespace FileMover
{
    public partial class MyForm : Form
    {
        // The label to display the result
        private Label sourceResultLabel;

        // The buttons to trigger the actions
        private Button sourceBrowserButton;
        private Button destinationBrowserButton;
        private string selectedSourceFolderPath;
        private string selectedDestinationFolderPath;
        private int maxPathLength = 35;
        private string selectedValue;
        private RadioSelection radioSelection = RadioSelection.MoveWithoutOverwrite;


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyForm));
            introLabel = new Label();
            sourceBrowserButton = new Button();
            destinationBrowserButton = new Button();
            sourceResultLabel = new Label();
            moveButton = new Button();
            resetButton = new Button();
            destinationResultLabel = new Label();
            progressBar = new ProgressBar();
            selection3 = new RadioButton();
            selection2 = new RadioButton();
            selection1 = new RadioButton();
            SuspendLayout();
            // 
            // introLabel
            // 
            introLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            introLabel.Location = new Point(10, 10);
            introLabel.Name = "introLabel";
            introLabel.Size = new Size(363, 129);
            introLabel.TabIndex = 0;
            introLabel.Text = resources.GetString("introLabel.Text");
            introLabel.TextAlign = ContentAlignment.MiddleLeft;
            introLabel.Click += introLabel_Click;
            // 
            // sourceBrowserButton
            // 
            sourceBrowserButton.Location = new Point(10, 141);
            sourceBrowserButton.Name = "sourceBrowserButton";
            sourceBrowserButton.Size = new Size(132, 23);
            sourceBrowserButton.TabIndex = 1;
            sourceBrowserButton.Text = "Source Location";
            sourceBrowserButton.Click += SourceButtonClick;
            // 
            // destinationBrowserButton
            // 
            destinationBrowserButton.Location = new Point(10, 171);
            destinationBrowserButton.Name = "destinationBrowserButton";
            destinationBrowserButton.Size = new Size(132, 23);
            destinationBrowserButton.TabIndex = 2;
            destinationBrowserButton.Text = "Destination Location";
            destinationBrowserButton.Click += DestinationButtonClick;
            // 
            // sourceResultLabel
            // 
            sourceResultLabel.Location = new Point(149, 145);
            sourceResultLabel.Name = "sourceResultLabel";
            sourceResultLabel.Size = new Size(223, 15);
            sourceResultLabel.TabIndex = 3;
            // 
            // moveButton
            // 
            moveButton.Enabled = false;
            moveButton.Location = new Point(222, 230);
            moveButton.Name = "moveButton";
            moveButton.Size = new Size(75, 23);
            moveButton.TabIndex = 4;
            moveButton.Text = "Move";
            moveButton.Click += MoveButton_Click;
            // 
            // resetButton
            // 
            resetButton.Enabled = false;
            resetButton.Location = new Point(302, 230);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(75, 23);
            resetButton.TabIndex = 5;
            resetButton.Text = "Reset";
            resetButton.Click += ResetButton_Click;
            // 
            // destinationResultLabel
            // 
            destinationResultLabel.ForeColor = SystemColors.ControlText;
            destinationResultLabel.Location = new Point(149, 175);
            destinationResultLabel.Name = "destinationResultLabel";
            destinationResultLabel.Size = new Size(223, 15);
            destinationResultLabel.TabIndex = 3;
            destinationResultLabel.Click += destinationResultLabel_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(222, 208);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(155, 12);
            progressBar.TabIndex = 6;
            progressBar.Visible = false;
            // 
            // selection3
            // 
            selection3.AutoSize = true;
            selection3.Location = new Point(10, 237);
            selection3.Name = "selection3";
            selection3.Size = new Size(174, 19);
            selection3.TabIndex = 7;
            selection3.Text = "Move identical with cleanup";
            selection3.UseVisualStyleBackColor = true;
            selection3.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // selection2
            // 
            selection2.AutoSize = true;
            selection2.Location = new Point(10, 218);
            selection2.Name = "selection2";
            selection2.Size = new Size(137, 19);
            selection2.TabIndex = 8;
            selection2.Text = "Move and keep latest";
            selection2.UseVisualStyleBackColor = true;
            selection2.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // selection1
            // 
            selection1.AutoSize = true;
            selection1.Checked = true;
            selection1.Location = new Point(10, 199);
            selection1.Name = "selection1";
            selection1.Size = new Size(151, 19);
            selection1.TabIndex = 9;
            selection1.TabStop = true;
            selection1.Text = "Move without overwrite";
            selection1.UseVisualStyleBackColor = true;
            selection1.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // MyForm
            // 
            ClientSize = new Size(384, 261);
            Controls.Add(selection3);
            Controls.Add(selection2);
            Controls.Add(selection1);
            Controls.Add(progressBar);
            Controls.Add(introLabel);
            Controls.Add(sourceBrowserButton);
            Controls.Add(destinationBrowserButton);
            Controls.Add(destinationResultLabel);
            Controls.Add(sourceResultLabel);
            Controls.Add(moveButton);
            Controls.Add(resetButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "MyForm";
            Padding = new Padding(10);
            Text = "File Mover";
            Load += MyForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (selection1.Checked)
                radioSelection = RadioSelection.MoveWithoutOverwrite;
            else if (selection2.Checked)
                radioSelection = RadioSelection.MoveKeepLatest;
            else if (selection3.Checked)
                radioSelection = RadioSelection.MoveIdenticalWithCleanup;
        }

        private void SourceButtonClick(object sender, EventArgs e)
        {
            var returnPath = FolderDialog();

            if (returnPath != null)
            {
                selectedSourceFolderPath = returnPath;
                EnableMoveButton();
                EnableResetButton();


                if (selectedSourceFolderPath.Length > maxPathLength)
                {
                    int charactersToRemove = selectedSourceFolderPath.Length - maxPathLength;
                    sourceResultLabel.Text = "..." + selectedSourceFolderPath.Substring(charactersToRemove);
                }
                else
                {
                    sourceResultLabel.Text = selectedSourceFolderPath;
                }
            }
        }

        private void DestinationButtonClick(object sender, EventArgs e)
        {
            var returnPath = FolderDialog();

            if (returnPath != null)
            {
                selectedDestinationFolderPath = returnPath;

                if (selectedDestinationFolderPath.Length > maxPathLength)
                {
                    int charactersToRemove = selectedDestinationFolderPath.Length - maxPathLength;
                    destinationResultLabel.Text = "..." + selectedDestinationFolderPath.Substring(charactersToRemove);
                }
                else
                {
                    destinationResultLabel.Text = selectedDestinationFolderPath;
                }
            }
        }

        private string? FolderDialog()
        {
            var folderBrowser = new FolderBrowserDialog();
            folderBrowser.SelectedPath = folderBrowser.InitialDirectory;

            var result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderBrowser.SelectedPath;
            }
            return null;

        }

        private void EnableMoveButton()
        {
            if (selectedDestinationFolderPath != null && selectedSourceFolderPath != null)
            {
                moveButton.Enabled = true;
            }
            else
            {
                moveButton.Enabled = false;
            }
        }

        private void EnableResetButton()
        {
            if (selectedSourceFolderPath != null || selectedDestinationFolderPath != null)
            {
                resetButton.Enabled = true;
            }
            else
            {
                resetButton.Enabled = false;
            }
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Move Selected");

            if (selectedDestinationFolderPath != null && selectedSourceFolderPath != null)
            {
                moveButton.Enabled = false;
                resetButton.Enabled = false;
                progressBar.Visible = true;
                MoveFiles.Move(new InputData { destinationPath = selectedDestinationFolderPath, sourcePath = selectedSourceFolderPath }, this, radioSelection);
                progressBar.Visible = false;
            }
            // Add code to move the files
            EnableMoveButton();
            EnableResetButton();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Add code to reset the form
            sourceResultLabel.Text = "";
            destinationResultLabel.Text = "";
            selectedDestinationFolderPath = null;
            selectedSourceFolderPath = null;
            selection1.Checked = true;
            EnableMoveButton();
            EnableResetButton();
        }

        public void ProgressBarUpdate(int value)
        {
            Debug.WriteLine(value);
            progressBar.Value = value;
        }

        private System.Windows.Forms.Label introLabel;
        private Button moveButton;
        private Button resetButton;
        private System.Windows.Forms.Label destinationResultLabel;
        private ProgressBar progressBar;
        private RadioButton selection3;
        private RadioButton selection2;
        private RadioButton selection1;



    }
    public enum RadioSelection
    {
        MoveWithoutOverwrite,
        MoveKeepLatest,
        MoveIdenticalWithCleanup,
    }
}