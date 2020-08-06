using System;
using System.Drawing;
using System.Windows.Forms;

namespace IceInk.WinForm
{
    public partial class EkLoadingWinForm : Form
    {
        public EkLoadingWinForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }

        public void ShowTips(string tipsMsg, Color tipsMsgColor)
        {
            label1.AutoSize = true;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Text = tipsMsg;
            label1.ForeColor =tipsMsgColor;
        }
        private void EkLoadingWinForm_Shown(object sender, System.EventArgs e)
        {

        }
    }
}
