using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public partial class CustomerFormMainInterface : Product.ProductFormMainInterface
    {
        public CustomerFormMainInterface()
        {
            base.Load += new System.EventHandler(Initialize);
        }

        void Initialize(object sender, EventArgs e)
        {
            
        }
    }

    
}
