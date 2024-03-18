using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SSM
{
    public partial class SSM : Form
    {
        //public sprite information
        public List<SpriteData> sprites = [];
        //controls need to be public
        public Panel listpanel = new() { AutoScroll = true, BackColor = Color.Navy };
        public ListBox listbox = new() { DrawMode = DrawMode.OwnerDrawVariable };
        public Panel mainpanel = new() { AutoScroll = true, BackColor = Color.Navy };
        public PictureBox mainbox = new() { SizeMode = PictureBoxSizeMode.AutoSize };
        public Panel listbuttons = new() { AutoScroll = true, BackColor = Color.Navy, ForeColor = Color.Aqua };
        public Button listadd = new() { BackgroundImage = Resource.addbutton, FlatStyle = FlatStyle.Popup };
        public Button listdelete = new() { BackgroundImage = Resource.deletebutton, FlatStyle = FlatStyle.Popup };
        public Button listclear = new() { BackgroundImage = Resource.clearbutton, FlatStyle = FlatStyle.Popup };
        public Panel programbuttons = new() { AutoScroll = true, BackColor = Color.Navy, ForeColor = Color.Aqua };
        public Button programcompute = new() { BackgroundImage = Resource.computebutton, FlatStyle = FlatStyle.Popup };
        public Button programclear = new() { BackgroundImage = Resource.clearsheetbutton, FlatStyle = FlatStyle.Popup };
        public Button programsave = new() { BackgroundImage = Resource.savebutton, FlatStyle = FlatStyle.Popup };
        public Button programexit = new() { BackgroundImage = Resource.exitbutton, FlatStyle = FlatStyle.Popup };
        public ToolTip tooltip = new();
        //other variables
        public Bitmap mainscreen = new(1, 1);
        public List<MainGrid> grids = [];
        public int listindex = -1;
        public int mainindex = -1;
        public bool listdrag = false;
        public bool maindrag = false;
        private int spritetocopy = -1;
        private bool mousedown = false;

        public SSM()
        {
            InitializeComponent(); //Set up the main form
            Shown += SSM_Shown; //Set a shown event handler
            Resize += SSM_Resize; //Set a resize event handler
            FormClosing += SSM_FormClosing; //set a form closing event handler
            BackColor = Color.FromArgb(16, 16, 16); //set the form background color
        }

        private void SSM_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure?","Quit?",MessageBoxButtons.YesNo) == DialogResult.Yes) //if user agrees to quit the program
            {
                //do any disposing here
                mainscreen.Dispose(); //dispose of main screen
                Dispose(); //dispose of main form
                Application.Exit(); //exit the program
            }
            e.Cancel = true; //tell Windows not to close the form
        }

        private void SSM_Resize(object? sender, EventArgs e)
        {
            DoResize(WindowState); //call DoResize with current WindowState
        }

        private void SSM_Shown(object? sender, EventArgs e)
        {
            AddControls(); //add controls
            DoResize(FormWindowState.Normal); //do resize
        }

        private void AddControls()
        {
            listpanel.Controls.Add(listbox); //add controls to panels
            mainpanel.Controls.Add(mainbox);
            listbuttons.Controls.Add(listadd);
            listbuttons.Controls.Add(listdelete);
            listbuttons.Controls.Add(listclear);
            programbuttons.Controls.Add(programcompute);
            programbuttons.Controls.Add(programsave);
            programbuttons.Controls.Add(programclear);
            programbuttons.Controls.Add(programexit);
            Controls.Add(mainpanel); //add panels to main form
            Controls.Add(listpanel);
            Controls.Add(listbuttons);
            Controls.Add(programbuttons);
            SetEventHandlers(); //set event handlers for controls
        }
        private void DoResize(FormWindowState windowstate)
        {
            if (windowstate == FormWindowState.Minimized || !Controls.Contains(mainpanel)) //if we are minimized or if the main form doesn't contain a mainpanel (hasn't been set up)
            {
                return; //just exit stage left
            }
            ResizeControls(); //resize the controls
            Mainbox_Paint(); //redraw the mainbox
        }

        private void ResizeControls()
        {
            Point zerozero = new(0, 0); //convenience variable
            int width = ClientSize.Width - 20; //set width
            int height = ClientSize.Height - 20; //set height
            mainpanel.Location = new(10, 10); //set mainpanel location and size
            mainpanel.Size = new(width - 160, height - 60);
            mainbox.Location = zerozero; //set mainbox location and size
            mainbox.Size = mainpanel.Size;
            if (mainscreen.Width == 1 && mainscreen.Height == 1) //if mainscreen is default size
            {
                mainscreen = ResizeBitmap(mainscreen, mainbox.Size); //resize mainscreen to fit mainbox
            }
            listpanel.Location = new(width - 140, 10); //set listpanel location and size
            listpanel.Size = new(150, height - 130);
            listbox.Location = zerozero; //set listbox location and size
            listbox.Size = listpanel.Size;
            listbuttons.Location = new(width - 140, height - 110); //set listbuttons location and size
            listbuttons.Size = new(150, 120);
            listadd.Location = new(37, 5); //set listbuttons' buttons locations and sizes
            listadd.Size = new(75, 30);
            listdelete.Location = new(37, 45);
            listdelete.Size = listadd.Size;
            listclear.Location = new(37, 85);
            listclear.Size = listadd.Size;
            programbuttons.Location = new(10, height - 40); //set programbuttons location and size
            programbuttons.Size = new(width - 160, 50);
            SetButtons(width - 170, [ programcompute, programsave, programclear, programexit ], new(100, 40)); //set programbuttons' buttons locations and sizes
        }

        private static Bitmap ResizeBitmap(Bitmap old, Size size)
        {
            Bitmap newbitmap = new(size.Width, size.Height); //make a new bitmap
            using Graphics graphics = Graphics.FromImage(newbitmap); //using a graphics object from the new bitmap
            {
                graphics.DrawImage(old, 0, 0); //paint the old bitmap to its upper left corner
            }
            old.Dispose(); //get rid of the old bitmap
            return newbitmap; //return the new bitmap
        }

        private static void SetButtons(int width, List<Control> buttons, Size size)
        {
            for (int index = 0; index < buttons.Count; index++) //for each button
            {
                int x = index * (width - 10 - size.Width) / (buttons.Count - 1) + 5; //compute horizontal position of button
                buttons[index].Size = size; //set button size
                buttons[index].Location = new(x, 5); //set button location
            }
        }
        private void SetEventHandlers()
        {
            //mainbox.MouseClick += Mainbox_MouseClick;
            mainbox.MouseDown += Mainbox_MouseDown; //set mainbox mouse handlers
            mainbox.MouseUp += Mainbox_MouseUp;
            mainbox.MouseMove += Mainbox_MouseMove;
            //listbox.MouseClick += Listbox_MouseClick;
            listbox.MouseDown += Listbox_MouseDown; //set listbox mouse handlers
            listbox.MouseUp += Listbox_MouseUp;
            listbox.MouseMove += Listbox_MouseMove;
            listbox.MouseDoubleClick += Listbox_MouseDoubleClick;
            listadd.Click += Listadd_Click; //set event handlers for listbuttons' buttons
            listdelete.Click += Listdelete_Click;
            listclear.Click += Listclear_Click;
            programcompute.Click += Programcompute_Click; //set event handlers for programbuttons' buttons
            programsave.Click += Programsave_Click;
            programclear.Click += Programclear_Click;
            programexit.Click += Programexit_Click;
            listbox.DrawItem += Listbox_DrawItem; //set event handlers for listbox drawitem and listbox measureitem
            listbox.MeasureItem += Listbox_MeasureItem;
        }

        private void Listbox_MeasureItem(object? sender, MeasureItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < sprites.Count) //if index is in bounds
            {
                e.ItemWidth = sprites[e.Index].Size.Width + 2; //set item width with padding of 2
                e.ItemHeight = sprites[e.Index].Size.Height + 2; //set item height with padding of 2
            }
        }

        private void Listbox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < sprites.Count) //if index is in bounds
            {
                e.DrawBackground(); //draw background
                Color color1 = Color.Black, color2 = Color.White; //default colors are black and white
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) //if item is selected
                {
                    color1 = Color.Navy; //use navy and aqua
                    color2 = Color.Aqua;
                }
                Pen pen = new(color1); //set a pen to color1
                DrawHatch(e.Graphics, e.Bounds, color1, color2); //draw hatch pattern
                e.Graphics.DrawRectangle(pen, e.Bounds); //draw rectangle
                DrawCenteredBitmap(e.Graphics, sprites[e.Index].Bitmap, e.Bounds); //draw the sprite centered
            }
        }

        private void Programexit_Click(object? sender, EventArgs e)
        {
            Close(); //close the main form
        }

        private void Programclear_Click(object? sender, EventArgs e)
        {
            Size size = mainscreen.Size; //get size of mainscreen
            mainscreen.Dispose(); //dispose of mainscreen
            mainscreen = new(size.Width, size.Height); //make a new mainscreen with same size
            Mainbox_Paint(); //paint mainbox
        }

        private void Programsave_Click(object? sender, EventArgs e)
        {
            string? filename = GetSaveFile(); //get filename
            if (filename != null) //if we got one
            {
                string? path = Path.GetDirectoryName(filename); //get the path for filename
                string textfile = Path.GetFileNameWithoutExtension(filename) + ".txt"; //get the filename for writing text
                if (path != null) //if we have a path
                {
                    textfile = Path.Combine(path, textfile); //prepend it
                }
                try //error handling
                {
                    if (grids.Count > 0) //if we have grids
                    {
                        using Bitmap dupscreen = new(mainscreen.Width, mainscreen.Height); //make a new bitmap with mainscreen's size
                        {
                            using Graphics graphics = Graphics.FromImage(dupscreen); //using graphics object from it
                            {
                                foreach (var grid in grids) //for each grid
                                {
                                    Rectangle pos = PaintGrid(graphics, grid); //paint the grid
                                    if (grid.Sprite != null) //if grid has a sprite
                                    {
                                        using StreamWriter sw = new(textfile, true); //use streamwriter in append mode for text file
                                        {
                                            sw.WriteLine($"{Path.GetFileNameWithoutExtension(grid.Sprite.Name)}: {pos}"); //save information to stream writer
                                        }
                                    }
                                }
                            }
                            dupscreen.Save(filename); //save the bitmap to the filename
                        }
                    }
                }
                catch (Exception ex) //if there's an error
                {
                    MessageBox.Show($"Error {ex.Message} saving mainscreen to {filename}.", "Error", MessageBoxButtons.OK); //tell the user
                }
            }
        }

        private void Programcompute_Click(object? sender, EventArgs e)
        {
            bool copysprites = false; //flag for copy sprites
            int spritewidth = sprites.Max(sprite => sprite.Size.Width) + 2; //get width and height to use for each grid cell
            int spriteheight = sprites.Max(sprite => sprite.Size.Height) + 2;
            int spritecount = sprites.Count; //get number of sprites
            int columns = (int)Math.Ceiling(Math.Sqrt(spritecount)); //calculate the number of columns and rows needed to fit all the sprites
            int rows = (int)Math.Ceiling((double)spritecount / columns);
            int cellwidth = spritewidth * columns; //calculate the total width and height
            int cellheight = spriteheight * rows;
            mainscreen = ResizeBitmap(mainscreen, new Size(cellwidth, cellheight)); //resize the mainscreen to fit the cells
            grids.Clear(); //clear grids list
            if (MessageBox.Show("Copy sprites now?","Query",MessageBoxButtons.YesNo) == DialogResult.Yes) //if user wants mainscreen filled with sprites
            {
                copysprites = true; //set flag
            }
            for (int index = 0; index < columns * rows; index++) //make cells for all columns and rows
            {
                int x = (index % columns) * spritewidth + 1; //calculate the position of the sprite within the cell
                int y = (index / columns) * spriteheight + 1;
                Rectangle position = new(x, y, spritewidth, spriteheight); //add a new grid with the proper position
                MainGrid grid = new(position);
                if (copysprites && index < sprites.Count) //if copying sprites and index in range
                {
                    grid.Sprite = sprites[index]; //copy sprite to grid
                }
                grids.Add(grid);
            }
            Mainbox_Paint(); //redraw the mainbox
        }

        private void Listclear_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?","Clear List?",MessageBoxButtons.YesNo) != DialogResult.Yes) //if the user did not want to clear the list
            {
                return; //exit stage left
            }
            sprites.Clear(); //clear the arrays
            listbox.Items.Clear(); //clear the listbox
            listbox.Refresh(); //update the listbox layout
            listbox.Invalidate(); //redraw the listbox
            listindex = -1; //we no longer have a selected list item
        }

        private void Listdelete_Click(object? sender, EventArgs e)
        {
            if (listindex == -1) //if we don't have a selected list index
            {
                return; //exit stage left
            }
            if (MessageBox.Show("Are you sure?","Delete Sprite?",MessageBoxButtons.YesNo) != DialogResult.Yes) //if the user doesn't want to delete the selected sprite
            {
                return; //exit stage right
            }
            sprites.RemoveAt(listindex); //remove the sprite from the arrays
            listbox.Items.RemoveAt(listindex);
            listbox.Refresh(); //update the listbox layout
            listbox.Invalidate(); //redraw the listbox
            listindex = -1; //we no longer have a selected list item
        }

        private void Listadd_Click(object? sender, EventArgs e)
        {
            string[] filenames = GetOpenFiles(); //get filenames
            foreach (var filename in filenames) //for each filename
            {
                AddFileToList(filename); //add it to the lists
            }
            listbox.Refresh(); //update the listbox layout
            listbox.Invalidate(); //redraw the listbox
            listindex = -1; //we no longer have a selected list item
        }

        private void Listbox_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = listbox.IndexFromPoint(e.Location); //get listbox index from mouse position
            if (index >= 0 && index < sprites.Count) //if index is in bounds
            {
                MakeToolTip(listbox, sprites[index]);
            }
        }

        private void Listbox_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            spritetocopy = listbox.IndexFromPoint(e.Location); //get listbox index from mouse position
            if (mainindex != -1) //if mainindex is not -1
            {
                if (CopyFromListToMain()) //if we copy from listbox to mainbox
                {
                    return; //we are done
                }
            }
        }

        private bool CopyFromListToMain()
        {
            if (MessageBox.Show($"Copy sprite from listbox to {mainindex}?","Query",MessageBoxButtons.YesNo) == DialogResult.Yes) //if user wants to copy sprite
            {
                grids[mainindex].Sprite = sprites[spritetocopy]; //copy the sprite
                spritetocopy = -1; //reset the sprite to copy
                mainindex = -1; //reset the main index
                Mainbox_Paint(); //redraw the main box
                return true; //return true (we did the copy)
            }
            return false; //we didn't do the copy
        }
        private void Listbox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (listdrag) //if we are dragging
            {
                int index = listbox.IndexFromPoint(e.Location); //get index
                if (index >= 0 && index < sprites.Count && index != listindex) //if in bounds and we moved to a different location
                {
                    if (MessageBox.Show("Exchange these two sprites?","Query",MessageBoxButtons.YesNo) == DialogResult.Yes) //if user agrees to exchange sprites
                    {
                        sprites[index].Exchange(sprites[listindex]); //exchange sprites
                        listbox.Refresh(); //update the listbox layout
                        listbox.Invalidate(); //redraw the listbox
                    }
                }
                listindex = -1; //we no longer have a selected list item
                listdrag = false; //we are no longer dragging
            }
            else if (e.Button == MouseButtons.Left) //otherwise (not dragging) if using left mouse button
            {
                int index = listbox.IndexFromPoint(e.Location); //get listbox index from mouse position
                if (index >= 0 && index < sprites.Count) //if index is in bounds
                {
                    listindex = index; //set list index
                }
            }
            if (e is HandledMouseEventArgs arg) //if we can select handled
            {
                arg.Handled = true; //set handled to true
            }
        }

        private void Listbox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!listdrag) //if we are not dragging
            {
                int index = listbox.IndexFromPoint(e.Location); //get index
                if (index >= 0 && index < sprites.Count) //if in bounds
                {
                    listindex = index; //set listindex
                    listdrag = true; //set flag to show we are now dragging
                }
                if (e is HandledMouseEventArgs arg) //if we can select handled
                {
                    arg.Handled = true; //set handled to true
                }
            }
        }

        private void Mainbox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (grids.Count > 0) //if we have a grid
            {
                if (!mousedown) //if the mouse is not being held down, we will do a tool tip
                {
                    mainindex = FindGrid(e.Location); //get the grid index
                    if (mainindex > -1 && mainindex < grids.Count) //if index is in bounds
                    {
                        SpriteData? sprite = grids[mainindex].Sprite; //get sprite for grid
                        if (sprite != null) //if sprite exists
                        {
                            MakeToolTip(mainbox, sprite); //use it to make a tool tip
                        }
                        else //otherwise
                        {
                            tooltip.SetToolTip(mainbox, "empty"); //set the tool tip to "empty"
                        }
                    }
                }
                else //otherwise, we are holding the mouse down and moving, so
                {
                    int index = FindGrid(e.Location);
                    if (index != mainindex)
                    {
                        maindrag = true; //set the flag for dragging
                    }
                }
            }
            else //otherwise (we have no grid)
            {
                tooltip.SetToolTip(mainbox, "We don't have a grid, please click the Compute button first."); //tell user what they can do
            }
            if (e is HandledMouseEventArgs arg) //if we can select handled
            {
                arg.Handled = true; //set handled to true
            }
        }

        private void Mainbox_MouseUp(object? sender, MouseEventArgs e)
        {
            mousedown = false; //the mouse button is no longer being held down
            if (spritetocopy != -1) //if sprite to copy is valid
            {
                int index = FindGrid(e.Location);
                if (index > -1 && index < grids.Count)
                {
                    mainindex = index;
                    CopyFromListToMain();
                    return;
                }
            }
            if (maindrag) //if we are dragging
            {
                int index = FindGrid(e.Location); //get index
                if (index > -1 && index < grids.Count && index != mainindex) //if in bounds and we have moved to a different spot
                {
                    if (MessageBox.Show("Exchange sprites?", "Query", MessageBoxButtons.YesNo) == DialogResult.Yes) //if user agrees to exchange sprites
                    {
                        grids[index].ExchangeSprites(grids[mainindex]); //exchange them
                        Mainbox_Paint(); //redraw mainbox
                    }
                }
                mainindex = -1; //we no longer have a mainindex
                maindrag = false; //we are no longer dragging
            }
            if (e is HandledMouseEventArgs arg) //if we can select handled
            {
                arg.Handled = true; //set handled to true
            }
        }

        private void Mainbox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!maindrag) //if we are not dragging
            {
                int index = FindGrid(e.Location); //get index
                if (index > -1 && index < grids.Count) //if in bounds
                {
                    mainindex = index; //set index
                }
                if (e is HandledMouseEventArgs arg) //if we can select handled
                {
                    arg.Handled = true; //set handled to true
                }
                mousedown = true; //the mouse button is being held down
            }
        }

        private void Mainbox_Paint()
        {
            using Graphics graphics = Graphics.FromImage(mainscreen); //using graphics object from mainscreen
            {
                if (grids.Count > 0) //if we have grids
                {
                    foreach (var grid in grids) //for each grid
                    {
                        DrawHatch(graphics, grid.Position, Color.Gray, Color.White); //draw a checkerboard pattern
                        graphics.DrawRectangle(Pens.Black, grid.Position); //draw the outline
                        PaintGrid(graphics, grid); //draw sprite
                    }
                }
                else //otherwise (we have no grid)
                {
                    graphics.Clear(Color.Red); //clear the main screen to red
                }
            }
            mainbox.Image = mainscreen; //put mainscreen as mainbox's image
        }

        private static Rectangle PaintGrid(Graphics graphics, MainGrid grid)
        {
            Rectangle dst = Rectangle.Empty; //default rectangle is empty
            if (grid.Sprite != null) //if we have a sprite for this grid
            {
                Rectangle src = new(0, 0, grid.Sprite.Size.Width, grid.Sprite.Size.Height); //compute rectangles for drawing
                dst = new(grid.Position.Left + 1, grid.Position.Bottom - grid.Sprite.Size.Height - 1, grid.Sprite.Size.Width, grid.Sprite.Size.Height);
                graphics.DrawImage(grid.Sprite.Bitmap, dst, src, GraphicsUnit.Pixel); //draw the sprite
            }
            return dst; //return destination rectangle
        }
        private static string? GetSaveFile()
        {
            using SaveFileDialog savefiledialog = new() { Filter = "Png Files (*.png)|*.png", AddExtension = true, CheckPathExists = true, CreatePrompt = false }; //using savefiledialog
            {
                if (savefiledialog.ShowDialog() == DialogResult.OK) //if we got something
                {
                    return savefiledialog.FileName; //return the filename
                }
            }
            return null; //otherwise return null
        }

        private static string[] GetOpenFiles()
        {
            using OpenFileDialog openfiledialog = new() { Filter = "Png Files (*.png)|*.png", Multiselect = true, CheckFileExists = true, AddExtension = true }; //using openfiledialog
            {
                if (openfiledialog.ShowDialog() == DialogResult.OK) //if we got something
                {
                    return openfiledialog.FileNames; //return the filenames
                }
            }
            return []; //otherwise return an empty list
        }

        private void AddFileToList(string filename)
        {
            try //do error handling
            {
                Bitmap bitmap = new(filename); //try to load the file
                SpriteData sprite = new(filename, bitmap);
                sprites.Add(sprite); //add a new SpriteData
                listbox.Items.Add(sprite); //add to listbox
            }
            catch (Exception ex) //if there was an error
            {
                MessageBox.Show($"Error {ex.Message} loading {filename}.", "Error", MessageBoxButtons.OK); //show the error
            }
        }

        private static void DrawCenteredBitmap(Graphics graphics, Bitmap bitmap, Rectangle bounds)
        {
            int wide = bitmap.Width; //get width and height
            int high = bitmap.Height;
            if (wide > bounds.Width - 2) //if width is too large
            {
                wide = bounds.Width - 2; //resize but keep aspect ratio
                high = (int)Math.Round(wide * bitmap.Height / (double)bitmap.Width);
            }
            int left = (bounds.Width - wide) / 2 + bounds.Left; //compute left and top for centering
            int top = (bounds.Height - high) / 2 + bounds.Top;
            Rectangle dst = new(left, top, wide, high); //make rectangles
            Rectangle src = new(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImage(bitmap, dst, src, GraphicsUnit.Pixel); //draw the bitmap
        }

        public int FindGrid(Point spot)
        {
            for (int index = 0; index < grids.Count; index++) //iterate through grids
            {
                if (grids[index].PointInside(spot)) //if grid has point inside its boundaries
                {
                    return index; //return index of grid
                }
            }
            return -1; //if no grid found that has point, return -1 as an error indicator
        }

        private static void DrawHatch(Graphics graphics, Rectangle bounds, Color color1, Color color2)
        {
            using HatchBrush brush = new(HatchStyle.LargeCheckerBoard, color1, color2); //make hatch brush
            {
                graphics.FillRectangle(brush, bounds); //fill background 
            }
        }

        private void MakeToolTip(Control control, SpriteData sprite)
        {
            string tooltiptext = $"Width: {sprite.Size.Width}, Height: {sprite.Size.Height}, Filename: {sprite.Name}"; //get sprite information
            tooltip.SetToolTip(control, tooltiptext); //show tooltip
        }
        public class MainGrid
        {
            public Rectangle Position { get => _position; set => _position = value; }
            public SpriteData? Sprite { get => _sprite; set => _sprite = value; }
            private Rectangle _position;
            private SpriteData? _sprite;
            public MainGrid() { }
            public MainGrid(Rectangle position)
            {
                _position = position;
                _sprite = null;
            }
            public bool PointInside(Point point) => point.X >= _position.Left && point.X <= _position.Right && point.Y >= _position.Top && point.Y <= _position.Bottom;
            public void Exchange(MainGrid grid)
            {
                (grid.Position, Position) = (Position, grid.Position);
                (grid.Sprite, Sprite) = (Sprite, grid.Sprite);
            }
            public void ExchangeSprites(MainGrid grid) => (grid.Sprite, Sprite) = (Sprite, grid.Sprite);
        }

        public class SpriteData(string name, Bitmap bitmap)
        {
            public string Name { get; set; } = name;
            public Bitmap Bitmap { get; set; } = bitmap;
            public Size Size { get; set; } = new(bitmap.Width, bitmap.Height);

            public void Exchange(SpriteData sprite)
            {
                (sprite.Name, Name) = (Name, sprite.Name);
                (sprite.Bitmap, Bitmap) = (Bitmap, sprite.Bitmap);
                (sprite.Size, Size) = (Size, sprite.Size);
            }
        }
    }
}
