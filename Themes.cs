using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectRegionForDbd
{
    public static class Themes
    {
        public static void Dark(Form form, Button create, Button remove, Button export, Button choose,TextBox textBox, ComboBox comboBox, ComboBox comboBox1)
        {
            // Стандартный тёмный фон
            form.BackColor = Color.FromArgb(45, 45, 48);
            // Цвет Для Button
            form.ForeColor = Color.LightGray;
            create.BackColor = Color.FromArgb(63, 63, 70);
            remove.BackColor = Color.FromArgb(63, 63, 70);
            export.BackColor = Color.FromArgb(63, 63, 70);
            choose.BackColor = Color.FromArgb(63, 63, 70);
            // Для TextBox
            textBox.BackColor = Color.FromArgb(37, 37, 38);
            textBox.ForeColor = Color.White;
            // Для ComboBox
            comboBox.BackColor = Color.FromArgb(37, 37, 38);
            comboBox.ForeColor = Color.White;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox1.BackColor = Color.FromArgb(37, 37, 38);
            comboBox1.ForeColor = Color.White;
            comboBox1.FlatStyle = FlatStyle.Flat;
        }
    }
}
