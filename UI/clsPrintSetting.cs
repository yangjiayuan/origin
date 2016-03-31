using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;

namespace UI
{
    public class clsPrintSetting
    {
        public string PrinterName;
        public short Copies;
        public bool Landscape;
        public System.Drawing.Printing.Margins Margins;
        public System.Drawing.Printing.PaperSize PaperSize;
        public bool IsOverPrint;
        public string PageRange;
        public bool Duplex;

        public System.Drawing.Printing.PrinterSettings GetPrinterSettins()
        {
            PrinterSettings ps = new PrinterSettings();
            ps.Copies = this.Copies;
            ps.PrinterName = this.PrinterName;
            ps.DefaultPageSettings.Landscape = this.Landscape;
            ps.DefaultPageSettings.Margins = this.Margins;
            ps.DefaultPageSettings.PaperSize = this.PaperSize;

            return ps;
        }
    }
}
