using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Win7LogonChanger
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //если пользователь закрываем форму то не выходим из программы а только прячем форму О программе
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //при нажатии на кнопку написать разработчику
            //должна открываться программа E-mail агент с моим E-mail адерсом
            //но если это не происходит тогда выдаем сообщение об этом и указываем 
            //E-mail адрес что бы можно было написать электронное письмо
            try
            {
                //открываем E-mail клиент
            	System.Diagnostics.Process.Start("mailto:wizmax@yandex.ru");
            }
            catch (System.Exception ex)
            {
                //вот собственно само сообщение
                MessageBox.Show(ex.Message + "\r\n" + "Moжете связаться с разработчиком по адресу wizmax@yandex.ru", "Не сопоставлено ни одного приложения для работы с E-mail",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
