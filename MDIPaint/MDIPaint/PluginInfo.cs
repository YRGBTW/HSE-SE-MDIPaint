using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class PluginInfo : Form
    {
        public Dictionary<string, IPlugin> plugins;
        public PluginInfo()
        {
            InitializeComponent();
        }
        public PluginInfo(Dictionary<string, IPlugin> plugins)
        {
            InitializeComponent();
            this.plugins = plugins;
            foreach (var p in plugins)
            {
                clbPlugins.Items.Add(p.Value.Name);
            }
        }

        public Dictionary<string, IPlugin> GetSelectedPlugins()
        {
            Dictionary<string,IPlugin> selectedPlugins = new Dictionary<string, IPlugin>();
            foreach (int index in clbPlugins.CheckedIndices)
            {
                string pluginName = clbPlugins.Items[index].ToString();
                if (plugins.ContainsKey(pluginName))
                {
                    selectedPlugins.Add(pluginName, plugins[pluginName]);
                }   
            }
            return selectedPlugins;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            plugins = GetSelectedPlugins();
            Close();
        }

        private void clbPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbPlugins.SelectedItem != null)
            {
                string pluginName = clbPlugins.SelectedItem.ToString();
                IPlugin plugin = plugins[pluginName];

                VersionAttribute version = GetPluginVersion(plugin.GetType());

                label2.Text = version != null
                    ? $"Версия: {version.Major}.{version.Minor}"
                    : "Версия: Неизвестно";

                label3.Text = $"Автор: {plugin.Author}";
            }
        }

        private VersionAttribute GetPluginVersion(Type pluginType)
        {
            return (VersionAttribute)pluginType.GetCustomAttributes(typeof(VersionAttribute), false).FirstOrDefault();
        }
    }
}

