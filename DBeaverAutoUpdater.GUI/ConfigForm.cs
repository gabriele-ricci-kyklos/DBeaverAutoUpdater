using DBeaverAutoUpdater.Core.BE;
using DBeaverAutoUpdater.Core.BLL;
using GenericCore.Support;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DBeaverAutoUpdater.GUI
{
    public partial class ConfigForm : Form
    {
        private IConfigBLL configBLL = new ConfigBLL();
        private IUpdateBLL updateBLL = new UpdateBLL();

        private CommonOpenFileDialog _installPathDialog =
            new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(_installPathDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                installPathTextBox.Text = _installPathDialog.FileName;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool hasErrors = false;

            if(installPathTextBox.Text.IsNullOrBlankString())
            {
                hasErrors = true;
                errorProvider.SetError(installPathTextBox, "The value cannot be empty");
            }

            if(architectureComboBox.SelectedItem.IsNull())
            {
                hasErrors = true;
                errorProvider.SetError(architectureComboBox, "The value cannot be empty");
            }

            if(hasErrors)
            {
                return;
            }

            errorProvider.Clear();

            configBLL
                .SaveConfiguration
                (
                    new ConfigurationItem
                    {
                        DBeaverInstallPath = installPathTextBox.Text.TrimStart().TrimEnd(),
                        Architecture = architectureComboBox.SelectedItem.ToString().ToEnum(Architecture.X86)
                    }
                );
        }
    }
}
