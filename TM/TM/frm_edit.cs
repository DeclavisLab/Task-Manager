using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TM
{
    public partial class frm_edit : Form
    {
        string name;
        string assign;
        string des;
        state oldstate;
        DateTime cr;


        public frm_edit()
        {
            InitializeComponent();
        }

        public DialogResult Open(string _name, string _ass, string _des, DateTime _cr)
        {
            //base.ShowDialog();
            txt_name.Text = _name;
            txt_assign.Text = _ass;
            txt_des.Text = _des;
            cr = _cr;
            return base.ShowDialog();
        }

        public void ChangeTicket(string nam, TicketList list)
        {
            Ticket a = null;
            foreach (Ticket t in list.Items)
            {
                if(t.name == nam)
                {
                    a = t;
                    oldstate = a.mystate;
                    break;
                }
            }
            if (a != null)
            {
                DialogResult dr = Open(a.name, a.assign, a.description, a.created);
                if  (dr == DialogResult.OK)
                {
                    list.Items.Remove(a);
                    list.Items.Add(new Ticket(name, des, oldstate, assign, cr));
                }
                if(dr == DialogResult.No)
                {
                    list.Items.Remove(a);
                }
            }
                    
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool CheckFields()
        {
            if (txt_name.Text == "")
            {
                return false;
            }
            return true;
        }

        private void btn_change_Click(object sender, EventArgs e)
        {
            if (!CheckFields())
                MessageBox.Show("Not all required fileds are filled!");
            else
            {                
                name = txt_name.Text;
                assign = txt_assign.Text;
                des = txt_des.Text;               
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;           
            this.Close();
        }       
    }
}
