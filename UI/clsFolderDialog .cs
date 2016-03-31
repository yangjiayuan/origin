using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace UI
{
    public class clsFolderDialog : FolderNameEditor
    {
        FolderNameEditor.FolderBrowser fDialog = new
        System.Windows.Forms.Design.FolderNameEditor.FolderBrowser();
        public clsFolderDialog()
        {
        }
        public DialogResult DisplayDialog()
        {
            return DisplayDialog("��ѡ��һ���ļ���");
        }

        public DialogResult DisplayDialog(string description)
        {
            fDialog.Description = description;
            return fDialog.ShowDialog();
        }
        public string Path
        {
            get
            {
                return fDialog.DirectoryPath;
            }
        }
        ~clsFolderDialog()
        {
            fDialog.Dispose();
        }
    }
}
