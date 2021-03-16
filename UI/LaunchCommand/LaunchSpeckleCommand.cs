using Speckle.DesktopUI;
using System.Collections.Generic;
using System.Linq;
using TopSolid.Cad.Design.DB.Documents;
using TopSolid.Kernel.DB.D3.Planes;
using TopSolid.Kernel.DB.Parameters;
using TopSolid.Kernel.G.D3;
using TopSolid.Kernel.GR.Displays;
using TopSolid.Kernel.TX.Units;
using TopSolid.Kernel.UI.Commands;
using TopSolid.Kernel.WX;

namespace EPFL.SpeckleTopSolid.UI.LaunchCommand
{
    class LaunchSpeckleCommand : MenuCommand
    {
        
        protected override void Invoke()
        {
            //Show a message box to make sure the component is working 
            MessageBox.Show("BOOM");

            //Command as in other Speckle connectors, for the moement it does nothing
            //Bootstrapper BootstrapperTopSolid = new Bootstrapper()




            StartOrShowPanel();

        }

        //Just a simple copy/paste from ConnectorRhino
        public static Bootstrapper Bootstrapper { get; set; }
        internal void StartOrShowPanel()
        {
            if (Bootstrapper != null)
            {
                Bootstrapper.Application.MainWindow.Show();
                return;
            }

            Bootstrapper = new Bootstrapper()
            {
                Bindings = new ConnectorBindingsTopSolid()
            };

            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application();
            }

            Bootstrapper.Setup(System.Windows.Application.Current);
            Bootstrapper.Start(new string[] { });

            Bootstrapper.Application.MainWindow.Initialized += (o, e) =>
            {
                ((ConnectorBindingsTopSolid)Bootstrapper.Bindings).GetFileContextAndNotifyUI();
            };

            Bootstrapper.Application.MainWindow.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                Bootstrapper.Application.MainWindow.Hide();
                e.Cancel = true;
            };

            //    //var helper = new System.Windows.Interop.WindowInteropHelper(Bootstrapper.Application.MainWindow);
            //    //helper.Owner = RhinoApp.MainWindowHandle();
            //}
        }
}
