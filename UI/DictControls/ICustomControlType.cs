using System;
using System.Data;
using System.Windows.Forms;
using Base;

namespace UI
{
    interface ICustomControlType
    {
        string ControlTypeName { get; }
        string DisplayName { get; }
        string Name { get; }
        Control CreateRealControl(string Name);
        void SetControlChangedEventHandler(Control control, EventHandler handler);
        void SetDataBinding(Control control, COMField dataSource, string fieldName);
    }
}
