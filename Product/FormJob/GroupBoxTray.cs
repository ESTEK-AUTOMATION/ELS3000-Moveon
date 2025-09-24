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
    public partial class GroupBoxTray : UserControl
    {
        public GroupBoxTray()
        {
            InitializeComponent();
        }

        public void UpdateInputTrayImage(string InputTray)
        {
            if (InputTray == "Jedec Tray")
            {
                //load Jedec Tray Selected Image
                pictureBoxJedecTray1.Image = null;
                pictureBoxJedecTray1.Image = Properties.Resources.JedecTraySelected;
                pictureBoxJedecTray1.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray1.SizeMode = PictureBoxSizeMode.StretchImage;
                //Refresh Soft Tray Image
                pictureBoxSoftTray1.Image = null;
                pictureBoxSoftTray1.Image = Properties.Resources.SoftTray;
                pictureBoxSoftTray1.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray1.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            else if (InputTray == "Soft Tray")
            {
                //Refresh Jedec Tray Image
                pictureBoxJedecTray1.Image = null;
                pictureBoxJedecTray1.Image = Properties.Resources.JedecTray;
                pictureBoxJedecTray1.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray1.SizeMode = PictureBoxSizeMode.StretchImage;
                //load Soft Tray Selected Image
                pictureBoxSoftTray1.Image = null;
                pictureBoxSoftTray1.Image = Properties.Resources.SoftTraySelected;
                pictureBoxSoftTray1.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray1.SizeMode = PictureBoxSizeMode.StretchImage;
            }


        }

        public void UpdateOutputTrayImage(string OutputTray)
        {
            if (OutputTray == "Jedec Tray")
            {
                //load Jedec tray Image Selected
                pictureBoxJedecTray2.Image = null;
                pictureBoxJedecTray2.Image = Properties.Resources.JedecTraySelected;
                pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Refresh Soft tray image
                pictureBoxSoftTray2.Image = null;
                pictureBoxSoftTray2.Image = Properties.Resources.SoftTray;
                pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Refresh special carrier image
                pictureBoxSpecialCarrier.Image = null;
                pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrier;
                pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;


            }
            else if (OutputTray == "Soft Tray")
            {
                //Refresh Jedec tray Image
                pictureBoxJedecTray2.Image = null;
                pictureBoxJedecTray2.Image = Properties.Resources.JedecTray;
                pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Load Soft tray selected image
                pictureBoxSoftTray2.Image = null;
                pictureBoxSoftTray2.Image = Properties.Resources.SoftTraySelected;
                pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Refresh special carrier Image
                pictureBoxSpecialCarrier.Image = null;
                pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrier;
                pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            else if (OutputTray == "Special Carrier")
            {
                //Refresh Jedec tray Image
                pictureBoxJedecTray2.Image = null;
                pictureBoxJedecTray2.Image = Properties.Resources.JedecTray;
                pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Refresh Soft tray image
                pictureBoxSoftTray2.Image = null;
                pictureBoxSoftTray2.Image = Properties.Resources.SoftTray;
                pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //load special carrier selected Image
                pictureBoxSpecialCarrier.Image = null;
                pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrierSelected;
                pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
            }


        }

        public void updateSortingTray(string OutputTray, string SortingTray)
        {
            if (OutputTray == "Jedec Tray")
            {
                //load Jedec tray Image Selected
                pictureBoxJedecTray2.Image = null;
                pictureBoxJedecTray2.Image = Properties.Resources.JedecTraySelected;
                pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                //Sorting Table Selection
                if (SortingTray == "Soft Tray")
                {
                    //Load Soft tray sorting image
                    pictureBoxSoftTray2.Image = null;
                    pictureBoxSoftTray2.Image = Properties.Resources.SoftTraySorting;
                    pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                    pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                    //Refresh special carrier Image
                    pictureBoxSpecialCarrier.Image = null;
                    pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrier;
                    pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                    pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (SortingTray == "Special Carrier")
                {
                    //Refresh Soft tray image
                    pictureBoxSoftTray2.Image = null;
                    pictureBoxSoftTray2.Image = Properties.Resources.SoftTray;
                    pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                    pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                    //load special carrier selected Image
                    pictureBoxSpecialCarrier.Image = null;
                    pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrierSorting;
                    pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                    pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else if (OutputTray == "Soft Tray")
            {
                pictureBoxSoftTray2.Image = null;
                pictureBoxSoftTray2.Image = Properties.Resources.SoftTraySelected;
                pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;

                //Sorting Table Selection
                if (SortingTray == "Jedec Tray")
                {
                    //load Jedec tray Image Selected
                    pictureBoxJedecTray2.Image = null;
                    pictureBoxJedecTray2.Image = Properties.Resources.JedecTraySorting;
                    pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                    pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;

                    //Refresh special carrier image
                    pictureBoxSpecialCarrier.Image = null;
                    pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrier;
                    pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                    pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
                }

                else if (SortingTray == "Special Carrier")
                {
                    //Refresh Jedec tray Image
                    pictureBoxJedecTray2.Image = null;
                    pictureBoxJedecTray2.Image = Properties.Resources.JedecTray;
                    pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                    pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;

                    //load special carrier selected Image
                    pictureBoxSpecialCarrier.Image = null;
                    pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrierSorting;
                    pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                    pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else if (OutputTray == "Special Carrier")
            {
                //load special carrier selected Image
                pictureBoxSpecialCarrier.Image = null;
                pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrierSelected;
                pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
                //Sorting Table Selection
                if (SortingTray == "Jedec Tray")
                {
                    //load Jedec tray Image Selected
                    pictureBoxJedecTray2.Image = null;
                    pictureBoxJedecTray2.Image = Properties.Resources.JedecTraySorting;
                    pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                    pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                    //Refresh Soft tray image
                    pictureBoxSoftTray2.Image = null;
                    pictureBoxSoftTray2.Image = Properties.Resources.SoftTray;
                    pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                    pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (SortingTray == "Soft Tray")
                {
                    //Refresh Jedec tray Image
                    pictureBoxJedecTray2.Image = null;
                    pictureBoxJedecTray2.Image = Properties.Resources.JedecTray;
                    pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                    pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                    //Load Soft tray selected image
                    pictureBoxSoftTray2.Image = null;
                    pictureBoxSoftTray2.Image = Properties.Resources.SoftTraySorting;
                    pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                    pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
        }

        public void UpdateRejectTrayImage(string OutputTray)
        {
            if (OutputTray == "Jedec Tray")
            {
                //load Jedec tray Image Selected
                pictureBoxJedecTray2.Image = null;
                pictureBoxJedecTray2.Image = Properties.Resources.JedecTrayRejected;
                pictureBoxJedecTray2.Size = new System.Drawing.Size(160, 320);
                pictureBoxJedecTray2.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            else if (OutputTray == "Soft Tray")
            {

                //Load Soft tray selected image
                pictureBoxSoftTray2.Image = null;
                pictureBoxSoftTray2.Image = Properties.Resources.SoftTrayRejected;
                pictureBoxSoftTray2.Size = new System.Drawing.Size(272, 240);
                pictureBoxSoftTray2.SizeMode = PictureBoxSizeMode.StretchImage;


            }
            else if (OutputTray == "Special Carrier")
            {

                //load special carrier selected Image
                pictureBoxSpecialCarrier.Image = null;
                pictureBoxSpecialCarrier.Image = Properties.Resources.SpecialCarrierRejected;
                pictureBoxSpecialCarrier.Size = new System.Drawing.Size(191, 256);
                pictureBoxSpecialCarrier.SizeMode = PictureBoxSizeMode.StretchImage;
            }


        }
    }
}
