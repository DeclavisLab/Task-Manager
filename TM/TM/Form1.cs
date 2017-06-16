﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Diagnostics;

namespace TM
{
    public partial class Form1 : Form
    {
        const string Version = "v2.1 Beta";
        bool git_init = false;
        public static Config conf = new Config();

        TicketList tickets = new TicketList();
        ListViewGroup lvg_todo = new ListViewGroup("Todo");
        ListViewGroup lvg_inprog = new ListViewGroup("In Progress");
        ListViewGroup lvg_closed = new ListViewGroup("Closed");
        public Form1()
        {
            SetVersion();
            InitializeComponent();
            if (File.Exists(Path.Combine(Application.StartupPath,"c.tm"))) { CLoading(); } else { CSave(); CLoading(); }
            StartupGit();

        }

        void SetVersion()
        {
            conf.Version = Version;
            tickets.Version = Version;
        }

        #region Git

        void StartupGit()
        {           
            updateToolStripMenuItem.Enabled = false;
            if (!conf.git_e) { return; }
            if (conf.gitpath != null || conf.gitpath != "") { git_init = true; }
            if (git_init) { this.Text += " - Git enabled"; updateToolStripMenuItem.Enabled = true; } 
        }

        void Git_Add_TM()
        {
            GitCommand("add .tm");
            GitCommand("commit -m \"Updated the .TM File [TM]\"");
        }

        void Git_Add_All(string n)
        {
            return;
            GitCommand("add --all");//?
            GitCommand("commit -m \"Resolved Task "+n+" [TM]\"");
        }

        void GitCommand(string com)
        {
            if(!git_init) { return; }
            ProcessStartInfo startInfo = new ProcessStartInfo(conf.gitpath);

            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Application.StartupPath;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = com;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            /*List<string> output = new List<string>();
            string lineVal = process.StandardOutput.ReadLine();

            while (lineVal != null)
            {

                output.Add(lineVal);
                lineVal = process.StandardOutput.ReadLine();

            }*/
            string msg = process.StandardOutput.ReadToEnd();
            if(msg != "" || msg != null)
            {
                MessageBox.Show(msg);
            }

            msg = process.StandardError.ReadToEnd();
            if (msg != "" || msg != null)
            {
                MessageBox.Show(msg);
            }
            //int val = output.Count();

            process.WaitForExit();
        }

        public void CSave()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, "c.tm"));
            sw.Write(ToCXML(conf));
            sw.Close();
        }
        void CLoading()
        {
            StreamReader sr = new StreamReader(File.OpenRead(Path.Combine(Application.StartupPath, "c.tm")));
            string Read = sr.ReadToEnd().Replace(Environment.NewLine, String.Empty); ;
            sr.Close();
            conf = FromCXML(Read);
        }

        public static Config FromCXML(string xml)
        {
            try
            {
                using (StringReader stringReader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Config));
                    return (Config)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception e) { return new Config(); }
        }
        public string ToCXML(Config obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }
        #endregion
        #region Board
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckData();            
        }

        public void CheckData()
        {
            if(File.Exists(Path.Combine(Application.StartupPath,".tm"))) {            
                Loading();                
            }
            InitBoard();
        }

       
        public void InitBoard()
        {           
            listView1.Groups.Add(lvg_todo);
            listView3.Groups.Add(lvg_inprog);
            listView2.Groups.Add(lvg_closed);
            //listView1.Alignment = ListViewAlignment.SnapToGrid;
            listView1.AllowDrop = true;
            listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            listView1.DragEnter += new DragEventHandler(listView_DragEnter);
            listView1.ItemDrag += new ItemDragEventHandler(lvi1ItemDrag);
            listView2.AllowDrop = true;
            listView2.DragDrop += new DragEventHandler(listView2_DragDrop);
            listView2.DragEnter += new DragEventHandler(listView_DragEnter);
            listView2.ItemDrag += new ItemDragEventHandler(lvi2ItemDrag);
            listView3.AllowDrop = true;
            listView3.DragDrop += new DragEventHandler(listView3_DragDrop);
            listView3.DragEnter += new DragEventHandler(listView_DragEnter);
            listView3.ItemDrag += new ItemDragEventHandler(lvi3ItemDrag);
            RefreshBoard();
        }
        void RefreshBoard()
        {
            Save();
            lvg_todo.Items.Clear();
            lvg_inprog.Items.Clear();
            lvg_closed.Items.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();

            foreach (Ticket t in tickets.Items)
            {
                if (t.mystate == state.Todo)
                    listView1.Items.Add(new ListViewItem(t.name, "311.ico", lvg_todo));
                if (t.mystate == state.InProgress)
                    listView3.Items.Add(new ListViewItem(t.name, "311.ico", lvg_inprog));
                if (t.mystate == state.Closed)
                    listView2.Items.Add(new ListViewItem(t.name, "311.ico", lvg_closed));

            }
            if (lvg_todo.Items.Count == 0)
                {
                    // add empty list view item
                    ListViewItem lvi = new ListViewItem(string.Empty);
                    lvi.Group = lvg_todo;
                    listView1.Items.Add(lvi);
                }
            
            if (lvg_closed.Items.Count == 0)
            {
                // add empty list view item
                ListViewItem lvi = new ListViewItem(string.Empty);
                lvi.Group = lvg_closed;
                listView2.Items.Add(lvi);
            }
            if (lvg_inprog.Items.Count == 0)
            {
                // add empty list view item
                ListViewItem lvi = new ListViewItem(string.Empty);
                lvi.Group = lvg_inprog;
                listView3.Items.Add(lvi);
            }

        }
        void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                //if (item.Tag is Ticket)
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            item.ListView.Items.Remove(item);
            foreach (Ticket t in tickets.Items)
            {
                if (t.name == item.Text)
                {
                    t.mystate = state.Todo;
                    t.changenow();
                    RefreshBoard();
                    break;
                }
            }
        }        

        void listView2_DragDrop(object sender, DragEventArgs e)
        {
            var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            item.ListView.Items.Remove(item);
            foreach(Ticket t in tickets.Items)
            {
                if(t.name == item.Text)
                {
                    t.mystate = state.Closed;
                    t.changenow();
                    RefreshBoard();
                    Git_Add_All(t.name);
                    break;
                }
            }            
        }

        void listView3_DragDrop(object sender, DragEventArgs e)
        {
            var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            item.ListView.Items.Remove(item);
            foreach (Ticket t in tickets.Items)
            {
                if (t.name == item.Text)
                {
                    t.mystate = state.InProgress;
                    t.changenow();
                    RefreshBoard();
                    break;
                }
            }
        }

        private void lvi1ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.DoDragDrop(listView1.SelectedItems[0], DragDropEffects.Move);
            updateToolStripMenuItem.Text = "*Git Update";
        }
        private void lvi2ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.DoDragDrop(listView2.SelectedItems[0], DragDropEffects.Move);
            updateToolStripMenuItem.Text = "*Git Update";
        }
        private void lvi3ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.DoDragDrop(listView3.SelectedItems[0], DragDropEffects.Move);
            updateToolStripMenuItem.Text = "*Git Update";
        }      

        public void Save()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, ".tm"));
            sw.Write(ToXML(tickets));
            sw.Close();                       
        }
        void Loading()
        {
            StreamReader sr = new StreamReader(File.OpenRead(Path.Combine(Application.StartupPath, ".tm")));
            string Read = sr.ReadToEnd().Replace(Environment.NewLine, String.Empty); ;
            sr.Close();
            tickets = FromXML(Read);
        }

        public static TicketList FromXML(string xml)
        {
            try
            {
                using (StringReader stringReader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TicketList));
                    return (TicketList)serializer.Deserialize(stringReader);
                }
            }catch(Exception e) { return new TicketList(); }
        }
        public string ToXML(TicketList obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TicketList));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Debuging
            RefreshBoard();

        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            new frm_edit().ChangeTicket(listView1.SelectedItems[0].Text, tickets);
            RefreshBoard();

        }

        private void listView2_ItemActivate(object sender, EventArgs e)
        {
            new frm_edit().ChangeTicket(listView2.SelectedItems[0].Text, tickets);
            RefreshBoard();

        }

        private void listView3_ItemActivate(object sender, EventArgs e)
        {
            new frm_edit().ChangeTicket(listView3.SelectedItems[0].Text, tickets);
            RefreshBoard();

        }

        #endregion
        #region Toolbar
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new frm_config().ShowDialog() == DialogResult.OK)
            {
                CSave();
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ticket newt = new frm_add().NewTicket();
            if (newt != null)
            {
                tickets.Items.Add(newt);
                RefreshBoard();
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Git_Add_TM();
        }
        #endregion
    }
}
