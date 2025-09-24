using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class SortingSubPage : UserControl
    {
        public string Name = "";
        public int StartNo = 0;
        public int EndNo = 0;
        public int SelectType = 0;
        public string RangeStart;
        public string RangeEnd;
        public SortingSubPage()
        {
            InitializeComponent();
        }
        public void updateInterface()
        {
            labelIndex.Text = Name;
            labelStartNo.Text = StartNo.ToString();
            labelEndNo.Text = EndNo.ToString();
            if (SelectType == 1)
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                textBox1.Text = RangeStart;
            }
            else if (SelectType == 2)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                textBox3.Text = RangeStart;
                textBox2.Text = RangeEnd;
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxFix.Visible = true;
            groupBoxRange.Visible = false;
            SelectType = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxFix.Visible = false;
            groupBoxRange.Visible = true;
            SelectType = 2;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RangeStart = textBox1.Text;
            RangeEnd = "";
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            RangeStart = textBox3.Text;
            //RangeEnd = "";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            RangeEnd = textBox2.Text;
        }
    }
}
