
/**************************************************************************
*   
*   =================================
*   CLR 版本    ：4.0.30319.42000
*   命名空间    ：IceInk
*   文件名称    ：EkWinFormHelper.cs
*   =================================
*   创 建 者    ：IceInk
*   创建日期    ：2020/7/14 10:10:53 
*   功能描述    ：
*           WinForm 的一些封装 帮助类
*                 TODO 尚不完整，以后接着添加
*   使用说明    ：
*          扩展类 方法 直接调用
*   =================================
*  
***************************************************************************/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace IceInk.WinForm
{
    /// <summary>
    /// //基于WinForm窗体的帮助类
    /// </summary>
    public static class EkWinFormHelper
    {

        /// <summary>
        /// 判断窗体是否打开
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <returns></returns>
        public static bool CheckIsOpenForm<T>() where T : Form
        {
            Type wormType = typeof(T);

            return Application.OpenForms.Cast<Form>().Any(frm => frm.Name.Equals(wormType.Name));
        }

        /// <summary>
        /// 打开窗体
        /// 如果窗体已经打开则返回窗体,
        /// 否则打开一个新窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <returns></returns>
        public static T OpenForm<T>()
            where T : Form, new()
        {
            Type wormType = typeof(T);

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name.Equals(wormType.Name))
                {
                    return frm as T;
                }
            }

            return new T();
        }

        #region  管理(加载中...或者 请稍后.... )等类型的窗口

        private static EkLoadingWinForm mLoadingWinForm;
        private static readonly object mSyncLock = new object();  //加锁使用
        private static Thread mLoadingThread;
        public static void ShowLoadingForm(Form parentForm, string tipsMsg = null, Color msgColor = default)
        {
            //TODO 遇到个问题：本来用线程打开Loading窗口，但是遇到一些问题 没有解决
            //mLoadingThread = new Thread(() =>
            //    { OpenLoadingForm(parentForm, tipsMsg); })
            //{
            //    IsBackground = true
            //};
            //mLoadingThread.SetApartmentState(ApartmentState.STA);
            //mLoadingThread.Start();

            parentForm.BeginInvoke(new MethodInvoker(() =>
            {
                OpenLoadingForm(parentForm, tipsMsg, msgColor);
            }));


        }
        /// <summary>
        /// 关闭(加载中...) 窗口
        /// </summary>
        public static void HideLoadingForm(Form parentForm)
        {
            //Thread.Sleep(50); //可能到这里线程还未起来，所以进行延时，可以确保线程起来，彻底关闭窗口
            //if (mLoadingWinForm != null)
            //{
            //    lock (mSyncLock)
            //    {
            //        Thread.Sleep(50);
            //        if (mLoadingWinForm != null)
            //        {
            //            Thread.Sleep(50); //通过三次延时，确保可以彻底关闭窗口
            //            CloseLoadingForm();

            //            //mLoadingWinForm.Invoke(new mCloseDelegate(CloseLoadingForm));
            //        }

            //    }
            //}
            parentForm.BeginInvoke(new MethodInvoker(() =>
            {
                CloseLoadingForm(parentForm);
            }));

        }


        private static void OpenLoadingForm(Form parentForm, string tipsMsg = null, Color color = default)
        {
            if (mLoadingWinForm == null)
                mLoadingWinForm = new EkLoadingWinForm();

            parentForm.Enabled = false;//设置底窗口不能点击

            mLoadingWinForm.TopLevel = true;
            mLoadingWinForm.ShowTips(tipsMsg, color);
            mLoadingWinForm.TopMost = true;
            mLoadingWinForm.DesktopLocation = parentForm.DesktopLocation;
            mLoadingWinForm.ShowDialog();
        }


        private static void CloseLoadingForm(Form parentForm)
        {
            mLoadingWinForm.Hide();

            mLoadingWinForm = null;
            parentForm.Enabled = true;
            mLoadingThread?.Abort();
        }

        #endregion
    }


    /// <summary>
    /// winForm中 textBox控件的帮助扩展类
    /// </summary>
    public static class EkWinFormTextBoxHelper
    {
        /// <summary>
        /// 只能输入正整数
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        public static void EkOnlyEnterPositiveInt(this TextBox textBox, KeyPressEventArgs e)
        {
            //如果输入的不是退格和数字，则屏蔽输入
            if (!(e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 只能输整数
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        public static void EkOnlyEnterInt(this TextBox textBox, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-')
            {//如果输入负号 负号只能输入在第一位
                if (textBox.TextLength > 0 || textBox.Text.Contains("-"))
                    e.Handled = true;
            }
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            else if (!(e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 只能输入正数
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        public static void EkOnlyEnterPositiveNumber(this TextBox textBox, KeyPressEventArgs e)
        {
            //判断按键是不是要输入的类型。
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if (e.KeyChar == 46) //小数点
            {//小数点不能在第一位 
                if (textBox.TextLength == 0 || textBox.Text.Contains("."))
                    e.Handled = true;
            }
        }

    }


    /// <summary>
    /// WinForm中 DataGridView控件帮助类
    /// </summary>
    public static class EkWinFormDataGridViewHelper
    {
        /// <summary>
        /// DataGridView自动添加行编号
        /// </summary>
        /// <param name="dataGridView">DataGridView</param>
        /// <param name="cellNumIndex">编号列下标</param>
        public static void EkAutoRowNumber(this DataGridView dataGridView, int cellNumIndex = 1)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                int index = i + 1;
                dataGridView.Rows[i].Cells[cellNumIndex].Value = index;
            }
        }

        /// <summary>
        /// 获取所有选择DataGridView的数据
        /// (选择是根据 DataGridViewCheckBoxCell 是否打勾)
        /// </summary>
        /// <typeparam name="T">选择的数据类型</typeparam>
        /// <param name="dataGridView"></param>
        /// <param name="checkBoxCellValue">CheckBoxCell所在列的名称</param>
        /// <returns></returns>
        public static List<T> EkSelectDataList<T>(this DataGridView dataGridView, string checkBoxCellValue)
        {
            int allCount = dataGridView.RowCount;
            if (allCount < 1)
                return default;
            List<T> readProductList = new List<T>();
            for (int i = 0; i < allCount; i++)
            {
                DataGridViewRow rowCell = dataGridView.Rows[i];
                DataGridViewCheckBoxCell cell = rowCell.Cells[checkBoxCellValue] as DataGridViewCheckBoxCell;
                if (cell?.FormattedValue == null ||
                    (!cell.FormattedValue.ToString().ToLower().Equals("true")))
                    continue;
                if (rowCell.DataBoundItem is T readProduct)
                    readProductList.Add(readProduct);
            }

            return readProductList;
        }
        /// <summary>
        /// 设置 DataGridView 中(CheckBox)只能单选
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="currentRowIndex">当前选中的行</param>
        /// <param name="checkBoxColIndex">checkbox 所在的列</param>
        public static void EkSingleCheckBox(this DataGridView dataGridView, int currentRowIndex, int checkBoxColIndex = 0)
        {
            int rowCount = dataGridView.RowCount;
            if (rowCount < 1)
                return;
            for (int i = 0; i < rowCount; i++)
            {
                dataGridView.Rows[i].Cells[checkBoxColIndex].Value = (i == currentRowIndex);
            }
        }
        /// <summary>
        /// 反向选择 DataGridView 中(CheckBox)
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="checkBoxColIndex">checkbox 所在的列</param>
        public static void EkReverseCheckBox(this DataGridView dataGridView, int checkBoxColIndex = 0)
        {
            int rowCount = dataGridView.RowCount;
            if (rowCount < 1)
                return;
            for (int i = 0; i < rowCount; i++)
            {
                object value = dataGridView.Rows[i].Cells[checkBoxColIndex].Value;
                bool b = false;
                if (value != null) b = (bool)value;
                dataGridView.Rows[i].Cells[checkBoxColIndex].Value = !b;
            }
        }
        /// <summary>
        /// 选中全部 DataGridView 中(CheckBox)
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="checkBoxColIndex">checkbox 所在的列</param>
        public static void EkCheckAllCheckBox(this DataGridView dataGridView, int checkBoxColIndex = 0)
        {
            int rowCount = dataGridView.RowCount;
            if (rowCount < 1)
                return;
            for (int i = 0; i < rowCount; i++)
            {
                dataGridView.Rows[i].Cells[checkBoxColIndex].Value = true;
            }
        }
    }

}
