namespace Isabella.App.Infrastrucuture
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Isabella.App.ViewModels;

    /// <summary>
    /// InstanceLocator
    /// </summary>
    public class InstanceLocator
    {
        public MainViewModel Main { get; set; }

        public InstanceLocator()
        {
            Main = new MainViewModel();
        }   
    }
}
