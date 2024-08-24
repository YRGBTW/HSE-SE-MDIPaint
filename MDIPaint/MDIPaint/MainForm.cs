using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class MainForm : Form
    {

        public static DocumentForm documentForm;
    
        public static Color Color { get; set; }
        public static string Tool { get; set; }
        public static int Width { get; set; }

        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();
        public MainForm()
        {
            InitializeComponent();
            FindPlugins();
            CreatePluginsMenu();
            Color = Color.Black;
            Width = 5;
            Кисть.Text = $"{Width}";
            Tool = "Pen";
        }

        void FindPlugins()
        {
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                       
                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("IPlugin");

                        if (iface != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            if (!plugins.ContainsKey(plugin.Name))
                            {
                                plugins.Add(plugin.Name, plugin);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                        MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
        }

        private void CreatePluginsMenu()
        {
            foreach (var p in plugins)
            {
                var item = фильтрToolStripMenuItem.DropDownItems.Add(p.Value.Name);
                item.Click += OnPluginClick;
            }
        }

        private void OnPluginClick(object sender, EventArgs args)
        {
            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            plugin.Transform((Bitmap) documentForm.bitmap);
            documentForm.Refresh();
        }



        private void файлToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            сохранитьКакToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }

        private void другойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            if(colorDialog.ShowDialog() == DialogResult.OK) 
            {
                Color = colorDialog.Color;
            }
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentForm = new DocumentForm(this);
            documentForm.MdiParent = this;
            documentForm.Show();
        }

        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new CanvasSizeForm((DocumentForm)ActiveMdiChild); 
            frm.MdiParent = this;
            frm.Show();
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Red;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Blue;
        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Green;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm activeForm = (DocumentForm)ActiveMdiChild;
            if (activeForm.Filename != null && activeForm.isOpenedFile == false)
            {
                activeForm.bitmap.Save(activeForm.Filename);
            }
            else
            {
                сохранитьКакToolStripMenuItem_Click(sender,e);
            }
        }
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                
                var frm = new DocumentForm(this,new Bitmap(dlg.FileName));
                frm.isOpenedFile = true;
                frm.Filename = dlg.FileName;
                frm.MdiParent = this;
                frm.Show();
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            ImageFormat[] ff = { ImageFormat.Bmp, ImageFormat.Jpeg };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                documentForm.Filename = dlg.FileName;
                var frm = (DocumentForm)ActiveMdiChild;
                frm.GetBitmap().Save(dlg.FileName, ff[dlg.FilterIndex - 1]);
            }
        }
        public void SaveBeforeClosing(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem_Click(sender, e);
        }
        private void рисунокToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            размерХолстаToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void сверхуВнизToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void Кисть_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int size;
                if (int.TryParse(Кисть.Text, out size ))
                {
                    if (size > 0)
                        Width = size;
                }
                Кисть.Text = $"{Width}";
            }
        }

        private void линияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tool = "Line";
        }

        private void ластикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tool = "Eraser";
        }

        private void элипсToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tool = "Elypse";
        }

        private void звездаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StarCreation frm = new StarCreation();
            frm.MdiParent = this;
            frm.Show();
            if (DocumentForm.starEndCount > 0)
            {
                Tool = "Star";
            }

        }

        private void пероToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tool = "Pen";
        }

        private void ScaleUp_Click(object sender, EventArgs e)
        {
            if(ActiveMdiChild!= null)
            {
                DocumentForm.ScaleUp((DocumentForm)ActiveMdiChild);
            }
        }

        private void ScaleDown_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                DocumentForm.ScaleDown((DocumentForm)ActiveMdiChild);
            }
        }

        private void менеджерПлагиновToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindPlugins();
            PluginInfo frm = new PluginInfo(plugins);
            frm.MdiParent = this;
            frm.FormClosed += (s, args) => RefreshPluginsMenu(frm.plugins);
            frm.Show();
        }

        public void RefreshPluginsMenu(object plugins)
        {
            this.plugins = (Dictionary<string, IPlugin>)plugins;

            var itemsToRemove = new List<ToolStripItem>();
            foreach (ToolStripItem item in фильтрToolStripMenuItem.DropDownItems)
            {
                if (!item.ToString().Contains("Менеджер плагинов"))
                {
                    itemsToRemove.Add(item);
                }
            }
            foreach (ToolStripItem item in itemsToRemove)
            {
                фильтрToolStripMenuItem.DropDownItems.Remove(item);
            }

            CreatePluginsMenu();
        }
    }   
}
