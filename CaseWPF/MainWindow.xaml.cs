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
using System.IO;

namespace CaseWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
        public partial class MainWindow : Window {
        public struct LiteItem {
            public string name;
            public int money;
            public LiteItem(string name, int money) { this.name = name; this.money = money; }
        }
        public class Item {
            public string name;
            public BitmapImage img;
            public BitmapImage imgActive;
            public int money;
            public Item(string name, BitmapImage img, BitmapImage imgActive, int money) {
                this.name = name;
                this.img = img;
                this.imgActive = imgActive;
                this.money = money;
            }
        }
        public class Case : Item { // 1 - 45, 2 - 30, 3 - 20, 4 - 5: предмет - шанс выпадения
            public Item[] items = new Item[4];
            public int procent;
            public Case(string name, BitmapImage img, BitmapImage imgActive, int money, int procent, Item I0, Item I1, Item I2, Item I3) : base(name, img, imgActive, money) {
                items[0] = I0;
                items[1] = I1;
                items[2] = I2;
                items[3] = I3;
                this.procent = procent;
            }
        }
        public class Inventory
        {
            private Item[] items = new Item[25];
            private Image[] slots = new Image[25];
            public int active = -1;

            public Inventory(Image[] img)
            {
                slots = img;
                for (int i = 0; i < 25; i++)
                {
                    items[i] = null;
                    slots[i].Source = LoadBitmap("Items/none.png");
                }
            }

            public string GetItemName(int index)
            {
                return items[index] != null ? items[index].name : "none";
            }

            public void AddItem(Item item)
            {
                int slot = FindFreeSlot();
                items[slot] = item;
                slots[slot].Source = item.img;
            }

            public void DeleteItem(int index)
            {
                items[index] = null;
                slots[index].Source = LoadBitmap("Items/none.png");
                Sort();
            }

            public void SetActiveItem(int index)
            {
                if (active != -1)
                    slots[active].Source = items[active] != null ? items[active].img : LoadBitmap("Items/none.png");
                slots[index].Source = items[index] != null ? items[index].imgActive : LoadBitmap("Items/noneA.png");
                active = index;
            }

            public LiteItem GetActiveItem() { return items[active] != null ? new LiteItem(items[active].name, items[active].money) : new LiteItem("None", 0); }

            private int FindFreeSlot()
            {
                int result = -1;
                for (int i = 0; i < 25; i++)
                    if ((object)items[i] == (object)null){ result = i; break; }
                return result;
            }

            private void Sort()
            {
                Item[] sortedItems = new Item[25];
                List<Item> list = new List<Item>();
                foreach (Item i in items) if ((object)i != (object)null) list.Add(i);
                for (int i = 0; i < list.Count; i++) sortedItems[i] = list[i];
                items = sortedItems;
                for (int i = 0; i < 25; i++) slots[i].Source = i == active ? (items[i] != null ? items[i].imgActive : LoadBitmap("Items/noneA.png")) : (items[i] != null ? items[i].img : LoadBitmap("Items/none.png"));
            }
        }

        public Chat chat = new Chat();
        bool win = false;
        string code;
        string codePlayer = "*****";
        int codePos = 0;
        public Inventory inventory;
        public List<Case> cases = new List<Case>();
        public int money = 100;
        public int activeShop = -1;

        public MainWindow() {
            InitializeComponent();

            chat.ShowInTaskbar = false;
            chat.Show();
            cases.Add(new Case("Деревянный ящик", LoadBitmap("Items/1.png"), LoadBitmap("Items/1A.png"), 10, 1,
                new Item("Вилка", LoadBitmap("Items/11.png"), LoadBitmap("Items/11A.png"), 8),
                new Item("Половник", LoadBitmap("Items/12.png"), LoadBitmap("Items/12A.png"), 12),
                new Item("Нож", LoadBitmap("Items/13.png"), LoadBitmap("Items/13A.png"), 15),
                new Item("Топор", LoadBitmap("Items/14.png"), LoadBitmap("Items/14A.png"), 20)
                ));
            
            cases.Add(new Case("Каменная урна", LoadBitmap("Items/2.png"), LoadBitmap("Items/2A.png"), 50, 3,
                new Item("Свиток", LoadBitmap("Items/21.png"), LoadBitmap("Items/21A.png"), 40),
                new Item("Книга", LoadBitmap("Items/22.png"), LoadBitmap("Items/22A.png"), 60),
                new Item("Кольцо", LoadBitmap("Items/23.png"), LoadBitmap("Items/23A.png"), 75),
                new Item("Аметист", LoadBitmap("Items/24.png"), LoadBitmap("Items/24A.png"), 100)
                ));
            
            cases.Add(new Case("Железный контейнер", LoadBitmap("Items/3.png"), LoadBitmap("Items/3A.png"), 200, 5,
                new Item("Металолом", LoadBitmap("Items/31.png"), LoadBitmap("Items/31A.png"), 150),
                new Item("Банка", LoadBitmap("Items/32.png"), LoadBitmap("Items/32A.png"), 225),
                new Item("Шестерня", LoadBitmap("Items/33.png"), LoadBitmap("Items/33A.png"), 300),
                new Item("Радиоприёмник", LoadBitmap("Items/34.png"), LoadBitmap("Items/34A.png"), 400)
                ));

            cases.Add(new Case("Золотой сундук", LoadBitmap("Items/4.png"), LoadBitmap("Items/4A.png"), 750, 8,
                new Item("Браслет", LoadBitmap("Items/41.png"), LoadBitmap("Items/41A.png"), 600),
                new Item("Кристалл", LoadBitmap("Items/42.png"), LoadBitmap("Items/42A.png"), 800),
                new Item("Брилиант", LoadBitmap("Items/43.png"), LoadBitmap("Items/43A.png"), 1100),
                new Item("Золото", LoadBitmap("Items/44.png"), LoadBitmap("Items/44A.png"), 1500)
                ));

            cases.Add(new Case("Преобразователь материи", LoadBitmap("Items/5.png"), LoadBitmap("Items/5A.png"), 3000, 10,
                new Item("Нефть", LoadBitmap("Items/51.png"), LoadBitmap("Items/51A.png"), 2750),
                new Item("Плутоний", LoadBitmap("Items/52.png"), LoadBitmap("Items/52A.png"), 3500),
                new Item("Тритий", LoadBitmap("Items/53.png"), LoadBitmap("Items/53A.png"), 4500),
                new Item("Антиматерия", LoadBitmap("Items/54.png"), LoadBitmap("Items/54A.png"), 6000)
                ));

            for (int i = 0; i < 5; i++) ((Image)FindName("Ca" + i)).Source = cases[i].img;
            /*
             * Деревянный ящик - 10
             *      Вилка - 8
             *      Половник - 12
             *      Нож - 15
             *      Топор - 20
             * Каменная урна - 50
             *      Свиток - 40
             *      Книга - 60
             *      Кольцо - 75
             *      Аметист - 100
             * Железный контейнер - 200
             *      Металолом - 150
             *      Банка - 225
             *      Шестерня - 300
             *      Радиоприёмник - 400
             * Золотой сундук - 750
             *      Браслет - 600
             *      Кристалл - 800
             *      Брилиант - 1100
             *      Золото - 1500
             * Преобразователь материи - 3000
             *      Нефть - 2750
             *      Плутоний - 3500
             *      Тритий - 4500
             *      Антиматерия - 6000
             */

            Image[] img = new Image[25];
            for (int i = 0; i < 25; i++) img[i] = (Image)FindName("Im" + i);
            inventory = new Inventory(img);
            Main_Panel.Visibility = Visibility.Hidden;
        }
        //
        // START PANEL
        //
        private void NewGame_Button_Click(object sender, RoutedEventArgs e) {
            if (File.Exists("OCsave"))
            {
                MessageBoxResult result = MessageBox.Show("Вы точно хотите начать игру заного? Все старые сохранения удаляться", "Осторожно!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete("OCsave");
                    NewGame();
                }
            }
            else NewGame();
        }

        private void DownloadGame_Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("OCsave"))
            {
                string[] file = File.ReadAllLines("OCsave");
                List<Item> list = new List<Item>();
                for (int i = 0; i < cases.Count; i++)
                {
                    list.Add(cases[i]);
                    for (int j = 0; j < cases[i].items.Length; j++)
                        list.Add(cases[i].items[j]);
                }
                for (int i = 0; i < 25; i++)
                    for (int j = 0; j < list.Count; j++)
                        if (file[i].Equals(list[j].name)) inventory.AddItem(list[j]);
                money = Convert.ToInt32(file[25]);
                Money_Label.Content = money;
                codePlayer = file[26];
                Code_Label.Content = codePlayer;
                code = file[27];
                codePos = Convert.ToInt32(file[28]);
                win = file[29].Equals("0") ? false : true;
                Main_Panel.Visibility = Visibility.Visible;
                this.Height = 245;
            }
            else MessageBox.Show("Файл сохранения не найден", "Ошибка!", MessageBoxButton.OK);
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void NewGame()
        {
            code = GenerateCode();
            List<string> file = new List<string>();
            for (int i = 0; i < 25; i++) file.Add("none"); // slots
            file.Add("100");                               // money
            file.Add("*****");
            file.Add(code);
            file.Add(Convert.ToString(codePos));
            file.Add("0");
            File.WriteAllLines("OCsave", file);
            Main_Panel.Visibility = Visibility.Visible;
            this.Height = 245;
        }

        private string GenerateCode()
        { 
            Random rnd = new Random();
            string[] abc = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "g", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string result = "";
            for (int i = 0; i < 5; i++) result += abc[rnd.Next(0, 26)];
            return result;
        }

        public static BitmapImage LoadBitmap(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new BitmapImage(new Uri(fs.Name));
        }

        private void PictureBox_Click(object sender, MouseButtonEventArgs e)
        {
            inventory.SetActiveItem(Convert.ToInt32(((Image)sender).Name.Split(new Char[1] { Convert.ToChar("m") }, StringSplitOptions.None)[1]));
            Name_Label.Content = inventory.GetActiveItem().name;
            Sell_Button.Content = inventory.GetActiveItem().money != 0 ? "Продать за " + inventory.GetActiveItem().money : "Продать";
            Sell_Button.IsEnabled = inventory.GetActiveItem().money != 0 ? true : false;
            bool a = false;
            foreach (Case c in cases) if (c.name.Equals(inventory.GetActiveItem().name)) a = true;
            Open_Button.IsEnabled = a;
        }

        private void ShopBox_Click(object sender, MouseButtonEventArgs e)
        {
            int index = Convert.ToInt32(((Image)sender).Name.Substring(2, 1));
            if (activeShop != -1)
                ((Image)FindName("Ca" + activeShop)).Source = cases[activeShop].img;
            ((Image)FindName("Ca" + index)).Source = cases[index].imgActive;
            activeShop = index;
            Cost_Label.Content = cases[index].money;
            CaseName_Label.Content = cases[index].name;
            Buy_Button.IsEnabled = money >= cases[index].money ? true : false;
        }

        private void Buy_Button_Click(object sender, RoutedEventArgs e)
        {
            inventory.AddItem(cases[activeShop]);
            money -= cases[activeShop].money;
            Money_Label.Content = money;
            Buy_Button.IsEnabled = money >= cases[activeShop].money ? true : false;
        }

        private void Open_Button_Click(object sender, RoutedEventArgs e)
        {
            string str1 = "";
            string str2 = "";
            string str3 = "";

            Random rnd = new Random();
            Case cas = null;
            Item item = null;
            Case bonus = null;
            string newCode = "";
            int index = -1;
            for (int i = 0; i < 5; i++) if (cases[i].name.Equals(inventory.GetActiveItem().name)) { cas = cases[i]; index = i; }

            int randItem = rnd.Next(0, 100);
            int randBonus = rnd.Next(0, 5);
            int randCode = rnd.Next(0, 100);

            item = randItem >= 0 && randItem < 45 ? cas.items[0] : randItem >= 45 && randItem < 75 ? cas.items[1] : randItem >= 75 && randItem < 95 ? cas.items[2] : cas.items[3];
            bonus = !cas.Equals(cases[0]) ? (randBonus == 0 ? cases[index - 1] : null) : null;
            newCode = randCode < cas.procent && codePos < 5 ? Convert.ToString(code[codePos]) : "";

            inventory.DeleteItem(inventory.active);
            inventory.AddItem(item);
            str1 = "Открыв '" + cas.name + "' вы получили: " + item.name + ". ";
            if (bonus != null) { inventory.AddItem(bonus); str2 = "Так же там вы нашли '" + bonus.name + "'. "; }
            if (!newCode.Equals("") && win == false)
            {
                str3 = "И узнали следующий символ кода: " + newCode;
                Char[] chr = codePlayer.ToCharArray();
                chr[codePos] = Convert.ToChar(newCode);
                codePos++;
                codePlayer = new string(chr);
                Code_Label.Content = codePlayer;
            }

            Money_Label.Content = money;
            Name_Label.Content = inventory.GetActiveItem().name;
            Sell_Button.Content = inventory.GetActiveItem().money != 0 ? "Продать за " + inventory.GetActiveItem().money : "Продать";
            Sell_Button.IsEnabled = inventory.GetActiveItem().money != 0 ? true : false;
            bool a = false;
            foreach (Case c in cases) if (c.name.Equals(inventory.GetActiveItem().name)) a = true;
            Open_Button.IsEnabled = a;

            chat.textBox.Text = str1 + str2 + str3 + Environment.NewLine + chat.textBox.Text;
        }

        private void Sell_Button_Click(object sender, RoutedEventArgs e)
        {
            money += inventory.GetActiveItem().money;
            inventory.DeleteItem(inventory.active);
            if (activeShop != -1)
                Buy_Button.IsEnabled = money >= cases[activeShop].money ? true : false;
            Money_Label.Content = money;
            Name_Label.Content = inventory.GetActiveItem().name;
            Sell_Button.Content = inventory.GetActiveItem().money != 0 ? "Продать за " + inventory.GetActiveItem().money : "Продать";
            Sell_Button.IsEnabled = inventory.GetActiveItem().money != 0 ? true : false;
            bool a = false;
            foreach (Case c in cases) if (c.name.Equals(inventory.GetActiveItem().name)) a = true;
            Open_Button.IsEnabled = a;
        }

        private void Code_Button_Click(object sender, RoutedEventArgs e)
        {
            if (win == false) {
                if (code.Equals(Code_Box.Text))
                {
                    win = true;
                    MessageBoxResult result = MessageBox.Show("Введённый код верный! Вы выйграли! Хотите играть дальше? Если вы нажмёте 'Нет', то игра сохраниться", "Поздравляем!", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No) { Save_Button_Click(new object(), new RoutedEventArgs()); Application.Current.Shutdown(); }
                }
            }
            else MessageBox.Show("Ты уже выйграл. Можешь начать игру заного или продолжать играть без кода", "Ошибка");
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> file = new List<string>();
            for (int i = 0; i < 25; i++) file.Add(inventory.GetItemName(i));
            file.Add(Convert.ToString(money));
            file.Add(codePlayer);
            file.Add(code);
            file.Add(Convert.ToString(codePos));
            file.Add(win ? "1" : "0");
            File.Delete("OCsave");
            File.WriteAllLines("OCsave", file.ToArray());
        }
    }
}
