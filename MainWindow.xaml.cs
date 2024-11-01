using GIIS21.SqlEngine;
using POS20.Attributes;
using POS20.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static Int32 CurrentUserID = 32;

        public MainWindow()
        {   
            InitializeComponent();
            /*Product product = new Product();            
            product.Model = "Roma";
            product.Stones = new List<Stone>();
            Stone stone1 = new Stone();
            stone1.ID = 1;
            stone1.Type = 4;
            Stone stone2 = new Stone();
            stone2.ID = 2;
            stone2.Type = 3;
            product.Stones.Add(stone1);
            product.Stones.Add(stone2);            
            product.Save();*/

            UserFilter userFilter = new UserFilter();
            userFilter.CollectFilter();

            /*User user = new User();
            user.SelectFilter(user);

            Type dataType = user.GetType();
            var properties = dataType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                FilterInt32 exportParameter = property.GetCustomAttribute<FilterInt32>();
                if (exportParameter != null)
                {
                    Int32 min = exportParameter.Min;
                    Int32 max = exportParameter.Max;                    
                }
            }*/

            //Int32 userCount = user.SelectSummary(user);   

            /*User users = new User();            
            List<User> userList = users.Select(users);

            Random random = new Random(); 
            foreach (User user in userList)
            {
                user.RoleID = random.Next(2) + 1;
                user.Inn = random.Next(99999999).ToString("D8");
                user.Save();
            }*/

            /*Role role = new Role();
            role.ID = 0;
            role.Name = $"admin";
            role.Description = $"Администратор";
            role.Save();

            role.ID = 0;
            role.Name = $"director";
            role.Description = $"Директор";
            role.Save();

            role.ID = 0;
            role.Name = $"mainAdmin";
            role.Description = $"Старший администратор";
            role.Save();

            role.ID = 0;
            role.Name = $"cassir";
            role.Description = $"Кассир";
            role.Save();*/
        }
    }
}
