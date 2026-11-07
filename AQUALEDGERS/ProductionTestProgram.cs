
using System;
using System.Windows.Forms;

namespace AQUALEDGERS
{
    internal static class ProductionTestProgram
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ProductionForm());
        }
    }
}
