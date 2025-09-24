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
    public partial class groupboxRecipeControl : UserControl
    {
        public groupboxRecipeControl()
        {
            InitializeComponent();
        }

        private void richTextBoxMessageRecipeMain_TextChanged(object sender, EventArgs e)
        {
            richTextBoxMessageRecipeMain.SelectionStart = richTextBoxMessageRecipeMain.TextLength;
            richTextBoxMessageRecipeMain.ScrollToCaret();
        }
    }
}
