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
    public partial class frm_add : Form
    {
        string name;
        string assign;
        string des;


        public frm_add()
        {
            InitializeComponent();
        }

        public Ticket NewTicket()
        {
            if(base.ShowDialog()==DialogResult.OK)
            {
                return new Ticket(name, des, state.Todo, assign, DateTime.Now);
            }
            else { return null; }            
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool CheckFields()
        {
            if(txt_name.Text == "")
            {
                return false;
            }
            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if(!CheckFields())
                MessageBox.Show("Not all required fileds are filled!");
            else
            {
                DialogResult = DialogResult.OK;
                name = txt_name.Text;
                assign = txt_assign.Text;
                des = txt_des.Text;
                this.Close();
            }
        }
    }
}
