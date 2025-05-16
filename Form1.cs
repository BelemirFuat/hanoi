namespace hanoi
{
    public partial class Form1 : Form
    {
        private int diskCount = 3;
        private int moveCount = 0;
        private List<Stack<Panel>> towers;
        private Panel selectedDisk = null;
        private Point originalLocation;
        private ComboBox diskSelector;
        private Label moveLabel;
        private Button restartButton;
        private Panel[] towerPanels;
        public Form1()
        {
            InitializeComponent();
            InitUI();
            InitGame();
        }
        private void InitUI()
        {
            this.Text = "Hanoi Kulesi";
            this.Size = new Size(800, 600);

            diskSelector = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            for (int i = 3; i <= 8; i++)
                diskSelector.Items.Add(i);
            diskSelector.SelectedIndex = 0;
            diskSelector.SelectedIndexChanged += (s, e) => RestartGame();
            this.Controls.Add(diskSelector);

            moveLabel = new Label { Location = new Point(140, 25), Text = "Hamle: 0", AutoSize = true };
            this.Controls.Add(moveLabel);

            restartButton = new Button { Text = "Yeniden Başlat", Location = new Point(220, 20) };
            restartButton.Click += (s, e) => RestartGame();
            this.Controls.Add(restartButton);

            towerPanels = new Panel[3];
            towers = new List<Stack<Panel>>();

            for (int i = 0; i < 3; i++)
            {
                Panel tower = new Panel
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightGray,
                    Width = 200,
                    Height = 400,
                    Location = new Point(100 + i * 220, 100),
                    Tag = i
                };
                tower.AllowDrop = true;
                tower.DragEnter += Tower_DragEnter;
                tower.DragDrop += Tower_DragDrop;
                towerPanels[i] = tower;
                this.Controls.Add(tower);
                towers.Add(new Stack<Panel>());
            }
        }

        private void InitGame()
        {
            moveCount = 0;
            moveLabel.Text = "Hamle: 0";
            diskCount = int.Parse(diskSelector.SelectedItem.ToString());
            foreach (var stack in towers) stack.Clear();
            foreach (var panel in towerPanels) panel.Controls.Clear();

            for (int i = diskCount; i >= 1; i--)
            {
                Panel disk = new Panel
                {
                    Width = 40 + i * 20,
                    Height = 20,
                    BackColor = Color.Red,
                    Tag = i,
                    Cursor = Cursors.Hand
                };
                disk.MouseDown += Disk_MouseDown;
                towers[0].Push(disk);
                PositionDisk(towerPanels[0], disk);
            }
        }

        private void RestartGame()
        {
            InitGame();
        }

        private void PositionDisk(Panel tower, Panel disk)
        {
            int count = tower.Controls.Count;
            disk.Location = new Point((tower.Width - disk.Width) / 2, tower.Height - 22 - count * 22);
            tower.Controls.Add(disk);
            disk.BringToFront();
        }

        private void Disk_MouseDown(object sender, MouseEventArgs e)
        {
            Panel disk = sender as Panel;
            int towerIndex = Array.FindIndex(towerPanels, p => p.Controls.Contains(disk));
            if (towers[towerIndex].Peek() == disk)
            {
                selectedDisk = disk;
                originalLocation = disk.Location;
                disk.BackColor = Color.Blue;
                disk.DoDragDrop(disk, DragDropEffects.Move);
            }
        }

        private void Tower_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Tower_DragDrop(object sender, DragEventArgs e)
        {
            if (selectedDisk == null) return;

            Panel targetTower = sender as Panel;
            int targetIndex = (int)targetTower.Tag;
            int sourceIndex = Array.FindIndex(towerPanels, p => p.Controls.Contains(selectedDisk));

            if (towers[targetIndex].Count > 0 && (int)towers[targetIndex].Peek().Tag < (int)selectedDisk.Tag)
            {
                MessageBox.Show("Bu işlemi yapamazsınız!", "Geçersiz Hamle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                selectedDisk.BackColor = Color.Red;
                selectedDisk = null;
                return;
            }

            towerPanels[sourceIndex].Controls.Remove(selectedDisk);
            towers[sourceIndex].Pop();
            towers[targetIndex].Push(selectedDisk);
            selectedDisk.BackColor = Color.Red;
            PositionDisk(targetTower, selectedDisk);
            moveCount++;
            moveLabel.Text = $"Hamle: {moveCount}";
            selectedDisk = null;

            if (towers[2].Count == diskCount)
                MessageBox.Show($"Tebrikler! Oyunu {moveCount} Hamlede Başarıyla Tamamladınız.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
