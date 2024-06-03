using GIIS21.SqlEngine;
using POS20.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS20
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {           
            
            InitializeComponent();
        
            //Product product = new Product();
            //product.ID = 22;
            //product.Model = "Sasha1";
            //product.Stones = new List<Stone>();

            Product product = new Product();
            product.ID = 1;
            product.Model = "Sasha2";
            product.Stones = new List<Stone>();

            Stone stone1 = new Stone();
            stone1.Type = 1;
            Stone stone2 = new Stone();
            stone2.Type = 2;
            product.Stones.Add(stone1);
            product.Stones.Add(stone2);

            product.Save();
        }
    }
}
