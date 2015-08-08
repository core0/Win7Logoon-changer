using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Win7LogonChanger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //апи для блокировки раб. стола
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        //путь к изменяемому изобр.
        static string _pathFileLogOnChange = @"C:\windows\system32\oobe\info\backgrounds\backgroundDefault.jpg";

        //путь к стандартному изобр.
        string _pathFileLogOnNormal = @"C:\Windows\System32\oobe\background.bmp";


        //переменная в которой будет содеражаться значение реестра
        //о том используется ли пользовательский LogOn
        public static int CheckKeyInt;

        //полчение информации о пользовательском LogOn
        FileInfo EndFile = new FileInfo(_pathFileLogOnChange);

        //функция которая определяем устанавливать ли выбраный LogOn
        //и показывать ли изображение в PictureBox 
        public void ShowImageForPictureBox1(bool stations,bool showmessage)
        {
            try
            {
                //и отображаем выбранный LogOn
                if (stations)
                {
                    //отображаем его в PictureBox

                    pictureBox1.ImageLocation = _pathFileLogOnChange;

                    //сообщение об изменении
                    if (showmessage)
                        MessageBox.Show("Logon успешно изменен", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else //отображение стандартного LogOn
                {
                    pictureBox1.ImageLocation = _pathFileLogOnNormal;

                    if (showmessage)
                    	MessageBox.Show("Установле исходный Logon", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessException(ex);
            }
        }

        public void ShowMessException(Exception ex)
        {
            MessageBox.Show(ex.Message + "\r\nПерезапуститье программу", "эксепшен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //используется ли дефолтный logon
        public void getVall_OEMBack()
        {
            try
            {
                //читаем ключ в реестре (установлен ли пользовательский LogOn)
                RegistryKey CheckKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background");
                CheckKeyInt = (int)CheckKey.GetValue("OEMBackground");

            }
            catch (System.Exception ex)
            {
                //если ключа OEMBackground нет
                RegistryKey CreateKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background");
                CreateKey.SetValue("OEMBackground", 0);
                CheckKeyInt = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //устанавливаем фильтр для openFileDialog1
            openFileDialog1.Filter = "JPG files (*.jpg)|*.jpg";

            getVall_OEMBack();

            //проверяем переменную
            if (CheckKeyInt == 1)
            {
                try
                {
                    //создаем директории для пользовательского logon
                    Directory.CreateDirectory(@"C:\windows\system32\oobe\info\");
                    Directory.CreateDirectory(@"C:\windows\system32\oobe\info\backgrounds\");

                    //отображаем пользовательский LogOn и не показываем MessageBox 
                    ShowImageForPictureBox1(true,false);
                }
                catch (System.Exception ex)
                {
                    ShowMessException(ex);
                    ShowImageForPictureBox1(false, false);
                }
            }
            else//если CheckKeyInt == 0
            {
                try
                {
                    //создаем директории для пользов. LogOn
                    Directory.CreateDirectory(@"C:\windows\system32\oobe\info\");
                    Directory.CreateDirectory(@"C:\windows\system32\oobe\info\backgrounds\");

                    //отображаем стандартн. LogOn и не показываем message
                    ShowImageForPictureBox1(false,false);
                }
                catch (System.Exception ex)
                {
                    ShowMessException(ex);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //открываем файл через openFileDialog1
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //проверяем его вес
                FileInfo CheckSizeImg = new FileInfo(openFileDialog1.FileName);
                long normSize = 256000;

                //если не больше 250 кб
                if (CheckSizeImg.Length <= normSize)
                {
                    try
                    {
                        //объявл. переменную, она будет содержать путь к файлу
                        String pathIMG = openFileDialog1.FileName;
                        openFileDialog1.Dispose();

                        //проверяем если ли уже пользовательский LogOn в дериктории
                        //C:\windows\system32\oobe\info\backgrounds\
                        FileInfo pathIMGexists = new FileInfo(pathIMG);

                        //копируем наш выбраный файл в дерикторию польозовательского LogOn
                        File.Copy(pathIMG, _pathFileLogOnChange,true);

                        //отображаем выбранный LogOn и отображаем Message об изменении
                        ShowImageForPictureBox1(true,true);

                    }
                    catch (Exception ex)//в случае ошибке, показываем Текст ошибки и отображаем стандартный LogOn
                    {
                        MessageBox.Show("Ошибка: " + ex.Message + "\r\nПерезапуститье программу", "Ой эксепшен :(", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ShowImageForPictureBox1(false, false);
                    }

                }
                else
                {
                    //если файл больше 250 кб
                    MessageBox.Show("Размер изображения не должен превышать 250 Kбайт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    //проверяем значение в реестре
	                RegistryKey CheckKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background");
	                CheckKeyInt = (int)CheckKey.GetValue("OEMBackground");

                    //если 0 тогда изменяем на 1(отображать пользовательский Logon)
                    if (CheckKeyInt == 0)
                    {
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background", "OEMBackground", 1);
                    }

                }
                catch (System.Exception ex)
                {
                    ShowMessException(ex);
                }



            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //тестируем установленный Logon
            LockWorkStation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //кнопка вернуть стандартный LogOn
            try
            {
                //изменяем знач в реестре на 0 и отображаем станд. LogOn и показываем Message 
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background", "OEMBackground", 0);
                ShowImageForPictureBox1(false,true);
            }
            catch (System.Exception ex)
            {
                ShowMessException(ex);
            }
        }

        //показываем форму О программе
        private void button4_Click(object sender, EventArgs e)
        {
            Form Form2 = new Form2();
            Form2.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void GoToBlogDeveloper_Click(object sender, EventArgs e)
        {
            //посетить блог разработчика
            System.Diagnostics.Process.Start("");
        }
    }
}