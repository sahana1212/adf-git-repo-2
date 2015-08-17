using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;


namespace Visual_Novel_Database
{
    public partial class frmMain : Form
    {
        List<Visual_Novel> VNS = new List<Visual_Novel>(); //List of all Visual Novels in database
        List<WrittenTagsRootObject> plain_tags; //Contains all plain_tags, meaning the loaded tagdump.json
        List<string> character_images;
        int image_number = 0; //used for cycling through screenshots
        int char_number = 0; //used for cycling through character images

        public frmMain()
        {
            InitializeComponent();
        }

        //Fetch information from 'VNDB.org'
        private void btnFetch_Click(object sender, EventArgs e)
        {
            //Fetches inforamtion from 'VNDB.org'

            int parsedValue = 0;

            if (txtLoginName.Text == "") //Check if LoginName was entered
            {
                MessageBox.Show("You have to enter a Loginname; You don't have to enter your actual account name!", "Enter loginname", MessageBoxButtons.OK);
                return;
            }

            if (txtID.Text == "") //Check if ID has been entered
            {
                MessageBox.Show("You have to enter an ID first!", "No ID", MessageBoxButtons.OK);
                return;
            }
            else if (!int.TryParse(txtID.Text, out parsedValue)) //and if the ID is numerical
            {
                MessageBox.Show("An ID can only contains numbers!", "No chars allowed", MessageBoxButtons.OK);
                return;
            }

            foreach (Visual_Novel novel in VNS) //Check if Visual Novel is already in local database
            {
                if (novel.id == Convert.ToInt32(txtID.Text))
                {
                    MessageBox.Show("Visual Novel is alrady in the database", "VN already exists!", MessageBoxButtons.OK);
                    return;
                }
            }

            int error = 1;

            error = fetchInformation(false); //Call 'fetchinformation; 'false' means it is not used for updating existing VN but for adding a new VN

            if (error == 0)
                return;
        }

        //Adds VN to database
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Adds VN to database

            if (txtName.Text == "") //Check if data has been fetched
            {
                MessageBox.Show("You have to fetch a VN first", "No Adding before Fetching!", MessageBoxButtons.OK);
                return;
            }

            if (txtPath.Text == "") //Check if path to VN has been entered
            {
                DialogResult msg = MessageBox.Show("You have not entered a path to the executeable, are you sure you want to add the VN without being able to launch it?", "No path added", MessageBoxButtons.YesNo);
                if (msg == DialogResult.No) //Check if user wants to proceed even if no path has been entered
                    return;
            }

            string name = txtName.Text;

            Regex illegalCharacters = new Regex("[!?*]|[ ]{2}"); 

            name = illegalCharacters.Replace(name, " "); //Replace illegal characters in name with empty spaces
            name.Replace('"', ' ');

            //Create the new VN
            Visual_Novel novel = new Visual_Novel(name.Trim(), txtOriginalName.Text, txtTags.Text, rtbDescription.Text, txtPath.Text, Convert.ToInt32(txtVNID.Text), character_images);

            character_images.Clear();

            txtPath.Text = ""; //Reset textboxes
            lblStatus.Text = "Visual Novel with ID: " + txtID.Text + " added"; //And set status text

            txtName.Text = "";
            txtOriginalName.Text = "";
            txtVNID.Text = "";
            txtTags.Text = "";
            rtbDescription.Text = "";
            txtID.Text = "";
            pcbImages.ImageLocation = "";
        }

        //Loads database into the listbox
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //Read local "VisualNovels.json" file and fills listbox

            lbVN.Items.Clear(); //Reset listbox items
            bool addVN = true;
            VNS.Clear(); //Reset all Visual Novels
            image_number = 0; //Reset variables used for cycling through images
            char_number = 0;
            toolTip1.Active = false; //Deactivate screenshot and character_images tooltips

            if (plain_tags == null) //Check if tagdump has already been read
                plain_tags = JsonConvert.DeserializeObject<List<WrittenTagsRootObject>>(File.ReadAllText(@"tags.json")); //if not read it

            List<Visual_Novel> VisualNovels; //Declare new list of Visual Novels

            if (File.Exists("VisualNovels.json")) //Read existing database and save it in list
                VisualNovels = JsonConvert.DeserializeObject<List<Visual_Novel>>(File.ReadAllText(@"VisualNovels.json"));
            else
                return;

            foreach (Visual_Novel novel in VisualNovels) //Go through all Visual Novels you have read
            {                                            //and basically check if it should be added to listbox
                foreach (string cat in novel.Category)   //depending on your selected category and if VN already
                {                                        //exits on listbox
                    if (cbCategory.Items.Count == 0)
                    {
                        addVN = true;
                        break;
                    }
                    else if (cbCategory.SelectedIndex != -1)
                    {
                        if (cat.ToString().ToLower() == cbCategory.SelectedItem.ToString().ToLower())
                        {
                            if (lbVN.Items.Count == 0)
                                addVN = true;

                            foreach (string name in lbVN.Items)
                            {
                                if (name == novel.englishName)
                                    addVN = false;
                                else
                                    addVN = true;
                            }
                            break;
                        }
                        else
                            addVN = false;
                    }
                }
                if (addVN == true)
                {
                    lbVN.Items.Add(novel.englishName.ToString().Trim());
                }
                addVN = true;
            }

            foreach (Visual_Novel novel in VisualNovels) //Add all Visual Novels you have just read to global list which contains all VN's
                VNS.Add(novel);

            bool add_category = true;

            foreach (Visual_Novel novel in VNS) //Go through all Visual Novels and check if it is necessary to add
            {                                   //categories to the category combobox
                foreach (string addCategory in novel.Category)
                {
                    foreach (string existingCategory in cbCategory.Items)
                    {
                        if (addCategory.ToString().ToLower() == existingCategory.ToString().ToLower())
                            add_category = false;
                    }
                    if (add_category == true)
                    {
                        cbCategory.Items.Add(addCategory.ToString()); 
                        tsAddToCategory.DropDownItems.Add(addCategory.ToString(), null, add_to_category_Click); //Add categories to contextmenu
                        tsRemoveFromCategory.DropDownItems.Add(addCategory.ToString(), null, remove_from_category_Click);
                    }
                    add_category = true;
                }
            }

            int item_count = 0; 

            foreach (string novel in lbVN.Items) 
                item_count++;

            if (cbCategory.SelectedIndex != -1) //Read how many Visual Novels exists in listbox and display number in statusstrip
                lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: " + cbCategory.SelectedItem;
            else
                lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: All";
        }

        //When a VN has been selected show inforamtion in controls
        private void lbVN_SelectedIndexChanged(object sender, EventArgs e)
        {
            //When a VN has been selected
            //search through Visual Novel list for matching VN
            //and display information in controls

            image_number = 0; //Reset variables used for cycling through images 
            char_number = 0;
            lbTags.Items.Clear(); //Clear the tag listbox
            toolTip1.Active = false; //and deactivate tooltips used for images

            if (lbVN.SelectedItem != null) //Check if a VN has been selected
            {
                foreach (Visual_Novel novel in VNS) //Go through all VN's
                {
                    if (novel.englishName.Trim() == lbVN.SelectedItem.ToString()) //and if it matches with selected VN
                    {
                        string[] tags = novel.tags.Split(',');

                        for (int i = 0; i < tags.Length; i++)
                            tags[i] = tags[i].Trim(); //Read the tags from the VN

                        txtEnglishName.Text = novel.englishName; //and save the information in controls
                        txtOriginal.Text = novel.originalName;
                        lbTags.Items.AddRange(tags);
                        rtbDescriptionvn.Text = novel.description;
                        pbImage.ImageLocation = "https:" + novel.Images[0]; //Load VN's main image to screenshot picturebox
                        if (novel.Char_Images.Count > 0) //if there are no character images
                            pbCharacters.ImageLocation = novel.Char_Images[0];
                        else
                            pbCharacters.ImageLocation = "https:" + novel.Images[0]; //display VN's main image instead of first character image

                        bool enable_category = false;

                        foreach (ToolStripMenuItem item in tsRemoveFromCategory.DropDownItems) //Go through all categories
                        {
                            foreach (string category in novel.Category) //and decide which contextmenu controls should be enabled
                            {
                                if (category.ToString().ToLower() == item.Text.ToLower())
                                {
                                    enable_category = true;
                                    break;
                                }
                            }
                            if (enable_category == true)
                                item.Enabled = true;
                            else
                                item.Enabled = false;
                            enable_category = false;
                        }

                        return;
                    }
                }
            }
        }

        //A simple logical '&' tag sort
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //Compares how many tags have been matched
            //Sorts VN index accordingly
            //and sorts VN's according to their index in the listbox

            if (txtSearch.Text == "")
            {
                MessageBox.Show("You have to enter search terms first", "No searching without search terms!", MessageBoxButtons.OK);
                return;
            }

            foreach (Visual_Novel novel in VNS) //Reset tag hits and index
            {
                novel.sort_index = 0;
                novel.tag_hit = 0;
            }

            string[] search_tags = txtSearch.Text.Split(' ');
            string[] tmp;

            foreach (Visual_Novel novel in VNS) //Count how many tags have been matched
            {
                tmp = novel.tags.Split(',');

                foreach (string tag in search_tags)
                {
                    foreach (string novel_tag in tmp)
                    {
                        if (tag.Trim().Replace("_", " ").ToLower() == novel_tag.Trim().ToLower())
                            novel.tag_hit++;
                    }
                }
            }

            int i = 0;

            foreach (Visual_Novel novel in VNS) //increment VN's indexes
            {
                foreach (string category in novel.Category)
                {
                    if (cbCategory.SelectedIndex == -1)
                        cbCategory.SelectedIndex = 0;

                    if (category.ToString() == cbCategory.SelectedItem.ToString())
                    {
                        novel.sort_index = i;
                        i++;
                    }
                }
            }

            foreach (Visual_Novel out_novel in VNS) //Sort indexes according to tag hits
            {
                foreach (Visual_Novel in_novel in VNS)
                {
                    if (out_novel.tag_hit > in_novel.tag_hit)
                        if (in_novel.sort_index < out_novel.sort_index)
                        {
                            int index_tmp = out_novel.sort_index;
                            out_novel.sort_index = in_novel.sort_index;
                            in_novel.sort_index = index_tmp;
                        }
                }
            }

            lbVN.Items.Clear();

            foreach (Visual_Novel novel in VNS) //add all VN's to listbox
                foreach (string category in novel.Category)
                    if (category.ToString() == cbCategory.SelectedItem.ToString())
                    {
                        lbVN.Items.Add(novel.englishName);
                        break;
                    }

            foreach (Visual_Novel novel in VNS) //and sort them according to their indexes
                foreach (string category in novel.Category)
                    if (category.ToString() == cbCategory.SelectedItem.ToString())
                    {
                        lbVN.Items.RemoveAt(novel.sort_index);
                        lbVN.Items.Insert(novel.sort_index, novel.englishName);
                        break;
                    }
        }

        //By clicking on the Image_PictureBox you can cycle through all screenshots
        private void pbImage_Click(object sender, EventArgs e)
        {
            //Go through all images from vndb.org

            toolTip1.Active = true;

            foreach (Visual_Novel novel in VNS) //Cycle through screenshots; reset counter once cycle has been completed
                if ((novel.englishName == txtEnglishName.Text) && (image_number <= novel.Images.Count - 2))
                {
                    pbImage.ImageLocation = "https:" + novel.Images[image_number + 1];
                    image_number++;
                    toolTip1.Show("Picture " + (image_number + 1) + " of " + (novel.Images.Count), pbImage);
                    return;
                }
                else if ((novel.englishName == txtEnglishName.Text) && (image_number > novel.Images.Count - 2))
                {
                    image_number = -1;
                    pbImage.ImageLocation = "https:" + novel.Images[image_number + 1];
                    image_number++;
                    toolTip1.Show("Picture 1 of " + (novel.Images.Count), pbImage);
                    return;
                }
        }

        //By clicking on the Characters_PictureBox you can cycle through all character images
        private void pbCharacters_Click(object sender, EventArgs e)
        {
            //Go through all character images from vndb.org

            toolTip1.Active = true;

            foreach (Visual_Novel novel in VNS) //Cycle through character images; reset counter once cycle has been completed
                if ((novel.englishName == txtEnglishName.Text) && (char_number <= novel.Char_Images.Count - 2) && (novel.Char_Images.Count > 0))
                {
                    pbCharacters.ImageLocation = novel.Char_Images[char_number + 1];
                    char_number++;
                    toolTip1.Show("Picture " + (char_number + 1) + " of " + (novel.Char_Images.Count), pbCharacters);
                    return;
                }
                else if ((novel.englishName == txtEnglishName.Text) && (char_number > novel.Char_Images.Count - 2) && (novel.Char_Images.Count > 0))
                {
                    char_number = -1;
                    pbCharacters.ImageLocation = novel.Char_Images[char_number + 1];
                    char_number++;
                    toolTip1.Show("Picture 1 of " + (novel.Char_Images.Count), pbCharacters);
                    return;
                }
        }

        //Deletes Visual Novel
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbVN.SelectedIndex == -1) //Check if a VN has been selected
            {
                MessageBox.Show("You have to select an VN first", "No Deleting without Selecting!", MessageBoxButtons.OK);
                return;
            }

            int index = 0;

            foreach (Visual_Novel novel in VNS) //Go through VN to get the index of the selected VN
            {
                if (novel.englishName.Trim() == lbVN.SelectedItem.ToString().Trim())
                {
                    break;
                }
                index++;
            }
            lblStatus2.Text = VNS[index].englishName + " has been deleted (ID = " + VNS[index].id + ")"; //Update statusstrip
            VNS.RemoveAt(index); //Remove the VN from list
            lbVN.Items.Remove(lbVN.SelectedItem); //and from listbox

            if (lbVN.Items.Count == 0) //If no VN's are left in category delete it
                cbCategory.Items.Remove(cbCategory.SelectedItem);

            Update_VNS(); //Update "VisualNovels.json"
        }

        //Updates Visual Novel
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lbVN.SelectedIndex == -1) //Check if a Visual Novel has been selected
            {
                MessageBox.Show("You have to select an VN first", "No Updating without Selecting!", MessageBoxButtons.OK);
                return;
            }

            int vns_number = fetchInformation(true); //Fetch information from 'VNDB' for selected Visual Novel
                                                     //'true' because it is for updating a VN and not creating a new one
            if (vns_number == 0)
                return;

            lblStatus2.Text = VNS[vns_number].englishName + " has been updated (ID = " + VNS[vns_number].id + ")"; //Update statusstrip

            Update_VNS(); //Update "VisualNovels.json"
        }

        //Show tag description when hovering over tag
        private void lbTags_MouseMove(object sender, MouseEventArgs e)
        {
            int index = lbTags.IndexFromPoint(e.X, e.Y); //Get index of item the cursor is above

            if (index == -1) //If no index could be found abort
                return;

            foreach (WrittenTagsRootObject tag in plain_tags) //Go through all tags from the tagdump
            {
                if (lbTags.Items[index].ToString() == tag.name) //and show tooltip with tag description
                {
                    if (toolTip2.GetToolTip(lbTags) == tag.description) //If currently shown tooltip matches abort (otherwise the tooltip will blink all the time)
                        return;
                    toolTip2.RemoveAll();
                    toolTip2.ReshowDelay = 0;
                    toolTip2.Show(tag.description, lbTags);
                    return;
                }
            }
        }

        //Show tag description when selecting a tag
        private void lbTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (WrittenTagsRootObject tag in plain_tags) //Go through tagdump
            {
                if (lbTags.SelectedItem.ToString().Trim() == tag.name) //and show description in tooltip
                {
                    toolTip2.RemoveAll();
                    toolTip2.Show(tag.description, lbTags);
                }
            }
        }

        //Creates a new custom category
        private void btnCreateCategory_Click(object sender, EventArgs e)
        {
            if (txtCategory.Text == "") //If no category name has been entered abort
            {
                MessageBox.Show("Enter a category name first", "No category name", MessageBoxButtons.OK);
                txtCategory.Focus();
                return;
            }

            bool add_category = true;

            foreach (string existing_category in cbCategory.Items) //If category already exists abort
                if (existing_category.ToString().ToLower() == txtCategory.Text.ToLower())
                    add_category = false;

            if (add_category == true) //add category to combobox
                cbCategory.Items.Add(txtCategory.Text);

            add_category = true;

            foreach (ToolStripMenuItem item in tsAddToCategory.DropDownItems) //Go through 'add to category' menuitem
                if (item.ToString().ToLower() == txtCategory.Text.ToLower()) //if category already exists abort
                    add_category = false;

            if (add_category == true) //Create new menuitem for contextmenu using category name (for addtocategory menuitem)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = txtCategory.Text;
                item.Click += add_to_category_Click;
                tsAddToCategory.DropDownItems.Add(txtCategory.Text, null, add_to_category_Click);
            }

            add_category = true;

            foreach (ToolStripMenuItem item in tsRemoveFromCategory.DropDownItems)  //Go through 'remove from category' menuitem
                if (item.ToString().ToLower() == txtCategory.Text.ToLower())        //if category already exists abort                
                    add_category = false;

            if (add_category == true) //Create new menuitem for contextmenu using category name (for removefromcategory menuitem)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = txtCategory.Text;
                item.Click += remove_from_category_Click;
                tsRemoveFromCategory.DropDownItems.Add(txtCategory.Text, null, remove_from_category_Click);
            }
        }

        //Refresh listbox if another category has been selected
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbVN.Items.Clear(); //Clear listbox
            bool add_VN = false;

            foreach (Visual_Novel novel in VNS) //Go through all VN's
            {
                foreach (string cat in novel.Category) //Check which VN's match the category
                {
                    if ((lbVN.Items.Count == 0) && (cat.ToString().ToLower() == cbCategory.SelectedItem.ToString().ToLower()))
                        add_VN = true;

                    foreach (string item in lbVN.Items) //Check if VN has been already added
                    {
                        if ((cat.ToString().ToLower() == cbCategory.SelectedItem.ToString().ToLower()) && (novel.englishName != item.ToString()))
                            add_VN = true;
                    }
                }
                if (add_VN == true) //add VN if necessary
                { 
                    lbVN.Items.Add(novel.englishName);
                    add_VN = false;
                }
            }

            int item_count = 0;

            foreach (string VN in lbVN.Items) 
                item_count++;

            if (cbCategory.SelectedIndex != -1) //Count listbox items and display it in statusstrip
                lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: " + cbCategory.SelectedItem;
            else
                lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: All";
        }
  
        //Start VN
        private void tsStart_Click(object sender, EventArgs e)
        {
            foreach (Visual_Novel novel in VNS) //Search for matching VN
            {
                if (novel.englishName == lbVN.SelectedItem.ToString()) //and start it
                {
                    System.Diagnostics.Process.Start(novel.path);
                    lblStatus2.Text = novel.englishName + " has been started."; //update statusstrip
                    break;
                }
            }
        }

        //Resize form if screenshot is too large
        private void pbImage_Resize(object sender, EventArgs e)
        {
            groupBox5.Size = new Size(pbImage.Size.Width + 14, pbImage.Height + 45); //Resize groupbox containing the screenshot

            this.MinimumSize = new Size(0, 750); //Resize the form
            this.Size = new Size(groupBox5.Width + 965, groupBox5.Height + 100);
        }

        //Open Visual Novel on 'VNDB.org'
        private void viewOnVNDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Visual_Novel novel in VNS) //Search matching VN
                if (novel.englishName == lbVN.SelectedItem.ToString())
                {
                    System.Diagnostics.Process.Start("https://vndb.org/v" + novel.id); //and open it on 'VNDB.org'
                    lblStatus2.Text = novel.englishName + " has been opened on 'VNDB'.";
                    break;
                }
        }

        //Read "VisualNovels.json" when program is started
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("VisualNovels.json"))
                VNS = JsonConvert.DeserializeObject<List<Visual_Novel>>(File.ReadAllText(@"VisualNovels.json"));
            else
                return;
        }

        //Resize depending on which tab has been selected
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            TabControl clicked_Control = (TabControl)sender;

            if (clicked_Control.SelectedIndex == 1)
            {
                this.MinimumSize = new Size(0, 0);
                this.Size = new Size(1150, groupBox7.Height + 100);            
            }
            else if (clicked_Control.SelectedIndex == 2)
            {
                this.MinimumSize = new Size(0, 0);
                this.Size = new Size(565, 700);
            }
            else
            {
                this.MinimumSize = new Size(0, 750);
                this.Size = new Size(groupBox5.Width + 965, 750);
            }
        }

        //Add VN to selected category; Raised by contextmenu 'Add to Category'
        private void add_to_category_Click(object sender, EventArgs e)
        {
            bool add_category = true;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;

            foreach (Visual_Novel novel in VNS) //Go through VN's
            {
                if (novel.englishName == lbVN.SelectedItem.ToString()) //and add VN to category using magic...
                {
                    foreach (string category in novel.Category)
                        if (category.ToString().ToLower() == clickedItem.Text.ToLower())
                            add_category = false;
                }
                if ((add_category == true) && (novel.englishName == lbVN.SelectedItem.ToString()))
                {
                    novel.Category.Add(clickedItem.Text);

                    lblStatus2.Text = novel.englishName + " has been added to category: " + clickedItem.Text;

                    Update_VNS();

                    bool enable_category = false;

                    foreach (ToolStripMenuItem item in tsRemoveFromCategory.DropDownItems)
                    {
                        foreach (string category in novel.Category)
                        {
                            if (category.ToString().ToLower() == item.Text.ToLower())
                            {
                                enable_category = true;
                                break;
                            }
                        }
                        if (enable_category == true)
                            item.Enabled = true;
                        else
                            item.Enabled = false;
                        enable_category = false;
                    }

                    break;
                }
            }
        }

        //Remove VN from selected category; Raised by contextmenu 'Remove from Category'
        private void remove_from_category_Click(object sender, EventArgs e)
        {
            int index = 0;

            foreach (Visual_Novel novel in VNS) //Go through VN's 
            {
                if (novel.englishName == lbVN.SelectedItem.ToString()) //and remove selected VN from category using magic...
                {
                    ToolStripMenuItem clickeditem = (ToolStripMenuItem)sender;

                    if (clickeditem.Text == "All")
                    {
                        MessageBox.Show("Visual Novels can't be removed from category: 'All'", "Removal impossible", MessageBoxButtons.OK);
                        return;
                    }

                    foreach (string cat in novel.Category)
                    {
                        index++;
                        if (cat.ToLower() == clickeditem.Text.ToLower())
                            break;
                    }
                    lblStatus2.Text = novel.englishName + " has been removed from category: " + novel.Category[index - 1].ToString();

                    novel.Category.RemoveAt(index - 1);

                    bool delete_category = true;

                    foreach (Visual_Novel tmp in VNS)
                    {
                        foreach (string tmp2 in tmp.Category)
                        {
                            if (tmp2.ToLower() == clickeditem.Text.ToLower())
                                delete_category = false;
                        }
                    }

                    if (delete_category == true)
                    {
                        for (int i = 0; i < tsAddToCategory.DropDownItems.Count; i++)
                            if (tsAddToCategory.DropDownItems[i].Text.ToString().ToLower() == cbCategory.SelectedItem.ToString().ToLower())
                                tsAddToCategory.DropDownItems.RemoveAt(i);

                        for (int i = 0; i < tsRemoveFromCategory.DropDownItems.Count; i++)
                            if (tsRemoveFromCategory.DropDownItems[i].Text.ToString().ToLower() == cbCategory.SelectedItem.ToString().ToLower())
                                tsRemoveFromCategory.DropDownItems.RemoveAt(i);

                        cbCategory.Items.Remove(cbCategory.SelectedItem);
                    }

                    Update_VNS();

                    btnRefresh_Click(sender, e);

                    break;

                }
            }
        }

        //Fetches information from 'VNDB.org'
        private int fetchInformation(bool Update)
        {
            //This basically fetches information from vndb using the API
            //First it establishes a connection and logs in
            //using the entered LoginName or a random loginname

            //Then it request basic vn information
            //and deserializes the jsonresponse
            //After that it displays the reponse in 
            //the matching textboxes

            //Now it requets detailed information
            //deserializes it again....
            //and stores information in matching controls

            //Now it requests tags
            //deserializes them
            //and stores them in txtTags-Textbox
            //mind you: The tags are all in int format(only ID's)!

            //Now we read our tags-dump
            //deserialize them 
            //and change our int tags to the matching string tags

            //That's it.

            Connection conn = new Connection();
            string nice = " ";
            int id = 0, vns_number = 0;
            Random rnd = new Random();

            int error = 0;

            int login_id = rnd.Next(1000);

            conn.Open();

            if (txtLoginName.Text != "")
                error = conn.Login(txtLoginName.Text);
            else
                error = conn.Login(Convert.ToString("asd" + login_id));

            if (error == 1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.jsonresponse, "Login Error", MessageBoxButtons.OK);
                return 0;
            }

            if (Update == false)
                error = conn.Query("get vn basic (id = " + txtID.Text + " )"); //request basic information
            else
            {
                foreach (Visual_Novel novel in VNS)
                {
                    if (novel.englishName == lbVN.SelectedItem.ToString().Trim())
                    {
                        id = novel.id;
                        error = conn.Query("get vn basic (id = " + id + " )");
                        break;
                    }
                    vns_number++;
                }
            }

            if (error == 1)
            {
                MessageBox.Show("Error while requesting information. Response: " + conn.jsonresponse, "Query Error", MessageBoxButtons.OK);
                return 0;
            }

            BasicRootObject basic_information = Newtonsoft.Json.JsonConvert.DeserializeObject<BasicRootObject>(conn.jsonresponse); //deserialize it
            List<BasicItem> basic_item = basic_information.items;

            if (Update == false)
            {
                txtName.Text = basic_item[0].title;
                txtOriginalName.Text = basic_item[0].original;
                txtVNID.Text = Convert.ToString(basic_item[0].id);
                txtTags.Text = "";
            }
            else
            {
                VNS[vns_number].englishName = basic_item[0].title;
                VNS[vns_number].originalName = basic_item[0].original;
                VNS[vns_number].id = basic_item[0].id;
            }

            if (Update == false)
                error = conn.Query("get vn details (id = " + txtID.Text + " )");
            else
            {
                error = conn.Query("get vn details (id = " + id + " )");
            }

            if (error == 1)
            {
                MessageBox.Show("Error while requesting information. Response: " + conn.jsonresponse, "Query Error", MessageBoxButtons.OK);
                return 0;
            }

            DetailsRootObject detailed_information = Newtonsoft.Json.JsonConvert.DeserializeObject<DetailsRootObject>(conn.jsonresponse); //deserialize it
            List<DetailsItem> details_item = detailed_information.items;

            if (Update == false)
            {
                rtbDescription.Text = details_item[0].description;
                pcbImages.ImageLocation = details_item[0].image;
            }
            else
                VNS[vns_number].description = details_item[0].description;

            if (Update == false)
                error = conn.Query("get character details (vn = " + txtID.Text + " )");
            else
                error = conn.Query("get character details (vn = " + id + " )");

            if (error == 1)
            {
                MessageBox.Show("Error while requesting information. Response: " + conn.jsonresponse, "Query Error", MessageBoxButtons.OK);
                return 0;
            }

            CharacterRootObject character_information = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterRootObject>(conn.jsonresponse);
            List<CharacterItem> character_item = character_information.items;

            character_images = new List<string>();
            character_images.Clear();

            foreach (CharacterItem item in character_item)
            {
                if (item.image != null)
                    character_images.Add(item.image);
            }

            if (Update == false)
                error = conn.Query("get vn tags (id = " + txtID.Text + " )");
            else
                error = conn.Query("get vn tags (id = " + id + " )");

            if (error == 1)
            {
                MessageBox.Show("Error while requesting information. Response: " + conn.jsonresponse, "Query Error", MessageBoxButtons.OK);
                return 0;
            }

            TagsRootObject tags_information = Newtonsoft.Json.JsonConvert.DeserializeObject<TagsRootObject>(conn.jsonresponse); //deserialize them
            List<TagsItem> tags_item = tags_information.items;

            if (Update == false)
            {
                for (int i = 0; i < tags_item[0].tags.Count; i++) //Store them in textbox
                {
                    if (txtTags.Text == "")
                        nice = "";
                    else
                        nice = " ";

                    txtTags.Text = txtTags.Text + nice + Convert.ToString(tags_item[0].tags[i][0]);
                }
            }
            else
            {
                VNS[vns_number].tags = "";
                for (int i = 0; i < tags_item[0].tags.Count; i++) //Store them in textbox
                {
                    if (VNS[vns_number].tags == "")
                        nice = "";
                    else
                        nice = " ";

                    VNS[vns_number].tags = VNS[vns_number].tags + nice + Convert.ToString(tags_item[0].tags[i][0]);
                }
            }

            if (plain_tags == null) //Read tag-dump and deserialize it (only once -> needs time!)
                plain_tags = JsonConvert.DeserializeObject<List<WrittenTagsRootObject>>(File.ReadAllText(@"tags.json"));

            string[] tags;

            if (Update == false)
                tags = txtTags.Text.Split(' ');
            else
                tags = VNS[vns_number].tags.Split(' ');

            if (Update == false)
                txtTags.Text = "";
            else
                VNS[vns_number].tags = "";

            foreach (string tmp in tags) //Change our 'int'-tags into matching 'string'-tags
            {
                foreach (WrittenTagsRootObject tmp2 in plain_tags)
                {
                    if ((((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "ero") && (chkSexual.Checked == true)) ||
                        ((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "cont") && (chkContent.Checked == true)) ||
                        ((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "tech") && (chkTechnical.Checked == true))) && Update == false)
                    {
                        if (txtTags.Text == "")
                            nice = "";
                        else
                            nice = ", ";

                        txtTags.Text = txtTags.Text + nice + tmp2.name;
                    }
                    else if ((((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "ero") && (chkSexual.Checked == true)) ||
                        ((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "cont") && (chkContent.Checked == true)) ||
                        ((Convert.ToInt32(tmp) == tmp2.id) && (tmp2.cat == "tech") && (chkTechnical.Checked == true))) && Update == true)
                    {
                        if (VNS[vns_number].tags == "")
                            nice = "";
                        else
                            nice = ", ";

                        VNS[vns_number].tags = VNS[vns_number].tags + nice + tmp2.name;
                    }
                }
            }
            conn.CloseConnection();
            return vns_number;
        }

        //Update Visual Novels 
        private void Update_VNS()
        {
            File.Delete("VisualNovels.json"); //Delete old database

            FileStream aFile = new FileStream("VisualNovels.json", FileMode.OpenOrCreate); //Create it anew
            aFile.Dispose();

            File.WriteAllText(@"VisualNovels.json", JsonConvert.SerializeObject(VNS)); //And write Visual Novels into it
        }

    }
}

