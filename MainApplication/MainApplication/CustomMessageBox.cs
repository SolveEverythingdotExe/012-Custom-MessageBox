using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApplication
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        //first we will create a static method to be able to call the custom message box
        //the method/function should return dialogresult
        public static DialogResult Show(String Text)
        { 
            //create an instance
            CustomMessageBox messageBox = new CustomMessageBox();

            //lets set the text
            messageBox.lblText.Text = Text;

            //lets show as a dialog
            messageBox.ShowDialog();

            return messageBox.DialogResult;
        }
        //It's done, now lets apply on our form and test

        //lets set the value that will return once the buttons have clicked
        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
