using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace sudoku
{
    public partial class Form1 : Form
    {
        // ####################################
        // ######## global variables ##########
        // ####################################

        // subgrid and field in that subgrid
        int[] selectedField = { 9, 9 };
        // number stored inside field, 1[] - subgrid, 2[] - field
        int[,] grid = new int[9, 9];

        int moveID = 0;
        public struct Moves
        {
            public Moves(int grid, int field, int number)
            {
                Grid = grid;
                Field = field;
                Number = number;
            }

            public int Grid { get; set; }
            public int Field { get; set; }
            public int Number { get; set; }

            public void SetMoves(int grid, int field, int number)
            {
                Grid = grid;
                Field = field;
                Number = number;
            }
        }

        Moves[] moves = new Moves[160];

        public Form1()
        {
            InitializeComponent();

            newGame();
        }

        // ####################################
        // ########### form events ############
        // ####################################

        // =========== form close =============

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Sudoku", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        // ####################################
        // ######### menustrip items ##########
        // ####################################

        // ============ new game ==============

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to start new game?", "Sudoku", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                newGame();
        }

        // ============== exit ================

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        // ========== about program ===========

        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        // ============= help page ============

        private void helpPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://fldsmfr.rf.gd");
        }

        // ####################################
        // ########## button events ###########
        // ####################################

        // ============= number ===============

        private void btn_Click(object sender, EventArgs e)
        {
            if (selectedField[0] < 9)
            {
                int number = Convert.ToInt16(((Button)sender).Tag);
                Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + number.ToString() + "s");

                grid[selectedField[0], selectedField[1]] = number;

                replaceImage(image);

                moveID++;
                moves[moveID].SetMoves(selectedField[0], selectedField[1], number);

                checkIfWon();
            }
        }

        // ============= restart ==============

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart this game?", "Sudoku", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                selectedField[0] = 0;
                selectedField[1] = 0;

                Array.Clear(grid, 0, grid.Length);

                foreach (PictureBox field in this.Controls.OfType<PictureBox>().Where(field => field.Name.StartsWith("imgField")))
                    field.Image = Properties.Resources._0;
            }
        }

        // ============== undo ================

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (moveID > 0)
            {
                replaceImage(Properties.Resources._0);

                grid[selectedField[0], selectedField[1]] = 0;
                moveID--;
                selectedField[0] = moves[moveID].Grid;
                selectedField[1] = moves[moveID].Field;
                grid[selectedField[0], selectedField[1]] = moves[moveID].Number;

                Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + grid[selectedField[0], selectedField[1]].ToString() + "s");
                replaceImage(image);
            }
        }

        // =========== grid field ==============

        private void imgField_Click(object sender, EventArgs e)
        {
            int number;
            if (selectedField[0] < 9)
            {
                number = grid[selectedField[0], selectedField[1]];
                Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + number.ToString());
                replaceImage(image);
            }

            selectedField[0] = Convert.ToInt16(((PictureBox)sender).Tag.ToString().Split('-')[0]);
            selectedField[1] = Convert.ToInt16(((PictureBox)sender).Tag.ToString().Split('-')[1]);

            number = grid[selectedField[0], selectedField[1]];
            Bitmap imageS = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + number.ToString() + "s");

            ((PictureBox)sender).Image = imageS;
        }

        // ####################################
        // ######### other functions ##########
        // ####################################

        // ============= new game =============

        private void newGame()
        {
            selectedField[0] = 9;
            selectedField[1] = 9;

            Array.Clear(grid, 0, grid.Length);

            replaceImage(Properties.Resources._0);

            int[,,] table = { { { 0, 2, 8, 7, 0, 5, 6, 3, 0 }, { 3, 6, 1, 2, 0, 4, 0, 7, 0 }, { 7, 9, 5, 0, 3, 0, 0, 0, 8 },
                                { 8, 5, 2, 3, 7, 0, 0, 1, 0 }, { 0, 0, 7, 9, 2, 5, 0, 3, 8 }, { 3, 0, 9, 0, 1, 6, 0, 7, 2 },
                                { 0, 8, 7, 2, 4, 9, 0, 0, 0 }, { 1, 9, 3, 7, 5, 6, 8, 4, 2 }, { 2, 6, 0, 1, 8, 3, 9, 5, 7 } },
                              { { 7, 0, 0, 0, 0, 5, 0, 1, 0 }, { 0, 0, 8, 7, 1, 0, 0, 0, 4 }, { 0, 1, 0, 0, 8, 9, 7, 5, 0 },
                                { 0, 6, 1, 9, 0, 0, 5, 7, 0 }, { 8, 7, 0, 0, 0, 0, 9, 0, 0 }, { 0, 4, 3, 0, 7, 0, 0, 0, 0 },
                                { 0, 9, 0, 0, 0, 7, 8, 0, 6 }, { 1, 0, 6, 0, 8, 0, 0, 3, 7 }, { 0, 3, 7, 6, 0, 0, 0, 0, 0 } },
                              { { 0, 0, 0, 4, 0, 0, 0, 0, 5 }, { 9, 0, 0, 0, 0, 3, 0, 0, 0 }, { 0, 1, 2, 0, 9, 0, 8, 0, 4 },
                                { 0, 0, 7, 1, 0, 0, 0, 0, 0 }, { 6, 3, 0, 0, 0, 2, 4, 0, 8 }, { 2, 4, 0, 0, 3, 6, 1, 0, 0 },
                                { 7, 0, 0, 0, 1, 4, 0, 5, 3 }, { 3, 0, 0, 2, 0, 9, 7, 0, 0 }, { 4, 2, 0, 0, 7, 0, 0, 0, 0 } },
                              { { 4, 0, 5, 2, 0, 0, 6, 1, 9 }, { 0, 0, 0, 4, 0, 0, 2, 0, 0 }, { 0, 0, 7, 0, 0, 0, 0, 0, 0 },
                                { 5, 0, 4, 0, 2, 0, 0, 0, 1 }, { 9, 2, 0, 1, 6, 0, 5, 4, 8 }, { 7, 1, 0, 9, 4, 0, 2, 0, 0 },
                                { 0, 5, 0, 3, 0, 0, 0, 0, 0 }, { 6, 0, 4, 0, 0, 0, 3, 0, 9 }, { 8, 0, 0, 1, 0, 0, 0, 7, 0 } },
                              { { 6, 0, 5, 0, 0, 0, 0, 1, 8 }, { 2, 0, 0, 0, 0, 8, 0, 0, 0 }, { 7, 0, 8, 0, 9, 0, 2, 0, 0 },
                                { 0, 6, 1, 0, 0, 0, 5, 3, 0 }, { 0, 0, 7, 1, 0, 5, 6, 0, 2 }, { 9, 5, 0, 0, 0, 6, 0, 0, 0 },
                                { 0, 0, 0, 0, 5, 0, 0, 7, 0 }, { 5, 2, 0, 7, 0, 4, 9, 6, 3 }, { 0, 7, 0, 0, 6, 2, 0, 0, 4 } } };

            /*,
                              { {  }, {  }, {  },
                                {  }, {  }, {  },
                                {  }, {  }, {  } }*/

            Random rnd = new Random();
            int rnd2 = rnd.Next(0, 3);

            for (int i = 0; i < 9;  i++)
            {
                selectedField[0] = i;
                for (int j = 0; j < 9; j++)
                {
                    selectedField[1] = j;
                    grid[selectedField[0], selectedField[1]] = table[rnd2, selectedField[0], selectedField[1]];
                    Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + grid[selectedField[0], selectedField[1]].ToString());
                    replaceImage(image);
                }
            }

            selectedField[0] = 9;
            selectedField[1] = 9;
        }

        private void replaceImage(Bitmap image)
        {
            var previous = this.Controls.Find("imgField" + selectedField[0].ToString() + selectedField[1].ToString(), true);
            if (previous != null)
                foreach (var item in previous)
                    ((PictureBox)item).Image = image;
        }

        private void checkIfWon()
        {
            int emptyAmount = 81;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != 0)
                        emptyAmount--;
                }
            }
            if (emptyAmount == 0)
            {
                bool isGood = true;
                // subgrid
                for (int i = 0; i < 9; i++)
                    // field
                    for (int j = 0; j < 9; j++)
                    {
                        // amount of subgrids to check on right
                        int z;

                        if (i == 0 || i == 3 || i == 6)
                            z = 3;
                        else if (i == 1 || i == 4 || i == 7)
                            z = 2;
                        else
                            z = 1;

                        // amount of fields to check on right
                        int y;

                        if (j == 0 || j == 3 || j == 6)
                            y = 3;
                        else if (j == 1 || j == 4 || j == 7)
                            y = 2;
                        else
                            y = 1;

                        for (int k = 0; k < z; k++)
                            for (int l = 1; l < y; l++)
                                if (grid[i, j] == grid[i + k, j + l])
                                    isGood = false;

                        // amount of subgrids to check on bottom
                        if (i == 0 || i == 1 || i == 2)
                            z = 9;
                        else if (i == 3 || i == 4 || i == 5)
                            z = 6;
                        else
                            z = 3;

                        // amount of fields to check on bottom
                        if (j == 0 || j == 1 || j == 2)
                            y = 9;
                        else if (j == 3 || j == 4 || j == 5)
                            y = 6;
                        else
                            y = 3;

                        for (int k = 0; k < z; k += 3)
                            for (int l = 3; l < y; l += 3)
                                if (grid[i, j] == grid[i + k, j + l])
                                    isGood = false;

                        /*// amount of subgrids on the right or bottom
                        int x;

                        if (i == 0 || i == 3 || i == 6)
                            x = 3;
                        else if (i == 1 || i == 4 || i == 7)
                            x = 2;
                        else
                            x = 1;

                        // amount of fields on the right or bottom
                        int w;

                        if (j == 0 || j == 3 || j == 6)
                            w = 2;
                        else if (j == 1 || j == 4 || j == 7)
                            w = 1;
                        else
                            w = 0;

                        for (int k = 0; k < z; k++)
                            for (int l = 0; l < y; l++)
                                for (int m = 0; m < x; m++)
                                    for (int n = 0; n < w; n++)
                                    {
                                        if (grid[i, j] == grid[i + 1, j])
                                            isGood = false;
                                        if (grid[i, j] == grid[i, j + 3])
                                            isGood = false;
                                    }*/
                    }

                if (isGood)
                    MessageBox.Show("You won!", "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
